Once a sequence of bytes has been converted into palette indices, this stage of binary drawing converts them into the base format of RGBA 8:8:8:8, with the bytes in `RR GG BB AA` order.

## Explicit vs. Implicit Palettes
Palettes are of the following kinds:
- Explicit
	- Default
	- User-specified
- Implicit
Explicit palettes are limited to $2^{16}$ = 65,536 colors. The user may provide the palette by specifying any file, from which each 32-bit RGBA 8:8:8:8 color is read off in order, in big-endian format.

If the user-specified file is not long enough to specify all colors in the palette, the user may select one of the following options to provide the remaining colors:
- Default grayscale (up to 8 bits): The remaining colors are filled with the default grayscale palette, specified below.
- Default RGB: The remaining colors are filled with the default RGB palette, specified below.
- Default RGBA: The remaining colors are filled with the default RGBA palette, specified below.
- Specified Color: The user provides a single RGBA color which fills all remaining colors.
- Seeded Random: The first four bytes of the file are used as a seed to an RNG that fills the remaining colors via random bytes.
- Unseeded Random: Maximum chaos - the remaining colors are filled with an RNG left unseeded.

If no user-specified palette is specified, a default palette is generated using one of the methods described below.

| Pixel Format               | Grayscale   | RGB              | RGBA               |
| -------------------------- | ----------- | ---------------- | ------------------ |
| One bit per pixel          | 1-bit       | Unsupported      | Unsupported        |
| Two bits per pixel         | 2-bit       | Unsupported      | Unsupported        |
| Three bits per pixel       | 3-bit       | 1:1:1            | Unsupported        |
| Four bits per pixel        | 4-bit       | 1:2:1            | 1:1:1:1            |
| Eight bits per pixel       | 8-bit       | 3:3:2            | 2:2:2:2            |
| Sixteen bits per pixel     | Unsupported | 5:6:5            | 4:4:4:4            |
| Twenty-four bits per pixel | Unsupported | 8:8:8 (implicit) | 6:6:6:6 (implicit) |
| Thirty-two bits per pixel  | Unsupported | Unsupported      | 8:8:8:8 (implicit) |

## Grayscale Default Palette
A grayscale palette for _b_ bits contains 2^b shades of gray, evenly spaced from black (0, 0, 0) to white (255, 255, 255). $b$ can be from 1 to 8.

The spacing is computed by dividing 255 by the number of _intervals_ between shades — which is one less than the number of shades, or 2^b − 1. Each shade is then that interval size multiplied by its index, rounded up. All shades have an alpha of 255 - that is, fully opaque.
## RGB Default Palette
The same principle applies to RGB palettes. Each channel — red, green, and blue — is assigned a bit depth from 0 to 8, with at least one channel having a non-zero value and the sum of all three being no more than 16. A channel with _n_ bits produces 2^n evenly-spaced intensity values from 0 to 255, exactly as in the grayscale case. A channel with 0 bits is always 0. The palette is then the Cartesian product of all three channels' value sets — every combination of one red value, one green value, and one blue value becomes one palette entry. All shades have an alpha of 255 - that is, fully opaque.
### RGBA Default Palettes
This can naturally be extended to RGBA palettes. Each channel - red, green, blue, and alpha, is assigned a bit depth of 0 to 8, with at least one channel having a non-zero value, and the sum of all four being no more than 16.
## Implicit Palettes
When using the 24 or 32 bit per pixel pixel format or any of the floating point pixel formats, an explicit in-memory palette is eschewed for mapping indices directly to color values.
- RGB 8:8:8 simply uses the palette index as the red, green, and blue values and uses an alpha of 255.
- RGBA 6:6:6:6 does internally use 64-shade ramps but doesn't create a large in-memory palette.
- RGBA 8:8:8:8 simply uses the palette index as pixel's value directly.
## Psuedocode
```
int32[] rgbPalette(uint8 redBits, uint8 greenBits, uint8 blueBits) {
	int8[] redShades = ramp(redBits);
	int8[] greenShades = ramp(greenBits);
	int8[] blueShades = ramp(blueBits);
	int32[] result = new int32[
		redShades.length
		* greenShades.length
		* blueShades.length
	];
	
	for (int32 r from 0 to redShades.length) {
		for (int32 g from 0 to greenShades.length) {
			for (int32 b from 0 to blueShades.length) {
				int32 resultIndex = (r << (redBits + greenBits))
					| (g << greenBits)
					| b;
				int32 resultRed = redShades[r] << (redBits + greenBits);
				int32 resultGreen = greenShades[g] << greenBits;
				int32 resultBlue = blueShades[b];
				result[resultIndex] = resultRed | resultGreen | resultBlue;
			}
		}
	}
	
	return fullAlpha(result);
}
```

```
int32[] grayscalePalette(uint8 bits) {
	int8[] grayShades = ramp(bits);
	int32[] result = new int32[grayShades.length];
	for (int32 i from 0 to grayShades.length) {
		result[i] = (grayShades[i] << 16)
			| (grayShades[i] << 8)
			| grayShades[i];
	}
	
	return fullAlpha(result);
}
```

```
int8[] ramp(uint8 bits) {
	uint8 steps = bits - 1;
	double stepSize = 255d / steps;
	int8[] result = new int8[1 << bits];
	double current = 0d;
	
	for (int32 i from 0 to 1 << bits) {
		result[i] = (int32)ceiling(current);
		current += stepSize;
	}
	
	return fullAlpha(result);
}
```

```
int32[] fullAlpha(int32[] palette) {
	int32[] result = new int32[palette.length];
	for (int32 i from 0 to palette.length) {
		result[i] = ((palette[i]) << 8) | 0xFF;
	}
	return result;
}
```