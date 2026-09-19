using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Celarix.Imaging.BinaryDrawing.V2
{
    public struct ZoomableCanvasProgress
    {
        public int ZoomLevel { get; set; }
        public int MaxZoomLevel { get; set; }
        public int TileX { get; set; }
        public int TileY { get; set; }
        public int TileCountX { get; set; }
        public int TileCountY { get; set; }
    }

    public struct DrawingProgress
    {
        public long BytesProcessed { get; set; }
        public long TotalBytes { get; set; }
        public string CurrentStreamName { get; set; }
        public long CurrentStreamBytesProcessed { get; set; }
        public long CurrentStreamTotalBytes { get; set; }
        public int? FixedSizeImagesDrawn { get; set; }
        public ZoomableCanvasProgress? ZoomableCanvasProgress { get; set; }
    }

    public class BinaryDrawingOptions
    {
        public PixelFormat PixelFormat { get; set; }
        public OutputType OutputType { get; set; }
        public StreamNamePrinting StreamNamePrinting { get; set; }
        public StreamNameLevel StreamNameLevel { get; set; }
        public PixelOrder PixelOrder { get; set; }
        public Palette Palette { get; set; }
        public int? FixedWidth { get; set; }
        public Size? FixedSize { get; set; }
    }


}
