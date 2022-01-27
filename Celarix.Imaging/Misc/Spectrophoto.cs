using System;
using System.Collections.Generic;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Celarix.Imaging.Misc
{
    public static class Spectrophoto
    {
        // Ported from https://github.com/kylophone/spectrophoto/
        // Licensed with the MIT license
        private const int SampleRate = 48000;
        
        public static float[] ConvertImageToSpectrogram(Image<Rgb24> image,
            int durationInSeconds)
        {
            int totalSamples = (durationInSeconds * SampleRate);

            if (totalSamples < 0)
            {
                throw new ArgumentException(nameof(durationInSeconds), $"Duration {durationInSeconds} is negative");
            }

            var imageHeight = image.Height;
            var imageWidth = image.Width;
            // var sineTable = new float[totalSamples, image.Height];
            var sineTable = new float[imageHeight][];
            var nyquist = SampleRate / 2f;
            var hertzStep = nyquist / imageHeight;
            var result = new float[totalSamples * image.Width];
            var resultPosition = 0;

            for (int y = 0; y < imageHeight; y++)
            {
                sineTable[y] = new float[totalSamples];
                for (int x = 0; x < totalSamples; x++)
                {
                    var frequency = nyquist - (hertzStep * y);
                    var envelope = (float)Math.Sin(2f * Math.PI * ((float)x / totalSamples));
                    sineTable[y][x] = envelope * (float)Math.Sin(2f * Math.PI * frequency * ((float)x / SampleRate));
                }
            }

            for (var x = 0; x < imageWidth; x++)
            {
                var sample = 0f;

                for (var s = 0; s < totalSamples; s++)
                {
                    for (var y = 0; y < imageHeight; y++)
                    {
                        var pixel = image[x, y];
                        sample += (((pixel.R + pixel.G + pixel.B) / 255f) * sineTable[y][s]) / imageHeight;
                    }

                    sample *= 0.9f;
                }

                result[resultPosition++] = sample;
            }

            return result;
        }
    }
}
