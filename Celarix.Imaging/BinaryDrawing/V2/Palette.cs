using System;
using SixLabors.ImageSharp.PixelFormats;

namespace Celarix.Imaging.BinaryDrawing.V2
{
    public class Palette
    {
        public Rgba32[] Colors { get; set; }

        public Palette(Rgba32[] colors)
        {
            Colors = colors ?? throw new ArgumentNullException(nameof(colors));
        }

        public static byte GenerateRampValue(int step, int rampBitDepth)
        {
            if (rampBitDepth == 0) return 0;
            int maxValue = (1 << rampBitDepth) - 1;
            
            if (step <= 0) return 0;
            if (step >= maxValue) return 255;
            
            return (byte)Math.Min(255, (int)Math.Ceiling(step * 256d / maxValue));
        }

        // Standard predefined palettes according to outline
        
        public static Palette BlackAndWhite()
        {
            return new Palette(new[]
            {
                new Rgba32(0, 0, 0, 255),
                new Rgba32(255, 255, 255, 255)
            });
        }

        public static Palette Grayscale(int bitDepth)
        {
            int numColors = 1 << bitDepth;
            var colors = new Rgba32[numColors];
            for (int i = 0; i < numColors; i++)
            {
                byte val = GenerateRampValue(i, bitDepth);
                colors[i] = new Rgba32(val, val, val, 255);
            }
            return new Palette(colors);
        }

        public static Palette RGB(int rBits, int gBits, int bBits)
        {
            int totalBits = rBits + gBits + bBits;
            int numColors = 1 << totalBits;
            var colors = new Rgba32[numColors];
            
            int rShift = gBits + bBits;
            int gShift = bBits;
            
            int rMask = (1 << rBits) - 1;
            int gMask = (1 << gBits) - 1;
            int bMask = (1 << bBits) - 1;

            for (int i = 0; i < numColors; i++)
            {
                int rStep = (i >> rShift) & rMask;
                int gStep = (i >> gShift) & gMask;
                int bStep = i & bMask;

                colors[i] = new Rgba32(
                    GenerateRampValue(rStep, rBits),
                    GenerateRampValue(gStep, gBits),
                    GenerateRampValue(bStep, bBits),
                    255
                );
            }
            return new Palette(colors);
        }

        public static Palette RGBA(int rBits, int gBits, int bBits, int aBits)
        {
            int totalBits = rBits + gBits + bBits + aBits;
            int numColors = 1 << totalBits;
            var colors = new Rgba32[numColors];
            
            int rShift = gBits + bBits + aBits;
            int gShift = bBits + aBits;
            int bShift = aBits;
            
            int rMask = (1 << rBits) - 1;
            int gMask = (1 << gBits) - 1;
            int bMask = (1 << bBits) - 1;
            int aMask = (1 << aBits) - 1;

            for (int i = 0; i < numColors; i++)
            {
                int rStep = (i >> rShift) & rMask;
                int gStep = (i >> gShift) & gMask;
                int bStep = (i >> bShift) & bMask;
                int aStep = i & aMask;

                colors[i] = new Rgba32(
                    GenerateRampValue(rStep, rBits),
                    GenerateRampValue(gStep, gBits),
                    GenerateRampValue(bStep, bBits),
                    GenerateRampValue(aStep, aBits)
                );
            }
            return new Palette(colors);
        }

        public static Palette GetDefault(PixelFormat format)
        {
            return format switch
            {
                PixelFormat.Bpp1 => BlackAndWhite(),
                PixelFormat.Bpp2 => Grayscale(2), // 2bpp defaults to b&w? outline says: "The default palette is black and white. The stripe width is 4." Wait, outline explicitly says 2bpp uses palette color #0 black, #1 white?, actually outline says: "00 bits use palette color #0... 11 bits use palette color #3. The default palette is black and white." (Probably Grayscale(2) is black, dark gray, light gray, white).
                // Actually let's just use Grayscale(2) which will generate exactly 4 grayscale steps.
                PixelFormat.Bpp4 => Grayscale(4),
                PixelFormat.Bpp8 => Grayscale(8),
                PixelFormat.Bpp16 => RGB(5, 6, 5),
                // 24bpp uses RGB888 directly, no palette array needed for it, but just in case
                PixelFormat.Bpp24 => null,
                PixelFormat.Bpp32 => null,
                _ => null
            };
        }
    }
}
