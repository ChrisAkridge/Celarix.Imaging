using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground
{
    internal enum ImageEntryLoadState
    {
        Unloaded,
        Loading,
        Loaded
    }

    public enum LoadedImageKind
    {
        SingleImage,
        Striped,
        ZoomableCanvas
    }
}
