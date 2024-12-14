using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Celarix.Imaging.ByteViewCLI.Commands
{
    [Verb("BinaryDrawFrames", HelpText = "Draws the contents of any file as a series of fixed-size images.")]
    internal sealed class BinaryDrawFrames
    {
        [Option('i', "input", Required = true, HelpText = "The path to the file to draw.")]
        public string InputPath { get; set; }

        [Option('o', "output", Required = true, HelpText = "The folder to save the images to.")]
        public string OutputPath { get; set; }

        [Option('d', "bitdepth", Required = true, HelpText = "The bit depth of the image. Options are 1, 2, 4, 8, 16, 24, and 32.")]
        public string BitDepthText { get; set; }

        [Option('c', "colormode", Required = true, HelpText = "The color mode of the image. Options are grayscale, rgb, and rgba. Not all bit depths support all color modes, choosing an unsupported options will default to another.")]
        public string ColorMode { get; set; }

        [Option('w', "width", Required = true, HelpText = "The width of the images in pixels.")]
        public string WidthText { get; set; }

        [Option('h', "height", Required = true, HelpText = "The height of the images in pixels.")]
        public string HeightText { get; set; }

        public int BitDepth => int.TryParse(BitDepthText, out int bitDepth) ? bitDepth : throw new ArgumentException("Invalid bit depth.");
        public int Width => int.TryParse(WidthText, out int width) ? width : throw new ArgumentException("Invalid width.");
        public int Height => int.TryParse(HeightText, out int height) ? height : throw new ArgumentException("Invalid height.");

        public bool ValidateAndPrintErrors()
        {
            if (ColorMode != null) { ColorMode = ColorMode.ToLowerInvariant(); }

            if (!File.Exists(InputPath))
            {
                Console.WriteLine("The input file does not exist.");
                return false;
            }

            if (!int.TryParse(BitDepthText, out var bitDepth)
                || bitDepth is not (1 or 2 or 4 or 8 or 16 or 24 or 32))
            {
                Console.WriteLine("Bit depth must be 1, 2, 4, 8, 16, 24, or 32.");
                return false;
            }

            string[] validColorModes = ["grayscale", "rgb", "rgba"];
            if (!validColorModes.Any(c => c.Equals(ColorMode, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Color mode must be grayscale, rgb, or rgba.");
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
