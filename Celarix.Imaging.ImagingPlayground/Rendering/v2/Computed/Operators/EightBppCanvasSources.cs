using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground.Rendering.v2.Computed.Operators
{
    public abstract class Unary8BppCanvasSource : PixelIndependentCanvasSource
    {
        public override Size Level0TileCount => new(1, 1);

        public abstract byte Operate(byte value);

        public override Rgba32 GetPixelValue(int x, int y)
        {
            if (x > 15 || y > 15) { return SixLabors.ImageSharp.Color.White; }
            var value = (byte)((y << 4) | x);
            value = Operate(value);
            return new Rgba32(value, value, value, 255);
        }
    }

    public abstract class Binary8BppCanvasSource : PixelIndependentCanvasSource
    {
        public override Size Level0TileCount => new(1, 1);
        public abstract byte Operate(byte value1, byte value2);
        public override Rgba32 GetPixelValue(int x, int y)
        {
            if (x > 255 || y > 255) { return SixLabors.ImageSharp.Color.White; }
            var result = Operate((byte)x, (byte)y);
            return new Rgba32(result, result, result, 255);
        }
    }

    public sealed class Identity8BppCanvasSource : Unary8BppCanvasSource
    {
        public override string Name => "(byte)+x";
        public override byte Operate(byte value) => (byte)+value;
    }

    public sealed class Inverse8BppCanvasSource : Unary8BppCanvasSource
    {
        public override string Name => "(byte)-x";
        public override byte Operate(byte value) => (byte)-value;
    }

    public sealed class BitwiseNot8BppCanvasSource : Unary8BppCanvasSource
    {
        public override string Name => "(byte)~(x)";
        public override byte Operate(byte value) => (byte)~value;
    }

    public sealed class LogicalNot8BppCanvasSource : Unary8BppCanvasSource
    {
        public override string Name => "(byte)!x";
        public override byte Operate(byte value) => (byte)(value == 0 ? 1 : 0);
    }

    public sealed class Factorial8BppCanvasSource : Unary8BppCanvasSource
    {
        public override string Name => "(byte)x!";
        public override byte Operate(byte value)
        {
            byte product = 1;
            while (value > 0)
            {
                product = (byte)(product * value);
                if (product == 0) { return 0; }
                value -= 1;
            }
            return product;
        }
    }

    public sealed class Addition8BppCanvasSource : Binary8BppCanvasSource
    {
        public override string Name => "(byte)(x + y)";
        public override byte Operate(byte value1, byte value2) => (byte)(value1 + value2);
    }

    public sealed class Subtraction8BppCanvasSource : Binary8BppCanvasSource
    {
        public override string Name => "(byte)(x - y)";
        public override byte Operate(byte value1, byte value2) => (byte)(value1 - value2);
    }

    public sealed class Multiplication8BppCanvasSource : Binary8BppCanvasSource
    {
        public override string Name => "(byte)(x * y)";
        public override byte Operate(byte value1, byte value2) => (byte)(value1 * value2);
    }

    public sealed class Division8BppCanvasSource : Binary8BppCanvasSource
    {
        public override string Name => "(byte)(x / y)";
        public override byte Operate(byte value1, byte value2) => value2 == 0 ? (byte)0 : (byte)(value1 / value2);
    }

    public sealed class Modulo8BppCanvasSource : Binary8BppCanvasSource
    {
        public override string Name => "(byte)(x % y)";
        public override byte Operate(byte value1, byte value2) => value2 == 0 ? (byte)0 : (byte)(value1 % value2);
    }

    public sealed class BitwiseAnd8BppCanvasSource : Binary8BppCanvasSource
    {
        public override string Name => "(byte)(x & y)";
        public override byte Operate(byte value1, byte value2) => (byte)(value1 & value2);
    }

    public sealed class BitwiseOr8BppCanvasSource : Binary8BppCanvasSource
    {
        public override string Name => "(byte)(x | y)";
        public override byte Operate(byte value1, byte value2) => (byte)(value1 | value2);
    }

    public sealed class BitwiseXor8BppCanvasSource : Binary8BppCanvasSource
    {
        public override string Name => "(byte)(x ^ y)";
        public override byte Operate(byte value1, byte value2) => (byte)(value1 ^ value2);
    }

    public sealed class BitwiseLeftShift8BppCanvasSource : Binary8BppCanvasSource
    {
        public override string Name => "(byte)(x << y)";
        public override byte Operate(byte value1, byte value2)
        {
            var shift = value2 & 0b111;
            return (byte)(value1 << shift);
        }
    }

    public sealed class BitwiseRightShift8BppCanvasSource : Binary8BppCanvasSource
    {
        public override string Name => "(byte)(x >> y)";
        public override byte Operate(byte value1, byte value2)
        {
            var shift = value2 & 0b111;
            return (byte)(value1 >> shift);
        }
    }

    public sealed class ArithmeticRightShift8BppCanvasSource : Binary8BppCanvasSource
    {
        public override string Name => "(byte)(x >>> y)";
        public override byte Operate(byte value1, byte value2)
        {
            var shift = value2 & 0b111;
            var signExtended = (byte)(~((((value1 & 0x80) != 0 ? 1 : 0) << shift) - 1));
            return (byte)((value1 >> shift) | signExtended);
        }
    }

    public sealed class RotateLeft8BppCanvasSource : Binary8BppCanvasSource
    {
        public override string Name => "(byte)RotateLeft(x, y)";
        public override byte Operate(byte value1, byte value2)
        {
            var shift = value2 & 0b111;
            return (byte)((value1 << shift) | (value1 >> (8 - shift)));
        }
    }

    public sealed class RotateRight8BppCanvasSource : Binary8BppCanvasSource
    {
        public override string Name => "(byte)RotateRight(x, y)";
        public override byte Operate(byte value1, byte value2)
        {
            var shift = value2 & 0b111;
            return (byte)((value1 >> shift) | (value1 << (8 - shift)));
        }
    }

    public sealed class Equals8BppCanvasSource : Binary8BppCanvasSource
    {
        public override string Name => "(byte)(x == y)";
        public override byte Operate(byte value1, byte value2) => (byte)(value1 == value2 ? 255 : 0);
    }

    public sealed class NotEquals8BppCanvasSource : Binary8BppCanvasSource
    {
        public override string Name => "(byte)(x != y)";
        public override byte Operate(byte value1, byte value2) => (byte)(value1 != value2 ? 255 : 0);
    }

    public sealed class GreaterThan8BppCanvasSource : Binary8BppCanvasSource
    {
        public override string Name => "(byte)(x > y)";
        public override byte Operate(byte value1, byte value2) => (byte)(value1 > value2 ? 255 : 0);
    }

    public sealed class LessThan8BppCanvasSource : Binary8BppCanvasSource
    {
        public override string Name => "(byte)(x < y)";
        public override byte Operate(byte value1, byte value2) => (byte)(value1 < value2 ? 255 : 0);
    }

    public sealed class GreaterThanOrEqual8BppCanvasSource : Binary8BppCanvasSource
    {
        public override string Name => "(byte)(x >= y)";
        public override byte Operate(byte value1, byte value2) => (byte)(value1 >= value2 ? 255 : 0);
    }

    public sealed class LessThanOrEqual8BppCanvasSource : Binary8BppCanvasSource
    {
        public override string Name => "(byte)(x <= y)";
        public override byte Operate(byte value1, byte value2) => (byte)(value1 <= value2 ? 255 : 0);
    }

    public sealed class CompareTo8BppCanvasSource : Binary8BppCanvasSource
    {
        public override string Name => "(byte)CompareTo(x, y)";
        public override byte Operate(byte value1, byte value2)
        {
            if (value1 < value2) { return 0; }
            else if (value1 > value2) { return 255; }
            else { return 128; }
        }
    }

    public sealed class NthRoot8BppCanvasSource : Binary8BppCanvasSource
    {
        public override string Name => "(byte)Root(x, y)";
        public override byte Operate(byte value1, byte value2)
        {
            if (value2 == 0) { return 255; }
            if (value1 == 0) { return 0; }
            double root = Math.Pow(value1, 1.0 / value2);
            return (byte)root;
        }
    }

    public sealed class BaseNLogarithm8BppCanvasSource : Binary8BppCanvasSource
    {
        public override string Name => "(byte)Log(x, y)";
        public override byte Operate(byte value1, byte value2)
        {
            if (value1 == 0) { return 0; }
            if (value2 == 0 || value2 == 1) { return 255; }
            double log = Math.Log(value1, value2);
            return (byte)log;
        }
    }
}