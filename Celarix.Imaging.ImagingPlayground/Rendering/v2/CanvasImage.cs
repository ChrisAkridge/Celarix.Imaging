using Celarix.Imaging.Packing;
using Celarix.Imaging.Utilities;
using Serilog;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SkiaSharp;

namespace Celarix.Imaging.ImagingPlayground.Rendering.v2
{
    internal sealed class CanvasImage
    {
        public required Func<FactoryOptions, Task<SKImage>> Factory { get; init; }
        public required Func<long> SizeEstimator { get; init; }

        public required int CanvasX { get; init; }
        public required int CanvasY { get; init; }
        public required int Width { get; init; }
        public required int Height { get; init; }
        public int? OnlyAtZoomLevel { get; init; }

        public System.Drawing.Point TopLeftCanvasPoint => new(CanvasX, CanvasY);
        public System.Drawing.Point TopRightCanvasPoint => new(CanvasX + Width, CanvasY);
        public System.Drawing.Point BottomLeftCanvasPoint => new(CanvasX, CanvasY + Height);
        public System.Drawing.Point BottomRightCanvasPoint => new(CanvasX + Width, CanvasY + Height);
        public System.Drawing.Rectangle CanvasRect => new(CanvasX, CanvasY, Width, Height);

        public static CanvasImage FromFile(string filePath,
            int canvasX,
            int canvasY,
            int? onlyAtZoomLevel = null)
        {
            Log.Debug("Creating CanvasImage from file: {FilePath} at canvas position ({CanvasX}, {CanvasY}) at zoom level {OnlyAtZoomLevel}",
                filePath, canvasX, canvasY, onlyAtZoomLevel);
            var size = TryGetKnownImageSize(filePath);

            return new CanvasImage
            {
                Factory = options => LoadFromFile(filePath, options),
                SizeEstimator = () => EstimateSizeFromFile(filePath, size),
                CanvasX = canvasX,
                CanvasY = canvasY,
                Width = size.Width,
                Height = size.Height,
                OnlyAtZoomLevel = onlyAtZoomLevel
            };
        }

        public static CanvasImage FromImageSharpImage(Image<Rgba32> imageSharpImage,
            int canvasX,
            int canvasY,
            int? onlyAtZoomLevel = null)
        {
            ArgumentNullException.ThrowIfNull(imageSharpImage);

            Log.Debug("Creating CanvasImage from ImageSharp image at canvas position ({CanvasX}, {CanvasY}) at zoom level {OnlyAtZoomLevel}",
                canvasX, canvasY, onlyAtZoomLevel);
            return new CanvasImage
            {
                Factory = options => Helpers.CreateSkImageFromImageSharp(imageSharpImage, options.CancellationToken),
                SizeEstimator = () => (long)imageSharpImage.Width * imageSharpImage.Height * 4,
                CanvasX = canvasX,
                CanvasY = canvasY,
                Width = imageSharpImage.Width,
                Height = imageSharpImage.Height,
                OnlyAtZoomLevel = onlyAtZoomLevel
            };
        }

        private static SixLabors.ImageSharp.Size TryGetKnownImageSize(string filePath)
        {
            if (ImageSizeLoader.TryGetSize(filePath, out var size) && size.Width > 0 && size.Height > 0)
            {
                return size;
            }

            var info = SixLabors.ImageSharp.Image.Identify(filePath);
            if (info != null)
            {
                return new SixLabors.ImageSharp.Size(info.Width, info.Height);
            }

            return new SixLabors.ImageSharp.Size(1000, 1000);
        }

        private static long EstimateSizeFromFile(string filePath, SixLabors.ImageSharp.Size fallbackSize)
        {
            if (ImageSizeLoader.TryGetSize(filePath, out var size) && size.Width > 0 && size.Height > 0)
            {
                return (long)size.Width * size.Height * 4;
            }

            return (long)fallbackSize.Width * fallbackSize.Height * 4;
        }

        private static async Task<SKImage> LoadFromFile(string filePath, FactoryOptions options)
        {
            var imageLoadResult = await ImageLoader.LoadImage(filePath, options.CancellationToken);
            if (imageLoadResult.Result != ImageLoadAttemptResult.Success || imageLoadResult.LoadedImage == null)
            {
                throw new InvalidOperationException(
                    $"Failed to load image from {filePath}. Result: {imageLoadResult.Result}, Exception: {imageLoadResult.Exception}");
            }

            using var image = imageLoadResult.LoadedImage;
            return await Helpers.CreateSkImageFromImageSharp(image, options.CancellationToken);
        }
    }
}
