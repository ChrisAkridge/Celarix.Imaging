using Celarix.Imaging.BinaryDrawing.v3;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.BinaryDrawing.v4.PixelSources
{
    public sealed class NamedPixelSourceProvider
    {
        // 1024 pixels * 3 bytes per pixel (24-bit)
        // Allows us to read enough bytes to always fill an integer number of pixels at supported
        // bit depths.
        private const int ByteBufferSize = 3072;

        private readonly byte[] _byteBuffer = new byte[ByteBufferSize];
        private readonly Rgba32[] _pixelBuffer;
        private readonly NamedByteSourceProvider _byteProvider;
        private readonly IPixelSource _pixelSource;

        public string CurrentFilePath => _byteProvider.CurrentFilePath;

        public NamedPixelSourceProvider(NamedByteSourceProvider byteProvider, BinaryDrawingBitDepths bitDepth)
        {
            var pixelFormat = PixelFormatInfo.GetFormat(bitDepth switch
            {
                BinaryDrawingBitDepths.OneBit => v2.PixelFormat.Binary1Bpp,
                BinaryDrawingBitDepths.FourBit => v2.PixelFormat.Binary4Bpp,
                BinaryDrawingBitDepths.EightBit => v2.PixelFormat.Binary8Bpp,
                BinaryDrawingBitDepths.SixteenBit => v2.PixelFormat.Binary16Bpp,
                BinaryDrawingBitDepths.TwentyFourBit => v2.PixelFormat.Binary24Bpp,
                _ => throw new ArgumentException($"Unsupported bit depth: {bitDepth}")
            });

            var bytesPerGroup = pixelFormat.SourceBytesPerGroup;
            var groupsPerBuffer = ByteBufferSize / bytesPerGroup;
            var pixelsPerGroup = pixelFormat.OutputPixelsPerGroup;
            var pixelBufferSize = groupsPerBuffer * pixelsPerGroup;

            _byteProvider = byteProvider;
            _pixelBuffer = new Rgba32[pixelBufferSize];
            _pixelSource = bitDepth switch
            {
                //BinaryDrawingBitDepths.OneBit => new OneBitPixelSource(_byteProvider),
                //BinaryDrawingBitDepths.FourBit => new FourBitPixelSource(_byteProvider),
                //BinaryDrawingBitDepths.EightBit => new EightBitPixelSource(_byteProvider),
                //BinaryDrawingBitDepths.SixteenBit => new SixteenBitPixelSource(_byteProvider),
                BinaryDrawingBitDepths.TwentyFourBit => new TwentyFourBitPixelSource(_byteBuffer),
                _ => throw new ArgumentException($"Unsupported bit depth: {bitDepth}")
            };
        }
    }
}
