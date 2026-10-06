using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows.Media;
using L2Config.App.Infrastructure;
using L2Config.Core.Backups;
using L2Config.Core.Characters;
using L2Config.Core.Client;
using L2Config.Core.Shops;
using L2Config.Core.Storage;

namespace L2Config.App.ViewModels;

/// <summary>
/// The GM Shop tab: a shopkeeper beside the gatekeeper in every town, selling the world's own items at the world's own
/// prices. Everything it writes is a new file in a folder the server already reads, so an L2Everdream update leaves it
/// alone; everything it replaces is backed up first, and the world has to be restarted from the launcher for a change to
/// take. This is the one place the app adds something to a world rather than only changing a setting, so it says so, and
/// Remove puts the world back exactly as it was.
/// </summary>
public sealed class ShopsViewModel : ObservableObject
{
	private readonly Func<L2Locations?> _locations;
	private readonly Func<IReadOnlyList<SettingViewModel>> _serverSettings;
	private readonly AppSettings _appSettings;
	private readonly IDialogs _dialogs;
	private readonly Action<string> _showSetting;
	private ShopChoices _choices = new();
	private ShopDefinition? _defaults;
	private ItemCatalog? _items;
	private IconLibrary? _icons;
	private ShopStatus? _status;
	private ShopPageViewModel? _selectedPage;
	private string _itemSearch = "";
	private string? _message;
	private bool _busy;

	public ShopsViewModel(Func<L2Locations?> locations, Func<IReadOnlyList<SettingViewModel>> serverSettings,
		AppSettings appSettings, IDialogs dialogs, Action<string> showSetting)
	{
		_locations = locations;
		_serverSettings = serverSettings;
		_appSettings = appSettings;
		_dialogs = dialogs;
		_showSetting = showSetting;
		InstallCommand = new RelayCommand(() => Write(install: true), () => CanWrite);
		RemoveCommand = new RelayCommand(() => Write(install: false), () => CanWrite && Installed);
		SelectPageCommand = new RelayCommand(p => { if (p is ShopPageViewModel page) SelectedPage = page; });
		ShowSettingCommand = new RelayCommand(p => { if (p is string id) showSetting(id); });
		ExportCommand = new RelayCommand(Export, () => Shop is not null);
	}

	public string Title => "GM Shop";

	public string Intro =>
		"A shopkeeper standing beside the gatekeeper in every town, selling the items your own world knows about at the " +
		"prices your own world puts on them. This is the one thing the app adds to your world rather than just changing a " +
		"setting: it writes files of its own that an L2Everdream update leaves alone, and Remove takes every one of them " +
		"back out again.";

	public bool HasServer => _locations() is { HasServer: true };

	// ------------------------------------------------------------------ loading

	public async Task LoadAsync()
	{
		if (_locations() is not { HasServer: true, ServerRoot: { } server })
		{
			return;
		}
		Busy = true;
		try
		{
			_choices = ShopChoices.Load(AppSettings.Folder);
			var clientSystem = _locations()?.ClientSystemDir;
			var (items, defaults, icons) = await Task.Run(() =>
			{
				var items = ItemCatalog.LoadFromServer(server);
				var shop = ShopDefaults.Build(items, ArmorSets.ReadFromServer(server), Gatekeepers.ReadFromServer(server));
				IconLibrary? icons = null;
				try
				{
					icons = clientSystem is null ? null : IconLibrary.ForClient(clientSystem);
				}
				catch (IOException)
				{
					icons = null;
				}
				return (items, shop, icons);
			});
			_items = items;
			_defaults = defaults;
			_icons = icons;
			Rebuild();
		}
		finally
		{
			Busy = false;
		}
	}

	/// <summary>The shop as it stands: the world's own defaults with the player's choices laid over them.</summary>
	public ShopDefinition? Shop => _defaults is null ? null : _choices.ApplyTo(_defaults);

	private void Rebuild()
	{
		if (Shop is not { } shop)
		{
			return;
		}

		Towns.Clear();
		foreach (var placement in shop.Placements)
		{
			Towns.Add(new ShopTownViewModel(placement, Changed));
		}

		var selected = SelectedPage?.PageTitle;
		Pages.Clear();
		foreach (var page in shop.Pages)
		{
			Pages.Add(new ShopPageViewModel(page));
		}
		_selectedPage = Pages.FirstOrDefault(p => p.PageTitle == selected) ?? Pages.FirstOrDefault();
		foreach (var page in Pages)
		{
			page.Refresh(_selectedPage);
		}
		ShowItems();
		Look();
		RaiseAll();
	}

