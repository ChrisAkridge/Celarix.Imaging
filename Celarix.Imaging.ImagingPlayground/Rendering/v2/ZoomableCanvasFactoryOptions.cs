using Celarix.Imaging.ImagingPlayground.Rendering.v2;
using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground.Rendering.v2
{
    public sealed class ZoomableCanvasFactoryOptions : FactoryOptions
    {
        public override LoadedImageKind Kind => LoadedImageKind.ZoomableCanvas;

        public int ZoomLevel { get; }
        public int TileX { get; }
        public int TileY { get; }
        
        public ZoomableCanvasFactoryOptions(CancellationToken cancellationToken, int zoomLevel, int tileX, int tileY) : base(cancellationToken)
        {
            ZoomLevel = zoomLevel;
            TileX = tileX;
            TileY = tileY;
        }

        public override string ToString()
        {
            return $"ZoomableCanvasFactoryOptions [ZoomLevel={ZoomLevel},TileX={TileX},TileY={TileY}]";
        }
    }
}
