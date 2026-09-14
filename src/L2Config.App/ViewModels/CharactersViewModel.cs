using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using L2Config.App.Infrastructure;
using L2Config.Core.Characters;
using L2Config.Core.Storage;

namespace L2Config.App.ViewModels;

/// <summary>The Characters tab: player characters in the running world, with their inventory adena.</summary>
public sealed class CharactersViewModel : ObservableObject
{
	private readonly Func<L2Locations?> _locations;
	private readonly Func<long> _maxAdena;
	private List<CharacterRowViewModel> _all = [];
	private string _searchText = "";
	private string? _problem;
	private bool _isLoading;

	public CharactersViewModel(Func<L2Locations?> locations, Func<long> maxAdena)
	{
		_locations = locations;
		_maxAdena = maxAdena;
		RefreshCommand = new RelayCommand(() => _ = RefreshAsync(), () => !IsLoading);
	}

	public string Title => "Characters";

	public string Intro =>
		"Player characters in your running world. Adena can be changed only while the character is logged out — the server " +
		"keeps a logged-in character's inventory in memory and would overwrite the change. Changes are written to the world's " +
		"database straight away and noted in character-edits.log beside the backups.";

	public ObservableCollection<CharacterRowViewModel> Rows { get; } = [];
	public RelayCommand RefreshCommand { get; }

	public string SearchText
	{
		get => _searchText;
		set
		{
			if (Set(ref _searchText, value ?? ""))
			{
				ApplyFilter();
			}
		}
	}

	/// <summary>Why characters can't be shown (no server folder, world not running), or null.</summary>
	public string? Problem
	{
		get => _problem;
		private set
		{
			if (Set(ref _problem, value))
			{
				OnPropertyChanged(nameof(HasProblem));
			}
		}
	}

	public bool HasProblem => Problem is not null;

	public bool IsLoading
	{
		get => _isLoading;
		private set => Set(ref _isLoading, value);
	}

	public string Summary => _all.Count switch
	{
		0 => "",
		1 => "1 player character",
		var n => $"{n} player characters",
	};

	public async Task RefreshAsync()
	{
		var locations = _locations();
		if (locations is not { HasServer: true })
		{
			_all = [];
			Problem = "Choose the server folder in the Server tab first.";
			ApplyFilter();
			return;
		}

		IsLoading = true;
		try
		{
			var database = WorldDatabase.FromServerFolder(locations);
			var characters = await database.ListPlayerCharactersAsync();
			_all = characters.Select(c => new CharacterRowViewModel(c, database, _maxAdena(), () => _ = RefreshAsync())).ToList();
			Problem = _all.Count == 0 ? "The world has no player characters yet. Create one in game first." : null;
		}
		catch (WorldDatabaseException ex)
		{
			_all = [];
			Problem = ex.Message;
		}
		finally
		{
			IsLoading = false;
			OnPropertyChanged(nameof(Summary));
			ApplyFilter();
		}
	}

	private void ApplyFilter()
	{
		Rows.Clear();
		foreach (var row in _all.Where(r => _searchText.Length == 0
			|| r.Character.Name.Contains(_searchText, StringComparison.OrdinalIgnoreCase)
			|| r.Character.Account.Contains(_searchText, StringComparison.OrdinalIgnoreCase)))
		{
			Rows.Add(row);
		}
	}
}

public sealed class CharacterRowViewModel : ObservableObject
{
	private readonly WorldDatabase _database;
	private readonly Action _refresh;
	private string _adenaText;
	private string? _message;
	private bool _messageIsError;
	private bool _isSaving;

	public CharacterRowViewModel(PlayerCharacter character, WorldDatabase database, long maxAdena, Action refresh)
	{
		Character = character;
		_database = database;
		_refresh = refresh;
		MaxAdena = maxAdena;
		_adenaText = (character.Adena ?? 0).ToString(CultureInfo.InvariantCulture);
		ApplyCommand = new RelayCommand(() => _ = ApplyAsync(), () => CanApply);
		UndoCommand = new RelayCommand(() => AdenaText = CurrentAdena.ToString(CultureInfo.InvariantCulture), () => IsDirty);
	}

