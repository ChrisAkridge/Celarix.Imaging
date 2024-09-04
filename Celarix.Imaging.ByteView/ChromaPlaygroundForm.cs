using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Celarix.Imaging.Misc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.PixelFormats;
using Image = SixLabors.ImageSharp.Image;

namespace Celarix.Imaging.ByteView
{
	public partial class ChromaPlaygroundForm : Form
	{
		private Image<Rgba32> rgbImage;
		private Image<Rgba32> yCbCrImage;
		private Image<Rgba32> modifiedImage;
		private DisplayedChromaPlaygroundImage displayedImage;
		private string imageFileName;
		private bool shouldRunEventHandlers = true;

		public ChromaPlaygroundForm()
		{
			InitializeComponent();
			SetControlEnabledStates();
		}

		private DisplayedChromaPlaygroundImage DisplayedImage
		{
			get => displayedImage;
			set
			{
				displayedImage = value;
				SetStatusText();
				SetControlEnabledStates();
			}
		}

		#region Implementations
		private void LoadImage(string path)
		{
			imageFileName = System.IO.Path.GetFileName(path);
			rgbImage = Image.Load<Rgba32>(path);
			yCbCrImage = null;
			DisplayedImage = DisplayedChromaPlaygroundImage.RGB;

			shouldRunEventHandlers = false;
			NUDBitPlaneIndex.Value = 7;
			shouldRunEventHandlers = true;

			SetPictureBoxImage(rgbImage, DisplayedChromaPlaygroundImage.RGB);
			SetControlEnabledStates();
		}

		private void DisplaySingleColorChannel(ColorChannel channel)
		{
			modifiedImage?.Dispose();
			modifiedImage = null;
			DisplayedChromaPlaygroundImage newDisplayedImage;

			if (channel is ColorChannel.Red or ColorChannel.Green or ColorChannel.Blue or ColorChannel.Alpha)
			{
				modifiedImage = rgbImage.CloneAs<Rgba32>();
				newDisplayedImage = DisplayedChromaPlaygroundImage.RGB;
			}
			else
			{
				// The YCbCr image, when unmodified, is just the same as the RGB image.
				// But we want to keep it separate, so we can make transforms on the RGB image, too.
				yCbCrImage ??= rgbImage.CloneAs<Rgba32>();
				modifiedImage = yCbCrImage.CloneAs<Rgba32>();
				newDisplayedImage = DisplayedChromaPlaygroundImage.YCbCr;
			}

			ChromaPlaygroundHelpers.MutateImage(modifiedImage,
				p => ChromaPlaygroundHelpers.GetColorChannel(p, channel));
			SetPictureBoxImage(modifiedImage, newDisplayedImage);
		}

		private void DisplayBitPlane(ColorChannel channel, int bitPlaneIndex)
		{
			modifiedImage?.Dispose();
			modifiedImage = rgbImage.CloneAs<Rgba32>();
			ChromaPlaygroundHelpers.MutateImage(modifiedImage,
				p => ChromaPlaygroundHelpers.GetBitPlane(p, channel, bitPlaneIndex));
			SetPictureBoxImage(modifiedImage, DisplayedChromaPlaygroundImage.RGB);
		}

		private void ChromaSubsample(ChromaSubsamplingMode mode)
		{
			modifiedImage?.Dispose();
			modifiedImage = null;

			var baseImageForSubsampling = yCbCrImage.CloneAs<Rgba32>();
			var subsampledImage = ChromaPlaygroundHelpers.ChromaSubsample(baseImageForSubsampling, mode);
			modifiedImage = subsampledImage;
			baseImageForSubsampling.Dispose();

			SetPictureBoxImage(modifiedImage, DisplayedChromaPlaygroundImage.YCbCr);
		}

		private void SimpleReduceBitDepth(int bitsPerPixel)
		{
			modifiedImage?.Dispose();
			modifiedImage = rgbImage.CloneAs<Rgba32>();

			for (int y = 0; y < modifiedImage.Height; y++)
			{
				for (int x = 0; x < modifiedImage.Width; x++)
				{
					var originalPixel = modifiedImage[x, y];
					float red;
					float green;
					float blue;
					
					switch (bitsPerPixel)
					{
						case 16:
							// RGB 5:6:5
							red = (originalPixel.R >> 3) / 31f;
							green = (originalPixel.G >> 2) / 63f;
							blue = (originalPixel.B >> 3) / 31f;

							break;
						case 8:
							// RGB 3:3:2
							red = (originalPixel.R >> 5) / 7f;
							green = (originalPixel.G >> 5) / 7f;
							blue = (originalPixel.B >> 6) / 3f;

							break;
						case 4:
							// RGB 1:2:1
							red = (originalPixel.R >> 7) != 0 ? 1f : 0f;
							green = (originalPixel.G >> 6) / 3f;
							blue = (originalPixel.B >> 7) != 0 ? 1f : 0f;

							break;
						default:
							throw new ArgumentOutOfRangeException(nameof(bitsPerPixel), bitsPerPixel, null);
					}

					modifiedImage[x, y] = new Rgba32((byte)(red * 255f),
						(byte)(green * 255f),
						(byte)(blue * 255f));
				}
			}

			SetPictureBoxImage(modifiedImage, DisplayedChromaPlaygroundImage.RGB);
		}

