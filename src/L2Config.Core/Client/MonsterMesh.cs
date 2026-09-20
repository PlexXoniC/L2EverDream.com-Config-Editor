namespace L2Config.Core.Client;

/// <summary>
/// A monster's model in its reference pose: points, normals, texture coordinates and the triangles between them,
/// with the skin each triangle is painted from.
/// </summary>
public sealed record MeshModel(float[] Points, float[] Normals, float[] Uvs, int[] Triangles, int[] TriangleSkins)
{
	public int VertexCount => Points.Length / 3;
	public int TriangleCount => Triangles.Length / 3;

	/// <summary>How many skins this model paints itself with; the names come from the client's npc table.</summary>
	public int SkinCount => TriangleSkins.Length == 0 ? 0 : TriangleSkins.Max() + 1;
}

/// <summary>
/// Reads a Lineage 2 skeletal mesh (UE2, package version 123 / licensee 28) out of a <c>.ukx</c> package.
/// <para>
/// Lineage 2 leaves the stock UE2 point, wedge and face arrays empty and keeps the geometry in the LOD models: each
/// has a list of 52-byte wedges (position, a normal of length 512, texture coordinates, four bones and four weights)
/// and an index buffer of triangles over them. Those positions are the reference pose, so a still needs no skinning.
/// </para>
/// </summary>
public static class MonsterMesh
{
	/// <summary>Enough detail for a small picture without reading the whole highest-detail mesh.</summary>
	private const int TriangleBudget = 4000;

	public static MeshModel? Read(byte[] export)
	{
		try
		{
			return Parse(export);
		}
		catch (Exception ex) when (ex is IndexOutOfRangeException or ArgumentOutOfRangeException or OverflowException or InvalidDataException)
		{
			return null;
		}
	}

	private static MeshModel? Parse(byte[] export)
	{
		var r = new UePackage.Cursor(export);

		// UObject: the property list (these meshes have none, so this is the name "None").
		r.Index();

		// UPrimitive: bounding box and sphere.
		r.Skip(24 + 1 + 16);

		// ULodMesh
		var version = r.I32();
		r.I32();                                  // VertexCount
		SkipArray(r, 4);                          // Verts (packed; empty for a skeletal mesh)
		SkipIndexArray(r);                        // Textures
		r.Skip(12 + 12 + 12);                     // MeshScale, MeshOrigin, RotOrigin
		SkipArray(r, 2);                          // FaceLevel
		SkipArray(r, 8);                          // Faces (empty)
		SkipArray(r, 2);                          // CollapseWedgeThus
		SkipArray(r, 10);                         // Wedges (empty)
		// Materials: each says which of the mesh's skins it paints with.
		var materialCount = r.Index();
		var skinOfMaterial = new int[Math.Max(materialCount, 0)];
		for (var i = 0; i < skinOfMaterial.Length; i++)
		{
			r.U32();                              // PolyFlags
			skinOfMaterial[i] = r.I32();          // TextureIndex
		}
		r.Skip(4 + 4 + 4 + 4 + 4 + 4);            // MeshScaleMax, LODHysteresis, LODStrength, LODMinVerts, LODMorph, LODZDisplace
		if (version >= 3)
		{
			r.I32();                              // HasImpostor
			r.Index();                            // SpriteMaterial
			r.Skip(12 + 12 + 12 + 4);             // ImpLocation, ImpRotation, ImpScale, ImpColor
			r.Skip(4 + 4 + 4);                    // ImpSpaceMode, ImpDrawMode, ImpLightMode
		}
		if (version >= 4)
		{
			r.Skip(4);                            // SkinTesselationFactor
		}
		if (version >= 5)
		{
			r.Skip(4);                            // Lineage 2 only
		}

		// USkeletalMesh
		SkipArray(r, 12);                         // Points2
		var bones = r.Index();
		for (var i = 0; i < bones; i++)
		{
			r.Index();                            // name
			r.Skip(4 + 44 + 4 + 4);               // flags, joint position, child count, parent
		}
		r.Index();                                // Animation
		r.I32();                                  // SkeletalDepth
		if (r.Index() != 0)
		{
			return null;                          // WeightIndices is always empty in these packages
		}
		SkipArray(r, 4);                          // BoneInfluences
		SkipIndexArray(r);                        // AttachAliases
		SkipIndexArray(r);                        // AttachBoneNames
		SkipArray(r, 48);                         // AttachCoords

		var lodCount = r.Index();
		MeshModel? best = null;
		for (var i = 0; i < lodCount; i++)
		{
			var lod = ReadLod(r, skinOfMaterial);
			// The LODs run from most to least detailed: keep the smallest one that still looks right.
			if (lod is not null && (best is null || (lod.TriangleCount >= TriangleBudget && lod.TriangleCount < best.TriangleCount)))
			{
				best = lod;
			}
		}
		return best;
	}

