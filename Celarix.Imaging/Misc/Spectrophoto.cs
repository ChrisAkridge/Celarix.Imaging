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
        private const int SAMPLE_RATE = 48000;

        private static void column_to_PCM(float[] buf, float[] column, int nb_samples, int height, float[][] sin_lut)
        {
            float sample = 0f;

            for (int i = 0; i < nb_samples; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    sample += (column[j] * sin_lut[j][i]) / height;
                }

                sample *= 0.9f;
                buf[i] = sample;
            }
        }

        private static float get_pixel_intensity(Rgb24 pixel, int n)
        {
            int RGB_sum = pixel.R + pixel.G + pixel.B;

            float intensity = (float)RGB_sum / n / 255f;

            return intensity;
        }

        public static void ImageToSpectrogram(Image<Rgb24> image, int duration, string outputFilePath)
        {
            int x = image.Width;
            int y = image.Height;
            int n = 3;

            int nb_samples = (duration * SAMPLE_RATE) / x;

            if (nb_samples < 0)
            {
                throw new ArgumentException($"Duration {duration} is negative", nameof(duration));
            }

            using var output_file = File.Open(outputFilePath, FileMode.Create, FileAccess.ReadWrite);
            using var writer = new BinaryWriter(output_file);

            var column = new float[y];
            var buf = new float[nb_samples];
            var sin_lut = new float[y][];

            for (int i = 0; i < y; i++)
            {
                sin_lut[i] = new float[nb_samples];
            }

            float nyquist = SAMPLE_RATE / 2f;
            float hz_step = nyquist / y;

            for (int i = 0; i < y; i++)
            {
                for (int j = 0; j < nb_samples; j++)
                {
                    var freq = nyquist - (hz_step * i);
                    var env = Math.Sin(2 * Math.PI * ((float)j / nb_samples));
                    sin_lut[i][j] = (float)(env * Math.Sin(2 * Math.PI * freq * ((float)j / SAMPLE_RATE)));
                }
            }

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    var intensity = get_pixel_intensity(image[i, j], 1);
                    column[j] = intensity;
                }
                column_to_PCM(buf, column, nb_samples, y, sin_lut);

                foreach (var f in buf) { writer.Write(f); }
            }
        }
    }
}
