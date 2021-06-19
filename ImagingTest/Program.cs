using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Celarix.Imaging.BinaryDrawing;
using Celarix.Imaging.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.PixelFormats;

namespace ImagingTest
{
    internal class Program
	{
		private static async Task Main(string[] args)
        {
            var stopwatch = Stopwatch.StartNew();
			var files = Directory.GetFiles(@"C:\Users\ChrisAckridge\Documents\Notes\Personal", "*.*", SearchOption.AllDirectories);
			var streams = new NamedMultiStream(files.ToDictionary(f => f, f => (Stream)File.OpenRead(f)));
			var lastFileIndex = 0;

			while (streams.Position < streams.Length)
			{
				var image = await Drawer.DrawFixedSizeWithSourceText(new Size(640, 480),
					streams,
					24,
					null,
					CancellationToken.None,
					null);
				image.SaveAsPng($@"C:\Users\ChrisAckridge\Pictures\Imaging\image_{lastFileIndex:D4}.png");
				Console.WriteLine($"Saved {lastFileIndex}");
				lastFileIndex++;
			}

            stopwatch.Stop();
			Console.WriteLine(stopwatch.Elapsed);
        }
	}
}
