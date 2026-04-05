using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.Pipeline
{
    public enum ByteToPixelFormat
    {
        _1Bpp,
        _2Bpp,
        _3Bpp,
        _4Bpp,
        _8Bpp,
        _16Bpp,
        _24Bpp,
        _32Bpp,
        Float16,
        Float32,
        Float64
    }

    public enum BinaryDrawingColorSpace
    {
        NotApplicable,
        Grayscale,
        RGB,
        RGBA,
        UserPalette
    }

    public enum BinaryDrawingPixelLayout
    {
        Raster,
        Striped
    }

    public enum BinaryDrawingSizingMode
    {
        Default,
        FixedWidth,
        FixedSize
    }

    public enum BinaryDrawingTitleMode
    {
        NoTitles,
        SingleTitle,
        TitlePerStream
    }

    #region Color Spaces and Channels
    public enum ColorSpace
    {
        RGB,
        HSL,
        HSV,
        YCbCr,
        YPbPr,
        YDbDr,
        YIQ,
        CMYK,
        CieLab,
        CieLch,
        CieLchuv,
        CieLuv,
        CieXyy,
        CieXyz,
        HunterLab,
        LinearRgb,
        Oklab,
        LMS
    }

    [Flags]
    public enum RGBChannels
    {
        None = 0,
        Red = 1,
        Green = 2,
        Blue = 4,
    }

    [Flags]
    public enum HSLChannels
    {
        None = 0,
        Hue = 1,
        Saturation = 2,
        Lightness = 4,
    }

    [Flags]
    public enum HSVChannels
    {
        None = 0,
        Hue = 1,
        Saturation = 2,
        Value = 4,
    }

    [Flags]
    public enum YCbCrChannels
    {
        None = 0,
        Luma = 1,
        Cb = 2,
        Cr = 4,
    }

    [Flags]
    public enum YPbPrChannels
    {
        None = 0,
        Luma = 1,
        Pb = 2,
        Pr = 4,
    }

    [Flags]
    public enum YDbDrChannels
    {
        None = 0,
        Luma = 1,
        Db = 2,
        Dr = 4,
    }

    [Flags]
    public enum YIQChannels
    {
        None = 0,
        Luma = 1,
        I = 2,
        Q = 4,
    }

    [Flags]
    public enum CMYKChannels
    {
        None = 0,
        Cyan = 1,
        Magenta = 2,
        Yellow = 4,
        Key = 8,
    }

    [Flags]
    public enum CieLabChannels
    {
        None = 0,
        L = 1,
        a = 2,
        b = 4,
    }

    [Flags]
    public enum CieLchChannels
    {
        None = 0,
        L = 1,
        c = 2,
        h = 4,
    }

    [Flags]
    public enum CieLchuvChannels
    {
        None = 0,
        L = 1,
        c = 2,
        h = 4,
    }

    [Flags]
    public enum  CieLuvChannels
    {
        None = 0,
        L = 1,
        u = 2,
        v = 4
    }

    [Flags]
    public enum CieXyyChannels
    {
        None = 0,
        Yl = 1,
        x = 2,
        y = 4,
    }

    [Flags]
    public enum CieXyzChannels
    {
        None = 0,
        X = 1,
        Y = 2,
        Z = 4,
    }

    [Flags]
    public enum HunterLabChannels
    {
        None = 0,
        L = 1,
        a = 2,
        b = 4,
    }

    [Flags]
    public enum LinearRgbChannels
    {
        None = 0,
        Red = 1,
        Green = 2,
        Blue = 4,
    }

    [Flags]
    public enum OklabChannels
    {
        None = 0,
        L = 1,
        a = 2,
        b = 4,
    }

    [Flags]
    public enum LMSChannels
    {
        None = 0,
        L = 1,
        M = 2,
        S = 4,
    }
    #endregion
}
