# PixelConversion Enumeration Values

## `To1BppBlackWhite`

Converts each byte into 8 bits and then produces 8 pixels from MSB to LSB. 0 maps to black (#000000FF), 1 maps to white (#FFFFFFFF).

## `To2BppGrayscale`

Converts each byte into 4 bit pairs and produces 4 pixels from MSB to LSB, mapping to one-third grays. 00 maps to black, 01 maps to one-third gray (#555555FF), 10 maps to two-thirds gray (#AAAAAAFF), 11 maps to white (#FFFFFFFF).

Note that there is a class `DefaultPalettes` that specifies how to compute the shades of each color channel (`DefaultPalettes.GenerateRange`, when given (2^n - 1)).

## `To4BppGrayscale`

Converts each byte into 2 nybbles and produces 2 pixels from MSB to LSB, mapping to one-fifteenth grays.

## `To4BppRGB121`

Converts each byte into 2 nybbles and produces 2 pixels from MSB to LSB, with the bits mapped as `RGGB`. Red and blue get one bit and thus are either black or full intensity. Green gets 2 bits and has shades of thirds.

## `To4BppRGBA1111`

Converts each byte into 2 nybbles and produces 2 pixels from MSB to LSB, with the bits mapped as `RGBA`. All channels get one bit and thus are either black/transparent or full intensity.

## `To8BppGrayscale`

Converts each byte into 1 pixel mapping to one-255th grays.

## `To8BppRGB332`

Converts each byte into 1 pixel, with the bits mapped as `RRRGGGBB`. Red and green get 3 bits each and have shades of sevenths. Blue gets two bits and has shades of thirds.

## `To8BppRGBA2222`

Converts each byte into 1 pixel, with the bits mapped as `RRGGBBAA`. All channels get two bits and have shades of thirds.

## `To16BppRGB565`

Converts each byte into 2 pixels, with the bits mapped as `RRRRRGGG GGGBBBBB`. Red and blue get 5 bits each and have shades of thirty-firsts. Green gets 6 bits and has shades of sixty-thirds.

## `To16BppRGBA4444`

Converts each byte into 2 pixels, with the bits mapped as `RRRRGGGG BBBBAAAA`. All channels get 4 bits and have shades of fifteenths.

## `To24BppRGB888`

Converts each byte into 3 pixels, with the bits mapped as `RRRRRRRR GGGGGGGG BBBBBBBB`. All channels get 8 bits. This is the typical pixel format for most modern displays.

## `To24BppRGBA6666`

Converts each byte into 3 pixels, with the bits mapped as `RRRRRRGG GGGGBBBB BBAAAAAA`. All channels get 6 bits and have shades of sixty-thirds.

## `To32BppRGB8888`

Converts each byte into 4 pixels, with the bits mapped as `RRRRRRRR GGGGGGGG BBBBBBBB AAAAAAAA`. All channels get 8 bits. This is the typical pixel format for PNG images.