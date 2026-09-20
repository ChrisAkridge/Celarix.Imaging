using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground.Operations
{
    internal delegate void LoggingDelegate(string message);
    internal delegate void SetImageDelegate(Image<Rgba32> image);
}