	private void Changed()
	{
		if (Shop is not { } shop)
		{
			return;
		}
		foreach (var placement in shop.Placements)
		{
			_choices.Remember(placement);
		}
		_choices.Save(AppSettings.Folder);
		Look();
		OnPropertyChanged(nameof(TownsText));
	}

	private void Look()
	{
		if (_locations() is not { ServerRoot: { } server } || Shop is not { } shop)
		{
			return;
		}
		_status = ShopInstaller.Look(server, shop, (file, key) =>
			_serverSettings().FirstOrDefault(s => s.Definition.File == file && s.Definition.Key == key)?.Value);
		OnPropertyChanged(nameof(StatusText));
		OnPropertyChanged(nameof(Problems));
		OnPropertyChanged(nameof(HasProblems));
		OnPropertyChanged(nameof(Installed));
	}

	// ------------------------------------------------------------------ the shopkeeper

	public string ShopName
	{
		get => _choices.Name;
		set
		{
			if (_choices.Name != value)
			{
				_choices.Name = value;
				_choices.Save(AppSettings.Folder);
				OnPropertyChanged();
				Look();
			}
		}
	}

	public string ShopTitle
	{
		get => _choices.Title;
		set
		{
			if (_choices.Title != value)
			{
				_choices.Title = value;
				_choices.Save(AppSettings.Folder);
				OnPropertyChanged();
				Look();
			}
		}
	}

	public bool Enabled
	{
		get => _choices.Enabled;
		set
		{
			if (_choices.Enabled != value)
			{
				_choices.Enabled = value;
				_choices.Save(AppSettings.Folder);
				OnPropertyChanged();
				Look();
			}
		}
	}

	/// <summary>The npc the client already knows how to draw, which is what the shopkeeper looks like.</summary>
	public string LookId
	{
		get => _choices.DisplayId.ToString(CultureInfo.InvariantCulture);
		set
		{
			if (int.TryParse(value, out var id) && id > 0 && id != _choices.DisplayId)
			{
				_choices.DisplayId = id;
				_choices.Save(AppSettings.Folder);
				OnPropertyChanged();
				Look();
			}
		}
	}

	// ------------------------------------------------------------------ towns

	public ObservableCollection<ShopTownViewModel> Towns { get; } = [];

	public string TownsText => Shop is { } shop
		? $"{shop.Towns.Count()} of {shop.Placements.Count} places"
		: "";

	// ------------------------------------------------------------------ pages and prices

	public ObservableCollection<ShopPageViewModel> Pages { get; } = [];

	public ObservableCollection<ShopLineViewModel> Items { get; } = [];

	public ShopPageViewModel? SelectedPage
	{
		get => _selectedPage;
		set
		{
			if (Set(ref _selectedPage, value))
			{
				foreach (var page in Pages)
				{
					page.Refresh(value);
				}
				ShowItems();
			}
		}
	}

	public string ItemSearch
	{
		get => _itemSearch;
		set
		{
			if (Set(ref _itemSearch, value))
			{
				ShowItems();
			}
		}
	}

