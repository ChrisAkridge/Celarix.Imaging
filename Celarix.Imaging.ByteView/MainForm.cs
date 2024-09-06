//	MainForm.cs
//
//	The main form for the application; displays files as bitmapped images.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Celarix.Imaging.BinaryDrawing;
using Celarix.Imaging.Progress;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Image = System.Drawing.Image;
using Size = System.Drawing.Size;

namespace Celarix.Imaging.ByteView
{
	public partial class MainForm : Form
	{
		// The currently selected bit depth.
		private int bitDepth;

		// The currently selected color mode.
		private ColorMode colorMode;

		// The current set of file paths to generate an image from.
		private string[] filePaths;

		// The current image displayed on the form.
		private Image image;

		// The size in bytes of all the files currently displayed in the image.
		private long imageSizeBytes;

		private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();
		private readonly Progress<DrawingProgress> progress = new Progress<DrawingProgress>();

		/// <summary>
		/// Initializes a new instance of the <see cref="MainForm"/> class. 
		/// </summary>
		public MainForm()
		{
			InitializeComponent();

			progress.ProgressChanged += Progress_ProgressChanged;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MainForm"/> class.
		/// </summary>
		/// <param name="filePath">A path to a file to display in the window.</param>
		public MainForm(string filePath)
		{
			InitializeComponent();

			if (!File.Exists(filePath))
			{
				MessageBox.Show($"The file at {filePath} does not exist.", "File Not Found",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			filePaths = new[] { filePath };
			progress.ProgressChanged += Progress_ProgressChanged;

			// TODO: have caller draw image somehow
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			ComboBitDepths.SelectedIndex = 5;
		}

		private void ComboBitDepths_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch (ComboBitDepths.SelectedIndex)
			{
				case 0:
					bitDepth = 1;
					RadioGrayscale.Enabled = true;
					RadioRGB.Enabled = false;
					RadioARGB.Enabled = false;
					RadioPaletted.Enabled = true;
					RadioGrayscale.Checked = true;
					break;
				case 1:
					bitDepth = 2;
					RadioGrayscale.Enabled = true;
					RadioRGB.Enabled = false;
					RadioARGB.Enabled = false;
					RadioPaletted.Enabled = true;
					RadioGrayscale.Checked = true;
					break;
				case 2:
					bitDepth = 4;
					RadioGrayscale.Enabled = true;
					RadioRGB.Enabled = true;
					RadioARGB.Enabled = false;
					RadioPaletted.Enabled = true;
					RadioGrayscale.Checked = true;
					break;
				case 3:
					bitDepth = 8;
					RadioGrayscale.Enabled = true;
					RadioRGB.Enabled = true;
					RadioARGB.Enabled = true;
					RadioPaletted.Enabled = true;
					RadioGrayscale.Checked = true;
					break;
				case 4:
					bitDepth = 16;
					RadioGrayscale.Enabled = false;
					RadioRGB.Enabled = true;
					RadioARGB.Enabled = true;
					RadioPaletted.Enabled = false;
					RadioRGB.Checked = true;
					break;
				case 5:
					bitDepth = 24;
					RadioGrayscale.Enabled = false;
					RadioRGB.Enabled = true;
					RadioARGB.Enabled = true;
					RadioPaletted.Enabled = false;
					RadioRGB.Checked = true;
					break;
				case 6:
					bitDepth = 32;
					RadioGrayscale.Enabled = false;
					RadioRGB.Enabled = false;
					RadioARGB.Enabled = true;
					RadioPaletted.Enabled = false;
					RadioARGB.Checked = true;
					break;
				default:
					break;
			}
		}

		private void Image_MouseMove(object sender, MouseEventArgs e)
		{
			if (image == null || (e.X > image.Width || e.Y > image.Height)) { return; }

			ImageInfoUtilities.GetAddressFromImageCoordinate(image.Width, image.Height, e.X, e.Y,
				bitDepth, out long address, out int bitIndex);
			string labelAddressText = ImageInfoUtilities.FormatAddress(address, bitIndex);

			LabelAddress.Text = labelAddressText;
		}

		private void Panel_MouseEnter(object sender, EventArgs e) => Panel.Focus();

		private void RadioARGB_CheckedChanged(object sender, EventArgs e)
		{
			if (RadioARGB.Checked)
			{
				colorMode = ColorMode.Argb;
			}
		}

		private void RadioGrayscale_CheckedChanged(object sender, EventArgs e)
		{
			if (RadioGrayscale.Checked)
			{
				colorMode = ColorMode.Grayscale;
			}
		}

		private void RadioPaletted_CheckedChanged(object sender, EventArgs e)
		{
			if (RadioPaletted.Checked)
			{
				colorMode = ColorMode.Paletted;
			}
		}

		private void RadioRGB_CheckedChanged(object sender, EventArgs e)
		{
			if (RadioRGB.Checked)
			{
				colorMode = ColorMode.Rgb;
			}
		}

		private void TSBCancel_Click(object sender, EventArgs e) => tokenSource.Cancel();

		private void TSBLargeFileProcessor_Click(object sender, EventArgs e)
		{
			using var lfpForm = new LargeFileProcessorForm();
			lfpForm.ShowDialog();
		}

		private async void TSBOpenFiles_Click(object sender, EventArgs e)
		{
			if (OpenFile.ShowDialog() != DialogResult.OK) { return; }

			filePaths = OpenFile.FileNames;
			filePaths = filePaths.OrderBy(s => s).ToArray();
			await GenerateImage();
		}

		private async void TSBOpenFolder_Click(object sender, EventArgs e)
		{
			if (FolderSelector.ShowDialog() != DialogResult.OK) { return; }

			filePaths = Directory.EnumerateFiles(FolderSelector.SelectedPath, "*.*",
				SearchOption.AllDirectories).ToArray();
			filePaths = filePaths.OrderBy(s => s).ToArray();
			await GenerateImage();
		}

		private void TSBOpenPicture_Click(object sender, EventArgs e)
		{
			if (OpenPicture.ShowDialog() != DialogResult.OK) { return; }

			string filePath = OpenPicture.FileName;
			imageSizeBytes = new FileInfo(filePath).Length;
			image = (Bitmap)Image.FromFile(filePath);
			bitDepth = 32;
			SetPictureBoxImage();

			SetTitleBar();
		}

		private async void TSBOpenRaw_Click(object sender, EventArgs e)
		{
			if (OpenFile.ShowDialog() != DialogResult.OK) { return; }

			int width = 0, height = 0;
			using (var sizeForm = new RawImageSizeForm())
			{
				if (sizeForm.ShowDialog() == DialogResult.OK)
				{
					width = sizeForm.ImageWidth;
					height = sizeForm.ImageHeight;
					if (width == 0 || height == 0) { return; }
				}
			}

			var imageSharpImage = await Task.Run(() => Drawer.DrawFixedSize(
				new SixLabors.ImageSharp.Size(width, height),
				File.OpenRead(OpenFile.FileName),
				32,
				null,
				tokenSource.Token,
				progress));

			image = await Task.Run(() => imageSharpImage.ToSystemDrawingImage());
			SetPictureBoxImage();
			bitDepth = 32;
			imageSizeBytes = image.Width * image.Height * 4;
			SetTitleBar();
		}

		private async void TSBRefresh_Click(object sender, EventArgs e)
		{
			if (filePaths != null) { await GenerateImage(); }
		}

		private async void TSBSaveAs_Click(object sender, EventArgs e)
		{
			if (image == null || SaveFile.ShowDialog() != DialogResult.OK) { return; }

			string filePath = SaveFile.FileName;
			string extension = Path.GetExtension(filePath);
			switch (extension)
			{
				case null: return;
				case ".png":
					image.Save(filePath, ImageFormat.Png);
					break;
				case ".jpg":
				case ".jpeg":
					image.Save(filePath, ImageFormat.Jpeg);
					break;

				case ".gif":
					image.Save(filePath, ImageFormat.Gif);
					break;
				case ".raw":
					{
						var imageSharpImage = await Task.Run(() => image.ToImageSharpImage<Rgba32>());
						byte[] bytes = await Task.Run(() => Drawer.ToRaw(imageSharpImage, tokenSource.Token, progress));
						File.WriteAllBytes(filePath, bytes);
						break;
					}

				default:
					throw new ArgumentOutOfRangeException(nameof(extension), $"Cannot save a file with extension {extension}.");
			}
		}
		private async void TSBSort_Click(object sender, EventArgs e)
		{
			if (image == null) { return; }

			var imageSharpImage = await Task.Run(() => image.ToImageSharpImage<Rgba32>());
			var sortedImage = await Task.Run(() => Drawer.Sort(imageSharpImage, tokenSource.Token, progress));
			image = await Task.Run(() => sortedImage.ToSystemDrawingImage());
			SetPictureBoxImage();
		}

		private async void TSBUnique_Click(object sender, EventArgs e)
		{
			if (image == null) { return; }

			var imageSharpImage = await Task.Run(() => image.ToImageSharpImage<Rgba32>());
			var uniqueImage = await Task.Run(() => Drawer.UniqueColors(imageSharpImage, tokenSource.Token, progress));
			image = await Task.Run(() => uniqueImage.Image.ToSystemDrawingImage());

			image = await Task.Run(() => uniqueImage.Image.ToSystemDrawingImage());
			SetPictureBoxImage();
			Text = $"ByteView - {uniqueImage.UniqueColors} unique colors";
		}

		private async Task GenerateImage()
		{
			if (filePaths == null || filePaths.Length == 0) { return; }

			var source = new FileSource(filePaths);
			imageSizeBytes = source.FileSizes.Sum();
			IReadOnlyList<Rgba32> palette = null;
			if (bitDepth != 24 && bitDepth != 32) { palette = DefaultPalettes.GetPalette(bitDepth, colorMode); }

			try
			{
				var imageSharpImage = await Task.Run(() => Drawer.Draw(source.GetStream(),
					bitDepth,
					palette,
					tokenSource.Token,
					progress));

				image = await Task.Run(() => imageSharpImage.ToSystemDrawingImage());
				SetPictureBoxImage();
			}
			catch (TaskCanceledException) { ProgressBar.Value = 0; }
		}

		private void Progress_ProgressChanged(object o, DrawingProgress d)
		{
			var percentage = (int)(d.DrawnPixels * 100m / d.TotalPixels);
			Invoke((MethodInvoker)(() => ProgressBar.Value = percentage));
		}

		// Sets the title bar to "ByteView - {image size in bytes} - {image dimensions} - {pixels}
		private void SetTitleBar()
		{
			string imageSizeText = Utilities.GenerateFileSizeAbbreviation((ulong)imageSizeBytes, out _);
			string imagePixelCount = ImageInfoUtilities.GetPixelCountString(image.Width * image.Height);
			Text = $"ByteView - {imageSizeText} - {image.Width}x{image.Height} - {imagePixelCount}";
		}

		private void SetPictureBoxImage()
		{
			PictureBox.Image = image;
			PictureBox.Size = new Size(image.Width, image.Height);
		}

		private void TSBChromaPlayground_Click(object sender, EventArgs e)
		{
			new ChromaPlaygroundForm(this).ShowDialog();
		}
		
		internal Image<Rgba32> GetImage() => image?.ToImageSharpImage<Rgba32>();
	}
}
