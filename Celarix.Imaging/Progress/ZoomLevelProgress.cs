using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.Progress
{
	public sealed class ZoomLevelProgress
	{
		public int ZoomLevel { get; set; }
		public int SavedTileX { get; set; }
		public int SavedTileY { get; set; }
	}
}
