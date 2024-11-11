using System;
using System.Windows.Forms;

namespace Celarix.Imaging.ByteView
{
	/// <summary>
	/// A form used to select the size of image displayed for files loaded as raw bytes.
	/// </summary>
	public partial class RawImageSizeForm : Form
    {
		/// <summary>
		/// Gets the desired width of the image as specified by the user.
		/// </summary>
        public int? ImageWidth
        {
            get
            {
	            if (string.IsNullOrWhiteSpace(TextBoxWidth.Text))
	            {
		            return null;
	            }
                if (!int.TryParse(TextBoxWidth.Text, out int result))
                {
                    MessageBox.Show("Invalid width.", "Invalid Width", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                return result;
            }
        }

		/// <summary>
		/// Gets the desired height of the image as specified by the user.
		/// </summary>
		public int? ImageHeight
        {
            get
            {
	            if (string.IsNullOrWhiteSpace(TextBoxHeight.Text))
	            {
		            return null;
	            }
                if (!int.TryParse(TextBoxHeight.Text, out int result))
                {
                    MessageBox.Show("Invalid height.", "Invalid Height", MessageBoxButtons.OK, 
						MessageBoxIcon.Error);
                }
                return result;
            }
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="RawImageSizeForm"/> class.
		/// </summary>
        public RawImageSizeForm()
        {
            InitializeComponent();
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
			DialogResult = DialogResult.OK;
			Close();
        }
    }
}
