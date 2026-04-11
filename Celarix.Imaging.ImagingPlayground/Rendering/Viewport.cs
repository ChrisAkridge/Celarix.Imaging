using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground.Rendering
{
    internal sealed class Viewport
    {
        public SKPoint Location { get; }
        public SKSize Size { get; }
        public int ZoomableCanvasTileSize { get; } = LibraryConfiguration.Instance.ZoomableCanvasTileEdgeLength;

        public int ZoomLevel
        {
            get
            {
                var width = Size.Width;
                var widthInTiles = width / ZoomableCanvasTileSize;
                var zoomLevel = widthInTiles <= 0 ? 0 :
                    (int)Math.Ceiling(Math.Log2(widthInTiles));
                return zoomLevel;
            }
        }

        public Viewport(SKPoint location, SKSize size, int? zoomableCanvasTileSize = null)
        {
            Location = location;
            Size = size;
            if (zoomableCanvasTileSize != null)
            {
                ZoomableCanvasTileSize = zoomableCanvasTileSize.Value;
            }
        }

        public bool Intersects(CanvasImage image)
        {
            var imagePosition = image.Position;
            var imageSize = image.Size;

            if (image.OnlyAtZoomLevel != null)
            {
                if (image.OnlyAtZoomLevel != ZoomLevel)
                {
                    return false;
                }
                var scaleFactor = Math.Pow(2, ZoomLevel);
                imagePosition = Scale(image.Position, scaleFactor);
                imageSize = Scale(image.Size, scaleFactor);
            }

            var viewportRect = new SKRect(Location.X, Location.Y, Location.X + Size.Width, Location.Y + Size.Height);
            var imageRect = new SKRect(imagePosition.X, imagePosition.Y, imagePosition.X + imageSize.Width, imagePosition.Y + imageSize.Height);
            return viewportRect.IntersectsWith(imageRect);
        }

        private static SKPoint Scale(SKPoint point, double scaleFactor)
        {
            return new SKPoint(point.X * (float)scaleFactor, point.Y * (float)scaleFactor);
        }

        private static SKSize Scale(SKSize size, double scaleFactor)
        {
            return new SKSize(size.Width * (float)scaleFactor, size.Height * (float)scaleFactor);
        }
    }
}
