Okay, now we have the way to convert bytes to pixels in various formats, how do we make images out of them? Let's go over it all.
## The Input
- A list of file paths.
- A pixel layout, either **Raster** or **Striped**.
- A title mode, either **No Title Bars**, **One Title Bar**, **Title Bar per File**.
- A target:
	- **Single Image Automatic**
	- **Single Image Fixed Width**
	- **Single Image Fixed Size**
	- **Multiple Images to Folder**
	- **Zoomable Canvas**
The pipeline is split in two - the part that takes the bytes of each file and converts it to `Rgba32` instances given the additional input parameters like pixel format, color mode, etc. This document concerns the laying out of said pixels to the target. The pixel source looks like this:
```csharp
interface IPixelSource
{
	long TotalPixels { get; }
	long Position { get; }
	bool Finished { get; }
	
	int Read(Span<Rgba32> destination);
}
```
And, to account for multiple files, is presented like this:
```csharp
Dictionary<string, IPixelSource>
```
where the key is the file path. Let's take a look at each target and see the logic in sizing it all out. Sections make references to functions defined in the Psuedocode Functions section below.
## Input Preconditions
- Single Image Automatic
	- (none)
- Single Image Fixed Width
	- If the pixel layout is Striped, the width must be at least one stripe wide.
- Single Image Fixed Size
	- If title bars are enabled, the height must be at least 25 pixels.
	- If the pixel layout is Striped, the width must be at least one stripe wide.
- Multiple Images to Folder
	- If title bars are enabled, the height must be at least 25 pixels.
	- If the pixel layout is Striped, the width must be at least one stripe wide.
## Single Image Automatic
### No Title Bars
The simplest path, every pixel from every file can be dumped in order onto one big image. For the Raster pixel layout, sum all the pixels of all the files as `allPixels` and you can get the size from `automaticSizeRaster(allPixels)`.

For the Striped pixel layout, we can compute the `stripeWidth` integer from an internal table. The size is `automaticSizeStriped(allPixels, stripeWidth)`.
## One Title Bar
Also pretty simple. A title bar is 24 pixels tall (though this should be defined as a constant so it can changed later), so compute the size as if there was no title bar and then add 24 pixels to the height. The pixel data is then drawn beginning at `(0, 24)`.
### Title Bar per File
This is where things get tricky. What we want is a stack of alternating title bars and pixel blocks which is as wide as the widest pixel block naturally is. So we need a two-pass approach.
1. For each pixel count, use the automatic sizing functions to compute its natural size.
2. Take the width of the widest size as the variable `trueWidth`.
3. For each pixel count, use the `fixedWidthSizeRaster` or `fixedWidthSizeStriped` functions to recompute their new true sizes. Add 24 pixels to the height of each true size as room for the title bar.
4. The resulting width is `trueWidth` and the resulting height is the sum of the heights of the true sizes.
## Single Image Fixed Width
This creates an image as tall as it needs to be for a given width. The functions `fixedWidthSizeRaster` and `fixedWidthSizeStriped` below provide the way to get the resulting image size.
## Single Image Fixed Size
This is the one single image mode that will discard excess pixels. You specify the width and height, so that's what it is. If you want title bars, we'll fit them in, but when we're out of space, that's it. A title bar will not be drawn if there are fewer than 24 scanlines left to fit them on.
## Multiple Images To Folder
This mode is best thought of as a scanline-by-scanline mode. We can either be drawing a title bar or individual scanlines of pixel data. Title bars always take 24 scanlines, BUT we don't start drawing a title bar if we're less than 24 scanlines from the bottom of the current image, we leave the remainder black, save the current image, and go to the next.
### Zoomable Canvas
### No Title Bars
This is Single Image Automatic but bigger. The same `automaticSizeRaster` or `automaticSizeStriped` are used with the sum of the pixel counts of all pixel sources.
## One Title Bar
This is No Title Bars but 24 pixels taller. We do the same sizing as we do in No Title Bars mode, and then add 24 pixels of height to the result.
## Title Bar per File
So, we could reuse the same logic as in Single Image Automatic, but that won't look too good - a mix of big files with immense natural widths mixed with small files will result in said small files getting just 1 or 2 very VERY wide rows and being hard to see. So we're going to need a different layout.

Luckily, each pixel source is independently sized first. We use `automaticSizeRaster` or `automaticSizeStriped` to determine a size. Then we use ImageSharp's text measuring to figure out how long the title bar will be. We imagine a canvas on which we place the title bar at (0, 0), which expands it to `(titleWidth, 24)`. Then, given the height of the automatic sizing, we add that to this height to get `(titleWidth, 24 + pixelBlockHeight)`. We fill in this space with a shade of gray and center the pixel block in it, giving us a rectangle containing a title bar and a gray panel on which the pixels are written. We then add 2 pixels on the top, left, right, and bottom as a margin to get a final size of `(titleWidth + 4, 28 + pixelBlockHeight)`, the margin being darker gray.

