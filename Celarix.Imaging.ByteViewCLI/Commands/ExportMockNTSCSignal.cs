using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Celarix.Imaging.ByteViewCLI.Commands
{
    [Verb("ExportMockNTSCSignal", HelpText = "Converts an image into a RAW audio file containing a mock-NTSC representation of the image.")]
    internal sealed class ExportMockNTSCSignal
    {
        [Option('i', "input", Required = true, HelpText = "The path to the image to convert.")]
        public string InputPath { get; set; }

        [Option('o', "output", Required = true, HelpText = "The path to save the RAW audio file to.")]
        public string OutputPath { get; set; }

        public bool ValidateAndPrintErrors()
        {
            if (!File.Exists(InputPath))
            {
                Console.WriteLine("The input file does not exist.");
                return false;
            }

            return true;
        }
    }
}
