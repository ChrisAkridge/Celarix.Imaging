using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.Pipeline
{
    public readonly struct SelectedChannels
    {
        private readonly int flags;
        
        public ColorSpace ColorSpace { get; }
        public bool Alpha { get; }

        public RGBChannels RGB => ColorSpace != ColorSpace.RGB
                    ? throw new InvalidOperationException($"Cannot access RGB channels when the selected color space is {ColorSpace}.")
                    : (RGBChannels)flags;
        public HSLChannels HSL => ColorSpace != ColorSpace.HSL
                    ? throw new InvalidOperationException($"Cannot access HSL channels when the selected color space is {ColorSpace}.")
                    : (HSLChannels)flags;
        public HSVChannels HSV => ColorSpace != ColorSpace.HSV
                    ? throw new InvalidOperationException($"Cannot access HSV channels when the selected color space is {ColorSpace}.")
                    : (HSVChannels)flags;
        public YCbCrChannels YCbCr => ColorSpace != ColorSpace.YCbCr
                    ? throw new InvalidOperationException($"Cannot access YCbCr channels when the selected color space is {ColorSpace}.")
                    : (YCbCrChannels)flags;
        public YPbPrChannels YPbPr => ColorSpace != ColorSpace.YPbPr
                    ? throw new InvalidOperationException($"Cannot access YPbPr channels when the selected color space is {ColorSpace}.")
                    : (YPbPrChannels)flags;
        public YDbDrChannels YDbDr => ColorSpace != ColorSpace.YDbDr
                    ? throw new InvalidOperationException($"Cannot access YDbDr channels when the selected color space is {ColorSpace}.")
                    : (YDbDrChannels)flags;
        public YIQChannels YIQ => ColorSpace != ColorSpace.YIQ
                    ? throw new InvalidOperationException($"Cannot access YIQ channels when the selected color space is {ColorSpace}.")
                    : (YIQChannels)flags;
        public CMYKChannels CMYK => ColorSpace != ColorSpace.CMYK
                    ? throw new InvalidOperationException($"Cannot access CMYK channels when the selected color space is {ColorSpace}.")
                    : (CMYKChannels)flags;
        public CieLabChannels CieLab => ColorSpace != ColorSpace.CieLab
                    ? throw new InvalidOperationException($"Cannot access CieLab channels when the selected color space is {ColorSpace}.")
                    : (CieLabChannels)flags;
        public CieLchChannels CieLch => ColorSpace != ColorSpace.CieLch
                    ? throw new InvalidOperationException($"Cannot access CieLch channels when the selected color space is {ColorSpace}.")
                    : (CieLchChannels)flags;
        public CieLchuvChannels CieLchuv => ColorSpace != ColorSpace.CieLchuv
                    ? throw new InvalidOperationException($"Cannot access CieLchuv channels when the selected color space is {ColorSpace}.")
                    : (CieLchuvChannels)flags;
        public CieLuvChannels CieLuv => ColorSpace != ColorSpace.CieLuv
                    ? throw new InvalidOperationException($"Cannot access CieLuv channels when the selected color space is {ColorSpace}.")
                    : (CieLuvChannels)flags;
        public CieXyyChannels CieXyy => ColorSpace != ColorSpace.CieXyy
                    ? throw new InvalidOperationException($"Cannot access CieXyy channels when the selected color space is {ColorSpace}.")
                    : (CieXyyChannels)flags;
        public CieXyzChannels CieXyz => ColorSpace != ColorSpace.CieXyz
                    ? throw new InvalidOperationException($"Cannot access CieXyz channels when the selected color space is {ColorSpace}.")
                    : (CieXyzChannels)flags;
        public HunterLabChannels HunterLab => ColorSpace != ColorSpace.HunterLab
                    ? throw new InvalidOperationException($"Cannot access HunterLab channels when the selected color space is {ColorSpace}.")
                    : (HunterLabChannels)flags;
        public LinearRgbChannels LinearRgb => ColorSpace != ColorSpace.LinearRgb
                    ? throw new InvalidOperationException($"Cannot access LinearRgb channels when the selected color space is {ColorSpace}.")
                    : (LinearRgbChannels)flags;
        public OklabChannels Oklab => ColorSpace != ColorSpace.Oklab
                    ? throw new InvalidOperationException($"Cannot access Oklab channels when the selected color space is {ColorSpace}.")
                    : (OklabChannels)flags;
        public LMSChannels LMS => ColorSpace != ColorSpace.LMS
                    ? throw new InvalidOperationException($"Cannot access LMS channels when the selected color space is {ColorSpace}.")
                    : (LMSChannels)flags;

        public SelectedChannels(RGBChannels rgb, bool alpha = true)
        {
            flags = (int)rgb;
            ColorSpace = ColorSpace.RGB;
            Alpha = alpha;
        }

        public SelectedChannels(HSLChannels hsl, bool alpha = true)
        {
            flags = (int)hsl;
            ColorSpace = ColorSpace.HSL;
            Alpha = alpha;
        }

        public SelectedChannels(HSVChannels hsv, bool alpha = true)
        {
            flags = (int)hsv;
            ColorSpace = ColorSpace.HSV;
            Alpha = alpha;
        }

        public SelectedChannels(YCbCrChannels ycbcr, bool alpha = true)
        {
            flags = (int)ycbcr;
            ColorSpace = ColorSpace.YCbCr;
            Alpha = alpha;
        }

        public SelectedChannels(YPbPrChannels ypbpr, bool alpha = true)
        {
            flags = (int)ypbpr;
            ColorSpace = ColorSpace.YPbPr;
            Alpha = alpha;
        }

        public SelectedChannels(YDbDrChannels ydbdr, bool alpha = true)
        {
            flags = (int)ydbdr;
            ColorSpace = ColorSpace.YDbDr;
            Alpha = alpha;
        }

        public SelectedChannels(YIQChannels yiq, bool alpha = true)
        {
            flags = (int)yiq;
            ColorSpace = ColorSpace.YIQ;
            Alpha = alpha;
        }

        public SelectedChannels(CMYKChannels cmyk, bool alpha = true)
        {
            flags = (int)cmyk;
            ColorSpace = ColorSpace.CMYK;
            Alpha = alpha;
        }

        public SelectedChannels(CieLabChannels cielab, bool alpha = true)
        {
            flags = (int)cielab;
            ColorSpace = ColorSpace.CieLab;
            Alpha = alpha;
        }

        public SelectedChannels(CieLchChannels cielch, bool alpha = true)
        {
            flags = (int)cielch;
            ColorSpace = ColorSpace.CieLch;
            Alpha = alpha;
        }

        public SelectedChannels(CieLchuvChannels cielchuv, bool alpha = true)
        {
            flags = (int)cielchuv;
            ColorSpace = ColorSpace.CieLchuv;
            Alpha = alpha;
        }

        public SelectedChannels(CieLuvChannels cieluv, bool alpha = true)
        {
            flags = (int)cieluv;
            ColorSpace = ColorSpace.CieLuv;
            Alpha = alpha;
        }

        public SelectedChannels(CieXyyChannels ciexyy, bool alpha = true)
        {
            flags = (int)ciexyy;
            ColorSpace = ColorSpace.CieXyy;
            Alpha = alpha;
        }

        public SelectedChannels(CieXyzChannels ciexyz, bool alpha = true)
        {
            flags = (int)ciexyz;
            ColorSpace = ColorSpace.CieXyz;
            Alpha = alpha;
        }

        public SelectedChannels(HunterLabChannels hunterlab, bool alpha = true)
        {
            flags = (int)hunterlab;
            ColorSpace = ColorSpace.HunterLab;
            Alpha = alpha;
        }

        public SelectedChannels(LinearRgbChannels linearrgb, bool alpha = true)
        {
            flags = (int)linearrgb;
            ColorSpace = ColorSpace.LinearRgb;
            Alpha = alpha;
        }

        public SelectedChannels(OklabChannels oklab, bool alpha = true)
        {
            flags = (int)oklab;
            ColorSpace = ColorSpace.Oklab;
            Alpha = alpha;
        }

        public SelectedChannels(LMSChannels lms, bool alpha = true)
        {
            flags = (int)lms;
            ColorSpace = ColorSpace.LMS;
            Alpha = alpha;
        }
    }
}
