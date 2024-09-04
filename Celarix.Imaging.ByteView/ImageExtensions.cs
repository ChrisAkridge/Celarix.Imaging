using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace Celarix.Imaging.ByteView
{
	public static class ImageExtensions
	{
        public static System.Drawing.Image ToSystemDrawingImage<TPixel>(this Image<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
        {
            // https://swharden.com/CsharpDataVis/alt/drawing-with-ImageSharp.md
            var stream = new MemoryStream();
            image.SaveAsPng(stream);
            stream.Seek(0L, SeekOrigin.Begin);
            return System.Drawing.Image.FromStream(stream);
        }

        public static Image<TPixel> ToImageSharpImage<TPixel>(this System.Drawing.Image image)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            // https://stackoverflow.com/questions/1668469/system-drawing-image-to-stream-c-sharp
            var stream = new MemoryStream();
            image.Save(stream, ImageFormat.Png);
            stream.Seek(0L, SeekOrigin.Begin);
            return Image.Load<TPixel>(stream);
        }
    }
}
