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
                case RunOption.BinaryDrawMultipleFiles:
	                RunBinaryDrawMultipleFiles(parseResult);
					break;
                case RunOption.BinaryDrawFrames:
	                RunBinaryDrawFrames(parseResult);
	                break;
                case RunOption.BinaryDrawFramesMultipleFiles:
	                RunBinaryDrawFramesMultipleFiles(parseResult);
	                break;
                case RunOption.BinaryDrawCanvas:
	                RunBinaryDrawCanvas(parseResult);
	                break;
                case RunOption.BinaryDrawCanvasMultipleFiles:
	                RunBinaryDrawCanvasMultipleFiles(parseResult);
	                break;
                case RunOption.BinaryDrawFixedSize:
	                RunBinaryDrawFixedSize(parseResult);
	                break;
                case RunOption.ExtractColorChannel:
	                RunExtractColorChannel(parseResult);
	                break;
                case RunOption.ExtractBitPlane:
	                RunExtractBitPlane(parseResult);
	                break;
                case RunOption.ChromaSubsample:
	                RunChromaSubsample(parseResult);
	                break;
                case RunOption.ReduceBitDepth:
	                RunReduceBitDepth(parseResult);
	                break;
                case RunOption.ExportMockNTSCSignal:
	                RunExportMockNTSCSignal(parseResult);
	                break;
                case RunOption.ExportSimpleSignal:
	                RunExportSimpleSignal(parseResult);
	                break;
                case RunOption.UniqueColors:
	                RunUniqueColors(parseResult);
	                break;
                case RunOption.Sort:
	                RunSort(parseResult);
	                break;
                case RunOption.UniqueColorsInSpace:
	                RunUniqueColorsInSpace(parseResult);
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
        
        private static void RunBinaryDrawMultipleFiles(object parseResult)
		{
	        var options = (BinaryDrawMultipleFiles)parseResult;
	        if (!options.ValidateAndPrintErrors()) { return; }

	        var service = new BinaryDrawerService();
	        service.RunBinaryDrawMultipleFiles(options.InputNameOption, options.Input, options.OutputPath, options.BitDepth, options.ColorMode);
		}

		private static void RunBinaryDrawFrames(object parseResult)
		{
			var options = (BinaryDrawFrames)parseResult;

			if (!options.ValidateAndPrintErrors()) { return; }

			var service = new BinaryDrawerService();

			service.RunBinaryDrawFrames(options.InputPath, options.OutputPath, options.BitDepth, options.ColorMode, options.Width, options.Height);
		}
		
		private static void RunBinaryDrawFramesMultipleFiles(object parseResult)
		{
			var options = (BinaryDrawFramesMultipleFiles)parseResult;

			if (!options.ValidateAndPrintErrors()) { return; }

			var service = new BinaryDrawerService();

			service.RunBinaryDrawFramesMultipleFiles(options.InputNameOption, options.Input, options.OutputPath, options.BitDepth, options.ColorMode,
				options.Width, options.Height, options.DrawFileName);
		}
		
		private static void RunBinaryDrawCanvas(object parseResult)
		{
			var options = (BinaryDrawCanvas)parseResult;

			if (!options.ValidateAndPrintErrors()) { return; }

			var service = new BinaryDrawerService();

			service.RunBinaryDrawCanvas(options.InputPath, options.OutputPath, options.BitDepth, options.ColorMode);
		}
		
		private static void RunBinaryDrawCanvasMultipleFiles(object parseResult)
		{
			var options = (BinaryDrawCanvasMultipleFiles)parseResult;

			if (!options.ValidateAndPrintErrors()) { return; }

			var service = new BinaryDrawerService();

			service.RunBinaryDrawCanvasMultipleFiles(options.InputNameOption, options.Input, options.OutputPath, options.BitDepth, options.ColorMode);
		}
		
		private static void RunBinaryDrawFixedSize(object parseResult)
		{
			var options = (BinaryDrawFixedSize)parseResult;

			if (!options.ValidateAndPrintErrors()) { return; }

			var service = new BinaryDrawerService();

			service.RunBinaryDrawFixedSize(options.InputPath, options.OutputPath, options.Width, options.Height);
		}
		
		private static void RunExtractColorChannel(object parseResult)
		{
			var options = (ExtractColorChannel)parseResult;

			if (!options.ValidateAndPrintErrors()) { return; }

			var service = new ChromaPlaygroundService();

			service.RunExtractColorChannel(options.InputPath, options.OutputPath, options.Channel, options.FfmpegPath);
		}
		
		private static void RunExtractBitPlane(object parseResult)
		{
			var options = (ExtractBitPlane)parseResult;

			if (!options.ValidateAndPrintErrors()) { return; }

			var service = new ChromaPlaygroundService();

			service.RunExtractBitPlane(options.InputPath, options.OutputPath, options.Channel, options.Bit, options.FfmpegPath);
		}
		
		private static void RunChromaSubsample(object parseResult)
		{
			var options = (ChromaSubsample)parseResult;

			if (!options.ValidateAndPrintErrors()) { return; }

			var service = new ChromaPlaygroundService();

			service.RunChromaSubsample(options.InputPath, options.OutputPath, options.SubsamplingMode, options.FfmpegPath);
		}
		
		private static void RunReduceBitDepth(object parseResult)
		{
			var options = (ReduceBitDepth)parseResult;

			if (!options.ValidateAndPrintErrors()) { return; }

			var service = new ChromaPlaygroundService();

			service.RunReduceBitDepth(options.InputPath, options.OutputPath, options.BitDepth, options.ColorMode, options.FfmpegPath);
		}
		
		private static void RunExportMockNTSCSignal(object parseResult)
		{
			var options = (ExportMockNTSCSignal)parseResult;

			if (!options.ValidateAndPrintErrors()) { return; }

			var service = new ChromaPlaygroundService();

			service.RunExportMockNTSCSignal(options.InputPath, options.OutputPath);
		}
		
		private static void RunExportSimpleSignal(object parseResult)
		{
			var options = (ExportSimpleSignal)parseResult;

			if (!options.ValidateAndPrintErrors()) { return; }

			var service = new ChromaPlaygroundService();

			service.RunExportSimpleSignal(options.InputPath, options.OutputPath);
		}
		
		private static void RunUniqueColors(object parseResult)
		{
			var options = (UniqueColors)parseResult;

			if (!options.ValidateAndPrintErrors()) { return; }

			var service = new BinaryDrawerService();

			service.RunUniqueColors(options.InputPath, options.OutputPath);
		}
		
		private static void RunSort(object parseResult)
		{
			var options = (Sort)parseResult;

			if (!options.ValidateAndPrintErrors()) { return; }

			var service = new BinaryDrawerService();

			service.RunSort(options.InputPath, options.OutputPath, options.SortMode);
		}
		
		private static void RunUniqueColorsInSpace(object parseResult)
		{
			var options = (UniqueColorsInSpace)parseResult;

			if (!options.ValidateAndPrintErrors()) { return; }

			var service = new BinaryDrawerService();

			service.RunUniqueColorsInSpace(options.InputPath, options.OutputPath, options.BitDepth);
		}
		
		// thank you, Copilot!

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
