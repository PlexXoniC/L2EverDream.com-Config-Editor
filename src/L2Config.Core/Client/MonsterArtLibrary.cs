namespace L2Config.Core.Client;

/// <summary>
/// Draws a monster from the player's own game client: the client says which model belongs to an npc id
/// (<see cref="NpcGrp"/>), the model is read out of its animation package (<see cref="MonsterMesh"/>) and drawn
/// (<see cref="MeshRenderer"/>). Nothing is bundled with this program and nothing is downloaded — with no client
/// folder chosen, there is simply no picture.
/// </summary>
public sealed class MonsterArtLibrary : IDisposable
{
	/// <summary>How many steps a monster takes to turn all the way round.</summary>
	public const int TurntableFrames = 24;

	private readonly string _animations;
	private readonly string[] _textureFolders;
	private readonly IReadOnlyDictionary<int, NpcModel> _models;
	private readonly Dictionary<string, UePackage?> _packages = new(StringComparer.OrdinalIgnoreCase);
	private readonly Dictionary<int, IReadOnlyList<IconImage>?> _cache = [];
	private readonly Lock _lock = new();

	private MonsterArtLibrary(string animations, string[] textureFolders, IReadOnlyDictionary<int, NpcModel> models)
	{
		_animations = animations;
		_textureFolders = textureFolders;
		_models = models;
	}

	public int KnownModels => _models.Count;

	public static MonsterArtLibrary? ForClient(string? clientSystemDir)
	{
		if (clientSystemDir is null || Path.GetDirectoryName(clientSystemDir.TrimEnd(Path.DirectorySeparatorChar)) is not { } root)
		{
			return null;
		}
		var animations = Path.Combine(root, "animations");
		if (!Directory.Exists(animations))
		{
			return null;
		}
		var models = NpcGrp.Read(clientSystemDir);
		string[] textures = [Path.Combine(root, "systextures"), Path.Combine(root, "textures")];
		return models.Count == 0 ? null : new MonsterArtLibrary(animations, textures, models);
	}

	public NpcModel? ModelFor(int npcId) => _models.TryGetValue(npcId, out var model) ? model : null;

	/// <summary>A picture of one monster, or null when this client has no model for it.</summary>
	public IconImage? Render(int npcId, int size = 208) => Turn(npcId, size, frames: 1)?[0];

	/// <summary>
	/// Every step of the monster turning on the spot, painted with its own skins. Null when this client has no model
	/// for it. Drawing a whole turn takes a moment, so results are kept.
	/// </summary>
	public IReadOnlyList<IconImage>? Turn(int npcId, int size = 208, int frames = TurntableFrames)
	{
		lock (_lock)
		{
			if (_cache.TryGetValue(npcId, out var cached) && (cached is null || cached.Count >= frames))
			{
				return cached;
			}
			IReadOnlyList<IconImage>? turn = null;
			if (ModelFor(npcId) is { } model && Package(model.MeshPackage) is { } package
				&& package.Export(model.MeshName) is { } export
				&& MonsterMesh.Read(export) is { } mesh)
			{
				turn = MeshRenderer.Turntable(mesh, Skins(model, mesh.SkinCount), size, Math.Max(frames, 1));
			}
			_cache[npcId] = turn;
			return turn;
		}
	}

	/// <summary>The model's own skins, in the order the client lists them for that npc.</summary>
	private IReadOnlyList<IconImage?> Skins(NpcModel model, int needed)
	{
		var skins = new List<IconImage?>();
		foreach (var texture in model.Textures)
		{
			var dot = texture.IndexOf('.');
			skins.Add(dot <= 0 ? null : ReadTexture(texture[..dot], texture[(dot + 1)..]));
		}
		while (skins.Count < needed)
		{
			skins.Add(skins.Count > 0 ? skins[0] : null);
		}
		return skins;
	}

	private IconImage? ReadTexture(string packageName, string textureName)
	{
		foreach (var folder in _textureFolders)
		{
			var path = Path.Combine(folder, packageName + ".utx");
			if (!File.Exists(path))
			{
				continue;
			}
			using var package = UePackage.Open(path);
			if (package?.Export(textureName) is not { } export)
			{
				continue;
			}
			if (UeTexture.Read(export, package.Names, package.OffsetOf(textureName)) is { } picture)
			{
				return picture;
			}
			// The name is a shader: follow it to the picture it paints with.
			if (UeTexture.Diffuse(export, package.Names) is { } reference
				&& package.ObjectName(reference) is { Length: > 0 } diffuse
				&& package.Export(diffuse) is { } diffuseExport)
			{
				return UeTexture.Read(diffuseExport, package.Names, package.OffsetOf(diffuse));
			}
		}
		return null;
	}

	private UePackage? Package(string name)
	{
		if (_packages.TryGetValue(name, out var cached))
		{
			return cached;
		}
		var package = UePackage.Open(Path.Combine(_animations, name + ".ukx"));
		_packages[name] = package;
		return package;
	}

	public void Dispose()
	{
		lock (_lock)
		{
			foreach (var package in _packages.Values)
			{
				package?.Dispose();
			}
			_packages.Clear();
		}
	}
}
