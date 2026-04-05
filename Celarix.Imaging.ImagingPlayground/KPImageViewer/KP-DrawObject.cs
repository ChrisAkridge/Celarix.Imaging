using System;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

// From https://www.codeproject.com/Articles/71225/Image-Viewer-UserControl
namespace Celarix.Imaging.ImagingPlayground.KPImageViewer
{
    public class KP_DrawObject
    {
        private KpImageViewer KpViewer;
        private	Rectangle boundingRect;
		private	Point dragPoint;
		private	bool dragging;
        private Bitmap? bmp;
        private Bitmap? bmpPreview;
        private MultiPageImage? multiBmp;
        private GifImage? gifBmp;

        private double zoom = 1.0;
        private int panelWidth => KpViewer.PanelWidth;
        private int panelHeight => KpViewer.PanelHeight;

        private int rotation = 0;

        private bool multiFrame = false;
        private bool multiPage = false;
        private int pages = 1;
        private int currentPage = 0;

        public Rectangle BoundingBox => boundingRect;

        public void Dispose()
        {
            Image?.Dispose();
        }

        public bool IsDragging => dragging;

        public GifImage Gif => gifBmp;

        public Size OriginalSize
        {
            get
            {
                if (Image != null)
                {
                    if (multiFrame == true)
                    {
                        if (gifBmp != null)
                        {
                            if (gifBmp.Rotation == 0 || gifBmp.Rotation == 180)
                            {
                                return gifBmp.CurrentFrameSize;
                            }
                            else
                            {
                                return new Size(gifBmp.CurrentFrameSize.Height, gifBmp.CurrentFrameSize.Width);
                            }
                        }

                        return Size.Empty;
                    }
                    else
                    {
                        return Image.Size;
                    }
                }
                else
                {
                    return Size.Empty;
                }
            }
        }

        public Size CurrentSize => new Size(boundingRect.Width, boundingRect.Height);

        public bool MultiPage => multiPage;

        public int Pages => pages;

        public int CurrentPage => currentPage;

        public double Zoom => zoom;

        public int Rotation
        {
            get { return rotation; }
            set
            {
                // Making sure that the rotation is only 0, 90, 180 or 270 degrees!
                if (value == 90 || value == 180 || value == 270 || value == 0)
                {
                    rotation = value;
                }
            }
        }

        public Bitmap? GetPage(int pageNumber)
        {
            if (multiBmp == null)
            {
                return null;
            }

            int pages = multiBmp.Image.GetFrameCount(FrameDimension.Page);
            if (pages > pageNumber && pageNumber >= 0)
            {
				multiBmp.Image.SelectActiveFrame(FrameDimension.Page, pageNumber);
                return new Bitmap(multiBmp.Image);
            }

            return null;
        }
        public int? ImageWidth
        {
            get
            {
                if (multiFrame == true)
                {
                    if (gifBmp != null)
                    {
                        if (gifBmp.Rotation == 0 || gifBmp.Rotation == 180)
                        {
                            return gifBmp.CurrentFrameSize.Width;
                        }
                        else
                        {
                            return gifBmp.CurrentFrameSize.Height;
                        }
                    }

                    return 0;
                }
                else
                {
                    return Image?.Width;
                }
            }
        }

        public int? ImageHeight
        {
            get
            {
                if (multiFrame == true)
                {
                    if (gifBmp != null)
                    {
                        if (gifBmp.Rotation == 0 || gifBmp.Rotation == 180)
                        {
                            return gifBmp.CurrentFrameSize.Height;
                        }
                        else
                        {
                            return gifBmp.CurrentFrameSize.Width;
                        }
                    }

                    return 0;
                }
                else
                {
                    return Image?.Height;
                }
            }
        }

