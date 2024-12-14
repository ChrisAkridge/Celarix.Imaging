using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Celarix.Imaging.BinaryDrawing;
using Celarix.Imaging.IO;
using Celarix.Imaging.Progress;
using SixLabors.ImageSharp.PixelFormats;

namespace Celarix.Imaging.ByteViewCLI
{
    internal sealed class BinaryDrawerService
    {
        private readonly Progress<DrawingProgress> progress = new Progress<DrawingProgress>();

        public BinaryDrawerService()
        {
            progress.ProgressChanged += Progress_ProgressChanged;
        }

        private void Progress_ProgressChanged(object? sender, DrawingProgress e)
        {
            var percentage = (int)Math.Round(e.DrawnPixels * 100m / e.TotalPixels, 2);
            Console.WriteLine($"Drawing progress: {percentage}% ({e.DrawnPixels:#,###} of {e.TotalPixels:#,###} pixels).");
        }

        public void RunBinaryDraw(string inputPath, string outputPath, int bitDepth, string colorMode)
        {
            Console.WriteLine($"Drawing binary image for {inputPath}...");

            var bitDepthAndColorMode = bitDepth switch
            {
                1 => BitDepthAndColorMode.Grayscale_1BPP,
                2 => BitDepthAndColorMode.Grayscale_2BPP,
                4 => colorMode switch
                {
                    "grayscale" => BitDepthAndColorMode.Grayscale_4BPP,
                    "rgb" => BitDepthAndColorMode.RGB121_4BPP,
                    "rgba" => BitDepthAndColorMode.RGBA1111_4BPP,
                    _ => throw new ArgumentException($"Unsupported color mode {colorMode} for 4 bit-per-pixel images.")
                },
                8 => colorMode switch
                {
                    "grayscale" => BitDepthAndColorMode.Grayscale_8BPP,
                    "rgb" => BitDepthAndColorMode.RGB332_8BPP,
                    "rgba" => BitDepthAndColorMode.RGBA2222_8BPP,
                    _ => throw new ArgumentException($"Unsupported color mode {colorMode} for 8 bit-per-pixel images.")
                },
                16 => colorMode switch
                {
                    "grayscale" => BitDepthAndColorMode.RGB565_16BPP,
                    "rgb" => BitDepthAndColorMode.RGB565_16BPP,
                    "rgba" => BitDepthAndColorMode.RGBA4444_16BPP,
                    _ => throw new ArgumentException($"Unsupported color mode {colorMode} for 16 bit-per-pixel images.")
                },
                24 => colorMode switch
                {
                    "grayscale" => BitDepthAndColorMode.RGB888_24BPP,
                    "rgb" => BitDepthAndColorMode.RGB888_24BPP,
                    "rgba" => BitDepthAndColorMode.RGBA6666_24BPP,
                    _ => throw new ArgumentException($"Unsupported color mode {colorMode} for 24 bit-per-pixel images.")
                },
                32 => BitDepthAndColorMode.RGBA8888_32BPP,
                _ => throw new ArgumentException($"Unsupported bit depth {bitDepth}.")
            };

            var imagingColorMode = bitDepthAndColorMode switch
            {
                BitDepthAndColorMode.Grayscale_1BPP => ColorMode.Grayscale,
                BitDepthAndColorMode.Grayscale_2BPP => ColorMode.Grayscale,
                BitDepthAndColorMode.Grayscale_4BPP => ColorMode.Grayscale,
                BitDepthAndColorMode.RGB121_4BPP => ColorMode.Rgb,
                BitDepthAndColorMode.RGBA1111_4BPP => ColorMode.Argb,
                BitDepthAndColorMode.Grayscale_8BPP => ColorMode.Grayscale,
                BitDepthAndColorMode.RGB332_8BPP => ColorMode.Rgb,
                BitDepthAndColorMode.RGBA2222_8BPP => ColorMode.Argb,
                BitDepthAndColorMode.RGB565_16BPP => ColorMode.Rgb,
                BitDepthAndColorMode.RGBA4444_16BPP => ColorMode.Argb,
                BitDepthAndColorMode.RGB888_24BPP => ColorMode.Rgb,
                BitDepthAndColorMode.RGBA6666_24BPP => ColorMode.Argb,
                BitDepthAndColorMode.RGBA8888_32BPP => ColorMode.Argb,
                _ => throw new ArgumentException($"Unsupported bit depth and color mode {bitDepthAndColorMode}.")
            };

            Console.WriteLine($"Bit depth and color mode: {bitDepthAndColorMode}");

            var source = new FileSource([inputPath]);
            var imageSizeBytes = source.FileSizes.Sum();
            Console.WriteLine($"Image size: {imageSizeBytes:#,###} bytes");

            IReadOnlyList<Rgba32>? palette = null;
            if (bitDepth is not (24 or 32))
            {
                palette = DefaultPalettes.GetPalette(bitDepth, imagingColorMode);
            }

            var imageSharpImage = Drawer.Draw(source.GetStream(),
                bitDepth,
                palette,
                CancellationToken.None,
                progress);
            Console.WriteLine($"Resulting image is {imageSharpImage.Width}x{imageSharpImage.Height} ({imageSharpImage.Width * imageSharpImage.Height:#,###} pixels)");
            Utilities.SaveImage(outputPath, imageSharpImage);
        }
    }
}
