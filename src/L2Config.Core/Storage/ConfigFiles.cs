using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using L2Config.Core.Ini;

namespace L2Config.Core.Storage;

/// <summary>One file (or pair of files) that holds settings.</summary>
public abstract class ConfigFile
{
	protected ConfigFile(string displayName)
	{
		DisplayName = displayName;
	}

	public string DisplayName { get; }

	/// <summary>Why this file could not be read, or null when it loaded.</summary>
	public string? LoadError { get; protected set; }

	public bool IsLoaded => LoadError is null;

	/// <summary>Every path this file writes to, for backups and for "open in folder".</summary>
	public abstract IReadOnlyList<string> Paths { get; }

	public abstract void Load();
	public abstract string? Get(string? section, string key);
	public abstract void Set(string? section, string key, string value);
	public abstract void Save(BackupSession backup);

	protected static void WriteAtomically(string path, byte[] bytes)
	{
		var temp = path + ".l2config.tmp";
		File.WriteAllBytes(temp, bytes);
		File.Move(temp, path, overwrite: true);
	}
}

/// <summary>
/// A Mobius server .ini. The server reads the install copy; the L2Everdream launcher also keeps a protected
/// player copy (L2Everdream-data\db\config) that it merges back over the install copy on every start and
/// update. Reading prefers the player copy; saving writes both so they never disagree.
/// </summary>
public sealed class ServerIniFile : ConfigFile
{
	private readonly string _installPath;
	private readonly string _playerCopyPath;
	private TextFile? _install;
	private TextFile? _playerCopy;

	public ServerIniFile(string displayName, string installPath, string playerCopyPath)
		: base(displayName)
	{
		_installPath = installPath;
		_playerCopyPath = playerCopyPath;
	}

	public override IReadOnlyList<string> Paths =>
		_playerCopy is null ? [_installPath] : [_installPath, _playerCopyPath];

	public override void Load()
	{
		LoadError = null;
		_install = null;
		_playerCopy = null;
		if (!File.Exists(_installPath))
		{
			LoadError = $"Not found: {_installPath}";
			return;
		}
		_install = TextFile.Read(_installPath);
		if (File.Exists(_playerCopyPath))
		{
			_playerCopy = TextFile.Read(_playerCopyPath);
		}
	}

	public override string? Get(string? section, string key) =>
		_playerCopy?.Document.Get(section, key) ?? _install?.Document.Get(section, key);

	public override void Set(string? section, string key, string value)
	{
		if (_install is null)
		{
			throw new InvalidOperationException(LoadError ?? "File is not loaded.");
		}
		_install.Document.Set(section, key, value);
		_playerCopy?.Document.Set(section, key, value);
	}

	public override void Save(BackupSession backup)
	{
		foreach (var file in new[] { _install, _playerCopy })
		{
			if (file is null)
			{
				continue;
			}
			backup.Preserve(file.Path);
			WriteAtomically(file.Path, file.Encode());
		}
	}
}

/// <summary>A client ini (plain, XOR or RSA encrypted) with [Section] headers.</summary>
public sealed class ClientIniFile : ConfigFile
{
	private readonly string _path;
	private IniDocument? _document;
	private L2IniFormat _format;

	public ClientIniFile(string displayName, string path)
		: base(displayName)
	{
		_path = path;
	}

	public override IReadOnlyList<string> Paths => [_path];

	public override void Load()
	{
		LoadError = null;
		_document = null;
		if (!File.Exists(_path))
		{
			LoadError = $"Not found: {_path}";
			return;
		}
		try
		{
			_document = IniDocument.Parse(L2IniCodec.Decode(File.ReadAllBytes(_path), out _format));
		}
		catch (Exception ex) when (ex is InvalidDataException or NotSupportedException)
		{
			LoadError = $"{Path.GetFileName(_path)} could not be read: {ex.Message}";
		}
	}

	public override string? Get(string? section, string key) => _document?.Get(section, key);

	public override void Set(string? section, string key, string value) =>
		(_document ?? throw new InvalidOperationException(LoadError ?? "File is not loaded.")).Set(section, key, value);

