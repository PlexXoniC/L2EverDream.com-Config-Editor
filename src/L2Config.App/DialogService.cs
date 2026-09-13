using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shell;
using L2Config.App.ViewModels;
using Microsoft.Win32;

namespace L2Config.App;

/// <summary>Folder pickers and small themed dialogs (no native grey message boxes).</summary>
public sealed class DialogService : IDialogs
{
	private static Window owner => Application.Current.MainWindow;

	public string? PickFolder(string title, string? initial)
	{
		var dialog = new OpenFolderDialog { Title = title, Multiselect = false };
		if (initial is not null && Directory.Exists(initial))
		{
			dialog.InitialDirectory = initial;
		}
		return dialog.ShowDialog(owner) == true ? dialog.FolderName : null;
	}

	public bool Confirm(string title, string message, string confirmText) => Show(title, message, confirmText, "Cancel");

	public void Notice(string title, string message) => Show(title, message, "OK", null);

	public void OpenFolder(string path)
	{
		Directory.CreateDirectory(path);
		Process.Start(new ProcessStartInfo("explorer.exe", $"\"{path}\"") { UseShellExecute = true });
	}

	private bool Show(string title, string message, string primary, string? secondary)
	{
		var result = false;
		var window = new Window
		{
			Owner = owner,
			Width = 460,
			SizeToContent = SizeToContent.Height,
			WindowStartupLocation = WindowStartupLocation.CenterOwner,
			WindowStyle = WindowStyle.None,
			ResizeMode = ResizeMode.NoResize,
			AllowsTransparency = true,
			Background = Brushes.Transparent,
			ShowInTaskbar = false,
			Title = title,
		};
		WindowChrome.SetWindowChrome(window, new WindowChrome { CaptionHeight = 40, GlassFrameThickness = new Thickness(0) });

		var primaryButton = new Button
		{
			Content = primary,
			Style = (Style)owner.FindResource(secondary is null ? "GhostButton" : "PrimaryButton"),
			IsDefault = true,
			Margin = new Thickness(10, 0, 0, 0),
		};
		primaryButton.Click += (_, _) => { result = true; window.Close(); };

		var buttons = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 22, 0, 0) };
		if (secondary is not null)
		{
			var cancel = new Button { Content = secondary, Style = (Style)owner.FindResource("GhostButton"), IsCancel = true };
			cancel.Click += (_, _) => window.Close();
			buttons.Children.Add(cancel);
		}
		buttons.Children.Add(primaryButton);

		window.Content = new Border
		{
			Margin = new Thickness(12),
			Padding = new Thickness(24, 20, 24, 20),
			CornerRadius = new CornerRadius(16),
			Background = (Brush)owner.FindResource("OverlayBrush"),
			BorderBrush = (Brush)owner.FindResource("VioletHairBrush"),
			BorderThickness = new Thickness(1),
			Effect = new System.Windows.Media.Effects.DropShadowEffect { BlurRadius = 30, ShadowDepth = 8, Opacity = 0.7, Color = Colors.Black },
			Child = new StackPanel
			{
				Children =
				{
					new TextBlock
					{
						Text = title,
						FontFamily = (FontFamily)owner.FindResource("DisplayFont"),
						FontSize = 21,
						FontWeight = FontWeights.SemiBold,
						Foreground = (Brush)owner.FindResource("StarlightBrush"),
						TextWrapping = TextWrapping.Wrap,
					},
					new TextBlock
					{
						Text = message,
						Margin = new Thickness(0, 10, 0, 0),
						FontSize = 13,
						LineHeight = 19,
						Foreground = (Brush)owner.FindResource("SoftTextBrush"),
						TextWrapping = TextWrapping.Wrap,
					},
					buttons,
				},
			},
		};
		window.ShowDialog();
		return result;
	}
}
