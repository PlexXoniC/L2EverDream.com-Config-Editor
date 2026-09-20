using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using L2Config.App.Infrastructure;
using L2Config.App.ViewModels;
using L2Config.Core.Backups;
using L2Config.Core.Catalog;

namespace L2Config.App;

public partial class App : Application
{
	protected override void OnStartup(StartupEventArgs e)
	{
		base.OnStartup(e);

		SettingsCatalog catalog;
		try
		{
			using var catalogStream = OpenEmbedded("catalog.json") ?? throw new IOException("catalog.json is not embedded in the app.");
			catalog = CatalogLoader.Load(catalogStream);
		}
		catch (Exception ex) when (ex is IOException or System.Text.Json.JsonException or InvalidDataException)
		{
			MessageBox.Show($"The settings catalog could not be loaded.\n\n{ex.Message}", "L2Everdream Config", MessageBoxButton.OK, MessageBoxImage.Error);
			Shutdown(1);
			return;
		}

		var snapshot = SnapshotOptions.Parse(e.Args);
		if (snapshot?.Backups is { } backupsRoot)
		{
			BackupSession.DefaultRoot = backupsRoot;
		}
		var settings = snapshot is null
			? AppSettings.Load()
			: new AppSettings
			{
				Transient = true, ServerFolder = snapshot.Server, ClientFolder = snapshot.Client, LastTab = snapshot.Tab, ShowAdvanced = snapshot.Advanced,
				FullBackupFolder = snapshot.FullBackups,
			};

		CustomConfigReport? customConfig;
		using (var customStream = OpenEmbedded("custom-config.json"))
		{
			customConfig = customStream is null ? null : CustomConfigReport.Load(customStream);
		}
		var viewModel = new MainViewModel(catalog, customConfig, settings, new DialogService());
		var window = new MainWindow(viewModel);
		MainWindow = window;

		if (snapshot is not null)
		{
			window.Width = snapshot.Width;
			window.Height = snapshot.Height;
			window.WindowStartupLocation = WindowStartupLocation.Manual;
			window.Left = -10000;
			window.ShowInTaskbar = false;
			snapshot.Apply(viewModel);
			window.ContentRendered += (_, _) => window.Dispatcher.BeginInvoke(() =>
			{
				var until = DateTime.Now.AddSeconds(5);
				void Pump(Func<bool> busy)
				{
					while (busy() && DateTime.Now < until)
					{
						// A nested frame processes queued work. This callback runs at ApplicationIdle, so async continuations
						// are posted at that priority too: exit the frame only at SystemIdle, after they have run.
						var frame = new DispatcherFrame();
						window.Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, () => frame.Continue = false);
						Dispatcher.PushFrame(frame);
						Thread.Sleep(20);
					}
				}
				if (snapshot.Tab == "backups" && snapshot.Compare is { } compare)
				{
					// --compare <backup folder name | latest> opens the comparison checklist (nothing is restored).
					until = DateTime.Now.AddSeconds(15);
					var task = viewModel.BackupsTab.FullBackups.CompareByNameAsync(compare);
					Pump(() => !task.IsCompleted);
					if (viewModel.BackupsTab.FullBackups.Comparison is { } comparison)
					{
						comparison.Filter = snapshot.Filter ?? comparison.Filter;
						comparison.SearchText = snapshot.Search ?? "";
					}
					window.UpdateLayout();
				}
				if (snapshot.SkillDurations && viewModel.ServerTab.Page is SkillDurationsViewModel skills)
				{
					// --skill-durations opens the skill durations page (with --filter and --search; nothing is saved).
					until = DateTime.Now.AddSeconds(15);
					Pump(() => skills.IsLoading);
					skills.Filter = snapshot.Filter ?? skills.Filter;
					skills.SearchText = snapshot.Search ?? "";
					window.UpdateLayout();
				}
				if (snapshot.Tab is "rates" or "drops")
				{
					// --rate / --delivery set the rate on the Rates tab; on the Drops tab --monster picks a monster and
					// --view retail-now | now-planned | retail-planned picks which two sets of rates are compared.
					until = DateTime.Now.AddSeconds(20);
					var rates = viewModel.RatesTab;
					if (snapshot.Rate is { } rate)
					{
						rates.Rate = rate;
					}
					if (snapshot.Delivery is { } delivery)
					{
						rates.Delivery = delivery;
					}
					if (snapshot.Tab == "drops")
					{
						var drops = viewModel.DropsTab;
						Pump(() => drops.IsLoading);
						if (snapshot.View is { } view)
						{
							drops.Comparison = view switch
							{
								"now-planned" => DropComparison.NowToPlanned,
								"retail-planned" => DropComparison.RetailToPlanned,
								_ => DropComparison.RetailToNow,
							};
						}
						if (snapshot.Monster is { } monster)
						{
							drops.SearchText = monster;
							drops.Selected = drops.Monsters.FirstOrDefault(m => string.Equals(m.Name, monster, StringComparison.OrdinalIgnoreCase))
								?? drops.Monsters.FirstOrDefault();
						}
						Pump(() => !drops.HasPreview);
					}
					window.UpdateLayout();
				}
				if (snapshot.Tab == "characters")
				{
					// The character list loads from the database after the first render.
					Pump(() => viewModel.CharactersTab.IsLoading);
					// --inventory <character> opens the inventory editor; --item-search / --item / --amount fill in the add panel (nothing is applied).
					if (snapshot.Inventory is { } who && viewModel.CharactersTab.Rows.FirstOrDefault(r => r.Name == who) is { } row)
					{
						until = DateTime.Now.AddSeconds(15);
						row.OpenInventoryCommand.Execute(null);
						Pump(() => viewModel.CharactersTab.Inventory is not { IsBusy: false } inv || inv.Items.Count == 0);
						var inventory = viewModel.CharactersTab.Inventory!;
						if (snapshot.ItemSearch is not null)
						{
							inventory.SearchText = snapshot.ItemSearch;
						}
						if (snapshot.Item is { } id)
						{
							inventory.Selected = inventory.Results.FirstOrDefault(i => i.Id == id);
						}
						if (snapshot.Amount is not null)
						{
							inventory.AmountText = snapshot.Amount;
						}
					}
					window.UpdateLayout();
				}
				snapshot.Save(window);
				Shutdown(0);
			}, DispatcherPriority.ApplicationIdle);
		}
		window.Show();
	}

	/// <summary>The catalog and Custom Config report are embedded in the exe, so it runs standalone.</summary>
	private static Stream? OpenEmbedded(string name) => typeof(App).Assembly.GetManifestResourceStream(name);

	/// <summary>
	/// Developer aid: L2EverdreamConfig.exe --snapshot out.png [--server dir] [--client dir] [--tab client] [--search text]
	/// [--category id] [--group id] [--advanced] [--size 1280x820] [--backups dir] [--backups-mode full] [--full-backups dir]
	/// [--compare folder|latest] [--filter changed|shipped|new|all] [--skill-durations] [--tab rates --rate 5 --delivery 1]
	/// [--tab drops --monster name --view retail-now|now-planned|retail-planned]. Renders the window to a PNG and exits. Never saves settings.
	/// </summary>
	private sealed class SnapshotOptions
	{
		public required string Output { get; init; }
		public string? Server { get; private set; }
		public string? Client { get; private set; }
		public string Tab { get; private set; } = "server";
		public string? Search { get; private set; }
		public string? Category { get; private set; }
		public string? Group { get; private set; }
		public bool Advanced { get; private set; }
		public int Width { get; private set; } = 1280;
		public int Height { get; private set; } = 820;
		public string? Edit { get; private set; }
		public string? Inventory { get; private set; }
		public string? ItemSearch { get; private set; }
		public int? Item { get; private set; }
		public string? Amount { get; private set; }
		public string? Backups { get; private set; }
		public string? FullBackups { get; private set; }
		public string? BackupsMode { get; private set; }
		public string? Compare { get; private set; }
		public string? Filter { get; private set; }
		public bool SkillDurations { get; private set; }
		public double? Rate { get; private set; }
		public double? Delivery { get; private set; }
		public string? Monster { get; private set; }
		public string? View { get; private set; }

		public static SnapshotOptions? Parse(string[] args)
		{
			var i = Array.IndexOf(args, "--snapshot");
			if (i < 0 || i + 1 >= args.Length)
			{
				return null;
			}
			var options = new SnapshotOptions { Output = args[i + 1] };
			string? Next(string flag)
			{
				var at = Array.IndexOf(args, flag);
				return at >= 0 && at + 1 < args.Length ? args[at + 1] : null;
			}
			options.Server = Next("--server");
			options.Client = Next("--client");
			options.Tab = Next("--tab") ?? "server";
			options.Search = Next("--search");
			options.Category = Next("--category");
			options.Group = Next("--group");
			options.Edit = Next("--edit");
			options.Inventory = Next("--inventory");
			options.ItemSearch = Next("--item-search");
			options.Item = int.TryParse(Next("--item"), out var itemId) ? itemId : null;
			options.Amount = Next("--amount");
			options.Backups = Next("--backups");
			options.FullBackups = Next("--full-backups");
			options.BackupsMode = Next("--backups-mode");
			options.Compare = Next("--compare");
			options.Filter = Next("--filter");
			options.SkillDurations = args.Contains("--skill-durations");
			options.Rate = double.TryParse(Next("--rate"), System.Globalization.CultureInfo.InvariantCulture, out var rateValue) ? rateValue : null;
			options.Delivery = double.TryParse(Next("--delivery"), System.Globalization.CultureInfo.InvariantCulture, out var deliveryValue) ? deliveryValue : null;
			options.Monster = Next("--monster");
			options.View = Next("--view");
			options.Advanced = args.Contains("--advanced");
			if (Next("--size") is { } size && size.Split('x') is [var w, var h])
			{
				options.Width = int.Parse(w);
				options.Height = int.Parse(h);
			}
			return options;
		}

		public void Apply(MainViewModel viewModel)
		{
			if (Tab is "rates" or "drops")
			{
				viewModel.SelectedTab = Tab == "drops" ? viewModel.DropsTab : viewModel.RatesTab;
				return;
			}
			if (Tab is "characters" or "backups")
			{
				viewModel.SelectedTab = Tab == "backups" ? viewModel.BackupsTab : viewModel.CharactersTab;
				viewModel.BackupsTab.IsFullMode = BackupsMode == "full" || Compare is not null;
				return;
			}
			if (Tab == "custom" && viewModel.CustomTab is not null)
			{
				viewModel.SelectedTab = viewModel.CustomTab;
				return;
			}
			var tab = Tab == "client" ? viewModel.ClientTab : viewModel.ServerTab;
			viewModel.SelectedTab = tab;
			if (Category is not null && tab.AllCategories.FirstOrDefault(c => c.Id == Category) is { } category)
			{
				tab.SelectCategory(category);
			}
			if (Search is not null && !SkillDurations)
			{
				tab.SearchText = Search;
			}
			// --edit key=value shows an unsaved edit (in memory only).
			if (Edit?.Split('=', 2) is [var key, var value] && tab.Settings.FirstOrDefault(s => s.Definition.Key == key) is { } setting)
			{
				setting.Value = value;
			}
			if (SkillDurations && tab.Settings.FirstOrDefault(s => s.Definition.Editor == SettingEditor.SkillDurations) is { } durations)
			{
				durations.OpenEditorCommand?.Execute(null);
			}
		}

		public void Save(Window window)
		{
			if (Group is not null && window.DataContext is MainViewModel vm
				&& vm.SelectedTab is TabViewModel selected
				&& selected.AllCategories.SelectMany(c => c.Groups).FirstOrDefault(g => g.Id == Group) is { } group)
			{
				selected.SelectGroup(group);
				window.UpdateLayout();
			}
			var content = (FrameworkElement)window.Content;
			content.UpdateLayout();
			var dpi = VisualTreeHelper.GetDpi(window);
			var bitmap = new RenderTargetBitmap(
				(int)(content.ActualWidth * dpi.DpiScaleX), (int)(content.ActualHeight * dpi.DpiScaleY),
				dpi.PixelsPerInchX, dpi.PixelsPerInchY, PixelFormats.Pbgra32);
			bitmap.Render(content);
			var encoder = new PngBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create(bitmap));
			using var stream = File.Create(Output);
			encoder.Save(stream);
		}
	}
}
