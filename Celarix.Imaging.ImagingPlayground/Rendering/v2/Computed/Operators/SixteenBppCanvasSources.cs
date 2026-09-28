using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground.Rendering.v2.Computed.Operators
{
    public abstract class Unary16BppCanvasSource : PixelIndependentCanvasSource
    {
        public override Size Level0TileCount => new(1, 1);
        public abstract ushort Operate(ushort value);
        public override Rgba32 GetPixelValue(int x, int y)
        {
            if (x > 255 || y > 255)
            {
                return SixLabors.ImageSharp.Color.White;
            }

            var value = (ushort)((y << 8) | x);
            value = Operate(value);
            var redBits = value >> 11;
            var greenBits = (value >> 5) & 0b111111;
            var blueBits = value & 0b11111;
            var red = (byte)(255f * (redBits / 31f));
            var green = (byte)(255f * (greenBits / 63f));
            var blue = (byte)(255f * (blueBits / 31f));
            return new Rgba32(red, green, blue, 255);
        }
    }

    public abstract class Binary16BppCanvasSource : PixelIndependentCanvasSource
    {
        public override Size Level0TileCount => new(64, 64);
        public abstract ushort Operate(ushort value1, ushort value2);
        public override Rgba32 GetPixelValue(int x, int y)
        {
            var result = Operate((ushort)x, (ushort)y);
            var redBits = result >> 11;
            var greenBits = (result >> 5) & 0b111111;
            var blueBits = result & 0b11111;
            var red = (byte)(255f * (redBits / 31f));
            var green = (byte)(255f * (greenBits / 63f));
            var blue = (byte)(255f * (blueBits / 31f));
            return new Rgba32(red, green, blue, 255);
        }
    }

    public sealed class Identity16BppCanvasSource : Unary16BppCanvasSource
    {
        public override string Name => "(ushort)+x";
        public override ushort Operate(ushort value) => (ushort)+value;
    }

    public sealed class Inverse16BppCanvasSource : Unary16BppCanvasSource
    {
        public override string Name => "(ushort)-x";
        public override ushort Operate(ushort value) => (ushort)-value;
    }

    public sealed class BitwiseNot16BppCanvasSource : Unary16BppCanvasSource
    {
        public override string Name => "(ushort)~(x)";
        public override ushort Operate(ushort value) => (ushort)~value;
    }

    public sealed class LogicalNot16BppCanvasSource : Unary16BppCanvasSource
    {
        public override string Name => "(ushort)!x";
        public override ushort Operate(ushort value) => (ushort)(value == 0 ? 1 : 0);
    }

    public sealed class Factorial16BppCanvasSource : Unary16BppCanvasSource
    {
        public override string Name => "(ushort)x!";
        public override ushort Operate(ushort value)
        {
            if (value == 0) { return 1; }
            ushort result = 1;
            for (var i = value; i > 0; i--)
            {
                result = (ushort)(result * i);
                if (result == 0) { break; }
            }
            return result;
        }
    }

    public sealed class Addition16BppCanvasSource : Binary16BppCanvasSource
    {
        public override string Name => "(ushort)(x + y)";
        public override ushort Operate(ushort value1, ushort value2)
        {
            return (ushort)(value1 + value2);
        }
    }

    public sealed class Subtraction16BppCanvasSource : Binary16BppCanvasSource
    {
        public override string Name => "(ushort)(x - y)";
        public override ushort Operate(ushort value1, ushort value2)
        {
            return (ushort)(value1 - value2);
        }
    }

    public sealed class Multiplication16BppCanvasSource : Binary16BppCanvasSource
    {
        public override string Name => "(ushort)(x * y)";
        public override ushort Operate(ushort value1, ushort value2)
        {
            return (ushort)(value1 * value2);
        }
    }

    public sealed class Division16BppCanvasSource : Binary16BppCanvasSource
    {
        public override string Name => "(ushort)(x / y)";
        public override ushort Operate(ushort value1, ushort value2)
        {
            if (value2 == 0) { return 0; }
            return (ushort)(value1 / value2);
        }
    }

    public sealed class Modulo16BppCanvasSource : Binary16BppCanvasSource
    {
        public override string Name => "(ushort)(x % y)";
        public override ushort Operate(ushort value1, ushort value2)
        {
            if (value2 == 0) { return 0; }
            return (ushort)(value1 % value2);
        }
    }

    public sealed class BitwiseAnd16BppCanvasSource : Binary16BppCanvasSource
    {
        public override string Name => "(ushort)(x & y)";
        public override ushort Operate(ushort value1, ushort value2)
        {
            return (ushort)(value1 & value2);
        }
    }

    public sealed class BitwiseOr16BppCanvasSource : Binary16BppCanvasSource
    {
        public override string Name => "(ushort)(x | y)";
        public override ushort Operate(ushort value1, ushort value2)
        {
            return (ushort)(value1 | value2);
        }
    }

    public sealed class BitwiseXor16BppCanvasSource : Binary16BppCanvasSource
    {
        public override string Name => "(ushort)(x ^ y)";
        public override ushort Operate(ushort value1, ushort value2)
        {
            return (ushort)(value1 ^ value2);
        }
    }

    public sealed class LeftShift16BppCanvasSource : Binary16BppCanvasSource
    {
        public override string Name => "(ushort)(x << y)";
        public override ushort Operate(ushort value1, ushort value2)
        {
            return (ushort)(value1 << (value2 % 16));
        }
    }

    public sealed class RightShift16BppCanvasSource : Binary16BppCanvasSource
    {
        public override string Name => "(ushort)(x >> y)";
        public override ushort Operate(ushort value1, ushort value2)
        {
            return (ushort)(value1 >> (value2 % 16));
        }
    }

    public sealed class ArithmeticRightShift16BppCanvasSource : Binary16BppCanvasSource
    {
        public override string Name => "(ushort)(x >>> y)";
        public override ushort Operate(ushort value1, ushort value2)
        {
            var power2 = 1 << (value2 % 16);
            var inverseSignExtension = power2 - 1;
            var signExtension = ~inverseSignExtension;
            if ((value1 & 0x8000) == 0)
            {
                signExtension = 0;
            }
            return (ushort)((value1 >> value2) | signExtension);
        }
    }

    public sealed class RotateLeft16BppCanvasSource : Binary16BppCanvasSource
    {
        public override string Name => "(ushort)RotateLeft(x, y)";
        public override ushort Operate(ushort value1, ushort value2)
        {
            value2 %= 16;
            var high = value1 << value2;
            var low = value1 >> (16 - value2);
            return (ushort)(high | low);
        }
    }

    public sealed class RotateRight16BppCanvasSource : Binary16BppCanvasSource
    {
        public override string Name => "(ushort)RotateRight(x, y)";
        public override ushort Operate(ushort value1, ushort value2)
        {
            value2 %= 16;
            var high = value1 << (16 - value2);
            var low = value1 >> value2;
            return (ushort)(high | low);
        }
    }
}