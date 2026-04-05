using System.Drawing;
using System;

// From https://www.codeproject.com/Articles/71225/Image-Viewer-UserControl
namespace Celarix.Imaging.ImagingPlayground.KPImageViewer
{
	/// <summary>
	/// Original class to implement Double Buffering by
	/// NT Almond 
	/// 24 July 2003
	/// 
	/// Extended and adjusted by
	/// Jordy "Kaiwa" Ruiter
	/// </summary>
	class KP_DrawEngine
    {
		private	Graphics? graphics;			// A double-buffered Graphics instance. We create one, but then just seem to use whatever's passed in to draw...
		private Bitmap? memoryBitmap;		// A space for the image with width and height set to the below fields.
		private	int	width;                  // The width of the drawing space.
		private int height;                 // The height of the drawing space.

        /// <summary>
        /// A wrapper around the graphics field.
        /// </summary>
        public Graphics? g => graphics;

        public void Render(Graphics g)
		{
			try
			{
				if (memoryBitmap != null)	// if our drawing space exists
				{
					g.DrawImage(memoryBitmap, new Rectangle(0, 0, width, height), 0, 0, width, height, GraphicsUnit.Pixel);	// draw our space to the graphics
				}
			}
			catch (Exception ex)
			{
                MessageBox.Show("ImageViewer error: " + ex.ToString());
			}
		}

		public bool CreateDoubleBuffer(Graphics g, int width, int height)
		{
            try
            {
                memoryBitmap?.Dispose();
                memoryBitmap = null;
                graphics?.Dispose();
                graphics = null;

                if (width == 0 || height == 0)
                    return false;

                this.width = width;
                this.height = height;

                memoryBitmap = new Bitmap(width, height);
                graphics = Graphics.FromImage(memoryBitmap);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
                return false;
            }
		}
		public bool CanDoubleBuffer()
		{
            try
            {
                return graphics != null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
                return false;
            }
		}

		public static Point GetImageCenter(Size imageSize)
		{
			// For each axis,
			//	If the axis has an even number of pixels,
			//		The center of that axis is (number of pixels / 2)				(100 / 2) == 50
			//	If the axis has an odd number of pixels,
			//		The center of that axis is (((number of pixels - 1) / 2) + 1)	(((55 - 1) / 2) + 1) == ((54 / 2) + 1) == 27 + 1 == 28
			Point result = Point.Empty;

			if (imageSize.Width % 2 == 0)
			{
				result.X = imageSize.Width / 2;
			}
			else
			{
				result.X = ((imageSize.Width - 1) / 2) + 1;
			}

			if (imageSize.Height % 2 == 0)
			{
				result.Y = imageSize.Height / 2;
			}
			else
			{
				result.Y = ((imageSize.Height - 1) / 2) + 1;
			}

			return result;
		}

		public void Dispose()
		{
            graphics?.Dispose();

            memoryBitmap?.Dispose();
        }
	}
}