	public override void Save(BackupSession backup)
	{
		if (_document is null)
		{
			return;
		}
		var bytes = L2IniCodec.Encode(_document.Text, _format);

		// Never write a client file we could not read back identically.
		if (L2IniCodec.Decode(bytes, out _) != _document.Text)
		{
			throw new InvalidDataException($"Refusing to save {Path.GetFileName(_path)}: the encoded file did not decode back to the same text.");
		}
		backup.Preserve(_path);
		WriteAtomically(_path, bytes);
	}
}

/// <summary>The launcher's world-profile.json. Unknown properties are kept untouched.</summary>
public sealed class WorldProfileFile : ConfigFile
{
	private static readonly JsonSerializerOptions WriteOptions = new() { WriteIndented = true };
	private readonly string _path;
	private JsonObject? _root;

	public WorldProfileFile(string displayName, string path)
		: base(displayName)
	{
		_path = path;
	}

	public override IReadOnlyList<string> Paths => [_path];

	public override void Load()
	{
		LoadError = null;
		_root = null;
		if (!File.Exists(_path))
		{
			LoadError = "No saved world yet. Open the L2Everdream launcher once to create one.";
			return;
		}
		try
		{
			_root = JsonNode.Parse(File.ReadAllText(_path)) as JsonObject ?? throw new JsonException("not an object");
		}
		catch (JsonException ex)
		{
			LoadError = $"world-profile.json could not be read: {ex.Message}";
		}
	}

	public override string? Get(string? section, string key) => _root?[key] switch
	{
		null => null,
		JsonValue v when v.TryGetValue<bool>(out var b) => b ? "true" : "false",
		JsonNode n => n.ToString(),
	};

	public override void Set(string? section, string key, string value)
	{
		if (_root is null)
		{
			throw new InvalidOperationException(LoadError ?? "File is not loaded.");
		}
		_root[key] = _root[key] switch
		{
			JsonValue v when v.TryGetValue<bool>(out _) => JsonValue.Create(string.Equals(value, "true", StringComparison.OrdinalIgnoreCase)),
			JsonValue v when v.TryGetValue<long>(out _) && long.TryParse(value, out var l) => JsonValue.Create(l),
			JsonValue v when v.TryGetValue<double>(out _) && double.TryParse(value, System.Globalization.CultureInfo.InvariantCulture, out var d) => JsonValue.Create(d),
			_ => JsonValue.Create(value),
		};
	}

	public override void Save(BackupSession backup)
	{
		if (_root is null)
		{
			return;
		}
		backup.Preserve(_path);
		WriteAtomically(_path, Encoding.UTF8.GetBytes(_root.ToJsonString(WriteOptions) + "\n"));
	}
}

/// <summary>A text file whose byte-order mark and encoding are remembered so saving changes nothing else.</summary>
internal sealed class TextFile
{
	private static readonly UTF8Encoding StrictUtf8 = new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

	private TextFile(string path, IniDocument document, Encoding encoding, bool hasBom)
	{
		Path = path;
		Document = document;
		Encoding = encoding;
		HasBom = hasBom;
	}

	public string Path { get; }
	public IniDocument Document { get; }
	private Encoding Encoding { get; }
	private bool HasBom { get; }

	public static TextFile Read(string path)
	{
		var bytes = File.ReadAllBytes(path);
		var hasBom = bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF;
		var body = hasBom ? bytes.AsSpan(3) : bytes;
		Encoding encoding;
		string text;
		try
		{
			text = StrictUtf8.GetString(body);
			encoding = StrictUtf8;
		}
		catch (DecoderFallbackException)
		{
			text = Encoding.Latin1.GetString(body);
			encoding = Encoding.Latin1;
		}
		return new TextFile(path, IniDocument.Parse(text), encoding, hasBom);
	}

	public byte[] Encode()
	{
		var body = Encoding.GetBytes(Document.Text);
		return HasBom ? [0xEF, 0xBB, 0xBF, .. body] : body;
	}
}
