using System;
using System.Collections.Generic;
using System.Text;
using SixLabors.ImageSharp;

namespace Celarix.Imaging.ByteView
{
	internal static class Utilities
	{
        /// <summary>
        /// Generates a suffix for file sizes.
        /// </summary>
        /// <param name="fileSize"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string GenerateFileSizeAbbreviation(ulong fileSize, out int number)
        {
            // TODO: This method is a GREAT candidate to go into ChrisAkridge.Common.
            char[] prefixes = { 'K', 'M', 'G', 'T', 'P', 'E', 'Z', 'Y' };

            if (fileSize < 1024UL)
            {
                number = (int)fileSize;
                return "B";
            }

            int prefixNumber = -1;
            while (fileSize >= 1024UL)
            {
                fileSize /= 1024UL;
                prefixNumber++;
            }

            number = (int)fileSize;
            return string.Concat(prefixes[prefixNumber], "B");
        }

        public static int GetTextHeight(int imageHeight) => Math.Max(24, imageHeight / 30);

        public static bool IsPerfectSquare(long n)
        {
            if (n < 1) { return false; }

            long squareRoot = (long)Math.Sqrt(n);
            return squareRoot * squareRoot == n;
        }

        public static Size GetSizeFromCount(long count)
        {
            var squareRoot = (long)Math.Sqrt(count);
            Size result;
            if (IsPerfectSquare(count)) { result = new Size((int)squareRoot, (int)squareRoot); }
            else
            {
                long height = squareRoot;
                long remainder = count - squareRoot * squareRoot;
                height += (int)Math.Ceiling((double)remainder / squareRoot);

                result = new Size((int)squareRoot, (int)height);
            }

            return result;
        }
    }
}
