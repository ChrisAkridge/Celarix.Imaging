// From https://www.codeproject.com/Articles/71225/Image-Viewer-UserControl
namespace Celarix.Imaging.ImagingPlayground.KPImageViewer
{
    public class PanelDoubleBuffered : Panel
    {
        public PanelDoubleBuffered()
        {
			DoubleBuffered = true;
			UpdateStyles();
        }
    }
}
