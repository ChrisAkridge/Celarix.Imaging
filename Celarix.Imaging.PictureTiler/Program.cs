using System;
using System.IO;
using System.Windows.Forms;
using Celarix.Imaging.Misc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Celarix.Imaging.PictureTiler
{
    internal static class Program
	{
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
        private static void Main()
        {
            var audio = Spectrophoto.ConvertImageToSpectrogram(
                Image.Load<Rgb24>(@"G:\Documents\Files\Pictures\Pictures\S Series\5s Series\5s000143.png"),
                10);
            using var stream = File.Create(@"G:\Documents\audio.raw");
            using var writer = new BinaryWriter(stream);

            foreach (var f in audio)
            {
                writer.Write(f);
            }

            writer.Close();
            
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
	}
}
