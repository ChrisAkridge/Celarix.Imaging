using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Celarix.Imaging.Utilities;

namespace Celarix.Imaging.Packing
{
    public class PackingOptions
    {
        public string OutputPath { get; set; }
        public bool Multipicture { get; set; }
        public string InputPath { get; set; }
        public bool InputNamesPathListFile { get; set; }
        public bool Recursive { get; set; }

        public IEnumerable<string> GetImagePaths()
        {
            if (!InputNamesPathListFile)
            {
                return Directory
                    .EnumerateFiles(InputPath, "*", new EnumerationOptions
                    {
                        RecurseSubdirectories = Recursive, IgnoreInaccessible = true
                    })
                    .Where(Helpers.ExtensionImpliesFileIsImage);
            }

            return File.ReadAllLines(InputPath);
        }
    }
}
