# Outline for `Celarix.Imaging.BinaryDrawing.V2`

An outline for a new implementation of the binary drawing code. This code converts any sequences of bytes into pixels and lays them out on images according to configuration options provided by the user or caller. This code is envisioned as a pipeline with replacable components.

## Components

### Byte Source

A byte source wraps one or more `Stream` objects with optional name(s), often the name of the file(s) that was used to make it. The byte source is responsible for providing the bytes to the pipeline. It has three states: **default**, **stream boundary**, and **end of streams**. The byte source exposes stream-like methods:

- `Read`: Reads bytes into a buffer, returning the number of bytes read. When reaching a stream boundary, it will only return up to the end of that stream. The caller can then check the state of the source to see if there are more bytes to read and also the new name of the next stream if it's using names.
- `Seek`: Seeks to a specific position in the stream. This is supported only if all internal streams are seekable. This treats all streams as if concatenated and will internally seek to the correct stream and position within it.
- `Position`: Returns the current position in the stream.
- `Length`: Returns the total length of the stream.
- `Name`: Returns the name of the current stream.
- `State`: Returns the current state of the byte source.

Methods for writing are not supported. Internally, the byte source either holds on to fully constructed `Stream` objects or, when possible, uses a lazy stream that consists of a lambda that constructs a stream on demand, along with the length of that stream and its position in the concatenated stream. The lazy stream approach is used when the caller specifies file paths to use. If the byte source moves to another stream and the old stream is a lazy stream, the underlying stream of the lazy stream will be closed and disposed.

### Image Layout

Here, we must take a detour and talk about our pixel formats, output formats, and options, regardless of how the code will do these things. The binary drawing code converts bytes to pixels and lays them out on images. The options available to the user are as follows:

- **Pixel Format**: Specifies how the bytes are converted into pixels. More on this in the Pixel Format section.
- **Output Type**: Specifies what the output of the pipeline will be:
    - **Single Image**: An `Image<RGBA32>` ImageSharp image instance that can be saved to disk or displayed directly.
    - **Zoomable Canvas**: A set of tile images that can be used to construct a zoomable canvas. More on this in the Zoomable Canvas section.
    - **Fixed Size To Folder**: A set of images that are all the same size, saved to a folder. Each image represents one part of the byte source, and will be named `image{D8}.png` starting from 0.
- **Stream Name Printing**: An enumeration that specifies the level of stream names and metadata that will be output to the image(s) as a 24-pixel tall strip within the image(s).
    - **None**: No name information is printed.
    - **TopOfImage**: The name of the stream represented by the first data pixel is printed along the top of each image (for Single Image and Fixed Size to Folder output types), or along the top of the entire zoomable canvas if Zoomable Canvas is the output type. If multiple streams are represented in a single image, a count of the additional streams is included (i.e. `C:\files\file1.bin +11`)
    - **TopOfStream**: Each stream receives a 24-pixel black line containing its name printed in white text, with its pixel data immediately below it. The pixel directly beneath the 24-pixel line at X = 0 represents the first bits or bytes of that stream, and pixels are laid out until the end of the image or the end of the stream. For Fixed Size to Folder, if the stream ends without room for at least 25 more pixels on the current image (24 pixels for another title plus 1 row of pixels), the remaining rows will be left black and the next image will begin the new stream. Single Image and Zoomable Canvas always allow the stream to continue on the same image until the end of all streams in the byte source. Note that choosing this option has a few impacts:
        - The overall width and height of the image is determined by the widest width and tallest height of any stream in the byte source. Thus, many streams will be laid out much wider than they would be on their own.
        - In Fixed Size to Folder, whenever a new image is started, a 24-pixel line is always drawn showing the current stream's name, even if that stream had already begun laying out pixels on the previous image.
