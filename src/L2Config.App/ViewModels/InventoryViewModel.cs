using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using L2Config.App.Infrastructure;
using L2Config.Core.Backups;
using L2Config.Core.Characters;

namespace L2Config.App.ViewModels;

/// <summary>One character's inventory: change or remove existing items (offline only) and queue new items for the server to deliver.</summary>
public sealed class InventoryViewModel : ObservableObject
{
	private const int ResultLimit = 60;
	private readonly WorldDatabase _database;
	private readonly ServerFacts _facts;
	private readonly CharactersViewModel _owner;
	private readonly IDialogs _dialogs;
	private readonly Action<string> _showSetting;
	private PlayerCharacter _character;
	private ItemCatalog? _items;
	private IReadOnlyList<InventoryItem> _inventory = [];
	private IReadOnlyList<PendingDelivery> _pending = [];
	private string _searchText = "";
	private ItemTemplate? _selected;
	private string _amountText = "1";
	private string _enchantText = "0";
	private string? _message;
	private bool _messageIsError;
	private string? _problem;
	private bool _isBusy;

	public InventoryViewModel(PlayerCharacter character, WorldDatabase database, ServerFacts facts, CharactersViewModel owner, IDialogs dialogs,
		Action<string> showSetting, RelayCommand closeCommand)
	{
		_character = character;
		_database = database;
		_facts = facts;
		_owner = owner;
		_dialogs = dialogs;
		_showSetting = showSetting;
		CloseCommand = closeCommand;
		RefreshCommand = new RelayCommand(() => _ = RefreshAsync(), () => !IsBusy);
		AddCommand = new RelayCommand(() => _ = AddAsync(), () => AddProblem is null && !IsBusy);
		SelectResultCommand = new RelayCommand(p => { if (p is ItemTemplate t) Selected = t; });
		ShowDeliverySettingCommand = new RelayCommand(() => { if (facts.DeliverySettingId is { } id) showSetting(id); }, () => facts.DeliverySettingId is not null);
	}

	public RelayCommand CloseCommand { get; }
	public RelayCommand RefreshCommand { get; }
	public RelayCommand AddCommand { get; }
	public RelayCommand SelectResultCommand { get; }
	public RelayCommand ShowDeliverySettingCommand { get; }

	public string Name => _character.Name;
	public string StatusWord => _character.OnlineState switch { 0 => "Offline", 2 => "Offline shop", _ => "Online" };
	public bool IsOnline => _character.IsOnline;
	public int Limit => _facts.Limits.For(_character);
	public long MaxAdena => _facts.MaxAdena;

	public ObservableCollection<InventoryRowViewModel> Items { get; } = [];
	public ObservableCollection<DeliveryRowViewModel> Deliveries { get; } = [];
	public ObservableCollection<ItemTemplate> Results { get; } = [];
	public bool HasDeliveries => Deliveries.Count > 0;

	public int SlotsUsed => _inventory.Count;
	public int SlotsIncludingPending => _items is null ? _inventory.Count : InventorySlots.UsedIncludingPending(_inventory.ToList(), _pending.SelectMany(p => p.Items), _items);

	public string SlotsText =>
		$"{SlotsUsed:N0} of {Limit:N0} slots used" + (SlotsIncludingPending > SlotsUsed ? $"  ·  {SlotsIncludingPending:N0} once waiting deliveries arrive" : "");

	public string LimitSourceText => $"The limit comes from {_facts.Limits.DescribeFor(_character)}. Skills that expand the inventory can add a little more in game.";

	public string EditLockText => IsOnline
		? "This character is logged in, so its items are read-only here: the server keeps a logged-in character's inventory in memory and would overwrite any change. Log out to change or remove items."
		: "Changes to existing items are written to the database immediately. The rows are backed up first.";

	/// <summary>The plain explanation of how new items get into an inventory, with the live state of the setting it needs.</summary>
	public string DeliveryExplanation =>
		"New items can't be written straight into the database while the world is running — the server assigns every item an ID " +
		"and could give the same ID to another item. So this app puts new items in a queue, and the SERVER adds them to the " +
		$"character the next time the character is logged in (it checks the queue every {_facts.DeliveryDelaySeconds} seconds).";

	public string DeliverySettingText => _facts.DeliveryEnabledInFile switch
	{
		true => "“Deliver items queued from the database” is On in your settings. If you only just turned it on, restart the world from the launcher before logging in — the server reads it at start-up.",
		false => "“Deliver items queued from the database” is OFF, so queued items will wait and never arrive. Turn it on in the Server tab, save, then restart the world from the launcher.",
		null => "Could not read “Deliver items queued from the database” (Custom/CustomMailManager.ini). Queued items only arrive while it is on.",
	};

