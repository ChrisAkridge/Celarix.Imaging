using SkiaSharp;
using System.Drawing;

namespace Celarix.Imaging.ImagingPlayground.Rendering.v2
{
    public interface IZoomableCanvasSource
    {
        Size TilePixelSize { get; }
        Size Level0TileCount { get; }

        Task<SKImage> LoadTileAsync(FactoryOptions options);
    }

    public static class ZoomableCanvasSourceExtensions
    {
        public static Size GetTotalLevel0PixelSize(this IZoomableCanvasSource source)
        {
            ArgumentNullException.ThrowIfNull(source);

            var width = checked((long)source.TilePixelSize.Width * source.Level0TileCount.Width);
            var height = checked((long)source.TilePixelSize.Height * source.Level0TileCount.Height);
            return new Size(checked((int)width), checked((int)height));
        }

        public static int GetMaxZoomLevel(this IZoomableCanvasSource source)
        {
            ArgumentNullException.ThrowIfNull(source);

            var longestAxisInTiles = Math.Max(source.Level0TileCount.Width, source.Level0TileCount.Height);
            var maxZoomLevel = 0;
            long levelCoverageInLevel0Tiles = 1;
            while (levelCoverageInLevel0Tiles < longestAxisInTiles)
            {
                levelCoverageInLevel0Tiles <<= 1;
                maxZoomLevel += 1;
            }

            return maxZoomLevel;
        }

        public static Size GetTileCountForZoomLevel(this IZoomableCanvasSource source, int zoomLevel)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentOutOfRangeException.ThrowIfNegative(zoomLevel);

            var scale = 1L << zoomLevel;
            var tilesWide = (int)((source.Level0TileCount.Width + scale - 1) / scale);
            var tilesHigh = (int)((source.Level0TileCount.Height + scale - 1) / scale);
            return new Size(tilesWide, tilesHigh);
        }

        public static bool IsInBounds(this IZoomableCanvasSource source, int zoomLevel, int tileX, int tileY)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentOutOfRangeException.ThrowIfNegative(zoomLevel);

            var tileCount = source.GetTileCountForZoomLevel(zoomLevel);
            return tileX >= 0
                && tileY >= 0
                && tileX < tileCount.Width
                && tileY < tileCount.Height;
        }

        public static Rectangle GetCanvasRectangleForTile(this IZoomableCanvasSource source,
            int zoomLevel,
            int tileX,
            int tileY)
        {
            ArgumentNullException.ThrowIfNull(source);

            if (!source.IsInBounds(zoomLevel, tileX, tileY))
            {
                throw new ArgumentOutOfRangeException($"Tile ({tileX}, {tileY}) at zoom level {zoomLevel} is out of bounds.");
            }

            var levelScale = 1L << zoomLevel;
            var canvasX = checked((long)tileX * source.TilePixelSize.Width * levelScale);
            var canvasY = checked((long)tileY * source.TilePixelSize.Height * levelScale);

            var maxWidth = source.GetTotalLevel0PixelSize().Width - canvasX;
            var maxHeight = source.GetTotalLevel0PixelSize().Height - canvasY;
            var width = Math.Min((long)source.TilePixelSize.Width * levelScale, maxWidth);
            var height = Math.Min((long)source.TilePixelSize.Height * levelScale, maxHeight);
            return new Rectangle(checked((int)canvasX),
                checked((int)canvasY),
                checked((int)width),
                checked((int)height));
        }
    }
}
