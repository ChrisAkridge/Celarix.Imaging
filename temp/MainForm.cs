//	MainForm.cs
//
//	The main form for the application; displays files as bitmapped images.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Celarix.Imaging.ByteView
{
	public partial class MainForm : Form
    {
        // The currently selected bit depth.
        private BitDepth bitDepth;

        // The currently selected color mode.
        private ColorMode colorMode;

        // The current set of file paths to generate an image from.
        private string[] filePaths;

        // The current image displayed on the form.
        private Bitmap image;

		// The size in bytes of all the files currently displayed in the image.
		private long imageSizeBytes;

		/// <summary>
		/// Initializes a new instance of the <see cref="MainForm"/> class. 
		/// </summary>
		public MainForm()
		{
			InitializeComponent();
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
			Worker_DoWork(this, new DoWorkEventArgs(this));
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
					bitDepth = BitDepth.OneBpp;
					RadioGrayscale.Enabled = true;
					RadioRGB.Enabled = false;
					RadioARGB.Enabled = false;
					RadioPaletted.Enabled = true;
					RadioGrayscale.Checked = true;
                    break;
                case 1:
					bitDepth = BitDepth.TwoBpp;
					RadioGrayscale.Enabled = true;
					RadioRGB.Enabled = false;
					RadioARGB.Enabled = false;
					RadioPaletted.Enabled = true;
					RadioGrayscale.Checked = true;
                    break;
                case 2:
					bitDepth = BitDepth.FourBpp;
					RadioGrayscale.Enabled = true;
					RadioRGB.Enabled = true;
					RadioARGB.Enabled = false;
					RadioPaletted.Enabled = true;
					RadioGrayscale.Checked = true;
                    break;
                case 3:
					bitDepth = BitDepth.EightBpp;
					RadioGrayscale.Enabled = true;
					RadioRGB.Enabled = true;
					RadioARGB.Enabled = true;
					RadioPaletted.Enabled = true;
					RadioGrayscale.Checked = true;
                    break;
                case 4:
					bitDepth = BitDepth.SixteenBpp;
					RadioGrayscale.Enabled = false;
					RadioRGB.Enabled = true;
					RadioARGB.Enabled = true;
					RadioPaletted.Enabled = false;
					RadioRGB.Checked = true;
                    break;
                case 5:
					bitDepth = BitDepth.TwentyFourBpp;
					RadioGrayscale.Enabled = false;
					RadioRGB.Enabled = true;
					RadioARGB.Enabled = true;
					RadioPaletted.Enabled = false;
					RadioRGB.Checked = true;
                    break;
                case 6:
					bitDepth = BitDepth.ThirtyTwoBpp;
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
                colorMode = ColorMode.ARGB;
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
				colorMode = ColorMode.RGB;
            }
        }

        private void TSBCancel_Click(object sender, EventArgs e) => Worker.CancelAsync();

        private void TSBLargeFileProcessor_Click(object sender, EventArgs e)
        {
			using (var lfpForm = new LargeFileProcessorForm())
			{
				lfpForm.ShowDialog();
			}
        }

        private void TSBOpenFiles_Click(object sender, EventArgs e)
        {
            if (OpenFile.ShowDialog() != DialogResult.OK) { return; }

            filePaths = OpenFile.FileNames;
            filePaths = filePaths.OrderBy(s => s).ToArray();
            Worker.RunWorkerAsync();
        }

        private void TSBOpenFolder_Click(object sender, EventArgs e)
        {
            if (FolderSelector.ShowDialog() != DialogResult.OK) { return; }

            filePaths = Directory.EnumerateFiles(FolderSelector.SelectedPath, "*.*", 
                SearchOption.AllDirectories).ToArray();
            filePaths = filePaths.OrderBy(s => s).ToArray();
            Worker.RunWorkerAsync();
        }

        private void TSBOpenPicture_Click(object sender, EventArgs e)
        {
            if (OpenPicture.ShowDialog() != DialogResult.OK) { return; }

            string filePath = OpenPicture.FileName;
            imageSizeBytes = new FileInfo(filePath).Length;
            image = (Bitmap)Image.FromFile(filePath);
            bitDepth = BitDepth.ThirtyTwoBpp;
            SetPictureBoxImage();

            SetTitleBar();
        }

		private void TSBOpenRaw_Click(object sender, EventArgs e)
		{
            if (OpenFile.ShowDialog() != DialogResult.OK) { return; }

            string filePath = OpenFile.FileName;
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

            image = Drawer.OpenRaw(filePath, width, height);
            SetPictureBoxImage();
            bitDepth = BitDepth.ThirtyTwoBpp;
            imageSizeBytes = image.Width * image.Height * 4;
            SetTitleBar();
        }

        private void TSBRefresh_Click(object sender, EventArgs e)
        {
            if (filePaths != null)
            {
				Worker.RunWorkerAsync();
            }
        }

        private void TSBSaveAs_Click(object sender, EventArgs e)
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
                    byte[] bytes = Drawer.BitmapToByteArray(image);
                    File.WriteAllBytes(filePath, bytes);
                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException(nameof(extension), $"Cannot save a file with extension {extension}.");
            }
        }
        private void TSBSort_Click(object sender, EventArgs e)
        {
            if (image == null) { return; }

            image = Drawer.Sort(image);
			SetPictureBoxImage();
		}

		private void TSBUnique_Click(object sender, EventArgs e)
        {
            if (image == null) { return; }

            image = Drawer.UniqueColors(image, out string colorCount);
			SetPictureBoxImage();
			Text = $"ByteView - {colorCount} unique colors";
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (filePaths == null || filePaths.Length == 0) { return; }

            var source = new FileSource(filePaths);
            imageSizeBytes = source.FileSizes.Sum();
            int[] palette = null;
            if (bitDepth != BitDepth.TwentyFourBpp && bitDepth != BitDepth.ThirtyTwoBpp)
            {
                palette = DefaultPalettes.GetPalette(bitDepth, colorMode);
            }

            image = Drawer.Draw(source, bitDepth, palette, Worker);
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
			ProgressBar.Value = e.ProgressPercentage;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else
			{
				SetPictureBoxImage();
				SetTitleBar();
				ProgressBar.Value = 0;
			}
		}

        // Sets the title bar to "ByteView - {image size in bytes} - {image dimensions} - {pixels}
		private void SetTitleBar()
		{
			string imageSizeText = Helpers.GenerateFileSizeAbbreviation((ulong)imageSizeBytes, out _);
			string imagePixelCount = ImageInfoUtilities.GetPixelCountString(image.Width * image.Height);
			Text = $"ByteView - {imageSizeText} - {image.Width}x{image.Height} - {imagePixelCount}";
		}

		private void SetPictureBoxImage()
		{
			PictureBox.Image = image;
			PictureBox.Size = new Size(image.Width, image.Height);
		}
    }
}
