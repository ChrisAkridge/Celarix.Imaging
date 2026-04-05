using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Celarix.Imaging.ImagingPlayground
{
    public partial class MultiFileSelectorForm : Form
    {
        private readonly List<string> filePaths = new();
        private readonly Dictionary<string, FileInfo> cachedInfos = new();

        public IReadOnlyList<string> FilePaths => filePaths;

        public MultiFileSelectorForm()
        {
            InitializeComponent();
        }

        private void UpdateButtonStates()
        {
            ButtonRemoveFile.Enabled = ListFilePaths.SelectedIndex >= 0;
            ButtonClearFiles.Enabled = ListFilePaths.Items.Count > 0;
        }

        private FileInfo GetFileInfo(string filePath)
        {
            if (!cachedInfos.TryGetValue(filePath, out var info))
            {
                info = new FileInfo(filePath);
                cachedInfos[filePath] = info;
            }
            return info;
        }

        private void UpdateFileInfo()
        {
            var infoBuilder = new StringBuilder();
            infoBuilder.AppendLine($"{FilePaths.Count:#,###} file(s)");

            var totalBytes = 0L;
            foreach (var file in FilePaths)
            {
                var fileInfo = GetFileInfo(file);
                totalBytes += fileInfo.Length;
            }
            infoBuilder.AppendLine($"{FormatBytes(totalBytes)}");

            var pixelsAt1Bpp = totalBytes * 8;
            var pixelsAt2Bpp = totalBytes * 4;
            var pixelsAt3Bpp = ((long)Math.Ceiling(totalBytes / 3f)) * 8;
            var pixelsAt4Bpp = totalBytes * 2;
            var pixelsAt8Bpp = totalBytes;
            var pixelsAt16Bpp = (long)Math.Ceiling(totalBytes / 2f);
            var pixelsAt24Bpp = (long)Math.Ceiling(totalBytes / 3f);
            var pixelsAt32Bpp = (long)Math.Ceiling(totalBytes / 4f);

            infoBuilder.AppendLine($"At 1BPP: {pixelsAt1Bpp:#,###} ({SizeForPixels(pixelsAt1Bpp)} or {FramesForPixels(pixelsAt1Bpp)})");
            infoBuilder.AppendLine($"At 2BPP: {pixelsAt2Bpp:#,###} ({SizeForPixels(pixelsAt2Bpp)} or {FramesForPixels(pixelsAt2Bpp)})");
            infoBuilder.AppendLine($"At 3BPP: {pixelsAt3Bpp:#,###} ({SizeForPixels(pixelsAt3Bpp)} or {FramesForPixels(pixelsAt3Bpp)})");
            infoBuilder.AppendLine($"At 4BPP: {pixelsAt4Bpp:#,###} ({SizeForPixels(pixelsAt4Bpp)} or {FramesForPixels(pixelsAt4Bpp)})");
            infoBuilder.AppendLine($"At 8BPP: {pixelsAt8Bpp:#,###} ({SizeForPixels(pixelsAt8Bpp)} or {FramesForPixels(pixelsAt8Bpp)})");
            infoBuilder.AppendLine($"At 16BPP: {pixelsAt16Bpp:#,###} ({SizeForPixels(pixelsAt16Bpp)} or {FramesForPixels(pixelsAt16Bpp)})");
            infoBuilder.AppendLine($"At 24BPP: {pixelsAt24Bpp:#,###} ({SizeForPixels(pixelsAt24Bpp)} or {FramesForPixels(pixelsAt24Bpp)})");
            infoBuilder.AppendLine($"At 32BPP: {pixelsAt32Bpp:#,###} ({SizeForPixels(pixelsAt32Bpp)} or {FramesForPixels(pixelsAt32Bpp)})");

            LabelFileInfo.Text = infoBuilder.ToString();
        }

        private string FormatBytes(long bytes)
        {
            if (bytes >= 1_000_000_000)
            {
                return $"{(float)bytes / 1_000_000_000.0:F2} gigabytes";
            }

            if (bytes >= 1_000_000)
            {
                return $"{(float)bytes / 1_000_000.0:F2} megabytes";
            }

            if (bytes >= 1_000)
            {
                return $"{(float)bytes / 1_000.0:F2} kilobytes";
            }

            return $"{bytes} bytes";
        }

        private string SizeForPixels(long pixels)
        {
            var width = (int)Math.Floor(Math.Sqrt((double)pixels));
            var height = (int)Math.Ceiling((double)pixels / width);
            return $"{width}x{height}";
        }

        private string FramesForPixels(long pixels)
        {
            var frames = (int)Math.Ceiling((double)pixels / 1920 / 1080);
            return $"{frames} 1080p frame(s)";
        }

        private void ButtonAddFiles_Click(object sender, EventArgs e)
        {
            if (OFDAddFiles.ShowDialog() == DialogResult.OK)
            {
                foreach (var file in OFDAddFiles.FileNames)
                {
                    if (!filePaths.Contains(file))
                    {
                        filePaths.Add(file);
                        ListFilePaths.Items.Add(file);
                    }
                }
                UpdateButtonStates();
                UpdateFileInfo();
            }
        }

        private void ButtonAddFolderTopLevel_Click(object sender, EventArgs e)
        {
            if (FBDAddFolder.ShowDialog() == DialogResult.OK)
            {
                var folderPath = FBDAddFolder.SelectedPath;
                var files = Directory.GetFiles(folderPath, "*", SearchOption.TopDirectoryOnly);
                foreach (var file in files)
                {
                    if (!filePaths.Contains(file))
                    {
                        filePaths.Add(file);
                        ListFilePaths.Items.Add(file);
                    }
                }
                UpdateButtonStates();
                UpdateFileInfo();
            }
        }

        private void AddFilesFromFolderRecursive_Click(object sender, EventArgs e)
        {
            if (FBDAddFolder.ShowDialog() == DialogResult.OK)
            {
                var folderPath = FBDAddFolder.SelectedPath;
                var files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    if (!filePaths.Contains(file))
                    {
                        filePaths.Add(file);
                        ListFilePaths.Items.Add(file);
                    }
                }
                UpdateButtonStates();
                UpdateFileInfo();
            }
        }

        private void ButtonRemoveFile_Click(object sender, EventArgs e)
        {
            var selectedIndex = ListFilePaths.SelectedIndex;
            if (selectedIndex >= 0)
            {
                var filePath = ListFilePaths.Items[selectedIndex].ToString();
                filePaths.Remove(filePath!);
                ListFilePaths.Items.RemoveAt(selectedIndex);
                UpdateButtonStates();
                UpdateFileInfo();

                // Select the previous item if possible, otherwise the next item, otherwise nothing
                if (selectedIndex - 1 >= 0)
                {
                    ListFilePaths.SelectedIndex = selectedIndex - 1;
                }
                else if (selectedIndex < ListFilePaths.Items.Count)
                {
                    ListFilePaths.SelectedIndex = selectedIndex;
                }
            }
        }

        private void ButtonClearFiles_Click(object sender, EventArgs e)
        {
            filePaths.Clear();
            ListFilePaths.Items.Clear();
            UpdateButtonStates();
            UpdateFileInfo();
        }

        private void ListFilePaths_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