That gets us the sizes per pixel source, but how do we lay them out? `Celarix.Imaging.Packing.Packer`, of course! It's built to take in a big list of sizes and lay them out semi-efficiently on an infinite canvas. This gets us a list of rectangles - how big each pixel source becomes, and where it goes.
## Actually Drawing It
Look. This one's tough.
### Single Image and Zoomable Canvas
#### The Layout Canvas
```csharp
class LayoutCanvas
{
	Size Size { get; }
}
```
The layout canvas is a fixed-size pixel grid that represents the total area of where the binary drawing result can be laid out upon. The canvas is solid white.
#### Blocks
```csharp
abstract class Block
{
	Guid Id { get; }
	BlockKind Kind { get; }
	Rectangle CanvasRectangle { get; }
}
```
A block is a block of pixels somewhere on the canvas, with an auto-generated ID. It has the following kinds:
- `TitleBar`: A 24-pixel tall block containing a title.
- `SolidColor`: A block containing nothing but one color.
- `RasterScanlines` : A block containing pixel data rendered in the Raster pixel layout.
- `StripedColumn`: A block containing a single column of pixel data rendered in the Striped pixel layout.
##### Small and Large Jobs
If the size of the layout canvas is 4096x4096 or less, the binary drawing job is considered **small**. This means:
- Everything is done in memory on one `Image<Rgba32>`.
- No job recovery or reentrancy is performed.
If the size of the layout canvas is larger, or the target was specified to be Zoomable Canvas, the binary drawing job is considered **large**. This means:
- The result will always be a zoomable canvas.
- It will be saved to disk in a user-specified folder.
- Inside the folder, a `staging` folder is created.
- Job JSON is saved for reentrancy in case of crashing.
#### Drawing Title Bars
```csharp
class LayoutCanvas
{
	void DrawTitleBar(string title, Point location, int maxWidth);
}
```
Draws a title bar at the given location with the desired maximum width. The width of the text is measured in ImageSharp, if it is too wide to fit, it is shrunken (see Title Shrinking below) until it does.

If, after shrinking, the title now is not as wide as `maxWidth`, the title bar is broken into two blocks - a `TitleBar` block and a `SolidColorBlock` which spans the rest of `maxWidth`.
#### Drawing Raster Pixel Data
```csharp
class LayoutCanvas
{
	RasterBlock MakeRasterBlock(Rectangle canvasRectangle);
}

class RasterBlock : Block
{
	bool TryDrawScanline(IPixelSource pixelSource);
}
```
Draws pixel data left to right, top to bottom. If all scanlines are filled, `TryDrawScanline` returns false.
#### Drawing Striped Pixel Data
```csharp
class LayoutCanvas
{
	RasterBlock MakeStripedBlock(Rectangle canvasRectangle, int stripeWidth);
}

class StripedBlock : Block
{
	bool TryDrawColumn(IPixelSource pixelSource);
}
```
Draws pixel data in stripes, top to bottom. If all columns are filled, `TryDrawColumn` returns false.
#### Drawing Solid Color Blocks
```csharp
class LayoutCanvas
{
	void MakeSolidColorBlock(Rectangle canvasRectangle, Rgba32 color);
}
```
This is used to draw the margins and the gray background for the zoomable canvas target.
#### Large Job Handling
This uses a different implementation of `LayoutCanvas` built to handle saving jobs and working with the file system. The user can provide a folder path to place the zoomable canvas in, or if none is provided, a temporary folder is created. Inside this folder, a `staging` folder is also created. Blocks are saved to disk in various formats, their file names are in the format `{guid}.ext` or `{guid}_{index}.ext` if more than one file per block is required. Index is a 0-indexed auto-incrementing integer.
The alternate implementations of the above types have the following differences:
- Title bars are still shrunk if needed, then split into the title bar block and a solid color block. The title bar block is saved as a PNG to disk.
- Solid color blocks have an extension of `.bin` and consist of just 4 bytes, an RGBA value.
- Raster blocks are saved in different ways depending on the width of the block.
	- Below 4,096 pixels wide, a single PNG file is saved for the whole block.
	- Between 4,097 and 65,536 pixels wide:
		- The block is split vertically into PNG images that span the full width of the block but only some of the height. They are split based on width:
			- 4097 to 8192: Up to 256 pixels tall
			- 8193 to 16384: Up to 128 pixels tall
			- 16385 to 32768: Up to 64 pixels tall
			- 32769 to 65536: Up to 32 pixels tall
	- Above 65,536 pixels wide:
		- The block is split vertically into files containing uncompressed, raw RGBA values with no header or padding, with a `.bin` extension. They are split based on width:
			- 65537 to 131072: Up to 16 pixels tall
			- 131073 to 262144: Up to 8 pixels tall
			- 262145 to 524288: Up to 4 pixels tall
			- 524289 to 1048576: Up to 2 pixels tall
			- Above 1048577: 1 pixel tall
