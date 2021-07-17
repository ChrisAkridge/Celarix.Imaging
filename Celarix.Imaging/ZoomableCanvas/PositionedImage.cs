using SixLabors.ImageSharp;

namespace Celarix.Imaging.ZoomableCanvas
{
    public sealed class PositionedImage
	{
        public string ImageFilePath { get; set; }
        public Point Position { get; set; }
        public Size Size { get; set; }
	}
}
