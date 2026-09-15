using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using L2Config.App.Infrastructure;
using L2Config.Core.Skills;
using L2Config.Core.Storage;

namespace L2Config.App.ViewModels;

/// <summary>
/// The page editor for Player.ini SkillDurationList: every skill with a duration, its normal duration, and the duration you give it.
/// Edits go straight into the setting's value, so they are saved (with backups) by the normal Save changes bar.
/// </summary>
public sealed class SkillDurationsViewModel : ObservableObject
{
	private readonly SettingViewModel _setting;
	private readonly SettingViewModel? _gate;
	private readonly IDialogs _dialogs;
	private readonly Action<string> _showSetting;
	private SortedDictionary<int, int> _durations = [];
	private List<SkillDurationRowViewModel> _rows = [];
	private SkillCatalog? _catalog;
	private string _filter = "players";
	private string _searchText = "";
	private string _bulkText = "";
	private string? _bulkProblem;
	private string? _message;
	private string? _lastWritten;
	private bool _isLoading = true;
	private bool _suppress;

	public SkillDurationsViewModel(SettingViewModel setting, SettingViewModel? gate, Func<Task<SkillCatalog>> loadCatalog, Action close,
		IDialogs dialogs, Action<string> showSetting)
	{
		_setting = setting;
		_gate = gate;
		_dialogs = dialogs;
		_showSetting = showSetting;
		CloseCommand = new RelayCommand(close);
		FilterCommand = new RelayCommand(p => Filter = p as string ?? "players");
		BulkCommand = new RelayCommand(p => ApplyBulk(p as string ?? ""), _ => Visible.Count > 0);
		RemoveUnknownCommand = new RelayCommand(RemoveUnknown);
		SwitchOnCommand = new RelayCommand(() => { if (_gate is not null) _gate.BoolValue = true; });
		ShowGateCommand = new RelayCommand(() => { if (_gate is not null) showSetting(_gate.Definition.Id); });
		UndoAllCommand = new RelayCommand(() => _setting.UndoCommand.Execute(null), () => _setting.IsDirty);

		setting.PropertyChanged += OnSettingChanged;
		if (gate is not null)
		{
			gate.PropertyChanged += (_, e) =>
			{
				if (e.PropertyName == nameof(SettingViewModel.Value))
				{
					OnPropertyChanged(nameof(GateText));
				}
			};
		}
		_ = LoadAsync(loadCatalog);
	}

	public string Title => _setting.Name;
	public string Location => _setting.Location;

	public IReadOnlyList<string> Notes { get; } =
	[
		"The duration you set is used for everyone who casts the skill — you, simulated players and monsters. A longer debuff also lasts longer on you.",
		"Leave a box empty to keep the skill's normal duration. Durations go up to 12 hours; type them as 90s, 20m, 1h 30m or 1:30:00.",
		"Enchanted “+Time” skill levels add your duration to their enchanted duration (the server works this way).",
		"Changes are saved with Save changes like any other setting, and take effect the next time the world starts.",
	];

	public bool IsLoading
	{
		get => _isLoading;
		private set => Set(ref _isLoading, value);
	}

	public string? GateText => _gate is not null && !SettingValues.IsTrue(_gate.Value)
		? $"“{_gate.Name}” is Off, so the server ignores this list. Switch it on to use these durations."
		: null;

	public ObservableCollection<SkillDurationRowViewModel> Visible { get; } = [];
	public bool HasVisible => Visible.Count > 0;

	public string Filter
	{
		get => _filter;
		set
		{
			if (Set(ref _filter, value))
			{
				RaiseFilterFlags();
				ApplyFilter();
			}
		}
	}

	public bool IsPlayersFilter => _filter == "players";
	public bool IsSongsFilter => _filter == "songs";
	public bool IsDebuffsFilter => _filter == "debuffs";
	public bool IsNpcFilter => _filter == "npc";
	public bool IsChangedFilter => _filter == "changed";
	public bool IsAllFilter => _filter == "all";

	public int PlayersCount => _rows.Count(r => IsPlayerBuff(r.Skill));
	public int SongsCount => _rows.Count(r => r.Skill.Kind == SkillKind.SongOrDance);
	public int DebuffsCount => _rows.Count(r => r.Skill.Kind == SkillKind.Debuff);
	public int NpcCount => _rows.Count(r => r.Skill.NpcOnly);
	public int ChangedCount => _rows.Count(r => r.IsChanged);
	public int AllCount => _rows.Count;

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