		private void ReduceBitDepthGrayscale(int bitsPerPixel)
		{
			modifiedImage?.Dispose();
			modifiedImage = rgbImage.CloneAs<Rgba32>();
			
			// Easy hack to get the image in full grayscale
			ChromaPlaygroundHelpers.MutateImage(modifiedImage, p => ChromaPlaygroundHelpers.GetColorChannel(p, ColorChannel.Y));
			
			if (bitsPerPixel is not (8 or 4 or 2 or 1))
			{
				throw new ArgumentOutOfRangeException(nameof(bitsPerPixel), bitsPerPixel, "Grayscale bit depths must be 8, 4, 2, or 1.");
			}

			if (bitsPerPixel == 8)
			{
				SetPictureBoxImage(modifiedImage, DisplayedChromaPlaygroundImage.RGB);
				return;
			}

			for (int y = 0; y < modifiedImage.Height; y++)
			{
				for (int x = 0; x < modifiedImage.Width; x++)
				{
					var originalPixel = modifiedImage[x, y];
					float luma;

					switch (bitsPerPixel)
					{
						case 4:
							luma = (originalPixel.R >> 4) / 15f;
							break;
						case 2:
							luma = (originalPixel.R >> 6) / 3f;
							break;
						case 1:
							luma = (originalPixel.R >> 7) != 0 ? 1f : 0f;
							break;
						default:
							throw new ArgumentOutOfRangeException(nameof(bitsPerPixel), bitsPerPixel, null);
					}
					
					var newPixel = (byte)(luma * 255f);
					modifiedImage[x, y] = new Rgba32(newPixel, newPixel, newPixel, 255);
				}
			}
			
			SetPictureBoxImage(modifiedImage, DisplayedChromaPlaygroundImage.RGB);
		}

		private void ReduceBitDepthByTopColors(int bitsPerPixel)
		{
			modifiedImage?.Dispose();
			modifiedImage = rgbImage.CloneAs<Rgba32>();
			
			ChromaPlaygroundHelpers.MutateReduceBitDepthTopColors(modifiedImage, bitsPerPixel);
			
			SetPictureBoxImage(modifiedImage, DisplayedChromaPlaygroundImage.RGB);
		}
		#endregion

		private void SetControlEnabledStates()
		{
			var hasRGBImage = rgbImage != null;
			var hasYCbCrImage = yCbCrImage != null;
			var rgbDisplayed = DisplayedImage == DisplayedChromaPlaygroundImage.RGB && hasRGBImage;
			var yCbCrDisplayed = DisplayedImage == DisplayedChromaPlaygroundImage.YCbCr && hasYCbCrImage;

			// The Color Channels controls are enabled if we have an RGB image.
			RadioRedChannel.Enabled = hasRGBImage;
			RadioGreenChannel.Enabled = hasRGBImage;
			RadioBlueChannel.Enabled = hasRGBImage;
			RadioAlphaChannel.Enabled = hasRGBImage;
			RadioLuminance.Enabled = hasRGBImage;
			RadioCb.Enabled = hasRGBImage;
			RadioCr.Enabled = hasRGBImage;
			RadioChrominance.Enabled = hasRGBImage;
			RadioHueChannel.Enabled = hasRGBImage;
			RadioSaturationChannel.Enabled = hasRGBImage;
			RadioValueChannel.Enabled = hasRGBImage;
			ButtonColorChannelReset.Enabled = hasRGBImage;
			ButtonSetColorChannel.Enabled = hasRGBImage;

			// The Bit Planes controls are enabled if we have an RGB image and that image is currently being displayed.
			RadioBitPlaneRed.Enabled = rgbDisplayed;
			RadioBitPlaneGreen.Enabled = rgbDisplayed;
			RadioBitPlaneBlue.Enabled = rgbDisplayed;
			RadioBitPlaneAlpha.Enabled = rgbDisplayed;
			NUDBitPlaneIndex.Enabled = rgbDisplayed;

			// The Chroma Subsampling controls are enabled if we have a YCbCr image and that image is currently being displayed.
			Radio444.Enabled = yCbCrDisplayed;
			Radio422.Enabled = yCbCrDisplayed;
			Radio420.Enabled = yCbCrDisplayed;
			Radio411.Enabled = yCbCrDisplayed;
			ButtonChromaSubsamplingSet.Enabled = yCbCrDisplayed;

			// The Reduce Bit Depth controls are enabled if we have an RGB image and that image is currently being displayed.
			Radio16BPPRGB565.Enabled = rgbDisplayed;
			Radio16bppTop65536.Enabled = rgbDisplayed;
			Radio8bppRGB332.Enabled = rgbDisplayed;
			Radio8BPPTop256.Enabled = rgbDisplayed;
			Radio8BPPGrayscale.Enabled = rgbDisplayed;
			Radio4bppRGB121.Enabled = rgbDisplayed;
			Radio4BPPTop16.Enabled = rgbDisplayed;
			Radio4BPPGrayscale.Enabled = rgbDisplayed;
			Radio2BPPTop4.Enabled = rgbDisplayed;
			Radio2BPPGrayscale.Enabled = rgbDisplayed;
			Radio1BPPTop2.Enabled = rgbDisplayed;
			Radio1BPPGrayscale.Enabled = rgbDisplayed;

			// The Analog Signal controls are enabled if we have a YCbCr image and that image is currently being displayed.
			ButtonSaveSimplerAudio.Enabled = yCbCrDisplayed;
			ButtonSaveNTSCAudio.Enabled = yCbCrDisplayed;
		}

