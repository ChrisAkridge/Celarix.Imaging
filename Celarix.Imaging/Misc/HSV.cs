using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.Misc
{
	internal struct HSV
	{
		public float H { get; set; }
		public float S { get; set; }
		public float V { get; set; }
		
		public HSV(float h, float s, float v)
		{
			H = h;
			S = s;
			V = v;
		}

		public HSV(Rgba32 color)
		{
			float r = color.R / 255f;
			float g = color.G / 255f;
			float b = color.B / 255f;

			float max = Math.Max(r, Math.Max(g, b));
			float min = Math.Min(r, Math.Min(g, b));
			float delta = max - min;

			if (delta == 0)
			{
				H = 0;
			}
			else if (Math.Abs(max - r) < 0.001f)
			{
				H = 60 * (((g - b) / delta) % 6);
			}
			else if (Math.Abs(max - g) < 0.001f)
			{
				H = 60 * (((b - r) / delta) + 2);
			}
			else
			{
				H = 60 * (((r - g) / delta) + 4);
			}

			if (max == 0)
			{
				S = 0;
			}
			else
			{
				S = delta / max;
			}

			V = max;
		}
		
		public Rgba32 ToRgba32()
		{
			int hi = (int)(H / 60) % 6;
			float f = (H / 60) - (int)(H / 60);
			float p = V * (1 - S);
			float q = V * (1 - (f * S));
			float t = V * (1 - ((1 - f) * S));

			switch (hi)
			{
				case 0:
					return new Rgba32(V, t, p);
				case 1:
					return new Rgba32(q, V, p);
				case 2:
					return new Rgba32(p, V, t);
				case 3:
					return new Rgba32(p, q, V);
				case 4:
					return new Rgba32(t, p, V);
				default:
					return new Rgba32(V, p, q);
			}
		}
	}
}
