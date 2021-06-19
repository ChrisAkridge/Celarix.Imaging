using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.Tiling
{
	public sealed class TileOptions
	{
		public int TileWidth { get; set; }
		public int TileHeight { get; set; }
		public OrderTilesBy OrderTilesBy { get; set; }
	}
}
