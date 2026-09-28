using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground.Rendering
{
    internal static class Helpers
    {
        public static Task<SKImage> CreateSkImageFromImageSharp(Image<Rgba32> imageSharpImage,
            CancellationToken cancellationToken)
        {
            var buffer = new byte[imageSharpImage.Width * imageSharpImage.Height * 4];
            var pixel = 0;

            imageSharpImage.ProcessPixelRows(accessor =>
            {
                for (var y = 0; y < accessor.Height; y++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var row = accessor.GetRowSpan(y);
                    for (var x = 0; x < row.Length; x++)
                    {
                        var pixelData = row[x];
                        buffer[pixel++] = pixelData.R;
                        buffer[pixel++] = pixelData.G;
                        buffer[pixel++] = pixelData.B;
                        buffer[pixel++] = pixelData.A;
                    }
                }
            });

            var image = SKImage.FromPixelCopy(
                new SKImageInfo(imageSharpImage.Width, imageSharpImage.Height, SKColorType.Rgba8888),
                buffer,
                imageSharpImage.Width * 4);
            return Task.FromResult(image);
        }
    }
}
