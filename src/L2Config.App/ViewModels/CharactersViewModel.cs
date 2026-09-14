using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using L2Config.App.Infrastructure;
using L2Config.Core.Backups;
using L2Config.Core.Characters;
using L2Config.Core.Storage;

namespace L2Config.App.ViewModels;

/// <summary>What the Characters tab needs to know about the chosen server's settings.</summary>
public sealed record ServerFacts(
	L2Locations Locations,
	InventoryLimits Limits,
	long MaxAdena,
	bool? DeliveryEnabledInFile,
	int DeliveryDelaySeconds,
	string? DeliverySettingId);

/// <summary>The Characters tab: player characters in the running world, their adena, and an inventory editor.</summary>
public sealed class CharactersViewModel : ObservableObject
{
	private readonly Func<ServerFacts?> _facts;
	private readonly IDialogs _dialogs;
	private readonly Action<string> _showSetting;
	private List<CharacterRowViewModel> _all = [];
	private string _searchText = "";
	private string? _problem;
	private bool _isLoading;
	private InventoryViewModel? _inventory;
	private (string Root, ItemCatalog Catalog)? _items;

	public CharactersViewModel(Func<ServerFacts?> facts, IDialogs dialogs, Action<string> showSetting)
	{
		_facts = facts;
		_dialogs = dialogs;
		_showSetting = showSetting;
		RefreshCommand = new RelayCommand(() => _ = RefreshAsync(), () => !IsLoading);
		CloseInventoryCommand = new RelayCommand(() => { Inventory = null; _ = RefreshAsync(); });
	}

	public string Title => "Characters";

	public string Intro =>
		"Player characters in your running world. Changes go straight into the world's database, and the rows they change are " +
		"backed up first (see the Backups tab). Existing items can only be changed while the character is logged out — the " +
		"server keeps a logged-in character's inventory in memory and would overwrite the change.";

	public ObservableCollection<CharacterRowViewModel> Rows { get; } = [];
	public RelayCommand RefreshCommand { get; }
	public RelayCommand CloseInventoryCommand { get; }

	/// <summary>The open inventory editor, or null when the character list is shown.</summary>
	public InventoryViewModel? Inventory
	{
		get => _inventory;
		private set
		{
			if (Set(ref _inventory, value))
			{
				OnPropertyChanged(nameof(IsListVisible));
			}
		}
	}

	public bool IsListVisible => Inventory is null;

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
		if (Inventory is not null)
		{
			await Inventory.RefreshAsync();
			return;
		}
		var facts = _facts();
		if (facts is null || !facts.Locations.HasServer)
		{
			_all = [];
			Problem = "Choose the server folder in the Server tab first.";
			ApplyFilter();
			return;
		}

		IsLoading = true;
		try
		{
			var database = WorldDatabase.FromServerFolder(facts.Locations);
			var characters = await database.ListPlayerCharactersAsync();
			_all = characters.Select(c => new CharacterRowViewModel(c, database, facts, OpenInventory, () => _ = RefreshAsync())).ToList();
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

	/// <summary>The server's item list, read once per server folder.</summary>
	public async Task<ItemCatalog> ItemsAsync(string serverRoot)
	{
		if (_items is { } cached && cached.Root == serverRoot)
		{
			return cached.Catalog;
		}
		var catalog = await Task.Run(() => ItemCatalog.LoadFromServer(serverRoot));
		_items = (serverRoot, catalog);
		return catalog;
	}

	private void OpenInventory(CharacterRowViewModel row)
	{
		if (_facts() is not { } facts)
		{
			return;
		}
		Inventory = new InventoryViewModel(row.Character, row.Database, facts, this, _dialogs, _showSetting, CloseInventoryCommand);
		_ = Inventory.RefreshAsync();
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
	private readonly ServerFacts _facts;
	private readonly Action _refresh;
	private string _adenaText;
	private string? _message;
	private bool _isSaving;

	public CharacterRowViewModel(PlayerCharacter character, WorldDatabase database, ServerFacts facts, Action<CharacterRowViewModel> openInventory, Action refresh)
	{
		Character = character;
		Database = database;
		_facts = facts;
		_refresh = refresh;
		_adenaText = (character.Adena ?? 0).ToString(CultureInfo.InvariantCulture);
		ApplyCommand = new RelayCommand(() => _ = ApplyAsync(), () => CanApply);
		UndoCommand = new RelayCommand(() => AdenaText = CurrentAdena.ToString(CultureInfo.InvariantCulture), () => IsDirty);
		OpenInventoryCommand = new RelayCommand(() => openInventory(this));
	}

	public PlayerCharacter Character { get; }
	public WorldDatabase Database { get; }
	public long MaxAdena => _facts.MaxAdena;
	public string Name => Character.Name;

	public string Details =>
		$"Level {Character.Level}  ·  account {Character.Account}  ·  {Character.SlotsUsed:N0} of {_facts.Limits.For(Character):N0} inventory slots";

	public string StatusWord => Character.OnlineState switch { 0 => "Offline", 2 => "Offline shop", _ => "Online" };
	public bool IsOnline => Character.IsOnline;
	public bool HasAdenaRow => Character.AdenaObjectId is not null;
	public long CurrentAdena => Character.Adena ?? 0;
	public string CurrentAdenaText => $"{CurrentAdena:N0} adena";
	public string RangeText => $"Allowed 0 – {MaxAdena:N0} (Player.ini › MaxAdena). 0 removes the adena.";

	public bool CanEdit => !IsOnline && HasAdenaRow;

	public string? LockReason =>
		IsOnline ? "Log this character out to change its adena or items."
		: !HasAdenaRow ? "This character carries no adena. Use Inventory to have the server deliver some."
		: null;

	public RelayCommand ApplyCommand { get; }
	public RelayCommand UndoCommand { get; }
	public RelayCommand OpenInventoryCommand { get; }

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

	private async Task ApplyAsync()
	{
		var newCount = long.Parse(_adenaText.Trim(), CultureInfo.InvariantCulture);
		_isSaving = true;
		try
		{
			var backup = new BackupSession(BackupSession.DefaultRoot, DateTime.Now, BackupKind.Characters, $"{Character.Name} adena");
			var item = new InventoryItem(Character.AdenaObjectId!.Value, WorldDatabase.AdenaItemId, CurrentAdena, 0, false);
			var outcome = await Database.SetItemCountAsync(Character, item, newCount, "Adena", backup);
			if (outcome.Ok)
			{
				_refresh();
				return;
			}
			Message = "Not changed: " + outcome.Message;
		}
		catch (Exception ex) when (ex is WorldDatabaseException or MySqlConnector.MySqlException or IOException)
		{
			Message = $"Not changed: {ex.Message}";
		}
		finally
		{
			_isSaving = false;
		}
	}
}
