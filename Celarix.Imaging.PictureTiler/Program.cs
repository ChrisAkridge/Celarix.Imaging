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
            Spectrophoto.ImageToSpectrogram(
                Image.Load<Rgb24>(@"G:\Documents\Files\Pictures\Pictures\Cordilan Group\Roseate Series\r068.png"),
                10, @"G:\Documents\out0.raw");

            return;
            
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
	}
}
