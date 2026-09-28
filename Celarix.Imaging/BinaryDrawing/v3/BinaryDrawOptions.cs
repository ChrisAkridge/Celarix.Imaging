using Celarix.Imaging.BinaryDrawing.v2;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Celarix.Imaging.BinaryDrawing.v3
{
    public sealed class BinaryDrawOptions
    {
        public required IReadOnlyList<string> FilePaths { get; init; }
        public required PixelFormat PixelFormat { get; init; }
        public required ColorMode ColorMode { get; init; }
        public required PixelLayout PixelLayout { get; init; }
        public required SizeMode SizeMode { get; init; }
        public required TitleMode TitleMode { get; init; }
        public required TargetMode TargetMode { get; init; }
        public int? FixedWidth { get; init; }
        public int? FixedHeight { get; init; }
        public string? OutputFolder { get; init; }
        
        public Rgba32[]? UserPalette { get; init; }
        public required IProgress<DrawProgress> Progress { get; init; }
        public required CancellationToken CancellationToken { get; init; }

        internal PixelFormatInfo FormatInfo => PixelFormatInfo.GetFormat(PixelFormat);
    }
}
