using System.Drawing;

namespace Celarix.Imaging.ImagingPlayground.Rendering.v2
{
    internal interface IImageCache : IDisposable
    {
        Rectangle ViewportCanvasCoordinates { get; }
        Rectangle ViewportControlCoordinates { get; }
        IReadOnlyList<ImageEntry> VisibleSet { get; }

        event EventHandler? VisibleSetChanged;

        void ViewportChanged(Rectangle newViewportCanvasCoordinates);
        void SetViewportControlCoordinates(Rectangle newViewportControlCoordinates);
        Rectangle ControlRectangleForCanvasRectangle(Rectangle canvasRect);

        void SetSoftMemoryLimit(long bytes);
        void SetHardMemoryLimit(long bytes);
    }
}
