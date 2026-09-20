namespace L2Config.Core.Client;

/// <summary>
/// Draws a <see cref="MeshModel"/> into a small picture: orthographic, depth-buffered, painted with the model's own
/// skins and lit by one light. No graphics card and no graphics library, so it draws the same on every machine and in
/// the developer's snapshot mode.
/// </summary>
public static class MeshRenderer
{
	/// <summary>Lineage 2 models look along +Y, so this turns the model to face the viewer three-quarters on.</summary>
	public const double FacingYaw = 200;

	/// <summary>
	/// One frame. <paramref name="skins"/> are the model's textures in the order the client lists them; with none,
	/// the model is drawn in one colour.
	/// </summary>
	public static IconImage Render(MeshModel model, IReadOnlyList<IconImage?>? skins, int size,
		double yaw = FacingYaw, double pitch = 12)
	{
		var a = yaw * Math.PI / 180;
		var b = pitch * Math.PI / 180;
		double ca = Math.Cos(a), sa = Math.Sin(a), cb = Math.Cos(b), sb = Math.Sin(b);

		var n = model.VertexCount;
		var x = new double[n];
		var y = new double[n];
		var depth = new double[n];
		var light = new double[n];
		double lx = -0.4, ly = -0.72, lz = 0.57;
		var ll = Math.Sqrt(lx * lx + ly * ly + lz * lz);
		lx /= ll; ly /= ll; lz /= ll;

		double minX = double.MaxValue, maxX = double.MinValue, minY = double.MaxValue, maxY = double.MinValue;
		for (var i = 0; i < n; i++)
		{
			double px = model.Points[i * 3], py = model.Points[i * 3 + 1], pz = model.Points[i * 3 + 2];
			double rx = px * ca - py * sa, ry = px * sa + py * ca;
			double vy = ry * cb - pz * sb, vz = ry * sb + pz * cb;
			x[i] = rx;
			y[i] = vz;
			depth[i] = vy;
			minX = Math.Min(minX, rx); maxX = Math.Max(maxX, rx);
			minY = Math.Min(minY, vz); maxY = Math.Max(maxY, vz);

			double nx = model.Normals[i * 3], ny = model.Normals[i * 3 + 1], nz = model.Normals[i * 3 + 2];
			double tx = nx * ca - ny * sa, ty = nx * sa + ny * ca;
			double wy = ty * cb - nz * sb, wz = ty * sb + nz * cb;
			var len = Math.Sqrt(tx * tx + wy * wy + wz * wz);
			light[i] = len <= 0 ? 0 : Math.Clamp((tx * lx + wy * ly + wz * lz) / len, 0, 1);
		}

		var pixels = new byte[size * size * 4];
		if (n == 0)
		{
			return new IconImage(size, size, pixels);
		}
		var span = Math.Max(maxX - minX, maxY - minY);
		if (span <= 0)
		{
			return new IconImage(size, size, pixels);
		}
		// The model is measured once from the front, so it does not swell and shrink while it turns.
		var pad = size * 0.06;
		var scale = (size - 2 * pad) / span;
		var cx = (minX + maxX) / 2;
		var cy = (minY + maxY) / 2;
		for (var i = 0; i < n; i++)
		{
			x[i] = (x[i] - cx) * scale + size / 2.0;
			y[i] = size - ((y[i] - cy) * scale + size / 2.0);
		}

		var zbuf = new double[size * size];
		Array.Fill(zbuf, double.MaxValue);

		for (var t = 0; t < model.Triangles.Length; t += 3)
		{
			int i0 = model.Triangles[t], i1 = model.Triangles[t + 1], i2 = model.Triangles[t + 2];
			double x0 = x[i0], x1 = x[i1], x2 = x[i2], y0 = y[i0], y1 = y[i1], y2 = y[i2];
			var area = (x1 - x0) * (y2 - y0) - (x2 - x0) * (y1 - y0);
			if (Math.Abs(area) < 1e-9)
			{
				continue;
			}
			var face = t / 3;
			var skinIndex = face < model.TriangleSkins.Length ? model.TriangleSkins[face] : 0;
			var skin = skins is not null && skinIndex < skins.Count ? skins[skinIndex] : null;

			var left = (int)Math.Max(Math.Floor(Math.Min(x0, Math.Min(x1, x2))), 0);
			var right = (int)Math.Min(Math.Ceiling(Math.Max(x0, Math.Max(x1, x2))), size - 1);
			var top = (int)Math.Max(Math.Floor(Math.Min(y0, Math.Min(y1, y2))), 0);
			var bottom = (int)Math.Min(Math.Ceiling(Math.Max(y0, Math.Max(y1, y2))), size - 1);

			for (var py = top; py <= bottom; py++)
			{
				for (var px = left; px <= right; px++)
				{
					double sx = px + 0.5, sy = py + 0.5;
					var w0 = ((x1 - x0) * (sy - y0) - (sx - x0) * (y1 - y0)) / area;
					var w1 = ((sx - x0) * (y2 - y0) - (x2 - x0) * (sy - y0)) / area;
					var w2 = 1 - w0 - w1;
					if (w0 < 0 || w1 < 0 || w2 < 0)
					{
						continue;
					}
					var z = w2 * depth[i0] + w1 * depth[i1] + w0 * depth[i2];
					var at = py * size + px;
					if (z >= zbuf[at])
					{
						continue;
					}

					byte r = 214, g = 200, blue = 240;
					if (skin is not null)
					{
						var u = w2 * model.Uvs[i0 * 2] + w1 * model.Uvs[i1 * 2] + w0 * model.Uvs[i2 * 2];
						var v = w2 * model.Uvs[i0 * 2 + 1] + w1 * model.Uvs[i1 * 2 + 1] + w0 * model.Uvs[i2 * 2 + 1];
						Sample(skin, u, v, out r, out g, out blue);
					}

					zbuf[at] = z;
					// Game skins are painted for a lit world; on a small dark card they need lifting.
					var lit = 1.5 * (0.5 + 0.5 * Math.Clamp(w2 * light[i0] + w1 * light[i1] + w0 * light[i2], 0, 1));
					pixels[at * 4] = Lift(blue, lit);
					pixels[at * 4 + 1] = Lift(g, lit);
					pixels[at * 4 + 2] = Lift(r, lit);
					pixels[at * 4 + 3] = 255;
				}
			}
		}
		return new IconImage(size, size, pixels);
	}

	/// <summary>Every frame of a full turn, for a model that turns on the spot.</summary>
	public static IReadOnlyList<IconImage> Turntable(MeshModel model, IReadOnlyList<IconImage?>? skins, int size, int frames)
	{
		var turn = new List<IconImage>(frames);
		for (var i = 0; i < frames; i++)
		{
			turn.Add(Render(model, skins, size, FacingYaw + 360.0 * i / frames));
		}
		return turn;
	}

	private static byte Lift(byte channel, double light) => (byte)Math.Clamp(channel * light, 0, 255);

	private static void Sample(IconImage skin, double u, double v, out byte r, out byte g, out byte b)
	{
		var tx = (int)(Wrap(u) * skin.Width);
		var ty = (int)(Wrap(v) * skin.Height);
		tx = Math.Clamp(tx, 0, skin.Width - 1);
		ty = Math.Clamp(ty, 0, skin.Height - 1);
		var at = (ty * skin.Width + tx) * 4;
		b = skin.Bgra[at];
		g = skin.Bgra[at + 1];
		r = skin.Bgra[at + 2];
	}

	private static double Wrap(double value)
	{
		value -= Math.Floor(value);
		return value is >= 0 and < 1 ? value : 0;
	}
}
