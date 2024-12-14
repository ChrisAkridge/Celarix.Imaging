using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.ByteViewCLI
{
    internal enum RunOption
    {
        Invalid,
        BinaryDraw,
        BinaryDrawMultipleFiles,
        BinaryDrawFrames,
        BinaryDrawFramesMultipleFiles,
        BinaryDrawCanvas,
        BinaryDrawCanvasMultipleFiles,
        BinaryDrawFixedSize,
        ExtractColorChannel,
        ExtractBitPlane,
        ChromaSubsample,
        ReduceBitDepth,
        ExportMockNTSCSignal,
        ExportSimpleSignal,
        UniqueColors,
        Sort,
        UniqueColorsInSpace
    }

    internal enum BitDepthAndColorMode
    {
        Grayscale_1BPP,
        Grayscale_2BPP,
        Grayscale_4BPP,
        RGB121_4BPP,
        RGBA1111_4BPP,
        Grayscale_8BPP,
        RGB332_8BPP,
        RGBA2222_8BPP,
        RGB565_16BPP,
        RGBA4444_16BPP,
        RGB888_24BPP,
        RGBA6666_24BPP,
        RGBA8888_32BPP
    }
}
