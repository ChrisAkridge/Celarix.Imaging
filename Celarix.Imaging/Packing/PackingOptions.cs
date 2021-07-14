using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Celarix.Imaging.Packing
{
    public abstract class PackingOptions
    {
        public string OutputPath { get; set; }
        public bool Multipicture { get; set; }

        public abstract IEnumerable<string> GetImagePaths();
    }
    
	public sealed class FolderPackingOptions : PackingOptions
	{
		public string InputFolderPath { get; set; }
        public bool Recursive { get; set; }

        public override IEnumerable<string> GetImagePaths() =>
            Directory
                .EnumerateFiles(InputFolderPath, "*", new EnumerationOptions
                {
                    RecurseSubdirectories = Recursive, IgnoreInaccessible = true
                })
                .Where(Utilities.ExtensionImpliesFileIsImage);
    }

    public sealed class PathListPackingOptions : PackingOptions
    {
        public string PathListFilePath { get; set; }

        public override IEnumerable<string> GetImagePaths() => File.ReadAllLines(PathListFilePath);
    }
}
