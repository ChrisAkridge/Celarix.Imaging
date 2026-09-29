using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Celarix.Imaging.Misc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Threading.Tasks;
using Image = SixLabors.ImageSharp.Image;
using Path = System.IO.Path;

namespace Celarix.Imaging.ByteView
{
	/// <summary>
	/// Every effect is a function from the source image to a new image; there is no RGB/YCbCr "mode".
	/// (Effects that work in YCbCr convert internally.) The result is previewed live, and "Keep Result"
	/// promotes it to be the new source so that effects can be stacked.
	/// </summary>
	public partial class ChromaPlaygroundForm : Form
	{
		private enum Effect
		{
			None,
			ColorChannel,
			BitPlane,
			ChromaSubsampling,
			LumaSubsampling,
			ReduceBitDepth,
			UniqueColorMap
		}

		private enum BitDepthMethod
		{
			Shift,
			Grayscale,
			OptimizedPalette,
			TopColors
		}

		private sealed record Choice<T>(string Label, T Value)
		{
			public override string ToString() => Label;
		}

		private static readonly Choice<Effect>[] EffectChoices =
		{
			new("None (source image)", Effect.None),
			new("Color channel", Effect.ColorChannel),
			new("Bit plane", Effect.BitPlane),
			new("Chroma subsampling", Effect.ChromaSubsampling),
			new("Luma subsampling (silly)", Effect.LumaSubsampling),
			new("Reduce bit depth", Effect.ReduceBitDepth),
			new("Unique colors map", Effect.UniqueColorMap)
		};

		// The value is whether the layout is the compact one (true) or the fixed 4096x4096 one (false).
		private static readonly Choice<bool>[] ColorMapLayoutChoices =
		{
			new("Compact (only the colors present)", true),
			new("Fixed 4096x4096 (comparable between images)", false)
		};

		private static readonly Choice<ColorChannel>[] ColorChannelChoices =
		{
			new("RGB - Red", ColorChannel.Red),
			new("RGB - Green", ColorChannel.Green),
			new("RGB - Blue", ColorChannel.Blue),
			new("Alpha", ColorChannel.Alpha),
			new("YCbCr - Y (luminance)", ColorChannel.Y),
			new("YCbCr - Cb (blue - luminance)", ColorChannel.Cb),
			new("YCbCr - Cr (red - luminance)", ColorChannel.Cr),
			new("YCbCr - Cb and Cr (chrominance)", ColorChannel.CbPlusCr),
			new("HSV - Hue", ColorChannel.Hue),
			new("HSV - Saturation", ColorChannel.Saturation),
			new("HSV - Value", ColorChannel.Value),
			new("Oklab - L (lightness)", ColorChannel.OklabL),
			new("Oklab - a (green - red)", ColorChannel.OklabA),
			new("Oklab - b (blue - yellow)", ColorChannel.OklabB),
			new("Oklab - Chroma (colorfulness)", ColorChannel.OklabChroma),
			new("Oklab - Hue", ColorChannel.OklabHue),
			new("YIQ - I (orange - cyan)", ColorChannel.YiqI),
			new("YIQ - Q (purple - green)", ColorChannel.YiqQ),
			new("Linear light - Red", ColorChannel.LinearRed),
			new("Linear light - Green", ColorChannel.LinearGreen),
			new("Linear light - Blue", ColorChannel.LinearBlue),
			new("CMYK - Cyan", ColorChannel.Cyan),
			new("CMYK - Magenta", ColorChannel.Magenta),
			new("CMYK - Yellow", ColorChannel.Yellow),
			new("CMYK - Black (key)", ColorChannel.Black)
		};

		// A null channel means "all of R, G, and B at once".
		private static readonly Choice<ColorChannel?>[] BitPlaneChannelChoices =
		{
			new("All of red, green, and blue", null),
			new("Red", ColorChannel.Red),
			new("Green", ColorChannel.Green),
			new("Blue", ColorChannel.Blue),
			new("Alpha", ColorChannel.Alpha)
		};