	public string Summary => (_durations.Keys.Count(id => _catalog?.Find(id) is not null), _rows.Count(r => r.HasError)) switch
	{
		(0, 0) => "No skill has a custom duration yet.",
		(var n, 0) => n == 1 ? "1 skill has a custom duration." : $"{n} skills have a custom duration.",
		(var n, var bad) => $"{n} skill(s) have a custom duration. {bad} box(es) can't be read and keep their previous value.",
	};

	public string ShownText => Visible.Count == 1 ? "1 skill shown" : $"{Visible.Count} skills shown";

	public string? UnknownText
	{
		get
		{
			var unknown = UnknownIds;
			return unknown.Count == 0 ? null
				: $"The list also has {unknown.Count} skill id(s) that are not in your datapack, so the server ignores them: {string.Join(", ", unknown.Take(12))}{(unknown.Count > 12 ? "…" : "")}.";
		}
	}

	private IReadOnlyList<string> _unreadable = [];

	/// <summary>Entries in the file this page can't read; they are dropped the next time a duration is changed here.</summary>
	public string? UnreadableText => _unreadable.Count == 0 ? null
		: $"The list in the file has {_unreadable.Count} problem{(_unreadable.Count == 1 ? "" : "s")} (entries that can't be read are left out when you change a duration here): {string.Join(" ", _unreadable.Take(3))}";

	private List<int> UnknownIds => _catalog is null ? [] : _durations.Keys.Where(id => _catalog.Find(id) is null).ToList();

	public string BulkText
	{
		get => _bulkText;
		set
		{
			if (Set(ref _bulkText, value ?? ""))
			{
				BulkProblem = null;
			}
		}
	}

	public string? BulkProblem
	{
		get => _bulkProblem;
		private set => Set(ref _bulkProblem, value);
	}

	public string? Message
	{
		get => _message;
		private set => Set(ref _message, value);
	}

	public RelayCommand CloseCommand { get; }
	public RelayCommand FilterCommand { get; }
	public RelayCommand BulkCommand { get; }
	public RelayCommand RemoveUnknownCommand { get; }
	public RelayCommand SwitchOnCommand { get; }
	public RelayCommand ShowGateCommand { get; }
	public RelayCommand UndoAllCommand { get; }

	private async Task LoadAsync(Func<Task<SkillCatalog>> loadCatalog)
	{
		try
		{
			_catalog = await loadCatalog();
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or System.Xml.XmlException)
		{
			Message = "The skill list could not be read from the server folder: " + ex.Message;
			IsLoading = false;
			return;
		}
		ReadSetting();
		_rows = _catalog.All.Select(s => new SkillDurationRowViewModel(s, this)).ToList();
		foreach (var row in _rows)
		{
			row.Load(_durations.TryGetValue(row.Skill.Id, out var seconds) ? seconds : null);
		}
		IsLoading = false;
		RaiseCounts();
		ApplyFilter();
	}

	/// <summary>A row's box changed to a valid duration (or was emptied).</summary>
	internal void RowChanged(SkillDurationRowViewModel row, int? seconds)
	{
		if (_suppress)
		{
			return;
		}
		if (seconds is null)
		{
			_durations.Remove(row.Skill.Id);
		}
		else
		{
			_durations[row.Skill.Id] = seconds.Value;
		}
		Write();
	}

	internal void RowErrorChanged() => OnPropertyChanged(nameof(Summary));

