using System.Text;
using L2Config.Core.Ini;

namespace L2Config.Core.Client;

/// <summary>Which model and skins the client draws for one npc id.</summary>
public sealed record NpcModel(int Id, string Animation, string Mesh, IReadOnlyList<string> Textures)
{
	/// <summary>"LineageMonsters.antaras_m00" → the package file "LineageMonsters.ukx".</summary>
	public string MeshPackage => Mesh.Split('.')[0];

	public string MeshName => Mesh.Contains('.') ? Mesh[(Mesh.IndexOf('.') + 1)..] : Mesh;
}

/// <summary>
/// The client's <c>system\npcgrp.dat</c>: for every npc the game knows, the animation set, the skeletal mesh and the
/// skin textures the client draws it with. The file is encrypted exactly like <c>l2.ini</c> (Lineage2Ver413), so the
/// ini codec reads it; inside is a table of records, each starting with the npc id and the three names.
/// </summary>
public static class NpcGrp
{
	public static IReadOnlyDictionary<int, NpcModel> Read(string? clientSystemDir)
	{
		var empty = new Dictionary<int, NpcModel>();
		if (clientSystemDir is null)
		{
			return empty;
		}
		var path = Path.Combine(clientSystemDir, "npcgrp.dat");
		if (!File.Exists(path))
		{
			return empty;
		}
		try
		{
			var raw = Encoding.Latin1.GetBytes(L2IniCodec.Decode(File.ReadAllBytes(path), out _));
			return Parse(raw);
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException or NotSupportedException)
		{
			return empty;
		}
	}

	/// <summary>
	/// Records are read by their shape rather than by a fixed layout: an npc id, then three length-prefixed UTF-16
	/// names that all start with the package they live in. Anything that does not match is stepped over, so a record
	/// this reader does not understand costs one entry rather than the rest of the file.
	/// </summary>
	internal static Dictionary<int, NpcModel> Parse(ReadOnlySpan<byte> raw)
	{
		var models = new Dictionary<int, NpcModel>();
		var at = 4;   // the file starts with the record count
		while (at + 8 < raw.Length)
		{
			var id = BitConverter.ToInt32(raw[at..]);
			var next = at + 4;
			if (!TryName(raw, ref next, out var animation) || !animation.StartsWith("Lineage", StringComparison.OrdinalIgnoreCase)
				|| !TryName(raw, ref next, out var mesh) || !mesh.Contains('.'))
			{
				at++;
				continue;
			}
			var textureCount = next + 4 <= raw.Length ? BitConverter.ToInt32(raw[next..]) : -1;
			next += 4;
			var textures = new List<string>();
			var ok = textureCount is >= 0 and <= 16;
			for (var i = 0; ok && i < textureCount; i++)
			{
				if (TryName(raw, ref next, out var texture))
				{
					textures.Add(texture);
				}
				else
				{
					ok = false;
				}
			}
			if (!ok)
			{
				at++;
				continue;
			}
			if (id > 0)
			{
				models.TryAdd(id, new NpcModel(id, animation, mesh, textures));
			}
			at = next;
		}
		return models;
	}

	private static bool TryName(ReadOnlySpan<byte> raw, ref int at, out string name)
	{
		name = "";
		if (at + 4 > raw.Length)
		{
			return false;
		}
		var length = BitConverter.ToInt32(raw[at..]);
		if (length is < 2 or > 400 || length % 2 != 0 || at + 4 + length > raw.Length)
		{
			return false;
		}
		name = Encoding.Unicode.GetString(raw.Slice(at + 4, length)).TrimEnd('\0');
		at += 4 + length;
		return name.Length > 0;
	}
}
