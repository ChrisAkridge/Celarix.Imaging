using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using DrawingSize = System.Drawing.Size;

namespace Celarix.Imaging.ImagingPlayground.Rendering.v2.Computed
{
    public abstract class PixelIndependentCanvasSource : IZoomableCanvasSource
    {
        public abstract string Name { get; }

        public DrawingSize TilePixelSize => new DrawingSize(1024, 1024);

        public abstract DrawingSize Level0TileCount { get; }

        public abstract Rgba32 GetPixelValue(int x, int y);

        public async Task<SKImage> LoadTileAsync(FactoryOptions options)
        {
            if (options is not ZoomableCanvasFactoryOptions canvasOptions)
            {
                throw new ArgumentException(nameof(options));
            }

            var tileCanvasRect = this.GetCanvasRectangleForTile(canvasOptions.ZoomLevel, canvasOptions.TileX, canvasOptions.TileY);
            var stepSize = this.GetZoomLevelMultiplier(canvasOptions.ZoomLevel);

            var tile = new Image<Rgba32>(TilePixelSize.Width, TilePixelSize.Height);

            for (int y = 0; y < TilePixelSize.Height; y++)
            {
                for (int x = 0; x < TilePixelSize.Width; x++)
                {
                    var scaledX = tileCanvasRect.Left + (x * stepSize);
                    var scaledY = tileCanvasRect.Top + (y * stepSize);
                    tile[x, y] = GetPixelValue(scaledX, scaledY);
                }

                canvasOptions.CancellationToken.ThrowIfCancellationRequested();
            }

            var skImage = await Helpers.CreateSkImageFromImageSharp(tile, canvasOptions.CancellationToken);
            tile.Dispose();
            return skImage;
        }
    }
}
