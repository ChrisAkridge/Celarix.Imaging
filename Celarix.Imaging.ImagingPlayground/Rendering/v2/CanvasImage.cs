using Celarix.Imaging.Packing;
using Celarix.Imaging.Utilities;
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

        public SKPoint TopLeftCanvasPoint => new(CanvasX, CanvasY);
        public SKPoint TopRightCanvasPoint => new(CanvasX + Width, CanvasY);
        public SKPoint BottomLeftCanvasPoint => new(CanvasX, CanvasY + Height);
        public SKPoint BottomRightCanvasPoint => new(CanvasX + Width, CanvasY + Height);

        public static CanvasImage FromFile(string filePath,
            int canvasX,
            int canvasY,
            int? onlyAtZoomLevel = null)
        {
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

            return new CanvasImage
            {
                Factory = options => CreateSkImageFromImageSharp(imageSharpImage, options.CancellationToken),
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
            return await CreateSkImageFromImageSharp(image, options.CancellationToken);
        }

        private static Task<SKImage> CreateSkImageFromImageSharp(Image<Rgba32> imageSharpImage,
            CancellationToken cancellationToken)
        {
            var buffer = new byte[imageSharpImage.Width * imageSharpImage.Height * 4];
            var pixel = 0;

            imageSharpImage.ProcessPixelRows(accessor =>
            {
                for (var y = 0; y < accessor.Height; y++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var row = accessor.GetRowSpan(y);
                    for (var x = 0; x < row.Length; x++)
                    {
                        var pixelData = row[x];
                        buffer[pixel++] = pixelData.R;
                        buffer[pixel++] = pixelData.G;
                        buffer[pixel++] = pixelData.B;
                        buffer[pixel++] = pixelData.A;
                    }
                }
            });

            var image = SKImage.FromPixelCopy(
                new SKImageInfo(imageSharpImage.Width, imageSharpImage.Height, SKColorType.Rgba8888),
                buffer,
                imageSharpImage.Width * 4);
            return Task.FromResult(image);
        }
    }
}
