using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.BinaryDrawing.v2
{
    internal static class BufferFiller
    {
        public static int Fill1BppBuffer(byte[] byteBuffer,
            int[] colorBuffer,
            int bytesRead)
        {
            var colorsWritten = 0;
            for (var i = 0; i < bytesRead; i++)
            {
                var currentByte = byteBuffer[i];
                var colorStartIndex = i * 8;
                colorsWritten += WriteClamped(colorBuffer, colorStartIndex, [
                    (currentByte >> 7),
                    (currentByte >> 6) & 0b1,
                    (currentByte >> 5) & 0b1,
                    (currentByte >> 4) & 0b1,
                    (currentByte >> 3) & 0b1,
                    (currentByte >> 2) & 0b1,
                    (currentByte >> 1) & 0b1,
                    currentByte & 0b1
                ], out var full);
                if (full) { break; }
            }
            return colorsWritten;
        }

        public static int Fill2BppBuffer(byte[] byteBuffer,
            int[] colorBuffer,
            int bytesRead)
        {
            var colorsWritten = 0;
            for (var i = 0; i < bytesRead; i++)
            {
                var currentByte = byteBuffer[i];
                var colorStartIndex = i * 4;
                colorsWritten += WriteClamped(colorBuffer, colorStartIndex, [
                    (currentByte >> 6) & 0b11,
                    (currentByte >> 4) & 0b11,
                    (currentByte >> 2) & 0b11,
                    currentByte & 0b11
                ], out var full);
                if (full) { break; }
            }
            return colorsWritten;
        }

        public static int Fill3BppBuffer(byte[] byteBuffer,
            int[] colorBuffer,
            int bytesRead)
        {
            var colorsWritten = 0;
            var colorStartIndex = 0;
            for (var i = 0; i < bytesRead; i += 3)
            {
                var _0 = ReadByteOrZero(byteBuffer, bytesRead, i);
                var _1 = ReadByteOrZero(byteBuffer, bytesRead, i + 1);
                var _2 = ReadByteOrZero(byteBuffer, bytesRead, i + 2);

                int written = WriteClamped(colorBuffer, colorStartIndex, [
                    _0 >> 5,
                    (_0 >> 2) & 0b111,
                    (_0 & 0b11) << 1 | (_1 >> 7),
                    (_1 >> 4) & 0b111,
                    (_1 >> 1) & 0b111,
                    (_1 & 0b1) << 2 | (_2 >> 6),
                    (_2 >> 3) & 0b111,
                    _2 & 0b111
                ], out var full);
                colorsWritten += written;
                colorStartIndex += written;
                if (full) { break; }
            }
            return colorsWritten;
        }

        public static int Fill4BppBuffer(byte[] byteBuffer,
            int[] colorBuffer,
            int bytesRead)
        {
            var colorsWritten = 0;
            for (var i = 0; i < bytesRead; i++)
            {
                var currentByte = byteBuffer[i];
                var colorStartIndex = i * 2;
                colorsWritten += WriteClamped(colorBuffer, colorStartIndex, [
                    currentByte >> 4,
                    currentByte & 0b1111
                ], out var full);
                if (full) { break; }
            }
            return colorsWritten;
        }

        public static int Fill8BppBuffer(byte[] byteBuffer,
            int[] colorBuffer,
            int bytesRead)
        {
            var colorsWritten = 0;
            for (var i = 0; i < bytesRead; i++)
            {
                var currentByte = byteBuffer[i];
                if (i >= colorBuffer.Length) { break; }
                colorBuffer[i] = currentByte;
                colorsWritten += 1;
            }
            return colorsWritten;
        }

        public static int Fill16BppBuffer(byte[] byteBuffer,
            int[] colorBuffer,
            int bytesRead)
        {
            var colorsWritten = 0;
            for (var i = 0; i < bytesRead; i += 2)
            {
                var _0 = ReadByteOrZero(byteBuffer, bytesRead, i);
                var _1 = ReadByteOrZero(byteBuffer, bytesRead, i + 1);
                var colorIndex = i / 2;
                if (colorIndex >= colorBuffer.Length) { break; }
                colorBuffer[colorIndex] = (_0 << 8) | _1;
                colorsWritten += 1;
            }
            return colorsWritten;
        }

        public static int Fill24BppBuffer(byte[] byteBuffer,
            int[] colorBuffer,
            int bytesRead)
        {
            var colorsWritten = 0;
            for (var i = 0; i < bytesRead; i += 3)
            {
                var _0 = ReadByteOrZero(byteBuffer, bytesRead, i);
                var _1 = ReadByteOrZero(byteBuffer, bytesRead, i + 1);
                var _2 = ReadByteOrZero(byteBuffer, bytesRead, i + 2);
                var colorIndex = i / 3;
                if (colorIndex >= colorBuffer.Length) { break; }
                colorBuffer[colorIndex] = (_0 << 16) | (_1 << 8) | _2;
                colorsWritten += 1;
            }
            return colorsWritten;
        }

        public static int Fill32BppBuffer(byte[] byteBuffer,
            int[] colorBuffer,
            int bytesRead)
        {
            var colorsWritten = 0;
            for (var i = 0; i < bytesRead; i += 4)
            {
                var _0 = ReadByteOrZero(byteBuffer, bytesRead, i);
                var _1 = ReadByteOrZero(byteBuffer, bytesRead, i + 1);
                var _2 = ReadByteOrZero(byteBuffer, bytesRead, i + 2);
                var _3 = ReadByteOrZero(byteBuffer, bytesRead, i + 3);
                var colorIndex = i / 4;
                if (colorIndex >= colorBuffer.Length) { break; }
                colorBuffer[colorIndex] = unchecked((_0 << 24) | (_1 << 16) | (_2 << 8) | _3);
                colorsWritten += 1;
            }
            return colorsWritten;
        }

        private static int FillFloat16Buffer(byte[] byteBuffer,
            int[] colorBuffer,
            int bytesRead)
        {
            var colorsWritten = 0;
            for (var i = 0; i < bytesRead; i += 2)
            {
                var _0 = ReadByteOrZero(byteBuffer, bytesRead, i);
                var _1 = ReadByteOrZero(byteBuffer, bytesRead, i + 1);
                var halfBits = unchecked((short)((_0 << 8) | _1));
                var components = new FloatComponents(halfBits);
                var colorStartIndex = (i / 2) * 7;
                var guard = components.GuardColor;
                var m = components.Mantissa;
                var e = RampPart(components.Exponent, 0, 5);
                colorsWritten += WriteClamped(colorBuffer, colorStartIndex, [
                    guard,
                    guard,
                    components.Sign == 0 ? unchecked((int)0xFFFFFFFF) : 0,
                    guard,
                    Quad(e, e, e, 0xFF),
                    guard,
                    Quad(RampPart(m, 7, 4), RampPart(m, 3, 4), RampPart(m, 0, 3), 0xFF),
                ], out var full);
                if (full) { break; }
            }
            return colorsWritten;
        }

        private static int FillFloat32Buffer(byte[] byteBuffer,
            int[] colorBuffer,
            int bytesRead)
        {
            var colorsWritten = 0;
            for (var i = 0; i < bytesRead; i += 4)
            {
                var _0 = ReadByteOrZero(byteBuffer, bytesRead, i);
                var _1 = ReadByteOrZero(byteBuffer, bytesRead, i + 1);
                var _2 = ReadByteOrZero(byteBuffer, bytesRead, i + 2);
                var _3 = ReadByteOrZero(byteBuffer, bytesRead, i + 3);
                var singleBits = Quad(_0, _1, _2, _3); // heh-heh-heh
                var components = new FloatComponents(singleBits);
                var colorStartIndex = (i / 4) * 7;
                var guard = components.GuardColor;
                var m = components.Mantissa;
                var e = (byte)components.Exponent;
                colorsWritten += WriteClamped(colorBuffer, colorStartIndex, [
                    guard,
                    guard,
                    components.Sign == 0 ? unchecked((int)0xFFFFFFFF) : 0,
                    guard,
                    Quad(e, e, e, 0xFF),
                    guard,
                    Quad(RampPart(m, 16, 8), RampPart(m, 8, 8), RampPart(m, 0, 8), 0xFF),
                ], out var full);
                if (full) { break; }
            }
            return colorsWritten;
        }

        private static int FillFloat64Buffer(byte[] byteBuffer,
            int[] colorBuffer,
            int bytesRead)
        {
            var colorsWritten = 0;
            for (var i = 0; i < bytesRead; i += 8)
            {
                var _0 = ReadByteOrZero(byteBuffer, bytesRead, i);
                var _1 = ReadByteOrZero(byteBuffer, bytesRead, i + 1);
                var _2 = ReadByteOrZero(byteBuffer, bytesRead, i + 2);
                var _3 = ReadByteOrZero(byteBuffer, bytesRead, i + 3);
                var _4 = ReadByteOrZero(byteBuffer, bytesRead, i + 4);
                var _5 = ReadByteOrZero(byteBuffer, bytesRead, i + 5);
                var _6 = ReadByteOrZero(byteBuffer, bytesRead, i + 6);
                var _7 = ReadByteOrZero(byteBuffer, bytesRead, i + 7);
                var doubleBits = unchecked(((long)_0 << 56)
                    | ((long)_1 << 48)
                    | ((long)_2 << 40)
                    | ((long)_3 << 32)
                    | ((long)_4 << 24)
                    | ((long)_5 << 16)
                    | ((long)_6 << 8)
                    | _7);
            var components = new FloatComponents(doubleBits);
                var colorStartIndex = (i / 8) * 9;
                var guard = components.GuardColor;
                var m = components.Mantissa;
                var e = components.Exponent;
                colorsWritten += WriteClamped(colorBuffer, colorStartIndex, [
                    guard,
                    guard,
                    components.Sign == 0 ? unchecked((int)0xFFFFFFFF) : 0,
                    guard,
                    Quad(RampPart(e, 7, 4), RampPart(e, 3, 4), RampPart(e, 0, 3), 0xFF),
                    guard,
                    Quad(RampPart(m, 51, 2), RampPart(m, 49, 2), RampPart(m, 48, 1), 0xFF),
                    Quad(RampPart(m, 40, 8), RampPart(m, 32, 8), RampPart(m, 24, 8), 0xFF),
                    Quad(RampPart(m, 16, 8), RampPart(m, 8, 8), RampPart(m, 0, 8), 0xFF),
                ], out var full);
                if (full) { break; }
            }
            return colorsWritten;
        }

        private static byte ReadByteOrZero(ReadOnlySpan<byte> source, int bytesRead, int index) =>
            index < source.Length ? source[index] : (byte)0;

        private static int WriteClamped(Span<int> destination, int start, ReadOnlySpan<int> colors, out bool full)
        {
            var colorsWritten = 0;
            full = false;
            for (var i = 0; i < colors.Length; i++)
            {
                var destIndex = start + i;
                if (destIndex >= destination.Length)
                {
                    full = true;
                    break;
                }
                destination[destIndex] = colors[i];
                colorsWritten += 1;
            }
            return colorsWritten;
        }

        private static byte RampPart(long value, int shiftAmount, int bits)
        {
            var mask = (1 << bits) - 1;
            var shifted = value >> shiftAmount;
            var masked = shifted & mask;
            var ramp = 255f * (masked / (float)mask);
            return (byte)ramp;
        }

        private static int Quad(byte _0, byte _1, byte _2, byte _3) =>
            unchecked((_0 << 24) | (_1 << 16) | (_2 << 8) | _3);
    }
}
