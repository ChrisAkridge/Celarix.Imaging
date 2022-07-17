using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Celarix.Imaging.BinaryDrawing;

namespace Celarix.Imaging.ByteView
{
	static class Program
	{
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            // TODO: Fix the loading of files from command line args (maybe add a
            //       selection dialog for which mode to use)
            // TODO: Remove the PaletteEditorForm
            // TODO: Add a second kind of RAW file, Size-Prefixed Raw (*.sraw)
            //       that has two little-endian Int32s at the beginning which
            //       store the width and height of the image.
            // TODO: Package into an installer and figure out version numbering.
            LibraryConfiguration.Instance = new LibraryConfiguration
            {
                BinaryDrawingReportsProgressEveryNPixels = 1048576,
                ZoomableCanvasTileEdgeLength = 1024
            };
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length == 1)
            {
                string filePath = args[1];
                Application.Run(new MainForm(filePath));
            }
            else { Application.Run(new MainForm()); }
        }
    }
}
