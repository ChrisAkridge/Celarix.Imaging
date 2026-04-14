namespace Celarix.Imaging.ImagingPlayground
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            LibraryConfiguration.Instance = new LibraryConfiguration
            {
                BinaryDrawingReportsProgressEveryNPixels = 1048576,
                ZoomableCanvasTileEdgeLength = 1024
            };

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}