        public Bitmap? Image
        {
            get 
            {
                if (multiFrame == true)
                {
                    return gifBmp.CurrentFrame;
                }
                else if (multiPage == true)
                {
                    if (multiBmp != null)
                    {
                        return multiBmp.Page;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return bmp;
                }
            }
            set
            {
                try
                {
                    if (value != null)
                    {
                        currentPage = 0;

                        // No memory leaks here!
                        bmp?.Dispose();
                        bmp = null;
                        multiBmp?.Dispose();
                        multiBmp = null;

                        try
                        {
                            FrameDimension gifDimension = new FrameDimension(value.FrameDimensionsList[0]);
                            int gifFrames = value.GetFrameCount(gifDimension);

                            if (gifFrames > 1)
                            {
                                multiFrame = true;
                            }
                            else
                            {
                                multiFrame = false;
                            }

                            if (!multiFrame)
                            {
                                //Gets the total number of frames in the .tiff file
                                pages = value.GetFrameCount(FrameDimension.Page);
                                if (pages > 1) { multiPage = true; } else { multiPage = false; }
                            }
                        }
                        catch
                        {
                            multiPage = false;
                            pages = 1;
                        }

                        if (multiFrame == true)
                        {
							gifBmp = new GifImage(KpViewer, value, KpViewer.GifAnimation, KpViewer.GifFPS);
                        }
                        else if (multiPage == true)
                        {
							bmp = null;

							multiBmp = new MultiPageImage(value);
                        }
                        else
                        {
							bmp = value;
							multiBmp = null;
                        }

                        if (Image == null) { return; }

                        // Initial rotation adjustments
                        if (rotation != 0)
                        {
                            if (rotation == 180)
                            {
								Image?.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                boundingRect = new Rectangle(0, 0, (int)(ImageWidth! * zoom), (int)(ImageHeight! * zoom));
                            }
                            else
                            {
                                if (rotation == 90)
                                {
									Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                }
                                else if (rotation == 270)
                                {
									Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                }

                                // Flip the X and Y values
                                boundingRect = new Rectangle(0, 0, (int)(ImageHeight! * zoom), (int)(ImageWidth! * zoom));
                            }
                        }
                        else
                        {
							Image?.RotateFlip(RotateFlipType.RotateNoneFlipNone);
                            boundingRect = new Rectangle(0, 0, (int)(ImageWidth! * zoom), (int)(ImageHeight! * zoom));
                        }

                        zoom = 1.0;
                        bmpPreview = CreatePreviewImage();
                        FitToScreen();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ImageViewer error: " + ex.ToString());
                }
            }
        }

        public Image PreviewImage => bmpPreview;

        public string ImagePath
        {
            set
            {
                try
                {
                    // No memory leaks here!
                    bmp?.Dispose();
                    bmp = null;
                    multiBmp?.Dispose();
                    multiBmp = null;

                    Bitmap? temp = null;

                    // Make sure it does not crash on incorrect image formats
                    try
                    {
                        //temp = (Bitmap)Bitmap.FromFile(value);
                        temp = new Bitmap(value);
                    }
                    catch
                    {
                        temp = null;
                        MessageBox.Show("ImageViewer error: Incorrect image format!");
                    }

                    if (temp != null)
                    {
                        currentPage = 0;

                        try
                        {
                            string extension = Path.GetExtension(value);

                            if (extension == ".gif")
                            {
                                FrameDimension gifDimension = new FrameDimension(temp.FrameDimensionsList[0]);
                                int gifFrames = temp.GetFrameCount(gifDimension);

                                if (gifFrames > 1)
                                {
                                    multiFrame = true;
                                }
                                else
                                {
                                    multiFrame = false;
                                }
                            }
                            else
                            {
                                multiFrame = false;

                                //Gets the total number of frames in the .tiff file
                                pages = temp.GetFrameCount(FrameDimension.Page);
                                if (pages > 1) { multiPage = true; } else { multiPage = false; }
                            }
                        }
                        catch
                        {
                            multiPage = false;
                            pages = 1;
                        }

                        if (multiFrame == true)
                        {
							gifBmp = new GifImage(KpViewer, temp, KpViewer.GifAnimation, KpViewer.GifFPS);
                        }
                        else if (multiPage == true)
                        {
							bmp = null;

							multiBmp = new MultiPageImage(temp);
                        }
                        else
                        {
							bmp = temp;
							multiBmp = null;
                        }

                        if (Image == null) { return; }

                        // Initial rotation
                        if (rotation != 0)
                        {
                            if (rotation == 180)
                            {
								Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                boundingRect = new Rectangle(0, 0, (int)(ImageWidth! * zoom), (int)(ImageHeight! * zoom));
                            }
                            else
                            {
                                if (rotation == 90)
                                {
									Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                }
                                else if (rotation == 270)
                                {
									Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                }

                                // Flipping X and Y values!
                                boundingRect = new Rectangle(0, 0, (int)(ImageHeight! * zoom), (int)(ImageWidth! * zoom));
                            }
                        }
                        else
                        {
							Image.RotateFlip(RotateFlipType.RotateNoneFlipNone);
                            boundingRect = new Rectangle(0, 0, (int)(ImageWidth! * zoom), (int)(ImageHeight! * zoom));
                        }

                        zoom = 1.0;
                        bmpPreview = CreatePreviewImage();
                        FitToScreen();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ImageViewer error: " + ex.ToString());
                }
            }
        }

        public KP_DrawObject(KpImageViewer KpViewer, Bitmap bmp)
        {
            try
            {
                this.KpViewer = KpViewer;

                // Initial dragging to false and an Image.
                dragging = false;
				Image = bmp;
				Image.RotateFlip(RotateFlipType.RotateNoneFlipNone);

                boundingRect = new Rectangle(0, 0, (int)(ImageWidth! * zoom), (int)(ImageHeight! * zoom));
            }
            catch(Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        private ImageCodecInfo? GetCodec(string type)
        {
            ImageCodecInfo[] info = ImageCodecInfo.GetImageEncoders();

            for (int i = 0; i < info.Length; i++)
            {
                string EnumName = type.ToString();
                if (info[i].FormatDescription?.Equals(EnumName) == true)
                {
                    return info[i];
                }
            }
            return null;
        }

        public void SetPage(int page)
        {
            int p = page - 1;

            try
            {
                if (Image != null && multiBmp != null && multiPage == true)
                {
                    if (p < pages && p >= 0)
                    {
                        currentPage = p;

						multiBmp.SetPage(p);
						multiBmp.Rotate(rotation);

                        // No memory leaks here!
                        bmpPreview?.Dispose();
                        bmpPreview = null;

                        bmpPreview = CreatePreviewImage();
                        AvoidOutOfScreen();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        public void NextPage()
        {
            try
            {
                if (Image != null && multiBmp != null && multiPage == true)
                {
                    int nextPage = currentPage + 1;

                    if (nextPage < pages)
                    {
                        currentPage = nextPage;

						multiBmp.SetPage(currentPage);
						multiBmp.Rotate(rotation);

                        // No memory leaks here!
                        bmpPreview?.Dispose();
                        bmpPreview = null;

                        bmpPreview = CreatePreviewImage();
                        AvoidOutOfScreen();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        public void PreviousPage()
        {
            try
            {
                if (Image != null && multiBmp != null && multiPage == true)
                {
                    int prevPage = currentPage - 1;

                    if (prevPage >= 0)
                    {
                        currentPage = prevPage;

						multiBmp.SetPage(currentPage);
						multiBmp.Rotate(rotation);

                        // No memory leaks here!
                        bmpPreview?.Dispose();
                        bmpPreview = null;

                        bmpPreview = CreatePreviewImage();
                        AvoidOutOfScreen();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        public KP_DrawObject(KpImageViewer KpViewer)
        {
            try
            {
                this.KpViewer = KpViewer;
                // Initial dragging to false and No image.
                dragging = false;
				bmp = null;
				multiBmp = null;
				gifBmp = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        public void Rotate90()
        {
            try
            {
                if (Image != null)
                {
                    int tempWidth = boundingRect.Width;
                    int tempHeight = boundingRect.Height;

                    boundingRect.Width = tempHeight;
                    boundingRect.Height = tempWidth;

                    rotation = (rotation + 90) % 360;

                    if (multiFrame == true)
                    {
						gifBmp?.Rotate(90);
                    }
                    else if (MultiPage == true)
                    {
                        multiBmp?.Rotate(90);
                    }
                    else
                    {
						Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    }

                    AvoidOutOfScreen();

                    // No memory leaks here!
                    bmpPreview?.Dispose();
                    bmpPreview = null;

                    bmpPreview = CreatePreviewImage();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        public void Rotate180()
        {
            try
            {
                if (Image != null)
                {
                    int tempWidth = boundingRect.Width;
                    int tempHeight = boundingRect.Height;

                    boundingRect.Width = tempHeight;
                    boundingRect.Height = tempWidth;

                    rotation = (rotation + 180) % 360;

                    if (multiFrame == true)
                    {
						gifBmp?.Rotate(180);
                    }
                    else if (MultiPage == true)
                    {
                        multiBmp?.Rotate(180);
                    }
                    else
                    {
						Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    }

                    AvoidOutOfScreen();

                    // No memory leaks here!
                    bmpPreview?.Dispose();
                    bmpPreview = null;

                    bmpPreview = CreatePreviewImage();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        public void Rotate270()
        {
            try
            {
                if (Image != null)
                {
                    int tempWidth = boundingRect.Width;
                    int tempHeight = boundingRect.Height;

                    boundingRect.Width = tempHeight;
                    boundingRect.Height = tempWidth;

                    rotation = (rotation + 270) % 360;

                    if (multiFrame == true)
                    {
						gifBmp?.Rotate(270);
                    }
                    else if (MultiPage == true)
                    {
                        multiBmp?.Rotate(270);
                    }
                    else
                    {
						Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    }

                    AvoidOutOfScreen();

                    // No memory leaks here!
                    bmpPreview?.Dispose();
                    bmpPreview = null;

                    bmpPreview = CreatePreviewImage();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        private Bitmap RotateCenter(Bitmap bmpSrc, float theta)
        {
            if(theta == 180.0f)
            {
                Bitmap bmpDest = new Bitmap(bmpSrc.Width, bmpSrc.Height);
                Graphics gDest = Graphics.FromImage(bmpDest);

                gDest.DrawImage(bmpSrc, new Point(0, 0));
                
                bmpDest.RotateFlip(RotateFlipType.Rotate180FlipNone);

                return bmpDest;
            }
            else
            {
                Matrix mRotate = new Matrix();
                mRotate.Translate(bmpSrc.Width / -2, bmpSrc.Height / -2, MatrixOrder.Append);
                mRotate.RotateAt(theta, new Point(0, 0), MatrixOrder.Append);

                GraphicsPath gp = new GraphicsPath();
                // transform image points by rotation matrix
                gp.AddPolygon([new Point(0, 0), new Point(bmpSrc.Width, 0), new Point(0, bmpSrc.Height)]);
                gp.Transform(mRotate);
                PointF[] pts = gp.PathPoints;

                // create destination bitmap sized to contain rotated source image
                Rectangle bbox = RotateBoundingBox(bmpSrc, mRotate);
                Bitmap bmpDest = new Bitmap(bbox.Width, bbox.Height);

                Graphics gDest = Graphics.FromImage(bmpDest);
                Matrix mDest = new Matrix();
                mDest.Translate(bmpDest.Width / 2, bmpDest.Height / 2, MatrixOrder.Append);
                gDest.Transform = mDest;
                gDest.DrawImage(bmpSrc, pts);
                gDest.DrawRectangle(Pens.Red, bbox);
                gDest.Dispose();
                gp.Dispose();

                return bmpDest;
            }
        }

        private static Rectangle RotateBoundingBox(Image img, Matrix matrix)
        {
            GraphicsUnit gu = new GraphicsUnit();
            Rectangle rImg = Rectangle.Round(img.GetBounds(ref gu));

            // Transform the four points of the image, to get the resized bounding box.
            Point topLeft = new Point(rImg.Left, rImg.Top);
            Point topRight = new Point(rImg.Right, rImg.Top);
            Point bottomRight = new Point(rImg.Right, rImg.Bottom);
            Point bottomLeft = new Point(rImg.Left, rImg.Bottom);
            Point[] points = new Point[] { topLeft, topRight, bottomRight, bottomLeft };
            GraphicsPath gp = new GraphicsPath(points, new byte[] { (byte)PathPointType.Start, (byte)PathPointType.Line, (byte)PathPointType.Line, (byte)PathPointType.Line });
            gp.Transform(matrix);
            return Rectangle.Round(gp.GetBounds());
        }

        private Bitmap CreatePreviewImage()
        {
            // 148 && 117 as initial and default size for the preview panel.
            Rectangle previewRect = new Rectangle(0, 0, 148, 117);

            double x_ratio = (double)previewRect.Width / (double)BoundingBox.Width;
            double y_ratio = (double)previewRect.Height / (double)BoundingBox.Height;

            if ((BoundingBox.Width <= previewRect.Width) && (BoundingBox.Height <= previewRect.Height))
            {
                previewRect.Width = BoundingBox.Width;
                previewRect.Height = BoundingBox.Height;
            }
            else if ((x_ratio * BoundingBox.Height) < previewRect.Height)
            {
                previewRect.Height = Convert.ToInt32(Math.Ceiling(x_ratio * BoundingBox.Height));
                previewRect.Width = previewRect.Width;
            }
            else
            {
                previewRect.Width = Convert.ToInt32(Math.Ceiling(y_ratio * BoundingBox.Width));
                previewRect.Height = previewRect.Height;
            }

            Bitmap bmp = new Bitmap(previewRect.Width, previewRect.Height);

            if (multiFrame == true)
            {
                if (gifBmp != null)
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        if (gifBmp.Lock())
                        {
                            lock (gifBmp.CurrentFrame)
                            {
                                if (gifBmp.Rotation != 0)
                                {
                                    g.DrawImage(RotateCenter(gifBmp.CurrentFrame, gifBmp.Rotation), previewRect);
                                }
                                else
                                {
                                    g.DrawImage(gifBmp.CurrentFrame, previewRect);
                                }
                            }
                        }

						gifBmp.Unlock();
                    }
                }
            }
            else
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    if (Image != null)
                    {
                        g.DrawImage(Image, previewRect);
                    }
                }
            }

            return bmp;
        }

        public void ZoomToSelection(Rectangle selection, Point ptPbFull)
        {
            int x = (selection.X - ptPbFull.X);
            int y = (selection.Y - ptPbFull.Y);
            int width = selection.Width;
            int height = selection.Height;

            // So, where did my selection start on the entire picture?
            int selectedX = (int)((double)(((double)boundingRect.X - ((double)boundingRect.X * 2)) + (double)x) / zoom);
            int selectedY = (int)((double)(((double)boundingRect.Y - ((double)boundingRect.Y * 2)) + (double)y) / zoom);
            int selectedWidth = width;
            int selectedHeight = height;

            // The selection width on the scale of the Original size!
            if (zoom < 1.0 || zoom > 1.0)
            {
                selectedWidth = Convert.ToInt32((double)width / zoom);
                selectedHeight = Convert.ToInt32((double)height / zoom);
            }

            // What is the highest possible zoomrate?
            double zoomX = ((double)panelWidth / (double)selectedWidth);
            double zoomY = ((double)panelHeight / (double)selectedHeight);

            double newZoom = Math.Min(zoomX, zoomY);

            // Avoid Int32 crashes!
            if (newZoom * 100 < Int32.MaxValue && newZoom * 100 > Int32.MinValue)
            {
                SetZoom(newZoom);

                selectedWidth = (int)((double)selectedWidth * newZoom);
                selectedHeight = (int)((double)selectedHeight * newZoom);

                // Center the selected area
                int offsetX = 0;
                int offsetY = 0;
                if (selectedWidth < panelWidth)
                {
                    offsetX = (panelWidth - selectedWidth) / 2;
                }
                if (selectedHeight < panelHeight)
                {
                    offsetY = (panelHeight - selectedHeight) / 2;
                }

                boundingRect.X = (int)((int)((double)selectedX * newZoom) - ((int)((double)selectedX * newZoom) * 2)) + offsetX;
                boundingRect.Y = (int)((int)((double)selectedY * newZoom) - ((int)((double)selectedY * newZoom) * 2)) + offsetY;

                AvoidOutOfScreen();
            }
        }

        public void JumpToOrigin(int x, int y, int width, int height, int pWidth, int pHeight)
        {
            try
            {
                double zoom = (double)boundingRect.Width / (double)width;

                int originX = (int)(x * zoom);
                int originY = (int)(y * zoom);

                originX = originX - (originX * 2);
                originY = originY - (originY * 2);

                boundingRect.X = originX + (pWidth / 2);
                boundingRect.Y = originY + (pHeight / 2);

                AvoidOutOfScreen();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        public void JumpToOrigin(int x, int y, int width, int height)
        {
            try
            {
                boundingRect.X = (x - (width / 2)) - ((x - (width / 2)) * 2);
                boundingRect.Y = (y - (height / 2)) - ((y - (height / 2)) * 2);

                AvoidOutOfScreen();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        public Point PointToOrigin(int x, int y, int width, int height)
        {
            try
            {
                double zoomX = (double)width / (double)boundingRect.Width;
                double zoomY = (double)height / (double)boundingRect.Height;

                if (width > panelWidth)
                {
                    int oldX = (boundingRect.X - (boundingRect.X * 2)) + (panelWidth / 2);
                    int oldY = (boundingRect.Y - (boundingRect.Y * 2)) + (panelHeight / 2);

                    int newX = (int)(oldX * zoomX);
                    int newY = (int)(oldY * zoomY);

                    int originX = newX - (panelWidth / 2) - ((newX - (panelWidth / 2)) * 2);
                    int originY = newY - (panelHeight / 2) - ((newY - (panelHeight / 2)) * 2);

                    return new Point(originX, originY);
                }
                else
                {
                    if (height > panelHeight)
                    {
                        int oldY = (boundingRect.Y - (boundingRect.Y * 2)) + (panelHeight / 2);

                        int newY = (int)(oldY * zoomY);

                        int originY = newY - (panelHeight / 2) - ((newY - (panelHeight / 2)) * 2);

                        return new Point(0, originY);
                    }
                    else
                    {
                        return new Point(0, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
                return new Point(0, 0);
            }
        }

        public void ZoomIn()
        {
            try
            {
                if (Image != null)
                {
                    // Make sure zoom steps are with 25%
                    double index = 0.25 - (zoom % 0.25);
                    
                    if(index != 0)
                    {
                        zoom += index;
                    }
                    else
                    {
                        zoom += 0.25;
                    }

                    Point p = PointToOrigin(boundingRect.X, boundingRect.Y, (int)(ImageWidth * zoom), (int)(ImageHeight * zoom));

                    boundingRect = new Rectangle(p.X, p.Y, (int)(ImageWidth * zoom), (int)(ImageHeight * zoom));
                    AvoidOutOfScreen();
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        public void ZoomOut()
        {
            try
            {
                if (Image != null)
                {
                    // Make sure zoom steps are with 25% and higher than 0%
                    if (zoom - 0.25 > 0)
                    {
                        if (((zoom - 0.25) % 0.25) != 0)
                        {
                            zoom -= zoom % 0.25;
                        }
                        else
                        {
                            zoom -= 0.25;
                        }
                    }

                    Point p = PointToOrigin(boundingRect.X, boundingRect.Y, (int)(ImageWidth * zoom), (int)(ImageHeight * zoom));

                    boundingRect = new Rectangle(p.X, p.Y, (int)(ImageWidth * zoom), (int)(ImageHeight * zoom));
                    AvoidOutOfScreen();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        public void SetPosition(int x, int y)
        {
            boundingRect.X = -x;
            boundingRect.Y = -y;
        }

        public void SetPositionX(int x)
        {
            boundingRect.X = -x;
        }

        public void SetPositionY(int y)
        {
            boundingRect.Y = -y;
        }

        public void SetZoom(double z)
        {
            try
            {
                if (Image != null)
                {
                    zoom = z;

                    Point p = PointToOrigin(boundingRect.X, boundingRect.Y, (int)(ImageWidth * zoom), (int)(ImageHeight * zoom));

                    boundingRect = new Rectangle(p.X, p.Y, (int)(ImageWidth * zoom), (int)(ImageHeight * zoom));
                    AvoidOutOfScreen();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        public void Scroll(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                if (Image != null)
                {
                    if (e.Delta < 0)
                    {
                        ZoomOut();
                    }
                    else
                    {
                        ZoomIn();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        public void FitToScreen()
        {
            try
            {
                if (Image != null)
                {
                    double x_ratio = (double)panelWidth / (double)ImageWidth;
                    double y_ratio = (double)panelHeight / (double)ImageHeight;

                    if ((ImageWidth <= panelWidth) && (ImageHeight <= panelHeight))
                    {
                        boundingRect.Width = ImageWidth!.Value;
                        boundingRect.Height = ImageHeight!.Value;
                    }
                    else if ((x_ratio * ImageHeight) < panelHeight)
                    {
                        boundingRect.Height = Convert.ToInt32(Math.Ceiling(x_ratio * ImageHeight!.Value));
                        boundingRect.Width = panelWidth;
                    }
                    else
                    {
                        boundingRect.Width = Convert.ToInt32(Math.Ceiling(y_ratio * ImageWidth!.Value));
                        boundingRect.Height = panelHeight;
                    }

                    boundingRect.X = 0;
                    boundingRect.Y = 0;

                    zoom = ((double)boundingRect.Width / (double)ImageWidth);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

        public void AvoidOutOfScreen()
        {
            try
            {
                // Am I lined out to the left?
                if (boundingRect.X >= 0)
                {
                    boundingRect.X = 0;
                }
                else if ((boundingRect.X <= (boundingRect.Width - panelWidth) - ((boundingRect.Width - panelWidth) * 2)))
                {
                    if ((boundingRect.Width - panelWidth) - ((boundingRect.Width - panelWidth) * 2) <= 0)
                    {
                        // I am too far to the left!
                        boundingRect.X = (boundingRect.Width - panelWidth) - ((boundingRect.Width - panelWidth) * 2);
                    }
                    else
                    {
                        // I am too far to the right!
                        boundingRect.X = 0;
                    }
                }

                // Am I lined out to the top?
                if (boundingRect.Y >= 0)
                {
                    boundingRect.Y = 0;
                }
                else if ((boundingRect.Y <= (boundingRect.Height - panelHeight) - ((boundingRect.Height - panelHeight) * 2)))
                {
                    if ((boundingRect.Height - panelHeight) - ((boundingRect.Height - panelHeight) * 2) <= 0)
                    {
                        // I am too far to the top!
                        boundingRect.Y = (boundingRect.Height - panelHeight) - ((boundingRect.Height - panelHeight) * 2);
                    }
                    else
                    {
                        // I am too far to the bottom!
                        boundingRect.Y = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
        }

		public void Drag(Point pt)
		{
            try
            {
                if (Image != null)
                {
                    if (dragging == true)
                    {
                        // Am I dragging it outside of the panel?
                        if ((pt.X - dragPoint.X > (boundingRect.Width - panelWidth) - ((boundingRect.Width - panelWidth) * 2)) && (pt.X - dragPoint.X < 0))
                        {
                            // No, everything is just fine
                            boundingRect.X = pt.X - dragPoint.X;
                        }
                        else if ((pt.X - dragPoint.X > 0))
                        {
                            // Now don't drag it out of the panel please
                            boundingRect.X = 0;
                        }
                        else if((pt.X - dragPoint.X < (boundingRect.Width - panelWidth) - ((boundingRect.Width - panelWidth) * 2)))
                        {
                            // I am dragging it out of my panel. How many pixels do I have left?
                            if ((boundingRect.Width - panelWidth) - ((boundingRect.Width - panelWidth) * 2) <= 0)
                            {
                                // Make it fit perfectly
                                boundingRect.X = (boundingRect.Width - panelWidth) - ((boundingRect.Width - panelWidth) * 2);
                            }
                        }

                        // Am I dragging it outside of the panel?
                        if (pt.Y - dragPoint.Y > (boundingRect.Height - panelHeight) - ((boundingRect.Height - panelHeight) * 2) && (pt.Y - dragPoint.Y < 0))
                        {
                            // No, everything is just fine
                            boundingRect.Y = pt.Y - dragPoint.Y;
                        }
                        else if ((pt.Y - dragPoint.Y > 0))
                        {
                            // Now don't drag it out of the panel please
                            boundingRect.Y = 0;
                        }
                        else if (pt.Y - dragPoint.Y < (boundingRect.Height - panelHeight) - ((boundingRect.Height - panelHeight) * 2))
                        {
                            // I am dragging it out of my panel. How many pixels do I have left?
                            if ((boundingRect.Height - panelHeight) - ((boundingRect.Height - panelHeight) * 2) <= 0)
                            {
                                // Make it fit perfectly
                                boundingRect.Y = (boundingRect.Height - panelHeight) - ((boundingRect.Height - panelHeight) * 2);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
		}

		public void BeginDrag(Point pt)
		{
            try
            {
                if (Image != null)
                {
                    // Initial drag position
                    dragPoint.X = pt.X - boundingRect.X;
                    dragPoint.Y = pt.Y - boundingRect.Y;
                    dragging = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
		}

		public void EndDrag()
        {
            try
            {
                if (Image != null)
                {
                    dragging = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
		}

		public void Draw(Graphics g)
		{
            try
            {
                if (multiFrame == true)
                {
                    if (gifBmp?.CurrentFrame != null)
                    {
                        if(gifBmp.Lock())
                        {
                            lock (gifBmp.CurrentFrame)
                            {
                                if (gifBmp.Rotation != 0)
                                {
                                    Bitmap rotated = RotateCenter(gifBmp.CurrentFrame, gifBmp.Rotation);
                                    g.DrawImage(rotated, boundingRect);

                                    rotated.Dispose();
                                }
                                else
                                {
                                    g.DrawImage(gifBmp.CurrentFrame, boundingRect);
                                }
                            }
                        }
						gifBmp.Unlock();
                    }
                }
                if (multiPage == true)
                {
                    if (multiBmp != null)
                    {
                        if (multiBmp.Image != null)
                        {
                            g.DrawImage(multiBmp.Image, boundingRect);
                        }
                    }
                }
                else
                {
                    if (bmp != null)
                    {
                        g.DrawImage(bmp, boundingRect);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
		}
    }
}
