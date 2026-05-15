**A unified WinForms UI for all current Celarix.Imaging functionality, plus more!**
## Operations
- [[Binary Draw File(s)]]
	- To Image
	- To Zoomable Canvas
	- To Fixed-Size Frames
- [[Sort Image]] by [[Color Spaces|Color Space]]
- [[Unique Colors]] by [[Color Spaces|Color Space]]
	- Unique Colors on [[Full-Palette Image]]
- View Specific Color Channels
- View RGBA Bit Planes
- Chroma Subsample
	- Doing away with the confusing J:a:b notation and having two properties: horizontal denominator and vertical denominator for chroma samples
- Downsample to Palette
	- Downsample to RGB x:y:z (uses rounding and shifting)
	- Downsample to top N colors
		- ...with Euclidean distance in a [[Color Spaces|color space]]
		- ...with median cut in a [[Color Spaces|color space]]
		- ...with k-means in a [[Color Spaces|color space]] (random, rerollable!)
- Resize Image
	- With sampling modes like nearest neighbor, closest pixel (gaps left white), average over color space (much smearing)
- Play or Export Analog Signals
	- Opens a wizard dialog
	- Two modes
		- Mock-NTSC with blanking intervals, sync pulse, color burst, and QAM-encoded chroma on the luma signal
		- Simple mode, with sinusoidal carriers at 500Hz (red), 1 kHz (green), and 1.5 kHz, blue, separated by silence per scanline
	- Displays statistics about the duration of each scanline and the total duration of the audio.
	- Can be played in the window or saved to WAV or MP3.
- Import Analog Signal
	- Opens a wizard dialog.
	- Lets you pick any WAV or MP3 audio file.
	- Attempts to decode in either the mock-NTSC format or the simple format, as specified by the user.
	- The user must also specify the width of the image. This determines where the blanking intervals should be.
	- Bandpass filters at the simple signal color carriers and the mock-NTSC carrier extract the chrome data, and a notch filter extracts the luma.
- Mathematical and Bitwise Operations
	- A set of unary, binary, and ternary operators over images.
	- Images can be images from disk, images in the main viewer, or "constant" images of a user-selected color.
- Generate Test Images
	- Full palettes, solid color images, etc.
- Make Grid from Pictures
	- Opens a wizard dialog.
	- Equivalent to PictureTiler's Tile mode.
- Pack Pictures onto Canvas
	- Opens a wizard dialog
	- Equivalent to PictureTiler's Canvas mode.
## Contents
- [[Pixel Formats]]
- [[Color Modes]]
## Notes
Anything relying on RNG should let the user specify the seed in the `PropertyGrid`, along with a seed option of fixed or changing every refresh.