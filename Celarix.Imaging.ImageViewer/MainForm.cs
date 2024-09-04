using Celarix.Imaging.ImageViewer.IO;

namespace Celarix.Imaging.ImageViewer
{
	public partial class MainForm : Form
	{
		private const int ScrollbarWidth = 18;

		private FileList fileList = new FileList();
		private ViewerMode viewerMode = ViewerMode.Image;
		private ImageDisplayMode imageDisplayMode = ImageDisplayMode.Standard;
		private SortMode sortMode = SortMode.ByFileName;
		private bool useSmoothedZooming = true;
		private bool editingTextBox = false;
		
		public MainForm()
		{
			InitializeComponent();
		}
	}
}
