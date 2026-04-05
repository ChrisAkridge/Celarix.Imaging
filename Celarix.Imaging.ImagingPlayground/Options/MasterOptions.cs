using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground.Options
{
    internal sealed class MasterOptions
    {
        [TypeConverter(typeof(FileListConverter))]
        [Editor(typeof(MultiFileEditor), typeof(UITypeEditor))]
        [Category("General")]
        public Models.FileList? Files { get; set; }

        [Category("Binary Drawing")]
        public PixelFormat PixelFormat { get; set; } = PixelFormat.Binary8Bpp;

        [Category("Binary Drawing")]
        public ColorMode ColorMode { get; set; } = ColorMode.Grayscale;

        [Category("Binary Drawing")]
        public PixelLayout PixelLayout { get; set; } = PixelLayout.Raster;

        [Category("Binary Drawing")]
        public SizeMode SizeMode { get; set; } = SizeMode.Automatic;

        [Category("Binary Drawing")]
        public int FixedWidth { get; set; } = 0;

        [Category("Binary Drawing")]
        public int FixedHeight { get; set; } = 0;

        [Category("Binary Drawing")]
        public TitleMode TitleMode { get; set; } = TitleMode.None;

        [Category("Binary Drawing")]
        public TargetMode TargetMode { get; set; } = TargetMode.SingleImage;

        [Category("Binary Drawing")]
        public string OutputFolder { get; set; } = string.Empty;
    }
}
