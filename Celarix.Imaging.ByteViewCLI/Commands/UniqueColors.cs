using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Celarix.Imaging.ByteViewCLI.Commands
{
    [Verb("UniqueColors", HelpText = "Counts all unique colors in an image.")]
    internal sealed class UniqueColors
    {
        [Option('i', "input", Required = true, HelpText = "The path to the image to analyze.")]
        public string InputPath { get; set; }

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
