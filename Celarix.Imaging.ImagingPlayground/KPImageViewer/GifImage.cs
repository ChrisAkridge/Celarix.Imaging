using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Timers;
namespace Celarix.Imaging.ImagingPlayground.KPImageViewer
{
    // From https://www.codeproject.com/Articles/71225/Image-Viewer-UserControl
    public class GifImage : IDisposable
    {
        private KpImageViewer KpViewer;
        private Image? gif;
        private FrameDimension dimension;
        private int frameCount;
        private int rotation = 0;
        private int currentFrame = 0;
        private Bitmap? currentFrameBmp = null;
        private Size currentFrameSize = new Size();
        private bool updating = false;
        private System.Timers.Timer? timer = null;
        private double framesPerSecond = 0;
        private bool animationEnabled = true;

        public Size CurrentFrameSize
        {
            get
            {
                return currentFrameSize;
            }
        }

        public void Dispose()
        {
            Lock();
            timer?.Enabled = false;
            gif?.Dispose();
            gif = null;
            Unlock();

            timer?.Dispose();
        }

        public double FPS
        {
            get { return (1000.0 / framesPerSecond); }
            set
            {
                if (value <= 30.0 && value > 0.0)
                {
                    framesPerSecond = 1000.0 / value;

                    if (timer != null)
                    {
                        timer.Interval = (int)framesPerSecond;
                    }
                }
            }
        }

        public bool AnimationEnabled
        {
            get { return animationEnabled; }
            set
            {
                animationEnabled = value;

                if (timer != null)
                {
                    timer.Enabled = animationEnabled;
                }
            }
        }

        public GifImage(KpImageViewer KpViewer, Image img, bool animation, double fps)
        {
			updating = true;
            this.KpViewer = KpViewer;
			gif = img;
			dimension = new FrameDimension(gif.FrameDimensionsList[0]);
			frameCount = gif.GetFrameCount(dimension);
			gif.SelectActiveFrame(dimension, 0);
			currentFrame = 0;
			animationEnabled = animation;

			timer = new System.Timers.Timer();

			updating = false;

            framesPerSecond = 1000.0 / fps; // 15 FPS
			timer.Enabled = animationEnabled;
			timer.Interval = framesPerSecond;
			timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);

			currentFrameBmp = (Bitmap)gif;
			currentFrameSize = new Size(currentFrameBmp.Size.Width, currentFrameBmp.Size.Height);
        }

        void timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            NextFrame();
        }

        public bool Lock()
        {
            if (updating == false)
            {
                while (updating)
                {
                    // Wait
                }

                return true;
            }

            return false;
        }

        public void Unlock()
        {
            updating = false;
        }

        public void NextFrame()
        {
            try
            {
                if (gif != null)
                {
                    if (Lock())
                    {
                        lock (gif)
                        {
                            gif.SelectActiveFrame(dimension, currentFrame);
                            currentFrame++;

                            if (currentFrame >= frameCount)
                            {
                                currentFrame = 0;
                            }

                            OnFrameChanged();
                        }
                    }

                    Unlock();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public int Rotation
        {
            get { return rotation; }
        }

        public void Rotate(int rotation)
        {
            this.rotation = (this.rotation + rotation) % 360;
        }

        private void OnFrameChanged()
        {
			currentFrameBmp = (Bitmap?)gif;
			currentFrameSize = new Size(currentFrameBmp?.Size.Width ?? 0, currentFrameBmp?.Size.Height ?? 0);

			KpViewer.InvalidatePanel();
        }

        public Bitmap? CurrentFrame => currentFrameBmp;

        public int FrameCount => frameCount;
    }
}
