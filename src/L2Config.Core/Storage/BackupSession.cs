namespace L2Config.Core.Storage;

/// <summary>
/// Copies each file once, before its first write, into a timestamped folder. Kept outside the L2Everdream
/// install (which updates delete) and outside the launcher's own your-files folder.
/// </summary>
public sealed class BackupSession
{
	private readonly HashSet<string> _preserved = new(StringComparer.OrdinalIgnoreCase);

	public BackupSession(string backupsRoot, DateTime now)
	{
		Folder = Path.Combine(backupsRoot, now.ToString("yyyyMMdd-HHmmss"));
	}

	public string Folder { get; }

	public IReadOnlyCollection<string> PreservedFiles => _preserved;

	public static string DefaultRoot => Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "L2EverdreamConfig", "backups");

	public void Preserve(string path)
	{
		if (!File.Exists(path) || !_preserved.Add(Path.GetFullPath(path)))
		{
			return;
		}
		var full = Path.GetFullPath(path);
		var relative = Path.Combine(Path.GetPathRoot(full)!.TrimEnd('\\', ':', '/'), full[Path.GetPathRoot(full)!.Length..]);
		var destination = Path.Combine(Folder, relative);
		Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
		File.Copy(path, destination, overwrite: false);
	}
}
