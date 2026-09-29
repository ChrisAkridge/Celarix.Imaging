using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Celarix.Imaging.ByteView
{
	/// <summary>
	/// A PictureBox that scales with nearest-neighbor sampling, so that zooming in shows crisp pixels instead of
	/// a blur, which matters when individual pixels carry meaning (bit planes, color maps).
	/// </summary>
	public class PixelatedPictureBox : PictureBox
	{
		protected override void OnPaint(PaintEventArgs pe)
		{
			pe.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
			pe.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
			base.OnPaint(pe);
		}
	}
}
