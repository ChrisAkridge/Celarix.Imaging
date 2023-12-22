using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Celarix.Imaging.Misc;
using Celarix.Imaging.ZoomableCanvas;
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
            LibraryConfiguration.Instance = new LibraryConfiguration
            {
                BinaryDrawingReportsProgressEveryNPixels = 0,
                ZoomableCanvasTileEdgeLength = 1024
            };
            
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
    }

    internal sealed class StringProgressToLogger : IProgress<string>
    {
        public void Report(string value) { Debug.WriteLine(value); }
    }
}
