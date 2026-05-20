using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground.Rendering.v2.Computed
{
    public sealed class TestZoomableCanvasSource : IZoomableCanvasSource
    {
        public string Name => "Test Zoomable Canvas";

        public Size TilePixelSize => new(1024, 1024);

        public Size Level0TileCount => new(4, 4);

        public Task<SKImage> LoadTileAsync(FactoryOptions options)
        {
            if (options is not ZoomableCanvasFactoryOptions canvasOptions)
            {
                throw new ArgumentException(nameof(options));
            }

            var innerColor = canvasOptions.ZoomLevel switch
            {
                0 => SKColors.Red,
                1 => SKColors.Green,
                2 => SKColors.Blue,
                _ => SKColors.Yellow
            };

            var outerColor = canvasOptions.ZoomLevel switch
            {
                0 => SKColors.DarkRed,
                1 => SKColors.DarkGreen,
                2 => SKColors.DarkBlue,
                _ => SKColors.DarkGoldenrod
            };

            var outerLineWidth = 10;

            var info = new SKImageInfo(TilePixelSize.Width, TilePixelSize.Height);
            var surface = SKSurface.Create(info);
            var canvas = surface.Canvas;
            canvas.Clear(innerColor);
            SKPaint outerLinePaint = new() { Color = outerColor, StrokeWidth = outerLineWidth };
            canvas.DrawLine(new(0, 0), new(0, 1023), outerLinePaint); // Left
            canvas.DrawLine(new(0, 1023), new(1023, 1023), outerLinePaint); // Bottom
            canvas.DrawLine(new(1023, 1023), new(1023, 0), outerLinePaint); // Right
            canvas.DrawLine(new(1023, 0), new(0, 0), outerLinePaint); // Top

            var font = new SKFont(SKTypeface.FromFamilyName("Calibri"), size: 20f);
            var textPaint = new SKPaint() { Color = SKColors.White, IsAntialias = true };
            canvas.DrawText($"{canvasOptions.TileX}, {canvasOptions.TileY}", new SKPoint(20, 50), SKTextAlign.Left, font, textPaint);

            return Task.FromResult(surface.Snapshot());
        }
    }
}
