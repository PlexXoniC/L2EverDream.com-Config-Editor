using System.Collections.ObjectModel;
using L2Config.App.Infrastructure;
using L2Config.Core.Catalog;

namespace L2Config.App.ViewModels;

/// <summary>The Server tab or the Client tab: its folder, section list, search and settings list.</summary>
public sealed class TabViewModel : ObservableObject
{
	private readonly SettingsCatalog _catalog;
	private readonly Func<bool> _showAdvanced;
	private List<SettingViewModel> _settings = [];
	private string _searchText = "";
	private bool _changedOnly;
	private CategoryNode? _selectedCategory;
	private string? _folderPath;
	private string? _folderProblem;
	private string? _runtimeNotice;
	private int _dirtyCount;

	public TabViewModel(string scope, SettingsCatalog catalog, Func<bool> showAdvanced, Action chooseFolder)
	{
		Scope = scope;
		_catalog = catalog;
		_showAdvanced = showAdvanced;
		ChooseFolderCommand = new RelayCommand(chooseFolder);
		SelectCategoryCommand = new RelayCommand(p => { if (p is CategoryNode c) SelectCategory(c); });
		SelectGroupCommand = new RelayCommand(p => { if (p is GroupNode g) SelectGroup(g); });
		ClearSearchCommand = new RelayCommand(() => SearchText = "");

		foreach (var category in catalog.Categories.Where(c => c.Scope == scope))
		{
			var node = new CategoryNode(category.Id, category.Name, category.Glyph, category.Description);
			foreach (var group in category.Groups)
			{
				node.Groups.Add(new GroupNode(node, group.Id, group.Name));
			}
			AllCategories.Add(node);
		}
	}

	public string Scope { get; }
	public bool IsServer => Scope == "server";
	public string Title => IsServer ? "Server" : "Client";
	public string FolderKicker => IsServer ? "SERVER FOLDER" : "CLIENT FOLDER";
	public string ChooseFolderText => IsServer ? "Choose server folder" : "Choose client folder";

	public string EmptyTitle => IsServer ? "Where is your L2Everdream server?" : "Where is your Lineage 2 client?";

	public string EmptyText => IsServer
		? "Choose the folder that holds the server — the one with the game and login folders inside (for the L2Everdream launcher this is usually L2Everdream in your local app data)."
		: "Choose the Lineage 2 Interlude client folder — the one with the system folder inside, or the system folder itself.";

	public List<CategoryNode> AllCategories { get; } = [];
	public ObservableCollection<CategoryNode> Categories { get; } = [];
	public ObservableCollection<object> Rows { get; } = [];

	public RelayCommand ChooseFolderCommand { get; }
	public RelayCommand SelectCategoryCommand { get; }
	public RelayCommand SelectGroupCommand { get; }
	public RelayCommand ClearSearchCommand { get; }

	/// <summary>Raised when the list should scroll a row into view.</summary>
	public event Action<object>? ScrollRequested;

	public IReadOnlyList<SettingViewModel> Settings => _settings;

	public string? FolderPath
	{
		get => _folderPath;
		set
		{
			if (Set(ref _folderPath, value))
			{
				OnPropertyChanged(nameof(HasFolder));
			}
		}
	}

	/// <summary>Set when the chosen folder is missing something; shown instead of the settings.</summary>
	public string? FolderProblem
	{
		get => _folderProblem;
		set
		{
			if (Set(ref _folderProblem, value))
			{
				OnPropertyChanged(nameof(HasFolder));
			}
		}
	}

	public bool HasFolder => FolderPath is not null && FolderProblem is null;

	/// <summary>"Your world is running…" / "The game client is open…", or null.</summary>
	public string? RuntimeNotice
	{
		get => _runtimeNotice;
		set => Set(ref _runtimeNotice, value);
	}

	public int DirtyCount
	{
		get => _dirtyCount;
		private set => Set(ref _dirtyCount, value);
	}

	public string SearchText
	{
		get => _searchText;
		set
		{
			if (Set(ref _searchText, value ?? ""))
			{
				OnPropertyChanged(nameof(IsSearching));
				Refresh();
			}
		}
	}

	public bool IsSearching => SettingSearch.Tokenize(_searchText).Length > 0;

	public bool ChangedOnly
	{
		get => _changedOnly;
		set
		{
			if (Set(ref _changedOnly, value))
			{
				Refresh();
			}
		}
	}

	public CategoryNode? SelectedCategory
	{
		get => _selectedCategory;
		private set
		{
			if (_selectedCategory is not null)
			{
				_selectedCategory.IsSelected = false;
			}
			if (Set(ref _selectedCategory, value))
			{
				OnPropertyChanged(nameof(PageTitle));
				OnPropertyChanged(nameof(PageGlyph));
				OnPropertyChanged(nameof(PageDescription));
			}
			if (value is not null)
			{
				value.IsSelected = true;
			}
		}
	}

