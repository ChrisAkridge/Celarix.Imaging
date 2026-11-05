using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Celarix.Imaging.Video
{
	/// <summary>What ffprobe reports about a video's first video stream, as ffmpeg will decode it.</summary>
	/// <param name="FrameRate">The frame rate as ffmpeg writes it, such as "30000/1001".</param>
	/// <param name="HasAudio">Whether the file has an audio stream; the first one is carried over to the output.</param>
	public sealed record VideoInfo(int Width, int Height, string FrameRate, double FramesPerSecond, double? DurationSeconds, bool HasAudio);

	/// <param name="Message">A short description of what is happening right now.</param>
	/// <param name="Fraction">How much of the whole job is done, from 0 to 1, or null if that is not known.</param>
	public sealed record VideoProgress(string Message, double? Fraction);

	/// <summary>Everything needed to run an effect over a video.</summary>
	/// <param name="Effect">Applied to every frame. Must be safe to call from several threads at once.</param>
	/// <param name="MeasureOutputSize">For effects whose output size depends on the frame, reports what size the
	/// effect would produce for a frame. Every frame is measured in a first pass, and the output is as large as the
	/// biggest result; smaller results are placed in the top-left corner on black. Leave null if the effect
	/// returns the same size as its input.</param>
	public sealed record VideoProcessingOptions(string InputPath,
		string OutputPath,
		VideoOutputFormat Format,
		Func<Image<Rgba32>, Image<Rgba32>> Effect,
		Func<Image<Rgba32>, (int Width, int Height)>? MeasureOutputSize = null,
		TimeSpan? Start = null,
		TimeSpan? Duration = null);

	/// <summary>
	/// Runs an image effect over every frame of a video using the ffmpeg and ffprobe found on PATH. Frames are
	/// streamed: one ffmpeg process decodes to raw pixels, the effect runs in memory, and a second ffmpeg process
	/// encodes the result, so nothing is written to disk besides the output file.
	/// </summary>
	public static class VideoProcessor
	{
		// Frames in flight at once are limited so that large frames (such as a 4096x4096 color map) do not use all memory.
		private const long MemoryBudgetBytes = 1_500_000_000;
		private const int MaxFramesInFlight = 8;

		// The share of the progress bar given to the canvas-sizing pass, when there is one.
		private const double SizingPassShare = 0.15;

		/// <summary>Checks that ffmpeg and ffprobe can be started, returning an explanation if not.</summary>
		public static bool TryFindTools(out string error)
		{
			foreach (var tool in new[] { "ffmpeg", "ffprobe" })
			{
				try
				{
					using var process = Process.Start(new ProcessStartInfo(tool, "-version")
					{
						UseShellExecute = false,
						RedirectStandardOutput = true,
						RedirectStandardError = true,
						CreateNoWindow = true
					})!;
					process.StandardOutput.ReadToEnd();
					process.WaitForExit();
				}
				catch (Win32Exception)
				{
					error = $"{tool} was not found. Install FFmpeg and make sure {tool} is on your PATH.";
					return false;
				}
			}

			error = string.Empty;
			return true;
		}

		public static VideoInfo Probe(string inputPath)
		{
			var output = RunToCompletion("ffprobe", new[]
			{
				"-v", "error", "-select_streams", "v:0",
				"-show_entries", "stream=width,height,r_frame_rate,avg_frame_rate,duration:stream_side_data=rotation:stream_tags=rotate:format=duration",
				"-of", "json", inputPath
			});

			using var document = JsonDocument.Parse(output);
			if (!document.RootElement.TryGetProperty("streams", out var streams) || streams.GetArrayLength() == 0)
			{
				throw new InvalidOperationException($"{Path.GetFileName(inputPath)} has no video stream.");
			}

			var stream = streams[0];
			var width = stream.GetProperty("width").GetInt32();
			var height = stream.GetProperty("height").GetInt32();

			// Prefer the average rate (accurate for variable frame rate video); fall back to the container's base rate.
			var frameRate = new[] { "avg_frame_rate", "r_frame_rate" }
				.Select(name => stream.TryGetProperty(name, out var value) ? value.GetString() : null)
				.FirstOrDefault(text => TryParseFraction(text, out var parsed) && parsed > 0)
				?? throw new InvalidOperationException("The video's frame rate could not be determined.");
			TryParseFraction(frameRate, out var framesPerSecond);

			// ffmpeg turns rotated videos upright while decoding, so frames come out with width and height swapped.
			var rotation = 0d;
			if (stream.TryGetProperty("side_data_list", out var sideData))
			{
				foreach (var entry in sideData.EnumerateArray())
				{
					if (entry.TryGetProperty("rotation", out var value) && value.ValueKind == JsonValueKind.Number)
					{
						rotation = value.GetDouble();
					}
				}
			}
			if (stream.TryGetProperty("tags", out var tags) && tags.TryGetProperty("rotate", out var rotateTag)
				&& double.TryParse(rotateTag.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var tagRotation))
			{
				rotation = tagRotation;
			}
			if (Math.Abs((int)Math.Round(rotation)) % 180 == 90)
			{
				(width, height) = (height, width);
			}

			double? duration = null;
			if (TryGetDouble(stream, "duration", out var streamDuration))
			{
				duration = streamDuration;
			}
			else if (document.RootElement.TryGetProperty("format", out var format) && TryGetDouble(format, "duration", out var formatDuration))
			{
				duration = formatDuration;
			}

			var audioOutput = RunToCompletion("ffprobe", new[]
			{
				"-v", "error", "-select_streams", "a:0", "-show_entries", "stream=codec_type", "-of", "csv=p=0", inputPath
			});

			return new VideoInfo(width, height, frameRate, framesPerSecond, duration, audioOutput.Trim().Length > 0);
		}

		public static Task ProcessAsync(VideoProcessingOptions options, IProgress<VideoProgress>? progress, CancellationToken cancellationToken) =>
			Task.Run(() =>
			{
				try
				{
					RunJob(options, progress, cancellationToken);
				}
				catch (AggregateException ex) when (ex.InnerExceptions.Count > 0)
				{
					// Parallel.For wraps what went wrong; the first real failure is what matters.
					System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex.InnerExceptions[0]).Throw();
				}
			}, cancellationToken);

		private static void RunJob(VideoProcessingOptions options, IProgress<VideoProgress>? progress, CancellationToken cancellationToken)
		{
			if (!TryFindTools(out var toolError)) { throw new InvalidOperationException(toolError); }

			var info = Probe(options.InputPath);
			var start = options.Start is { } s && s > TimeSpan.Zero ? s : TimeSpan.Zero;
			if (info.DurationSeconds is { } videoLength && start.TotalSeconds >= videoLength)
			{
				throw new InvalidOperationException(
					$"The start time ({FormatDuration(start)}) is at or past the end of the video, which is {FormatDuration(TimeSpan.FromSeconds(videoLength))} long.");
			}

			double? clipSeconds = options.Duration?.TotalSeconds;
			if (info.DurationSeconds is { } total)
			{
				var available = Math.Max(0d, total - start.TotalSeconds);
				clipSeconds = clipSeconds is { } requested ? Math.Min(requested, available) : available;
			}
			long? estimatedFrames = clipSeconds is { } seconds ? Math.Max(1, (long)Math.Ceiling(seconds * info.FramesPerSecond)) : null;

			var frameBytes = checked(info.Width * info.Height * 4);
			var canvasWidth = info.Width;
			var canvasHeight = info.Height;
			var stopwatch = Stopwatch.StartNew();
			var lastReport = TimeSpan.Zero;

			void Report(string message, double? fraction, bool force = false)
			{
				if (!force && stopwatch.Elapsed - lastReport < TimeSpan.FromMilliseconds(200)) { return; }
				lastReport = stopwatch.Elapsed;
				progress?.Report(new VideoProgress(message, fraction is { } f ? Math.Clamp(f, 0d, 1d) : null));
			}

			string? audioPath = null;
			try
			{
				if (options.MeasureOutputSize is { } measure)
				{
					(canvasWidth, canvasHeight) = MeasureCanvas(options, info, start, options.Duration, frameBytes, estimatedFrames, measure, Report, cancellationToken);
				}

				// Most video encoders need even dimensions.
				canvasWidth += canvasWidth % 2;
				canvasHeight += canvasHeight % 2;

				if (info.HasAudio)
				{
					audioPath = CutAudio(options, start);
				}

				EncodeFrames(options, info, start, frameBytes, canvasWidth, canvasHeight, estimatedFrames, options.MeasureOutputSize != null, audioPath, Report, cancellationToken);
				Report("Finished.", 1d, force: true);
			}
			catch
			{
				TryDelete(options.OutputPath);
				throw;
			}
			finally
			{
				if (audioPath != null) { TryDelete(audioPath); }
			}
		}

		/// <summary>
		/// Copies the chosen range of the first audio track into a temporary file. Muxing this file, rather than
		/// seeking within the original, is what keeps audio and video lined up: ffmpeg otherwise leaves a stream-copied
		/// audio track and the piped video offset from each other by the start time.
		/// </summary>
		private static string CutAudio(VideoProcessingOptions options, TimeSpan start)
		{
			var path = Path.Combine(Path.GetTempPath(), $"celarix_audio_{Guid.NewGuid():N}.mka");
			var arguments = new List<string> { "-v", "error", "-y", "-nostdin" };
			AddTimeRange(arguments, start, options.Duration);
			arguments.AddRange(new[] { "-i", options.InputPath, "-vn", "-sn", "-dn", "-map", "0:a:0", "-c:a", "copy", path });
			RunToCompletion("ffmpeg", arguments);

			return path;
		}

		private static (int Width, int Height) MeasureCanvas(VideoProcessingOptions options,
			VideoInfo info,
			TimeSpan start,
			TimeSpan? duration,
			int frameBytes,
			long? estimatedFrames,
			Func<Image<Rgba32>, (int Width, int Height)> measure,
			Action<string, double?, bool> report,
			CancellationToken cancellationToken)
		{
			var degree = Math.Min(Environment.ProcessorCount, MaxFramesInFlight);
			var inputBuffers = CreateBuffers(degree, frameBytes);
			var sizes = new (int Width, int Height)[degree];
			var largest = (Width: 1, Height: 1);

			using var decoder = StartDecoder(options.InputPath, info, start, duration);
			var count = RunBatches(decoder, frameBytes, inputBuffers, cancellationToken,
				(buffer, slot) =>
				{
					using var image = Image.LoadPixelData<Rgba32>(buffer.AsSpan(0, frameBytes), info.Width, info.Height);
					sizes[slot] = measure(image);
				},
				slot => largest = (Math.Max(largest.Width, sizes[slot].Width), Math.Max(largest.Height, sizes[slot].Height)),
				done => report($"Pass 1 of 2: measuring frame {done:#,0}{OfEstimate(estimatedFrames)} to size the output...",
					estimatedFrames is { } e ? SizingPassShare * done / e : null, false));

			decoder.ThrowIfFailed(cancellationToken);
			if (count == 0) { throw new InvalidOperationException("The video has no frames in the selected range."); }

			report($"Pass 1 of 2 done: output will be {largest.Width}x{largest.Height}.", SizingPassShare, true);
			return largest;
		}

		private static void EncodeFrames(VideoProcessingOptions options,
			VideoInfo info,
			TimeSpan start,
			int frameBytes,
			int canvasWidth,
			int canvasHeight,
			long? estimatedFrames,
			bool hadSizingPass,
			string? audioPath,
			Action<string, double?, bool> report,
			CancellationToken cancellationToken)
		{
			var canvasBytes = checked(canvasWidth * canvasHeight * 4);

			// Each frame in flight holds its raw input and output, plus the images the effect makes; budget for four copies.
			var perFrameBytes = (2L * frameBytes) + (2L * canvasBytes);
			var degree = (int)Math.Clamp(MemoryBudgetBytes / perFrameBytes, 1, Math.Min(Environment.ProcessorCount, MaxFramesInFlight));
			var inputBuffers = CreateBuffers(degree, frameBytes);
			var outputBuffers = CreateBuffers(degree, canvasBytes);

			var passShare = hadSizingPass ? 1d - SizingPassShare : 1d;
			var passOffset = hadSizingPass ? SizingPassShare : 0d;
			var passLabel = hadSizingPass ? "Pass 2 of 2: " : string.Empty;
			var stopwatch = Stopwatch.StartNew();

			using var decoder = StartDecoder(options.InputPath, info, start, options.Duration);
			using var encoder = StartEncoder(options, info, audioPath, canvasWidth, canvasHeight);
			var encoderInput = encoder.Process.StandardInput.BaseStream;

			try
			{
				var framesWritten = RunBatches(decoder, frameBytes, inputBuffers, cancellationToken,
					(buffer, slot) =>
					{
						using var image = Image.LoadPixelData<Rgba32>(buffer.AsSpan(0, frameBytes), info.Width, info.Height);
						using var result = options.Effect(image);
						ComposeOntoCanvas(result, outputBuffers[slot], canvasWidth, canvasHeight);
					},
					slot => encoderInput.Write(outputBuffers[slot], 0, canvasBytes),
					done =>
					{
						var framesPerSecond = done / Math.Max(0.001, stopwatch.Elapsed.TotalSeconds);
						var remaining = estimatedFrames is { } e && framesPerSecond > 0
							? $", about {FormatDuration(TimeSpan.FromSeconds(Math.Max(0, e - done) / framesPerSecond))} left"
							: string.Empty;
						report($"{passLabel}Processing frame {done:#,0}{OfEstimate(estimatedFrames)} ({framesPerSecond:0.0} frames/s{remaining})",
							estimatedFrames is { } total ? passOffset + (passShare * done / total) : null, false);
					});

				decoder.ThrowIfFailed(cancellationToken);
				if (framesWritten == 0) { throw new InvalidOperationException("The video has no frames in the selected range."); }

				report("Finishing the video file...", 0.99d, true);
				encoderInput.Close();
				encoder.Process.WaitForExit();
				encoder.ThrowIfFailed(cancellationToken);
			}
			catch (IOException)
			{
				// Writing to the encoder most likely failed because ffmpeg quit; its own message says why.
				if (encoder.Process.WaitForExit(5000))
				{
					encoder.ThrowIfFailed(cancellationToken);
				}
				throw;
			}
		}

		/// <summary>
		/// Copies an effect's result into a canvas-sized buffer, top-left, leaving the rest black. The buffer holds
		/// 4 bytes per pixel in R, G, B order with the fourth byte ignored by the encoder (the "rgb0" format).
		/// </summary>
		private static void ComposeOntoCanvas(Image<Rgba32> result, byte[] canvas, int canvasWidth, int canvasHeight)
		{
			Array.Clear(canvas);

			var copyWidth = Math.Min(result.Width, canvasWidth);
			var copyHeight = Math.Min(result.Height, canvasHeight);
			result.ProcessPixelRows(accessor =>
			{
				for (int y = 0; y < copyHeight; y++)
				{
					MemoryMarshal.AsBytes(accessor.GetRowSpan(y).Slice(0, copyWidth))
						.CopyTo(canvas.AsSpan(y * canvasWidth * 4, copyWidth * 4));
				}
			});
		}

		/// <summary>
		/// Reads frames from the decoder in batches, runs processFrame on each batch's frames in parallel, then calls
		/// consume for each one in order. Returns the number of frames read.
		/// </summary>
		private static long RunBatches(ToolProcess decoder,
			int frameBytes,
			byte[][] inputBuffers,
			CancellationToken cancellationToken,
			Action<byte[], int> processFrame,
			Action<int> consume,
			Action<long> onFramesDone)
		{
			var stream = decoder.Process.StandardOutput.BaseStream;
			var options = new ParallelOptions { MaxDegreeOfParallelism = inputBuffers.Length, CancellationToken = cancellationToken };
			long totalFrames = 0;

			while (true)
			{
				cancellationToken.ThrowIfCancellationRequested();

				var filled = 0;
				while (filled < inputBuffers.Length && ReadFrame(stream, inputBuffers[filled], frameBytes))
				{
					filled++;
				}
				if (filled == 0) { break; }

				Parallel.For(0, filled, options, slot => processFrame(inputBuffers[slot], slot));
				for (int slot = 0; slot < filled; slot++)
				{
					consume(slot);
				}

				totalFrames += filled;
				onFramesDone(totalFrames);

				// A short batch means the decoder ran out of frames.
				if (filled < inputBuffers.Length) { break; }
			}

			return totalFrames;
		}

		private static bool ReadFrame(Stream stream, byte[] buffer, int frameBytes)
		{
			var read = 0;
			while (read < frameBytes)
			{
				var count = stream.Read(buffer, read, frameBytes - read);
				if (count == 0) { return false; }
				read += count;
			}
			return true;
		}

		private static ToolProcess StartDecoder(string inputPath, VideoInfo info, TimeSpan start, TimeSpan? duration)
		{
			var arguments = new List<string> { "-v", "error", "-nostdin" };
			AddTimeRange(arguments, start, duration);
			arguments.AddRange(new[]
			{
				"-i", inputPath, "-map", "0:v:0", "-an", "-sn", "-dn",
				"-r", info.FrameRate, "-f", "rawvideo", "-pix_fmt", "rgba", "-"
			});

			return new ToolProcess("ffmpeg", arguments, redirectInput: false, redirectOutput: true);
		}

		private static ToolProcess StartEncoder(VideoProcessingOptions options, VideoInfo info, string? audioPath, int canvasWidth, int canvasHeight)
		{
			var arguments = new List<string>
			{
				"-y", "-v", "error", "-nostdin",
				// Input 0: the processed frames on stdin
				"-f", "rawvideo", "-pix_fmt", "rgb0", "-s", $"{canvasWidth}x{canvasHeight}", "-framerate", info.FrameRate, "-i", "-"
			};

			if (audioPath != null)
			{
				// Input 1: the original's audio, already cut to the same range
				arguments.AddRange(new[] { "-i", audioPath });
			}

			// Mappings are output options, so they follow every input.
			arguments.AddRange(new[] { "-map", "0:v:0" });
			if (audioPath != null)
			{
				arguments.AddRange(new[] { "-map", "1:a:0" });
			}

			arguments.AddRange(options.Format.VideoArguments.Split(' ', StringSplitOptions.RemoveEmptyEntries));
			if (audioPath != null)
			{
				arguments.AddRange(options.Format.AudioArguments.Split(' ', StringSplitOptions.RemoveEmptyEntries));
				arguments.Add("-shortest");
			}
			arguments.Add(options.OutputPath);

			return new ToolProcess("ffmpeg", arguments, redirectInput: true, redirectOutput: false);
		}

		private static void AddTimeRange(List<string> arguments, TimeSpan start, TimeSpan? duration)
		{
			if (start > TimeSpan.Zero)
			{
				arguments.AddRange(new[] { "-ss", FormatSeconds(start) });
			}
			if (duration is { } length)
			{
				arguments.AddRange(new[] { "-t", FormatSeconds(length) });
			}
		}

		private static string FormatSeconds(TimeSpan time) => time.TotalSeconds.ToString("0.######", CultureInfo.InvariantCulture);

		private static string FormatDuration(TimeSpan time) => time.TotalHours >= 1
			? $"{(int)time.TotalHours}h {time.Minutes}m"
			: time.TotalMinutes >= 1 ? $"{time.Minutes}m {time.Seconds}s" : $"{Math.Max(1, time.Seconds)}s";

		private static string OfEstimate(long? estimatedFrames) => estimatedFrames is { } e ? $" of about {e:#,0}" : string.Empty;

		private static byte[][] CreateBuffers(int count, int size)
		{
			var buffers = new byte[count][];
			for (int i = 0; i < count; i++)
			{
				buffers[i] = new byte[size];
			}
			return buffers;
		}

		private static bool TryParseFraction(string? text, out double value)
		{
			value = 0;
			if (string.IsNullOrWhiteSpace(text)) { return false; }

			var parts = text.Split('/');
			if (!double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var numerator)) { return false; }
			if (parts.Length == 1)
			{
				value = numerator;
				return true;
			}

			if (!double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var denominator) || denominator == 0)
			{
				return false;
			}

			value = numerator / denominator;
			return true;
		}

		private static bool TryGetDouble(JsonElement element, string name, out double value)
		{
			value = 0;
			return element.TryGetProperty(name, out var property)
				&& property.ValueKind == JsonValueKind.String
				&& double.TryParse(property.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out value);
		}

		private static string RunToCompletion(string tool, IEnumerable<string> arguments)
		{
			using var process = new ToolProcess(tool, arguments, redirectInput: false, redirectOutput: true);
			var output = process.Process.StandardOutput.ReadToEnd();
			process.Process.WaitForExit();
			process.ThrowIfFailed(CancellationToken.None);
			return output;
		}

		private static void TryDelete(string path)
		{
			try
			{
				if (File.Exists(path)) { File.Delete(path); }
			}
			catch (IOException)
			{
				// A leftover partial file is not worth hiding the real error.
			}
		}

		/// <summary>A running ffmpeg or ffprobe, whose messages on stderr are kept so failures can be explained.</summary>
		private sealed class ToolProcess : IDisposable
		{
			private const int MaxStoredLines = 30;
			private readonly Queue<string> errorLines = new();
			private readonly string toolName;

			public Process Process { get; }

			public ToolProcess(string fileName, IEnumerable<string> arguments, bool redirectInput, bool redirectOutput)
			{
				toolName = fileName;
				var startInfo = new ProcessStartInfo(fileName)
				{
					UseShellExecute = false,
					RedirectStandardInput = redirectInput,
					RedirectStandardOutput = redirectOutput,
					RedirectStandardError = true,
					CreateNoWindow = true,
					StandardErrorEncoding = Encoding.UTF8
				};
				foreach (var argument in arguments)
				{
					startInfo.ArgumentList.Add(argument);
				}

				Process = new Process { StartInfo = startInfo };
				Process.ErrorDataReceived += (_, e) =>
				{
					if (e.Data == null) { return; }
					lock (errorLines)
					{
						errorLines.Enqueue(e.Data);
						if (errorLines.Count > MaxStoredLines) { errorLines.Dequeue(); }
					}
				};
				Process.Start();
				Process.BeginErrorReadLine();
			}

			public void ThrowIfFailed(CancellationToken cancellationToken)
			{
				Process.WaitForExit();
				cancellationToken.ThrowIfCancellationRequested();
				if (Process.ExitCode == 0) { return; }

				string details;
				lock (errorLines) { details = string.Join(Environment.NewLine, errorLines); }
				throw new InvalidOperationException($"{toolName} stopped with exit code {Process.ExitCode}.{Environment.NewLine}{details}");
			}

			public void Dispose()
			{
				// If we are leaving early (cancelled or failed), do not leave ffmpeg running.
				try
				{
					if (!Process.HasExited)
					{
						Process.Kill(true);
						Process.WaitForExit(2000);
					}
				}
				catch (InvalidOperationException)
				{
					// Already gone.
				}
				Process.Dispose();
			}
		}
	}
}
