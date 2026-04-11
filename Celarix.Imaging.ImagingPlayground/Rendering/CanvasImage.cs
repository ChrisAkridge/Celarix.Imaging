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
        public Func<Image<Rgba32>> Factory { get; }

        public SKPoint Position { get; }
        public SKSize Size { get; }
        public int? OnlyAtZoomLevel { get; }

        internal Image<Rgba32>? Image { get; set; }

        // Helpers
        public SKPoint TopLeft => Position;
        public SKPoint TopRight => new SKPoint(Position.X + Size.Width, Position.Y);
        public SKPoint BottomLeft => new SKPoint(Position.X, Position.Y + Size.Height);
        public SKPoint BottomRight => new SKPoint(Position.X + Size.Width, Position.Y + Size.Height);

        public CanvasImage(Func<Image<Rgba32>> factory, SKPoint position, int? onlyAtZoomLevel = null)
        {
            Factory = factory;
            Position = position;
            OnlyAtZoomLevel = onlyAtZoomLevel;
        }

        public CanvasImage(string filePath, SKPoint position, int? onlyAtZoomLevel = null)
            : this(() => LoadFromFile(filePath), position, onlyAtZoomLevel)
        {
        }

        private static Image<Rgba32> LoadFromFile(string filePath)
        {
            var result = ImageLoader.LoadImage(filePath);
            if (result.Result != ImageLoadAttemptResult.Success || result.LoadedImage == null)
            {
                throw new InvalidOperationException($"Failed to load image from {filePath}. Result: {result.Result}, Exception: {result.Exception}");
            }
            return result.LoadedImage;
        }
    }
}
