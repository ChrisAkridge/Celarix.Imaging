using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.BinaryDrawing
{
	internal static class PixelBufferer
	{
        public static int[] BufferToPixels(byte[] buffer, int count, int bitDepth)
        {
            var pixels = bitDepth switch
            {
                1 => BufferTo1bppPixels(buffer, count),
                2 => BufferTo2bppPixels(buffer, count),
                4 => BufferTo4bppPixels(buffer, count),
                8 => BufferTo8bppPixels(buffer, count),
                16 => BufferTo16bppPixels(buffer, count),
                24 => BufferTo24bppPixels(buffer, count),
                32 => BufferTo32BppPixels(buffer, count),
                _ => throw new ArgumentException(nameof(bitDepth))
            };

            return pixels;
        }

        private static int[] BufferTo1bppPixels(byte[] buffer, int count)
        {
            var result = new int[count * 8];

            for (var i = 0; i < count; i++)
            {
                var b = buffer[i];
                result[i * 8] = b >> 7;
                result[(i * 8) + 1] = (b >> 6) & 1;
                result[(i * 8) + 2] = (b >> 5) & 1;
                result[(i * 8) + 3] = (b >> 4) & 1;
                result[(i * 8) + 4] = (b >> 3) & 1;
                result[(i * 8) + 5] = (b >> 2) & 1;
                result[(i * 8) + 6] = (b >> 1) & 1;
                result[(i * 8) + 7] = b & 1;
            }

            return result;
        }

        private static int[] BufferTo2bppPixels(byte[] buffer, int count)
        {
            var result = new int[count * 4];

            for (var i = 0; i < count; i++)
            {
                var b = buffer[i];
                result[i * 4] = b >> 6;
                result[(i * 4) + 1] = (b >> 4) & 3;
                result[(i * 4) + 2] = (b >> 2) & 3;
                result[(i * 4) + 3] = b & 3;
            }

            return result;
        }

        private static int[] BufferTo4bppPixels(byte[] buffer, int count)
        {
            var result = new int[count * 2];

            for (var i = 0; i < count; i++)
            {
                var b = buffer[i];
                result[i * 2] = b >> 4;
                result[(i * 2) + 1] = b & 15;
            }

            return result;
        }

        private static int[] BufferTo8bppPixels(byte[] buffer, int count)
        {
            var result = new int[count];

            for (var i = 0; i < count; i++) { result[i] = buffer[i]; }

            return result;
        }

        private static int[] BufferTo16bppPixels(byte[] buffer, int count)
        {
            var resultSize = (int)Math.Ceiling(count / 2f);
            var result = new int[resultSize];

            for (var i = 0; i < count; i += 2)
            {
                var high = buffer[i];
                var low = ((i + 1) < count)
                    ? buffer[i + 1]
                    : 0;

                result[i / 2] = (high << 8) | low;
            }

            return result;
        }

        private static int[] BufferTo24bppPixels(byte[] buffer, int count)
        {
            var resultSize = (int)Math.Ceiling(count / 3f);
            var result = new int[resultSize];

            for (var i = 0; i < count; i += 3)
            {
                var r = buffer[i];
                var g = ((i + 1) < count)
                    ? buffer[i + 1]
                    : 0;

                var b = ((i + 2) < count)
                    ? buffer[i + 2]
                    : 0;

                result[i / 3] = (r << 16) | (g << 8) | b;
            }

            return result;
        }

        private static int[] BufferTo32BppPixels(byte[] buffer, int count)
        {
            var resultSize = (int)Math.Ceiling(count / 4f);
            var result = new int[resultSize];

            for (var i = 0; i < count; i += 4)
            {
                var r = buffer[i];
                var g = ((i + 1) < count)
                    ? buffer[i + 1]
                    : 0;

                var b = ((i + 2) < count)
                    ? buffer[i + 2]
                    : 0;

                var a = ((i + 3) < count)
                    ? buffer[i + 3]
                    : 0;

                result[i / 4] = (r << 24) | (g << 16) | (b << 8) | a;
            }

            return result;
        }
	}
}
