using System.Text.Json;
using L2Config.Core.Storage;

namespace L2Config.Core.Tests;

internal static class TestPaths
{
	/// <summary>
	/// This machine's server and client folders, never committed. Read from the environment variables
	/// L2CONFIG_TEST_SERVER and L2CONFIG_TEST_CLIENT, or from the git-ignored test-paths.local.json at the repository root:
	/// { "server": "...\\L2Everdream", "client": "...\\l2\\system" }. Tests only read these and write to temp copies.
	/// </summary>
	public static readonly L2Locations RealLocations = LoadRealLocations();

	public static string RepoRoot
	{
		get
		{
			var dir = new DirectoryInfo(AppContext.BaseDirectory);
			while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "L2EverdreamConfig.slnx")))
			{
				dir = dir.Parent;
			}
			return dir?.FullName ?? throw new InvalidOperationException("Repository root not found.");
		}
	}

	public static string CatalogPath => Path.Combine(RepoRoot, "catalog", "catalog.json");

	private static L2Locations LoadRealLocations()
	{
		var server = Environment.GetEnvironmentVariable("L2CONFIG_TEST_SERVER");
		var client = Environment.GetEnvironmentVariable("L2CONFIG_TEST_CLIENT");
		var file = Path.Combine(RepoRoot, "test-paths.local.json");
		if ((server is null || client is null) && File.Exists(file))
		{
			using var doc = JsonDocument.Parse(File.ReadAllText(file));
			server ??= doc.RootElement.TryGetProperty("server", out var s) ? s.GetString() : null;
			client ??= doc.RootElement.TryGetProperty("client", out var c) ? c.GetString() : null;
		}
		return new L2Locations(server, client is null ? null : L2Locations.ResolveClientSystemDir(client) ?? client);
	}

	/// <summary>A throwaway copy of the local install's config folders, player copies, world profile and client inis.</summary>
	public static L2Locations CopyRealFilesToTemp()
	{
		var root = Path.Combine(Path.GetTempPath(), "l2config-tests", Guid.NewGuid().ToString("N"));
		var install = Path.Combine(root, "L2Everdream");
		var client = Path.Combine(root, "client", "system");
		var copy = new L2Locations(install, client);

		CopyDirectory(RealLocations.GameConfigDir, copy.GameConfigDir);
		CopyDirectory(RealLocations.LoginConfigDir, copy.LoginConfigDir);
		CopyDirectory(RealLocations.PlayerGameConfigDir, copy.PlayerGameConfigDir);
		CopyDirectory(RealLocations.PlayerLoginConfigDir, copy.PlayerLoginConfigDir);
		Directory.CreateDirectory(Path.GetDirectoryName(copy.WorldProfilePath)!);
		File.Copy(RealLocations.WorldProfilePath, copy.WorldProfilePath);
		Directory.CreateDirectory(client);
		foreach (var name in new[] { "l2.ini", "Option.ini" })
		{
			File.Copy(Path.Combine(RealLocations.ClientSystemDir!, name), Path.Combine(client, name));
		}
		return copy;
	}

	private static void CopyDirectory(string from, string to)
	{
		if (!Directory.Exists(from))
		{
			return;
		}
		foreach (var file in Directory.EnumerateFiles(from, "*", SearchOption.AllDirectories))
		{
			var target = Path.Combine(to, Path.GetRelativePath(from, file));
			Directory.CreateDirectory(Path.GetDirectoryName(target)!);
			File.Copy(file, target);
		}
	}
}

/// <summary>A test that needs a real L2Everdream install and client (see TestPaths); skipped when they are not configured.</summary>
internal sealed class LocalInstallFactAttribute : FactAttribute
{
	public LocalInstallFactAttribute()
	{
		if (!TestPaths.RealLocations.HasServer || !TestPaths.RealLocations.HasClient
			|| !File.Exists(TestPaths.RealLocations.WorldProfilePath))
		{
			Skip = "Needs a local L2Everdream install and client: set test-paths.local.json or L2CONFIG_TEST_SERVER / L2CONFIG_TEST_CLIENT.";
		}
	}
}
