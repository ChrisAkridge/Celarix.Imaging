using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.BinaryDrawing.v2
{
    public sealed class DrawResult
    {
        public required DrawResultKind Kind { get; init; }
        public Image<Rgba32>? SingleImage { get; init;  }
        public string? StripedImageFolderPath { get; init; }
        public Exception? Exception { get; init; }

        public static DrawResult Failure(Exception? exception = null)
        {
            return new DrawResult
            {
                Kind = DrawResultKind.Failure,
                SingleImage = null,
                StripedImageFolderPath = null,
                Exception = exception
            };
        }
    }
}
