A pixel format is a function that converts one or more bytes into one or more palette indices, used in binary drawing. Bytes are converted in big-endian order, so bytes with lower addresses are converted before bytes with higher addresses.

| Name                       | Group           | Input Bytes | Output Pixels | Minimum Palette Size | Stripe width |
| -------------------------- | --------------- | ----------- | ------------- | -------------------- | ------------ |
| One bit per pixel          | Bytes-to-pixels | 1           | 8             | 2 colors             | 8 pixels     |
| Two bits per pixel         | Bytes-to-pixels | 1           | 4             | 4 colors             | 4 pixels     |
| Three bits per pixel       | Bytes-to-pixels | 3           | 8             | 8 colors             | 8 pixels     |
| Four bits per pixel        | Bytes-to-pixels | 1           | 2             | 16 colors            | 2 pixels     |
| Eight bits per pixel       | Bytes-to-pixels | 1           | 1             | 256 colors           | 1 pixel      |
| Sixteen bits per pixel     | Bytes-to-pixels | 2           | 1             | 65,536 colors        | 1 pixel      |
| Twenty-four bits per pixel | Bytes-to-pixels | 3           | 1             | 16,777,216 colors    | 1 pixel      |
| Thirty-two bits per pixel  | Bytes-to-pixels | 4           | 1             | 4,294,967,296 colors | 1 pixel      |
| Float16                    | Floating point  | 2           | 6             | 16,777,216 colors    | 6 pixels     |
| Float32                    | Floating point  | 4           | 6             | 16,777,216 colors    | 6 pixels     |
| Float64                    | Floating point  | 8           | 8             | 16,777,216 colors    | 8 pixels     |

Bytes-to-pixels formats produce indices of $n$ bits as an unsigned $n$-bit integer with range 0 to $2^n - 1$. If the source has an insufficient number of bytes to fill all pixels, extra 0x00 bytes are added to the end.

Floating point formats produce specific pixel patterns requiring a palette of at least 16,777,216 colors via producing sequences of indices representing those pixels.

Below, $L$ means the number of bytes in the source and $P$ is the number of pixels that result.
## Bytes-to-Pixels Formats
### One bit per pixel
$\text{byte} \rightarrow \text{index} \cdot 8$
$P = 8L$
Takes one byte and converts it into 8 palette indices from 0 to 1, from MSB to LSB:
```
Bit:   76543210
Index: 01234567
```
For example, 0x89 becomes (1, 0, 0, 0, 1, 0, 0, 1).
### Two bits per pixel
$\text{byte} \rightarrow \text{index} \cdot 4$
$P = 4L$
Takes one byte and converts it into 4 palette indices from 0 to 3:
```
Bit:   76543210
Index: 00112233
```
For example, 0x89 becomes (2, 0, 2, 1).
### Three bits per pixel
$\text{byte} \cdot 3 \rightarrow \text{index} \cdot 8$
$P = 8 \cdot \lceil \frac{L}{3} \rceil$
Takes three bytes and converts them into 8 palette indices from 0 to 7:
```
Bit:   76543210 76543210 76543210
Index: 00011122 23334445 55666777
```
For example, 0x8923AA becomes:
```
   8   9    2   3    A   A
10001001 00100011 10101010
..4..2.. 2..2..1. .6..5..2
```
(4, 2, 2, 2, 1, 6, 5, 2)
### Four bits per pixel
$\text{byte} \rightarrow \text{index} \cdot 2$
$P = 2L$
Takes one byte and converts it into 2 palette indices from 0 to 15:
```
Bit:   76543210
Index: 00001111
```
For example, 0x89 becomes (8, 9).
### Eight bits per pixel
$\text{byte} \rightarrow \text{index}$
$P = L$
Takes one byte and converts it into 1 palette index from 0 to 255. For example, 0x89 becomes 137.
### Sixteen bits per pixel
$\text{byte} \cdot 2 \rightarrow \text{index}$
$P = \lceil \frac{L}{2} \rceil$
Takes two bytes and converts them into 1 palette index from 0 to 65,535. For example, 0x8923 becomes 35,107.
### Twenty-four bits per pixel
$\text{byte} \cdot 3 \rightarrow \text{index}$
$P = \lceil \frac{L}{3} \rceil$
Takes three bytes and converts them into 1 palette index from 0 to 16,777,215. For example, 0x8923AA becomes 8,987,562.
### Thirty-two bits per pixel
$\text{byte} \cdot 4 \rightarrow \text{index}$
$P = \lceil \frac{L}{4} \rceil$
Takes four bytes and converts them into 1 palette index from 0 to 4,294,967,295. For example, 0x8923AACC becomes 2,300,816,076.
## Floating Point Formats
Floating point formats are the most common way computers represent numbers with fractional components. The IEEE-754 formats are the most common floating point formats with widespread hardware acceleration. Thus, we include some pixel formats which convert bytes into pixel representations of these floating point numbers.

