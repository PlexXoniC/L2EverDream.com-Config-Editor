using System.Collections.ObjectModel;
using L2Config.App.Infrastructure;

namespace L2Config.App.ViewModels;

/// <summary>A category in the left section list.</summary>
public sealed class CategoryNode : ObservableObject
{
	private int _count;
	private bool _isExpanded;
	private bool _isSelected;

	public CategoryNode(string id, string name, string glyph, string description)
	{
		Id = id;
		Name = name;
		Glyph = glyph;
		Description = description;
	}

	public string Id { get; }
	public string Name { get; }
	public string Glyph { get; }
	public string Description { get; }
	public ObservableCollection<GroupNode> Groups { get; } = [];

	public int Count
	{
		get => _count;
		set => Set(ref _count, value);
	}

	public bool IsExpanded
	{
		get => _isExpanded;
		set => Set(ref _isExpanded, value);
	}

	public bool IsSelected
	{
		get => _isSelected;
		set => Set(ref _isSelected, value);
	}
}

/// <summary>A group inside a category in the left section list.</summary>
public sealed class GroupNode : ObservableObject
{
	private int _count;
	private bool _isSelected;

	public GroupNode(CategoryNode category, string id, string name)
	{
		Category = category;
		Id = id;
		Name = name;
	}

	public CategoryNode Category { get; }
	public string Id { get; }
	public string Name { get; }

	public int Count
	{
		get => _count;
		set => Set(ref _count, value);
	}

	public bool IsSelected
	{
		get => _isSelected;
		set => Set(ref _isSelected, value);
	}
}

/// <summary>A heading row in the settings list.</summary>
public sealed record GroupHeaderRow(string GroupId, string Title, string? Breadcrumb, int Count);
