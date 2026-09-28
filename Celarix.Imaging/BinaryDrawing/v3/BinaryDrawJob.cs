using Celarix.Imaging.BinaryDrawing.v2;
using Celarix.Imaging.BinaryDrawing.v3.PixelSources;
using Celarix.Imaging.ZoomableCanvas;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.BinaryDrawing.v3
{
    public sealed class BinaryDrawJob
    {
        internal const int TitleBarHeight = 24;
        internal const int ZoomableCanvasMargin = 2;
        internal const string TitleBarFont = "Consolas";
        internal const float TitleBarFontSize = 12f;

        private bool _isLargeJob;
        private List<NamedPixelSource> _pixelSources;
        private Size? _canvasSize;
        private Font _titleBarFont;
        private TextOptions _titleBarFontOptions;
        private ZoomableCanvasSource[]? _canvasSources;

        public BinaryDrawOptions Options { get; }
        public TargetMode EffectiveTargetMode => _isLargeJob ? TargetMode.ZoomableCanvas : Options.TargetMode;

        internal BinaryDrawJob(BinaryDrawOptions options)
        {
            Options = options;

            _titleBarFont = SystemFonts.CreateFont(TitleBarFont, TitleBarFontSize);
            _titleBarFontOptions = new TextOptions(_titleBarFont)
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };
        }

        public async Task Draw()
        {
            OptionsValidOrThrow();

            // Pick the palette based on the options
            Rgba32[] palette = Options.ColorMode == ColorMode.Paletted
                ? Options.UserPalette!
                : DefaultPaletteProvider.GetPalette(Options.PixelFormat, Options.ColorMode);

            // Size the canvas and determine if this is a large job
            var filePixelCounts = new List<(string FilePath, long PixelCount)>();
            var formatInfo = Options.FormatInfo;
            foreach (var file in Options.FilePaths)
            {
                var info = new FileInfo(file);
                var length = info.Length;
                var groups = (int)Math.Ceiling((double)length / formatInfo.SourceBytesPerGroup);
                var pixels = (long)groups * formatInfo.OutputPixelsPerGroup;
                filePixelCounts.Add((file, pixels));
            }
            var totalPixels = filePixelCounts.Sum(c => c.PixelCount);

            bool fixedWidth = Options.FixedWidth.HasValue;
            bool fixedHeight = Options.FixedHeight.HasValue;

            if (Options.TargetMode == TargetMode.SingleImage)
            {
                if (!fixedWidth && !fixedHeight)
                {
                    SizeSingleImageAutomatic(filePixelCounts);
                }
                else if (fixedWidth && !fixedHeight)
                {
                    SizeSingleImageFixedWidth(filePixelCounts);
                }
                else if (fixedWidth && fixedHeight)
                {
                    _canvasSize = new Size(Options.FixedWidth!.Value, Options.FixedHeight!.Value);
                }
                else
                {
                    throw new ArgumentException("Invalid fixed dimension configuration.");
                }
            }
            else if (Options.TargetMode == TargetMode.MultipleToFolder)
            {
                _canvasSize = new(Options.FixedWidth!.Value, Options.FixedHeight!.Value);
            }
            else if (Options.TargetMode == TargetMode.ZoomableCanvas)
            {
                if (Options.TitleMode is TitleMode.None or TitleMode.OnePerImage)
                {
                    // Same thing as single image, just probably really big
                    if (!fixedWidth)
                    {
                        SizeSingleImageAutomatic(filePixelCounts);
                    }
                    else
                    {
                        SizeSingleImageFixedWidth(filePixelCounts);
                    }
                }
                else if (Options.TitleMode == TitleMode.OnePerFile)
                {
                    List<(string FilePath, Size Size)> sourceSizes = [];

                    foreach (var filePixelCount in filePixelCounts)
                    {
                        var pixelBlockSize = AutomaticSize(filePixelCount.PixelCount, Options.PixelLayout, formatInfo.StripeWidth);
                        var titleSize = MeasureText(filePixelCount.FilePath);
                        titleSize = new Size(
                            pixelBlockSize.Width > titleSize.Width ? pixelBlockSize.Width : titleSize.Width,
                            TitleBarHeight);
                        var sourceWidth = titleSize.Width + (ZoomableCanvasMargin * 2);
                        var sourceHeight = pixelBlockSize.Height + titleSize.Height + (ZoomableCanvasMargin * 2);
                        sourceSizes.Add((filePixelCount.FilePath, new Size(sourceWidth, sourceHeight)));
                    }

                    // Use our own Packer to arrange the sources within the canvas
                    var blocks = sourceSizes.Select(s => new Packing.Block<string>() { Size = s.Size, Source = s.FilePath }).ToList();
                    var packer = new Packing.Packer<string>();
                    packer.Fit(blocks, new NullProgress<string>());

                    var locations = blocks
                        .Where(b => b.Fit != null)
                        .Select(b => new ZoomableCanvasSource(new Rectangle(b.Fit!.Location, b.Size), b.Source))
                        .ToArray();
                    var left = locations.MinBy(l => l.Rectangle.X);
                    var top = locations.MinBy(l => l.Rectangle.Y);
                    var right = locations.MaxBy(l => l.Rectangle.Right);
                    var bottom = locations.MaxBy(l => l.Rectangle.Bottom);
                    _canvasSize = new Size(right!.Rectangle.Right - left!.Rectangle.X, bottom!.Rectangle.Bottom - top!.Rectangle.Y);
                    _canvasSources = locations;
                }
            }
        }

        private void OptionsValidOrThrow()
        {
            if (Options.FilePaths.Count == 0)
            {
                throw new ArgumentException("At least one file must be provided.");
            }

            if (!Options.FormatInfo.SupportedColorModes.Contains(Options.ColorMode))
            {
                throw new ArgumentException($"Pixel format {Options.PixelFormat} does not support color mode {Options.ColorMode}.");
            }

            if (Options.TargetMode is TargetMode.ZoomableCanvas or TargetMode.MultipleToFolder && string.IsNullOrEmpty(Options.OutputFolder))
            {
                throw new ArgumentException("Output folder must be specified for the selected target mode.");
            }

            if (!Options.FixedWidth.HasValue && Options.FixedHeight.HasValue)
            {
                throw new ArgumentException("Fixed height mode is unsupported at this time. Please specify a fixed width or neither dimension for automatic sizing.");
            }

            if (Options.FixedWidth.HasValue && Options.FixedWidth.Value <= 0)
            {
                throw new ArgumentException("Fixed width must be a positive integer.");
            }

            if (Options.FixedHeight.HasValue && Options.FixedHeight.Value <= 0)
            {
                throw new ArgumentException("Fixed height must be a positive integer.");
            }

            if (Options.PixelLayout == PixelLayout.Striped
                && Options.FixedWidth.HasValue
                && Options.FixedWidth.Value < Options.FormatInfo.StripeWidth)
            {
                throw new ArgumentException("Fixed width must be greater than or equal to the stripe width for striped layouts.");
            }

            if (Options.ColorMode == ColorMode.Paletted
                && (Options.UserPalette == null || Options.UserPalette.Length != Options.FormatInfo.PaletteRequiredCount))
            {
                throw new ArgumentException("A user palette with the required number of colors must be provided for paletted color mode.");
            }
        }

        private static Size AutomaticSize(long pixels, PixelLayout layout, int stripeWidth)
        {
            return layout switch
            {
                PixelLayout.Raster => AutomaticSizeRaster(pixels),
                PixelLayout.Striped => AutomaticSizeStriped(pixels, stripeWidth),
                _ => throw new ArgumentException($"Unsupported pixel layout {layout}.")
            };
        }

        private static Size AutomaticSizeRaster(long pixels)
        {
            var width = (int)Math.Ceiling(Math.Sqrt(pixels));
            var height = (int)Math.Ceiling((double)pixels / width);
            return new Size(width, height);
        }

        private static Size AutomaticSizeStriped(long pixels, int stripeWidth)
        {
            if (pixels % stripeWidth != 0)
            {
                throw new ArgumentException($"Pixel count {pixels} is not divisible by stripe width {stripeWidth}.");
            }

            var stripes = pixels / stripeWidth;
            var height = (int)Math.Ceiling(Math.Sqrt(stripes));
            var stripesWide = (int)Math.Ceiling((double)stripes / height);
            return new Size(stripesWide * stripeWidth, height);
        }

        private static Size FixedWidthSize(long pixels, PixelLayout layout, int stripeWidth, int fixedWidth)
        {
            return layout switch
            {
                PixelLayout.Raster => FixedWidthSizeRaster(pixels, fixedWidth),
                PixelLayout.Striped => FixedWidthSizeStriped(pixels, stripeWidth, fixedWidth),
                _ => throw new ArgumentException($"Unsupported pixel layout {layout}.")
            };
        }

        private static Size FixedWidthSizeRaster(long pixels, int fixedWidth)
        {
            var height = (int)Math.Ceiling((double)pixels / fixedWidth);
            return new Size(fixedWidth, height);
        }

        private static Size FixedWidthSizeStriped(long pixels, int stripeWidth, int fixedWidth)
        {
            if (pixels % stripeWidth != 0)
            {
                throw new ArgumentException($"Pixel count {pixels} is not divisible by stripe width {stripeWidth}.");
            }

            if (fixedWidth < stripeWidth)
            {
                throw new ArgumentException($"Fixed width {fixedWidth} is less than stripe width {stripeWidth}.");
            }

            var stripes = pixels / stripeWidth;
            var stripesPerRow = fixedWidth / stripeWidth;
            var height = (int)Math.Ceiling((double)stripes / stripesPerRow);
            return new Size(fixedWidth, height);
        }

        private static Size AddHeight(Size size, int additionalHeight)
        {
            return new Size(size.Width, size.Height + additionalHeight);
        }

        private void SizeSingleImageAutomatic(List<(string FilePath, long PixelCount)> filePixelCounts)
        {
            var totalPixels = filePixelCounts.Sum(c => c.PixelCount);
            var formatInfo = Options.FormatInfo;

            if (Options.TitleMode is TitleMode.None or TitleMode.OnePerImage)
            {
                _canvasSize = AutomaticSize(totalPixels, Options.PixelLayout, formatInfo.StripeWidth);

                if (Options.TitleMode == TitleMode.OnePerImage)
                {
                    _canvasSize = AddHeight(_canvasSize.Value, TitleBarHeight);
                }
            }
            else if (Options.TitleMode == TitleMode.OnePerFile)
            {
                var naturalSizes = new List<Size>();
                foreach (var filePixelCount in filePixelCounts)
                {
                    naturalSizes.Add(AutomaticSize(filePixelCount.PixelCount, Options.PixelLayout, formatInfo.StripeWidth));
                }

                var trueWidth = naturalSizes.MaxBy(s => s.Width).Width;
                var trueSizes = new List<Size>();
                foreach (var naturalSize in naturalSizes)
                {
                    Size trueSize = FixedWidthSize(naturalSize.Width * naturalSize.Height, Options.PixelLayout, formatInfo.StripeWidth, trueWidth);
                    trueSize = AddHeight(trueSize, TitleBarHeight);
                    trueSizes.Add(trueSize);
                }
                _canvasSize = new Size(trueWidth, trueSizes.Sum(s => s.Height));
            }
        }

        private void SizeSingleImageFixedWidth(List<(string FilePath, long PixelCount)> filePixelCounts)
        {
            var totalPixels = filePixelCounts.Sum(c => c.PixelCount);
            var formatInfo = Options.FormatInfo;

            if (Options.TitleMode is TitleMode.None or TitleMode.OnePerImage)
            {
                _canvasSize = FixedWidthSize(totalPixels, Options.PixelLayout, formatInfo.StripeWidth, Options.FixedWidth!.Value);

                if (Options.TitleMode == TitleMode.OnePerImage)
                {
                    _canvasSize = AddHeight(_canvasSize.Value, TitleBarHeight);
                }
            }
            else if (Options.TitleMode == TitleMode.OnePerFile)
            {
                var naturalSizes = new List<Size>();
                foreach (var filePixelCount in filePixelCounts)
                {
                    naturalSizes.Add(FixedWidthSize(filePixelCount.PixelCount, Options.PixelLayout, formatInfo.StripeWidth, Options.FixedWidth!.Value));
                }
                var trueSizes = new List<Size>();
                foreach (var naturalSize in naturalSizes)
                {
                    var trueSize = new Size(naturalSize.Width, naturalSize.Height + TitleBarHeight);
                    trueSizes.Add(trueSize);
                }
                _canvasSize = new Size(Options.FixedWidth!.Value, trueSizes.Sum(s => s.Height));
            }
        }

        private Size MeasureText(string text)
        {
            var measurement = TextMeasurer.MeasureSize(text, _titleBarFontOptions);
            return new Size((int)Math.Ceiling(measurement.Width), (int)Math.Ceiling(measurement.Height));
        }

        private string ShrinkTextToFit(string text, int maxWidth)
        {
            throw new NotImplementedException();
        }
    }

    internal record struct ZoomableCanvasSource(Rectangle Rectangle, string FilePath)
    {
        public static implicit operator (Rectangle Rectangle, string FilePath)(ZoomableCanvasSource value)
        {
            return (value.Rectangle, value.FilePath);
        }

        public static implicit operator ZoomableCanvasSource((Rectangle Rectangle, string FilePath) value)
        {
            return new ZoomableCanvasSource(value.Rectangle, value.FilePath);
        }
    }
}
