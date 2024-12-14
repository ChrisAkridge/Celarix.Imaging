//	FileSource.cs
//
//	Converts a list of files to a byte array.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Celarix.Imaging.IO
{
    /// <summary>
    /// A sequence of file paths that can be converted into a byte array
    /// containing all the bytes of all the files.
    /// </summary>
    public sealed class FileSource
    {
        /// <summary>
        /// An array of all file paths in this source.
        /// </summary>
        private readonly string[] filePaths;

        /// <summary>
        /// An array of the sizes of all files in this source.
        /// </summary>
        private readonly int[] fileSizes;

        /// <summary>
        /// Gets an immutable list of all file paths in this source.
        /// </summary>
        public IReadOnlyList<string> FilePaths => Array.AsReadOnly(filePaths);
        /// <summary>
        /// Gets an immutable list of the sizes of all file paths in this source.
        /// </summary>
        public IReadOnlyList<int> FileSizes => Array.AsReadOnly(fileSizes);

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSource"/> class.
        /// </summary>
        /// <param name="cFilePaths">An array of file paths.</param>
        public FileSource(string[] cFilePaths)
        {
            if (cFilePaths == null || cFilePaths.Length == 0)
            {
                throw new ArgumentException("The provided file paths array was null or empty.",
                    nameof(cFilePaths));
            }

            filePaths = cFilePaths;
            fileSizes = new int[filePaths.Length];

            for (int i = 0; i < filePaths.Length; i++)
            {
                var info = new FileInfo(filePaths[i]);

                if (info.Length > int.MaxValue)
                {
                    string fileName = Path.GetFileName(filePaths[i]);
                    throw new IOException(
                        $"The file {fileName} is more than 2 GB, which is the maximum.");
                }

                fileSizes[i] = (int)info.Length;
            }
        }

        /// <summary>
        /// Returns the bytes of every file in this source, in order, as a byte array.
        /// </summary>
        /// <returns>The bytes of every file.</returns>
        public byte[] GetFiles()
        {
            long totalSize = fileSizes.Aggregate(0L, (current, fileSize) => current + fileSize);

            if (totalSize > int.MaxValue)
            {
                throw new IOException(
                    "The size of all files is larger than 2 GB, which is the maximum.");
            }

            int totalSizeInRange = (int)totalSize;

            var result = new byte[totalSizeInRange];
            int byteIndex = 0;
            for (int i = 0; i < filePaths.Length; i++)
            {
                using var reader = new BinaryReader(File.Open(filePaths[i], FileMode.Open));

                int bytesRead = reader.Read(result, byteIndex, fileSizes[i]);
                if (bytesRead != fileSizes[i])
                {
                    throw new IOException(
                        $"Read only {bytesRead} bytes out of a {fileSizes[i]}-byte file.");
                }
                byteIndex += fileSizes[i];
            }

            return result;
        }

        public NamedMultiStream GetStream() => new NamedMultiStream(filePaths);
    }
}
