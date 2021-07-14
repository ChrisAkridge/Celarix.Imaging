using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Celarix.Imaging.Packing;
using SixLabors.ImageSharp;

namespace Celarix.Imaging
{
	internal static class Utilities
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
            var canvasHeight = (int)Math.Ceiling(size.Height / 256d);
            var canvasWidth = (int)Math.Ceiling(size.Width / 256d);
            
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
            List<string> parts = text.Split(pathSeparator).ToList();

            string driveLetter = parts[0];
            parts.RemoveAt(0);

            string fileName = parts[parts.Count - 1];
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

        internal static string GetLFPImageText(IList<string> fileNameList) =>
            fileNameList.Count == 1 ? fileNameList.First() : $"{fileNameList.First()} + {fileNameList.Count - 1}";

        internal static Size GetImageSize(string imageFilePath)
        {
            if (ImageSizeLoader.TryGetSize(imageFilePath, out Size size)) { return size; }

            using var image = Image.Load(imageFilePath);
            return image.Size();
        }

        internal static bool ExtensionImpliesFileIsImage(string filePath) =>
            filePath.EndsWith("gif", StringComparison.InvariantCultureIgnoreCase)
            || filePath.EndsWith("jpg", StringComparison.InvariantCultureIgnoreCase)
            || filePath.EndsWith("jpeg", StringComparison.InvariantCultureIgnoreCase)
            || filePath.EndsWith("png", StringComparison.InvariantCultureIgnoreCase);
    }
}
