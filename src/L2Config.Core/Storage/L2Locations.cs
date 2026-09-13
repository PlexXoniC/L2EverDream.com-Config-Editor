namespace L2Config.Core.Storage;

/// <summary>
/// The server and client folders the user chose. Either can be missing; nothing is guessed.
/// </summary>
public sealed record L2Locations(string? ServerRoot, string? ClientSystemDir)
{
	public const string DataFolderName = "L2Everdream-data";

	public bool HasServer => ServerRoot is not null && IsServerRoot(ServerRoot);
	public bool HasClient => ClientSystemDir is not null && File.Exists(Path.Combine(ClientSystemDir, "l2.ini"));

	public string GameConfigDir => Path.Combine(ServerRoot ?? "", "game", "config");
	public string LoginConfigDir => Path.Combine(ServerRoot ?? "", "login", "config");

	/// <summary>
	/// The launcher keeps its player data beside the install folder (…\L2Everdream → …\L2Everdream-data).
	/// A server folder without it simply has no protected copies and no world profile.
	/// </summary>
	public string DataRoot => Path.Combine(Path.GetDirectoryName(Path.GetFullPath(ServerRoot ?? "."))!, DataFolderName);

	/// <summary>The launcher's protected copies: edits here survive updates and are applied on every start.</summary>
	public string PlayerGameConfigDir => Path.Combine(DataRoot, "db", "config", "game");
	public string PlayerLoginConfigDir => Path.Combine(DataRoot, "db", "config", "login");

	public string WorldProfilePath => Path.Combine(DataRoot, "worlds", "world-profile.json");

	/// <summary>True when <paramref name="folder"/> is an L2Everdream / L2J Mobius server folder (has game\config).</summary>
	public static bool IsServerRoot(string folder) =>
		Directory.Exists(Path.Combine(folder, "game", "config")) && File.Exists(Path.Combine(folder, "game", "config", "Server.ini"));

	/// <summary>
	/// Accepts either the client's main folder or its system folder and returns the system folder,
	/// or null when neither contains l2.ini.
	/// </summary>
	public static string? ResolveClientSystemDir(string folder)
	{
		if (File.Exists(Path.Combine(folder, "l2.ini")))
		{
			return folder;
		}
		var system = Path.Combine(folder, "system");
		return File.Exists(Path.Combine(system, "l2.ini")) ? system : null;
	}
}
