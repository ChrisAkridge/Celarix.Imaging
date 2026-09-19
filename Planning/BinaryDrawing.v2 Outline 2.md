# Outline for `Celarix.Imaging.BinaryDrawing.V2`

A pipeline converting byte sequences into pixels and laying them out on images based on configuration.

## Components

### Byte Source
Wraps one or more `Stream` objects to provide bytes to the pipeline.
- **States**: default, stream boundary, end of streams.
- **Methods**: `Read` (stops at boundaries, callers can check and continue if not at end), `Seek` (if supported), `Position`, `Length`, `Name`, `State`. 
- **Optimization**: Uses lazy streams to open/close files on demand.

### Image Layout & Options
- **Pixel Format**: Defines how bytes map to pixels (see below).
- **Output Type**:
  - **Single Image**: One `Image<RGBA32>` output.
  - **Zoomable Canvas**: Tile hierarchy for large images.
  - **Fixed Size To Folder**: Sequential equally-sized images (`image{D8}.png`).
- **Stream Name Printing** (24-pixel header rows):
  - **None**: No headers.
  - **TopOfImage**: Drawn once at the top of the canvas/image. 
  - **TopOfStream**: Drawn for each stream right above its data. Sets minimum canvas width to the widest stream.
- **Stream Name Level** (Header content):
  - **NameOnly**: Path string with middle-truncation (`...`) if needed.
  - **NameAndSize**: Shows formatted size (right) and name (left). Size is laid on #0000A0. Size uses IEC prefixes and two decimal digits of precision.
  - **NameAndProgress** *(Fixed Size To Folder only)*: Green background fill indicating stream progress.
  - **NameProgressAndSize** *(Fixed Size To Folder only)*: Size + name + progress formatting.
- **Pixel Order**:
  - **Raster**: Left-to-right, top-to-bottom.
  - **Stripe**: 1-pixel-tall horizontal segments per byte/float, wrapping to the next column.

### Palettes
Arrays of RGBA32 colors for formats <= 16 BPP. Users define custom palettes or use defaults:
- **Black and White**: #000000FF and #FFFFFFFF.
- **Grayscale**: A ramp of grayscale values.
- **RGB** / **RGBA**: Color spaces produced by 3 or 4 ramps of varying bit widths.
- **Ramp Calculation**: For an *n*-bit channel, there are $2^n - 1$ steps from black to full intensity. For a given step, its 8-bit channel value is calculated by dividing 256 by the total number of steps and rounding up. (e.g., a 4-bit channel has 15 steps; `256 / 15 ≈ 17.06`, so step values increment by 17 or 18).

### Pixel Formats
Maps Big Endian bytes to pixels.
- **Integer formats** (default palettes noted):
  - **1 BPP** (8px/byte): Black & White. Stripe width 8.
  - **2 BPP** (4px/byte): Grayscale. Stripe width 4.
  - **4 BPP** (2px/byte): Grayscale, RGB 1:2:1, RGBA 1:1:1:1. Stripe width 4.
  - **8 BPP** (1px/byte): Grayscale, RGB 3:3:2, RGBA 2:2:2:2. Stripe width 2.
  - **16 BPP** (1px/2 bytes): RGB 5:6:5, RGBA 4:4:4:4. Stripe width 2.
  - **24 BPP** (1px/3 bytes): RGB 8:8:8, RGBA 6:6:6:6. Stripe width 1.
  - **32 BPP** (1px/4 bytes): RGBA 8:8:8:8. Stripe width 1.
- **Single-Precision Float (4 bytes)**: Guard | 24-bit Mantissa (1x RGB 8:8:8 px) | Guard | 8-bit Exponent (1x RGB 3:3:2 px) | Guard. Stripe width 5.
- **Double-Precision Float (8 bytes)**: Guard | 53-bit Mantissa (2x RGB 8:8:8 px, 1x RGB 2:2:1 px) | Guard | 11-bit Exponent (1x RGB 4:5:2 px) | Guard. Stripe width 7.
- *Guard colors*: Normal (Light Gray), Subnormal (Dark Gray), Info/Infinity (Green), NaN (Red).

### Sizing Rules
Calculates output dimensions based on constraints and pixel counts (`P = pixel count`).
- **Standard**: Calculates optimized dimensions based on layout.
  - *Raster (Square Root)*: Width = `floor(sqrt(P))`, Height = `ceil(P/Width)`.
  - *Stripe*: ColHeight = `floor(sqrt(ceil(P/StripeWidth)))`, Width = `cols * StripeWidth`.
- **Fixed Width**: Height scales to accommodate $P$ within width.
- **Fixed Size**: Fixed bounds. Excess pixels are discarded.
- *Headers*: Allocates extra 24px rows for `TopOfImage` and `TopOfStream` as needed.

## Zoomable Canvas
An approach for rendering massive outputs without memory limits.
- Breaks image into 1024x1024 PNG tiles.
- Saved in `/root/{zoomLevel}/y/x.png`.
- **Level 0**: Base resolution.
- **Level N**: Each tile is created by scaling down a 2x2 grid from Level N-1. Code exists already to create higher zoom levels from a finished zoom level 0.
- Requires optimized drawing logic to minimize file thrashing when rendering across boundaries.