	public bool DeliveryDisabled => _facts.DeliveryEnabledInFile != true;

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

	public bool IsBusy
	{
		get => _isBusy;
		private set => Set(ref _isBusy, value);
	}

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

	// ------------------------------------------------------------------ add item

	public string SearchText
	{
		get => _searchText;
		set
		{
			if (Set(ref _searchText, value ?? ""))
			{
				UpdateResults();
			}
		}
	}

	public string ResultsText => _items is null ? "Loading the item list…"
		: SearchText.Trim().Length == 0 ? $"{_items.Count:N0} items. Type a name or item ID."
		: Results.Count == 0 ? "No item matches."
		: Results.Count >= ResultLimit ? $"Showing the first {ResultLimit}. Type more to narrow it down."
		: $"{Results.Count} matching";

	public ItemTemplate? Selected
	{
		get => _selected;
		set
		{
			if (Set(ref _selected, value))
			{
				if (value?.Stackable == true)
				{
					EnchantText = "0";
				}
				RaiseAddState();
			}
		}
	}

	public bool HasSelection => Selected is not null;
	public bool CanEnchant => Selected is { Stackable: false, Type: "Weapon" or "Armor" };

	public string AmountText
	{
		get => _amountText;
		set
		{
			if (Set(ref _amountText, value ?? ""))
			{
				RaiseAddState();
			}
		}
	}

	public string EnchantText
	{
		get => _enchantText;
		set
		{
			if (Set(ref _enchantText, value ?? ""))
			{
				RaiseAddState();
			}
		}
	}

	/// <summary>True when the selected stackable item is already carried: the amount is added to that stack immediately instead of queued.</summary>
	public bool AddsToExistingStack => Selected is { Stackable: true } s && ExistingStack(s.Id) is not null;

	public string AddButtonText => AddsToExistingStack ? "Add to the stack now" : "Queue for delivery";

	/// <summary>What pressing the add button will do, in plain words, including the slot check.</summary>
	public string? AddPlan
	{
		get
		{
			if (Selected is not { } item || _items is null || !TryAmount(out var amount) || AddProblem is not null)
			{
				return null;
			}
			if (AddsToExistingStack)
			{
				var stack = ExistingStack(item.Id)!;
				return $"Adds {amount:N0} to the {stack.Count:N0} {item.Name} this character already carries, right now (no new slot needed).";
			}
			var needed = SlotsNeeded(item, amount);
			return $"Needs {needed:N0} more slot{(needed == 1 ? "" : "s")}: {SlotsIncludingPending + needed:N0} of {Limit:N0} once delivered. " +
				$"The server adds it the next time {Name} logs in.";
		}
	}

	/// <summary>Why the add button is disabled, or null when adding is allowed.</summary>
	public string? AddProblem
	{
		get
		{
			if (_items is null)
			{
				return "Loading…";
			}
			if (Selected is not { } item)
			{
				return "Pick an item from the list.";
			}
			if (IsOnline)
			{
				return "Log the character out first, so the inventory count is exact.";
			}
			if (!TryAmount(out var amount))
			{
				return $"Enter an amount from 1 to {MaxAmountFor(item):N0}.";
			}
			if (!int.TryParse(EnchantText.Trim(), out var enchant) || enchant < 0 || enchant > 65535 || (!CanEnchant && enchant != 0))
			{
				return CanEnchant ? "Enter an enchant level from 0 to 65535." : "This item can't be enchanted.";
			}
			var current = CarriedCount(item.Id);
			var cap = item.Id == WorldDatabase.AdenaItemId ? _facts.MaxAdena : InventoryLimits.MaxStackCount;
			if (item.Stackable && current + amount > cap)
			{
				return item.Id == WorldDatabase.AdenaItemId
					? $"That would make {current + amount:N0} adena; the most a character may hold is {cap:N0} (Player.ini › MaxAdena)."
					: $"That would make a stack of {current + amount:N0}; the most one stack can hold is {cap:N0}.";
			}
			if (!AddsToExistingStack)
			{
				var needed = SlotsNeeded(item, amount);
				if (SlotsIncludingPending + needed > Limit)
				{
					return $"Not enough room: {SlotsIncludingPending + needed:N0} slots would be needed and the limit is {Limit:N0}. " +
						"Remove items, or raise the inventory limit in the Server tab.";
				}
			}
			return null;
		}
	}

