# ByteView Chroma Playground: Convert image in YCbCr format to "analog"

Takes a normal RGB image file and treats it as YCbCr (with desired chroma subsampling, if chosen), then converts it to an "analog signal" similar to NTSC video, although not nearly compatible! This is just for fun, mind you. The analog signal is saved as a raw audio stream at 48,000 Hz with 32-bit floating-point samples.

The construction process starts by generating the chroma carrier. A sine wave and a cosine wave at 2,000 Hz are generated and the amplitudes are modulated based on the strength of the Cb and Cr channels, with 0.0 amplitude corresponding to Cb/Cr = 0 and 1.0 amplitude corresponding to Cb/Cr = 1. 4 cycles of the sine and cosine carriers correspond to 1 pixel of Cb/Cr data (if chroma subsampling is used, the chroma samples just repeat in the signal). Thus, each second contains 2000 / 4 = 500 pixels of chroma data, and since the chroma sample rate has to match the luma sample rate, that leads to 500 pixels per second for each scanline.

A 2,000 Hz signal within a 48,000 Hz sample rate means there are 24 samples for every cycle of the sine/cosine wave, and thus 24 different coherent phase positions, leading to a pretty limited chroma space, but that's the price of such a low sample rate. The sine and cosine channels are added together and then their floating point samples are normalized to +/-0.1 to be added to the luma carrier.

The luma carrier is much simpler. It lies between +/-0.9 in the floating point space to give full room for the chroma channel, where -0.9 corresponds to a luma of 0 and +0.9 corresponds to a luma of 1. The visible portion of the scanline takes some time, denoted *s*. Working off of Retro Game Mechanics Explained's [excellent video on SNES lag frames and blanking](https://www.youtube.com/watch?v=Q8ph2OVqZeM) (timestamp 2:55), we can derive relative durations of what the horizontal blanking would be for our differently-sized image. The image in our signal will be scanned progressively and there will be no vertical blanking since there's only one "frame" to scan.

- A single field of NTSC video lasts roughly 16.64 milliseconds
- Scanlines along with their horizontal blanking intervals are drawn for 14.23 milliseconds (85.52% of the frame)
- The frame is split into 224 scanlines, each lasting 63.51 microseconds (0.38% of the frame)
- The visible portion of the scanline lasts 47.82 microseconds (75.30% of the scanline)
- The horizontal blanking interval of each scanline lasts 15.69 microseconds (24.70% of the scanline)

So our scaling factors can be derived from this. Working up:

- The visible portion of our scanline lasts *s*.
- The horizontal blanking interval of our scanline lasts 0.247 \* *s*.

And... uh... that's it, really. Again, we aren't including a vertical blanking interval. Each scanline takes 1.247 times the length of a single visible portion of a scanline.

For example, let's take a 1920x1080 image. We can draw 500 pixels per second, leading to *s* = 3.84 seconds. Horizontal blanking takes 948.48 milliseconds, and the entire scanline takes 4.79 seconds. Slow-scan TV indeed. The full image would take 5,171.56 seconds, or 1 hour 26 minutes 11 seconds.

Well, this wasn't meant to be useful, huh? What goes into the horizontal blanking interval? First, we do need to adjust our scale again. The luma + chroma we just built ranges from -1 to +1, but the full signal requires us to go lower than -1 to account for the sync pulse.

Normal television signals made by sane people who aren't me are based on voltages provided to the electron scanning beam. Per [this page](https://clearview-communications.com/insights/understanding-composite-video-signals/), the voltage range is divided, 70-30, into the part where the visible portion of the scanline resides, and a part below that where the sync pulse lives. So, if our maximum voltage is 1 volt, the visible portion of the scanline would always have a voltage between 300 millivolts and 1 volt, and the sync pulse would go to 0 millivolts.

Uh, probably. At least, that's how we'll handle it here. To make the range match, we need to take our +/-1 of the visible part of the scanline and shift it to fit into the 70/30 range, which pushes it to a range of -0.6 to +1.

Now we get to build the horizontal blanking interval. Immediately after the scanline's visible part, we follow it up with a front porch, a flat segment of audio at an amplitude of -0.6, corresponding to the bottom of the "visible" amplitude. [Wikipedia](https://en.wikipedia.org/wiki/Analog_television) lists it as lasting about 1.5 microseconds in NTSC, which is 9.5% of what the horizontal blanking interval's duration is, so we'll spend the first 9.5% of our blanking interval at this amplitude (90.11 milliseconds for our 1920x1080 image).

