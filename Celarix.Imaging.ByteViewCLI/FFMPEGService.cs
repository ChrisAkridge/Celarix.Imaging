using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.ByteViewCLI
{
	internal static class FFMPEGService
	{
		public static void RunActionOnVideo(Action<string> frameAction,
			string ffmpegBinaryPath,
			string inputVideoPath,
			string outputFilePath)
		{
			Console.WriteLine($"Using FFmpeg on {inputVideoPath}...");
			var outputFolderPath = Path.GetDirectoryName(outputFilePath);
			Directory.CreateDirectory(outputFolderPath!);
			Directory.CreateDirectory(Path.Combine(outputFolderPath!, "out"));
			
			ExtractFrames(ffmpegBinaryPath, inputVideoPath, outputFilePath);
			var framesInFolder = Directory.GetFiles(Path.Combine(outputFolderPath!, "out"), "frame_*.png");
			Console.WriteLine($"Extracted {framesInFolder.Length:#,###} from {inputVideoPath}...");

			foreach (var framePath in framesInFolder)
			{
				Console.WriteLine($"Processing {Path.GetFileName(framePath)} of {framesInFolder.Length:#,###}...");
				frameAction(framePath);
			}

			Console.WriteLine($"Combining frames of {inputVideoPath} back into {outputFilePath}...");
			CombineFrames(ffmpegBinaryPath, inputVideoPath, outputFilePath);
		}
		
		private static void ExtractFrames(string ffmpegBinaryPath, string inputVideoPath, string outputFilePath)
		{
			var outputFolderPath = Path.GetDirectoryName(outputFilePath);
			var arguments = $"-i \"{inputVideoPath}\" \"{Path.Combine(outputFolderPath!, "out", "frame_%06d.png")}\"";
			RunFFMPEGWithArguments(ffmpegBinaryPath, arguments);
		}

		private static void CombineFrames(string ffmpegBinaryPath, string inputVideoPath, string outputFilePath)
		{
			var frameRate = GetFrameRateFromVideo(ffmpegBinaryPath, inputVideoPath);
			JoinFramesIntoVideo(ffmpegBinaryPath, outputFilePath, frameRate);
			ExtractAudio(ffmpegBinaryPath, inputVideoPath, outputFilePath);
			CombineAudioAndVideo(ffmpegBinaryPath, outputFilePath);
			
			var outputFolderPath = Path.GetDirectoryName(outputFilePath);
			var framesInFolder = Directory.GetFiles(Path.Combine(outputFolderPath!, "out"), "frame_*.png");
			foreach (var frame in framesInFolder)
			{
				File.Delete(frame);
			}
			Directory.Delete(Path.Combine(outputFolderPath!, "out"), true);
		}

		private static decimal GetFrameRateFromVideo(string ffmpegBinaryPath, string inputVideoPath)
		{
			var videoInfoOutput = RunFFMPEGCaptureOutput(ffmpegBinaryPath, $"-i \"{inputVideoPath}\"");
			var firstFPSIndex = videoInfoOutput.IndexOf("fps", StringComparison.InvariantCultureIgnoreCase) - 1;
			var fpsDigits = new List<char>();
			while (char.IsDigit(videoInfoOutput[firstFPSIndex]) || videoInfoOutput[firstFPSIndex] == '.' || videoInfoOutput[firstFPSIndex] == ' ')
			{
				fpsDigits.Add(videoInfoOutput[firstFPSIndex]);
				firstFPSIndex--;
			}
			var fpsString = new string(Enumerable.Reverse(fpsDigits).ToArray()).Trim();
			return decimal.Parse(fpsString);
		}
		
		private static void JoinFramesIntoVideo(string ffmpegBinaryPath, string outputFileName, decimal frameRate)
		{
			var outputFolderPath = Path.GetDirectoryName(outputFileName);
			var arguments =
				$"-framerate {frameRate} -i \"{Path.Combine(outputFolderPath!, "out", "frame_%06d.png")}\" -c:v libx264 -pix_fmt yuv420p \"{Path.Combine(outputFolderPath!, "out", Path.GetFileName(outputFileName))}\"";
			RunFFMPEGWithArguments(ffmpegBinaryPath, arguments);
		}

		private static void ExtractAudio(string ffmpegBinaryPath, string inputVideoPath, string outputFilePath)
		{
			var outputFolderPath = Path.GetDirectoryName(outputFilePath);
			var arguments = $"-i \"{inputVideoPath}\" -vn -acodec copy \"{Path.Combine(outputFolderPath!, "out", "temp_audio.mka")}\"";
			RunFFMPEGWithArguments(ffmpegBinaryPath, arguments);
		}

		private static void CombineAudioAndVideo(string ffmpegBinaryPath, string outputFileName)
		{
			var outputFolderPath = Path.GetDirectoryName(outputFileName);
			var arguments = $"-i \"{Path.Combine(outputFolderPath!, "out", Path.GetFileName(outputFileName))}\" -i \"{Path.Combine(outputFolderPath!, "out", "temp_audio.mka")}\" -c:v copy -c:a aac -strict experimental \"{outputFileName}\"";
			RunFFMPEGWithArguments(ffmpegBinaryPath, arguments);
		}

		private static void RunFFMPEGWithArguments(string ffmpegBinaryPath, string arguments)
		{
			using var process = new Process();

			process.StartInfo = new ProcessStartInfo
			{
				FileName = ffmpegBinaryPath,
				Arguments = arguments,
				UseShellExecute = false,
				RedirectStandardError = true,
				CreateNoWindow = true
			};

			process.ErrorDataReceived += (_, e) =>
			{
				if (e.Data != null)
				{
					Console.WriteLine(e.Data);
				}
			};
			process.Start();
			process.BeginErrorReadLine();
			process.WaitForExit();
		}
		
		private static string RunFFMPEGCaptureOutput(string ffmpegBinaryPath, string arguments)
		{
			using var process = new Process();

			process.StartInfo = new ProcessStartInfo
			{
				FileName = ffmpegBinaryPath,
				Arguments = arguments,
				UseShellExecute = false,
				RedirectStandardError = true,
				CreateNoWindow = true
			};

			process.Start();
			var output = process.StandardError.ReadToEnd();
			process.WaitForExit();

			return output;
		}
	}
}
