using System.ComponentModel;
using L2Config.App.Infrastructure;
using L2Config.Core.Catalog;
using L2Config.Core.Storage;

namespace L2Config.App.ViewModels;

/// <summary>One line on a setting card explaining how another setting changes what this one does. Updates live as either is edited.</summary>
public sealed class RelationViewModel : ObservableObject
{
	public RelationViewModel(SettingRelation relation, SettingViewModel other, bool reverse, Action<string> showSetting)
	{
		Relation = relation;
		Other = other;
		IsReverse = reverse;
		ShowCommand = new RelayCommand(() => showSetting(other.Definition.Id));
		other.PropertyChanged += OnOtherChanged;
	}

	public SettingRelation Relation { get; }
	public SettingViewModel Other { get; }

	/// <summary>True when shown on the setting that the other one depends on ("this changes …").</summary>
	public bool IsReverse { get; }

	public RelayCommand ShowCommand { get; }
	public string OtherName => Other.Name;

	public bool IsProblem => Relation.IsRequirement && !IsReverse && !Other.IsMissing && !Relation.IsSatisfiedBy(Other.Value);

	public string Kicker => (Relation.IsRequirement, IsReverse) switch
	{
		(true, false) => IsProblem ? "HAS NO EFFECT RIGHT NOW" : "DEPENDS ON",
		(true, true) => "CONTROLS",
		_ => "WORKS WITH",
	};

	public string Text
	{
		get
		{
			if (Relation.IsRequirement && !IsReverse)
			{
				var needed = DescribeValue(Relation.Value);
				if (Other.IsMissing)
				{
					return $"Needs “{OtherName}” to be {needed}. It can't be checked: {Other.MissingText}";
				}
				var current = DescribeValue(Other.Value);
				return IsProblem
					? $"“{OtherName}” is {current}, and this only works when it is {needed}.{Note}"
					: $"Works because “{OtherName}” is {current}.";
			}
			if (Relation.IsRequirement)
			{
				return $"“{OtherName}” only has an effect while this is {DescribeValue(Relation.Value)}.";
			}
			return $"“{OtherName}”.{Note}";
		}
	}

	private string Note => Relation.Note is { Length: > 0 } note ? " " + note : "";

	private static string DescribeValue(string? value) => value?.Trim().ToLowerInvariant() switch
	{
		"true" => "On",
		"false" => "Off",
		">0" => "above 0",
		null or "" => "(empty)",
		var v => $"“{value}”",
	};

	private void OnOtherChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName is nameof(SettingViewModel.Value))
		{
			OnPropertyChanged(nameof(IsProblem));
			OnPropertyChanged(nameof(Kicker));
			OnPropertyChanged(nameof(Text));
		}
	}
}