		private void SetStatusText()
		{
			var displayedImageName = DisplayedImage switch
			{
				DisplayedChromaPlaygroundImage.RGB => "RGB",
				DisplayedChromaPlaygroundImage.YCbCr => "YCbCr",
				_ => "Unknown"
			};

			var otherImageAvailable = DisplayedImage switch
			{
				DisplayedChromaPlaygroundImage.RGB => yCbCrImage != null ? " (YCbCr available)" : "",
				DisplayedChromaPlaygroundImage.YCbCr => rgbImage != null ? " (RGB available)" : "",
				_ => ""
			};

			TSSMain.Text = $"{imageFileName} | {rgbImage.Width}x{rgbImage.Height} | Viewing {displayedImageName} image{otherImageAvailable}";
		}

		private void SetPictureBoxImage(Image<Rgba32> imageSharpImage, DisplayedChromaPlaygroundImage displayedImage)
		{
			if (imageSharpImage == null)
			{
				PictureMain.Image = null;
				return;
			}

			var bitmap = imageSharpImage.ToSystemDrawingImage();
			PictureMain.Image = bitmap;
			DisplayedImage = displayedImage;
		}

		private void TSBOpenImage_Click(object sender, EventArgs e)
		{
			if (OFDOpenImage.ShowDialog() == DialogResult.OK)
			{
				LoadImage(OFDOpenImage.FileName);
			}
		}

		private void ButtonSetColorChannel_Click(object sender, EventArgs e)
		{
			if (RadioRedChannel.Checked)
			{
				DisplaySingleColorChannel(ColorChannel.Red);
			}
			else if (RadioGreenChannel.Checked)
			{
				DisplaySingleColorChannel(ColorChannel.Green);
			}
			else if (RadioBlueChannel.Checked)
			{
				DisplaySingleColorChannel(ColorChannel.Blue);
			}
			else if (RadioAlphaChannel.Checked)
			{
				DisplaySingleColorChannel(ColorChannel.Alpha);
			}
			else if (RadioLuminance.Checked)
			{
				DisplaySingleColorChannel(ColorChannel.Y);
			}
			else if (RadioCb.Checked)
			{
				DisplaySingleColorChannel(ColorChannel.Cb);
			}
			else if (RadioCr.Checked)
			{
				DisplaySingleColorChannel(ColorChannel.Cr);
			}
			else if (RadioChrominance.Checked)
			{
				DisplaySingleColorChannel(ColorChannel.CbPlusCr);
			}
			else if (RadioHueChannel.Checked)
			{
				DisplaySingleColorChannel(ColorChannel.Hue);
			}
			else if (RadioSaturationChannel.Checked)
			{
				DisplaySingleColorChannel(ColorChannel.Saturation);
			}
			else if (RadioValueChannel.Checked)
			{
				DisplaySingleColorChannel(ColorChannel.Value);
			}
		}

		private void ButtonColorChannelReset_Click(object sender, EventArgs e)
		{
			modifiedImage?.Dispose();
			modifiedImage = null;
			SetPictureBoxImage(rgbImage, DisplayedChromaPlaygroundImage.RGB);
		}

