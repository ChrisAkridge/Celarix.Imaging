using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Celarix.Imaging.ByteViewCLI.Commands
{
    [Verb("Sort", HelpText = "Sorts the pixels in an image.")]
    internal sealed class Sort
    {
        [Option('i', "input", Required = true, HelpText = "The path to the image to sort.")]
        public string InputPath { get; set; }

        [Option('o', "output", Required = true, HelpText = "The path to save the sorted image to.")]
        public string OutputPath { get; set; }

        [Option('s', "sort-mode", Required = true, HelpText = "The sorting method to use on the pixels. Valid options are "
            + "\"rgb\", which treats pixels as 24-bit integers, "
            + "\"hsv\", which sorts first by hue angle, then by saturation, then by value, "
            + "and \"ycbcr\", which sorts first by luminance, then by Cb, then by Cr.")]
        public string SortMode { get; set; }

        public bool ValidateAndPrintErrors()
        {
            if (!File.Exists(InputPath))
            {
                Console.WriteLine("The input file does not exist.");
                return false;
            }

            string[] validSortModes = ["rgb", "hsv", "ycbcr"];
            if (!validSortModes.Contains(SortMode))
            {
                Console.WriteLine("Invalid sort mode. Valid options are \"rgb\", \"hsv\", and \"ycbcr\".");
                return false;
            }

            return true;
        }
    }
}