- **Stream Name Level**: An enumeration that specifies how much data is shown on the stream name line.
    - **NameOnly**: Only the name of the stream is printed. If the name contains path separator characters AND it is too long to fit in the width provided, the longest substrings between path separators (excluding the substring before the first path separator and the substring after the last path separator) will be replaced with `...` until the name fits in the width. If the name does not fit even after this, middle segments of the path will be removed until it does. (i.e. `C:\...\...\...\file.txt`). If it's STILL not wide enough, the text is printed anyway, with cropping happening.
    - **NameAndSize**: First, the file's size is printed right-aligned on the width of the image and receives a dark blue background color to fit its size. The size is displayed in bytes using IEC base-1024 unit prefixes with two digits of precision past the decimal point (i.e. `259 bytes`, `109.09 KiB`, etc.) The name is then printed to the left, with the narrower width remaining to be used as the width for path segment replacement and truncation. If the remaining space is just not wide enough even after truncation, the name is printed and allowed to crop, but it never overlaps the size text's portion.
    - **NameAndProgress**: Supported only by Fixed Size to Folder, setting this on any other mode is equivalent to name only. When a new image begins mid-stream, a portion of the black background of the 24-pixel line will be colored dark green in accordance to the percentage of the stream that has already been laid out on previous images. The name is then printed normally in white with proper path segment replacement and truncation, using the full width of the 24-pixel line.
    - **NameProgressAndSize**: Supported only by Fixed Size to Folder, setting this on any other mode is equivalent to name only. When a new image begins mid-stream, a portion of the black background of the 24-pixel line will be colored dark green in accordance to the percentage of the stream that has already been laid out on previous images. The size is then printed right-aligned on the width of the image and receives a dark blue background color to fit its size. The size is displayed in bytes using IEC base-1024 unit prefixes with two digits of precision past the decimal point (i.e. `259 bytes`, `109.09 KiB`, etc.) The name is then printed normally in white with proper path segment replacement and truncation, using the remaining width to the left of the size. If the remaining space is just not wide enough even after truncation, the name is printed and allowed to crop, but it never overlaps the size text's portion.
- **Pixel Order**: Specifies the order in which pixels are laid out in the data sections of the image (that is, sections not in the 24-pixel lines.)
    - **Raster**: Pixels are laid out in a raster scan pattern, left-to-right, top-to-bottom.
    - **Stripe**: For 1 BPP, 2 BPP, 4 BPP, single-precision float, and double-precision float, a stripe width in pixels is defined. Setting this mode lays out all the pixels in one stripe and then moves down a row and lays out the pixels in the next stripe. Each stripe is one pixel tall and represents either 1 byte (for 1, 2, or 4 BPP) or 4/8 bytes (for the floating point formats). Stripes continue to the bottom of the image and wrap to the top, one stripe over. This uses a different sizing method in Single Image and Zoomable Canvas than Raster, please see Sizing below.

### Palettes

A palette is an array of RGBA32 colors, where each color uses 8 bits per channel. Palettes can be specified with many of the pixel formats as long as they use 16 bits per pixel or fewer. The user may provide their own palette, or choose a default palette, in the options object that is passed to the API.

Default palettes are either hardcoded or use a method to compute grayscale or color channel ramps. The default palette kinds are:

- **Black and white**: A hardcoded palette of #000000FF and #FFFFFFFF.
- **Grayscale**: A ramp of grayscale values.
- **RGB**: An RGB color space produced by three ramps, often of different bit widths, one per channel.
- **RGBA**: An RGBA color space produced by four ramps, often of different bit widths, one per channel.

A ramp is defined using the number of bits that make it up, which we'll call n. The ramp operates in steps of 1 / (n - 1). That is, for a 4-bit ramp with 16 possible values, the steps will be in fifteenths. The first color is always black, then the steps are 1/15th, 2/15ths, etc. up to 15/15ths, which is full intensity. To compute the value at each step, simply divide 256 by it and always round up (in this example, 256/15 = 17.066..., so the steps are 0, 18, 35, 53, 71, 89, 106, 124, 142, 159, 177, 194, 212, 229, 247, and 255.)

### Pixel Format

The following pixel formats are defined, specifying how bytes are converted into pixels. Pixels start from the most significant bits of each byte and are in big endian format - that is, the bytes 0x11, 0x22, and 0x33 in 24 BPP mode will produce the pixel #112233.

- **1 bit per pixel**: Each pixel is made from one bit, and 8 pixels make up a byte. The pixels are produced in order from the MSB to the LSB (i.e. `01234567`). 0 bits use palette color #0, 1 bits use palette color #1. The default palette is black and white. The stripe width is 8 pixels.
- **2 bits per pixel**: Each pixel is made from groups of two bits called bit pairs, and 4 pixels make up a byte. The pixels are produced in order from the MSB to the LSB (i.e. `00112233`). 00 bits use palette color #0, 01 bits use palette color #1, 10 bits use palette color #2, and 11 bits use palette color #3. The default palette is black and white. The stripe width is 4 pixels.
- **4 bits per pixel**: Each pixel is made from groups of four bits called nybbles, and 2 pixels make up a byte. The pixels are produced in order from the MSB to the LSB (i.e. `00001111`). Each nybble is, as by now it should be clear, treated as the index into the palette. The stripe width is 2. There are three default palettes:
    - A grayscale ramp in steps of 1/15ths
    - An RGB 1:2:1 color space where 1 bit is used for red and blue (thus, they can be either black or full intensity), and 2 bits are used for green (thus, green is a ramp in steps of 1/3rds)
    - An RGBA 1:1:1:1 color space where 1 bit is used for each channel. Thus, each channel can only be black/transparent or full intensity/opaque.
