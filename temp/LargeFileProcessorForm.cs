// LargeFileProcessorForm.cs
//
// Converts many files into many bitmaps of the same resolution.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Celarix.Imaging.ByteView
{
	/// <summary>
	/// Generates multiple images of a certain size from a large file or group of files.
	/// </summary>
	public partial class LargeFileProcessorForm : Form
    {
        /// <summary>
        /// A list of file paths to all the source files.
        /// </summary>
        private readonly List<KeyValuePair<string, FileInfo>> sourceFiles = new List<KeyValuePair<string, FileInfo>>();

		/// <summary>
		/// The desired color mode for the generated images.
		/// </summary>
        private ColorMode colorMode;

		/// <summary>
		/// The desired bit depth for the generated images.
		/// </summary>
        private BitDepth bitDepth;

		/// <summary>
		/// Gets the width of the desired image in pixels.
		/// </summary>
        private int ImageWidth => int.Parse(TextBoxImageWidth.Text, CultureInfo.InvariantCulture);

        /// <summary>
        /// Gets the height of the desired image in pixels.
        /// </summary>
        private int ImageHeight => int.Parse(TextBoxImageHeight.Text, CultureInfo.InvariantCulture);

        /// <summary>
        /// Initializes a new instance of the <see cref="LargeFileProcessorForm"/> class.
        /// </summary>
        public LargeFileProcessorForm()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Initializes the value of some controls on the form.
        /// </summary>
        /// <param name="sender">The object that invoked this event handler.</param>
        /// <param name="e">Arguments for this event.</param>
        private void LargeFileProcessorForm_Load(object sender, EventArgs e)
        {
            ComboBoxBitDepths.SelectedIndex = 0;
            UpdateFileInfo();
            UpdateImageInfo();
        }

		/// <summary>
		/// Adds files selected in <see cref="OFDAddFile"/> to the source files.
		/// </summary>
		/// <param name="sender">The object that invoked this event handler.</param>
		/// <param name="e">Arguments for this event.</param>
		private void ButtonAddFiles_Click(object sender, EventArgs e)
		{
			if (OFDAddFile.ShowDialog() == DialogResult.OK)
			{
				AddFiles(OFDAddFile.FileNames);
			}
		}
		private void ButtonAddFolder_Click(object sender, EventArgs e)
		{
            if (FBDAddFolder.ShowDialog() != DialogResult.OK) { return; }

			AddFolderWorker.RunWorkerAsync(FBDAddFolder.SelectedPath);
        }

		private void ButtonClose_Click(object sender, EventArgs e) => Close();

		private void ButtonGenerate_Click(object sender, EventArgs e)
		{
			// Verify the folder path.
			if (!Directory.Exists(TextOutputFolder.Text)) { Directory.CreateDirectory(TextOutputFolder.Text); }

			ButtonStop.Enabled = true;
            Worker.RunWorkerAsync();
        }

		private void ButtonSelectOutputFolder_Click(object sender, EventArgs e)
		{
			if (FBDOutputFolder.ShowDialog() == DialogResult.OK)
			{
				TextOutputFolder.Text = FBDOutputFolder.SelectedPath;
			}
		}

		private void ButtonStop_Click(object sender, EventArgs e) => Worker.CancelAsync();

		/// <summary>
		/// Changes the bit depth to the one specified by <see cref="ComboBoxBitDepths"/> and toggles the color modes appropriately.
		/// </summary>
		/// <param name="sender">The object that invoked this event handler.</param>
		/// <param name="e">Arguments for this event.</param>
		private void ComboBoxBitDepths_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch (ComboBoxBitDepths.SelectedIndex)
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

			ValidateSize();
			UpdateImageInfo();
		}

		/// <summary>
		/// Removes the selected file from the source files if the Delete key has been pressed.
		/// </summary>
		/// <param name="sender">The object that invoked this event handler.</param>
		/// <param name="e">Arguments for this event.</param>
		private void ListBoxFiles_KeyDown(object sender, KeyEventArgs e)
		{
            if (e.KeyCode != Keys.Delete || ListBoxFiles.SelectedIndex < 0) { return; }

            int index = ListBoxFiles.SelectedIndex;
            string filePathToRemove = (string)ListBoxFiles.Items[index];
            ListBoxFiles.Items.RemoveAt(index);

            int indexToRemove = sourceFiles.FindIndex(kvp => kvp.Key == filePathToRemove);
            sourceFiles.RemoveAt(indexToRemove);

            if (ListBoxFiles.Items.Count > 0)
            {
                ListBoxFiles.SelectedIndex = index - 1;
            }

            UpdateFileInfo();
        }

		/// <summary>
		/// Sets the color mode to grayscale.
		/// </summary>
		/// <param name="sender">The object that invoked this event handler.</param>
		/// <param name="e">Arguments for this event.</param>
		private void RadioGrayscale_CheckedChanged(object sender, EventArgs e)
		{
			colorMode = ColorMode.Grayscale;
		}

		/// <summary>
		/// Sets the color mode to RGB.
		/// </summary>
		/// <param name="sender">The object that invoked this event handler.</param>
		/// <param name="e">Arguments for this event.</param>
		private void RadioRGB_CheckedChanged(object sender, EventArgs e)
		{
			colorMode = ColorMode.RGB;
		}

		/// <summary>
		/// Sets the color mode to ARGB.
		/// </summary>
		/// <param name="sender">The object that invoked this event handler.</param>
		/// <param name="e">Arguments for this event.</param>
		private void RadioARGB_CheckedChanged(object sender, EventArgs e)
		{
			colorMode = ColorMode.ARGB;
		}

		/// <summary>
		/// Sets the color mode to paletted.
		/// </summary>
		/// <param name="sender">The object that invoked this event handler.</param>
		/// <param name="e">Arguments for this event.</param>
		private void RadioPaletted_CheckedChanged(object sender, EventArgs e)
		{
			colorMode = ColorMode.Paletted;
		}

		private void TextBoxFileEntry_KeyDown(object sender, KeyEventArgs e)
		{
            if (e.KeyCode != Keys.Enter) { return; }

            string filePath = TextBoxFileEntry.Text;
            if (!File.Exists(filePath))
            {
                MessageBox.Show("The file path you entered is not valid.", "Invalid File Path",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            sourceFiles.Add(new KeyValuePair<string, FileInfo>(filePath, new FileInfo(filePath)));
            ListBoxFiles.Items.Add(filePath);
            UpdateFileInfo();
        }

		/// <summary>
		/// Validates the image's width and height and updates the <see cref="LabelImageData"/> label.
		/// </summary>
		/// <param name="sender">The object that invoked this event handler.</param>
		/// <param name="e">Arguments for this event.</param>
		private void TextBoxImageWidth_TextChanged(object sender, EventArgs e)
		{
			ValidateSize();
			UpdateImageInfo();
		}

		/// <summary>
		/// Validates the image's height and updates the <see cref="LabelImageData"/> label.
		/// </summary>
		/// <param name="sender">The object that invoked this event handler.</param>
		/// <param name="e">Arguments for this event.</param>
		private void TextBoxImageHeight_TextChanged(object sender, EventArgs e)
		{
			ValidateSize();
			UpdateImageInfo();
		}

		private void Worker_DoWork(object sender, DoWorkEventArgs e)
		{
			// Step 1: Set up some important variables.
            int imageSize = GetImageSize();
            long sourceFilesSize = sourceFiles.Sum(kvp => kvp.Value.Length);

			int totalImages, remainder = 0;
			if (sourceFilesSize % imageSize == 0)
			{
				totalImages = (int)(sourceFilesSize / imageSize);
			}
			else
			{
				totalImages = (int)((sourceFilesSize / imageSize) + 1);
				remainder = (int)((totalImages * imageSize) - sourceFilesSize);
			}

			// Step 2: Create each image.
			int fileIndex = 0;
            int[] palette = null;
			if (bitDepth != BitDepth.TwentyFourBpp && bitDepth != BitDepth.ThirtyTwoBpp)
			{
				palette = DefaultPalettes.GetPalette(bitDepth, colorMode);
			}

			var reader = new MultiFileStream(sourceFiles.Select(kvp => kvp.Key).ToList());
            for (int i = 0; i < totalImages; i++)
            {
                string statusText = $"Creating images ({i + 1} of {totalImages}).";
                int percentage = (int)(100m * ((i + 1m) / totalImages));
                Invoke((MethodInvoker)(() => LabelStatus.Text = statusText));
                Invoke((MethodInvoker)(() => Progress.Value = percentage));

                byte[] currentImage;
                IList<string> filesOnImages;
                if (remainder != 0 && i == totalImages - 1)
                {
                    currentImage = new byte[imageSize - remainder];
                    filesOnImages = reader.ReadReturnFileNames(currentImage, imageSize - remainder);
                }
                else
                {
                    currentImage = new byte[imageSize];
                    filesOnImages = reader.ReadReturnFileNames(currentImage, imageSize);
                }

                Bitmap result;
                if (!CheckDrawFileNames.Checked)
                {
                    result = Drawer.Draw(currentImage,
                        bitDepth,
                        palette,
                        Worker,
                        new Size(ImageWidth, ImageHeight));
                }
                else
                {
                    result = Drawer.DrawWithText(currentImage,
                        bitDepth,
                        palette,
                        Worker,
                        new Size(ImageWidth, ImageHeight),
                        Helpers.GetLFPImageText(filesOnImages));
                }

                string resultPath = Path.Combine(TextOutputFolder.Text,
                    $"image_{fileIndex:D8}.png");

                result.Save(resultPath, ImageFormat.Png);
                fileIndex++;

                if (Worker.CancellationPending) { goto cleanup; }
            }

		cleanup:
			const string cleanupStatusText = "Waiting...";
			const int cleanupProgress = 0;
			Invoke((MethodInvoker)(() => LabelStatus.Text = cleanupStatusText));
			Invoke((MethodInvoker)(() => Progress.Value = cleanupProgress));
			Invoke((MethodInvoker)(() => ButtonStop.Enabled = false));
		}

		private void AddFiles(IEnumerable<string> paths)
		{
			foreach (string filePath in paths)
			{
				sourceFiles.Add(new KeyValuePair<string, FileInfo>(filePath, new FileInfo(filePath)));
				ListBoxFiles.Items.Add(filePath);
			}
			UpdateFileInfo();
		}

		// Updates the LabelFilesData control when files are added or removed.
		private void UpdateFileInfo()
        {
            ulong totalSize = 0UL;
            foreach (var kvp in sourceFiles)
            {
                totalSize += (ulong)kvp.Value.Length;
            }

            string sizeSuffix = Helpers.GenerateFileSizeAbbreviation(totalSize, out int size);
			LabelFilesData.Text =
                $"{sourceFiles.Count} file{((sourceFiles.Count == 1) ? "" : "s")} loaded. Total size: {size} {sizeSuffix}.";
        }

		// Updates the LabelImageData control when the size TextBoxes are changed.
        private void UpdateImageInfo()
        {
            if (!ButtonGenerate.Enabled) { return; }

            int pixels = ImageWidth * ImageHeight;
            int size = GetImageSize();
            string sizeString = Helpers.GenerateFileSizeAbbreviation((ulong)size, out int sizeNumber);
			int effectiveHeight = (CheckDrawFileNames.Checked) ? (ImageHeight - Helpers.GetTextHeight(ImageHeight)) : ImageHeight;

			LabelImageData.Text =
                $"{pixels} pixels. Total size: {sizeNumber} {sizeString}. Effective height: {effectiveHeight}.";
        }

		// Validates that the size TextBoxes contain positive numbers.
        private void ValidateSize()
        {
            bool valid = false;
            if (!int.TryParse(TextBoxImageWidth.Text, out int _) || !int.TryParse(TextBoxImageHeight.Text, out int _))
			{
				LabelImageData.Text = "Invalid width or height.";
			}
			else if (GetImageSize() == 0)
			{
				LabelImageData.Text = "Image size is too low to produce files.";
			}
			else if (GetImageSize() == int.MaxValue || GetImageSize() < 0)
			{
				LabelImageData.Text = "Image size is too large.";
			}
            else
            {
                valid = true;
            }

			ButtonGenerate.Enabled = valid;
        }

		// Gets the size of each image in bytes.
        private int GetImageSize()
        {
            int width = ImageWidth;
            int height = (CheckDrawFileNames.Checked)
				? ImageHeight - Helpers.GetTextHeight(ImageHeight)
				: ImageHeight;
            decimal[] divisors = { decimal.MinValue, (1m / 8m), (1m / 4m), (1m / 2m), 1m, 2m, 3m, 4m };
			decimal bytes = Math.Floor(width * height * divisors[(int)bitDepth]);

			if (bytes < int.MaxValue)
			{
				return (int)bytes;
			}

            return int.MaxValue;
        }

		private void CheckDrawFileNames_CheckedChanged(object sender, EventArgs e)
		{
			UpdateImageInfo();
		}

		private void AddFolderWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            IEnumerable<string> filePathEnumerable = Directory.EnumerateFiles((string)e.Argument,
                "*.*",
                SearchOption.AllDirectories);

            var newSourceFiles = new List<KeyValuePair<string, FileInfo>>();
            int filesFoundSoFar = 0;

            foreach (string filePath in filePathEnumerable)
            {
                newSourceFiles.Add(new KeyValuePair<string, FileInfo>(filePath, new FileInfo(filePath)));
                filesFoundSoFar++;

                if (filesFoundSoFar % 500 != 0) { continue; }

                string statusText = $"Found {filesFoundSoFar} files in folder so far.";
                Invoke((MethodInvoker)(() => LabelStatus.Text = statusText));
            }

            string finalStatusText = $"Found {filesFoundSoFar} files in folder, total.";
            Invoke((MethodInvoker)(() => LabelStatus.Text = finalStatusText));

			newSourceFiles = newSourceFiles.OrderBy(kvp => kvp.Key).ToList();
			sourceFiles.AddRange(newSourceFiles);

            Invoke((MethodInvoker)(() =>
			{
				ListBoxFiles.Items.AddRange(newSourceFiles.Select(kvp => kvp.Key).ToArray());
                UpdateFileInfo();
            }));
            Invoke((MethodInvoker)(() => LabelStatus.Text = "Waiting..."));
		}
	}
}
