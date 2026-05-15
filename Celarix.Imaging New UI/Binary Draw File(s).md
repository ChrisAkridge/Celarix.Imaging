Draws an image of pixels using the bytes in one or more files specified by the user.
- Files: Specifies the files to use to draw the file, in the order to be drawn in.
- [[Pixel Formats|Pixel Format]]: Specifies the pixel format to use.
	- Binary modes: 1, 2, 3, 4, 8, 16, 24, and 32 bits per pixel
	- IEEE-754 half-, single-, and double-precision floating point visualizations
	- Text map format, as in File Analysis
		- Optional hue-wheel background
		- Encoding modes highlight bad characters
- [[Color Modes|Color Mode]]: Specifies the color mode to use. Not all color modes are supported by all formats.
	- Grayscale (1, 2, 3, 4, and 8 bpp)
	- RGB (3, 4, 8, 16, and 24 bpp)
	- RGB + Alpha (4, 8, 16, 24, and 32 bpp)
	- User palette (1, 2, 3, 4, 8, and 16 bpp)
- [[Pixel Layout]]: Specifies the way to draw the pixels to the image.
	- Raster: Pixels are drawn left-to-right, top-to-bottom.
	- Striped: Stripes are drawn left-to-right, then top to bottom. Once a single column of stripes is drawn, stripes start on the next column.
- [[Size Options]]: Lets the user pick a fixed width, fixed size, or automatic sizing.
	- Automatic: Image is sized automatically.
	- Fixed Width: Image computes its height given a width.
	- Fixed Size: Image's size is specified by the user. If it is too small to fit all pixels and the Target is not Multiple to Folder, extra pixels are ignored.
- [[Title Mode]]: Specifies if and where to draw title bars.
	- No title bars
	- One title bar
	- One title bar per file
- Target: Specifies where the image is drawn to.
	- Single image: Image is shown in app. Struggles with large amounts of bytes.
	- Zoomable canvas: Image is drawn to a zoomable canvas in a specified folder. Uses an intermediary batch-of-scanlines format to reduce I/O operations.
	- Multiple to Folder: Together with Fixed Size, saves images one at a time to a folder, allowing processing immense numbers of bytes easily.
## Binary Drawing Pipeline
To simplify handling so many different options, I think a new version of the main binary drawing code is in order. Our input is one or more files, represented as a stream of bytes, but more than that, the stream is aware of which bytes the file is currently coming from. Thus, when we read bytes, we don't merely have the states of EoF/not EoF, but rather:
- Not end of file: More bytes exist in the current file
- End of file: A file has ended, but another one has begun; check the name if you need it
- End of stream: All files have ended.
The streams we use here don't need to implement `Stream` as they really only have a few things we care about: reading bytes, what the current name is, and position and length. Specifically, we actually get two kinds of things we read: bytes and pixels.
`NamedStream<T>` becomes the type that we use in these two phases, with the following signature:
```csharp
class NamedStream<T>
{
	string CurrentFileName { get; }
	int FileNumber { get; }
	int TotalFiles { get; }
	long Position { get; }
	long TotalLength { get; }
	NamedStreamReadResult Read(T[] buffer, out int elementsRead);
}
```
We just try to fill `buffer` after default-initializing every element in `Read`.  The two types in use are `byte` and `Rgba32`, and we can read into one buffer and immediately convert to pixels in another buffer, offload them to the image, update progress, log, check cancellation token, etc. The buffer sizes do depend on the Pixel Format:
- Binary formats
	- 1bpp: 1024 bytes, 8192 pixels
	- 2bpp: 1024 bytes, 4096 pixels
	- 3bpp: 768 bytes, 2048 pixels
	- 4bpp: 1024 bytes, 2048 pixels
	- 8bpp: 1024 bytes, 1024 pixels
	- 16bpp: 1024 bytes, 512 pixels
	- 24bpp: 768 bytes, 256 pixels
	- 32bpp: 1024 bytes, 256 pixels
- Decimal formats are more complicated to handle, especially since we have to effectively read backwards. This might be worth an entirely separate pipeline so long as callers see the same interface.
- Floating point formats:
	- Half: 1024 bytes, $512 \cdot 6 = 3072$ pixels
	- Single: 1024 bytes, $256 \cdot 6 = 1536$ pixels
	- Double: 1024 bytes, $128 \cdot 8 = 1024$ pixels
- Text maps at least can be read forward, but they likely need their own pipeline that feeds line-by-line.
### The Steps
1. Is the pixel format set to text map?
	1. Yes: go to [[Text Map Setup]].
	2. No: continue.
2. What is the target?
	1. Single file: Go to [[Get Single Image Size]].
	2. Zoomable canvas:
	3. Multiple to Folder