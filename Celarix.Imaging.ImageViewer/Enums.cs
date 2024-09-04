using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.ImageViewer
{
	internal enum FileListType
	{
		FolderStandardMode,
		FolderBinaryDrawingMode,
		Playlist
	}

	internal enum SortMode
	{
		ByFileName,
		ByDateCreated,
		ByFileSize
	}

	internal enum ViewerMode
	{
		Image,
		Video
	}

	internal enum ImageDisplayMode
	{
		Standard,
		Comic,
		BinaryDrawing,
		Bitplane
	}
}
