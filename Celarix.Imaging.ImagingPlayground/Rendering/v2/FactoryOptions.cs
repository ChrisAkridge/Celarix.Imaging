using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground.Rendering.v2
{
    public class FactoryOptions
    {
        public required CancellationToken CancellationToken { get; init; }
        public required LoadedImageKind Kind { get; init; }

        public int? StripeIndex { get; init; }

        public int? ZoomLevel { get; init; }
        public int? TileX { get; init; }
        public int? TileY { get; init; }
        public int? TileEdgeLength { get; init; }

        public SKPoint? TopLeftCanvasPoint
        {
            get
            {
                if (!ZoomLevel.HasValue || !TileX.HasValue || !TileY.HasValue || !TileEdgeLength.HasValue)
                {
                    return null;
                }

                var topLeftX = TileX.Value * TileEdgeLength.Value;
                var topLeftY = TileY.Value * TileEdgeLength.Value;
                topLeftX <<= ZoomLevel.Value;
                topLeftY <<= ZoomLevel.Value;
                return new SKPoint(topLeftX, topLeftY);
            }
        }
    }
}