	private static MeshModel? ReadLod(UePackage.Cursor r, int[] skinOfMaterial)
	{
		SkipArray(r, 4);                          // SkinningData
		SkipArray(r, 16);                         // SkinPoints
		r.I32();                                  // NumSoftWedges
		var sections = ReadSections(r);           // SoftSections: which triangles use which material
		ReadSections(r);                          // RigidSections (Lineage 2 monsters have none)
		var soft = ReadIndices(r);
		ReadIndices(r);                           // RigidIndices
		r.Skip(4 + 4 + 4);                        // FSkinVertexStream: Revision, two unknowns
		SkipArray(r, 32);                         // rigid vertices
		r.I32(); SkipArray(r, 8);                 // VertInfluences (lazy)
		r.I32(); SkipArray(r, 10);                // Wedges (lazy, empty)
		r.I32(); SkipArray(r, 8);                 // Faces (lazy, empty)
		r.I32(); SkipArray(r, 12);                // Points (lazy, empty)
		r.Skip(4 + 4);                            // LODDistanceFactor, LODHysteresis
		r.Skip(4 + 4 + 4 + 4);                    // NumSharedVerts, LODMaxInfluences, two unknowns
		r.I32();                                  // UseNewWedges (Lineage 2)

		var wedgeCount = r.Index();
		if (wedgeCount <= 0 || soft.Length < 3)
		{
			r.Skip(Math.Max(0, wedgeCount) * 52);
			return null;
		}
		var points = new float[wedgeCount * 3];
		var normals = new float[wedgeCount * 3];
		var uvs = new float[wedgeCount * 2];
		for (var i = 0; i < wedgeCount; i++)
		{
			points[i * 3] = r.F32();
			points[i * 3 + 1] = r.F32();
			points[i * 3 + 2] = r.F32();
			normals[i * 3] = r.F32();
			normals[i * 3 + 1] = r.F32();
			normals[i * 3 + 2] = r.F32();
			uvs[i * 2] = r.F32();
			uvs[i * 2 + 1] = r.F32();
			r.Skip(4 + 16);                       // four bones, four weights
		}

		var triangles = new int[soft.Length / 3 * 3];
		for (var i = 0; i < triangles.Length; i++)
		{
			if (soft[i] >= wedgeCount)
			{
				return null;
			}
			triangles[i] = soft[i];
		}

		// Each section owns a run of triangles and names the material — and so the skin — they are painted with.
		var skins = new int[triangles.Length / 3];
		foreach (var section in sections)
		{
			var skin = section.Material < skinOfMaterial.Length ? skinOfMaterial[section.Material] : 0;
			for (var face = section.FirstFace; face < section.FirstFace + section.NumFaces && face < skins.Length; face++)
			{
				skins[face] = Math.Max(skin, 0);
			}
		}
		return new MeshModel(points, normals, uvs, triangles, skins);
	}

	private static ushort[] ReadIndices(UePackage.Cursor r)
	{
		var count = r.Index();
		var indices = new ushort[Math.Max(count, 0)];
		for (var i = 0; i < indices.Length; i++)
		{
			indices[i] = r.U16();
		}
		r.I32();                                  // Revision
		return indices;
	}

	private static List<(int Material, int FirstFace, int NumFaces)> ReadSections(UePackage.Cursor r)
	{
		var count = r.Index();
		var sections = new List<(int, int, int)>(Math.Max(count, 0));
		for (var i = 0; i < count; i++)
		{
			var material = r.U16();
			r.Skip(12);                           // stream and wedge ranges, bone index, one unused field
			var firstFace = r.U16();
			var numFaces = r.U16();
			var boneMap = r.Index();              // Lineage 2 bone map
			r.Skip(boneMap * 4);
			sections.Add((material, firstFace, numFaces));
		}
		return sections;
	}

	private static void SkipArray(UePackage.Cursor r, int itemSize) => r.Skip(r.Index() * itemSize);

	private static void SkipIndexArray(UePackage.Cursor r)
	{
		var count = r.Index();
		for (var i = 0; i < count; i++)
		{
			r.Index();
		}
	}
}
