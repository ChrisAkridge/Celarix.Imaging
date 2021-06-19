using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Celarix.Imaging.JobRecovery;
using Celarix.Imaging.Packing;
using Celarix.Imaging.Progress;
using Celarix.Imaging.Tiling;
using Celarix.Imaging.ZoomableCanvas;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Point = System.Drawing.Point;

namespace Celarix.Imaging.PictureTiler
{
	public partial class MainForm : Form
    {
        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();

		public MainForm()
		{
			InitializeComponent();
		}

        private async void MainForm_Load(object sender, EventArgs e)
        {
            var latestJobFilePath = JobManager.GetLatestJobFilePath(JobSources.Packer);
            if (latestJobFilePath == null)
            {
                return;
            }

            var dialogResult =
                MessageBox.Show("An existing packer job has been located.\r\nDo you want to continue it?",
                    "PictureTiler", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

            if (dialogResult == DialogResult.Yes)
            {
                await Pack(resuming: true);
            }
            else
            {
                JobManager.CompleteJob(JobSources.Packer);
            }
        }

        private void ButtonTilerSelectInputFolder_Click(object sender, EventArgs e)
		{
            if (FBDMain.ShowDialog() == DialogResult.OK) { TextTilerInputFolder.Text = FBDMain.SelectedPath; }
		}

		private void ButtonTilerSelectOutputPath_Click(object sender, EventArgs e)
		{
            if (SFDMain.ShowDialog() == DialogResult.OK) { TextTilerOutputPath.Text = SFDMain.FileName; }
		}

		private async void ButtonTilerGenerate_Click(object sender, EventArgs e)
        {
            ButtonTilerCancel.Enabled = true;

            var options = new TileOptions
            {
                TileWidth = (int)NUDTilerTileWidth.Value,
                TileHeight = (int)NUDTilerTileHeight.Value
            };

			var imagesInFolder = Directory.GetFiles(TextTilerInputFolder.Text, "*", SearchOption.TopDirectoryOnly)
				.Where(Utilities.IsFileAnImage).ToList();

            var images = Utilities.ImageEnumerable(imagesInFolder);
            ProgressTiler.Maximum = imagesInFolder.Count;

            var progress = new Progress<int>();
            progress.ProgressChanged += (s, p) =>
            {
                var statusText = $"Tiling images ({p} of {imagesInFolder.Count})...";

                Invoke((MethodInvoker)(() =>
                {
                    ProgressTiler.Value = p;
                    LabelTilerStatus.Text = statusText;
                }));
            };

            try
            {
                var image = await Task.Run(() => Tiler.Tile(options,
                    images,
                    imagesInFolder.Count,
                    tokenSource.Token,
                    progress));

                await Task.Run(() => image.SaveAsPngAsync(TextTilerOutputPath.Text));
            }
            catch (TaskCanceledException) { }

            ButtonTilerCancel.Enabled = false;
            ProgressTiler.Value = 0;
            LabelTilerStatus.Text = "Waiting...";
        }

		private void ButtonPackerSelectInputPath_Click(object sender, EventArgs e)
		{
            if (FBDMain.ShowDialog() == DialogResult.OK) { TextPackerInputPath.Text = FBDMain.SelectedPath; }
        }

		private void ButtonPackerSelectOutputPath_Click(object sender, EventArgs e)
		{
            if (FBDMain.ShowDialog() == DialogResult.OK) { TextPackerOutputPath.Text = FBDMain.SelectedPath; }
        }

		private async void ButtonPackerGenerate_Click(object sender, EventArgs e)
        {
            await Pack(resuming: false);
        }

		private void ButtonCanvasSelectInputPath_Click(object sender, EventArgs e)
        {
            if (OFDMain.ShowDialog() == DialogResult.OK) { TextCanvasInputPath.Text = OFDMain.FileName; }
        }

		private void ButtonCanvasSelectOutputPath_Click(object sender, EventArgs e)
        {
            if (FBDMain.ShowDialog() == DialogResult.OK) { TextCanvasOutputPath.Text = FBDMain.SelectedPath; }
        }

        private async void ButtonCanvasGenerate_Click(object sender, EventArgs e)
        {
            ButtonCanvasCancel.Enabled = true;

            var progress = new Progress<string>();

            progress.ProgressChanged += (s, p) =>
                Invoke((MethodInvoker)(() => LabelCanvasStatus.Text = p));

            try
            {
                await Task.Run(() => CanvasGenerator.Generate(TextCanvasInputPath.Text,
                    new SixLabors.ImageSharp.Size(256, 256),
                    TextCanvasOutputPath.Text,
                    tokenSource.Token,
                    progress));
            }
            catch (TaskCanceledException) { }

            LabelCanvasStatus.Text = "Waiting...";
        }

		private void ButtonCanvasCancel_Click(object sender, EventArgs e) { tokenSource.Cancel(); }

		private void ButtonPackerCancel_Click(object sender, EventArgs e) { tokenSource.Cancel(); }

		private void ButtonTilerCancel_Click(object sender, EventArgs e) { tokenSource.Cancel(); }

		private void groupBox1_Enter(object sender, EventArgs e)
		{

		}

        private async Task Pack(bool resuming)
        {
            ButtonPackerCancel.Enabled = true;

            var recursive = CheckPackerRecursive.Checked;
            var zoomableCanvas = CheckPackerMultipicture.Checked;

            var options = new PackingOptions
            {
                InputFolderPath = TextPackerInputPath.Text,
                OutputPath = TextPackerOutputPath.Text,
                Multipicture = zoomableCanvas,
                Recursive = recursive
            };

            var progress = new Progress<string>();

            progress.ProgressChanged += (s, p) =>
                Invoke((MethodInvoker)(() => LabelPackerStatus.Text = p));

            try
            {
                if (!resuming)
                {
                    await Task.Run(() => ImagePacker.Pack(options,
                        tokenSource.Token,
                        progress));
                }
                else
                {
                    await Task.Run(() => ImagePacker.ResumePack(tokenSource.Token,
                        progress));
                }
            }
            catch (TaskCanceledException) { }

            ButtonPackerCancel.Enabled = false;
            ProgressPacker.Value = 0;
            LabelPackerStatus.Text = "Waiting...";
        }
    }
}
