using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Celarix.Imaging.ImagingPlayground
{
    public partial class ViewerForm : Form
    {
        public ViewerForm()
        {
            InitializeComponent();
        }

        public void DisplaySingleImage(Image<Rgba32> image)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => DisplaySingleImage(image)));
                return;
            }
            
            infiniteCanvasControl1.LoadInMemoryImage(image);
        }
    }
}