- **8 bits per pixel**: Each pixel is made from one byte. The stripe width is 1. There are three default palettes:
    - A grayscale ramp in steps of 1/255ths
    - An RGB 3:3:2 color space where 3 bits are used for red and green (thus, they can be either black or full intensity), and 2 bits are used for blue (thus, blue is a ramp in steps of 1/3rds)
    - An RGBA 2:2:2:2 color space where 2 bits are used for each channel. Thus, each channel is a ramp in steps of 1/3rd.
- **16 bits per pixel**: Each pixel is made from two bytes. The byte order is big endian. The stripe size is 1. There are two default palettes:
    - An RGB 5:6:5 color space where 5 bits are used for red and blue (thus, they are ramps in steps of 1/31sts), and 6 bits are used for green (thus, green is a ramp in steps of 1/63rds).
    - An RGBA 4:4:4:4 color space where 4 bits are used for each channel. Thus, each channel is a ramp in steps of 1/15th.
- **24 bits per pixel**: Each pixel is made from three bytes. The byte order is big endian. The stripe width is 1 pixel. There are two default palettes:
    - An RGB 8:8:8 color space where 8 bits are used for each channel. Thus, each channel is a ramp in steps of 1/255ths. **This is the most common RGB bit depth for modern images.**
    - An RGBA 6:6:6:6 color space where 6 bits are used for each channel. Thus, each channel is a ramp in steps of 1/63rds.
- **32 bits per pixel**: Each pixel is made from four bytes. The byte order is big endian. The stripe width is 1 pixel. There is only one default palette, RGBA 8:8:8:8, where 8 bits are used for each channel. **This is most common RGBA bit depth for PNG images.**
- **Single-precision float**: This is a format that doesn't directly map bytes to pixels. Instead, each 4 bytes in big endian order map to a sequence of pixels as follows:
    - A guard pixel.
    - A pixel representing the mantissa. Single-precision floats use a 23-bit mantissa with an implied 24th bit, either 1 for normal numbers or 0 for subnormal numbers. The implied leading bit is followed by the mantissa, and these 24 bits together produce one RGB 8:8:8 pixel.
    - A guard pixel.
    - A pixel representing the exponent. The exponent size is 8 bits, so one RGB 3:3:2 pixel is used to represent it.
    - A final guard pixel. The guard pixels for this format and double-precision floats have these colors:
        - Light gray #A0A0A0 is used for normal numbers.
        - Dark gray #303030 is used for subnormal numbers.
        - Green #00FF00 is used for infinities.
        - Red #FF0000 is used for NaN.
- **Double-precision float**: This is a format that doesn't directly map bytes to pixels. Instead, each 8 bytes in big endian order map to a sequence of pixels as follows:
    - A guard pixel.
    - A pixel representing the high portion of the mantissa. Double-precision floats use a 52-bit mantissa with an implied 53rd bit, either 1 for normal numbers or 0 for subnormal numbers. The implied leading bit is followed by the mantissa, and these 53 bits together produce two pixels. This first pixel represents the top 24 bits of the full mantissa in RGB 8:8:8 format.
    - A pixel representing the middle portion of the mantissa. This pixel represents the next 24 bits of the full mantissa in RGB 8:8:8 format.
    - A pixel representing the low portion of the mantissa. This pixel represents the last 5 bits as an RGB 2:2:1 pixel (thus, red and green are ramps in 1/3rd steps and blue is either black or full blue).
    - A guard pixel.
    - A pixel representing the exponent. The exponent size is 11 bits, so an RGB 4:5:3 pixel is used to represent it (red is a ramp in steps of 1/15ths, green a ramp in steps of 1/31sts, blue a ramp in steps 1/7ths)
    - A final guard pixel. The guard pixels for this format use the same colors as the single-precision floats.

### Sizing

Determining the output size is a matter both of the specified options and the input data. The user may specify the following sizing options to help determine how the size of the image is determined:

