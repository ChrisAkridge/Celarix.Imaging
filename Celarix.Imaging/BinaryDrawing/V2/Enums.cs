namespace Celarix.Imaging.BinaryDrawing.V2
{
    public enum PixelFormat
    {
        Bpp1, Bpp2, Bpp4, Bpp8, Bpp16, Bpp24, Bpp32, FloatSingle, FloatDouble
    }

    public enum OutputType
    {
        SingleImage, ZoomableCanvas, FixedSizeFolder
    }

    public enum StreamNamePrinting
    {
        None, TopOfImage, TopOfStream
    }

    public enum StreamNameLevel
    {
        NameOnly, NameAndSize, NameAndProgress, NameProgressAndSize
    }

    public enum PixelOrder
    {
        Raster, Stripe
    }

    public enum ByteSourceState
    {
        Default, StreamBoundary, EndOfStreams
    }
}
