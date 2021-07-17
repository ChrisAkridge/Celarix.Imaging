using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging
{
    public sealed class LibraryConfiguration
    {
        public static LibraryConfiguration Instance { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating how many pixels are drawn by types in
        /// BinaryDrawing before it invokes a progress callback.
        /// </summary>
        public int BinaryDrawingReportsProgressEveryNPixels { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether *.url files to LocalPictureServer
        /// will be generated in canvas folder. These links will open LocalPictureServer
        /// to the canvas folder.
        /// </summary>
        public bool GenerateLocalPictureServerLinksInCanvasFolders { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating the base URL for LocalPictureServer.
        /// </summary>
        public string LocalPictureServerBaseUrl { get; set; }
    }
}
