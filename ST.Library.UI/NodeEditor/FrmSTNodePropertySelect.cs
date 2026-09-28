using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Drawing;
using SkiaSharp;

namespace ST.Library.UI.NodeEditor
{
    internal class FrmSTNodePropertySelect : Form
    {
        private STNodePropertyDescriptor m_descriptor;
        private int m_nItemHeight = 25;

        private static Type m_t_bool = typeof(bool);
        private Color m_clr_item_1 = Color.FromArgb(10, 0, 0, 0);// Color.FromArgb(255, 40, 40, 40);
        private Color m_clr_item_2 = Color.FromArgb(10, 255, 255, 255);// Color.FromArgb(255, 50, 50, 50);
        private object m_item_hover;

        public FrmSTNodePropertySelect(STNodePropertyDescriptor descriptor) {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            m_descriptor = descriptor;
            Size = descriptor.RectangleR.Size;
            ShowInTaskbar = false;
            BackColor = descriptor.Control.BackColor;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        }

        private List<object> m_lst_item = new List<object>();

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            Point pt = m_descriptor.Control.PointToScreen(m_descriptor.RectangleR.Location);
            pt.Y += m_descriptor.Control.ScrollOffset;
            Location = pt;
            if (m_descriptor.PropertyInfo.PropertyType.IsEnum) {
                foreach (var v in Enum.GetValues(m_descriptor.PropertyInfo.PropertyType)) m_lst_item.Add(v);
            } else if (m_descriptor.PropertyInfo.PropertyType == m_t_bool) {
                m_lst_item.Add(true);
                m_lst_item.Add(false);
            } else {
                Close();
                return;
            }
            Height = m_lst_item.Count * m_nItemHeight;
            Rectangle rect = Screen.GetWorkingArea(this);
            if (Bottom > rect.Bottom) Top -= (Bottom - rect.Bottom);
            MouseLeave += (s, ea) => Close();
            LostFocus += (s, ea) => Close();
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            int nIndex = e.Y / m_nItemHeight;
            if (nIndex < 0 || nIndex >= m_lst_item.Count) return;
            var item = m_lst_item[e.Y / m_nItemHeight];
            if (m_item_hover == item) return;
            m_item_hover = item;
            Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs e) {
            base.OnMouseClick(e);
            Close();
            int nIndex = e.Y / m_nItemHeight;
            if (nIndex < 0) return;
            if (nIndex > m_lst_item.Count) return;
            try {
                m_descriptor.SetValue(m_lst_item[nIndex], null);
            } catch (Exception ex) {
                m_descriptor.OnSetValueError(ex);
            }
        }
    }
}