	private void ShowItems()
	{
		Items.Clear();
		if (Shop?.Pages.FirstOrDefault(p => p.Title == SelectedPage?.PageTitle) is not { } page || _items is null)
		{
			OnPropertyChanged(nameof(ItemsText));
			return;
		}
		var search = _itemSearch.Trim();
		foreach (var line in Lines(page))
		{
			if (search.Length == 0 || line.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
			{
				Items.Add(line);
			}
		}
		OnPropertyChanged(nameof(ItemsText));
	}

	private IEnumerable<ShopLineViewModel> Lines(ShopPage page)
	{
		if (page.Kind == ShopPageKind.Buy)
		{
			foreach (var item in page.Items)
			{
				var template = _items!.Find(item.ItemId);
				yield return new ShopLineViewModel(page.Title, item.ItemId, template?.Name ?? $"Item {item.ItemId}",
					template?.Description ?? "", item.Price, template?.Price ?? item.Price,
					ItemIcon.For(_icons, template?.Icon), Edited);
			}
			yield break;
		}
		foreach (var bundle in page.Bundles)
		{
			var first = bundle.Items.Count > 0 ? _items!.Find(bundle.Items[0].ItemId) : null;
			yield return new ShopLineViewModel(page.Title, bundle.Items.Count > 0 ? bundle.Items[0].ItemId : 0,
				bundle.Name, $"{bundle.Items.Count} pieces in one purchase", bundle.Price, bundle.Price,
				ItemIcon.For(_icons, first?.Icon), Edited);
		}
	}

	private void Edited(ShopLineViewModel line)
	{
		_choices.RememberPrice(line.PageTitle, line.ItemId, line.Price, line.Original);
		_choices.RememberRemoved(line.PageTitle, line.ItemId, !line.Included);
		_choices.Save(AppSettings.Folder);
		foreach (var page in Pages)
		{
			if (page.PageTitle == line.PageTitle && Shop?.Pages.FirstOrDefault(p => p.Title == line.PageTitle) is { } fresh)
			{
				page.Recount(fresh);
			}
		}
		Look();
	}

	public string ItemsText => SelectedPage is null ? "" : $"{Items.Count} of {SelectedPage.Count}";

	// ------------------------------------------------------------------ what the world holds

	public bool Installed => _status is not null && _status.State != ShopState.NotInstalled;

	public bool HasProblems => _status is { Problems.Count: > 0 };

	public IReadOnlyList<string> Problems => _status?.Problems ?? [];

	public string StatusText => _status?.State switch
	{
		null => "Choose your server folder in the Server tab first.",
		ShopState.NotInstalled => "Not in your world yet. Nothing has been written.",
		ShopState.Installed when _status.SpawnsEnabled =>
			"In your world and switched on. Restart your world from the launcher if you have just changed something.",
		ShopState.Installed => "In your world, but switched off: the files are there and no shopkeeper is spawned.",
		_ => "In your world, but not what is set here — an update or a hand edit changed something. Write it again to put it back.",
	};

	public bool Busy
	{
		get => _busy;
		private set => Set(ref _busy, value);
	}

	public string? Message
	{
		get => _message;
		private set => Set(ref _message, value);
	}

	private bool CanWrite => HasServer && Shop is not null && !Busy;

	public RelayCommand InstallCommand { get; }
	public RelayCommand RemoveCommand { get; }
	public RelayCommand SelectPageCommand { get; }
	public RelayCommand ShowSettingCommand { get; }
	public RelayCommand ExportCommand { get; }

	private void Write(bool install)
	{
		if (_locations() is not { ServerRoot: { } server } || Shop is not { } shop)
		{
			return;
		}
		if (!install && !_dialogs.Confirm("Take the shop out?",
			"Every file this app wrote for the shop is backed up and then deleted. Nothing else in your world is touched, "
			+ "and the shopkeepers are gone once you restart your world.", "Take it out"))
		{
			return;
		}
		var backup = new BackupSession(BackupSession.DefaultRoot, DateTime.Now, BackupKind.Settings,
			install ? "gm-shop-written" : "gm-shop-removed");
		try
		{
			if (install)
			{
				ShopInstaller.Install(server, shop, backup);
				Message = $"Written. Restart your world from the launcher and the shopkeeper will be standing in {shop.Towns.Count()} place(s).";
			}
			else
			{
				ShopInstaller.Remove(server, shop, backup);
				Message = "Removed. Restart your world from the launcher and the shopkeeper will be gone.";
			}
		}
		catch (IOException problem)
		{
			Message = $"Could not write to your world's folder: {problem.Message}";
		}
		catch (UnauthorizedAccessException problem)
		{
			Message = $"Could not write to your world's folder: {problem.Message}";
		}
		Look();
	}

	/// <summary>Writes the same files into a folder of the player's choosing, to install by hand or keep.</summary>
	private void Export()
	{
		if (Shop is not { } shop)
		{
			return;
		}
		if (_dialogs.PickFolder("Where should the shop's files go?", null) is not { } folder)
		{
			return;
		}
		foreach (var file in ShopFiles.Build(shop))
		{
			var path = Path.Combine(folder, file.RelativePath.Replace('/', Path.DirectorySeparatorChar));
			Directory.CreateDirectory(Path.GetDirectoryName(path)!);
			File.WriteAllText(path, file.Content);
		}
		Message = $"A copy of the shop's files is in {folder}. They go under game\\data in a world.";
	}

	private void RaiseAll()
	{
		OnPropertyChanged(nameof(Shop));
		OnPropertyChanged(nameof(ShopName));
		OnPropertyChanged(nameof(ShopTitle));
		OnPropertyChanged(nameof(LookId));
		OnPropertyChanged(nameof(Enabled));
		OnPropertyChanged(nameof(TownsText));
		OnPropertyChanged(nameof(SelectedPage));
		OnPropertyChanged(nameof(HasServer));
	}
}

/// <summary>One town on the list: whether a shopkeeper stands there, and how far it has been nudged.</summary>
public sealed class ShopTownViewModel(ShopPlacement placement, Action changed) : ObservableObject
{
	public string Where => placement.Beside.Where;

