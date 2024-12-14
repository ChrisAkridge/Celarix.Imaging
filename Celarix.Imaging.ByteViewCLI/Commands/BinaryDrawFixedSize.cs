using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Celarix.Imaging.ByteViewCLI.Commands
{
    [Verb("BinaryDrawFixedSize", HelpText = "Draws the contents of any file onto a single image of a specified size, with 24 bits per pixel and RGB 8:8:8 as the color mode.")]
    internal sealed class BinaryDrawFixedSize
    {
        [Option('i', "input", Required = true, HelpText = "The path to the file to draw.")]
        public string InputPath { get; set; }

        [Option('o', "output", Required = true, HelpText = "The folder to save the canvas to.")]
        public string OutputPath { get; set; }

        [Option('w', "width", Required = true, HelpText = "The width of the images in pixels.")]
        public string WidthText { get; set; }

        [Option('h', "height", Required = true, HelpText = "The height of the images in pixels.")]
        public string HeightText { get; set; }

        public int Width => int.TryParse(WidthText, out int width) ? width : throw new ArgumentException("Invalid width.");
        public int Height => int.TryParse(HeightText, out int height) ? height : throw new ArgumentException("Invalid height.");

        public bool ValidateAndPrintErrors()
        {
            if (!File.Exists(InputPath))
            {
                Console.WriteLine("The input file does not exist.");
                return false;
            }

            if (!int.TryParse(WidthText, out var width)
                || width <= 0)
            {
                Console.WriteLine("Width must be a positive integer.");
                return false;
            }

            if (!int.TryParse(HeightText, out var height)
                           || height <= 0)
            {
                Console.WriteLine("Height must be a positive integer.");
                return false;
            }

            return true;
        }
    }
}
