using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.BinaryDrawing.v2
{
    internal class FloatComponents
    {
        private const int FloatSubnormalColor = unchecked(0x303030FF);
        private const int FloatInfinityColor = unchecked(0x00FF00FF);
        private const int FloatNaNColor = unchecked((int)0xFF0000FF);
        private const int FloatNormalColor = unchecked((int)0xA0A0A0FF);

        public FloatPixelFormat Format { get; }
        public int Sign { get; }
        public int Exponent { get; }
        public long MantissaBits { get; }

        public FloatKind Kind
        {
            get
            {
                if (Exponent == 0) { return FloatKind.Subnormal; }
                var maxExponent = Format switch
                {
                    FloatPixelFormat.Half => 31,
                    FloatPixelFormat.Single => 255,
                    FloatPixelFormat.Double => 2047,
                    _ => throw new ArgumentOutOfRangeException(nameof(Kind))
                };
                if (Exponent == maxExponent)
                {
                    return MantissaBits == 0L ? FloatKind.Infinity : FloatKind.NaN;
                }
                return FloatKind.Normal;
            }
        }

        public int GuardColor
        {
            get
            {
                return Kind switch
                {
                    FloatKind.Normal => FloatNormalColor,
                    FloatKind.Subnormal => FloatSubnormalColor,
                    FloatKind.Infinity => FloatInfinityColor,
                    FloatKind.NaN => FloatNaNColor,
                    _ => throw new InvalidOperationException("Unreachable.")
                };
            }
        }

        public long Mantissa
        {
            get
            {
                if (Kind == FloatKind.Normal)
                {
                    return MantissaBits | (1L << (Format switch
                    {
                        FloatPixelFormat.Half => 10,
                        FloatPixelFormat.Single => 23,
                        FloatPixelFormat.Double => 52,
                        _ => throw new ArgumentOutOfRangeException(nameof(Kind))
                    }));
                }
                return MantissaBits;
            }
        }

        public FloatComponents(short halfBits)
        {
            Format = FloatPixelFormat.Half;
            Sign = halfBits >> 15;
            Exponent = (halfBits >> 10) & 0b11111;
            MantissaBits = halfBits & 0b11_11111111;
        }

        public FloatComponents(int singleBits)
        {
            Format = FloatPixelFormat.Single;
            Sign = singleBits >> 31;
            Exponent = (singleBits >> 23) & 0xFF;
            MantissaBits = singleBits & 0x7FFFFF;
        }

        public FloatComponents(long doubleBits)
        {
            Format = FloatPixelFormat.Double;
            Sign = (int)(doubleBits >> 63);
            Exponent = (int)((doubleBits >> 52) & 0x7FF);
            MantissaBits = doubleBits & 0xFFFFFFFFFFFFF;
        }
    }
}