An IEEE-754 binary floating point number has three parts:
- A sign bit $s$, indicating whether the number is positive or negative
- An exponent of $e$ bits
- And a mantissa of $m$ bits
The exponent field uses an offset-binary representation, where a bias $b$ is subtracted from the exponent to produce the unbiased exponent. An exponent with all bits 0 or all bits 1 have special interpretations:
- $e = 000\ldots000$:
	- $m = 0$: The number is 0 or -0.
	- $m \neq 0$: The number is subnormal (see below).
- $e = 111\ldots111$:
	- $m = 0$: The number is positive or negative infinity.
	- $m \neq 0$: The number is not-a-number, also known as NaN.
The mantissa has an implied leading bit, which is 1 when the exponent is non-zero and 0 when the exponent is 0. Numbers with a leading 1 bit are called _normal_ and numbers with a leading 0 bit are called _subnormal_.
The actual value of the number specified by a given sign, exponent, and mantissa is:
- For normal numbers, $(-1)^{s} \cdot 2^{\text{exponent} - b} \cdot 1.\text{mantissa}_2$
- For subnormal numbers, $(-1)^s \cdot 2^{0 - b} \cdot 0.\text{mantissa}_2$
We use three floating point formats in pixel formats:

| Format                     | Bytes | Exponent bits | Mantissa bits | Exponent bias |
| -------------------------- | ----- | ------------- | ------------- | ------------- |
| Half-precision (Float16)   | 2     | 5             | 10            | 15            |
| Single-precision (Float32) | 4     | 8             | 23            | 127           |
| Double-precision (Float64) | 8     | 11            | 52            | 1023          |
Converting the bytes into indices for these formats is a three-step process:
- First, the required number of bytes are taken in and used to compute the sign, exponent, and mantissa. The exponent is left in its biased form, its bits to be directly displayed. The mantissa has the implied leading bit prepended to it.
- Second, the values are used to produce 24-bit RGB 8:8:8 pixels. The order is:
	- 1 pixel for the sign bit, either white (#FFFFFF) for positive or black (#000000) for negative.
	- 1 guard pixel
	- $\lceil \frac{1}{24} \cdot m_\text{width} \rceil$ pixels for the mantissa (where $m_\text{width}$) is the width of the mantissa in bits plus 1 for the implied leading bit.
	- 1 guard pixel
	- 1 pixel for the exponent, grayscale if and only if it is 8 bits wide or less, otherwise RGB.
	- 1 guard pixel
- Third, the pixels are used to produce indices into a palette of at least $2^{24}$ = 16,777,216 colors. The conversion is `(r, g, b) => (r << 16) | ( << 8) | b`.
Guard pixels separate each part of the float visually and also each float from the next. Guard pixels have the following colors:
- Light gray (#A0A0A0) if the float is a normal number.
- Dark gray (#303030) if the float is a subnormal number.
- Green (#00FF00) if the float is an infinity.
- Red (#FF0000) if the float is a NaN.
Psuedocode for the guard color:
```
int32 guardColor(int16 exponent, int16 maxExponent, int64 mantissa) {
	if (exponent == 0) {
		return 0x303030;    // subnormal
	} else if (exponent == maxExponent) {
		if (mantissa == 0) {
			return 0x00FF00;    // infinity
		} else {
			return 0xFF0000;    // NaN
		}
	} else {
		return 0xA0A0A0;    // normal
	}
}
```
## Float16
$\text{byte} \cdot 2 \rightarrow \text{index} \cdot 6$
$P = 6 \cdot \lceil \frac{L}{2} \rceil$
Takes two bytes and produces a set of six pixels representing an IEEE-754 half-precision binary floating point number. The bit pattern of such a float is:
```
Byte:        0        1
Bit:  76543210 76543210
Part: SEEEEEMM MMMMMMMM
```
The mantissa pixel is 11 bits when adding the implied leading bit and becomes an RGB 4:4:3 pixel (see [[Color Modes]] for more information). The exponent pixel is 5 bits and becomes a grayscale pixel with 32 shades.

Psuedocode for the conversion:
```
int32[] toFloat16(uint16 bytes) {
	uint16 sign = bytes >> 15;
	uint16 exponent = (bytes >> 10) & 0x1F;
	uint16 mantissa = bytes & 0x3FF;
	int32 guard = guardColor(exponent, 0x1F, mantissa);
	if (exponent != 0) { mantissa |= (1 << 11); }
	int32[] pixels = new int32[6];
	pixels[0] = (sign == 0) ? 0xFFFFFF : 0x000000;
	pixels[1] = guard;
	pixels[2] = rgbPalette(4, 4, 3)[mantissa];
	pixels[3] = guard;
	pixels[4] = grayscalePalette(5)[exponent];
	pixels[5] = guard;
	return pixels;
}
```
Psuedocode for `rgbPalette` and `grayscalePalette` can be found in [[Color Modes]].
### Float32
$\text{byte} \cdot 4 \rightarrow \text{index} \cdot 6$
$P = 6 \cdot \lceil \frac{L}{4} \rceil$
Takes four bytes and produces a set of six pixels representing an IEEE-754 single-precision binary floating point number. The bit pattern of such a float is:
```
Byte:        0        1        2        3
Bit:  76543210 76543210 76543210 76543210
Part: SEEEEEEE EMMMMMMM MMMMMMMM MMMMMMMM
```
The mantissa pixel is 24 bits when adding the implied leading bit and nicely becomes an RGB 8:8:8 pixel. The exponent pixel is 8 bits and becomes a grayscale pixel with 256 shades.

Psuedocode for the conversion:
```
int32[] toFloat32(uint32 bytes) {
	uint32 sign = bytes >> 31;
	uint32 exponent = (bytes >> 23) & 0xFF;
	uint32 mantissa = bytes & 0x7FFFFF;
	int32 guard = guardColor(exponent, 0xFF, mantissa);
	if (exponent != 0) { mantissa |= (1 << 24); }
	int32[] pixels = new int32[6];
	pixels[0] = (sign == 0) ? 0xFFFFFF : 0x000000;
	pixels[1] = guard;
	pixels[2] = mantissa;
	pixels[3] = guard;
	pixels[4] = (exponent << 16)
		| (exponent << 8)
		| exponent;
	pixels[5] = guard;
	return pixels;
}
```
### Float64
$\text{byte} \cdot 8 \rightarrow \text{index} \cdot 8$
$P = 8 \cdot \lceil \frac{L}{8} \rceil$
Takes four bytes and produces a set of six pixels representing an IEEE-754 single-precision binary floating point number. The bit pattern of such a float is:
```
Byte:        0        1        2        3        4        5           6        7
Bit:  76543210 76543210 76543210 76543210 76543210 76543210 76543210 76543210
Part: SEEEEEEE EEEEMMMM MMMMMMMM MMMMMMMM MMMMMMMM MMMMMMMM MMMMMMMM MMMMMMMM
```
The mantissa pixels are 24, 24, and 5 bits when adding the implied leading bit and become two RGB 8:8:8 pixels followed by one RGB 2:2:1 pixel. The exponent pixel is 11 bits and becomes an RGB 4:4:3 pixel.

Psuedocode for the conversion:
```
int32[] toFloat64(uint64 bytes) {
	uint64 sign = bytes >> 63;
	uint64 exponent = (bytes >> 53) & 0x7FF;
	uint64 mantissa = bytes & 0xFFFFFFFFFFFFF;
	int32 guard = guardColor(exponent, 0x7FF, mantissa);
	if (exponent != 0) { mantissa |= (1 << 53); }
	int32[] pixels = new int32[8];
	pixels[0] = (sign == 0) ? 0xFFFFFF : 0x000000;
	pixels[1] = guard;
	pixels[2] = mantissa >> 29;
	pixels[3] = (mantissa >> 5) & 0xFFFFFF;
	pixels[4] = rgbPalette(2, 2, 1)[mantissa & 0x1f];
	pixels[5] = guard;
	pixels[6] = rgbPalette(4, 4, 3)[exponent];
	pixels[7] = guard;
	return pixels;
}
```
Psuedocode for `rgbPalette` can be found in [[Color Modes]].