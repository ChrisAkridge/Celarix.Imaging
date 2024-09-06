using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.ColorSpaces.Conversion;

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
			var rgb = new Rgb(color.R / 255f, color.G / 255f, color.B / 255f);
			var hsv = ColorSpaceConverter.ToHsv(rgb);
			H = hsv.H;
			S = hsv.S;
			V = hsv.V;
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
