The Unique Colors action converts an image to the specified color spaces and takes counts of each unique color in that image. Essentially, it deduplicates pixels in an image given a color space.
## Method
Given a color space and an image,
1. Produce an image with each pixel converted to the color space given.
2. Scan over the pixels in raster order, adding only those not seen before to a list of unique colors. This has the benefit that the order of the colors in the list is also the order they first appeared in the image.
3. Use the square root method to produce a width and height for the unique colors image:
	1. Given $C$ unique colors, let width be $\lfloor \sqrt{C} \rfloor$.
	2. Let height be $\lceil \frac{C}{\sqrt{C}} \rceil$.
4. Produce a solid black image of this size and draw the unique color pixels in raster order.
## On [[Full-Palette Image]]
Another way of viewing unique colors is by producing a full-palette image and coloring black all pixels on this image that don't appear in the original image. The user can invoke this in two ways:
- Using a color space with up to 3 8-bit components. The user specifies the color space.
- Using a [[Pixel Formats|pixel format]] and a [[Color Modes|color mode]]. The user specifies these both and must be a valid combination.
### Using Color Space
1. Produce an image with each pixel converted to the color space given.
2. Create an array of 16,777,216 booleans to track which colors are present in the image.
	1. Pixel to index: `(first_channel << 16) | (second_channel << 8) | third_channel`
	2. Index to pixel: `(index >> 16, (index >> 8) & 0xFF, index & 0xFF)`.
3. Using the index-to-pixel method, create a 4096x4096 image and lay out pixels in raster order. Place a black pixel if the index is `false` and the color of that index if it is `true`.
### Using Pixel Format + Color Mode
1. Produce the reference [[Full-Palette Image]] for that pixel format.
2. Create a two-dimension boolean array of the same dimensions as the full-palette image.
3. Scan the pixels in the source image. If a pixel matches exactly to any color in the full-palette image, set the corresponding boolean to `true`.
4. On the full-palette image, set to black any pixels whose corresponding boolean is `false`.
#### Possible Optimizations?
- Maybe loop over the palette image instead of the source? Maybe build a search tree of pixels in the source, or deduplicate the source first?