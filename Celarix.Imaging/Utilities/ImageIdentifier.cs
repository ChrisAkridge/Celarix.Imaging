using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Celarix.Imaging.Utilities
{
    public static class ImageIdentifier
    {
        private static readonly byte[] buffer = new byte[8];
        private static readonly byte[] bufferEnd = new byte[2];

        private static readonly byte[] bmp =
        {
            0x42, 0x4D
        }; // BMP "BM"

        private static readonly byte[] gif87a =
        {
            0x47, 0x49, 0x46, 0x38, 0x37,
            0x61
        }; // "GIF87a"

        private static readonly byte[] gif89a =
        {
            0x47, 0x49, 0x46, 0x38, 0x39,
            0x61
        }; // "GIF89a"

        private static readonly byte[] png =
        {
            0x89, 0x50, 0x4e, 0x47, 0x0D,
            0x0A, 0x1A, 0x0A
        }; // PNG "\x89PNG\x0D\0xA\0x1A\0x0A"

        private static readonly byte[] tiffI =
        {
            0x49, 0x49, 0x2A, 0x00
        }; // TIFF II "II\x2A\x00"

        private static readonly byte[] tiffM =
        {
            0x4D, 0x4D, 0x00, 0x2A
        }; // TIFF MM "MM\x00\x2A"

        private static readonly byte[] jpeg =
        {
            0xFF, 0xD8, 0xFF
        }; // JPEG JFIF (SOI "\xFF\xD8" and half next marker xFF)

        private static readonly byte[] jpegEnd =
        {
            0xFF, 0xD9
        }; // JPEG EOI "\xFF\xD9"

        // https://stackoverflow.com/a/49683945/2709212
        /// <summary>
        /// Reads the header of different image formats
        /// </summary>
        /// <param name="filePath">Image file</param>
        /// <returns>true if valid file signature (magic number/header marker) is found</returns>
        public static bool IsValidImageFile(string filePath)
        {
            using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                if (fs.Length > buffer.Length)
                {
                    fs.Read(buffer, 0, buffer.Length);
                    fs.Position = (int)fs.Length - bufferEnd.Length;
                    fs.Read(bufferEnd, 0, bufferEnd.Length);
                }

                fs.Close();
            }

            if (ByteArrayStartsWith(buffer, bmp)
                || ByteArrayStartsWith(buffer, gif87a)
                || ByteArrayStartsWith(buffer, gif89a)
                || ByteArrayStartsWith(buffer, png)
                || ByteArrayStartsWith(buffer, tiffI)
                || ByteArrayStartsWith(buffer, tiffM)) { return true; }

            if (ByteArrayStartsWith(buffer, jpeg))
            {
                // Offset 0 (Two Bytes): JPEG SOI marker (FFD8 hex)
                // Offest 1 (Two Bytes): Application segment (FF?? normally ??=E0)
                // Trailer (Last Two Bytes): EOI marker FFD9 hex
                if (ByteArrayStartsWith(bufferEnd, jpegEnd)) { return true; }
            }

            return false;
        }

        /// <summary>
        /// Returns a value indicating whether a specified subarray occurs within array
        /// </summary>
        /// <param name="a">Main array</param>
        /// <param name="b">Subarray to seek within main array</param>
        /// <returns>true if a array starts with b subarray or if b is empty; otherwise false</returns>
        private static bool ByteArrayStartsWith(IReadOnlyList<byte> a, IReadOnlyCollection<byte> b) =>
            a.Count >= b.Count && !b.Where((t, i) => a[i] != t).Any();
    }
}
