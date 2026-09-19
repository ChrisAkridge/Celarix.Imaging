# `Celarix.Imaging.BinaryDrawing`

The `BinaryDrawing` namespace provides tools for **visualizing raw binary data as images**. The central idea is: treat any stream of bytes as if it were raw pixel data, choose a bit-depth and a colour palette, and render the result. The namespace also includes pixel-sorting utilities, a unique-colour extractor, and a raw-byte exporter.

---

## Overview

| Type | Kind | Purpose |
|---|---|---|
| `Drawer` | `static class` | Main entry point — all drawing/analysis operations |
| `DefaultPalettes` | `static class` | Pre-built palettes for 1 – 16 bpp colour modes |
| `PixelBufferer` | `internal static class` | Splits raw byte buffers into palette indices |
| `PartiallyKnownSize` | `readonly struct` | Describes an image size where one or both dimensions may be unknown |
| `UniqueColorsImage<TPixel>` | `sealed class` | Return value that pairs an image with its unique-colour count |
| `DrawingMode` | `enum` | Controls how decoded pixels are laid out on the output image |

---

## `Drawer`

`Drawer` is a static façade that exposes all high-level operations. It relies on:
- `StreamEnumerable` / `PixelBufferer` to decode bytes into pixel values
- `Helpers.GetSizeFromCount` to auto-calculate square-ish image dimensions
- `LibraryConfiguration` for tunable constants (progress reporting interval, canvas tile size)
- `CanvasGenerator` for tiled zoomable-canvas output

### Public Methods

#### `Draw`
```csharp
Image<Rgba32> Draw(Stream stream, int bitDepth,
    IReadOnlyList<Rgba32> palette, CancellationToken cancellationToken,
    IProgress<DrawingProgress> progress,
    DrawingMode drawingMode = DrawingMode.Raster)
```
Reads the entire `stream`, decodes it at the given `bitDepth`, maps each value through `palette`, and returns a single `Image<Rgba32>`. The image dimensions are calculated automatically to be as square as possible via `Helpers.GetSizeFromCount`.

