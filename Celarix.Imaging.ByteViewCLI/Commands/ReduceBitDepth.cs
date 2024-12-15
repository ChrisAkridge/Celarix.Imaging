using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Celarix.Imaging.ByteViewCLI.Commands
{
    [Verb("ReduceBitDepth", HelpText = "Reduces the bit depth of an image.")]
    internal sealed class ReduceBitDepth
    {
        [Option('i', "input", Required = true, HelpText = "The path to the image to reduce the bit depth of.")]
        public string InputPath { get; set; }

        [Option('o', "output", Required = true, HelpText = "The folder to save the image with reduced bit depth to.")]
        public string OutputPath { get; set; }

        [Option('d', "bitDepth", Required = true, HelpText = "The bit depth to reduce the image to. Valid options are 16, 8, 4, 2, and 1.")]
        public string BitDepthText { get; set; }

        [Option('c', "colorMode", Required = true, HelpText = "The color mode paired with the bit depth. Valid options are "
            + "\"shift\", which shifts the top bits down to fit into the remaining bits (5:6:5 for 16-bit images, 3:3:2 for 8-bit images, 1:2:1 for 4-bit images), "
            + "\"topn\", which finds the top N colors by count and uses those as the palette (65,536 for 16-bit images, 256 for 8-bit images, 16 for 4-bit images, 4 for 2-bit images, and 2 for 1-bit images), "
            + "\"grayscale\", which converts the image to grayscale (available only for 8-, 4-, 2-, and 1-bit bit depths).")]
        public string ColorMode { get; set; }

        [Option('f', "ffmpegpath", Required = false, HelpText = "The path to an ffmpeg executable. If provided, only files processable by ffmpeg are valid for the -i/--input option," +
        "but all frames of the video will have their bit depths reduced and zipped back up into a video.")]
        public string FfmpegPath { get; set; }

        public int BitDepth => int.TryParse(BitDepthText, out int bitDepth) ? bitDepth : throw new ArgumentException("Invalid bit depth.");

        public bool ValidateAndPrintErrors()
        {
            if (!File.Exists(InputPath))
            {
                Console.WriteLine("The input file does not exist.");
                return false;
            }

            string[] validBitDepths = ["16", "8", "4", "2", "1"];
            if (!validBitDepths.Contains(BitDepthText))
            {
                Console.WriteLine("Invalid bit depth. Valid options are 16, 8, 4, 2, and 1.");
                return false;
            }

            string[] validColorModes = ["shift", "topn", "grayscale"];
            if (!validColorModes.Contains(ColorMode))
            {
                Console.WriteLine("Invalid color mode. Valid options are \"shift\", \"topn\", and \"grayscale\".");
                return false;
            }

            if (validColorModes.Equals("grayscale") && BitDepth == 16)
            {
	            Console.WriteLine("Grayscale mode is not available for 16-bit images.");
				return false;
            }
            
            if (validColorModes.Equals("shift") && BitDepth is 2 or 1)
            {
	            Console.WriteLine("Shift mode is not available for 2- or 1-bit images.");
	            return false;
            }

            if (!string.IsNullOrWhiteSpace(FfmpegPath) && !File.Exists(FfmpegPath))
            {
                Console.WriteLine("The ffmpeg executable does not exist at the provided path.");
                return false;
            }

            return true;
        }
    }
}
