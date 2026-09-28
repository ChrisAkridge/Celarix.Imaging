There's a lot of different options that can be set that impacts the size of a single image. Here's how to compute it all based on the options.

Early exit: if the sizing mode is Fixed Size, return that. **Proceed to** [[Single Image Preparation]].
## 1. Count Pixel Blocks
First, we need to know the title mode. If we have zero or one title bars, then the rest of the image is just the entire set of files converted to pixels. If we have one title bar per file, then we need to get more creative. Let a **pixel block** be a rectangular part of the image where we binary draw pixels. For zero or one title bars, there is one pixel block consisting of all the files, and for one-per-file, there are $n$ for $n$ files.
## 2. Count Pixels in Block
We can get simple pixel counts, if not their dimensions, using pixel format alone. For $b$ bytes:
- 1 bit per pixel: $8b$ pixels
- 2 bits per pixel: $4b$ pixels
- 3 bits per pixels: $\lceil \frac{8b}{3} \rceil$ pixels
- 4 bits per pixel: $2b$ pixels
- 8 bits per pixel: $b$ pixels
- 16 bits per pixel: $\lceil \frac{b}{2} \rceil$ pixels
- 24 bits per pixel: $\lceil \frac{b}{3} \rceil$ pixels
- 32 bits per pixel: $\lceil \frac{b}{4} \rceil$ pixels
- Float16: $\lceil \frac{6b}{2} \rceil$ pixels
- Float32: $\lceil \frac{6b}{4} \rceil$ pixels
- Float64: $\lceil \frac{8b}{8} \rceil$ pixels
## 3. Size Each Block
### Automatic Width
#### Raster
The classic. For $p$ pixels:
1. Let $w = \lceil sqrt(p) \rceil$.
2. Let $h = \lceil \frac{p}{w} \rceil$.
3. The size of the block is $(w, h)$.
#### Striped
Striped images lay out a stripe of pixels left-to-right and then go down one row to write another stripe. The striping rules vary by pixel format:

| Pixel Format       | Stripe Width | Representing |
| ------------------ | ------------ | ------------ |
| 1 bit per pixel    | 8 pixels     | 1 byte       |
| 2 bits per pixel   | 4 pixels     | 1 byte       |
| 3 bits per pixel   | 8 pixels     | 3 bytes      |
| 4 bits per pixel   | 2 pixels     | 1 byte       |
| 8 bits per pixel   | 1 pixel      | 1 byte       |
| 16 bits per pixel  | 1 pixel      | 2 bytes      |
| 24 bits per pixel  | 1 pixel      | 3 bytes      |
| 32 bits per pixel  | 1 pixel      | 4 bytes      |
| Float16            | 6 pixels     | 2 bytes      |
| Float32            | 6 pixels     | 4 bytes      |
| Float64            | 8 pixels     | 8 bytes      |
Consider the input byte stream to be padded with `0x00` bytes to fill all pixels. Now, to determine the size of a pixel block in striped mode:
1. Let $s$ be the number of stripes that can be made from the byte stream.
2. Let $h = \lceil sqrt(s) \rceil$. This is the height of the pixel block in pixels.
3. Let $w_s = \lceil \frac{s}{h} \rceil$. This is the width of the pixel block in stripes.
4. Let $p$ be the stripe width in pixels.
5. The size of the pixel block is $(w_s \cdot p, h)$.
### Fixed Width
#### Raster
Basically the same as in automatic width, except $w$ is predefined.
#### Striped
If the predefined $w$ is not divisible by the stripe width $p$, we leave the right side of the pixel block black such that $\lfloor \frac{w}{p} \rfloor$ stripes are drawn, instead. Since we start with width in the automatic mode, we now have to start from height. **$w$ must be $>= p$!**
1. Let $w_s = \lfloor \frac{w}{p} \rfloor$. This tells us how many stripes we can fit inside our predefined width.
2. Let $s$  be the number of stripes in the image.
3. Let $h = \frac{s}{w_s}$.
4. The size of the pixel block is $(w, h)$.
## 4. Resize Narrow Blocks (Automatic sizing and one-title-bar-per-file only)
In one-title-bar-per-file mode, we end up with many pixel blocks of various widths. We want to resize them all to be the same width as the widest one. We need to size each pixel block first so we know which the widest is.

Let $w_{max}$ be the width of the widest pixel block. We can reuse the same logic for computing the size of each pixel block for a predefined width as defined above.
## 5. Compute Final Size
The final size is initialized to $(w_{max}, 0)$. Add all the heights of all pixel blocks, then all the heights of the title bars. Title bar height is a constant in the code that you can change. For zero title bars, it has 0 height, for one title bar, it has that constant as a height, for one-title-bar-per-file, it has $n$ times the constant for $n$ files.

## Proceed to [[Single Image Preparation]].