	public string Position => $"{placement.X:N0}, {placement.Y:N0}";

	public bool IsMainTown => placement.Beside.IsMainTown;

	public bool Enabled
	{
		get => placement.Enabled;
		set
		{
			if (placement.Enabled != value)
			{
				placement = placement with { Enabled = value };
				OnPropertyChanged();
				changed();
			}
		}
	}

	public string NudgeX
	{
		get => placement.OffsetX.ToString(CultureInfo.InvariantCulture);
		set => Nudge(value, isX: true);
	}

	public string NudgeY
	{
		get => placement.OffsetY.ToString(CultureInfo.InvariantCulture);
		set => Nudge(value, isX: false);
	}

	private void Nudge(string text, bool isX)
	{
		if (!int.TryParse(text, out var value))
		{
			return;
		}
		value = Math.Clamp(value, -2000, 2000);
		placement = isX ? placement with { OffsetX = value } : placement with { OffsetY = value };
		OnPropertyChanged(isX ? nameof(NudgeX) : nameof(NudgeY));
		OnPropertyChanged(nameof(Position));
		changed();
	}

	/// <summary>The placement as it stands, so the view model can hand it back to the choices file.</summary>
	public ShopPlacement Placement => placement;
}

/// <summary>One button on the shopkeeper's first screen.</summary>
public sealed class ShopPageViewModel(ShopPage page) : ObservableObject
{
	public string PageTitle { get; } = page.Title;

	public int Count { get; private set; } = page.Count;

	public string Kind { get; } = page.Kind == ShopPageKind.Buy ? "buy" : "exchange";

	public string Summary => $"{Count} {(Kind == "buy" ? "items" : "sets")}";

	public bool IsSelected { get; private set; }

	public void Refresh(ShopPageViewModel? selected)
	{
		IsSelected = ReferenceEquals(this, selected);
		OnPropertyChanged(nameof(IsSelected));
	}

	public void Recount(ShopPage fresh)
	{
		Count = fresh.Count;
		OnPropertyChanged(nameof(Count));
		OnPropertyChanged(nameof(Summary));
	}
}

/// <summary>One line of a page: an item (or a whole set), what it costs, and whether it is on sale at all.</summary>
public sealed class ShopLineViewModel : ObservableObject
{
	private readonly Action<ShopLineViewModel> _edited;
	private long _price;
	private bool _included = true;
	private string _priceText;

	public ShopLineViewModel(string pageTitle, int itemId, string name, string description, long price, long original,
		ImageSource? image, Action<ShopLineViewModel> edited)
	{
		PageTitle = pageTitle;
		ItemId = itemId;
		Name = name;
		Description = description;
		Original = original;
		Image = image;
		_price = price;
		_priceText = price.ToString("N0", CultureInfo.CurrentCulture);
		_edited = edited;
	}

	public string PageTitle { get; }
	public int ItemId { get; }
	public string Name { get; }
	public string Description { get; }
	public long Original { get; }
	public ImageSource? Image { get; }

	public long Price => _price;

	public bool IsChanged => _price != Original;

	public string OriginalText => $"worth {Original:N0}";

	public string PriceText
	{
		get => _priceText;
		set
		{
			if (_priceText == value)
			{
				return;
			}
			_priceText = value;
			OnPropertyChanged();
			if (long.TryParse(value, NumberStyles.AllowThousands | NumberStyles.Integer, CultureInfo.CurrentCulture, out var parsed)
				&& parsed >= 0)
			{
				_price = Math.Min(parsed, int.MaxValue);
				OnPropertyChanged(nameof(IsChanged));
				_edited(this);
			}
		}
	}

	public bool Included
	{
		get => _included;
		set
		{
			if (Set(ref _included, value))
			{
				_edited(this);
			}
		}
	}
}