		private static readonly Choice<ChromaSubsamplingMode>[] SubsamplingChoices =
		{
			new("4:4:4 (full resolution)", ChromaSubsamplingMode.YCbCr444),
			new("4:2:2 (2x1 blocks)", ChromaSubsamplingMode.YCbCr422),
			new("4:2:0 (2x2 blocks)", ChromaSubsamplingMode.YCbCr420),
			new("4:1:1 (4x1 blocks)", ChromaSubsamplingMode.YCbCr411),
			new("8:1:1 (non-standard, 8x8 blocks)", ChromaSubsamplingMode.YCbCr811),
			new("16:1:1 (non-standard, 16x16 blocks)", ChromaSubsamplingMode.YCbCr1611),
			new("256:1:1 (VERY non-standard, 256x256 blocks)", ChromaSubsamplingMode.YCbCr25611)
		};

		private static readonly Choice<BitDepthMethod>[] BitDepthMethodChoices =
		{
			new("Discard low bits (fixed RGB layout)", BitDepthMethod.Shift),
			new("Grayscale", BitDepthMethod.Grayscale),
			new("Optimized palette (k-means in Oklab)", BitDepthMethod.OptimizedPalette),
			new("Most common colors (top N)", BitDepthMethod.TopColors)
		};

		private static readonly Dictionary<BitDepthMethod, int[]> BitDepthsByMethod = new()
		{
			[BitDepthMethod.Shift] = new[] { 16, 8, 4 },
			[BitDepthMethod.Grayscale] = new[] { 8, 4, 2, 1 },
			[BitDepthMethod.OptimizedPalette] = new[] { 8, 4, 2, 1 },
			[BitDepthMethod.TopColors] = new[] { 16, 8, 4, 2, 1 }
		};

		private static readonly Dictionary<Effect, string> EffectDescriptions = new()
		{
			[Effect.None] = "Shows the source image unchanged. Pick an effect above to preview it.",
			[Effect.ColorChannel] = "Shows a single channel of the image, from RGB, YCbCr, or HSV.",
			[Effect.BitPlane] = "Shows one bit of each pixel's value as black or full-intensity. Bit 7 is the most significant.",
			[Effect.ChromaSubsampling] = "Keeps luminance at full resolution but stores color in larger blocks, as video codecs do.",
			[Effect.LumaSubsampling] = "The reverse of chroma subsampling: color stays at full resolution while brightness is stored in blocks. Nothing does this on purpose.",
			[Effect.ReduceBitDepth] = "Reduces the number of bits used to store each pixel. Dithering trades banding for noise-like patterns.",
			[Effect.UniqueColorMap] = "Draws each distinct color in the image as one pixel, with similar colors placed near each other (along a Hilbert curve through Oklab). Compact packs only the colors present; Fixed gives every possible color the same spot in a 4096x4096 map, most of it empty, so different images can be compared."
		};

		private readonly MainForm byteViewForm;

		private Image<Rgba32> loadedImage;
		private Image<Rgba32> sourceImage;
		private Image<Rgba32> resultImage;
		private string imageFileName;
		private int previewVersion;
		private bool isUpdatingControls;

		public ChromaPlaygroundForm(MainForm byteViewForm)
		{
			this.byteViewForm = byteViewForm;
			InitializeComponent();

			ComboEffect.Items.AddRange(EffectChoices);
			ComboEffect.SelectedIndex = 0;
			ApplyFitToWindow();
			OnEffectChanged();
		}

		private Effect SelectedEffect => ((Choice<Effect>)ComboEffect.SelectedItem).Value;

		// What's currently on screen, and what saving/exporting acts on.
		private Image<Rgba32> DisplayedImage => TSBShowOriginal.Checked || resultImage == null ? sourceImage : resultImage;

