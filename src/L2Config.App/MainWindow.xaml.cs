using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using L2Config.App.ViewModels;

namespace L2Config.App;

public partial class MainWindow : Window
{
	private readonly MainViewModel _viewModel;

	public MainWindow(MainViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		DataContext = viewModel;
		viewModel.ServerTab.ScrollRequested += row => ScrollToRow(viewModel.ServerTab, row);
		viewModel.ClientTab.ScrollRequested += row => ScrollToRow(viewModel.ClientTab, row);
	}

	protected override void OnClosing(CancelEventArgs e)
	{
		if (_viewModel.PendingCount > 0 && !new DialogService().Confirm(
				"Unsaved changes",
				$"You have {_viewModel.PendingText.ToLowerInvariant()}. Close without saving?",
				"Close without saving"))
		{
			e.Cancel = true;
		}
		base.OnClosing(e);
	}

	/// <summary>Brings a group heading to the top of the settings list.</summary>
	private void ScrollToRow(TabViewModel tab, object row)
	{
		if (_viewModel.SelectedTab != tab)
		{
			return;
		}
		Dispatcher.BeginInvoke(() =>
		{
			if (FindChild<ListBox>(TabHost, "RowsList") is not { } list)
			{
				return;
			}
			list.UpdateLayout();
			var scroller = FindChild<ScrollViewer>(list, null);
			var index = tab.Rows.IndexOf(row);
			if (scroller is null || index < 0)
			{
				return;
			}
			if (index == 0)
			{
				scroller.ScrollToTop();
				return;
			}
			list.ScrollIntoView(row);
			list.UpdateLayout();
			if (list.ItemContainerGenerator.ContainerFromIndex(index) is FrameworkElement container)
			{
				var offset = container.TransformToAncestor(scroller).Transform(new Point(0, 0)).Y;
				scroller.ScrollToVerticalOffset(scroller.VerticalOffset + offset);
			}
		}, System.Windows.Threading.DispatcherPriority.Loaded);
	}

	private static T? FindChild<T>(DependencyObject parent, string? name) where T : FrameworkElement
	{
		for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
		{
			var child = VisualTreeHelper.GetChild(parent, i);
			if (child is T match && (name is null || match.Name == name))
			{
				return match;
			}
			if (FindChild<T>(child, name) is { } found)
			{
				return found;
			}
		}
		return null;
	}

	private void OnMinimize(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

	private void OnMaximize(object sender, RoutedEventArgs e) =>
		WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

	private void OnClose(object sender, RoutedEventArgs e) => Close();
}