	private void ApplyBulk(string action)
	{
		var rows = Visible.ToList();
		if (rows.Count == 0)
		{
			return;
		}
		Func<SkillDurationRowViewModel, int?> newValue;
		string description;
		switch (action)
		{
			case "x2" or "x3":
				var factor = action == "x2" ? 2 : 3;
				newValue = r => Math.Min(r.Skill.MaxSeconds * factor, SkillCatalog.MaxSeconds);
				description = $"{factor}× their normal duration (at most 12 hours)";
				break;
			case "1h" or "2h":
				var hours = action == "1h" ? 1 : 2;
				newValue = _ => hours * 3600;
				description = hours == 1 ? "1 hour" : "2 hours";
				break;
			case "reset":
				newValue = _ => null;
				description = "their normal duration (removed from the list)";
				break;
			default:
				if (SkillDurationList.ParseDuration(BulkText) is not { } typed)
				{
					BulkProblem = "Type a duration first, e.g. 45m, 1h 30m or 3600.";
					return;
				}
				if (typed is < 1 or > SkillCatalog.MaxSeconds)
				{
					BulkProblem = "Durations go from 1 second to 12 hours.";
					return;
				}
				newValue = _ => typed;
				description = SkillDurationList.Describe(typed);
				break;
		}

		var names = string.Join(", ", rows.Take(5).Select(r => r.Name)) + (rows.Count > 5 ? $" and {rows.Count - 5} more" : "");
		if (!_dialogs.Confirm("Set durations",
				$"Set {rows.Count} skill{(rows.Count == 1 ? "" : "s")} shown to {description}?\n\n{names}\n\n" +
				"Nothing is written to the file until you press Save changes, and Undo on the setting card puts the list back.",
				"Set durations"))
		{
			return;
		}
		_suppress = true;
		foreach (var row in rows)
		{
			var value = newValue(row);
			row.Load(value);
			if (value is null)
			{
				_durations.Remove(row.Skill.Id);
			}
			else
			{
				_durations[row.Skill.Id] = value.Value;
			}
		}
		_suppress = false;
		Write();
		Message = $"{rows.Count} skill{(rows.Count == 1 ? "" : "s")} set to {description}. Press Save changes to write them.";
		if (_filter == "changed")
		{
			ApplyFilter();
		}
	}

	private void RemoveUnknown()
	{
		foreach (var id in UnknownIds)
		{
			_durations.Remove(id);
		}
		Write();
	}

	private void Write()
	{
		_unreadable = [];
		OnPropertyChanged(nameof(UnreadableText));
		_lastWritten = SkillDurationList.Format(_durations);
		_setting.Value = _lastWritten;
		RaiseCounts();
	}

	private void ReadSetting()
	{
		(_durations, _unreadable) = SkillDurationList.Parse(_setting.Value);
		_lastWritten = _setting.Value;
		OnPropertyChanged(nameof(UnreadableText));
	}

	/// <summary>Undo, Reset or Discard changed the value outside this page: show what the setting now says.</summary>
	private void OnSettingChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName != nameof(SettingViewModel.Value) || _setting.Value == _lastWritten || _catalog is null)
		{
			return;
		}
		ReadSetting();
		_suppress = true;
		foreach (var row in _rows)
		{
			row.Load(_durations.TryGetValue(row.Skill.Id, out var seconds) ? seconds : null);
		}
		_suppress = false;
		RaiseCounts();
		ApplyFilter();
	}

	private static bool IsPlayerBuff(TimedSkill s) => (s.LearnedByPlayers || s.FromBuffer) && s.Kind != SkillKind.Debuff;

	private void ApplyFilter()
	{
		var words = _searchText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
		Visible.Clear();
		foreach (var row in _rows)
		{
			var include = _filter switch
			{
				"songs" => row.Skill.Kind == SkillKind.SongOrDance,
				"debuffs" => row.Skill.Kind == SkillKind.Debuff,
				"npc" => row.Skill.NpcOnly,
				"changed" => row.IsChanged || row.HasError,
				"all" => true,
				_ => IsPlayerBuff(row.Skill),
			};
			if (include && words.All(w => row.Name.Contains(w, StringComparison.OrdinalIgnoreCase) || row.Skill.Id.ToString() == w))
			{
				Visible.Add(row);
			}
		}
		OnPropertyChanged(nameof(HasVisible));
		OnPropertyChanged(nameof(ShownText));
	}

	private void RaiseCounts()
	{
		OnPropertyChanged(nameof(Summary));
		OnPropertyChanged(nameof(UnknownText));
		OnPropertyChanged(nameof(ChangedCount));
		OnPropertyChanged(nameof(PlayersCount));
		OnPropertyChanged(nameof(SongsCount));
		OnPropertyChanged(nameof(DebuffsCount));
		OnPropertyChanged(nameof(NpcCount));
		OnPropertyChanged(nameof(AllCount));
		OnPropertyChanged(nameof(GateText));
	}

	private void RaiseFilterFlags()
	{
		OnPropertyChanged(nameof(IsPlayersFilter));
		OnPropertyChanged(nameof(IsSongsFilter));
		OnPropertyChanged(nameof(IsDebuffsFilter));
		OnPropertyChanged(nameof(IsNpcFilter));
		OnPropertyChanged(nameof(IsChangedFilter));
		OnPropertyChanged(nameof(IsAllFilter));
	}
}

