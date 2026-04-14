using Celarix.Imaging.Utilities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground.Rendering
{
    internal sealed class CanvasImage
    {
        public Func<CancellationToken, Task<SKImage>> Factory { get; private set; }

        public SKPoint Position { get; private set; }
        public SKSize Size { get; private set; }
        public int? OnlyAtZoomLevel { get; private set; }

        // Helpers
        public SKPoint TopLeft => Position;
        public SKPoint TopRight => new SKPoint(Position.X + Size.Width, Position.Y);
        public SKPoint BottomLeft => new SKPoint(Position.X, Position.Y + Size.Height);
        public SKPoint BottomRight => new SKPoint(Position.X + Size.Width, Position.Y + Size.Height);

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        private CanvasImage()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        {
            // For factory methods
        }

        public CanvasImage(Func<CancellationToken, Task<SKImage>> factory, SKPoint position, int? onlyAtZoomLevel = null)
        {
            Factory = factory;
            Position = position;
            OnlyAtZoomLevel = onlyAtZoomLevel;
        }

        public static CanvasImage FromFile(string filePath, SKPoint position, int? onlyAtZoomLevel = null)
        {
            var canvasImage = new CanvasImage();
            canvasImage.Factory = cancellationToken => LoadFromFile(canvasImage, filePath, cancellationToken);
            canvasImage.Position = position;
            canvasImage.OnlyAtZoomLevel = onlyAtZoomLevel;
            return canvasImage;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Position.X, Position.Y, Size.Width, Size.Height, OnlyAtZoomLevel ?? 0);
        }

        private static async Task<SKImage> LoadFromFile(CanvasImage image, string filePath, CancellationToken cancellationToken)
        {
            var imageSharpImage = await ImageLoader.LoadImage(filePath, cancellationToken);
            if (imageSharpImage.Result != ImageLoadAttemptResult.Success || imageSharpImage.LoadedImage == null)
            {
                // TODO: don't throw, instead log something and load an error image to show instead
                throw new InvalidOperationException($"Failed to load image from {filePath}. Result: {imageSharpImage.Result}, Exception: {imageSharpImage.Exception}");
            }
            image.Size = new SKSize(imageSharpImage.LoadedImage.Width, imageSharpImage.LoadedImage.Height);
            return await CreateSkImageFromImageSharp(imageSharpImage.LoadedImage, cancellationToken);
        }

        private static async Task<SKImage> CreateSkImageFromImageSharp(Image<Rgba32> imageSharpImage, CancellationToken cancellationToken)
        {
            // From https://stackoverflow.com/a/79086112/2709212
            var buffer = new byte[imageSharpImage.Width * imageSharpImage.Height * 4];
            int pixel = 0;
            imageSharpImage.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    var row = accessor.GetRowSpan(y);
                    for (int x = 0; x < row.Length; x++)
                    {
                        var pixelData = row[x];
                        buffer[pixel++] = pixelData.R;
                        buffer[pixel++] = pixelData.G;
                        buffer[pixel++] = pixelData.B;
                        buffer[pixel++] = pixelData.A;
                    }

                    if (cancellationToken.IsCancellationRequested)
                    {
                        // No need to throw anything, just mark the task cancelled...
                        return;
                    }
                }
            });

            // ...and throw here, saving us from having to load the rest of the rows.
            cancellationToken.ThrowIfCancellationRequested();

            var image = SKImage.FromPixelCopy(new SKImageInfo(imageSharpImage.Width, imageSharpImage.Height, SKColorType.Rgba8888), buffer, imageSharpImage.Width * 4);
            return image;
        }
    }
}
