using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Celarix.Imaging.ByteViewCLI.Commands
{
    [Verb("ExtractColorChannel", HelpText = "Extracts a single color channel from an image.")]
    internal sealed class ExtractColorChannel
    {
        [Option('i', "input", Required = true, HelpText = "The path to the image to extract the color channel from.")]
        public string InputPath { get; set; }

        [Option('o', "output", Required = true, HelpText = "The folder to save the extracted color channel to.")]
        public string OutputPath { get; set; }

        [Option('c', "channel", Required = true, HelpText = "The color channel to extract.")]
        public string ChannelText { get; set; }

        [Option('f', "ffmpegpath", Required = false, HelpText = "The path to an ffmpeg executable. If provided, only files processable by ffmpeg are valid for the -i/--input option," +
        "but all frames of the video will have their color channels extracted and zipped back up into a video.")]
        public string FfmpegPath { get; set; }

        public ColorChannel Channel => Enum.TryParse<ColorChannel>(ChannelText, true, out var channel) ? channel : throw new ArgumentException("Invalid color channel.");

        public bool ValidateAndPrintErrors()
        {
            if (!File.Exists(InputPath))
            {
                Console.WriteLine("The input file does not exist.");
                return false;
            }

            if (!Enum.TryParse<ColorChannel>(ChannelText, true, out _))
            {
                Console.WriteLine($"Invalid color channel. Valid options are {string.Join(", ", Enum.GetNames<ColorChannel>().Select(n => n.ToLowerInvariant()))}");
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