	public async Task RefreshAsync()
	{
		IsBusy = true;
		try
		{
			_items ??= await _owner.ItemsAsync(_facts.Locations.ServerRoot!);
			var characters = await _database.ListPlayerCharactersAsync();
			var fresh = characters.FirstOrDefault(c => c.CharId == _character.CharId);
			if (fresh is null)
			{
				Problem = "This character no longer exists.";
				return;
			}
			_character = fresh;
			_inventory = await _database.ListInventoryAsync(_character.CharId);
			_pending = await _database.ListDeliveriesAsync(_character.CharId);
			Problem = null;

			Items.Clear();
			foreach (var item in _inventory)
			{
				Items.Add(new InventoryRowViewModel(item, _items.Find(item.ItemId), this));
			}
			Deliveries.Clear();
			foreach (var delivery in _pending)
			{
				Deliveries.Add(new DeliveryRowViewModel(delivery, _items, this));
			}
			UpdateResults();
		}
		catch (WorldDatabaseException ex)
		{
			Problem = ex.Message;
		}
		catch (Exception ex)
		{
			// Never leave the screen saying "Loading…" forever: say what went wrong.
			Problem = $"The inventory could not be loaded: {ex.Message}";
		}
		finally
		{
			IsBusy = false;
			OnPropertyChanged(string.Empty);
		}
	}

	internal async Task SetCountAsync(InventoryRowViewModel row, long newCount)
	{
		if (newCount == 0 && !_dialogs.Confirm("Remove item", $"Remove {row.DisplayName} from {Name}'s inventory? The row is backed up first and can be restored from the Backups tab while {Name} is logged out.", "Remove"))
		{
			return;
		}
		await RunAsync($"{Name} {row.DisplayName}", backup => _database.SetItemCountAsync(_character, row.Item, newCount, row.DisplayName, backup));
	}

	internal async Task CancelDeliveryAsync(DeliveryRowViewModel row)
	{
		await RunAsync($"{Name} cancel delivery", backup => _database.CancelDeliveryAsync(_character, row.Delivery, backup));
	}

	private async Task AddAsync()
	{
		if (Selected is not { } item || !TryAmount(out var amount))
		{
			return;
		}
		var enchant = int.Parse(EnchantText.Trim(), CultureInfo.InvariantCulture);
		if (AddsToExistingStack)
		{
			var stack = ExistingStack(item.Id)!;
			await RunAsync($"{Name} {item.Name}", backup => _database.SetItemCountAsync(_character, stack, stack.Count + amount, item.Name, backup));
		}
		else
		{
			await RunAsync($"{Name} deliver {item.Name}", backup => _database.QueueDeliveryAsync(_character, item, amount, enchant, _facts.Limits, _items!, _facts.MaxAdena, backup));
		}
	}

	private async Task RunAsync(string backupTitle, Func<BackupSession, Task<EditOutcome>> action)
	{
		IsBusy = true;
		try
		{
			var backup = new BackupSession(BackupSession.DefaultRoot, DateTime.Now, BackupKind.Characters, backupTitle);
			var outcome = await action(backup);
			MessageIsError = !outcome.Ok;
			Message = outcome.Ok ? outcome.Message : "Not changed: " + outcome.Message;
		}
		catch (Exception ex) when (ex is WorldDatabaseException or MySqlConnector.MySqlException or IOException)
		{
			MessageIsError = true;
			Message = "Not changed: " + ex.Message;
		}
		finally
		{
			IsBusy = false;
		}
		await RefreshAsync();
	}