	public PlayerCharacter Character { get; }
	public long MaxAdena { get; }
	public string Name => Character.Name;
	public string Details => $"Level {Character.Level}  ·  account {Character.Account}";
	public string StatusWord => Character.OnlineState switch { 0 => "Offline", 2 => "Offline shop", _ => "Online" };
	public bool IsOnline => Character.IsOnline;
	public bool HasAdenaRow => Character.AdenaObjectId is not null;
	public long CurrentAdena => Character.Adena ?? 0;
	public string CurrentAdenaText => $"{CurrentAdena:N0} adena";
	public string RangeText => $"Allowed 0 – {MaxAdena:N0} (0 removes the adena)";

	public bool CanEdit => !IsOnline && HasAdenaRow;

	/// <summary>Why this character's adena can't be edited right now, or null.</summary>
	public string? LockReason =>
		IsOnline ? "Log this character out to change its adena."
		: !HasAdenaRow ? "This character carries no adena, and the app can't add a new item while the server is running. Pick up or receive any adena in game first."
		: null;

	public string AdenaText
	{
		get => _adenaText;
		set
		{
			if (Set(ref _adenaText, value ?? ""))
			{
				Message = null;
				OnPropertyChanged(nameof(Error));
				OnPropertyChanged(nameof(IsDirty));
			}
		}
	}

	public string? Error
	{
		get
		{
			if (!long.TryParse(_adenaText.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out var value))
			{
				return "Enter a whole number of adena, e.g. 1000000.";
			}
			return value > MaxAdena ? $"Must be at most {MaxAdena:N0} (the server's maximum adena)." : null;
		}
	}

	public bool IsDirty => Error is null && long.Parse(_adenaText.Trim(), CultureInfo.InvariantCulture) != CurrentAdena;

	public bool CanApply => CanEdit && IsDirty && !_isSaving;

	public string? Message
	{
		get => _message;
		private set => Set(ref _message, value);
	}

	public bool MessageIsError
	{
		get => _messageIsError;
		private set => Set(ref _messageIsError, value);
	}

	public RelayCommand ApplyCommand { get; }
	public RelayCommand UndoCommand { get; }

	private async Task ApplyAsync()
	{
		var newCount = long.Parse(_adenaText.Trim(), CultureInfo.InvariantCulture);
		_isSaving = true;
		try
		{
			var result = await _database.SetAdenaAsync(Character, CurrentAdena, newCount);
			switch (result)
			{
				case AdenaEditResult.Saved:
					Log(newCount);
					_refresh();
					return;
				case AdenaEditResult.CharacterOnline:
					Fail("Not changed: the character logged in. Log it out and try again.");
					break;
				case AdenaEditResult.AmountChanged:
					Fail("Not changed: the character's adena changed since this list was loaded. Refresh and try again.");
					break;
				case AdenaEditResult.NoAdenaRow:
					Fail("Not changed: the character no longer carries adena.");
					break;
				default:
					Fail("Not changed: the character no longer exists.");
					break;
			}
		}
		catch (Exception ex) when (ex is WorldDatabaseException or MySqlConnector.MySqlException)
		{
			Fail($"Not changed: {ex.Message}");
		}
		finally
		{
			_isSaving = false;
		}
	}

	private void Fail(string message)
	{
		MessageIsError = true;
		Message = message;
	}

	private void Log(long newCount)
	{
		try
		{
			Directory.CreateDirectory(AppSettings.Folder);
			File.AppendAllText(Path.Combine(AppSettings.Folder, "character-edits.log"),
				$"{DateTime.Now:yyyy-MM-dd HH:mm:ss}  {Character.Name} (charId {Character.CharId}, account {Character.Account})  adena {CurrentAdena} -> {newCount}{Environment.NewLine}");
		}
		catch (IOException)
		{
			// The database change already happened; a missing log line is not worth failing over.
		}
	}
}
