using System;
using System.IO;
using SixLabors.ImageSharp.PixelFormats;

namespace Celarix.Imaging.BinaryDrawing.V2
{
    public interface IStreamProvider
    {
        Stream GetStream();
        bool DisposeOnAdvance { get; }
        long Length { get; }
        string Name { get; }
    }

    public interface ICanvas : IDisposable
    {
        void SetPixelsLine(int x, int y, ReadOnlySpan<Rgba32> pixels, IProgress<DrawingProgress> progress = null);
        void DrawStreamHeaderLine(int y, string streamName, long size, double streamProgress, IProgress<DrawingProgress> processingProgress = null);
        void SaveAndDispose();
    }
}
