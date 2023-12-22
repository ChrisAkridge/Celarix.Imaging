using System;
using System.Collections.Generic;
using System.IO;
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

        private static void ColumnToPCM(float[] buf, float[] column, int sampleCount, int height, float[][] sineTable)
        {
            float sample = 0f;

            for (int i = 0; i < sampleCount; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    sample += (column[j] * sineTable[j][i]) / height;
                }

                sample *= 0.9f;
                buf[i] = sample;
            }
        }

        private static float GetPixelIntensity(Rgb24 pixel, int n)
        {
            int rgbSum = pixel.R + pixel.G + pixel.B;

            return (float)rgbSum / n / 255f;
        }

        public static void ImageToSpectrogram(Image<Rgb24> image, int duration, string outputFilePath)
        {
            int x = image.Width;
            int y = image.Height;

            int bufferSize = (duration * SampleRate) / x;

            if (bufferSize < 0)
            {
                throw new ArgumentException($"Duration {duration} is negative", nameof(duration));
            }

            using var outputFile = File.Open(outputFilePath, FileMode.Create, FileAccess.ReadWrite);
            using var writer = new BinaryWriter(outputFile);

            var column = new float[y];
            var buf = new float[bufferSize];
            var sineTable = new float[y][];

            for (int i = 0; i < y; i++)
            {
                sineTable[i] = new float[bufferSize];
            }

            float nyquist = SampleRate / 2f;
            float hzStep = nyquist / y;

            for (int i = 0; i < y; i++)
            {
                for (int j = 0; j < bufferSize; j++)
                {
                    var freq = nyquist - (hzStep * i);
                    var env = Math.Sin(2 * Math.PI * ((float)j / bufferSize));
                    sineTable[i][j] = (float)(env * Math.Sin(2 * Math.PI * freq * ((float)j / SampleRate)));
                }
            }

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    var intensity = GetPixelIntensity(image[i, j], 1);
                    column[j] = intensity;
                }
                ColumnToPCM(buf, column, bufferSize, y, sineTable);

                foreach (var f in buf) { writer.Write(f); }
            }
        }
    }
}
