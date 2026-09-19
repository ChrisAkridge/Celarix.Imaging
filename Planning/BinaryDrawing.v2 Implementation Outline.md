# `Celarix.Imaging.BinaryDrawing.V2` Implementation Outline

A C# class architecture designed to implement the V2 Binary Drawing specification. This architecture uses a component-based pipeline where abstraction handles reading, processing, converting, and writing separately.

## Configuration & Options

### `BinaryDrawingOptions` (Class)
A POCO class that holds the user's configuration for the entire pipeline.
- **Properties**:
  - `PixelFormat PixelFormat` (Enum: Bpp1, Bpp2, Bpp4, FloatSingle, etc.)
  - `OutputType OutputType` (Enum: SingleImage, ZoomableCanvas, FixedSizeFolder)
  - `StreamNamePrinting StreamNamePrinting` (Enum: None, TopOfImage, TopOfStream)
  - `StreamNameLevel StreamNameLevel` (Enum: NameOnly, NameAndSize, NameAndProgress, NameProgressAndSize)
  - `PixelOrder PixelOrder` (Enum: Raster, Stripe)
  - `Palette Palette` (Reference to a custom or default palette object)
  - `int? FixedWidth`
  - `Size? FixedSize`

## Input Abstractions

### `ByteSource` (Class)
Wraps multiple input streams to appear as a single continuous byte sequence with boundaries.
- **Public Methods**:
  - `int Read(byte[] buffer, int offset, int count)`: Reads bytes, stopping exactly at a stream boundary. Callers must check state before reading further.
  - `void Seek(long offset, SeekOrigin origin)`: Seeks across all bound streams (if supported).
- **Properties**:
  - `long Position`, `long Length`
  - `string CurrentStreamName`: Gets the original file path/name of the active stream.
  - `ByteSourceState State`: Current state enum (`Default`, `StreamBoundary`, `EndOfStreams`).

### `IStreamProvider` (Interface)
Provides a stream, allowing for lazily-evaluated streams.
- **Implementations**:
  - `LazyFileStreamProvider`: Opens a FileStream when requested, disposing it when advanced past.
  - `ImmediateStreamProvider`: Wraps a stream already loaded in memory (e.g., `MemoryStream`).
- **Methods**: `Stream GetStream()`, `long Length { get; }`, `string Name { get; }`

## Core Processing

### `Palette` (Class)
Encapsulates an array of colors and the methods to generate default palette ramps.
- **Properties**: `Rgba32[] Colors`
- **Internal Methods**:
  - `static Rgba32 GenerateRampColor(byte channelValue, int rampBitDepth)`: Generates individual steps using the `256 / (2^n - 1)` ceiling rounded math specified in the outline.
- **Static Generators** (e.g. `static Palette GetDefault(PixelFormat format)` or `CreateGrayscale()`)

### `PixelConverter` (Static Class)
Stateless utility that converts raw byte spans into spans of `Rgba32` pixels based on format.
- **Public Methods**:
  - `static void ConvertToPixels(ReadOnlySpan<byte> bytes, Span<Rgba32> pixels, PixelFormat format, Palette palette)`: Executes the bitwise shifts and palette lookups (or mantissa/exponent extraction for floats) necessary to convert bytes to RGBA32. Handles the custom guard colors and subnormal logic for floats.

### `SizeCalculator` (Static Class)
Math utilities for estimating required image sizes before layout.
- **Public Methods**:
  - `static Size CalculateDimensions(long totalPixels, BinaryDrawingOptions options)`: Executes the "Square Root", "Stripe-Sizing", or "Fixed" methods depending on format and constraints.
  - `static int GetPixelCountForFormat(long byteCount, PixelFormat format)`: Converts length in bytes to the number of expected pixels laid out.

