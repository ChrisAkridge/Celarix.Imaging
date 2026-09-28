using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground
{
    internal static class Extensions
    {
        public static long EstimateImageMemoryUsage(this Image<Rgba32> image)
        {
            // Each pixel is 4 bytes (R, G, B, A)
            return (long)image.Width * image.Height * 4;
        }

        public static SixLabors.ImageSharp.Point ToImageSharpPoint(this System.Drawing.Point point)
        {
            return new SixLabors.ImageSharp.Point(point.X, point.Y);
        }

        public static SkiaSharp.SKPoint ToSKPoint(this System.Drawing.Point point)
        {
            return new SkiaSharp.SKPoint(point.X, point.Y);
        }

        public static SixLabors.ImageSharp.Rectangle ToImageSharpRect(this System.Drawing.Rectangle rectangle)
        {
            return new SixLabors.ImageSharp.Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        public static SkiaSharp.SKRect ToSKRect(this System.Drawing.Rectangle rectangle)
        {
            return new SkiaSharp.SKRect(rectangle.X, rectangle.Y, rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height);
        }

        public static string FormatBytes(this long bytes)
        {
            var isNegative = bytes < 0;
            bytes = Math.Abs(bytes);
            var suffixes = new[] { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            var suffixIndex = 0;
            while (bytes > 1024)
            {
                bytes /= 1024;
                suffixIndex += 1;
            }

            var formatted = $"{bytes} {suffixes[suffixIndex]}";
            return isNegative ? $"-{formatted}" : formatted;
        }
    }
}