Wikipedia helpfully lists the sync pulse in NTSC as lasting 4.7 microseconds, or 29.95% of the blanking interval, so we'll use the next 29.95% immediately after the front porch to store the sync pulse, which is a flat segment of audio at -1 amplitude (284.07 milliseconds for our 1920x1080 image).

It seems the rest of the blanking interval is the back porch, which is again at an amplitude of -0.6. The back porch also contains a colorburst, which would indicate the signal is in color. We have 60.55% of our blanking interval left, and eyeballing [an image](https://en.wikipedia.org/wiki/Analog_television#/media/File:Videosignal_porch.jpg) on Wikipedia, it seems the color burst occurs about 10% of the way into the back porch.

The color burst will have the same frequency as our chroma carrier, 2,000 Hz, and will have a phase of 180 degrees, so it'll be made of a normal, 0-degree shifted sine wave subtracted from silence. Going off of NESDev's [NTSC video](https://www.nesdev.org/wiki/NTSC_video) page, I guess the colorburst's duration is 2.79 microseconds in NTSC, or 17.78% of the horizontal blanking interval.

Now we have all the parts. Here's our complete timing for a scanline and its entire horizontal blanking interval:

- Visible portion: *s*
- Horizontal blanking interval: *h* = 0.247 \* *s*
	- Front porch: 0.095 \* *h*
	- Sync pulse: 0.2995 \* *h*
	- Back porch: 0.6055 \* *h*
		- Before colorburst: 0.06055 \* *h*
		- Colorburst: 0.1778 \* *h*
		- After colorburst: 0.36715 \* *h*

So, for our 1920x1080 image, the timings would be as follows, given 500 pixels per second in the visible part of the scanline

- Visible portion: 3.84 seconds
- Horizontal blanking interval: 948.48 milliseconds
	- Front porch: 90.11 milliseconds
	- Sync pulse: 284.07 milliseconds
	- Back porch: 574.30 milliseconds
		- Before colorburst: 57.43 milliseconds
		- Colorburst: 168.64 milliseconds
		- After colorburst: 348.23 milliseconds

Or, in 48 kHz samples with rounding:

- Visible portion: 184,320 samples
- Horizontal blanking interval: 48,527 samples
	- Front porch: 4,325 samples
	- Sync pulse: 13,635 samples
	- Back porch: 27,566 samples
		- Before colorburst: 2,756 samples
		- Colorburst: 8,094 samples
		- After colorburst: 16,715 samples
		
## Simpler Analog Signal

Of course, mock-NTSC compatibility is nice and all, but we're not even making something meant to be high-quality or compatible, anyway! What's a simpler analog signal format that we might be able to run through more quickly?

In the simpler format, we'll have 3 sine wave carriers, one for red, green, and blue, respectively. Their frequencies are 500 Hz, 1 kHz, and 1.5 kHz, and the sample rate is still 48 kHz, so the red carrier gets 96 samples per cycle, the green gets 48 samples per cycle, and the blue 32 samples per cycle.

We'll set the sine wave amplitude range to -0.9 to +1 in the floating point +/-1 space, with -0.9 corresponding to `0x00` and +1 corresponding to `0xFF`. Each 256th step is an amplitude change of 0.007421875. Each pixel of the scanline lasts for 1 cycles of the red carrier (so, 2 cycles of the green carrier and 3 of the blue). Thus, we can send 500 pixels per second, same as our mock-NTSC signal. We'll make a very simple horizontal blanking interval of 100 cycles (2.083 milliseconds) at an amplitude of -1. No colorburst is required since we know we're transmitting in color.

Our 1920x1080 image still takes 3.84 seconds to send the visible portion of a scanline, but the 2.083 millisecond blanking interval makes that a mere 3.842083 seconds in total, compared to the mock-NTSC's 4.78848 seconds. The full 1,080 lines take 4,149.44964 seconds, or 1 hour 9 minutes 9 seconds, a time savings of 21 minutes.

Could we make the carriers at a higher frequency so we can send it faster? Sure, but then it would be harder to hear.