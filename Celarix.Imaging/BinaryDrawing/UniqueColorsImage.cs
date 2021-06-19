using System;
using System.Collections.Generic;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Celarix.Imaging.BinaryDrawing
{
	public sealed class UniqueColorsImage<TPixel> where TPixel : unmanaged, IPixel<TPixel>
	{
		public int UniqueColors { get; }
		public Image<TPixel> Image { get; }

        public UniqueColorsImage(int uniqueColors, Image<TPixel> image)
        {
            UniqueColors = uniqueColors;
            Image = image;
        }
	}
}
