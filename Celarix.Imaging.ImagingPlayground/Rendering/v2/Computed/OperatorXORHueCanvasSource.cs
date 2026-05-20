using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces.Conversion;
using SixLabors.ImageSharp.PixelFormats;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using DrawingSize = System.Drawing.Size;

namespace Celarix.Imaging.ImagingPlayground.Rendering.v2.Computed
{
    public sealed class OperatorXORHueCanvasSource : IZoomableCanvasSource
    {
        private static readonly Rgba32[] hueWheel = new Rgba32[4096];

        public string Name => "Operators: Bitwise XOR (12-bit hue wheel)";

        public DrawingSize TilePixelSize => new DrawingSize(1024, 1024);

        public DrawingSize Level0TileCount => new DrawingSize(4, 4);

        public OperatorXORHueCanvasSource()
        {
            for (var i = 0; i < 4096; i++)
            {
                hueWheel[i] = NumberToHue(i);
            }
        }

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
                    var number = scaledX ^ scaledY;
                    tile[x, y] = hueWheel[number];
                }

                canvasOptions.CancellationToken.ThrowIfCancellationRequested();
            }

            var skImage = await Helpers.CreateSkImageFromImageSharp(tile, canvasOptions.CancellationToken);
            tile.Dispose();
            return skImage;
        }

        private static Rgba32 NumberToHue(int number)
        {
            if (number < 0 || number > 4095)
            {
                throw new ArgumentOutOfRangeException(nameof(number));
            }

            var hue = number * 360f / 4096f;
            var hsv = new SixLabors.ImageSharp.ColorSpaces.Hsv(hue, 1f, 1f);
            return ColorSpaceConverter.ToRgb(hsv);
        }
    }
}
