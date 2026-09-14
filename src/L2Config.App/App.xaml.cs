using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using L2Config.App.Infrastructure;
using L2Config.App.ViewModels;
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
		var settings = snapshot is null
			? AppSettings.Load()
			: new AppSettings { Transient = true, ServerFolder = snapshot.Server, ClientFolder = snapshot.Client, LastTab = snapshot.Tab, ShowAdvanced = snapshot.Advanced };

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
				if (snapshot.Tab == "characters")
				{
					// The character list loads from the database after the first render.
					var until = DateTime.Now.AddSeconds(5);
					while (viewModel.CharactersTab.IsLoading && DateTime.Now < until)
					{
						window.Dispatcher.Invoke(() => { }, DispatcherPriority.Background);
						Thread.Sleep(50);
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
	/// [--category id] [--group id] [--advanced] [--size 1280x820]. Renders the window to a PNG and exits. Never saves settings.
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
			if (Tab == "characters")
			{
				viewModel.SelectedTab = viewModel.CharactersTab;
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
			if (Search is not null)
			{
				tab.SearchText = Search;
			}
			// --edit key=value shows an unsaved edit (in memory only).
			if (Edit?.Split('=', 2) is [var key, var value] && tab.Settings.FirstOrDefault(s => s.Definition.Key == key) is { } setting)
			{
				setting.Value = value;
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