- **Standard For Layout**: Uses either the square-root method or the stripe-sizing method specified below, depending on if the layout is Raster or Striped, respectively.
- **Fixed Width**: Specifiable for Single Image or Zoomable Canvas modes. This fixes the width of the output and lets the height grow to the required size to fit all pixels. If this is specified on Fixed Size To Folder, it is ignored and treated as Standard For Layout instead. If the layout is Striped, this width is rounded up to the nearest multiple of the stripe size.
- **Fixed Size**: Specifiable for all modes. On Fixed Size To Folder, this specifies how large each output image should be. If the layout is striped, the width is rounded up to the nearest multiple of the stripe size. On Single Image and Zoomable Canvas modes, this restricts the final size of the image - if there are more bytes after all pixels are filled, tough.

We can map the number of bytes to the number of pixels using the following formulae. This can be done for individual streams in the byte source or all streams together:

- 1 BPP: `length * 8`
- 2 BPP: `length * 4`
- 4 BPP: `length * 2`
- 8 BPP: `length`
- 16 BPP: `ceiling(length / 2)`
- 24 BPP: `ceiling(length / 3)`
- 32 BPP: `ceiling(length / 4)`
- Single-precision float: `ceiling(length / 4) * 5`
- Double-precision float: `ceiling(length / 8) * 7`

From there, the exact sizing depends on how many 24-pixel lines will be needed. Let's take a look at each case.

### For None

If no 24-pixel lines are needed, the output size is computed as follows:

- Standard for Layout
    - Raster
        - Single Image or Zoomable Canvas: The square root method is used as described below.
        - Fixed Size To Folder: A default size of 1920x1080 is used.
    - Striped
        - Single Image or Zoomable Canvas: The stripe-sizing method is used as described below.
        - Fixed Size To Folder: A default size of 1920x1080 is used. 1920 pixels wide is divisible by all stripe widths except for double-precision floating point (stripe size of 7), so in this case, a size of 1925x1080 is used instead.
- Fixed Width
    - Raster
        - Single Image or Zoomable Canvas: The total number of pixels for the entire byte source is computed, let it be called `P`. Then we take `P / width` and round up to the nearest integer. Let this be `H`. The output size is then `width x H`.
        - Fixed Size To Folder: The width is used as the image's width and a default of 1080 is used as the height.
    - Striped
        - Single Image or Zoomable Canvas: The stripe-sizing method is used as described below.
        - Fixed Size To Folder: The width is used as the image's width and a default of 1080 is used as the height. If the width is not divisible by the stripe size, it is rounded up to the nearest multiple of the stripe size.
- Fixed Size
    - Raster
        - Single Image or Zoomable Canvas: The output size is `width x height`.
        - Fixed Size To Folder: The output size for each image is `width x height`.
    - Striped
        - Single Image or Zoomable Canvas: The output size is `width x height`.
        - Fixed Size To Folder: The output size for each image is `width x height`.

### For TopOfImage

Only one 24-pixel line is needed here. Rather than restate all the sizing rules, know that they are the same as None except

- For Single Image and Zoomable Canvas modes, an extra 24 pixels are added to the top of the image.
- For Fixed Size To Folder, the same default sizes are used, but the top 24 pixels now offset the image data, leaving 1056 rows for it.
- For Fixed Size + Fixed Size To Folder, the minimum image height is 25 pixels, to allow at least some pixel data to be saved. There is no minimum for Single Image or Zoomable Canvas - if you specify a height under 25 pixels, you just won't get any image data.

### For TopOfStream

This one is trickier - each stream gets its own new 24-pixel line. Mostly, this involves looking at the number of bytes in each individual stream and converting those to pixel counts. Let's take a look at each method:

- Standard for Layout
    - Raster
        - Single Image or Zoomable Canvas: The square root method is used on the pixel counts on each individual stream of the byte source. Of all of them, the width is set to the width of the widest of all sizes. Then, for each stream, we compute the size it would be for that width first. For instance, a stream might naturally produce a 30x30 image but when laid out on a width of 900, it becomes 900x1. Then, we use the width, compute 24 * the number of streams to account for the 24-pixel lines, then add the sum of all recomputed heights as the height.
        - Fixed Size To Folder: A default size of 1920x1080 is used. Each 24-pixel line takes 24 pixels away from data, as expected.
    - Striped
        - Single Image or Zoomable Canvas: on the pixel counts on each individual stream of the byte source. Of all of them, the width is set to the width of the widest of all sizes. The same height recomputation method is used as in the raster case, but with the stripe width instead of the image width.
        - Fixed Size To Folder: A default size of 1920x1080 is used. 1920 pixels wide is divisible by all stripe widths except for double-precision floating point (stripe size of 7), so in this case, a size of 1925x1080 is used instead. Each 24-pixel line takes 24 pixels away from data, as expected.
