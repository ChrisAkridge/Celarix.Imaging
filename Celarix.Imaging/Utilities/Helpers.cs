using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Celarix.Imaging.Packing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Celarix.Imaging.Utilities
{
	internal static class Helpers
	{
        public static Size GetSizeFromCount(long count)
        {
            var squareRoot = (long)Math.Sqrt(count);
            Size result;
            if (IsPerfectSquare(count)) { result = new Size((int)squareRoot, (int)squareRoot); }
            else
            {
                long height = squareRoot;
                long remainder = count - (squareRoot * squareRoot);
                height += (int)Math.Ceiling((double)remainder / squareRoot);

                result = new Size((int)squareRoot, (int)height);
            }

            return result;
        }

        public static Size GetCanvasSizeFromImageSize(Size size)
        {
            var (width, height) = size;
            var canvasHeight = (int)Math.Ceiling(height / (double)LibraryConfiguration.Instance.ZoomableCanvasTileEdgeLength);
            var canvasWidth = (int)Math.Ceiling(width / (double)LibraryConfiguration.Instance.ZoomableCanvasTileEdgeLength);
            
            return new Size(canvasWidth, canvasHeight);
        }

        /// <summary>
        /// Checks if an integer's square root is an integer.
        /// </summary>
        /// <param name="n">The integer to check.</param>
        /// <returns>
        /// True if <paramref name="n" /> is a perfect square, false if it is not.
        /// </returns>
        public static bool IsPerfectSquare(long n)
        {
            if (n < 1) { return false; }

            long squareRoot = (long)Math.Sqrt(n);
            return squareRoot * squareRoot == n;
        }

        public static int GetTextHeight(int imageHeight) => Math.Max(24, imageHeight / 30);

        public static bool TryShortenFilePath(string text, out string result)
        {
            char pathSeparator = Path.DirectorySeparatorChar;
            var parts = text.Split(pathSeparator).ToList();

            string driveLetter = parts[0];
            parts.RemoveAt(0);

            string fileName = parts[^1];
            parts.RemoveAt(parts.Count - 1);

            string longestPart = parts.OrderByDescending(p => p.Length).First();

            if (longestPart.Length == 3)
            {
                result = text;
                return false;
            }

            int longestPartIndex = parts.IndexOf(longestPart);
            parts[longestPartIndex] = "...";

            result = string.Concat(driveLetter, pathSeparator, string.Join(pathSeparator.ToString(), parts), pathSeparator, fileName);
            return true;
        }

        internal static Size GetImageSize(string imageFilePath)
        {
            if (ImageSizeLoader.TryGetSize(imageFilePath, out var size)) { return size; }

            using var image = Image.Load(imageFilePath);
            return image.Size;
        }

        internal static bool ExtensionImpliesFileIsImage(string filePath) =>
            filePath.EndsWith("gif", StringComparison.InvariantCultureIgnoreCase)
            || filePath.EndsWith("jpg", StringComparison.InvariantCultureIgnoreCase)
            || filePath.EndsWith("jpeg", StringComparison.InvariantCultureIgnoreCase)
            || filePath.EndsWith("png", StringComparison.InvariantCultureIgnoreCase);

        public static string FormatException(Exception ex) =>
            $"{ex.GetType().Name}: {ex.Message}{Environment.NewLine}{ex.StackTrace}";
    }
}
