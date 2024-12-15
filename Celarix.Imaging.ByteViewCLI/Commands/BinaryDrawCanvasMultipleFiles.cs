using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Celarix.Imaging.ByteViewCLI.Commands
{
    [Verb("BinaryDrawCanvasMultipleFiles", HelpText = "Draws the contents of multiple files onto a set of tiles for a zoomable canvas.")]
    internal sealed class BinaryDrawCanvasMultipleFiles
    {
        [Option('p', "option", Required = true, HelpText = "Specifies whether the input option for the -i/--input parameter. Valid options are filelist and inlinepaths. " +
            "filelist means that --input is the path of a text file containing a list of newline-delimited file paths to use as the inputs. " +
            "inlinepaths means that --input is a comma-delimited list of file paths specified directly.")]
        public string InputNameOption { get; set; }

        [Option('i', "input", Required = true, HelpText = "The path to the file to draw.")]
        public string Input { get; set; }

        [Option('o', "output", Required = true, HelpText = "The folder to save the canvas to.")]
        public string OutputPath { get; set; }

        [Option('d', "bitdepth", Required = true, HelpText = "The bit depth of the image. Options are 1, 2, 4, 8, 16, 24, and 32.")]
        public string BitDepthText { get; set; }

        [Option('c', "colormode", Required = true, HelpText = "The color mode of the image. Options are grayscale, rgb, and rgba. Not all bit depths support all color modes, choosing an unsupported options will default to another.")]
        public string ColorMode { get; set; }

        public int BitDepth => int.TryParse(BitDepthText, out int bitDepth) ? bitDepth : throw new ArgumentException("Invalid bit depth.");

        public bool ValidateAndPrintErrors()
        {
            if (InputNameOption != null) { InputNameOption = InputNameOption.ToLowerInvariant(); }
            if (ColorMode != null) { ColorMode = ColorMode.ToLowerInvariant(); }

            string[] validInputNameOptions = ["filelist", "inlinepaths"];
            if (!validInputNameOptions.Any(c => c.Equals(InputNameOption, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Input name option must be filelist or inlinepaths.");
                return false;
            }

            if (InputNameOption!.Equals("filelist", StringComparison.OrdinalIgnoreCase) && !File.Exists(Input))
            {
                Console.WriteLine("The input file list does not exist.");
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

            return true;
        }
    }
}
