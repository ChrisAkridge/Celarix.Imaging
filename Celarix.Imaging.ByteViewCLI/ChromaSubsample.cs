using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Celarix.Imaging.ByteViewCLI
{
    [Verb("ChromeSubsample", HelpText = "Subsamples the chroma channels of an image.")]
    internal sealed class ChromaSubsample
    {
        [Option('i', "input", Required = true, HelpText = "The path to the image to subsample.")]
        public string InputPath { get; set; }

        [Option('o', "output", Required = true, HelpText = "The folder to save the subsampled image to.")]
        public string OutputPath { get; set; }

        [Option('s', "subsampling", Required = true, HelpText = "The subsampling mode to use. Valid options are "
            + "\"4:2:2\" (2x1 blocks of chroma), \"4:2:0\" (2x2 blocks of chroma), \"4:1:1\" (4x1 blocks of chroma), "
            + "\"8:1:1\" (8x8 blocks of chroma), \"16:1:1\" (16x16 blocks of chroma), and \"256:1:1\" (256x256 blocks of chroma).")]
        public string SubsamplingMode { get; set; }

        [Option('f', "ffmpegpath", Required = false, HelpText = "The path to an ffmpeg executable. If provided, only files processable by ffmpeg are valid for the -i/--input option," +
        "but all frames of the video will have their chroma subsampled and zipped back up into a video.")]
        public string FfmpegPath { get; set; }

        public bool ValidateAndPrintErrors()
        {
            if (!File.Exists(InputPath))
            {
                Console.WriteLine("The input file does not exist.");
                return false;
            }

            string[] validSubsamplingModes = ["4:2:2", "4:2:0", "4:1:1", "8:1:1", "16:1:1", "256:1:1"];
            if (!validSubsamplingModes.Contains(SubsamplingMode))
            {
                Console.WriteLine("Invalid subsampling mode. Valid options are \"4:2:2\", \"4:2:0\", \"4:1:1\", \"8:1:1\", \"16:1:1\", and \"256:1:1\".");
                return false;
            }

            if (!File.Exists(FfmpegPath))
            {
                Console.WriteLine("The ffmpeg executable does not exist at the provided path.");
                return false;
            }

            return true;
        }
    }
}
