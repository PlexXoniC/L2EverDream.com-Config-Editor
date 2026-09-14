using System.IO;
using System.Windows.Threading;
using L2Config.App.Infrastructure;
using L2Config.Core.Catalog;
using L2Config.Core.Storage;

namespace L2Config.App.ViewModels;

public sealed class MainViewModel : ObservableObject
{
	private readonly SettingsCatalog _catalog;
	private readonly AppSettings _appSettings;
	private readonly IDialogs _dialogs;
	private readonly DispatcherTimer _statusTimer;
	private SettingsStore? _store;
	private object _selectedTab;
	private bool _worldRunning;
	private bool _clientRunning;
	private string? _statusMessage;
	private string? _lastBackupFolder;
	private int _gameServerPort = 7777;

	public MainViewModel(SettingsCatalog catalog, CustomConfigReport? customConfig, AppSettings appSettings, IDialogs dialogs)
	{
		_catalog = catalog;
		_appSettings = appSettings;
		_dialogs = dialogs;

		ServerTab = new TabViewModel("server", catalog, () => ShowAdvanced, () => ChooseFolder(isServer: true));
		ClientTab = new TabViewModel("client", catalog, () => ShowAdvanced, () => ChooseFolder(isServer: false));
		CustomTab = customConfig is null ? null : new CustomConfigViewModel(customConfig, ShowSetting);
		CharactersTab = new CharactersViewModel(() => _store?.Locations, ReadMaxAdena);
		_selectedTab = TabFor(appSettings.LastTab);

		SaveCommand = new RelayCommand(Save, () => PendingCount > 0);
		DiscardCommand = new RelayCommand(Discard, () => PendingCount > 0);
		OpenBackupsCommand = new RelayCommand(() => _dialogs.OpenFolder(_lastBackupFolder ?? BackupSession.DefaultRoot));
		SelectTabCommand = new RelayCommand(p => SelectedTab = TabFor(p as string));

		Reload();
		if (_selectedTab == CharactersTab)
		{
			_ = CharactersTab.RefreshAsync();
		}

		_statusTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(4) };
		_statusTimer.Tick += async (_, _) => await RefreshRuntimeStatusAsync();
		_statusTimer.Start();
		_ = RefreshRuntimeStatusAsync();
	}

	public TabViewModel ServerTab { get; }
	public TabViewModel ClientTab { get; }
	public CustomConfigViewModel? CustomTab { get; }
	public bool HasCustomTab => CustomTab is not null;
	public CharactersViewModel CharactersTab { get; }

	/// <summary>A <see cref="TabViewModel"/> (Server, Client), the <see cref="CustomConfigViewModel"/> or the <see cref="CharactersViewModel"/>.</summary>
	public object SelectedTab
	{
		get => _selectedTab;
		set
		{
			if (Set(ref _selectedTab, value))
			{
				OnPropertyChanged(nameof(IsServerTab));
				OnPropertyChanged(nameof(IsClientTab));
				OnPropertyChanged(nameof(IsCustomTab));
				OnPropertyChanged(nameof(IsCharactersTab));
				_appSettings.LastTab = value switch
				{
					TabViewModel tab => tab.Scope,
					CharactersViewModel => "characters",
					_ => "custom",
				};
				_appSettings.Save();
				if (value == CharactersTab)
				{
					_ = CharactersTab.RefreshAsync();
				}
			}
		}
	}

	public bool IsServerTab => SelectedTab == ServerTab;
	public bool IsClientTab => SelectedTab == ClientTab;
	public bool IsCustomTab => CustomTab is not null && SelectedTab == CustomTab;
	public bool IsCharactersTab => SelectedTab == CharactersTab;

	private object TabFor(string? name) => name switch
	{
		"client" => ClientTab,
		"custom" when CustomTab is not null => CustomTab,
		"characters" => CharactersTab,
		_ => ServerTab,
	};

	/// <summary>The server's own adena cap (Player.ini MaxAdena; a negative value means the client's maximum).</summary>
	private long ReadMaxAdena() =>
		long.TryParse(ReadServerValue("MaxAdena"), out var max) && max >= 0 ? max : int.MaxValue;

	/// <summary>Opens a setting in the editor (used by the Custom Config tab).</summary>
	public void ShowSetting(string settingId)
	{
		var setting = ServerTab.Settings.Concat(ClientTab.Settings).FirstOrDefault(s => s.Definition.Id == settingId);
		if (setting is null)
		{
			return;
		}
		var tab = SettingTargets.IsClient(setting.Definition.Target) ? ClientTab : ServerTab;
		if (setting.Definition.Advanced)
		{
			ShowAdvanced = true;
		}
		tab.ChangedOnly = false;
		tab.SearchText = setting.Definition.Key;
		SelectedTab = tab;
	}

	public bool ShowAdvanced
	{
		get => _appSettings.ShowAdvanced;
		set
		{
			if (_appSettings.ShowAdvanced == value)
			{
				return;
			}
			_appSettings.ShowAdvanced = value;
			_appSettings.Save();
			OnPropertyChanged();
			ServerTab.Refresh();
			ClientTab.Refresh();
		}
	}

	public int PendingCount => ServerTab.DirtyCount + ClientTab.DirtyCount;

	public string PendingText => PendingCount switch
	{
		0 => "No unsaved changes",
		1 => "1 unsaved change",
		var n => $"{n} unsaved changes",
	};

	public string? StatusMessage
	{
		get => _statusMessage;
		private set => Set(ref _statusMessage, value);
	}

	public RelayCommand SaveCommand { get; }
	public RelayCommand DiscardCommand { get; }
	public RelayCommand OpenBackupsCommand { get; }
	public RelayCommand SelectTabCommand { get; }

	private void ChooseFolder(bool isServer)
	{
		var tab = isServer ? ServerTab : ClientTab;
		if (tab.DirtyCount > 0 && !_dialogs.Confirm(
				"Unsaved changes",
				$"You have {tab.DirtyCount} unsaved {tab.Title.ToLowerInvariant()} change(s). Choosing another folder discards them.",
				"Discard and choose"))
		{
			return;
		}

		var picked = _dialogs.PickFolder(isServer ? "Choose the L2Everdream server folder" : "Choose the Lineage 2 client folder", tab.FolderPath);
		if (picked is null)
		{
			return;
		}
		if (isServer)
		{
			if (!L2Locations.IsServerRoot(picked))
			{
				_dialogs.Notice("That isn't a server folder", $"{picked}\n\nA server folder has game\\config\\Server.ini inside it. Choose the folder that contains the game and login folders.");
				return;
			}
			_appSettings.ServerFolder = picked;
		}
		else
		{
			if (L2Locations.ResolveClientSystemDir(picked) is null)
			{
				_dialogs.Notice("That isn't a client folder", $"{picked}\n\nA Lineage 2 client has system\\l2.ini inside it. Choose the client folder or its system folder.");
				return;
			}
			_appSettings.ClientFolder = picked;
		}
		_appSettings.Save();
		Reload(keepPendingFor: isServer ? ClientTab : ServerTab);
	}

	/// <summary>Re-reads every file. Pending edits on <paramref name="keepPendingFor"/> are carried over.</summary>
	private void Reload(TabViewModel? keepPendingFor = null)
	{
		var carried = keepPendingFor?.Settings.Where(s => s.IsDirty).ToDictionary(s => s.Definition.Id, s => s.Value) ?? [];

		var serverRoot = _appSettings.ServerFolder;
		var clientSystem = _appSettings.ClientFolder is { } c ? L2Locations.ResolveClientSystemDir(c) : null;
		var locations = new L2Locations(serverRoot, clientSystem);
		_store = new SettingsStore(_catalog, locations);
		_store.Load();

		ServerTab.FolderPath = serverRoot;
		ServerTab.FolderProblem = serverRoot is null ? null
			: !locations.HasServer ? "This folder no longer contains game\\config\\Server.ini." : null;
		ClientTab.FolderPath = _appSettings.ClientFolder;
		ClientTab.FolderProblem = _appSettings.ClientFolder is null ? null
			: !locations.HasClient ? "This folder no longer contains system\\l2.ini." : null;

		_gameServerPort = int.TryParse(ReadServerValue("GameserverPort"), out var port) ? port : 7777;

		var groupNames = _catalog.Categories.SelectMany(c => c.Groups).ToDictionary(g => g.Id, g => g.Name);
		var server = new List<SettingViewModel>();
		var client = new List<SettingViewModel>();
		foreach (var definition in _catalog.Settings)
		{
			var isClient = SettingTargets.IsClient(definition.Target);
			var hasFolder = isClient ? locations.HasClient : locations.HasServer;
			var file = _store.FileFor(definition);
			var vm = new SettingViewModel(
				definition,
				groupNames[definition.Group],
				hasFolder ? _store.GetValue(definition) : null,
				hasFolder ? file.LoadError : "Choose the folder first.",
				OnSettingChanged);
			if (carried.TryGetValue(definition.Id, out var pending))
			{
				vm.Value = pending;
			}
			(isClient ? client : server).Add(vm);
		}
		ServerTab.SetSettings(server);
		ClientTab.SetSettings(client);
		CustomTab?.UpdateCurrentValues(_store, _catalog, locations.HasServer);
		if (IsCharactersTab)
		{
			_ = CharactersTab.RefreshAsync();
		}
		RaisePending();
	}

	private string? ReadServerValue(string key)
	{
		var definition = _catalog.Settings.FirstOrDefault(s => s.Target == SettingTargets.ServerGame && s.Key == key);
		return definition is null || _store is null ? null : _store.GetValue(definition);
	}

	private void OnSettingChanged()
	{
		ServerTab.UpdateDirtyCount();
		ClientTab.UpdateDirtyCount();
		RaisePending();
	}

	private void RaisePending()
	{
		OnPropertyChanged(nameof(PendingCount));
		OnPropertyChanged(nameof(PendingText));
	}

	private void Save()
	{
		var dirty = ServerTab.Settings.Concat(ClientTab.Settings).Where(s => s.IsDirty).ToList();
		var invalid = dirty.FirstOrDefault(s => s.HasError);
		if (invalid is not null)
		{
			_dialogs.Notice("Check this value", $"{invalid.Name}: {invalid.Error}");
			var invalidTab = SettingTargets.IsClient(invalid.Definition.Target) ? ClientTab : ServerTab;
			invalidTab.SearchText = invalid.Definition.Key;
			SelectedTab = invalidTab;
			return;
		}
		if (_clientRunning && dirty.Any(s => SettingTargets.IsClient(s.Definition.Target)))
		{
			_dialogs.Notice("Close the game first",
				"Lineage 2 is running. The client rewrites its own settings when it closes, which would undo these changes.\n\nClose the game, then press Save again.");
			return;
		}

		try
		{
			var result = _store!.Save(dirty.Select(s => (s.Definition, s.Value)).ToList(), BackupSession.DefaultRoot);
			foreach (var setting in dirty)
			{
				setting.AcceptSaved();
			}
			_lastBackupFolder = result.BackupFolder ?? _lastBackupFolder;
			var files = result.WrittenFiles.Count;
			StatusMessage = $"Saved {dirty.Count} change{(dirty.Count == 1 ? "" : "s")} to {files} file{(files == 1 ? "" : "s")}." +
				(_worldRunning && dirty.Any(s => !SettingTargets.IsClient(s.Definition.Target))
					? " Your world is running — server changes take effect the next time it starts."
					: "");
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException or InvalidOperationException or ArgumentException)
		{
			_dialogs.Notice("Nothing was saved", ex.Message);
			Reload();
			return;
		}
		OnSettingChanged();
		ServerTab.Refresh();
		ClientTab.Refresh();
		CustomTab?.UpdateCurrentValues(_store, _catalog, ServerTab.HasFolder);
	}

	private void Discard()
	{
		if (!_dialogs.Confirm("Discard changes", $"Undo all {PendingText.ToLowerInvariant()}?", "Discard"))
		{
			return;
		}
		Reload();
		StatusMessage = "Changes discarded.";
	}

	private async Task RefreshRuntimeStatusAsync()
	{
		_worldRunning = ServerTab.HasFolder && await RuntimeStatus.IsWorldRunningAsync(_gameServerPort);
		_clientRunning = RuntimeStatus.IsClientRunning();
		ServerTab.RuntimeNotice = _worldRunning
			? "Your world is running. Changes you save now take effect the next time the world starts."
			: null;
		ClientTab.RuntimeNotice = _clientRunning
			? "Lineage 2 is open. Close the game before saving client settings — it overwrites them when it exits."
			: null;
	}
}

public interface IDialogs
{
	string? PickFolder(string title, string? initial);
	bool Confirm(string title, string message, string confirmText);
	void Notice(string title, string message);
	void OpenFolder(string path);
}