The optional `drawingMode` controls pixel placement (see [`DrawingMode`](#drawingmode) below). It defaults to `Raster` so all existing callers are unaffected.

For **24-bpp** and **32-bpp** depths, pass `null` for `palette`; raw RGB/RGBA triples are used directly.

---

#### `DrawCanvas`
```csharp
void DrawCanvas(Stream stream, string outputFolderPath, int bitDepth,
    IReadOnlyList<Rgba32> palette, CancellationToken cancellationToken,
    IProgress<DrawingProgress> progress)
```
Like `Draw`, but writes the result to disk as a **tiled zoomable canvas** (a grid of `256 × 256` PNG tiles) rather than returning an in-memory image. Useful for very large streams that would not fit in memory as a single image.

---

#### `DrawFixedSize`
```csharp
Image<Rgba32> DrawFixedSize(PartiallyKnownSize size, Stream stream,
    int bitDepth, IReadOnlyList<Rgba32> palette,
    CancellationToken cancellationToken, IProgress<DrawingProgress> progress)
```
Like `Draw`, but forces at least one image dimension. Pass a `PartiallyKnownSize` with one or both dimensions set; the remaining dimension is derived from the pixel count. If both dimensions are supplied, the image is cropped/padded to exactly that size.

---

#### `DrawFixedSizeWithSourceText`
```csharp
Image<Rgba32> DrawFixedSizeWithSourceText(Size size,
    NamedMultiStream stream, int bitDepth,
    IReadOnlyList<Rgba32> palette, CancellationToken cancellationToken,
    IProgress<DrawingProgress> progress)
```
Renders a fixed-size binary image and overlays a source filename banner at the top of the image in Consolas 20 pt. When the input spans multiple files (via `NamedMultiStream`), the banner shows the primary filename followed by a `+N` suffix. The banner height is computed by `Helpers.GetTextHeight`.

---

#### `Sort`
```csharp
Image<Rgba32> Sort(Image<Rgba32> image, SortMode sortMode,
    CancellationToken cancellationToken, IProgress<DrawingProgress> progress)
```
Collects every pixel in the image, sorts them by the key defined by `sortMode`, then writes them back row-by-row into a new same-sized image. Sorting is stable within mode.

| `SortMode` | Sort Key |
|---|---|
| `RGB` | Packed `R << 24 \| G << 16 \| B << 8 \| A` |
| `HSV` | Converted to HSV; packed `H << 16 \| S << 8 \| V` |
| `YCbCr` | Converted to Y′CbCr; packed `Y << 16 \| Cb << 8 \| Cr` |

---

#### `UniqueColors`
```csharp
UniqueColorsImage<Rgba32> UniqueColors(Image<Rgba32> image,
    CancellationToken cancellationToken, IProgress<DrawingProgress> progress)
```
Scans the source image and collects the set of distinct `Rgba32` values. The unique colours are then written into a new, compactly-sized image (auto-sized via `Helpers.GetSizeFromCount`). Returns a `UniqueColorsImage<Rgba32>` containing both the count and the output image.

---

#### `ToRaw`
```csharp
byte[] ToRaw(Image<Rgba32> image, CancellationToken cancellationToken,
    IProgress<DrawingProgress> progress)
```
Converts an image back to a flat `byte[]` of raw RGBA values in row-major order (R, G, B, A per pixel).

---

### Supported Bit Depths

| Bit depth | Palette required? | Notes |
|---|---|---|
| 1 | Yes (2 entries) | 8 pixels per byte, MSB first |
| 2 | Yes (4 entries) | 4 pixels per byte |
| 4 | Yes (16 entries) | 2 pixels per byte |
| 8 | Yes (256 entries) | 1 pixel per byte |
| 16 | Yes (65 536 entries) | 2 bytes per pixel, big-endian |
| 24 | No (`null`) | 3 bytes per pixel, RGB |
| 32 | No (`null`) | 4 bytes per pixel, RGBA |

---

## `DefaultPalettes`

A static class that lazily initialises nine pre-built `Rgba32[]` palettes on first access and exposes them as `IReadOnlyList<Rgba32>` properties.

### Palettes

| Property | Bit depth | Entries | Colour model |
|---|---|---|---|
| `OneBppGrayscale` | 1 | 2 | Black → White |
| `TwoBppGrayscale` | 2 | 4 | Even steps from 0 → 255 |
| `FourBppGrayscale` | 4 | 16 | Even steps from 0 → 255 |
| `FourBppRgb121` | 4 | 16 | 1-bit R, 2-bit G, 1-bit B |
| `EightBppGrayscale` | 8 | 256 | Identity (index == grey level) |
| `EightBppRgb332` | 8 | 256 | 3-bit R, 3-bit G, 2-bit B |
| `EightBppArgb2222` | 8 | 256 | 2-bit A, 2-bit R, 2-bit G, 2-bit B |
| `SixteenBppRgb565` | 16 | 65 536 | 5-bit R, 6-bit G, 5-bit B |
| `SixteenBppArgb4444` | 16 | 65 536 | 4-bit A, 4-bit R, 4-bit G, 4-bit B |

All multi-bit channels are **linearly scaled** from their raw bit range to the full 0–255 byte range using `GenerateRange`.

### `GetPalette(int bitDepth, ColorMode mode)`

A convenience method to retrieve the correct palette by bit-depth and `ColorMode` (`Grayscale`, `Rgb`, or `Argb`). Throws `ArgumentException` for unsupported combinations.

---

## `PixelBufferer` *(internal)*

Converts raw `byte[]` buffers into arrays of `int` palette indices, one per pixel. Called internally by `StreamEnumerable.EnumeratePixels`.

| Method | Bit depth | Output per input byte |
|---|---|---|
| `BufferTo1bppPixels` | 1 | 8 index values (MSB first) |
| `BufferTo2bppPixels` | 2 | 4 index values |
| `BufferTo4bppPixels` | 4 | 2 index values |
| `BufferTo8bppPixels` | 8 | 1 index value |
| `BufferTo16bppPixels` | 16 | 0.5 index values (2 bytes → 1 big-endian `int`) |
| `BufferTo24bppPixels` | 24 | ⅓ index values (3 bytes → 1 packed RGB `int`) |
| `BufferTo32BppPixels` | 32 | ¼ index values (4 bytes → 1 packed RGBA `int`) |

All methods handle odd-length buffers gracefully by padding missing bytes with `0`.

---

## `PartiallyKnownSize`

```csharp
public readonly struct PartiallyKnownSize(int? width, int? height)
```

A lightweight value type that represents an image size where **one or both dimensions may be `null`** (unknown). Used as the `size` parameter to `Drawer.DrawFixedSize` to let the drawer infer the missing dimension from the pixel count.

| Scenario | `Width` | `Height` | Behaviour in `DrawFixedSize` |
|---|---|---|---|
| Width only | non-null | `null` | Height = pixels ÷ Width |
| Height only | `null` | non-null | Width = pixels ÷ Height |
| Both known | non-null | non-null | Image exactly `Width × Height` |
| Neither | `null` | `null` | `ArgumentException` thrown |

Supports C# tuple deconstruction via a `Deconstruct` method.

---

## `DrawingMode`

Controls how pixels decoded from the stream are placed on the output image. Passed as the optional last argument to `Drawer.Draw`.

| Value | Description |
|---|---|
| `Raster` | Left-to-right, top-to-bottom (default scan order). |
| `Striped` | Vertical-stripe order: a stripe of `stripeWidth` columns is filled top-to-bottom, then the next stripe begins one `stripeWidth` to the right. |

The stripe width for `Striped` mode is determined automatically from the bit depth:

| Bit depth | Stripe width |
|---|---|
| 1 bpp | 8 px |
| 2 bpp | 4 px |
| 4 bpp | 2 px |
| ≥8 bpp | 1 px |

The stripe width for sub-byte depths equals the number of pixels packed into a single byte, so each stripe corresponds to exactly one column of bytes in the source data.

---

## `UniqueColorsImage<TPixel>`

```csharp
public sealed class UniqueColorsImage<TPixel>
    where TPixel : unmanaged, IPixel<TPixel>
```

A simple result container returned by `Drawer.UniqueColors`. 

| Property | Type | Description |
|---|---|---|
| `UniqueColors` | `int` | The number of distinct pixel values found |
| `Image` | `Image<TPixel>` | A compactly-packed image containing one pixel per unique colour |

---

## Typical Usage

```csharp
using var file = File.OpenRead("data.bin");

// Auto-sized image, 8-bpp grayscale
var image = Drawer.Draw(
    stream: file,
    bitDepth: 8,
    palette: DefaultPalettes.EightBppGrayscale,
    cancellationToken: CancellationToken.None,
    progress: null);

image.SaveAsPng("output.png");
```

```csharp
// Force a 1920-pixel-wide image, derive height
var image = Drawer.DrawFixedSize(
    size: new PartiallyKnownSize(width: 1920, height: null),
    stream: file,
    bitDepth: 24,
    palette: null,           // 24-bpp uses raw RGB, no palette needed
    cancellationToken: CancellationToken.None,
    progress: null);
```
