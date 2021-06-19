using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.Packing
{
	public sealed class PackingOptions
	{
		public string InputFolderPath { get; set; }
		public string OutputPath { get; set; }
		public bool Recursive { get; set; }
		public bool Multipicture { get; set; }
	}
}