	private void UpdateResults()
	{
		Results.Clear();
		var query = SearchText.Trim();
		if (_items is not null && query.Length > 0)
		{
			var byId = int.TryParse(query, out var id) ? _items.Find(id) : null;
			if (byId is not null)
			{
				Results.Add(byId);
			}
			foreach (var item in _items.All.Where(i => i != byId && i.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
				.OrderBy(i => !i.Name.StartsWith(query, StringComparison.OrdinalIgnoreCase)).ThenBy(i => i.Name.Length).Take(ResultLimit))
			{
				Results.Add(item);
			}
		}
		OnPropertyChanged(nameof(ResultsText));
	}

	private void RaiseAddState()
	{
		OnPropertyChanged(nameof(HasSelection));
		OnPropertyChanged(nameof(CanEnchant));
		OnPropertyChanged(nameof(AddsToExistingStack));
		OnPropertyChanged(nameof(AddButtonText));
		OnPropertyChanged(nameof(AddPlan));
		OnPropertyChanged(nameof(AddProblem));
	}

	private bool TryAmount(out long amount) =>
		long.TryParse(AmountText.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out amount) && amount >= 1
		&& Selected is { } item && amount <= MaxAmountFor(item);

	private long MaxAmountFor(ItemTemplate item) =>
		item.Stackable ? (item.Id == WorldDatabase.AdenaItemId ? _facts.MaxAdena : InventoryLimits.MaxStackCount) : Math.Max(1, Limit);

	private InventoryItem? ExistingStack(int itemId) => IsOnline ? null : _inventory.FirstOrDefault(i => i.ItemId == itemId);

	private long CarriedCount(int itemId) =>
		_inventory.Where(i => i.ItemId == itemId).Sum(i => i.Count) + _pending.SelectMany(p => p.Items).Where(p => p.ItemId == itemId).Sum(p => p.Count);

	private int SlotsNeeded(ItemTemplate item, long amount)
	{
		var carried = _inventory.Select(i => i.ItemId).Concat(_pending.SelectMany(p => p.Items).Select(p => p.ItemId)).ToHashSet();
		return InventorySlots.SlotsFor(item.Id, amount, carried, _items!);
	}
}

public sealed class InventoryRowViewModel : ObservableObject
{
	private readonly InventoryViewModel _owner;
	private string _countText;

	public InventoryRowViewModel(InventoryItem item, ItemTemplate? template, InventoryViewModel owner)
	{
		Item = item;
		Template = template;
		_owner = owner;
		_countText = item.Count.ToString(CultureInfo.InvariantCulture);
		ApplyCommand = new RelayCommand(() => _ = owner.SetCountAsync(this, long.Parse(_countText.Trim(), CultureInfo.InvariantCulture)), () => CanApply);
		RemoveCommand = new RelayCommand(() => _ = owner.SetCountAsync(this, 0), () => CanEdit);
	}

	public InventoryItem Item { get; }
	public ItemTemplate? Template { get; }
	public string DisplayName => (Template?.Name ?? $"Unknown item {Item.ItemId}") + (Item.Enchant > 0 ? $" +{Item.Enchant}" : "");
	public string Details => $"Item ID {Item.ItemId}" + (Template is null ? "" : "  ·  " + Template.Description) + (Item.Equipped ? "  ·  equipped" : "");
	public bool IsStackable => Template?.Stackable == true;
	public string CountDisplay => IsStackable ? $"{Item.Count:N0}" : "1 slot";
	public bool CanEdit => !_owner.IsOnline && !_owner.IsBusy;
	public bool CanEditCount => CanEdit && IsStackable;

	public string CountText
	{
		get => _countText;
		set
		{
			if (Set(ref _countText, value ?? ""))
			{
				OnPropertyChanged(nameof(CountError));
			}
		}
	}

	public string? CountError =>
		!long.TryParse(_countText.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out var n) || n < 1 ? "Enter 1 or more (use Remove to take it away)."
		: Item.ItemId == WorldDatabase.AdenaItemId && n > _owner.MaxAdena ? $"At most {_owner.MaxAdena:N0} adena (Player.ini › MaxAdena)."
		: n > InventoryLimits.MaxStackCount ? $"At most {InventoryLimits.MaxStackCount:N0}."
		: null;

	public bool CanApply => CanEditCount && CountError is null && long.Parse(_countText.Trim(), CultureInfo.InvariantCulture) != Item.Count;

	public RelayCommand ApplyCommand { get; }
	public RelayCommand RemoveCommand { get; }
}

public sealed class DeliveryRowViewModel
{
	public DeliveryRowViewModel(PendingDelivery delivery, ItemCatalog items, InventoryViewModel owner)
	{
		Delivery = delivery;
		Text = string.Join(", ", delivery.Items.Select(i => $"{i.Count:N0} × {items.Find(i.ItemId)?.Name ?? $"item {i.ItemId}"}{(i.Enchant > 0 ? $" +{i.Enchant}" : "")}"));
		When = $"queued {delivery.Date}" + (delivery.Subject == WorldDatabase.DeliverySubject ? "" : $"  ·  {delivery.Subject}");
		CancelCommand = new RelayCommand(() => _ = owner.CancelDeliveryAsync(this));
	}

	public PendingDelivery Delivery { get; }
	public string Text { get; }
	public string When { get; }
	public RelayCommand CancelCommand { get; }
}