### `DrawingProgress` (Struct)
A struct to hold progress information for the pipeline.
- **Properties**:
  - `long BytesProcessed`: The number of bytes that have been processed.
  - `long TotalBytes`: The total number of bytes to be processed.
  - `string CurrentStreamName`: The name of the current stream being processed.
  - `long CurrentStreamBytesProcessed`: The number of bytes that have been processed for the current stream.
  - `long CurrentStreamTotalBytes`: The total number of bytes for the current stream.
  - `int? FixedSizeImagesDrawn`: The number of fixed size images that have been drawn when in Fixed Size To Folder mode.
  - `ZoomableCanvasProgress? ZoomableCanvasProgress`: The progress of the zoomable canvas when in Zoomable Canvas mode.

### `ZoomableCanvasProgress` (Struct)
A struct to hold progress information for the zoomable canvas.
- **Properties**:
  - `int ZoomLevel`: The current zoom level.
  - `int MaxZoomLevel`: The maximum zoom level required to fully make the canvas, easily computed using max(ceiling(log_2(width)), ceiling(log_2(height))).
  - `int TileX`: The current tile X coordinate at this zoom level.
  - `int TileY`: The current tile Y coordinate at this zoom level.
  - `int TileCountX`: The total number of tile X coordinates at this zoom level.
  - `int TileCountY`: The total number of tile Y coordinates at this zoom level.

## Output Abstractions (Image Layout)

### `ICanvas` (Interface)
The abstracted target for drawing pixels. Implementations handle the specifics between memory and file persistence.
- **Methods**:
  - `void SetPixelsLine(int x, int y, ReadOnlySpan<Rgba32> pixels)`: A block-draw method for faster raster or stripe laying.
  - `void DrawStreamHeaderLine(int y, string streamName, long size, double progress)`: Draws the 24-pixel high background with text, handling truncation and IEC formatting under the hood based on `BinaryDrawingOptions`.
  - `void SaveAndDispose()`: Flushes remaining data to disk.

### `SingleImageCanvas : ICanvas` (Class)
Holds an `Image<Rgba32>` in memory for rendering the entirety of the pipeline at once. Throws if dimensions are too large for memory.

### `FixedSizeFolderCanvas : ICanvas` (Class)
Buffers one `width x height` image at a time.
- **Internal Mechanics**: Checks if `SetPixelsLine` will run off the current image. If so, saves `image{D8}.png` to disk, increments the counter, creates a new blank image, sets progress greens, and resumes drawing. Methods also take an optional `IProgress<DrawingProgress>` object to report progress back to the caller.

### `ZoomableCanvasManager : ICanvas` (Class)
Tiles massive images onto disk to sidestep memory limits.
- **Internal Mechanics**:
  - Tracks which `0/y/x.png` tiles need to be instantiated at zoom level 0 based on `x, y` boundaries. Loads/saves 1024x1024 regions, maintaining a small LRU cache of actively drawn tiles to avoid file thrashing.
  - `void SaveAndDispose()` triggers a hierarchical downsampling loop, reading 2x2 blocks of zoom level N tiles to build zoom level N+1 tiles until the canvas fits in a single 1x1 tile.
  - Methods also take an optional `IProgress<DrawingProgress>` object to report progress back to the caller.

## Main Pipeline Orchestrator

### `BinaryDrawingPipeline` (Class)
Wires the whole process together. Takes the user's `BinaryDrawingOptions` and initialized `ByteSource`.
- **Public Methods**:
  - `void Run(string outputDirectoryOrFilePath)`: The primary execution block.
- **Private Pipeline Steps**:
  1. Evaluate `SizeCalculator` formulas to figure out total output Width and Height.
  2. Instantiate the appropriate `ICanvas` implementation.
  3. Pre-create the `Palette`.
  4. Begin loop `while ByteSource.State != EndOfStreams`:
     - Calculate target Y position for headers if `StreamNamePrinting` requires it.
     - Call `ICanvas.DrawStreamHeaderLine` if needed.
     - Read contiguous buffers from the stream.
     - Pass buffer to `PixelConverter` to get pixel span.
     - Iterate pixel span to find optimal `x, y` sequences based on `PixelOrder` (Raster vs Striped).
     - Call `ICanvas.SetPixelsLine` with chunks.
     - Check `ByteSource.State`. If `StreamBoundary`, advance stream, recalculate Y/header needs.
  5. Close stream, call `Canvas.SaveAndDispose()`.
