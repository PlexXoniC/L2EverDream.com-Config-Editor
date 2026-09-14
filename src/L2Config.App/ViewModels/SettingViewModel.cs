using System.Globalization;
using L2Config.App.Infrastructure;
using L2Config.Core.Catalog;
using L2Config.Core.Storage;

namespace L2Config.App.ViewModels;

/// <summary>One setting row: its friendly face, the real file/key it links to, and the value being edited.</summary>
public sealed class SettingViewModel : ObservableObject
{
	private readonly Action _changed;
	private string _value;
	private string? _original;

	public SettingViewModel(SettingDefinition definition, string groupName, string? diskValue, string? fileProblem, Action changed)
	{
		Definition = definition;
		GroupName = groupName;
		_changed = changed;
		FileProblem = fileProblem;
		_original = diskValue;
		_value = diskValue ?? "";
		Options = BuildOptions(definition, diskValue);
		ResetToDefaultCommand = new RelayCommand(() => Value = Definition.Default ?? Value, () => CanResetToDefault);
		UndoCommand = new RelayCommand(() => Value = _original ?? "", () => IsDirty);
	}

	public SettingDefinition Definition { get; }
	public string GroupName { get; }
	public string Name => Definition.Name;
	public string Description => Definition.Description;
	public bool HasDescription => !string.IsNullOrWhiteSpace(Definition.Description);

	/// <summary>The link to the real value: file › [section] › key.</summary>
	public string Location => Definition.Section is null
		? $"{Definition.File}  ›  {Definition.Key}"
		: $"{Definition.File}  ›  [{Definition.Section}]  ›  {Definition.Key}";

	public string? DefaultText => Definition.Default is null ? null : $"Default {FormatForDisplay(Definition.Default)}";
	public string? Unit => Definition.Unit;

	/// <summary>The allowed range, shown on the card so limits are visible before anyone hits them.</summary>
	public string? RangeText => Definition.Editor is SettingEditor.Number or SettingEditor.Slider && SettingValues.DescribeRange(Definition) is { } range
		? $"Allowed {range}"
		: null;
	public IReadOnlyList<ChoiceOption> Options { get; }

	/// <summary>How other settings change what this one does (and which settings this one controls).</summary>
	public System.Collections.ObjectModel.ObservableCollection<RelationViewModel> Relations { get; } = [];

	public bool HasRelations => Relations.Count > 0;

	/// <summary>Why the value can't be shown (file missing/unreadable), or null.</summary>
	public string? FileProblem { get; }

	public bool IsMissing => FileProblem is not null || _original is null;
	public bool IsManaged => Definition.IsManaged;
	public bool IsEditable => !IsManaged && !IsMissing;

	public string? MissingText => FileProblem ?? (_original is null ? "This setting is not in your file." : null);

	public string Value
	{
		get => _value;
		set
		{
			if (Set(ref _value, value ?? ""))
			{
				RaiseValueDependents();
				_changed();
			}
		}
	}

	public bool BoolValue
	{
		get => SettingValues.IsTrue(_value);
		set => Value = Definition.Target == SettingTargets.WorldProfile
			? (value ? "true" : "false")
			: SettingValues.FormatBool(value, _original);
	}

	public double NumberValue
	{
		get => SettingValues.TryParseNumber(_value, out var n) ? n : Definition.Min ?? 0;
		set => Value = Definition.ValueType == SettingValueType.Int
			? Math.Round(value).ToString(CultureInfo.InvariantCulture)
			: Math.Round(value, 2).ToString("0.######", CultureInfo.InvariantCulture);
	}

	public ChoiceOption? SelectedOption
	{
		get => Options.FirstOrDefault(o => string.Equals(o.Value, _value.Trim(), StringComparison.OrdinalIgnoreCase));
		set
		{
			if (value is not null)
			{
				Value = value.Value;
			}
		}
	}

	public string? Error => IsEditable ? SettingValues.Validate(Definition, _value) : null;
	public bool HasError => Error is not null;

	public bool IsDirty => IsEditable && !SettingValues.AreEqual(Definition, _value, _original);

	public bool IsChangedFromDefault =>
		Definition.Default is not null && _original is not null && !SettingValues.AreEqual(Definition, _original, Definition.Default);

	public bool CanResetToDefault => IsEditable && Definition.Default is not null && !SettingValues.AreEqual(Definition, _value, Definition.Default);

	/// <summary>A word, never just a colour: what state this row is in.</summary>
	public string? StateWord => IsDirty ? "Unsaved" : IsChangedFromDefault ? "Changed" : null;

	public RelayCommand ResetToDefaultCommand { get; }
	public RelayCommand UndoCommand { get; }

	/// <summary>Called after a successful save: what is on disk is now the edited value.</summary>
	public void AcceptSaved()
	{
		_original = _value;
		RaiseValueDependents();
	}

	private void RaiseValueDependents()
	{
		OnPropertyChanged(nameof(Value));
		OnPropertyChanged(nameof(BoolValue));
		OnPropertyChanged(nameof(NumberValue));
		OnPropertyChanged(nameof(SelectedOption));
		OnPropertyChanged(nameof(Error));
		OnPropertyChanged(nameof(HasError));
		OnPropertyChanged(nameof(IsDirty));
		OnPropertyChanged(nameof(IsChangedFromDefault));
		OnPropertyChanged(nameof(CanResetToDefault));
		OnPropertyChanged(nameof(StateWord));
	}

	private string FormatForDisplay(string raw)
	{
		if (Definition.Editor == SettingEditor.Toggle)
		{
			return SettingValues.IsTrue(raw) ? "On" : "Off";
		}
		var option = Options.FirstOrDefault(o => string.Equals(o.Value, raw, StringComparison.OrdinalIgnoreCase));
		return option?.Label ?? (raw.Length == 0 ? "(empty)" : raw);
	}

	private static IReadOnlyList<ChoiceOption> BuildOptions(SettingDefinition definition, string? current)
	{
		if (definition.Options is null)
		{
			return [];
		}
		var options = definition.Options.Select(ChoiceOption.Parse).ToList();
		if (current is not null && options.All(o => !string.Equals(o.Value, current.Trim(), StringComparison.OrdinalIgnoreCase)))
		{
			options.Add(new ChoiceOption(current.Trim(), $"{current.Trim()} (current value)"));
		}
		return options;
	}
}

public sealed record ChoiceOption(string Value, string Label)
{
	public static ChoiceOption Parse(string raw)
	{
		var eq = raw.IndexOf('=');
		if (eq <= 0)
		{
			return new ChoiceOption(raw, raw);
		}
		var value = raw[..eq].Trim();
		var label = raw[(eq + 1)..].Trim();
		return new ChoiceOption(value, label == value ? value : $"{label}");
	}

	public override string ToString() => Label;
}