public sealed class SkillDurationRowViewModel : ObservableObject
{
	private readonly SkillDurationsViewModel _owner;
	private string _text = "";
	private string? _error;
	private int? _seconds;

	public SkillDurationRowViewModel(TimedSkill skill, SkillDurationsViewModel owner)
	{
		Skill = skill;
		_owner = owner;
		ResetCommand = new RelayCommand(() => Text = "", () => _text.Length > 0);
		KindWord = skill.Kind switch { SkillKind.Debuff => "Debuff", SkillKind.SongOrDance => "Song / dance", _ => "Buff" };
		UsedBy = skill.NpcOnly ? "NPC and monster skill"
			: string.Join(" · ", new[] { skill.LearnedByPlayers ? "Players" : null, skill.FromBuffer ? "Buffer NPC" : null }.OfType<string>());
		DefaultText = skill.VariesByLevel
			? $"{SkillDurationList.Describe(skill.MinSeconds)} – {SkillDurationList.Describe(skill.MaxSeconds)} by level"
			: SkillDurationList.Describe(skill.MinSeconds);
		EnchantText = skill.EnchantedMaxSeconds is { } enchanted ? $"+Time enchant: up to {SkillDurationList.Describe(enchanted)}" : null;
	}

	public TimedSkill Skill { get; }
	public string Name => Skill.Name;
	public string IdText => $"ID {Skill.Id}";
	public string KindWord { get; }
	public bool IsDebuff => Skill.Kind == SkillKind.Debuff;
	public string UsedBy { get; }
	public string DefaultText { get; }
	public string? EnchantText { get; }
	public RelayCommand ResetCommand { get; }

	/// <summary>What is typed in the box: empty for the normal duration.</summary>
	public string Text
	{
		get => _text;
		set
		{
			if (!Set(ref _text, value ?? ""))
			{
				return;
			}
			var trimmed = _text.Trim();
			if (trimmed.Length == 0)
			{
				SetResult(null, null);
				return;
			}
			var seconds = SkillDurationList.ParseDuration(trimmed);
			if (seconds is null)
			{
				SetError("Type a duration such as 90s, 20m, 1h 30m or 1:30:00.");
			}
			else if (seconds is < 1 or > SkillCatalog.MaxSeconds)
			{
				SetError("Durations go from 1 second to 12 hours.");
			}
			else
			{
				SetResult(seconds, null);
			}
		}
	}

	public string? Error => _error;
	public bool HasError => _error is not null;
	public bool IsChanged => _seconds is not null;

	public string? PreviewText => _seconds is { } s && !HasError
		? $"= {SkillDurationList.Describe(s)}" + (Skill.EnchantedMaxSeconds is not null ? " (added to +Time enchanted levels)" : "")
		: null;

	/// <summary>Shows a value from the list without reporting it back as an edit.</summary>
	internal void Load(int? seconds)
	{
		_seconds = seconds;
		_text = seconds is null ? "" : SkillDurationList.Describe(seconds.Value);
		_error = seconds is < 1 or > SkillCatalog.MaxSeconds ? "Durations go from 1 second to 12 hours." : null;
		RaiseAll();
	}

	private void SetResult(int? seconds, string? error)
	{
		var hadError = HasError;
		_seconds = seconds;
		_error = error;
		RaiseAll();
		_owner.RowChanged(this, seconds);
		if (hadError)
		{
			_owner.RowErrorChanged();
		}
	}

	private void SetError(string error)
	{
		_error = error;
		RaiseAll();
		_owner.RowErrorChanged();
	}

	private void RaiseAll()
	{
		OnPropertyChanged(nameof(Text));
		OnPropertyChanged(nameof(Error));
		OnPropertyChanged(nameof(HasError));
		OnPropertyChanged(nameof(IsChanged));
		OnPropertyChanged(nameof(PreviewText));
	}
}
