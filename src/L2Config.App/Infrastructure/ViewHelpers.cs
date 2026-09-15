using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using L2Config.App.ViewModels;
using L2Config.Core.Catalog;

namespace L2Config.App.Infrastructure;

/// <summary>Visible when the bound value is non-null, non-empty, non-zero and not false. Parameter "invert" flips it.</summary>
public sealed class VisibleWhenConverter : IValueConverter
{
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		var visible = value switch
		{
			null => false,
			bool b => b,
			int i => i != 0,
			string s => s.Length > 0,
			_ => true,
		};
		if (parameter as string == "invert")
		{
			visible = !visible;
		}
		return visible ? Visibility.Visible : Visibility.Collapsed;
	}

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
		throw new NotSupportedException();
}

/// <summary>Picks the editor for a setting row from its catalog editor type.</summary>
public sealed class SettingEditorSelector : DataTemplateSelector
{
	public DataTemplate? Toggle { get; set; }
	public DataTemplate? Number { get; set; }
	public DataTemplate? Slider { get; set; }
	public DataTemplate? Choice { get; set; }
	public DataTemplate? Text { get; set; }
	public DataTemplate? Secret { get; set; }
	public DataTemplate? ReadOnly { get; set; }
	public DataTemplate? SkillDurations { get; set; }

	public override DataTemplate? SelectTemplate(object item, DependencyObject container)
	{
		if (item is not SettingViewModel setting)
		{
			return null;
		}
		if (!setting.IsEditable)
		{
			return ReadOnly;
		}
		return setting.Definition.Editor switch
		{
			SettingEditor.Toggle => Toggle,
			SettingEditor.Number => Number,
			SettingEditor.Slider => Slider,
			SettingEditor.Choice => Choice,
			SettingEditor.Secret => Secret,
			SettingEditor.SkillDurations => SkillDurations,
			_ => Text,
		};
	}
}

/// <summary>Stops letters being typed or pasted into number fields.</summary>
public static class NumericInput
{
	public static readonly DependencyProperty ValueTypeProperty = DependencyProperty.RegisterAttached(
		"ValueType", typeof(SettingValueType?), typeof(NumericInput), new PropertyMetadata(null, OnValueTypeChanged));

	public static SettingValueType? GetValueType(DependencyObject o) => (SettingValueType?)o.GetValue(ValueTypeProperty);

	public static void SetValueType(DependencyObject o, SettingValueType? value) => o.SetValue(ValueTypeProperty, value);

	private static void OnValueTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is not TextBox box)
		{
			return;
		}
		box.PreviewTextInput -= OnPreviewTextInput;
		box.PreviewKeyDown -= OnPreviewKeyDown;
		DataObject.RemovePastingHandler(box, OnPasting);
		if (e.NewValue is SettingValueType.Int or SettingValueType.Decimal)
		{
			box.PreviewTextInput += OnPreviewTextInput;
			box.PreviewKeyDown += OnPreviewKeyDown;
			DataObject.AddPastingHandler(box, OnPasting);
		}
	}

	private static void OnPreviewKeyDown(object sender, KeyEventArgs e)
	{
		if (e.Key == Key.Space)
		{
			e.Handled = true;
		}
	}

	private static void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
	{
		var box = (TextBox)sender;
		var proposed = box.Text.Remove(box.SelectionStart, box.SelectionLength).Insert(box.SelectionStart, e.Text);
		e.Handled = !IsPlausible(proposed, GetValueType(box));
	}

	private static void OnPasting(object sender, DataObjectPastingEventArgs e)
	{
		var box = (TextBox)sender;
		if (e.DataObject.GetData(typeof(string)) is not string text)
		{
			e.CancelCommand();
			return;
		}
		var proposed = box.Text.Remove(box.SelectionStart, box.SelectionLength).Insert(box.SelectionStart, text.Trim());
		if (!IsPlausible(proposed, GetValueType(box)))
		{
			e.CancelCommand();
		}
	}

	/// <summary>Allows partial input such as "-" or "1." while typing; full validation happens in the view model.</summary>
	private static bool IsPlausible(string text, SettingValueType? type)
	{
		var seenDot = false;
		for (var i = 0; i < text.Length; i++)
		{
			var c = text[i];
			if (char.IsAsciiDigit(c))
			{
				continue;
			}
			if (c == '-' && i == 0)
			{
				continue;
			}
			if (c == '.' && type == SettingValueType.Decimal && !seenDot)
			{
				seenDot = true;
				continue;
			}
			return false;
		}
		return true;
	}
}

/// <summary>Two-way binding for PasswordBox.Password, which WPF does not allow directly.</summary>
public static class PasswordBinding
{
	public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached(
		"Value", typeof(string), typeof(PasswordBinding),
		new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

	private static readonly DependencyProperty IsUpdatingProperty =
		DependencyProperty.RegisterAttached("IsUpdating", typeof(bool), typeof(PasswordBinding));

	public static string GetValue(DependencyObject o) => (string)o.GetValue(ValueProperty);

	public static void SetValue(DependencyObject o, string value) => o.SetValue(ValueProperty, value);

	private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is not PasswordBox box)
		{
			return;
		}
		box.PasswordChanged -= OnPasswordChanged;
		if (!(bool)box.GetValue(IsUpdatingProperty))
		{
			box.Password = e.NewValue as string ?? "";
		}
		box.PasswordChanged += OnPasswordChanged;
	}

	private static void OnPasswordChanged(object sender, RoutedEventArgs e)
	{
		var box = (PasswordBox)sender;
		box.SetValue(IsUpdatingProperty, true);
		SetValue(box, box.Password);
		box.SetValue(IsUpdatingProperty, false);
	}
}
