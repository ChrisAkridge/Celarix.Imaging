using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Drawing;
using ST.Library.UI.NodeEditor;
using SkiaSharp;

namespace ST.Library.UI
{
    internal class FrmSTNodePropertyInput : Form
    {
        private STNodePropertyDescriptor m_descriptor;
        private Rectangle m_rect;
        private TextBox m_tbx;

        public FrmSTNodePropertyInput(STNodePropertyDescriptor descriptor) {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            m_rect = descriptor.RectangleR;
            m_descriptor = descriptor;
            ShowInTaskbar = false;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            BackColor = descriptor.Control.AutoColor ? descriptor.Node.TitleColor : descriptor.Control.ItemSelectedColor;
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            Point pt = m_descriptor.Control.PointToScreen(m_rect.Location);
            pt.Y += m_descriptor.Control.ScrollOffset;
            Location = pt;
            Size = new System.Drawing.Size(m_rect.Width + m_rect.Height, m_rect.Height);

            m_tbx = new TextBox();
            m_tbx.Font = m_descriptor.Control.Font;
            m_tbx.ForeColor = m_descriptor.Control.ForeColor;
            m_tbx.BackColor = Color.FromArgb(255, m_descriptor.Control.ItemValueBackColor);
            m_tbx.BorderStyle = BorderStyle.None;

            m_tbx.Size = new Size(Width - 4 - m_rect.Height, Height - 2);
            m_tbx.Text = m_descriptor.GetStringFromValue();
            Controls.Add(m_tbx);
            m_tbx.Location = new Point(2, (Height - m_tbx.Height) / 2);
            m_tbx.SelectAll();
            m_tbx.LostFocus += (s, ea) => Close();
            m_tbx.KeyDown += new KeyEventHandler(tbx_KeyDown);
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
        }

        void tbx_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape) Close();
            if (e.KeyCode != Keys.Enter) return;
            try {
                m_descriptor.SetValue(((TextBox)sender).Text, null);
                m_descriptor.Control.Invalidate();//add rect;
            } catch (Exception ex) {
                m_descriptor.OnSetValueError(ex);
            }
            Close();
        }

        private void InitializeComponent() {
            SuspendLayout();
            // 
            // FrmSTNodePropertyInput
            // 
            ClientSize = new System.Drawing.Size(292, 273);
            Name = "FrmSTNodePropertyInput";
            ResumeLayout(false);
        }
    }
}
