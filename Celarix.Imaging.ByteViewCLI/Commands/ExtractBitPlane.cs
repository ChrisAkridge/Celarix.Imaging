using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Celarix.Imaging.ByteViewCLI.Commands
{
    [Verb("ExtractBitPlane", HelpText = "Extracts a single bit plane from an image. A pixel in the result is colored if the n-th bit is set in the original pixel for a given color channel, or all three if specified.")]
    internal sealed class ExtractBitPlane
    {
        [Option('i', "input", Required = true, HelpText = "The path to the image to extract the bit plane from.")]
        public string InputPath { get; set; }

        [Option('o', "output", Required = true, HelpText = "The folder to save the extracted bit plane to.")]
        public string OutputPath { get; set; }

        [Option('b', "bit", Required = true, HelpText = "The bit to extract.")]
        public string BitText { get; set; }

        [Option('c', "channel", HelpText = "The color channel to extract the bit plane from. Valid options are red, green, blue, and all.")]
        public string Channel { get; set; }

        [Option('f', "ffmpegpath", Required = false, HelpText = "The path to an ffmpeg executable. If provided, only files processable by ffmpeg are valid for the -i/--input option," +
        "but all frames of the video will have their bit planes extracted and zipped back up into a video.")]
        public string FfmpegPath { get; set; }

        public int Bit => int.TryParse(BitText, out int bit) ? bit : throw new ArgumentException("Invalid bit.");

        public bool ValidateAndPrintErrors()
        {
            if (!File.Exists(InputPath))
            {
                Console.WriteLine("The input file does not exist.");
                return false;
            }

            if (!int.TryParse(BitText, out var bit)
                || bit < 0
                || bit > 7)
            {
                Console.WriteLine("Bit must be an integer between 0 and 7.");
                return false;
            }

            var validColorChannels = new[] { "red", "green", "blue", "all" };
            if (Channel != null && !validColorChannels.Contains(Channel, StringComparer.OrdinalIgnoreCase))
            {
                Console.WriteLine($"Invalid color channel. Valid options are {string.Join(", ", validColorChannels)}.");
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