- Striped blocks are saved column-by-column.
	- A single column of pixels is saved in the raw RGBA format. It is `stripeWidth * columnHeight * 4` bytes in size. The order is left to right.
#### Large Job Rendering
To convert the files in the `staging` folder into a proper zoomable canvas, we start by determining the tile ranges of each block. Each of the four corners are converted to tile coordinates. We use the size of the layout canvas to determine how many tiles it consists of, then use rectangle intersection to get a set of blocks intersecting each tile.
For each tile, we initialize an image to white and look at the set. For each intersecting image, we compute the intersection and the location to draw it at such that the proper portion of the image will be drawn at the correct location. We draw each block as follows based on its file type:
- PNG image: Loaded as-is and drawn normally.
- Solid color image: The rectangle fill operation is used to fill that portion of the tile.
- Raw RGBA32 samples: The bytes of the file are loaded into a `Rgba32[,]` array in memory. We compute the intersection, giving us a rectangle to copy out of the file. We create another, smaller, `Rgba32[,]` instance, copy the values over, and then loop over the tile's pixels, setting them.
### Multiple to Folder
This is a slightly different mode on account on striped pixel layouts. We can think of this like a scanline sink - draw either 24-pixel tall title bars or draw individual scanlines of pixel data, until the current image is filled. We then save the image to disk and initialize a new one.

Striped pixel layout complicates this a bit. What we want to do is take whatever portion of the height of the current image that remains and treat it like a fixed height area. For instance, if we have only 3 pixels left, we can draw 3 stripes in the first column, then 3 in the next, and so forth.

```csharp
class MultipleToFolderSink
{
	Size ImageSize { get; }
	void DrawTitleBar(string text);
	void DrawRasterScanline(IPixelSource source);
	MultipleToFolderStripedBlock MakeStripedBlock(IPixelSource source, int stripeWidth);
}
```
- `DrawTitleBar` will save out the current image and start a new one if it doesn't have 24 scanlines of space left. It also shrinks the title if it doesn't fit.
- `MakeStripedBlock` will check to see if there is enough room for the entire source to be consumed and will only hand you a block big enough to do that. If not, it hands you a block representing the remainder of the current image. You can check `IPixelSource.Finished` to see if you need to keep going on the next image. You may need multiple images if there's enough data!
- By contrast, `DrawRasterScanline` you can just call in a loop until `IPixelSource.Finished` is true.
This sink also supports reentrancy and saving job files.
## Psuedocode Functions
```javascript
function automaticSizeRaster(pixels) {
	const w = Math.ceil(Math.sqrt(pixels));
	const h = Math.ceil(pixels / w);
	return Size(w, h);
}

function automaticSizeStriped(pixels, stripeWidth) {
	const stripes = pixels / stripeWidth; // will be an integer; pixel source MUST return enough pixels to be divisible by stripeWidth
	const h = Math.ceil(Math.sqrt(stripes));
	const w_s = Math.ceil(stripes / h);
	return Size(w_s * stripeWidth, h);
}

function fixedWidthSizeRaster(pixels, width) {
	const h = Math.ceil(pixels / w);
	return Size(width, h);
}

function fixedWidthSizeStriped(pixels, stripeWidth, width) {
	if (width < stripeWidth) {
		Error("Too narrow!");
	}
	
	const stripesPerRow = Math.floor(width / stripeWidth);
	const stripes = pixels / stripeWidth;
	const h = Math.ceil(stripes / stripesPerRow);
	return Size(width, h);
}
```
## Title Shrinking
We want to draw a title bar (really just some text) in a limited width. If it fits, great! If not, here's what to do.
1. While the text does not fit when measured,
	1. Find the longest path segment that is not the first or last (i.e. `C:\path\to\file.txt` has segments `path` and `to` we can work with).
	2. If the segment is longer than 3 characters, replace it with `...`. If it is shorter, break and go to step 2.
2. If the text now fits, return the shrunk title.
3. While the text does not fit when measured,
	1. Find adjacent path segments that are both `...`.
	2. Replace the pair of segments with one `...`.
4. Return whatever we got. If it's still too wide, oh well.