- Fixed Width
    - Raster
        - Single Image or Zoomable Canvas: This works much the same as in the Standard for Layout path, except that the width is pre-specified instead of computed from the streams.
        - Fixed Size To Folder: The width is used as the image's width and a default of 1080 is used as the height.
    - Striped
        - Single Image or Zoomable Canvas: This works much the same as in the Standard for Layout path, except that the width is pre-specified instead of computed from the streams.
        - Fixed Size To Folder: The width is used as the image's width and a default of 1080 is used as the height. If the width is not divisible by the stripe size, it is rounded up to the nearest multiple of the stripe size.
- Fixed Size
    - Raster
        - Single Image or Zoomable Canvas: The output size is `width x height`.
        - Fixed Size To Folder: The output size for each image is `width x height`.
    - Striped
        - Single Image or Zoomable Canvas: The output size is `width x height`.
        - Fixed Size To Folder: The output size for each image is `width x height`.

### The Square Root Method

- Given a number of pixels `P`, compute its square root.
- Take the floor of that and that is used as the width.
- The height is equal to that floor, and as long as `width * height < P`, increment the height by 1.

### The Stripe-Sizing Method

- Given a number of pixels `P` and a stripe width `W`, compute `P / W` and round up to the nearest integer. Let this be `H`. If this were to be laid out on a `W * H` image, it would be very tall and not very wide. So, instead, compute the square root of H as a column height.
- The floor of that square root becomes the height of the image. Then, the number of columns is computed by taking `(P / W) / height` and rounding up to the nearest integer. The width is then `columns * W`.

## Notes on Zoomable Canvases

The Single Image and Fixed Size To Folder modes aren't too complicated - they are a single image at a time, all stored in memory. Except big files can make BIG images. Fixed Size to Folder easily breaks this into chunks, but Zoomable Canvas lets a better view exist. But how does it work?

Consider the platonic full image. Let's put some numbers to this and say it is a binary drawing of a single file in raster mode, using 32-bit RGBA 8:8:8:8, with no 24-pixel lines. The underlying file is 1 gigabyte in size. That's a lotta pixels - at 4 bytes per pixel, it's 250 megapixels. Using the square-root method, we get dimensions of 15811 * 15812 - far too large to store in memory.

A zoomable canvas works by taking the entire image and breaking into tiles. A tile is a square image of a specified edge length, by default 1,024 pixels. This giant image is broken into a grid of tiles 16 across and 16 down - 256 tiles in total. These are stored as plain PNGs on disk in a particular folder structure. If the canvas is contained in `/root`, the tiles are stored in `/root/0/y/x.png` for each tile Y coordinate `y` and each tile x coordinate `x`, zero indexed. Thus, there'd be 16 folders `0` through `15` and 16 files per folder `0.png` through `15.png`.

But what's that extra 0? That represents the zoom level. Once all 256 tiles of the 16x16 grid are generated, we can then generate a new grid of tiles that are 8x8, each tile representing 2,048 pixels on a side but still being stored as 1024x1024. These are stored in `/root/1/y/x.png`. The method of generating higher zoom levels is to take each 2x2 grid of tiles from the previous zoom level, lay them out on a 2048x2048 image, then shrink the image to 1024x1024 to create a single tile for the next zoom level. This process continues until we get to a zoom level where the entire image fits on a single tile. For our example image, we get:

- Zoom level 0: 256 16x16 tiles, each representing 1024x1024 pixels
- Zoom level 1: 64 8x8 tiles, each representing 2048x2048 pixels
- Zoom level 2: 16 4x4 tiles, each representing 4096x4096 pixels
- Zoom level 3: 4 2x2 tiles, each representing 8192x8192 pixels
- Zoom level 4: 1 1x1 tile, representing 16384x16384 pixels

The total number of tiles here is 341. To view the canvas, I use a basic webserver hosting an OpenSeadragon viewer. The viewer is configured to display the tiles in the specified folder structure, and it allows the user to zoom in and out of the image.

For binary drawing, the basic idea is that, instead of drawing to a single image, we draw to an abstraction that handles loading and saving then unloading modified images at zoom level 0. Then, when the drawing process completes, an existing method is invoked to generate higher zoom levels.

Optimization techniques are going to have to come into play here - more than just row-major vs. column-major, bad algorithms can switch tiles being drawn to repeatedly or seek incessantly in the byte source, causing files to load and unload from disk.