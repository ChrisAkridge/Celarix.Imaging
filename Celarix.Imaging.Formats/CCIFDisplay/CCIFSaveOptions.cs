using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Celarix.Imaging.Formats;

namespace CCIFDisplay
{
    public partial class CCIFSaveOptions : Form
    {
        public CCIFCompressionMode CompressionMode =>
            (RadioZlibCompression.Checked) ? CCIFCompressionMode.ZlibCompression : CCIFCompressionMode.NoCompression;
        
        public CCIFSaveOptions()
        {
            InitializeComponent();
        }

        private void ButtonOK_Click(object sender, EventArgs e) { DialogResult = DialogResult.OK; }

        private void ButtonCancel_Click(object sender, EventArgs e) { DialogResult = DialogResult.Cancel; }
    }
}
