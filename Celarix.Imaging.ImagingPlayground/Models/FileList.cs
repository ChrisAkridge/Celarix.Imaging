using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground.Models
{
    public sealed class FileList
    {
        private readonly string[] filePaths;

        public IReadOnlyList<string> FilePaths => filePaths;

        public FileList(IEnumerable<string> filePaths)
        {
            this.filePaths = filePaths.ToArray();
        }
    }
}
