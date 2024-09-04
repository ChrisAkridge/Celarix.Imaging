using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.ImageViewer.IO
{
	internal static class IOHelpers
	{
		public static readonly string[] supportedImageExtensions;

		public static readonly string[] SupportedVideoExtensions;
		
		public static IReadOnlyList<string> supportedExtensions;
		
		static IOHelpers()
		{
			supportedImageExtensions =
			[
				".bmp",
				".gif",
				".jpg",
				".jpeg",
				".png",
				".webp"
			];

			SupportedVideoExtensions =
			[
				".avi",
				".mp4",
				".m4v",
				".mov",
				".flv",
				".mkv",
				".webm",
				".wmv",
				".vob"
			];
			supportedExtensions = supportedImageExtensions.Concat(SupportedVideoExtensions).ToArray();
		}

		public static bool IsSupportedFile(string filePath)
		{
			string extension = Path.GetExtension(filePath).ToLowerInvariant();
			return supportedExtensions.Contains(extension);
		}
	}
}
