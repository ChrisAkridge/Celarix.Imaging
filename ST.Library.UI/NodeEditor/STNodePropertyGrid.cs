using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.ComponentModel;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
/*
MIT License

Copyright (c) 2021 DebugST@crystal_lz

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */
/*
 * create: 2021-01-28
 * modify: 2021-03-02
 * Author: Crystal_lz
 * blog: http://st233.com
 * Gitee: https://gitee.com/DebugST
 * Github: https://github.com/DebugST
 */
namespace ST.Library.UI.NodeEditor
{
    /// <summary>
    /// STNode节点属性编辑器
    /// </summary>
    public class STNodePropertyGrid : SKControl
    {
        #region properties ==========

        private STNode _STNode;
        /// <summary>
        /// 当前显示的STNode
        /// </summary>
        [Description("当前显示的STNode"), Browsable(false)]
        public STNode STNode {
            get { return _STNode; }
        }

        private Color _ItemHoverColor = Color.FromArgb(50, 125, 125, 125);
        /// <summary>
        /// 获取或设置属性选项被鼠标悬停时候背景色
        /// </summary>
        [Description("获取或设置属性选项被鼠标悬停时候背景色")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color ItemHoverColor {
            get { return _ItemHoverColor; }
            set { _ItemHoverColor = value; }
        }

        private Color _ItemSelectedColor = Color.DodgerBlue;
        /// <summary>
        /// 获取或设置属性选项被选中时候背景色 当AutoColor被设置时此属性不能被设置
        /// </summary>
        [Description("获取或设置属性选项被选中时候背景色 当AutoColor被设置时此属性不能被设置"), DefaultValue(typeof(Color), "DodgerBlue")]
        public Color ItemSelectedColor {
            get { return _ItemSelectedColor; }
            set {
                if (_AutoColor) return;
                if (value == _ItemSelectedColor) return;
                _ItemSelectedColor = value;
                Invalidate();
            }
        }

        private Color _ItemValueBackColor = Color.FromArgb(255, 80, 80, 80);
        /// <summary>
        /// 获取或设置属性选项值背景色
        /// </summary>
        [Description("获取或设置属性选项值背景色")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color ItemValueBackColor {
            get { return _ItemValueBackColor; }
            set {
                _ItemValueBackColor = value;
                Invalidate();
            }
        }

        private Color _TitleColor = Color.FromArgb(127, 0, 0, 0);
        /// <summary>
        /// 获取或设置默认标题背景色
        /// </summary>
        [Description("获取或设置默认标题背景色")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color TitleColor {
            get { return _TitleColor; }
            set {
                _TitleColor = value;
                if (!_ShowTitle) return;
                Invalidate(m_rect_title);
            }
        }

        private Color _ErrorColor = Color.FromArgb(200, Color.Brown);
        /// <summary>
        /// 获取或设置属性设置错误时候提示信息背景色
        /// </summary>
        [Description("获取或设置属性设置错误时候提示信息背景色")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color ErrorColor {
            get { return _ErrorColor; }
            set { _ErrorColor = value; }
        }

        private Color _DescriptionColor = Color.FromArgb(200, Color.DarkGoldenrod);
        /// <summary>
        /// 获取或设置属性描述信息背景色
        /// </summary>
        [Description("获取或设置属性描述信息背景色")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color DescriptionColor {
            get { return _DescriptionColor; }
            set { _DescriptionColor = value; }
        }

        private bool _ShowTitle = true;
        /// <summary>
        /// 获取或设置是否显示节点标题
        /// </summary>
        [Description("获取或设置是否显示节点标题")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowTitle {
            get { return _ShowTitle; }
            set {
                _ShowTitle = value;
                SetItemRectangle();
                Invalidate();
            }
        }

        private bool _AutoColor = true;
        /// <summary>
        /// 获取或设置是否根据STNode自动设置控件高亮颜色
        /// </summary>
        [Description("获取或设置是否根据STNode自动设置控件高亮颜色"), DefaultValue(true)]
        public bool AutoColor {
            get { return _AutoColor; }
            set { _AutoColor = value; }
        }

        private bool _InfoFirstOnDraw;
        /// <summary>
        /// 获取或当节点被设置时候 是否优先绘制信息面板
        /// </summary>
        [Description("获取或设置当节点被设置时候 是否优先绘制信息面板"), DefaultValue(false)]
        public bool InfoFirstOnDraw {
            get { return _InfoFirstOnDraw; }
            set { _InfoFirstOnDraw = value; }
        }

        private bool _ReadOnlyModel;
        /// <summary>
        /// 获取或设置当前属性编辑器是否处于只读模式
        /// </summary>
        [Description("获取或设置当前属性编辑器是否处于只读模式"), DefaultValue(false)]
        public bool ReadOnlyModel {
            get { return _ReadOnlyModel; }
            set {
                if (value == _ReadOnlyModel) return;
                _ReadOnlyModel = value;
                Invalidate(m_rect_title);
            }
        }
        /// <summary>
        /// 获取当前滚动条高度
        /// </summary>
        [Description("获取当前滚动条高度")]
        public int ScrollOffset { get { return m_nOffsetY; } }

        #endregion

        #region protected fields ==========

        /// <summary>
        /// 作者链接地址区域
        /// </summary>
        protected Rectangle m_rect_link;
        /// <summary>
        /// 查看帮助按钮区域
        /// </summary>
        protected Rectangle m_rect_help;
        /// <summary>
        /// 编辑器标题区域
        /// </summary>
        protected Rectangle m_rect_title;
        /// <summary>
        /// 面板切换按钮区域
        /// </summary>
        protected Rectangle m_rect_switch;

        /// <summary>
        /// 控件在绘制过程中使用的垂直滚动偏移
        /// </summary>
        protected int m_nOffsetY;
        /// <summary>
        /// 保存的信息面板垂直滚动偏移
        /// </summary>
        protected int m_nInfoOffsetY;
        /// <summary>
        /// 保存的属性面板垂直滚动偏移
        /// </summary>
        protected int m_nPropertyOffsetY;

        /// <summary>
        /// 控件在绘制过程中使用的绘图区域总高度
        /// </summary>
        protected int m_nVHeight;
        /// <summary>
        /// 保存的信息面板需要的总高度
        /// </summary>
        protected int m_nInfoVHeight;
        /// <summary>
        /// 保存的属性面板需要的总高度
        /// </summary>
        protected int m_nPropertyVHeight;
        /// <summary>
        /// 信息面板中Key显示需要的水平宽度
        /// </summary>
        protected int m_nInfoLeft;

        #endregion

        private Type m_type;
        private string[] m_KeysString = new string[] { "作者", "邮箱", "链接", "查看帮助" };

        private int m_nTitleHeight = 20;
        private int m_item_height = 30;
        private Color m_clr_item_1 = Color.FromArgb(10, 0, 0, 0);
        private Color m_clr_item_2 = Color.FromArgb(10, 255, 255, 255);
        //所有属性列表保存在此List中
        private List<STNodePropertyDescriptor> m_lst_item = new List<STNodePropertyDescriptor>();

        private STNodePropertyDescriptor m_item_hover;        //当前被鼠标悬停的选项
        private STNodePropertyDescriptor m_item_hover_value;  //当前值区域被鼠标悬停的选项
        private STNodePropertyDescriptor m_item_down_value;   //当前值区域被鼠标点击的选项
        private STNodePropertyDescriptor m_item_selected;     //当前选中的选项
        private STNodeAttribute m_node_attribute;       //节点参数信息
        private bool m_b_hover_switch;                  //是否鼠标悬停在面板切换按钮上
        private bool m_b_current_draw_info;             //当前绘制的时候是信息面板

        private Point m_pt_move;                        //鼠标在控件上的实时坐标
        private Point m_pt_down;                        //上次鼠标在控件上点下的坐标
        private string m_str_err;                       //当被设置时 绘制错误信息
        private string m_str_desc;                      //当被设置时 绘制描述信息

        private Pen m_pen;
        private SolidBrush m_brush;
        private StringFormat m_sf;
        private DrawingTools m_dt;
        private SKCanvas m_canvas;

        /// <summary>
        /// 构造一个节点属性编辑器
        /// </summary>
        public STNodePropertyGrid() {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            m_pen = new Pen(Color.Black, 1);
            m_brush = new SolidBrush(Color.Black);
            m_sf = new StringFormat();
            m_sf.LineAlignment = StringAlignment.Center;
            m_sf.FormatFlags = StringFormatFlags.NoWrap;
            m_dt.Pen = m_pen;
            m_dt.SolidBrush = m_brush;

            ForeColor = Color.White;
            BackColor = Color.FromArgb(255, 35, 35, 35);

            MinimumSize = new Size(120, 50);
            Size = new Size(200, 150);
        }

        #region private method ==========

        private List<STNodePropertyDescriptor> GetProperties(STNode node) {
            List<STNodePropertyDescriptor> lst = new List<STNodePropertyDescriptor>();
            if (node == null) return lst;
            Type t = node.GetType();
            foreach (var p in t.GetProperties()) {
                var attrs = p.GetCustomAttributes(true);
                foreach (var a in attrs) {
                    if (!(a is STNodePropertyAttribute)) continue;
                    var attr = a as STNodePropertyAttribute;
                    object obj = Activator.CreateInstance(attr.DescriptorType);
                    if (!(obj is STNodePropertyDescriptor))
                        throw new ArgumentException("[STNodePropertyAttribute.DescriptorType]参数值必须为[STNodePropertyDescriptor]或者其子类的类型");
                    var desc = (STNodePropertyDescriptor)Activator.CreateInstance(attr.DescriptorType);
                    desc.Node = node;
                    desc.Name = attr.Name;
                    desc.Description = attr.Description;
                    desc.PropertyInfo = p;
                    desc.Control = this;
                    lst.Add(desc);
                }
            }
            return lst;
        }

        private STNodeAttribute GetNodeAttribute(STNode node) {
            if (node == null) return null;
            Type t = node.GetType();
            foreach (var v in t.GetCustomAttributes(true)) {
                if (!(v is STNodeAttribute)) continue;
                return (STNodeAttribute)v;
            }
            return null;
        }

        private void SetItemRectangle() {
            int nw_p = 0, nw_h = 0;
            using (var paint = new SKPaint { TextSize = Math.Max(10f, Font.Size), IsAntialias = true }) {
                foreach (var v in m_lst_item) {
                    float w = paint.MeasureText(v.Name ?? string.Empty);
                    if (w > nw_p) nw_p = (int)Math.Ceiling(w);
                }
                for (int i = 0; i < m_KeysString.Length - 1; i++) {
                    float w = paint.MeasureText(m_KeysString[i] ?? string.Empty);
                    if (w > nw_h) nw_h = (int)Math.Ceiling(w);
                }
            }
            nw_p += 5; nw_h += 5;
            nw_p = Math.Min(nw_p, Width >> 1);
            m_nInfoLeft = Math.Min(nw_h, Width >> 1);

            int nTitleHeight = _ShowTitle ? m_nTitleHeight : 0;
            for (int i = 0; i < m_lst_item.Count; i++) {
                STNodePropertyDescriptor item = m_lst_item[i];
                Rectangle rect = new Rectangle(0, i * m_item_height + nTitleHeight, Width, m_item_height);
                item.Rectangle = rect;
                rect.Width = nw_p;
                item.RectangleL = rect;
                rect.X = rect.Right;
                rect.Width = Width - rect.Left - 1;
                rect.Inflate(-4, -4);
                item.RectangleR = rect;
                item.OnSetItemLocation();
            }
            m_nPropertyVHeight = m_lst_item.Count * m_item_height;
            if (_ShowTitle) m_nPropertyVHeight += m_nTitleHeight;
        }

        #endregion

        #region override ==========

        /// <summary>
        /// 当控件重绘时候发生
        /// </summary>
        /// <param name="e">事件参数</param>
        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e) {
            base.OnPaintSurface(e);
            m_canvas = e.Surface.Canvas;
            m_canvas.Clear(SkiaDrawingHelper.ToSKColor(BackColor));
            if (_InfoFirstOnDraw) {
                OnDrawInfo(m_dt);
            } else {
                for (int i = 0; i < m_lst_item.Count; i++) OnDrawPropertyItem(m_dt, m_lst_item[i], i);
            }
            if (_ShowTitle) OnDrawTitle(m_dt);
            if (!string.IsNullOrEmpty(m_str_err)) OnDrawErrorInfo(m_dt);
            if (!string.IsNullOrEmpty(m_str_desc)) OnDrawDescription(m_dt);
            m_canvas = null;
        }
        /// <summary>
        /// 当鼠标在控件上移动时候发生
        /// </summary>
        /// <param name="e">事件参数</param>
        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            m_pt_move = e.Location;
            bool bHover = _ShowTitle && m_rect_switch.Contains(e.Location);
            if (bHover != m_b_hover_switch) {
                m_b_hover_switch = bHover;
                Invalidate(m_rect_switch);
            }
            Point pt = new Point(e.X, e.Y - (int)m_nOffsetY);
            MouseEventArgs mea = new MouseEventArgs(e.Button, e.Clicks, pt.X, pt.Y, e.Delta);
            if (m_b_current_draw_info)
                OnProcessHelpMouseMove(mea);
            else
                OnProcessPropertyMouseMove(mea);
        }
        /// <summary>
        /// 当鼠标在控件上点下时候发生
        /// </summary>
        /// <param name="e">事件参数</param>
        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            m_pt_down = e.Location;
            Focus();
            bool bRedraw = false;
            if (m_str_err != null) {
                bRedraw = true;
                m_str_err = null;
            }
            if (_ShowTitle) {
                if (m_rect_switch.Contains(e.Location)) {
                    if (m_node_attribute == null || m_lst_item.Count == 0) return;
                    m_b_current_draw_info = !m_b_current_draw_info;
                    Invalidate();
                    return;
                } else if (m_rect_title.Contains(e.Location)) {
                    return;
                }
            }
            if (_ShowTitle && m_rect_switch.Contains(e.Location)) {
                if (m_node_attribute == null || m_lst_item.Count == 0) return;
                m_b_current_draw_info = !m_b_current_draw_info;
                Invalidate();
                return;
            }
            Point pt = new Point(e.X, e.Y - (int)m_nOffsetY);
            MouseEventArgs mea = new MouseEventArgs(e.Button, e.Clicks, pt.X, pt.Y, e.Delta);

            if (m_b_current_draw_info)
                OnProcessInfoMouseDown(mea);
            else
                OnProcessPropertyMouseDown(mea);
            if (bRedraw) Invalidate();
        }
        /// <summary>
        /// 当鼠标在控件上抬起时候发生
        /// </summary>
        /// <param name="e">事件参数</param>
        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);
            m_str_desc = null;
            if (m_item_down_value != null && !_ReadOnlyModel) {
                Point pt = new Point(e.X, e.Y - (int)m_nOffsetY);
                MouseEventArgs mea = new MouseEventArgs(e.Button, e.Clicks, pt.X, pt.Y, e.Delta);
                m_item_down_value.OnMouseUp(mea);
                if (m_pt_down == e.Location && !_ReadOnlyModel) {
                    m_item_down_value.OnMouseClick(mea);
                }
            }
            m_item_down_value = null;
            Invalidate();
        }
        /// <summary>
        /// 当鼠标离开控件时候发生
        /// </summary>
        /// <param name="e">事件参数</param>
        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);
            m_b_hover_switch = false;
            if (m_item_hover_value != null && !_ReadOnlyModel) m_item_hover_value.OnMouseLeave(e);
            m_item_hover = null;
            Invalidate();
        }
        /// <summary>
        /// 当鼠标在控件上滚动滚轮时候发生
        /// </summary>
        /// <param name="e">事件参数</param>
        protected override void OnMouseWheel(MouseEventArgs e) {
            base.OnMouseWheel(e);
            if (e.Delta > 0) {
                if (m_nOffsetY == 0) return;
                m_nOffsetY += m_item_height;
                if (m_nOffsetY > 0) m_nOffsetY = 0;
            } else {
                if (Height - m_nOffsetY >= m_nVHeight) return;
                m_nOffsetY -= m_item_height;
            }
            if (m_b_current_draw_info)
                m_nInfoOffsetY = m_nOffsetY;
            else
                m_nPropertyOffsetY = m_nOffsetY;
            Invalidate();
        }
        /// <summary>
        /// 当设置控件矩形区域时候发生
        /// </summary>
        /// <param name="x">x坐标</param>
        /// <param name="y">y坐标</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="specified">指定需要设置的标识</param>
        //protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
        //    if (width < 120) width = 120;
        //    if (height < 50) height = 50;
        //    base.SetBoundsCore(x, y, width, height, specified);
        //}
        /// <summary>
        /// 当控件尺寸发生改变时候发生
        /// </summary>
        /// <param name="e">事件参数</param>
        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            m_rect_title.Width = Width;
            m_rect_title.Height = m_nTitleHeight;
            if (_ShowTitle)
                m_rect_switch = new Rectangle(Width - m_nTitleHeight + 2, 2, m_nTitleHeight - 4, m_nTitleHeight - 4);
            if (_STNode != null) SetItemRectangle();
        }

        #endregion

        #region virtual method ==========

        /// <summary>
        /// 当绘制属性选项时候发生
        /// </summary>
        /// <param name="dt">绘制工具</param>
        /// <param name="item">目标属性选项描述器</param>
        /// <param name="nIndex">选项所在索引</param>
        protected virtual void OnDrawPropertyItem(DrawingTools dt, STNodePropertyDescriptor item, int nIndex) {
            if (m_canvas == null) return;
            using (var fill = new SKPaint { Style = SKPaintStyle.Fill, IsAntialias = true })
            using (var text = new SKPaint { Color = SkiaDrawingHelper.ToSKColor(ForeColor), TextSize = Math.Max(10f, Font.Size), IsAntialias = true }) {
                fill.Color = SkiaDrawingHelper.ToSKColor((nIndex % 2 == 0) ? m_clr_item_1 : m_clr_item_2);
                m_canvas.DrawRect(item.Rectangle.Left, item.Rectangle.Top, item.Rectangle.Width, item.Rectangle.Height, fill);
                if (item == m_item_hover || item == m_item_selected) {
                    fill.Color = SkiaDrawingHelper.ToSKColor(_ItemHoverColor);
                    m_canvas.DrawRect(item.Rectangle.Left, item.Rectangle.Top, item.Rectangle.Width, item.Rectangle.Height, fill);
                }
                if (m_item_selected == item) {
                    m_canvas.DrawRect(item.Rectangle.X, item.Rectangle.Y, 5, item.Rectangle.Height, fill);
                    fill.Color = SkiaDrawingHelper.ToSKColor((_AutoColor && _STNode != null) ? _STNode.TitleColor : _ItemSelectedColor);
                    m_canvas.DrawRect(item.Rectangle.X, item.Rectangle.Y + 4, 5, item.Rectangle.Height - 8, fill);
                }
                var fm = text.FontMetrics;
                float y = item.RectangleL.Top + (item.RectangleL.Height - (fm.Descent - fm.Ascent)) / 2 - fm.Ascent;
                float x = item.RectangleL.Right - text.MeasureText(item.Name ?? string.Empty) - 2;
                m_canvas.DrawText(item.Name ?? string.Empty, x, y, text);
            }
            item.OnDrawValueRectangle(m_dt);
            if (_ReadOnlyModel && m_canvas != null) {
                using (var overlay = new SKPaint { Color = new SKColor(125, 125, 125, 125), Style = SKPaintStyle.Fill, IsAntialias = true })
                    m_canvas.DrawRect(item.RectangleR.Left, item.RectangleR.Top, item.RectangleR.Width, item.RectangleR.Height, overlay);
            }
        }
        /// <summary>
        /// 绘制属性窗口标题
        /// </summary>
        /// <param name="dt">绘制工具</param>
        protected virtual void OnDrawTitle(DrawingTools dt) {
            if (m_canvas == null) return;
            using (var fill = new SKPaint { Style = SKPaintStyle.Fill, IsAntialias = true })
            using (var text = new SKPaint { TextSize = Math.Max(10f, Font.Size), IsAntialias = true, Style = SKPaintStyle.Fill }) {
                fill.Color = SkiaDrawingHelper.ToSKColor(_AutoColor ? (_STNode == null ? _TitleColor : _STNode.TitleColor) : _TitleColor);
                m_canvas.DrawRect(m_rect_title.Left, m_rect_title.Top, m_rect_title.Width, m_rect_title.Height, fill);
                text.Color = SkiaDrawingHelper.ToSKColor(_STNode == null ? ForeColor : _STNode.ForeColor);
                var t = _STNode == null ? Text : _STNode.Title;
                var fm = text.FontMetrics;
                float y = m_rect_title.Top + (m_rect_title.Height - (fm.Descent - fm.Ascent)) / 2 - fm.Ascent;
                float x = m_rect_title.Left + (m_rect_title.Width - text.MeasureText(t ?? string.Empty)) / 2;
                m_canvas.DrawText(t ?? string.Empty, x, y, text);
            }
        }
        /// <summary>
        /// 当需要绘制属性描述信息时发生
        /// </summary>
        /// <param name="dt">绘制工具</param>
        protected virtual void OnDrawDescription(DrawingTools dt) {
            if (string.IsNullOrEmpty(m_str_desc) || m_canvas == null) return;
            using (var text = new SKPaint { Color = SkiaDrawingHelper.ToSKColor(ForeColor), TextSize = Math.Max(10f, Font.Size), IsAntialias = true })
            using (var fill = new SKPaint { Color = SkiaDrawingHelper.ToSKColor(_DescriptionColor), Style = SKPaintStyle.Fill, IsAntialias = true })
            using (var stroke = new SKPaint { Color = SkiaDrawingHelper.ToSKColor(_DescriptionColor), Style = SKPaintStyle.Stroke, StrokeWidth = 1, IsAntialias = true }) {
                var bounds = new SKRect();
                text.MeasureText(m_str_desc, ref bounds);
                Rectangle rect_desc = new Rectangle(0, Height - (int)Math.Ceiling(bounds.Height) - 8, Width, (int)Math.Ceiling(bounds.Height) + 8);
                m_canvas.DrawRect(rect_desc.Left, rect_desc.Top, rect_desc.Width, rect_desc.Height, fill);
                m_canvas.DrawRect(rect_desc.Left, rect_desc.Top, rect_desc.Width - 1, rect_desc.Height - 1, stroke);
                var fm = text.FontMetrics;
                float y = rect_desc.Top + (rect_desc.Height - (fm.Descent - fm.Ascent)) / 2 - fm.Ascent;
                m_canvas.DrawText(m_str_desc, rect_desc.Left + 4, y, text);
            }
        }
        /// <summary>
        /// 当需要绘制错误信息时发生
        /// </summary>
        /// <param name="dt">绘制工具</param>
        protected virtual void OnDrawErrorInfo(DrawingTools dt) {
            if (string.IsNullOrEmpty(m_str_err) || m_canvas == null) return;
            using (var text = new SKPaint { Color = SkiaDrawingHelper.ToSKColor(ForeColor), TextSize = Math.Max(10f, Font.Size), IsAntialias = true })
            using (var fill = new SKPaint { Color = SkiaDrawingHelper.ToSKColor(_ErrorColor), Style = SKPaintStyle.Fill, IsAntialias = true })
            using (var stroke = new SKPaint { Color = SkiaDrawingHelper.ToSKColor(_ErrorColor), Style = SKPaintStyle.Stroke, StrokeWidth = 1, IsAntialias = true }) {
                var bounds = new SKRect();
                text.MeasureText(m_str_err, ref bounds);
                Rectangle rect_desc = new Rectangle(0, 0, Width, (int)Math.Ceiling(bounds.Height) + 8);
                m_canvas.DrawRect(rect_desc.Left, rect_desc.Top, rect_desc.Width, rect_desc.Height, fill);
                m_canvas.DrawRect(rect_desc.Left, rect_desc.Top, rect_desc.Width - 1, rect_desc.Height - 1, stroke);
                var fm = text.FontMetrics;
                float y = rect_desc.Top + (rect_desc.Height - (fm.Descent - fm.Ascent)) / 2 - fm.Ascent;
                m_canvas.DrawText(m_str_err, rect_desc.Left + 4, y, text);
            }
        }
        /// <summary>
        /// 当绘制节点信息时候发生
        /// </summary>
        /// <param name="dt">绘制工具</param>
        protected virtual void OnDrawInfo(DrawingTools dt) {
            if (m_node_attribute == null || m_canvas == null) return;
            var attr = m_node_attribute;
            using (var text = new SKPaint { Color = SkiaDrawingHelper.ToSKColor(ForeColor), TextSize = Math.Max(10f, Font.Size), IsAntialias = true })
            using (var textDim = new SKPaint { Color = SkiaDrawingHelper.ToSKColor(Color.FromArgb(ForeColor.A / 2, ForeColor)), TextSize = Math.Max(10f, Font.Size), IsAntialias = true })
            using (var fill = new SKPaint { Style = SKPaintStyle.Fill, IsAntialias = true }) {
                int top = _ShowTitle ? m_nTitleHeight : 0;
                int y = top;
                void row(SKColor bg,string key,string val,SKPaint vp){
                    fill.Color=bg; m_canvas.DrawRect(0,y,Width,m_item_height,fill);
                    var fm=text.FontMetrics; float ty=y+(m_item_height-(fm.Descent-fm.Ascent))/2-fm.Ascent;
                    m_canvas.DrawText(key,2,ty,text); m_canvas.DrawText(val??string.Empty,m_nInfoLeft,ty,vp); y+=m_item_height;
                }
                row(SkiaDrawingHelper.ToSKColor(m_clr_item_2),m_KeysString[0],attr.Author,textDim);
                row(SkiaDrawingHelper.ToSKColor(m_clr_item_1),m_KeysString[1],attr.Mail,textDim);
                row(SkiaDrawingHelper.ToSKColor(m_clr_item_2),m_KeysString[2],attr.Link,new SKPaint{Color=SKColors.CornflowerBlue,TextSize=text.TextSize,IsAntialias=true});
                m_nInfoVHeight = y + m_item_height;
            }
        }
        /// <summary>
        /// 当在属性面板鼠标点下时候发生
        /// </summary>
        /// <param name="e">鼠标事件参数</param>
        protected virtual void OnProcessPropertyMouseDown(MouseEventArgs e) {
            bool bRedraw = false;
            if (m_item_selected != m_item_hover) {
                m_item_selected = m_item_hover;
                bRedraw = true;
            }
            m_item_down_value = null;
            if (m_item_selected == null) {
                if (bRedraw) Invalidate();
                return;
            }
            if (m_item_selected.RectangleR.Contains(e.Location)) {
                m_item_down_value = m_item_selected;
                if (!_ReadOnlyModel)
                    m_item_selected.OnMouseDown(e);
                else {
                    return;
                }
            } else if (m_item_selected.RectangleL.Contains(e.Location)) {
                m_str_desc = m_item_selected.Description;
                bRedraw = true;
            }
            if (bRedraw) Invalidate();
        }
        /// <summary>
        /// 当在信息面板鼠标点下时候发生
        /// </summary>
        /// <param name="e">鼠标事件参数</param>
        protected virtual void OnProcessInfoMouseDown(MouseEventArgs e) {
            try {
                if (m_rect_link.Contains(e.Location)) {
                    System.Diagnostics.Process.Start(m_node_attribute.Link);
                } else if (m_rect_help.Contains(e.Location)) {
                    STNodeAttribute.ShowHelp(m_type);
                }
            } catch (Exception ex) {
                SetErrorMessage(ex.Message);
            }
        }
        /// <summary>
        /// 当在属性面板鼠标移动时候发生
        /// </summary>
        /// <param name="e">鼠标事件参数</param>
        protected virtual void OnProcessPropertyMouseMove(MouseEventArgs e) {
            if (m_item_down_value != null) {
                m_item_down_value.OnMouseMove(e);
                return;
            }
            STNodePropertyDescriptor item = null;
            foreach (var v in m_lst_item) {
                if (v.Rectangle.Contains(e.Location)) {
                    item = v;
                    break;
                }
            }
            if (item != null) {
                if (item.RectangleR.Contains(e.Location)) {
                    if (m_item_hover_value != item) {
                        if (m_item_hover_value != null) m_item_hover_value.OnMouseLeave(e);
                        m_item_hover_value = item;
                        m_item_hover_value.OnMouseEnter(e);
                    }
                    m_item_hover_value.OnMouseMove(e);
                } else {
                    if (m_item_hover_value != null) m_item_hover_value.OnMouseLeave(e);
                }
            }
            if (m_item_hover != item) {
                m_item_hover = item;
                Invalidate();
            }
        }
        /// <summary>
        /// 当在信息面板鼠标移动时候发生
        /// </summary>
        /// <param name="e">鼠标事件参数</param>
        protected virtual void OnProcessHelpMouseMove(MouseEventArgs e) {
            if (m_rect_link.Contains(e.Location) || m_rect_help.Contains(e.Location)) {
                Cursor = Cursors.Hand;
            } else Cursor = Cursors.Arrow;
        }

        #endregion

        #region public ==========

        /// <summary>
        /// 设置需要显示的STNode节点
        /// </summary>
        /// <param name="node">目标节点</param>
        public void SetNode(STNode node) {
            if (node == _STNode) return;
            m_nInfoOffsetY = m_nPropertyOffsetY = 0;
            m_nInfoVHeight = m_nPropertyVHeight = 0;
            m_rect_link = m_rect_help = Rectangle.Empty;
            m_str_desc = m_str_err = null;
            _STNode = node;
            if (node != null) {
                m_type = node.GetType();
                m_lst_item = GetProperties(node);
                m_node_attribute = GetNodeAttribute(node);
                SetItemRectangle();
                m_b_current_draw_info = m_lst_item.Count == 0 || _InfoFirstOnDraw;
                if (_AutoColor) _ItemSelectedColor = _STNode.TitleColor;
            } else {
                m_type = null;
                m_lst_item.Clear();
                m_node_attribute = null;
            }
            Invalidate();
        }
        /// <summary>
        /// 设置信息页面Key的显示文本
        /// </summary>
        /// <param name="strAuthor">作者</param>
        /// <param name="strMail">邮箱</param>
        /// <param name="strLink">连接</param>
        /// <param name="strHelp">查看帮助</param>
        public void SetInfoKey(string strAuthor, string strMail, string strLink, string strHelp) {
            m_KeysString = new string[] { strAuthor, strMail, strLink, strHelp };
        }
        /// <summary>
        /// 设置要显示的错误信息
        /// </summary>
        /// <param name="strText">错误信息</param>
        public void SetErrorMessage(string strText) {
            m_str_err = strText;
            Invalidate();
        }

        #endregion
    }
}