		#region Loading
		private void LoadImage(string path)
		{
			try
			{
				SetNewImage(Image.Load<Rgba32>(path), Path.GetFileName(path));
			}
			catch (Exception ex) when (ex is ImageFormatException or UnknownImageFormatException or InvalidImageContentException)
			{
				MessageBox.Show($"The file at {path} is not a valid image file.", "Invalid Image File",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void SetNewImage(Image<Rgba32> image, string fileName)
		{
			imageFileName = fileName;
			loadedImage = image;
			sourceImage = image;
			TrackBitIndex.Value = TrackBitIndex.Maximum;
			RefreshPreview();
		}
		#endregion

		#region Effects
		/// <summary>
		/// Builds a function that applies the currently-configured effect, or null if there is nothing to apply.
		/// Parameters are read now, on the UI thread, so the function is safe to run in the background.
		/// </summary>
		private Func<Image<Rgba32>, Image<Rgba32>> BuildEffect()
		{
			switch (SelectedEffect)
			{
				case Effect.ColorChannel:
				{
					var channel = ((Choice<ColorChannel>)ComboParameter1.SelectedItem).Value;
					return source => Mutated(source, p => ChromaPlaygroundHelpers.GetColorChannel(p, channel));
				}
				case Effect.BitPlane:
				{
					var channel = ((Choice<ColorChannel?>)ComboParameter1.SelectedItem).Value;
					var bit = TrackBitIndex.Value;
					return channel is { } single
						? source => Mutated(source, p => ChromaPlaygroundHelpers.GetBitPlane(p, single, bit))
						: source => Mutated(source, p => ChromaPlaygroundHelpers.GetRGBBitPlane(p, bit));
				}
				case Effect.ChromaSubsampling:
				{
					var mode = ((Choice<ChromaSubsamplingMode>)ComboParameter1.SelectedItem).Value;
					return source => ChromaPlaygroundHelpers.ChromaSubsample(source, mode);
				}
				case Effect.LumaSubsampling:
				{
					var mode = ((Choice<ChromaSubsamplingMode>)ComboParameter1.SelectedItem).Value;
					return source => ChromaPlaygroundHelpers.LumaSubsample(source, mode);
				}
				case Effect.ReduceBitDepth:
				{
					var method = ((Choice<BitDepthMethod>)ComboParameter1.SelectedItem).Value;
					var bits = (int)ComboParameter2.SelectedItem;
					var dither = CheckDither.Checked;
					return method switch
					{
						BitDepthMethod.Shift => source => Cloned(source, i => ChromaPlaygroundHelpers.MutateReduceBitDepthByShifting(i, bits, dither)),
						BitDepthMethod.Grayscale => source => Cloned(source, i => ChromaPlaygroundHelpers.MutateReduceBitDepthGrayscale(i, bits, dither)),
						BitDepthMethod.OptimizedPalette => source => Cloned(source, i => ChromaPlaygroundHelpers.MutateReduceBitDepthKMeans(i, bits, dither)),
						_ => source => Cloned(source, i => ChromaPlaygroundHelpers.MutateReduceBitDepthTopColors(i, bits, dither))
					};
				}
				case Effect.UniqueColorMap:
				{
					var compact = ((Choice<bool>)ComboParameter1.SelectedItem).Value;
					return compact ? UniqueColorMap.RenderCompact : UniqueColorMap.RenderFixed;
				}
				default:
					return null;
			}
		}

		private static Image<Rgba32> Cloned(Image<Rgba32> source, Action<Image<Rgba32>> mutate)
		{
			var clone = source.Clone();
			mutate(clone);
			return clone;
		}

		private static Image<Rgba32> Mutated(Image<Rgba32> source, Func<Rgba32, Rgba32> transform) =>
			Cloned(source, i => ChromaPlaygroundHelpers.MutateImage(i, transform));

		private async void RefreshPreview()
		{
			var version = ++previewVersion;
			var effect = sourceImage == null ? null : BuildEffect();
			var source = sourceImage;

			Image<Rgba32> newResult = null;
			if (effect != null)
			{
				UseWaitCursor = true;
				try
				{
					newResult = await Task.Run(() => effect(source));
				}
				catch (Exception ex)
				{
					if (version == previewVersion)
					{
						UseWaitCursor = false;
						MessageBox.Show(ex.Message, "Effect Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
					return;
				}
			}

			if (version != previewVersion)
			{
				// A newer request superseded this one while it was running.
				newResult?.Dispose();
				return;
			}

			UseWaitCursor = false;
			resultImage?.Dispose();
			resultImage = newResult;
			UpdateDisplay();
		}
		#endregion

		#region Display
		private void UpdateDisplay()
		{
			var image = DisplayedImage;
			var hasImage = image != null;

			var oldBitmap = PictureMain.Image;
			PictureMain.Image = hasImage ? image.ToSystemDrawingImage() : null;
			oldBitmap?.Dispose();

			TSBSaveAs.Enabled = hasImage;
			TSDDExportSignal.Enabled = hasImage;
			TSBKeepResult.Enabled = resultImage != null;
			TSBShowOriginal.Enabled = resultImage != null;
			TSBResetEffect.Enabled = hasImage && !ReferenceEquals(sourceImage, loadedImage);
			ComboEffect.Enabled = hasImage;

			if (!hasImage)
			{
				TSSMain.Text = "Open an image to begin.";
				LabelSignalStats.Text = string.Empty;
				return;
			}

			var shown = image == sourceImage
				? (ReferenceEquals(sourceImage, loadedImage) ? "source image" : "source image (with kept results)")
				: $"preview: {ComboEffect.Text}";
			TSSMain.Text = $"{imageFileName} | {image.Width}x{image.Height} | Showing {shown}";
			LabelSignalStats.Text = $"{MockNTSCSignalGenerator.GetImageStatsString(image)}\r\n{SimpleSignalGenerator.GetImageStatsString(image)}".Trim();
		}

		private void ApplyFitToWindow()
		{
			if (TSBFitToWindow.Checked)
			{
				PictureMain.Dock = DockStyle.Fill;
				PictureMain.SizeMode = PictureBoxSizeMode.Zoom;
			}
			else
			{
				PictureMain.Dock = DockStyle.None;
				PictureMain.Location = System.Drawing.Point.Empty;
				PictureMain.SizeMode = PictureBoxSizeMode.AutoSize;
			}
		}

		/// <summary>
		/// Shows only the parameter controls the selected effect uses, and fills them with its choices.
		/// </summary>
		private void OnEffectChanged()
		{
			isUpdatingControls = true;
			try
			{
				var effect = SelectedEffect;
				LabelEffectDescription.Text = EffectDescriptions[effect];

				var hasParameter1 = effect != Effect.None;
				var hasParameter2 = effect == Effect.ReduceBitDepth;
				var hasBitIndex = effect == Effect.BitPlane;

				LabelParameter1.Visible = ComboParameter1.Visible = hasParameter1;
				LabelParameter2.Visible = ComboParameter2.Visible = hasParameter2;
				LabelBitIndex.Visible = TrackBitIndex.Visible = hasBitIndex;
				CheckDither.Visible = hasParameter2;

				ComboParameter1.Items.Clear();
				switch (effect)
				{
					case Effect.ColorChannel:
						LabelParameter1.Text = "Channel:";
						ComboParameter1.Items.AddRange(ColorChannelChoices);
						break;
					case Effect.BitPlane:
						LabelParameter1.Text = "Channel:";
						ComboParameter1.Items.AddRange(BitPlaneChannelChoices);
						break;
					case Effect.ChromaSubsampling:
					case Effect.LumaSubsampling:
						LabelParameter1.Text = "Subsampling:";
						ComboParameter1.Items.AddRange(SubsamplingChoices);
						break;
					case Effect.UniqueColorMap:
						LabelParameter1.Text = "Layout:";
						ComboParameter1.Items.AddRange(ColorMapLayoutChoices);
						break;
					case Effect.ReduceBitDepth:
						LabelParameter1.Text = "Method:";
						LabelParameter2.Text = "Bits per pixel:";
						ComboParameter1.Items.AddRange(BitDepthMethodChoices);
						break;
				}

				if (ComboParameter1.Items.Count > 0)
				{
					ComboParameter1.SelectedIndex = 0;
				}

				PopulateBitDepths();
				UpdateBitIndexLabel();
			}
			finally
			{
				isUpdatingControls = false;
			}
		}

		private void PopulateBitDepths()
		{
			ComboParameter2.Items.Clear();
			if (SelectedEffect != Effect.ReduceBitDepth) { return; }

			var method = ((Choice<BitDepthMethod>)ComboParameter1.SelectedItem).Value;
			foreach (var bits in BitDepthsByMethod[method])
			{
				ComboParameter2.Items.Add(bits);
			}

			ComboParameter2.SelectedIndex = 0;
		}

		private void UpdateBitIndexLabel()
		{
			LabelBitIndex.Text = $"Bit index: {TrackBitIndex.Value}" +
				(TrackBitIndex.Value == 7 ? " (most significant)" : TrackBitIndex.Value == 0 ? " (least significant)" : "");
		}
		#endregion

		#region Event handlers
		private void ComboEffect_SelectedIndexChanged(object sender, EventArgs e)
		{
			OnEffectChanged();
			RefreshPreview();
		}

		private void ComboParameter1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (isUpdatingControls) { return; }

			// The methods offer different bit depths, so the depth list follows the method.
			isUpdatingControls = true;
			PopulateBitDepths();
			isUpdatingControls = false;

			RefreshPreview();
		}

		private void ComboParameter2_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!isUpdatingControls) { RefreshPreview(); }
		}

		private void CheckDither_CheckedChanged(object sender, EventArgs e)
		{
			if (!isUpdatingControls) { RefreshPreview(); }
		}

		private void TrackBitIndex_ValueChanged(object sender, EventArgs e)
		{
			UpdateBitIndexLabel();
			if (!isUpdatingControls) { RefreshPreview(); }
		}

		private void TSBOpenImage_Click(object sender, EventArgs e)
		{
			if (OFDOpenImage.ShowDialog() == DialogResult.OK)
			{
				LoadImage(OFDOpenImage.FileName);
			}
		}

		private void TSBLoadFromByteView_Click(object sender, EventArgs e)
		{
			var image = byteViewForm.GetImage();
			if (image != null)
			{
				SetNewImage(image, "From ByteView");
			}
		}

		private void TSBSaveAs_Click(object sender, EventArgs e)
		{
			if (DisplayedImage != null && SFDSaveImage.ShowDialog() == DialogResult.OK)
			{
				DisplayedImage.SaveAsPng(SFDSaveImage.FileName);
			}
		}

		private void TSBKeepResult_Click(object sender, EventArgs e)
		{
			if (resultImage == null) { return; }

			// The old source isn't disposed: a background effect may still be reading it, and it may be the loaded image.
			sourceImage = resultImage;
			resultImage = null;
			TSBShowOriginal.Checked = false;

			// Restart the effect from the new source; for the channel/bit-plane/etc. controls this is just a fresh preview.
			ComboEffect.SelectedIndex = 0;
			RefreshPreview();
		}

		private void TSBRevert_Click(object sender, EventArgs e)
		{
			sourceImage = loadedImage;
			RefreshPreview();
		}

		private void TSBShowOriginal_CheckedChanged(object sender, EventArgs e) => UpdateDisplay();

		private void TSBFitToWindow_CheckedChanged(object sender, EventArgs e) => ApplyFitToWindow();

		private void TSMISaveNTSCAudio_Click(object sender, EventArgs e) =>
			ExportSignal(MockNTSCSignalGenerator.GenerateMockNTSCSignal);

		private void TSMISaveSimplerAudio_Click(object sender, EventArgs e) =>
			ExportSignal(SimpleSignalGenerator.GenerateSimpleSignal);

		private async void ExportSignal(Action<Image<Rgba32>, string> generate)
		{
			if (DisplayedImage == null || SFDSaveAnalogSignal.ShowDialog() != DialogResult.OK) { return; }

			// Work on a copy: the preview may be replaced (and disposed) while the export runs.
			using var image = DisplayedImage.Clone();
			var fileName = SFDSaveAnalogSignal.FileName;
			UseWaitCursor = true;
			try
			{
				await Task.Run(() => generate(image, fileName));
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Export Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				UseWaitCursor = false;
			}
		}
		#endregion
	}
}