	public string PageTitle => IsSearching ? "Search results" : SelectedCategory?.Name ?? "";
	public string PageGlyph => IsSearching ? "⌕" : SelectedCategory?.Glyph ?? "";

	public string PageDescription => IsSearching
		? ResultSummary
		: SelectedCategory?.Description ?? "";

	private string ResultSummary { get; set; } = "";

	public void SetSettings(List<SettingViewModel> settings)
	{
		_settings = settings;
		UpdateDirtyCount();
		SelectedCategory ??= AllCategories.FirstOrDefault();
		Refresh();
	}

	public void UpdateDirtyCount() => DirtyCount = _settings.Count(s => s.IsDirty);

	public void SelectCategory(CategoryNode category)
	{
		if (IsSearching)
		{
			var header = Rows.OfType<GroupHeaderRow>().FirstOrDefault(r => category.Groups.Any(g => g.Id == r.GroupId));
			if (header is not null)
			{
				ScrollRequested?.Invoke(header);
			}
			return;
		}
		category.IsExpanded = true;
		if (category != SelectedCategory)
		{
			SelectedCategory = category;
			Refresh();
		}
		if (Rows.Count > 0)
		{
			ScrollRequested?.Invoke(Rows[0]);
		}
	}

	public void SelectGroup(GroupNode group)
	{
		if (!IsSearching && group.Category != SelectedCategory)
		{
			SelectedCategory = group.Category;
			Refresh();
		}
		foreach (var g in AllCategories.SelectMany(c => c.Groups))
		{
			g.IsSelected = g == group;
		}
		var header = Rows.OfType<GroupHeaderRow>().FirstOrDefault(r => r.GroupId == group.Id);
		if (header is not null)
		{
			ScrollRequested?.Invoke(header);
		}
	}

	/// <summary>Rebuilds the section list counts and the rows for the current page, search and filters.</summary>
	public void Refresh()
	{
		var tokens = SettingSearch.Tokenize(_searchText);
		var showAdvanced = _showAdvanced();
		var visible = _settings.Where(s =>
				(showAdvanced || !s.Definition.Advanced || s.IsDirty)
				&& (!_changedOnly || s.IsChangedFromDefault || s.IsDirty)
				&& (tokens.Length == 0 || SettingSearch.Matches(s.Definition, s.GroupName, tokens)))
			.ToList();
		var byGroup = visible.ToLookup(s => s.Definition.Group);

		Categories.Clear();
		foreach (var category in AllCategories)
		{
			foreach (var group in category.Groups)
			{
				group.Count = byGroup[group.Id].Count();
			}
			category.Count = category.Groups.Sum(g => g.Count);
			category.IsExpanded = tokens.Length > 0 || category == SelectedCategory;
			if (category.Count > 0 || (tokens.Length == 0 && !_changedOnly))
			{
				Categories.Add(category);
			}
		}

		Rows.Clear();
		if (tokens.Length > 0)
		{
			foreach (var category in AllCategories)
			{
				foreach (var group in category.Groups.Where(g => g.Count > 0))
				{
					var hits = byGroup[group.Id].OrderByDescending(s => SettingSearch.Rank(s.Definition, tokens)).ToList();
					Rows.Add(new GroupHeaderRow(group.Id, group.Name, category.Name, hits.Count));
					foreach (var hit in hits)
					{
						Rows.Add(hit);
					}
				}
			}
			ResultSummary = visible.Count switch
			{
				0 => $"Nothing matches “{_searchText.Trim()}”. Try a shorter word, or the real setting name from the file.",
				1 => "1 setting matches.",
				var n => $"{n} settings match, across {Categories.Count} {(Categories.Count == 1 ? "category" : "categories")}.",
			};
		}
		else if (SelectedCategory is not null)
		{
			foreach (var group in SelectedCategory.Groups.Where(g => g.Count > 0))
			{
				Rows.Add(new GroupHeaderRow(group.Id, group.Name, null, group.Count));
				foreach (var setting in byGroup[group.Id])
				{
					Rows.Add(setting);
				}
			}
		}

		OnPropertyChanged(nameof(PageTitle));
		OnPropertyChanged(nameof(PageGlyph));
		OnPropertyChanged(nameof(PageDescription));
		OnPropertyChanged(nameof(HasRows));
		OnPropertyChanged(nameof(EmptyListText));
	}

	public bool HasRows => Rows.Count > 0;

	public string EmptyListText => IsSearching
		? ResultSummary
		: _changedOnly ? "Nothing in this section has been changed from its default." : "Nothing to show here. Turn on “Advanced” to see technical settings.";
}
