Most [[Pixel Formats|pixel formats]] and [[Color Modes|color modes]] has a picture representing its full palette. They are described below. Pixels in the image are laid out in the raster format, left to right, top to bottom. Each pixel represents a +1 to the pixel before it, causing overflow from the lowest channel into the next lowest.
## 1 bit per pixel
- Palette size: $2^1 = 2$
- A 2x1 image with index 0 (black, usually) and index 1 (white, usually) in that order.
## 2 bits per pixel
- Palette size: $2^2 = 4$
- A 2x2 image with indices 0 and 1 on top and 2 and 3 on the bottom.
## 3 bits per pixel
- Palette size: $2^3 = 8$
- A 4x2 image with indices 0-3 and 4-7.
## 4 bits per pixel
- Palette size: $2^4 = 16$
- A 4x4 image with indices 0-3, 4-7, 8-11, and 12-15.
### RGB 1:2:1
- Two shades of red and blue, 4 shades of green.
- Blue cycles every 2 pixels, or twice per row.
- Green cycles every 8 pixels, or every 2 rows.
- Red cycles over the whole image.
### RGBA 1:1:1:1
- Two shades of each channel.
- Alpha cycles every 2 pixels, or twice per row.
- Blue cycles every 4 pixels, or once per row.
- Green cycles every 8 pixels, or every 2 rows.
- Red cycles over the whole image.
## 8 bits per pixel
- Palette size: $2^8 = 256$
- A 16x16 image.
### RGB 3:3:2
- 4 shades for blue, 8 shades for red and green.
- Blue cycles every 4 pixels, or 4 times per row.
- Green cycles every 32 pixels, or every 2 rows.
- Red cycles over the whole image.
### RGBA 2:2:2:2
- 4 shades of each channel.
- Alpha cycles every 4 pixels, or 4 times per row.
- Blue cycles every 16 pixels, or once per row.
- Green cycles every 64 pixels, or every 4 rows.
- Red cycles over the whole image.
## 16 bits per pixel
- Palette size: $2^{16} = 65,536$
- A 256x256 image.
## RGB 5:6:5
- 32 shades of red and blue, 64 shades of green.
- Blue cycles every 32 pixels, or 4 times per row.
- Green cycles every 2,048 pixels, or every 8 rows.
- Red cycles over the whole image.
#### RGBA 4:4:4:4
- 16 shades of each channel.
- Alpha cycles every 16 pixels, or 16 times per row.
- Blue cycles every 256 pixels, or once per row.
- Green cycles every 4,096 pixels, or every 16 rows.
- Red cycles over the whole image.
## 24 bits per pixel
- Palette size: $2^{24} = 16,777,216$
- A 4096x4096 image:
	- Level 0 tiles: 4x4
	- 3 zoom levels total
### RGB 8:8:8
- 256 shades of each channel.
- Blue cycles every 256 pixels, or 16 times per row.
- Green cycles every 65,536 pixels, or every 16 rows.
- Red cycles over the whole image.
### RGBA 6:6:6:6
- 64 shades of each channel.
- Alpha cycles every 64 pixels, or 64 times per row.
- Blue cycles every 4,096 pixels, or once per row.
- Green cycles every 262,144 pixels, or every 64 rows.
- Red cycles over the whole image.
## 32 bits per pixel
- Palette size: $2^{32} = 4,294,967,296$
- A 65,536x65,536 image.
	- Level 0 tiles: 64x64
	- 6 zoom levels total
### RGBA 8:8:8:8
- 256 shades of each channel.
- Alpha cycles every 256 pixels, or 256 times per row.
- Blue cycles every 65,536 pixels, or once per row.
- Green cycles every 16,777,216 pixels, or every 256 rows.
- Red cycles over the whole image.