		private void NUDBitPlaneIndex_ValueChanged(object sender, EventArgs e)
		{
			if (!shouldRunEventHandlers) { return; }

			if (RadioBitPlaneRed.Checked)
			{
				DisplayBitPlane(ColorChannel.Red, (int)NUDBitPlaneIndex.Value);
			}
			else if (RadioBitPlaneGreen.Checked)
			{
				DisplayBitPlane(ColorChannel.Green, (int)NUDBitPlaneIndex.Value);
			}
			else if (RadioBitPlaneBlue.Checked)
			{
				DisplayBitPlane(ColorChannel.Blue, (int)NUDBitPlaneIndex.Value);
			}
			else if (RadioBitPlaneAlpha.Checked)
			{
				DisplayBitPlane(ColorChannel.Alpha, (int)NUDBitPlaneIndex.Value);
			}
		}

		private void RadioBitPlaneRed_CheckedChanged(object sender, EventArgs e)
		{
			if (RadioBitPlaneRed.Checked)
			{
				DisplayBitPlane(ColorChannel.Red, (int)NUDBitPlaneIndex.Value);
			}
		}

		private void RadioBitPlaneGreen_CheckedChanged(object sender, EventArgs e)
		{
			if (RadioBitPlaneGreen.Checked)
			{
				DisplayBitPlane(ColorChannel.Green, (int)NUDBitPlaneIndex.Value);
			}
		}

		private void RadioBitPlaneBlue_CheckedChanged(object sender, EventArgs e)
		{
			if (RadioBitPlaneBlue.Checked)
			{
				DisplayBitPlane(ColorChannel.Blue, (int)NUDBitPlaneIndex.Value);
			}
		}

		private void RadioBitPlaneAlpha_CheckedChanged(object sender, EventArgs e)
		{
			if (RadioBitPlaneAlpha.Checked)
			{
				DisplayBitPlane(ColorChannel.Alpha, (int)NUDBitPlaneIndex.Value);
			}
		}

		private void TSBConvertToRGB_Click(object sender, EventArgs e)
		{
			modifiedImage?.Dispose();
			modifiedImage = null;
			SetPictureBoxImage(rgbImage, DisplayedChromaPlaygroundImage.RGB);
		}

		private void TSBConvertToYCbCr_Click(object sender, EventArgs e)
		{
			modifiedImage?.Dispose();
			modifiedImage = null;
			yCbCrImage ??= rgbImage.CloneAs<Rgba32>();
			SetPictureBoxImage(yCbCrImage, DisplayedChromaPlaygroundImage.YCbCr);
		}

		private void TSBUseCurrent_Click(object sender, EventArgs e)
		{
			rgbImage?.Dispose();
			yCbCrImage?.Dispose();
			rgbImage = modifiedImage.CloneAs<Rgba32>();
			yCbCrImage = null;
			SetPictureBoxImage(rgbImage, DisplayedChromaPlaygroundImage.RGB);
		}

		private void ButtonChromaSubsamplingSet_Click(object sender, EventArgs e)
		{
			if (Radio444.Checked)
			{
				ChromaSubsample(ChromaSubsamplingMode.YCbCr444);
			}
			else if (Radio422.Checked)
			{
				ChromaSubsample(ChromaSubsamplingMode.YCbCr422);
			}
			else if (Radio420.Checked)
			{
				ChromaSubsample(ChromaSubsamplingMode.YCbCr420);
			}
			else if (Radio411.Checked)
			{
				ChromaSubsample(ChromaSubsamplingMode.YCbCr411);
			}
		}

		private void ButtonReduceBitDepth_Click(object sender, EventArgs e)
		{
			if (Radio16BPPRGB565.Checked) { SimpleReduceBitDepth(16); }
			else if (Radio8bppRGB332.Checked) { SimpleReduceBitDepth(8); }
			else if (Radio4bppRGB121.Checked) { SimpleReduceBitDepth(4); }
			else if (Radio8BPPGrayscale.Checked) { ReduceBitDepthGrayscale(8); }
			else if (Radio4BPPGrayscale.Checked) { ReduceBitDepthGrayscale(4); }
			else if (Radio2BPPGrayscale.Checked) { ReduceBitDepthGrayscale(2); }
			else if (Radio1BPPGrayscale.Checked) { ReduceBitDepthGrayscale(1); }
			else if (Radio16bppTop65536.Checked) { ReduceBitDepthByTopColors(16); }
			else if (Radio8BPPTop256.Checked) { ReduceBitDepthByTopColors(8); }
			else if (Radio4BPPTop16.Checked) { ReduceBitDepthByTopColors(4); }
			else if (Radio2BPPTop4.Checked) { ReduceBitDepthByTopColors(2); }
			else if (Radio1BPPTop2.Checked) { ReduceBitDepthByTopColors(1); }
		}
	}
}
