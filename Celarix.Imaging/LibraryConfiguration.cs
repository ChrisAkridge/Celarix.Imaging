using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging
{
    public sealed class LibraryConfiguration
    {
        public static LibraryConfiguration Instance { get; set; }

        /// <summary>
        /// Gets or sets a value indicating how many pixels are drawn by types in
        /// BinaryDrawing before it invokes a progress callback.
        /// </summary>
        public int BinaryDrawingReportsProgressEveryNPixels { get; set; } = 1048576;

        /// <summary>
        /// Gets or sets a value indicating how many pixels wide and tall each
        /// tile of a zoomable canvas is.
        /// </summary>
        public int ZoomableCanvasTileEdgeLength { get; set; } = 256;
    }
}
