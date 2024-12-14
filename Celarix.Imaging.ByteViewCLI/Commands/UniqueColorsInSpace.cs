using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Celarix.Imaging.ByteViewCLI.Commands
{
    [Verb("UniqueColorsInSpace", HelpText = "Converts an image into one that shows all the unique colors in that image, mapped onto an image large enough to represent every possible color. "
        + "Resolutions are 2x1 for 1-bit images, 2x2 for 2-bit images, 4x4 for 4-bit images, 16x16 for 8-bit images, 256x256 for 16-bit images, and 4096x4096 for 24-bit images. "
        + "Selecting a lower bit depth than the source will match colors by right-shifting the color channel bits to fit.")]
    internal sealed class UniqueColorsInSpace
    {
        [Option('i', "input", Required = true, HelpText = "The path to the image to convert.")]
        public string InputPath { get; set; }

        [Option('o', "output", Required = true, HelpText = "The path to save the converted image to.")]
        public string OutputPath { get; set; }

        [Option('b', "bitDepth", Required = true, HelpText = "The bit depth of the output image. Valid options are 1, 2, 4, 8, 16, and 24.")]
        public string BitDepthText { get; set; }

        public int BitDepth => int.TryParse(BitDepthText, out int bitDepth) ? bitDepth : throw new ArgumentException("Invalid bit depth.");

        public bool ValidateAndPrintErrors()
        {
            if (!File.Exists(InputPath))
            {
                Console.WriteLine("The input file does not exist.");
                return false;
            }

            if (BitDepth != 1 && BitDepth != 2 && BitDepth != 4 && BitDepth != 8 && BitDepth != 16 && BitDepth != 24)
            {
                Console.WriteLine("Invalid bit depth. Valid options are 1, 2, 4, 8, 16, and 24.");
                return false;
            }

            return true;
        }
    }
}
