using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Celarix.Imaging.Video;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Celarix.Imaging.ByteView
{
	/// <summary>An effect with its settings captured, ready to be run over video frames.</summary>
	/// <param name="Effect">The effect to apply to each frame.</param>
	/// <param name="MeasureOutputSize">For effects whose output size varies per frame; null if it never does.</param>
	/// <param name="Description">A one-line description of the effect and its settings.</param>
	/// <param name="Note">Anything worth warning about for this effect, or null.</param>
	public sealed record VideoEffectSettings(Func<Image<Rgba32>, Image<Rgba32>> Effect,
		Func<Image<Rgba32>, (int Width, int Height)> MeasureOutputSize,
		string Description,
		string Note);

	/// <summary>
	/// Applies the Chroma Playground's effect to every frame of a video file and writes the result as a new video.
	/// The dialog stays open beside the playground: the effect is read when Process Video is pressed, so it can be
	/// changed in the playground between runs. To layer effects, use one run's output as the next run's input.
	/// </summary>
	public partial class VideoProcessForm : Form
	{
		private readonly Func<VideoEffectSettings> getEffectSettings;

		private CancellationTokenSource cancellation;
		private bool isRunning;

		/// <param name="getEffectSettings">Returns the playground's current effect and settings, or null if no effect is chosen.</param>
		public VideoProcessForm(Func<VideoEffectSettings> getEffectSettings)
		{
			this.getEffectSettings = getEffectSettings;

			InitializeComponent();

			LabelRangeHint.MaximumSize = new System.Drawing.Size(500, 0);
			ComboFormat.Items.AddRange(VideoOutputFormat.All.Cast<object>().ToArray());
			ComboFormat.SelectedIndex = 0;
			SetRangeControlsEnabled();
			RefreshEffectDescription();
		}

		public bool IsRunning => isRunning;

		private VideoOutputFormat SelectedFormat => (VideoOutputFormat)ComboFormat.SelectedItem;

		/// <summary>
		/// Updates the effect shown at the top to match the playground. Does nothing while a video is being processed,
		/// since that run keeps the settings it started with.
		/// </summary>
		public void RefreshEffectDescription()
		{
			if (isRunning) { return; }

			var settings = getEffectSettings();
			ShowEffect(settings?.Description ?? "(none chosen - pick one in the playground)", settings?.Note);
		}

		private void ShowEffect(string description, string note)
		{
			LabelEffect.Text = $"Effect: {description}";
			LabelEffectNote.Text = note ?? string.Empty;
			LabelEffectNote.Visible = !string.IsNullOrEmpty(note);
		}

		private void VideoProcessForm_Load(object sender, EventArgs e)
		{
			if (!VideoProcessor.TryFindTools(out var error))
			{
				SetStatus(error);
				ButtonStart.Enabled = false;
			}
		}

		private void VideoProcessForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (isRunning)
			{
				e.Cancel = true;
				MessageBox.Show(this, "A video is being processed. Press Cancel to stop it first.", "Process Video",
					MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		#region Choosing files
		private void ButtonBrowseInput_Click(object sender, EventArgs e)
		{
			// Open where the path in the box points, so a folder of inputs can be left there and reused.
			OFDInput.InitialDirectory = ExistingDirectoryOf(TextInput.Text) ?? string.Empty;
			OFDInput.FileName = File.Exists(TextInput.Text) ? Path.GetFileName(TextInput.Text) : string.Empty;

			if (OFDInput.ShowDialog(this) == DialogResult.OK)
			{
				TextInput.Text = OFDInput.FileName;

				// A newly picked input gets a fresh output name, in the output folder already chosen if there is one.
				SuggestOutputPath();
			}
		}

		private void TextInput_TextChanged(object sender, EventArgs e)
		{
			// Offer an output path when an input is typed or pasted and the output is still blank.
			if (string.IsNullOrWhiteSpace(TextOutput.Text) && File.Exists(TextInput.Text))
			{
				SuggestOutputPath();
			}
		}

		/// <summary>
		/// Names the output after the input. It goes in the folder the output box already points to, or else beside the input.
		/// </summary>
		private void SuggestOutputPath()
		{
			if (!File.Exists(TextInput.Text)) { return; }

			var directory = ExistingDirectoryOf(TextOutput.Text) ?? Path.GetDirectoryName(TextInput.Text) ?? string.Empty;
			var name = Path.GetFileNameWithoutExtension(TextInput.Text);
			TextOutput.Text = Path.Combine(directory, $"{name} (playground){SelectedFormat.FileExtension}");
		}

		private void ButtonBrowseOutput_Click(object sender, EventArgs e)
		{
			var extension = SelectedFormat.FileExtension;
			SFDOutput.DefaultExt = extension.TrimStart('.');
			SFDOutput.Filter = $"{SelectedFormat.DisplayName}|*{extension}";
			SFDOutput.InitialDirectory = ExistingDirectoryOf(TextOutput.Text) ?? string.Empty;
			SFDOutput.FileName = string.IsNullOrWhiteSpace(TextOutput.Text) ? string.Empty : Path.GetFileName(TextOutput.Text.Trim().Trim('"'));

			if (SFDOutput.ShowDialog(this) == DialogResult.OK)
			{
				TextOutput.Text = SFDOutput.FileName;
			}
		}

		/// <summary>
		/// The folder a path points to: the path itself if it is a folder, otherwise the folder holding it.
		/// Null if the text is not a usable path or that folder does not exist.
		/// </summary>
		private static string ExistingDirectoryOf(string path)
		{
			if (string.IsNullOrWhiteSpace(path)) { return null; }

			try
			{
				var fullPath = Path.GetFullPath(path.Trim().Trim('"'));
				if (Directory.Exists(fullPath)) { return fullPath; }

				var directory = Path.GetDirectoryName(fullPath);
				return directory != null && Directory.Exists(directory) ? directory : null;
			}
			catch (Exception ex) when (ex is ArgumentException or NotSupportedException or PathTooLongException)
			{
				return null;
			}
		}

		private void ComboFormat_SelectedIndexChanged(object sender, EventArgs e)
		{
			LabelFormatNotes.Text = SelectedFormat.Notes;

			// Keep the output file's extension matching the container the format needs.
			if (!string.IsNullOrWhiteSpace(TextOutput.Text))
			{
				TextOutput.Text = Path.ChangeExtension(TextOutput.Text, SelectedFormat.FileExtension);
			}
		}

		private void CheckClip_CheckedChanged(object sender, EventArgs e) => SetRangeControlsEnabled();

		private void SetRangeControlsEnabled()
		{
			TextStart.Enabled = TextDuration.Enabled = LabelStart.Enabled = LabelDuration.Enabled = CheckClip.Checked;
		}
		#endregion

		#region Running
		private async void ButtonStart_Click(object sender, EventArgs e)
		{
			if (isRunning)
			{
				cancellation?.Cancel();
				ButtonStart.Enabled = false;
				SetStatus("Cancelling...");
				return;
			}

			if (!TryBuildOptions(out var options, out var settings)) { return; }

			// This run keeps the effect it started with, even if the playground's settings change while it runs.
			ShowEffect(settings.Description, settings.Note);
			cancellation = new CancellationTokenSource();
			SetRunning(true);
			ProgressMain.Value = 0;
			SetStatus("Starting...");

			// Created here, on the UI thread, so that its reports arrive on the UI thread too.
			var progress = new Progress<VideoProgress>(OnProgress);
			try
			{
				await VideoProcessor.ProcessAsync(options, progress, cancellation.Token);

				ProgressMain.Style = ProgressBarStyle.Continuous;
				ProgressMain.Value = ProgressMain.Maximum;
				var size = new FileInfo(options.OutputPath).Length;
				SetStatus($"Finished. Saved {FormatByteCount(size)} as {Path.GetFileName(options.OutputPath)}");
			}
			catch (OperationCanceledException)
			{
				ProgressMain.Style = ProgressBarStyle.Continuous;
				ProgressMain.Value = 0;
				SetStatus("Cancelled. The partial output file was removed.");
			}
			catch (Exception ex)
			{
				ProgressMain.Style = ProgressBarStyle.Continuous;
				ProgressMain.Value = 0;
				SetStatus("Failed. See the message for details.");
				MessageBox.Show(this, ex.Message, "Process Video", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				cancellation.Dispose();
				cancellation = null;
				SetRunning(false);

				// Catch up with whatever the playground was changed to during the run.
				RefreshEffectDescription();
			}
		}

		private void OnProgress(VideoProgress progress)
		{
			if (progress.Fraction is { } fraction)
			{
				ProgressMain.Style = ProgressBarStyle.Continuous;
				ProgressMain.Value = (int)Math.Clamp(fraction * ProgressMain.Maximum, 0, ProgressMain.Maximum);
			}
			else
			{
				ProgressMain.Style = ProgressBarStyle.Marquee;
			}

			SetStatus(progress.Message);
		}

		private void ButtonClose_Click(object sender, EventArgs e) => Close();

		private bool TryBuildOptions(out VideoProcessingOptions options, out VideoEffectSettings settings)
		{
			options = null;
			settings = getEffectSettings();
			if (settings == null)
			{
				return Invalid("Choose an effect in the Chroma Playground first. \"None\" would only re-encode the video.", null);
			}

			var inputPath = TextInput.Text.Trim().Trim('"');
			var outputPath = TextOutput.Text.Trim().Trim('"');

			if (!File.Exists(inputPath))
			{
				return Invalid("Choose an input video that exists.", TextInput);
			}

			if (string.IsNullOrWhiteSpace(outputPath))
			{
				return Invalid("Choose where to save the output video.", TextOutput);
			}

			if (string.Equals(Path.GetFullPath(inputPath), Path.GetFullPath(outputPath), StringComparison.OrdinalIgnoreCase))
			{
				return Invalid("The output file must be different from the input file.", TextOutput);
			}

			var outputDirectory = Path.GetDirectoryName(Path.GetFullPath(outputPath));
			if (!Directory.Exists(outputDirectory))
			{
				return Invalid($"The folder {outputDirectory} does not exist.", TextOutput);
			}

			TimeSpan? start = null;
			TimeSpan? duration = null;
			if (CheckClip.Checked)
			{
				if (!TryParseTime(TextStart.Text, out var parsedStart) || parsedStart < TimeSpan.Zero)
				{
					return Invalid("The start time isn't a valid time. Try 12.5, 1:30, or 0:01:30.25.", TextStart);
				}

				if (!TryParseTime(TextDuration.Text, out var parsedDuration) || parsedDuration <= TimeSpan.Zero)
				{
					return Invalid("The length must be a valid time greater than zero. Try 1, 0:05, or 0:01:30.", TextDuration);
				}

				start = parsedStart;
				duration = parsedDuration;
			}

			if (File.Exists(outputPath)
				&& MessageBox.Show(this, $"{Path.GetFileName(outputPath)} already exists. Replace it?", "Process Video",
					MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return false;
			}

			options = new VideoProcessingOptions(inputPath, outputPath, SelectedFormat, settings.Effect, settings.MeasureOutputSize, start, duration);
			return true;
		}

		private bool Invalid(string message, Control control)
		{
			MessageBox.Show(this, message, "Process Video", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			control?.Focus();
			return false;
		}

		private void SetRunning(bool running)
		{
			isRunning = running;
			ButtonStart.Text = running ? "Cancel" : "Process Video";
			ButtonStart.Enabled = true;
			ButtonClose.Enabled = !running;
			GroupInput.Enabled = GroupOutput.Enabled = GroupRange.Enabled = !running;
		}

		private void SetStatus(string text) => LabelStatus.Text = text;
		#endregion

		/// <summary>Accepts seconds ("12.5"), minutes and seconds ("1:30"), or hours too ("0:01:30.25").</summary>
		internal static bool TryParseTime(string text, out TimeSpan time)
		{
			time = TimeSpan.Zero;
			var parts = text.Trim().Split(':');
			if (parts.Length is < 1 or > 3) { return false; }

			var seconds = 0d;
			for (int i = 0; i < parts.Length; i++)
			{
				// Each field is read as a plain non-negative number; the fraction is allowed only on the last one.
				var style = i == parts.Length - 1 ? NumberStyles.AllowDecimalPoint : NumberStyles.None;
				if (!double.TryParse(parts[i], style, CultureInfo.InvariantCulture, out var value)) { return false; }

				seconds = (seconds * 60d) + value;
			}

			time = TimeSpan.FromSeconds(seconds);
			return true;
		}

		private static string FormatByteCount(long bytes)
		{
			string[] units = { "bytes", "KB", "MB", "GB", "TB" };
			var value = (double)bytes;
			var unit = 0;
			while (value >= 1024 && unit < units.Length - 1)
			{
				value /= 1024;
				unit++;
			}

			return unit == 0 ? $"{bytes:#,0} bytes" : $"{value:0.#} {units[unit]}";
		}
	}
}
