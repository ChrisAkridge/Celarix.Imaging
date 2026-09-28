Given a final size of the single image:
- Are both width and height less than 4,095 pixels OR the sizing mode is fixed size?
	- Yes: Prepare an `Image<Rgba32>` of that size and **proceed to**
	- No: Continue.

In order to still be able to show enormous single images in the application, we split it into stripes (not to be confused with striped pixel layout mode). For raster mode, we draw horizontal stripes, and for striped mode, we draw vertical stripes.

Stripes are saved as PNG files in a temporary folder. The PNGs have the file name format `{x}_{y}_{w}{h}.png`, indicating where they need to be laid out on the infinite canvas. The PNGs are one of two types: title bars or pixel stripes.
## Title Bars
Title bars have the constant title bar height as their height. Each PNG can be up to 4096 pixels wide. If the title bar is wider than that, it is split into 4096-pixel chunks. The title can spill into the next chunk, but this is vanishingly unlikely as it would need an enormously long file name well beyond MAX_PATH.
## Pixel Stripes
These are horizontal (raster) or vertical (striped) blocks of pixels, chosen due to the order we draw pixels in. Let $g$ be the major dimension of the entire image (width in raster mode, height in striped mode) and $h$ be the minor dimension (the opposite of the major dimension). The stripes will have a major dimension of $g$ and a minor dimension $s$ of their own, computed as follows:

| If...                 | Then...    |
| --------------------- | ---------- |
| $4096 <= g <= 8191$   | $s = 1024$ |
| $8192 <= g <= 16383$  | $s = 512$  |
| $16384 <= g <= 32767$ | $s = 256$  |
| $32768 <= g <= 65535$ | $s = 128$  |
| $65536 <= g$          | $s = 64$   |
Using the sizing blocks returned in [[Get Single Image Size]], we can write one image at a time to the folder and then indicate it is complete.