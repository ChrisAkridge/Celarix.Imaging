// LargeFileProcessorForm.cs
//
// Converts many files into many bitmaps of the same resolution.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Celarix.Imaging.BinaryDrawing;
using Celarix.Imaging.IO;
using Celarix.Imaging.Progress;
using Celarix.Imaging.ZoomableCanvas;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

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
        private int bitDepth;

        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();

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
	        LibraryConfiguration.Instance.ZoomableCanvasTileEdgeLength = 1024;
	        
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
		private async void ButtonAddFolder_Click(object sender, EventArgs e)
		{
            if (FBDAddFolder.ShowDialog() != DialogResult.OK) { return; }

            var progress = new Progress<int>();
            progress.ProgressChanged += (o, i) =>
                Invoke((MethodInvoker)(() => LabelStatus.Text = $"Found {i} files."));

            var newFiles = await Task.Run(() => AddAllFilesFromFolder(progress));
			ListBoxFiles.Items.AddRange(newFiles.ToArray());
            LabelStatus.Text = "Waiting...";

			UpdateFileInfo();
        }

		private void ButtonClose_Click(object sender, EventArgs e) => Close();

		private async void ButtonGenerate_Click(object sender, EventArgs e)
		{
			// Verify the folder path.
			if (!Directory.Exists(TextOutputFolder.Text)) { Directory.CreateDirectory(TextOutputFolder.Text); }

			ButtonStop.Enabled = true;

            if (!CheckDrawAsZoomableTiles.Checked) { await GenerateImages(); }
            else { await GenerateZoomableTiles();  }
        }

		private void ButtonSelectOutputFolder_Click(object sender, EventArgs e)
		{
			if (FBDOutputFolder.ShowDialog() == DialogResult.OK)
			{
				TextOutputFolder.Text = FBDOutputFolder.SelectedPath;
			}
		}

        private void ButtonStop_Click(object sender, EventArgs e) => tokenSource.Cancel();

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

            var index = ListBoxFiles.SelectedIndex;
            var filePathToRemove = (string)ListBoxFiles.Items[index];
            ListBoxFiles.Items.RemoveAt(index);

            var indexToRemove = sourceFiles.FindIndex(kvp => kvp.Key == filePathToRemove);
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
			colorMode = ColorMode.Rgb;
		}

		/// <summary>
		/// Sets the color mode to ARGB.
		/// </summary>
		/// <param name="sender">The object that invoked this event handler.</param>
		/// <param name="e">Arguments for this event.</param>
		private void RadioARGB_CheckedChanged(object sender, EventArgs e)
		{
			colorMode = ColorMode.Argb;
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

            var filePath = TextBoxFileEntry.Text;
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

        private async Task GenerateImages()
        {
            var filePaths = sourceFiles.Select(kvp => kvp.Key);
			var stream = new NamedMultiStream(filePaths);
            var imageByteCount = GetImageByteCount();
            var imageSize = new SixLabors.ImageSharp.Size(ImageWidth, ImageHeight);
            var allFilesSize = sourceFiles.Sum(kvp => kvp.Value.Length);
            var progress = new Progress<DrawingProgress>();

            int totalImages;
            if (allFilesSize % imageByteCount == 0) { totalImages = (int)(allFilesSize / imageByteCount); }
            else
            {
                totalImages = (int)((allFilesSize / imageByteCount) + 1);
            }

            IReadOnlyList<Rgba32> palette = null;
            if (bitDepth != 24 && bitDepth != 32) { palette = DefaultPalettes.GetPalette(bitDepth, colorMode); }

            for (var i = 0; i < totalImages; i++)
            {
                LabelStatus.Text = $"Creating images ({i + 1} of {totalImages}).";
                Progress.Value = (int)(100m * ((i + 1m) / totalImages));

                try
                {
                    Image<Rgba32> image;
                    if (!CheckDrawFileNames.Checked)
                    {
                        image = await Task.Run(() => Drawer.DrawFixedSize(new PartiallyKnownSize(ImageWidth, ImageHeight),
                            stream,
                            bitDepth,
                            palette,
                            tokenSource.Token,
                            progress));
                    }
                    else
                    {
                        image = await Task.Run(() => Drawer.DrawFixedSizeWithSourceText(imageSize,
                            stream,
                            bitDepth,
                            palette,
                            tokenSource.Token,
                            progress));
                    }

                    var resultPath = Path.Combine(TextOutputFolder.Text,
                        $"image_{i:D8}.png");

                    await Task.Run(() => image.SaveAsPngAsync(resultPath, tokenSource.Token));
                }
                catch (TaskCanceledException)
                {
                    LabelStatus.Text = "Waiting...";
                    Progress.Value = 0;
                    ButtonStop.Enabled = false;
                }
            }

            LabelStatus.Text = "Waiting...";
            Progress.Value = 0;
            ButtonStop.Enabled = false;
		}

        private async Task GenerateZoomableTiles()
        {
            var filePaths = sourceFiles.Select(kvp => kvp.Key);
            var stream = new NamedMultiStream(filePaths);
            var totalBytes = stream.Length;
            var imageBytes = (int)(256m * 256m * (bitDepth / 8m));
            var level0Tiles = (int)Math.Ceiling((decimal)totalBytes / imageBytes);
            var estimatedTotalTiles = (int)Math.Ceiling(level0Tiles * Math.Log(level0Tiles, 4d));
            var canvasSizeInTiles = Utilities.GetSizeFromCount(level0Tiles);
            var tileEdgeLength = LibraryConfiguration.Instance.ZoomableCanvasTileEdgeLength;
            var outputFolder = TextOutputFolder.Text;

            IReadOnlyList<Rgba32> palette = null;
            if (bitDepth != 24 && bitDepth != 32) { palette = DefaultPalettes.GetPalette(bitDepth, colorMode); }

            var level0Path = Path.Combine(outputFolder, "0");
            Directory.CreateDirectory(level0Path);

            var savedTiles = 0;
            for (var y = 0; y < canvasSizeInTiles.Height; y++)
            {
                for (var x = 0; x < canvasSizeInTiles.Width; x++)
                {
                    LabelStatus.Text = $"Creating level 0 tiles ({savedTiles + 1} of {level0Tiles}).";
                    Progress.Value = (int)(100m * ((savedTiles + 1m) / estimatedTotalTiles));

                    var tile = await Task.Run(() => Drawer.DrawFixedSize(new PartiallyKnownSize(tileEdgeLength, tileEdgeLength),
                        stream,
                        bitDepth,
                        palette,
                        tokenSource.Token,
                        null
                    ));

                    await Task.Run(() =>
                        tile.SaveAsPngAsync(Path.Combine(level0Path, $"{x},{y}.png"), tokenSource.Token));

                    savedTiles += 1;
                }
            }

            var levelGeneratorProgress = new Progress<string>();
            levelGeneratorProgress.ProgressChanged += (sender, progress) =>
            {
                var progressValue = (int)(100m * ((savedTiles + 1m) / estimatedTotalTiles));
                savedTiles += 1;

                Invoke((MethodInvoker)(() =>
                {
                    LabelStatus.Text = progress;
                    Progress.Value = progressValue;
                }));
            };

            var nextZoomLevel = 0;
            while (await Task.Run(() => ZoomLevelGenerator.TryCombineImagesForNextZoomLevel(Path.Combine(outputFolder, nextZoomLevel.ToString()),
                outputFolder,
                nextZoomLevel + 1,
                levelGeneratorProgress))) { nextZoomLevel += 1; }

            LabelStatus.Text = "Waiting...";
            Progress.Value = 0;
            ButtonStop.Enabled = false;
        }

        private void AddFiles(IEnumerable<string> paths)
		{
			foreach (var filePath in paths)
			{
				sourceFiles.Add(new KeyValuePair<string, FileInfo>(filePath, new FileInfo(filePath)));
				ListBoxFiles.Items.Add(filePath);
			}
			UpdateFileInfo();
		}

		// Updates the LabelFilesData control when files are added or removed.
		private void UpdateFileInfo()
		{
			var totalSize = GetSizeOfAllFiles();

			var sizeSuffix = Utilities.GenerateFileSizeAbbreviation(totalSize, out var size);
			LabelFilesData.Text =
				$"{sourceFiles.Count} file{((sourceFiles.Count == 1) ? "" : "s")} loaded. Total size: {size} {sizeSuffix}.";

            if (CheckDrawAsZoomableTiles.Checked) { UpdateImageInfo(); }
        }

		private ulong GetSizeOfAllFiles()
		{
			var totalSize = 0UL;
			foreach (var kvp in sourceFiles)
			{
				totalSize += (ulong)kvp.Value.Length;
			}

			return totalSize;
		}

		// Updates the LabelImageData control when the size TextBoxes are changed.
		private void UpdateImageInfo()
        {
            if (!ButtonGenerate.Enabled) { return; }

            if (!CheckDrawAsZoomableTiles.Checked)
            {
                var pixels = ImageWidth * ImageHeight;
                var size = GetImageByteCount();
                var sizeString = Utilities.GenerateFileSizeAbbreviation((ulong)size, out var sizeNumber);
                var effectiveHeight = CheckDrawFileNames.Checked
                    ? (ImageHeight - Utilities.GetTextHeight(ImageHeight))
                    : ImageHeight;

                LabelImageData.Text =
                    $"{pixels} pixels. Total size: {sizeNumber} {sizeString}. Effective height: {effectiveHeight}.";
            }
            else
            {
                var totalFileBytes = GetSizeOfAllFiles();

                if (totalFileBytes == 0)
                {
                    LabelImageData.Text = "No files selected.";
                    return;
                }

                var level0Tiles = (double)Math.Ceiling(totalFileBytes / (bitDepth / 8m) / 65536m);
                var maxZoomLevelEstimate = Math.Ceiling(Math.Log(level0Tiles, 2d));
                var totalTileEstimate = Math.Ceiling(level0Tiles * maxZoomLevelEstimate);

                LabelImageData.Text =
                    $"Roughly {totalTileEstimate} 256x256 tiles with maximum zoom level of {maxZoomLevelEstimate}";
            }
        }

		// Validates that the size TextBoxes contain positive numbers.
        private void ValidateSize()
        {
            var valid = false;
            if (!int.TryParse(TextBoxImageWidth.Text, out var _) || !int.TryParse(TextBoxImageHeight.Text, out var _))
			{
				LabelImageData.Text = "Invalid width or height.";
			}
			else if (GetImageByteCount() == 0)
			{
				LabelImageData.Text = "Image size is too low to produce files.";
			}
			else if (GetImageByteCount() == int.MaxValue || GetImageByteCount() < 0)
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
        private int GetImageByteCount()
        {
            var width = ImageWidth;
            var height = CheckDrawFileNames.Checked
				? ImageHeight - Utilities.GetTextHeight(ImageHeight)
				: ImageHeight;

            return GetImageByteCount(width, height, bitDepth);
        }

        private static int GetImageByteCount(int width, int height, int depth)
        {
            var divisors = new Dictionary<int, decimal>
            {
                { 1, 1m / 8m },
                { 2, 1m / 4m },
                { 4, 1m / 2m },
                { 8, 1m },
                { 16, 2m },
                { 24, 3m },
                { 32, 4m }
            };

            var bytes = Math.Floor(width * height * divisors[depth]);

            if (bytes < int.MaxValue) { return (int)bytes; }

            return int.MaxValue;
		}

		private void CheckDrawFileNames_CheckedChanged(object sender, EventArgs e)
		{
			UpdateImageInfo();
		}

        private void CheckDrawAsZoomableTiles_CheckedChanged(object sender, EventArgs e)
        {
            TextBoxImageWidth.Enabled = !CheckDrawAsZoomableTiles.Checked;
            TextBoxImageHeight.Enabled = !CheckDrawAsZoomableTiles.Checked;

            UpdateImageInfo();
        }

        private IEnumerable<string> AddAllFilesFromFolder(IProgress<int> progress)
        {
            var filePathEnumerable = Directory.EnumerateFiles(FBDAddFolder.SelectedPath,
                "*.*",
                SearchOption.AllDirectories);

			var newSourceFiles = new List<KeyValuePair<string, FileInfo>>();
            var filesFoundSoFar = 0;

            foreach (var filePath in filePathEnumerable)
            {
                newSourceFiles.Add(new KeyValuePair<string, FileInfo>(filePath, new FileInfo(filePath)));
                filesFoundSoFar++;

                if (filesFoundSoFar % 500 != 0) { continue; }

                progress?.Report(filesFoundSoFar);
            }

            progress?.Report(filesFoundSoFar);

            newSourceFiles = newSourceFiles.OrderBy(kvp => kvp.Key).ToList();
            sourceFiles.AddRange(newSourceFiles);

            return newSourceFiles.Select(kvp => kvp.Key);
        }
	}
}
