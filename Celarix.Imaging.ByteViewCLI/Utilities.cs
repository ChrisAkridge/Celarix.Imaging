using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Celarix.Imaging.ByteViewCLI
{
    internal static class Utilities
    {
        public static void SaveImage(string outputPath, Image<Rgba32> image)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
            image.SaveAsPng(outputPath);
            var savedFileInfo = new FileInfo(outputPath);
            Console.WriteLine($"Image saved to {savedFileInfo.FullName} ({savedFileInfo.Length:#,###} bytes).");
        }

        public static string[] LoadFilesFromInput(string inputOption, string input)
        {
	        var fileList = (inputOption.Equals("filelist", StringComparison.InvariantCultureIgnoreCase)
				? File.ReadAllLines(input)
		        : inputOption.Equals("inlinepaths", StringComparison.InvariantCultureIgnoreCase)
					? input.Split(',')
					: throw new ArgumentException($"Invalid input option {inputOption}."))
		        .Select(f => f.Trim())
		        .ToArray();

			var invalidFiles = fileList.Where(f => !File.Exists(f)).ToArray();

			foreach (var invalidFilePath in invalidFiles)
			{
				Console.WriteLine($"The file {invalidFilePath} does not exist.");
			}

			if (invalidFiles.Length > 0) { throw new FileNotFoundException(); }

			return fileList;
		}
    }
}
