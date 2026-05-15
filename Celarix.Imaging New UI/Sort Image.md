Sorting an image starts by selecting a color space. Then, the pixels in the current image are each converted to that color space and a sort is performed on the channels in the order they're defined in.

For example, choosing RGB leaves the pixels in the format they're in, which is already RGB, then takes the three channels of each pixel as one large integer (i.e. `(r << 16) | (g << 8) | b`), then sorts them.

The sorted pixels are laid back out onto the main image in raster order. Sorts can be done at the **image level**, **row level**, or **column level**.