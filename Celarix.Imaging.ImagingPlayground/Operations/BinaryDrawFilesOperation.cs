using Celarix.Imaging.BinaryDrawing.v2;
using Celarix.Imaging.IO.v2;
using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground.Operations
{
    internal sealed class BinaryDrawFilesOperation : IOperation
    {
        public string Name => "Binary Draw File(s)";

        public async Task RunAsync(OperationRunOptions options)
        {
            if (options.MasterOptions.Files is null || options.MasterOptions.Files.FilePaths.Count == 0)
            {
                MessageBox.Show($"No files selected. Please select at least one file to draw.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var masterOptions = options.MasterOptions;
            var byteStream = new NamedByteStream(masterOptions.Files.FilePaths);
            var drawOptions = new DrawOptions
            {
                ByteStream = byteStream,
                PixelFormat = masterOptions.PixelFormat,
                ColorMode = masterOptions.ColorMode,
                PixelLayout = masterOptions.PixelLayout,
                SizeMode = masterOptions.SizeMode,
                TitleMode = masterOptions.TitleMode,
                TargetMode = masterOptions.TargetMode,
                BandDirection = BandDirection.Horizontal,
                FixedWidth = masterOptions.FixedWidth,
                FixedHeight = masterOptions.FixedHeight,
                TextMapEncoding = TextMapEncoding.UTF8,
                UseTextMapHueBackground = false,
                Progress = new Progress<DrawProgress>(progress =>
                {
                    // intentionally left blank
                }),
                CancellationToken = options.CancellationToken
            };

            var result = await Drawer.DrawAsync(drawOptions);
            if (result.Kind == DrawResultKind.SingleImageInMemory)
            {
                var image = result.SingleImage;
                if (image is not null)
                {
                    options.SetImage(image);
                }
            }
        }
    }
}
