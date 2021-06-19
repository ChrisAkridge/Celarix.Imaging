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
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void TSBOpenImage_Click(object sender, EventArgs e)
        {
            if (OFDOpenImage.ShowDialog() != DialogResult.OK) return;
            
            string imagePath = OFDOpenImage.FileName;
            Picture.Image = Image.FromFile(imagePath);
        }

        private void TSBOpenCCIF_Click(object sender, EventArgs e)
        {
            if (OFDOpenCCIF.ShowDialog() != DialogResult.OK) return;

            string ccifPath = OFDOpenCCIF.FileName;
            MessageBox.Show("Open CCIF!");
        }

        private void TSBSaveAsCCIF_Click(object sender, EventArgs e)
        {
            if (SFDSaveCCIF.ShowDialog() != DialogResult.OK) return;

            string ccifPath = SFDSaveCCIF.FileName;
            var saveOptionsForm = new CCIFSaveOptions();
            saveOptionsForm.ShowDialog();

            if (saveOptionsForm.DialogResult != DialogResult.OK) return;

            CCIFCompressionMode compressionMode = saveOptionsForm.CompressionMode;
            
            MessageBox.Show("Save CCIF!");
        }
    }
}
