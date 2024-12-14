using Celarix.Imaging.ByteViewCLI.Commands;
using CommandLine;

namespace Celarix.Imaging.ByteViewCLI
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            LibraryConfiguration.Instance = new LibraryConfiguration
            {
                BinaryDrawingReportsProgressEveryNPixels = 1048576,
                ZoomableCanvasTileEdgeLength = 1024
            };

            var runOption = ParseArguments(args, out object parseResult);

            switch (runOption)
            {
                case RunOption.BinaryDraw:
                    RunBinaryDraw(parseResult);
                    break;
                default:
                    Console.WriteLine("Errors encountered while parsing arguments:");
                    foreach (var error in (IEnumerable<Error>)parseResult)
                    {
                        Console.WriteLine("\t" + error.Tag);
                    }

                    return;
            }
        }

        private static void RunBinaryDraw(object parseResult)
        {
            var options = (BinaryDraw)parseResult;
            if (!options.ValidateAndPrintErrors()) { return; }

            var service = new BinaryDrawerService();
            service.RunBinaryDraw(options.InputPath, options.OutputPath, options.BitDepth, options.ColorMode);
        }

        private static RunOption ParseArguments(IEnumerable<string> args, out object parseResult)
        {
            var result = RunOption.Invalid;
            object parsed = null;

            Parser.Default.ParseArguments<BinaryDraw,
                BinaryDrawMultipleFiles,
                BinaryDrawFrames,
                BinaryDrawFramesMultipleFiles,
                BinaryDrawCanvas,
                BinaryDrawCanvasMultipleFiles,
                BinaryDrawFixedSize,
                ExtractColorChannel,
                ExtractBitPlane,
                ChromaSubsample,
                ReduceBitDepth,
                ExportMockNTSCSignal,
                ExportSimpleSignal,
                UniqueColors,
                Sort,
                UniqueColorsInSpace>(args)
                .WithParsed<BinaryDraw>(opts =>
                {
                    result = RunOption.BinaryDraw;
                    parsed = opts;
                })
                .WithParsed<BinaryDrawMultipleFiles>(opts =>
                {
                    result = RunOption.BinaryDrawMultipleFiles;
                    parsed = opts;
                })
                .WithParsed<BinaryDrawFrames>(opts =>
                {
                    result = RunOption.BinaryDrawFrames;
                    parsed = opts;
                })
                .WithParsed<BinaryDrawFramesMultipleFiles>(opts =>
                {
                    result = RunOption.BinaryDrawFramesMultipleFiles;
                    parsed = opts;
                })
                .WithParsed<BinaryDrawCanvas>(opts =>
                {
                    result = RunOption.BinaryDrawCanvas;
                    parsed = opts;
                })
                .WithParsed<BinaryDrawCanvasMultipleFiles>(opts =>
                {
                    result = RunOption.BinaryDrawCanvasMultipleFiles;
                    parsed = opts;
                })
                .WithParsed<BinaryDrawFixedSize>(opts =>
                {
                    result = RunOption.BinaryDrawFixedSize;
                    parsed = opts;
                })
                .WithParsed<ExtractColorChannel>(opts =>
                {
                    result = RunOption.ExtractColorChannel;
                    parsed = opts;
                })
                .WithParsed<ExtractBitPlane>(opts =>
                {
                    result = RunOption.ExtractBitPlane;
                    parsed = opts;
                })
                .WithParsed<ChromaSubsample>(opts =>
                {
                    result = RunOption.ChromaSubsample;
                    parsed = opts;
                })
                .WithParsed<ReduceBitDepth>(opts =>
                {
                    result = RunOption.ReduceBitDepth;
                    parsed = opts;
                })
                .WithParsed<ExportMockNTSCSignal>(opts =>
                {
                    result = RunOption.ExportMockNTSCSignal;
                    parsed = opts;
                })
                .WithParsed<ExportSimpleSignal>(opts =>
                {
                    result = RunOption.ExportSimpleSignal;
                    parsed = opts;
                })
                .WithParsed<UniqueColors>(opts =>
                {
                    result = RunOption.UniqueColors;
                    parsed = opts;
                })
                .WithParsed<Sort>(opts =>
                {
                    result = RunOption.Sort;
                    parsed = opts;
                })
                .WithParsed<UniqueColorsInSpace>(opts =>
                                                                                                                                                                                                                                                                                                                                                                          {
                    result = RunOption.UniqueColorsInSpace;
                    parsed = opts;
                })
                .WithNotParsed(errs =>
                {
                    result = RunOption.Invalid;
                    parsed = errs;
                });

            parseResult = parsed!;
            return result;
        }
    }
}
