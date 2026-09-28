using Celarix.Imaging.BinaryDrawing.v2;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.BinaryDrawing.v3
{
    internal static class DefaultPaletteProvider
    {
        public static Rgba32[] GetPalette(PixelFormat format, ColorMode colorMode)
        {
            return format switch
            {
                PixelFormat.Binary1Bpp => GrayscaleRamp(2),
                PixelFormat.Binary2Bpp => GrayscaleRamp(4),
                PixelFormat.Binary3Bpp => colorMode switch
                {
                    ColorMode.Grayscale => GrayscaleRamp(8),
                    ColorMode.Rgb => RgbRamp(2, 2, 2),
                    _ => throw new ArgumentOutOfRangeException(nameof(colorMode)),
                },
                PixelFormat.Binary4Bpp => colorMode switch
                {
                    ColorMode.Grayscale => GrayscaleRamp(16),
                    ColorMode.Rgb => RgbRamp(2, 4, 2),
                    ColorMode.Rgba => RgbaRamp(2, 2, 2, 2),
                    _ => throw new ArgumentOutOfRangeException(nameof(colorMode)),
                },
                PixelFormat.Binary8Bpp => colorMode switch
                {
                    ColorMode.Grayscale => GrayscaleRamp(256),
                    ColorMode.Rgb => RgbRamp(8, 8, 4),
                    ColorMode.Rgba => RgbaRamp(4, 4, 4, 4),
                    _ => throw new ArgumentOutOfRangeException(nameof(colorMode)),
                },
                PixelFormat.Binary16Bpp => colorMode switch
                {
                    ColorMode.Rgb => RgbRamp(32, 64, 32),
                    ColorMode.Rgba => RgbaRamp(16, 16, 16, 16),
                    _ => throw new ArgumentOutOfRangeException(nameof(colorMode)),
                },
                PixelFormat.Binary24Bpp => [],
                PixelFormat.Binary32Bpp => [],
                PixelFormat.Float16 => [],
                PixelFormat.Float32 => [],
                PixelFormat.Float64 => [],
                _ => throw new ArgumentOutOfRangeException(nameof(format)),
            };
        }

        private static Rgba32[] GrayscaleRamp(int steps)
        {
            var palette = new Rgba32[steps];
            for (int i = 0; i < steps; i++)
            {
                byte intensity = (byte)(i * 255 / (steps - 1));
                palette[i] = new Rgba32(intensity, intensity, intensity);
            }
            return palette;
        }

        private static Rgba32[] RgbRamp(int redSteps, int greenSteps, int blueSteps)
        {
            var palette = new Rgba32[redSteps * greenSteps * blueSteps];
            int index = 0;
            for (int r = 0; r < redSteps; r++)
            {
                byte red = (byte)(r * 255 / (redSteps - 1));
                for (int g = 0; g < greenSteps; g++)
                {
                    byte green = (byte)(g * 255 / (greenSteps - 1));
                    for (int b = 0; b < blueSteps; b++)
                    {
                        byte blue = (byte)(b * 255 / (blueSteps - 1));
                        palette[index++] = new Rgba32(red, green, blue);
                    }
                }
            }
            return palette;
        }

        private static Rgba32[] RgbaRamp(int redSteps, int greenSteps, int blueSteps, int alphaSteps)
        {
            var palette = new Rgba32[redSteps * greenSteps * blueSteps * alphaSteps];
            int index = 0;
            for (int r = 0; r < redSteps; r++)
            {
                byte red = (byte)(r * 255 / (redSteps - 1));
                for (int g = 0; g < greenSteps; g++)
                {
                    byte green = (byte)(g * 255 / (greenSteps - 1));
                    for (int b = 0; b < blueSteps; b++)
                    {
                        byte blue = (byte)(b * 255 / (blueSteps - 1));
                        for (int a = 0; a < alphaSteps; a++)
                        {
                            byte alpha = (byte)(a * 255 / (alphaSteps - 1));
                            palette[index++] = new Rgba32(red, green, blue, alpha);
                        }
                    }
                }
            }
            return palette;
        }
    }
}
