using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.ComponentModel;
using System.Reflection;
using System.IO.Compression;
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
 * create: 2020-12-08
 * modify: 2021-04-12
 * Author: Crystal_lz
 * blog: http://st233.com
 * Gitee: https://gitee.com/DebugST
 * Github: https://github.com/DebugST
 */
namespace ST.Library.UI.NodeEditor
{
    public class STNodeEditor : SKControl
    {
        private const UInt32 WM_MOUSEHWHEEL = 0x020E;
        protected static readonly Type m_type_node = typeof(STNode);

        #region protected enum,struct --------------------------------------------------------------------------------------

        protected enum CanvasAction     //当前鼠标移动操作表示进行下列哪一个行为
        {
            None,                       //无
            MoveNode,                   //正在移动 Node
            ConnectOption,              //正在连接 Option
            SelectRectangle,            //正在选择矩形区域
            DrawMarkDetails             //正在绘制标记信息详情
        }

        protected struct MagnetInfo
        {
            public bool XMatched;       //X轴是否有磁铁匹配上
            public bool YMatched;
            public int X;               //与X轴那个数字匹配上
            public int Y;
            public int OffsetX;         //当前节点X位置与匹配上的X的相对偏移
            public int OffsetY;
        }

        #endregion

        #region Properties ------------------------------------------------------------------------------------------------------

        private float _CanvasOffsetX;
        /// <summary>
        /// 获取画布原点相对于控件 X 方向上的偏移位置
        /// </summary>
        [Browsable(false)]
        public float CanvasOffsetX {
            get { return _CanvasOffsetX; }
        }

        private float _CanvasOffsetY;
        /// <summary>
        /// 获取画布原点相对于控件 Y 方向上的偏移位置
        /// </summary>
        [Browsable(false)]
        public float CanvasOffsetY {
            get { return _CanvasOffsetY; }
        }

        private PointF _CanvasOffset;
        /// <summary>
        /// 获取画布原点相对于控件偏移位置
        /// </summary>
        [Browsable(false)]
        public PointF CanvasOffset {
            get {
                _CanvasOffset.X = _CanvasOffsetX;
                _CanvasOffset.Y = _CanvasOffsetY;
                return _CanvasOffset;
            }
        }

        private Rectangle _CanvasValidBounds;
        /// <summary>
        /// 获取画布中的有被用到的有效区域
        /// </summary>
        [Browsable(false)]
        public Rectangle CanvasValidBounds {
            get { return _CanvasValidBounds; }
        }

        private float _CanvasScale = 1;
        /// <summary>
        /// 获取画布的缩放比例
        /// </summary>
        [Browsable(false)]
        public float CanvasScale {
            get { return _CanvasScale; }
        }

        private float _Curvature = 0.3F;
        /// <summary>
        /// 获取或设置 Option 之间连线的曲度
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float Curvature {
            get { return _Curvature; }
            set {
                if (value < 0) value = 0;
                if (value > 1) value = 1;
                _Curvature = value;
                if (m_dic_gp_info.Count != 0) BuildLinePath();
            }
        }

        private bool _ShowMagnet = true;
        /// <summary>
        /// 获取或设置移动画布中 Node 时候 是否启用磁铁效果
        /// </summary>
        [Description("获取或设置移动画布中 Node 时候 是否启用磁铁效果"), DefaultValue(true)]
        public bool ShowMagnet {
            get { return _ShowMagnet; }
            set { _ShowMagnet = value; }
        }

        private bool _ShowBorder = true;
        /// <summary>
        /// 获取或设置 移动画布中是否显示 Node 边框
        /// </summary>
        [Description("获取或设置 移动画布中是否显示 Node 边框"), DefaultValue(true)]
        public bool ShowBorder {
            get { return _ShowBorder; }
            set {
                _ShowBorder = value;
                Invalidate();
            }
        }

        private bool _ShowGrid = true;
        /// <summary>
        /// 获取或设置画布中是否绘制背景网格线条
        /// </summary>
        [Description("获取或设置画布中是否绘制背景网格线条"), DefaultValue(true)]
        public bool ShowGrid {
            get { return _ShowGrid; }
            set {
                _ShowGrid = value;
                Invalidate();
            }
        }

        private bool _ShowLocation = true;
        /// <summary>
        /// 获取或设置是否在画布边缘显示超出视角的 Node 位置信息
        /// </summary>
        [Description("获取或设置是否在画布边缘显示超出视角的 Node 位置信息"), DefaultValue(true)]
        public bool ShowLocation {
            get { return _ShowLocation; }
            set {
                _ShowLocation = value;
                Invalidate();
            }
        }

        private STNodeCollection _Nodes;
        /// <summary>
        /// 获取画布中 Node 集合
        /// </summary>
        [Browsable(false)]
        public STNodeCollection Nodes {
            get {
                return _Nodes;
            }
        }

        private STNode _ActiveNode;
        /// <summary>
        /// 获取当前画布中被选中的活动 Node
        /// </summary>
        [Browsable(false)]
        public STNode ActiveNode {
            get { return _ActiveNode; }
            //set {
            //    if (value == _ActiveSelectedNode) return;
            //    if (_ActiveSelectedNode != null) _ActiveSelectedNode.OnLostFocus(EventArgs.Empty);
            //    _ActiveSelectedNode = value;
            //    _ActiveSelectedNode.IsActive = true;
            //    this.Invalidate();
            //    this.OnSelectedChanged(EventArgs.Empty);
            //}
        }

        private STNode _HoverNode;
        /// <summary>
        /// 获取当前画布中鼠标悬停的 Node
        /// </summary>
        [Browsable(false)]
        public STNode HoverNode {
            get { return _HoverNode; }
        }
        //========================================color================================
        private Color _GridColor = Color.Black;
        /// <summary>
        /// 获取或设置绘制画布背景时 网格线条颜色
        /// </summary>
        [Description("获取或设置绘制画布背景时 网格线条颜色"), DefaultValue(typeof(Color), "Black")]
        public Color GridColor {
            get { return _GridColor; }
            set {
                _GridColor = value;
                Invalidate();
            }
        }

        private Color _BorderColor = Color.Black;
        /// <summary>
        /// 获取或设置画布中 Node 边框颜色
        /// </summary>
        [Description("获取或设置画布中 Node 边框颜色"), DefaultValue(typeof(Color), "Black")]
        public Color BorderColor {
            get { return _BorderColor; }
            set {
                _BorderColor = value;
                if (m_img_border != null) m_img_border.Dispose();
                m_img_border = CreateBorderImage(value);
                Invalidate();
            }
        }

        private Color _BorderHoverColor = Color.Gray;
        /// <summary>
        /// 获取或设置画布中悬停 Node 边框颜色
        /// </summary>
        [Description("获取或设置画布中悬停 Node 边框颜色"), DefaultValue(typeof(Color), "Gray")]
        public Color BorderHoverColor {
            get { return _BorderHoverColor; }
            set {
                _BorderHoverColor = value;
                if (m_img_border_hover != null) m_img_border_hover.Dispose();
                m_img_border_hover = CreateBorderImage(value);
                Invalidate();
            }
        }

        private Color _BorderSelectedColor = Color.Orange;
        /// <summary>
        /// 获取或设置画布中选中 Node 边框颜色
        /// </summary>
        [Description("获取或设置画布中选中 Node 边框颜色"), DefaultValue(typeof(Color), "Orange")]
        public Color BorderSelectedColor {
            get { return _BorderSelectedColor; }
            set {
                _BorderSelectedColor = value;
                if (m_img_border_selected != null) m_img_border_selected.Dispose();
                m_img_border_selected = CreateBorderImage(value);
                Invalidate();
            }
        }

        private Color _BorderActiveColor = Color.OrangeRed;
        /// <summary>
        /// 获取或设置画布中活动 Node 边框颜色
        /// </summary>
        [Description("获取或设置画布中活动 Node 边框颜色"), DefaultValue(typeof(Color), "OrangeRed")]
        public Color BorderActiveColor {
            get { return _BorderActiveColor; }
            set {
                _BorderActiveColor = value;
                if (m_img_border_active != null) m_img_border_active.Dispose();
                m_img_border_active = CreateBorderImage(value);
                Invalidate();
            }
        }

        private Color _MarkForeColor = Color.White;
        /// <summary>
        /// 获取或设置画布绘制 Node 标记详情采用的前景色
        /// </summary>
        [Description("获取或设置画布绘制 Node 标记详情采用的前景色"), DefaultValue(typeof(Color), "White")]
        public Color MarkForeColor {
            get { return _MarkBackColor; }
            set {
                _MarkBackColor = value;
                Invalidate();
            }
        }

        private Color _MarkBackColor = Color.FromArgb(180, Color.Black);
        /// <summary>
        /// 获取或设置画布绘制 Node 标记详情采用的背景色
        /// </summary>
        [Description("获取或设置画布绘制 Node 标记详情采用的背景色")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color MarkBackColor {
            get { return _MarkBackColor; }
            set {
                _MarkBackColor = value;
                Invalidate();
            }
        }

        private Color _MagnetColor = Color.Lime;
        /// <summary>
        /// 获取或设置画布中移动 Node 时候 磁铁标记颜色
        /// </summary>
        [Description("获取或设置画布中移动 Node 时候 磁铁标记颜色"), DefaultValue(typeof(Color), "Lime")]
        public Color MagnetColor {
            get { return _MagnetColor; }
            set { _MagnetColor = value; }
        }

        private Color _SelectedRectangleColor = Color.DodgerBlue;
        /// <summary>
        /// 获取或设置画布中选择矩形区域的颜色
        /// </summary>
        [Description("获取或设置画布中选择矩形区域的颜色"), DefaultValue(typeof(Color), "DodgerBlue")]
        public Color SelectedRectangleColor {
            get { return _SelectedRectangleColor; }
            set { _SelectedRectangleColor = value; }
        }

        private Color _HighLineColor = Color.Cyan;
        /// <summary>
        /// 获取或设置画布中高亮连线的颜色
        /// </summary>
        [Description("获取或设置画布中高亮连线的颜色"), DefaultValue(typeof(Color), "Cyan")]
        public Color HighLineColor {
            get { return _HighLineColor; }
            set { _HighLineColor = value; }
        }

        private Color _LocationForeColor = Color.Red;
        /// <summary>
        /// 获取或设置画布中边缘位置提示区域前景色
        /// </summary>
        [Description("获取或设置画布中边缘位置提示区域前景色"), DefaultValue(typeof(Color), "Red")]
        public Color LocationForeColor {
            get { return _LocationForeColor; }
            set {
                _LocationForeColor = value;
                Invalidate();
            }
        }

        private Color _LocationBackColor = Color.FromArgb(120, Color.Black);
        /// <summary>
        /// 获取或设置画布中边缘位置提示区域背景色
        /// </summary>
        [Description("获取或设置画布中边缘位置提示区域背景色")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color LocationBackColor {
            get { return _LocationBackColor; }
            set {
                _LocationBackColor = value;
                Invalidate();
            }
        }

        private Color _UnknownTypeColor = Color.Gray;
        /// <summary>
        /// 获取或设置画布中当 Node 中 Option 数据类型无法确定时应当使用的颜色
        /// </summary>
        [Description("获取或设置画布中当 Node 中 Option 数据类型无法确定时应当使用的颜色"), DefaultValue(typeof(Color), "Gray")]
        public Color UnknownTypeColor {
            get { return _UnknownTypeColor; }
            set {
                _UnknownTypeColor = value;
                Invalidate();
            }
        }

        private Dictionary<Type, Color> _TypeColor = new Dictionary<Type, Color>();
        /// <summary>
        /// 获取或设置画布中 Node 中 Option 数据类型预设颜色
        /// </summary>
        [Browsable(false)]
        public Dictionary<Type, Color> TypeColor {
            get { return _TypeColor; }
        }

        #endregion

        #region protected properties ----------------------------------------------------------------------------------------
        /// <summary>
        /// 当前鼠标在控件中的实时位置
        /// </summary>
        protected Point m_pt_in_control;
        /// <summary>
        /// 当前鼠标在画布中的实时位置
        /// </summary>
        protected PointF m_pt_in_canvas;
        /// <summary>
        /// 鼠标点击时在控件上的位置
        /// </summary>
        protected Point m_pt_down_in_control;
        /// <summary>
        /// 鼠标点击时在画布中的位置
        /// </summary>
        protected PointF m_pt_down_in_canvas;
        /// <summary>
        /// 用于鼠标点击移动画布时候 鼠标点下时候的画布坐标位置
        /// </summary>
        protected PointF m_pt_canvas_old;
        /// <summary>
        /// 用于保存连线过程中保存点下 Option 的起点坐标
        /// </summary>
        protected Point m_pt_dot_down;
        /// <summary>
        /// 用于保存连线过程中鼠标点下的起点Option 当MouseUP时候 确定是否连接此节点
        /// </summary>
        protected STNodeOption m_option_down;
        /// <summary>
        /// 当前鼠标点下的 STNode
        /// </summary>
        protected STNode m_node_down;
        /// <summary>
        /// 当前鼠标是否位于控件中
        /// </summary>
        protected bool m_mouse_in_control;

        #endregion

        public STNodeEditor() {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            _Nodes = new STNodeCollection(this);
            BackColor = Color.FromArgb(255, 34, 34, 34);
            MinimumSize = new Size(100, 100);
            Size = new Size(200, 200);
            AllowDrop = true;

            m_real_canvas_x = _CanvasOffsetX = 10;
            m_real_canvas_y = _CanvasOffsetY = 10;
        }

        #region private fields --------------------------------------------------------------------------------------

        private DrawingTools m_drawing_tools;
        private SKCanvas m_canvas;
        private NodeFindInfo m_find = new NodeFindInfo();
        private MagnetInfo m_mi = new MagnetInfo();

        private RectangleF m_rect_select = new RectangleF();
        //节点边框预设图案
        private Image m_img_border;
        private Image m_img_border_hover;
        private Image m_img_border_selected;
        private Image m_img_border_active;
        //用于鼠标滚动或者触摸板移动画布时候的动画效果 该值为需要移动到的真实坐标地址 查看->MoveCanvasThread()
        private float m_real_canvas_x;
        private float m_real_canvas_y;
        //用于移动节点时候 保存鼠标点下时候选中的节点初始坐标
        private Dictionary<STNode, Point> m_dic_pt_selected = new Dictionary<STNode, Point>();
        //用于磁铁效果 移动节点时候 非选择节点的统计出来的需要参与磁铁效果的坐标 查看->BuildMagnetLocation()
        private List<int> m_lst_magnet_x = new List<int>();
        private List<int> m_lst_magnet_y = new List<int>();
        //用于磁铁效果 移动节点时候 活动选择节点统计出来需要参与磁铁效果的坐标 查看->CheckMagnet()
        private List<int> m_lst_magnet_mx = new List<int>();
        private List<int> m_lst_magnet_my = new List<int>();
        //用于鼠标滚动中计算时间触发间隔 根据间隔不同 画布产生的位移不同 查看->OnMouseWheel(),OnMouseHWheel()
        private DateTime m_dt_vw = DateTime.Now;
        private DateTime m_dt_hw = DateTime.Now;
        //移动鼠标过程中的当前行为
        private CanvasAction m_ca;
        //保存已选中的节点
        private HashSet<STNode> m_hs_node_selected = new HashSet<STNode>();

        private bool m_is_process_mouse_event = true;               //是否向下(Node or NodeControls)传递鼠标相关事件 如断开连接相关操作不应向下传递
        private bool m_is_buildpath;                                //用于重绘过程中 判断该次是否要重新建立缓存连线的路径
        private Pen m_p_line = new Pen(Color.Cyan, 2f);             //用于绘制已经连接的线条
        private Pen m_p_line_hover = new Pen(Color.Cyan, 4f);       //用于绘制鼠标悬停时候的线条
        private GraphicsPath m_gp_hover;                            //当前鼠标悬停的连线路径
        private StringFormat m_sf = new StringFormat();             //文本格式 用于Mark绘制时候 设置文本格式
        //保存每个连接线条与之对应的节点关系
        private Dictionary<GraphicsPath, ConnectionInfo> m_dic_gp_info = new Dictionary<GraphicsPath, ConnectionInfo>();
        //保存超出视觉区域的 Node 的位置
        private List<Point> m_lst_node_out = new List<Point>();
        //当前编辑器已加载的 Node 类型 用于从文件或者数据中加载节点使用
        private Dictionary<string, Type> m_dic_type = new Dictionary<string, Type>();

        private int m_time_alert;
        private int m_alpha_alert;
        private string m_str_alert;
        private Color m_forecolor_alert;
        private Color m_backcolor_alert;
        private DateTime m_dt_alert;
        private Rectangle m_rect_alert;
        private AlertLocation m_al;

        #endregion

        #region event ----------------------------------------------------------------------------------------------------
        /// <summary>
        /// 活动的节点发生变化时候发生
        /// </summary>
        [Description("活动的节点发生变化时候发生")]
        public event EventHandler ActiveChanged;
        /// <summary>
        /// 选择的节点发生变化时候发生
        /// </summary>
        [Description("选择的节点发生变化时候发生")]
        public event EventHandler SelectedChanged;
        /// <summary>
        /// 悬停的节点发生变化时候发生
        /// </summary>
        [Description("悬停的节点发生变化时候发生")]
        public event EventHandler HoverChanged;
        /// <summary>
        /// 当节点被添加时候发生
        /// </summary>
        [Description("当节点被添加时候发生")]
        public event STNodeEditorEventHandler NodeAdded;
        /// <summary>
        /// 当节点被移除时候发生
        /// </summary>
        [Description("当节点被移除时候发生")]
        public event STNodeEditorEventHandler NodeRemoved;
        /// <summary>
        /// 移动画布原点时候发生
        /// </summary>
        [Description("移动画布原点时候发生")]
        public event EventHandler CanvasMoved;
        /// <summary>
        /// 缩放画布时候发生
        /// </summary>
        [Description("缩放画布时候发生")]
        public event EventHandler CanvasScaled;
        /// <summary>
        /// 连接节点选项时候发生
        /// </summary>
        [Description("连接节点选项时候发生")]
        public event STNodeEditorOptionEventHandler OptionConnected;
        /// <summary>
        /// 正在连接节点选项时候发生
        /// </summary>
        [Description("正在连接节点选项时候发生")]
        public event STNodeEditorOptionEventHandler OptionConnecting;
        /// <summary>
        /// 断开节点选项时候发生
        /// </summary>
        [Description("断开节点选项时候发生")]
        public event STNodeEditorOptionEventHandler OptionDisConnected;
        /// <summary>
        /// 正在断开节点选项时候发生
        /// </summary>
        [Description("正在断开节点选项时候发生")]
        public event STNodeEditorOptionEventHandler OptionDisConnecting;

        protected virtual internal void OnSelectedChanged(EventArgs e) {
            if (SelectedChanged != null) SelectedChanged(this, e);
        }
        protected virtual void OnActiveChanged(EventArgs e) {
            if (ActiveChanged != null) ActiveChanged(this, e);
        }
        protected virtual void OnHoverChanged(EventArgs e) {
            if (HoverChanged != null) HoverChanged(this, e);
        }
        protected internal virtual void OnNodeAdded(STNodeEditorEventArgs e) {
            if (NodeAdded != null) NodeAdded(this, e);
        }
        protected internal virtual void OnNodeRemoved(STNodeEditorEventArgs e) {
            if (NodeRemoved != null) NodeRemoved(this, e);
        }
        protected virtual void OnCanvasMoved(EventArgs e) {
            if (CanvasMoved != null) CanvasMoved(this, e);
        }
        protected virtual void OnCanvasScaled(EventArgs e) {
            if (CanvasScaled != null) CanvasScaled(this, e);
        }
        protected internal virtual void OnOptionConnected(STNodeEditorOptionEventArgs e) {
            if (OptionConnected != null) OptionConnected(this, e);
        }
        protected internal virtual void OnOptionDisConnected(STNodeEditorOptionEventArgs e) {
            if (OptionDisConnected != null) OptionDisConnected(this, e);
        }
        protected internal virtual void OnOptionConnecting(STNodeEditorOptionEventArgs e) {
            if (OptionConnecting != null) OptionConnecting(this, e);
        }
        protected internal virtual void OnOptionDisConnecting(STNodeEditorOptionEventArgs e) {
            if (OptionDisConnecting != null) OptionDisConnecting(this, e);
        }

        #endregion event

        #region override -----------------------------------------------------------------------------------------------------

        protected override void OnCreateControl() {
            m_drawing_tools = new DrawingTools() {
                Pen = new Pen(Color.Black, 1),
                SolidBrush = new SolidBrush(Color.Black)
            };
            m_img_border = CreateBorderImage(_BorderColor);
            m_img_border_active = CreateBorderImage(_BorderActiveColor);
            m_img_border_hover = CreateBorderImage(_BorderHoverColor);
            m_img_border_selected = CreateBorderImage(_BorderSelectedColor);
            base.OnCreateControl();
            new Thread(MoveCanvasThread) { IsBackground = true }.Start();
            new Thread(ShowAlertThread) { IsBackground = true }.Start();
            m_sf = new StringFormat();
            m_sf.Alignment = StringAlignment.Near;
            m_sf.FormatFlags = StringFormatFlags.NoWrap;
            m_sf.SetTabStops(0, new float[] { 40 });
        }

        protected override void WndProc(ref Message m) {
            base.WndProc(ref m);
            try {
                Point pt = new Point(((int)m.LParam) >> 16, (ushort)m.LParam);
                pt = PointToClient(pt);
                if (m.Msg == WM_MOUSEHWHEEL) { //获取水平滚动消息
                    MouseButtons mb = MouseButtons.None;
                    int n = (ushort)m.WParam;
                    if ((n & 0x0001) == 0x0001) mb |= MouseButtons.Left;
                    if ((n & 0x0010) == 0x0010) mb |= MouseButtons.Middle;
                    if ((n & 0x0002) == 0x0002) mb |= MouseButtons.Right;
                    if ((n & 0x0020) == 0x0020) mb |= MouseButtons.XButton1;
                    if ((n & 0x0040) == 0x0040) mb |= MouseButtons.XButton2;
                    OnMouseHWheel(new MouseEventArgs(mb, 0, pt.X, pt.Y, ((int)m.WParam) >> 16));
                }
            } catch { /*add code*/ }
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e) {
            base.OnPaintSurface(e);
            m_canvas = e.Surface.Canvas;
            m_drawing_tools.Canvas = m_canvas;
            m_canvas.Clear(SkiaDrawingHelper.ToSKColor(BackColor));
            RenderEditorToCanvas(m_canvas);
            m_canvas = null;
        }

        private void RenderEditorToCanvas(SKCanvas canvas) {
            if (canvas == null) return;
            using (var bitmap = RenderEditorToBitmap()) {
                canvas.DrawBitmap(bitmap, 0, 0);
            }
        }

        private SKBitmap RenderEditorToBitmap() {
            var bitmap = new SKBitmap(Math.Max(Width, 1), Math.Max(Height, 1), true);
            using (var canvas = new SKCanvas(bitmap)) {
                canvas.Clear(SkiaDrawingHelper.ToSKColor(BackColor));
                RenderEditorToCanvasLegacy(canvas);
            }
            return bitmap;
        }

        private void RenderEditorToCanvasLegacy(SKCanvas canvas) {
            if (canvas == null) return;
            m_canvas = canvas;
            m_drawing_tools.Canvas = m_canvas;
            if (_ShowGrid) OnDrawGrid(m_drawing_tools, Width, Height);

            RenderLegacyNodeLayer(canvas);

            switch (m_ca) {
                case CanvasAction.MoveNode:
                    if (_ShowMagnet && _ActiveNode != null) OnDrawMagnet(m_drawing_tools, m_mi);
                    break;
                case CanvasAction.SelectRectangle:
                    OnDrawSelectedRectangle(m_drawing_tools, CanvasToControl(m_rect_select));
                    break;
                case CanvasAction.DrawMarkDetails:
                    if (!string.IsNullOrEmpty(m_find.Mark)) OnDrawMark(m_drawing_tools);
                    break;
            }
            // return bitmap;
        }

        //    if (this._ShowLocation) this.OnDrawNodeOutLocation(m_drawing_tools, this.Size, m_lst_node_out);
        //    this.OnDrawAlert();
        //}

        private void RenderLegacyNodeLayer(SKCanvas canvas) {
            if (canvas == null) return;
            m_drawing_tools.Canvas = canvas;
            canvas.Save();
            canvas.Translate(_CanvasOffsetX, _CanvasOffsetY);
            canvas.Scale(_CanvasScale, _CanvasScale);

            OnDrawConnectedLine(m_drawing_tools);
            OnDrawNode(m_drawing_tools, ControlToCanvas(ClientRectangle));

            if (m_ca == CanvasAction.ConnectOption) {
                using (var high = new SKPaint { Color = SkiaDrawingHelper.ToSKColor(_HighLineColor), Style = SKPaintStyle.Stroke, StrokeWidth = 2f, IsAntialias = true }) {
                    if (m_option_down.IsInput)
                        DrawBezier(canvas, high, m_pt_in_canvas, m_pt_dot_down, _Curvature);
                    else
                        DrawBezier(canvas, high, m_pt_dot_down, m_pt_in_canvas, _Curvature);
                }
            }
            canvas.Restore();
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            Focus();
            m_ca = CanvasAction.None;
            m_mi.XMatched = m_mi.YMatched = false;
            m_pt_down_in_control = e.Location;
            m_pt_down_in_canvas.X = ((e.X - _CanvasOffsetX) / _CanvasScale);
            m_pt_down_in_canvas.Y = ((e.Y - _CanvasOffsetY) / _CanvasScale);
            m_pt_canvas_old.X = _CanvasOffsetX;
            m_pt_canvas_old.Y = _CanvasOffsetY;

            if (m_gp_hover != null && e.Button == MouseButtons.Right) {     //断开连接
                DisConnectionHover();
                m_is_process_mouse_event = false; //终止MouseClick与MouseUp向下传递
                return;
            }

            NodeFindInfo nfi = FindNodeFromPoint(m_pt_down_in_canvas);
            if (!string.IsNullOrEmpty(nfi.Mark)) {                          //如果点下的是标记信息
                m_ca = CanvasAction.DrawMarkDetails;
                Invalidate();
                return;
            }

            if (nfi.NodeOption != null) {                                   //如果点下的Option的连接点
                StartConnect(nfi.NodeOption);
                return;
            }

            if (nfi.Node != null) {
                nfi.Node.OnMouseDown(new MouseEventArgs(e.Button, e.Clicks, (int)m_pt_down_in_canvas.X - nfi.Node.Left, (int)m_pt_down_in_canvas.Y - nfi.Node.Top, e.Delta));
                bool bCtrlDown = (Control.ModifierKeys & Keys.Control) == Keys.Control;
                if (bCtrlDown) {
                    if (nfi.Node.IsSelected) {
                        if (nfi.Node == _ActiveNode) {
                            SetActiveNode(null);
                        }
                    } else {
                        nfi.Node.SetSelected(true, true);
                    }
                    return;
                } else if (!nfi.Node.IsSelected) {
                    foreach (var n in m_hs_node_selected.ToArray()) n.SetSelected(false, false);
                }
                nfi.Node.SetSelected(true, false);                      //添加到已选择节点
                SetActiveNode(nfi.Node);
                if (PointInRectangle(nfi.Node.TitleRectangle, m_pt_down_in_canvas.X, m_pt_down_in_canvas.Y)) {
                    if (e.Button == MouseButtons.Right) {
                        if (nfi.Node.ContextMenuStrip != null) {
                            nfi.Node.ContextMenuStrip.Show(PointToScreen(e.Location));
                        }
                    } else {
                        m_dic_pt_selected.Clear();
                        lock (m_hs_node_selected) {
                            foreach (STNode n in m_hs_node_selected)    //记录已选择节点位置 如果需要移动已选中节点时候 将会有用
                                m_dic_pt_selected.Add(n, n.Location);
                        }
                        m_ca = CanvasAction.MoveNode;                   //如果点下的是节点的标题 则可以移动该节点
                        if (_ShowMagnet && _ActiveNode != null) BuildMagnetLocation();   //建立磁铁需要的坐标 如果需要移动已选中节点时候 将会有用
                    }
                } else
                    m_node_down = nfi.Node;
            } else {
                SetActiveNode(null);
                foreach (var n in m_hs_node_selected.ToArray()) n.SetSelected(false, false);//没有点下任何东西 清空已经选择节点
                m_ca = CanvasAction.SelectRectangle;                    //进入矩形区域选择模式
                m_rect_select.Width = m_rect_select.Height = 0;
                m_node_down = null;
            }
            //this.SetActiveNode(nfi.Node);
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            m_pt_in_control = e.Location;
            m_pt_in_canvas.X = ((e.X - _CanvasOffsetX) / _CanvasScale);
            m_pt_in_canvas.Y = ((e.Y - _CanvasOffsetY) / _CanvasScale);

            if (m_node_down != null) {
                m_node_down.OnMouseMove(new MouseEventArgs(e.Button, e.Clicks,
                    (int)m_pt_in_canvas.X - m_node_down.Left,
                    (int)m_pt_in_canvas.Y - m_node_down.Top, e.Delta));
                return;
            }

            if (e.Button == MouseButtons.Middle) {  //鼠标中键移动画布
                _CanvasOffsetX = m_real_canvas_x = m_pt_canvas_old.X + (e.X - m_pt_down_in_control.X);
                _CanvasOffsetY = m_real_canvas_y = m_pt_canvas_old.Y + (e.Y - m_pt_down_in_control.Y);
                Invalidate();
                return;
            }
            if (e.Button == MouseButtons.Left) {    //如果鼠标左键点下 判断行为
                m_gp_hover = null;
                switch (m_ca) {
                    case CanvasAction.MoveNode: MoveNode(e.Location); return;  //当前移动节点
                    case CanvasAction.ConnectOption: Invalidate(); return;     //当前正在连线
                    case CanvasAction.SelectRectangle:                              //当前正在选取
                        m_rect_select.X = m_pt_down_in_canvas.X < m_pt_in_canvas.X ? m_pt_down_in_canvas.X : m_pt_in_canvas.X;
                        m_rect_select.Y = m_pt_down_in_canvas.Y < m_pt_in_canvas.Y ? m_pt_down_in_canvas.Y : m_pt_in_canvas.Y;
                        m_rect_select.Width = Math.Abs(m_pt_in_canvas.X - m_pt_down_in_canvas.X);
                        m_rect_select.Height = Math.Abs(m_pt_in_canvas.Y - m_pt_down_in_canvas.Y);
                        foreach (STNode n in _Nodes) {
                            n.SetSelected(m_rect_select.IntersectsWith(n.Rectangle), false);
                        }
                        Invalidate();
                        return;
                }
            }
            //若不存在行为 则判断鼠标下方是否存在其他对象
            NodeFindInfo nfi = FindNodeFromPoint(m_pt_in_canvas);
            bool bRedraw = false;
            if (_HoverNode != nfi.Node) {          //鼠标悬停到Node上
                if (nfi.Node != null) nfi.Node.OnMouseEnter(EventArgs.Empty);
                if (_HoverNode != null)
                    _HoverNode.OnMouseLeave(new MouseEventArgs(e.Button, e.Clicks,
                        (int)m_pt_in_canvas.X - _HoverNode.Left,
                        (int)m_pt_in_canvas.Y - _HoverNode.Top, e.Delta));
                _HoverNode = nfi.Node;
                OnHoverChanged(EventArgs.Empty);
                bRedraw = true;
            }
            if (_HoverNode != null) {
                _HoverNode.OnMouseMove(new MouseEventArgs(e.Button, e.Clicks,
                    (int)m_pt_in_canvas.X - _HoverNode.Left,
                    (int)m_pt_in_canvas.Y - _HoverNode.Top, e.Delta));
                m_gp_hover = null;
            } else {
                GraphicsPath gp = null;
                foreach (var v in m_dic_gp_info) {          //判断鼠标是否悬停到连线路径上
                    if (v.Key.IsOutlineVisible(m_pt_in_canvas, m_p_line_hover)) {
                        gp = v.Key;
                        break;
                    }
                }
                if (m_gp_hover != gp) {
                    m_gp_hover = gp;
                    bRedraw = true;
                }
            }
            if (bRedraw) Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);
            var nfi = FindNodeFromPoint(m_pt_in_canvas);
            switch (m_ca) {                         //鼠标抬起时候 判断行为
                case CanvasAction.MoveNode:         //若正在移动Node 则重新记录当前位置
                    foreach (STNode n in m_dic_pt_selected.Keys.ToList()) m_dic_pt_selected[n] = n.Location;
                    break;
                case CanvasAction.ConnectOption:    //若正在连线 则结束连接
                    if (e.Location == m_pt_down_in_control) break;
                    if (nfi.NodeOption != null) {
                        if (m_option_down.IsInput)
                            nfi.NodeOption.ConnectOption(m_option_down);
                        else
                            m_option_down.ConnectOption(nfi.NodeOption);
                    }
                    break;
            }
            if (m_is_process_mouse_event && _ActiveNode != null) {
                var mea = new MouseEventArgs(e.Button, e.Clicks,
                    (int)m_pt_in_canvas.X - _ActiveNode.Left,
                    (int)m_pt_in_canvas.Y - _ActiveNode.Top, e.Delta);
                _ActiveNode.OnMouseUp(mea);
                m_node_down = null;
            }
            m_is_process_mouse_event = true;        //当前为断开连接操作不进行事件传递 下次将接受事件
            m_ca = CanvasAction.None;
            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e) {
            base.OnMouseEnter(e);
            m_mouse_in_control = true;
        }

        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);
            m_mouse_in_control = false;
            if (_HoverNode != null) _HoverNode.OnMouseLeave(e);
            _HoverNode = null;
            Invalidate();
        }

        protected override void OnMouseWheel(MouseEventArgs e) {
            base.OnMouseWheel(e);
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control) {
                float f = _CanvasScale + (e.Delta < 0 ? -0.1f : 0.1f);
                ScaleCanvas(f, Width / 2, Height / 2);
            } else {
                if (!m_mouse_in_control) return;
                var nfi = FindNodeFromPoint(m_pt_in_canvas);
                if (_HoverNode != null) {
                    _HoverNode.OnMouseWheel(new MouseEventArgs(e.Button, e.Clicks,
                        (int)m_pt_in_canvas.X - _HoverNode.Left,
                        (int)m_pt_in_canvas.Y - _HoverNode.Top, e.Delta));
                    return;
                }
                int t = (int)DateTime.Now.Subtract(m_dt_vw).TotalMilliseconds;
                if (t <= 30) t = 40;
                else if (t <= 100) t = 20;
                else if (t <= 150) t = 10;
                else if (t <= 300) t = 4;
                else t = 2;
                MoveCanvas(_CanvasOffsetX, m_real_canvas_y + (e.Delta < 0 ? -t : t), true, CanvasMoveArgs.Top);//process mouse mid
                m_dt_vw = DateTime.Now;
            }
        }

        protected virtual void OnMouseHWheel(MouseEventArgs e) {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control) return;
            if (!m_mouse_in_control) return;
            if (_HoverNode != null) {
                _HoverNode.OnMouseWheel(new MouseEventArgs(e.Button, e.Clicks,
                    (int)m_pt_in_canvas.X - _HoverNode.Left,
                    (int)m_pt_in_canvas.Y - _HoverNode.Top, e.Delta));
                return;
            }
            int t = (int)DateTime.Now.Subtract(m_dt_hw).TotalMilliseconds;
            if (t <= 30) t = 40;
            else if (t <= 100) t = 20;
            else if (t <= 150) t = 10;
            else if (t <= 300) t = 4;
            else t = 2;
            MoveCanvas(m_real_canvas_x + (e.Delta > 0 ? -t : t), _CanvasOffsetY, true, CanvasMoveArgs.Left);
            m_dt_hw = DateTime.Now;
        }
        //===========================for node other event==================================
        protected override void OnMouseClick(MouseEventArgs e) {
            base.OnMouseClick(e);
            if (_ActiveNode != null && m_is_process_mouse_event) {
                if (!PointInRectangle(_ActiveNode.Rectangle, m_pt_in_canvas.X, m_pt_in_canvas.Y)) return;
                _ActiveNode.OnMouseClick(new MouseEventArgs(e.Button, e.Clicks,
                    (int)m_pt_down_in_canvas.X - _ActiveNode.Left,
                    (int)m_pt_down_in_canvas.Y - _ActiveNode.Top, e.Delta));
            }
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);
            if (_ActiveNode != null) _ActiveNode.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e) {
            base.OnKeyUp(e);
            if (_ActiveNode != null) _ActiveNode.OnKeyUp(e);
            m_node_down = null;
        }

        protected override void OnKeyPress(KeyPressEventArgs e) {
            base.OnKeyPress(e);
            if (_ActiveNode != null) _ActiveNode.OnKeyPress(e);
        }

        #endregion

        protected override void OnDragEnter(DragEventArgs drgevent) {
            base.OnDragEnter(drgevent);
            if (DesignMode) return;
            if (drgevent.Data.GetDataPresent("STNodeType"))
                drgevent.Effect = DragDropEffects.Copy;
            else
                drgevent.Effect = DragDropEffects.None;

        }

        protected override void OnDragDrop(DragEventArgs drgevent) {
            base.OnDragDrop(drgevent);
            if (DesignMode) return;
            if (drgevent.Data.GetDataPresent("STNodeType")) {
                object data = drgevent.Data.GetData("STNodeType");
                if (!(data is Type)) return;
                var t = (Type)data;
                if (!t.IsSubclassOf(typeof(STNode))) return;
                STNode node = (STNode)Activator.CreateInstance((t));
                Point pt = new Point(drgevent.X, drgevent.Y);
                pt = PointToClient(pt);
                pt = ControlToCanvas(pt);
                node.Left = pt.X; node.Top = pt.Y;
                Nodes.Add(node);
            }
        }

        #region protected ----------------------------------------------------------------------------------------------------
        /// <summary>
        /// 当绘制背景网格线时候发生
        /// </summary>
        /// <param name="dt">绘制工具</param>
        /// <param name="nWidth">需要绘制宽度</param>
        /// <param name="nHeight">需要绘制高度</param>
        protected virtual void OnDrawGrid(DrawingTools dt, int nWidth, int nHeight) {
            if (m_canvas == null) return;
            using (var p1 = new SKPaint { Color = SkiaDrawingHelper.ToSKColor(Color.FromArgb(30, _GridColor)), Style = SKPaintStyle.Stroke, StrokeWidth = 1, IsAntialias = true })
            using (var p2 = new SKPaint { Color = SkiaDrawingHelper.ToSKColor(Color.FromArgb(65, _GridColor)), Style = SKPaintStyle.Stroke, StrokeWidth = 1, IsAntialias = true }) {
                float nIncrement = (20 * _CanvasScale);
                int n = 5 - (int)(_CanvasOffsetX / nIncrement);
                for (float f = _CanvasOffsetX % nIncrement; f < nWidth; f += nIncrement)
                    m_canvas.DrawLine(f, 0, f, nHeight, (n++ % 5 == 0 ? p2 : p1));
                n = 5 - (int)(_CanvasOffsetY / nIncrement);
                for (float f = _CanvasOffsetY % nIncrement; f < nHeight; f += nIncrement)
                    m_canvas.DrawLine(0, f, nWidth, f, (n++ % 5 == 0 ? p2 : p1));
                p1.Color = SkiaDrawingHelper.ToSKColor(Color.FromArgb(_Nodes.Count == 0 ? 255 : 120, _GridColor));
                m_canvas.DrawLine(_CanvasOffsetX, 0, _CanvasOffsetX, nHeight, p1);
                m_canvas.DrawLine(0, _CanvasOffsetY, nWidth, _CanvasOffsetY, p1);
            }
        }
        /// <summary>
        /// 当绘制 Node 时候发生
        /// </summary>
        /// <param name="dt">绘制工具</param>
        /// <param name="rect">可视画布区域大小</param>
        protected virtual void OnDrawNode(DrawingTools dt, Rectangle rect) {
            m_lst_node_out.Clear(); //清空超出视觉区域的 Node 的坐标
            foreach (STNode n in _Nodes) {
                if (_ShowBorder) OnDrawNodeBorder(dt, n);
                n.OnDrawNode(dt);                                       //调用 Node 进行自身绘制主体部分
                if (!string.IsNullOrEmpty(n.Mark)) n.OnDrawMark(dt);    //调用 Node 进行自身绘制 Mark 区域
                if (!rect.IntersectsWith(n.Rectangle)) {
                    m_lst_node_out.Add(n.Location);                     //判断此 Node 是否超出视觉区域
                }
            }
        }
        /// <summary>
        /// 当绘制 Node 边框时候发生
        /// </summary>
        /// <param name="dt">绘制工具</param>
        /// <param name="node">目标node</param>
        protected virtual void OnDrawNodeBorder(DrawingTools dt, STNode node) {
            Image img_border = null;
            if (_ActiveNode == node) img_border = m_img_border_active;
            else if (node.IsSelected) img_border = m_img_border_selected;
            else if (_HoverNode == node) img_border = m_img_border_hover;
            else img_border = m_img_border;
            RenderBorder(m_canvas, node.Rectangle, img_border);
            if (!string.IsNullOrEmpty(node.Mark)) RenderBorder(m_canvas, node.MarkRectangle, img_border);
        }
        /// <summary>
        /// 当绘制已连接路径时候发生
        /// </summary>
        /// <param name="dt">绘制工具</param>
        protected virtual void OnDrawConnectedLine(DrawingTools dt) {
            if (m_canvas == null) return;
            var t = typeof(object);
            using (var hover = new SKPaint { Color = SkiaDrawingHelper.ToSKColor(Color.FromArgb(50, 0, 0, 0)), Style = SKPaintStyle.Stroke, StrokeWidth = 4f, IsAntialias = true })
            using (var line = new SKPaint { Color = SKColors.Cyan, Style = SKPaintStyle.Stroke, StrokeWidth = 2f, IsAntialias = true }) {
                foreach (STNode n in _Nodes) {
                    foreach (STNodeOption op in n.OutputOptions) {
                        if (op == STNodeOption.Empty) continue;
                        Color lineColor;
                        if (op.DotColor != Color.Transparent) lineColor = op.DotColor;
                        else if (op.DataType == t) lineColor = _UnknownTypeColor;
                        else lineColor = _TypeColor.ContainsKey(op.DataType) ? _TypeColor[op.DataType] : _UnknownTypeColor;
                        line.Color = SkiaDrawingHelper.ToSKColor(lineColor);

                        foreach (var v in op.ConnectedOption) {
                            DrawBezier(m_canvas, hover, op.DotLeft + op.DotSize, op.DotTop + op.DotSize / 2,
                                v.DotLeft - 1, v.DotTop + v.DotSize / 2, _Curvature);
                            DrawBezier(m_canvas, line, op.DotLeft + op.DotSize, op.DotTop + op.DotSize / 2,
                                v.DotLeft - 1, v.DotTop + v.DotSize / 2, _Curvature);
                            if (m_is_buildpath) {
                                GraphicsPath gp = CreateBezierPath(op.DotLeft + op.DotSize, op.DotTop + op.DotSize / 2,
                                    v.DotLeft - 1, v.DotTop + v.DotSize / 2, _Curvature);
                                m_dic_gp_info.Add(gp, new ConnectionInfo() { Output = op, Input = v });
                            }
                        }
                    }
                }
                if (m_gp_hover != null) {
                    using (var hp = ToSKPath(m_gp_hover)) {
                        if (hp != null) {
                            hover.Color = SkiaDrawingHelper.ToSKColor(_HighLineColor);
                            m_canvas.DrawPath(hp, hover);
                        }
                    }
                }
            }
            m_is_buildpath = false;
        }

        /// <summary>
        /// 当绘制 Mark 详情信息时候发生
        /// </summary>
        /// <param name="dt">绘制工具</param>
        protected virtual void OnDrawMark(DrawingTools dt) {
            if (m_canvas == null || string.IsNullOrEmpty(m_find.Mark)) return;
            using (var textPaint = new SKPaint { TextSize = Math.Max(10f, Font.Size), IsAntialias = true }) {
                float textWidth = textPaint.MeasureText(m_find.Mark);
                Rectangle rect = new Rectangle(m_pt_in_control.X + 15,
                    m_pt_in_control.Y + 10,
                    (int)Math.Ceiling(textWidth) + 6,
                    4 + (Font.Height + 4) * m_find.MarkLines.Length);

                if (rect.Right > Width) rect.X = Width - rect.Width;
                if (rect.Bottom > Height) rect.Y = Height - rect.Height;
                if (rect.X < 0) rect.X = 0;
                if (rect.Y < 0) rect.Y = 0;

                using (var bg = new SKPaint { Color = SkiaDrawingHelper.ToSKColor(_MarkBackColor), Style = SKPaintStyle.Fill, IsAntialias = false })
                using (var border = new SKPaint { Color = SkiaDrawingHelper.ToSKColor(Color.FromArgb(255, _MarkBackColor)), Style = SKPaintStyle.Stroke, StrokeWidth = 1, IsAntialias = false })
                using (var fg = new SKPaint { Color = SkiaDrawingHelper.ToSKColor(_MarkForeColor), TextSize = Math.Max(10f, Font.Size), IsAntialias = true }) {
                    m_canvas.DrawRect(rect.Left, rect.Top, rect.Width, rect.Height, bg);
                    m_canvas.DrawRect(rect.Left, rect.Top, rect.Width - 1, rect.Height - 1, border);

                    var fm = fg.FontMetrics;
                    float baseY = rect.Y + 2 - fm.Ascent;
                    float textX = rect.X + 4;
                    for (int i = 0; i < m_find.MarkLines.Length; i++) {
                        float y = baseY + i * (Font.Height + 4);
                        m_canvas.DrawText(m_find.MarkLines[i], textX, y, fg);
                    }
                }
            }
        }
        /// <summary>
        /// 当移动 Node 时候 需要显示对齐参考线时候发生
        /// </summary>
        /// <param name="dt">绘制工具</param>
        /// <param name="mi">匹配的磁铁信息</param>
        protected virtual void OnDrawMagnet(DrawingTools dt, MagnetInfo mi) {
            if (m_canvas == null || _ActiveNode == null) return;
            using (var line = new SKPaint { Color = SkiaDrawingHelper.ToSKColor(_MagnetColor), Style = SKPaintStyle.Stroke, StrokeWidth = 1, IsAntialias = false })
            using (var fill = new SKPaint { Color = SkiaDrawingHelper.ToSKColor(Color.FromArgb(_MagnetColor.A / 3, _MagnetColor)), Style = SKPaintStyle.Fill, IsAntialias = false }) {
                if (mi.XMatched) {
                    float x = CanvasToControl(mi.X, true);
                    m_canvas.DrawLine(x, 0, x, Height, line);
                }
                if (mi.YMatched) {
                    float y = CanvasToControl(mi.Y, false);
                    m_canvas.DrawLine(0, y, Width, y, line);
                }

                m_canvas.Save();
                m_canvas.Translate(_CanvasOffsetX, _CanvasOffsetY);
                m_canvas.Scale(_CanvasScale, _CanvasScale);

                if (mi.XMatched) {
                    foreach (STNode n in _Nodes) {
                        if (n.Left == mi.X || n.Right == mi.X || n.Left + n.Width / 2 == mi.X) {
                            m_canvas.DrawRect(n.Left, n.Top, n.Width, n.Height, fill);
                        }
                    }
                }
                if (mi.YMatched) {
                    foreach (STNode n in _Nodes) {
                        if (n.Top == mi.Y || n.Bottom == mi.Y || n.Top + n.Height / 2 == mi.Y) {
                            m_canvas.DrawRect(n.Left, n.Top, n.Width, n.Height, fill);
                        }
                    }
                }

                m_canvas.Restore();
            }
        }
        /// <summary>
        /// 绘制选择的矩形区域
        /// </summary>
        /// <param name="dt">绘制工具</param>
        /// <param name="rectf">位于控件上的矩形区域</param>
        protected virtual void OnDrawSelectedRectangle(DrawingTools dt, RectangleF rectf) {
            if (m_canvas == null) return;
            var fill = Color.FromArgb(_SelectedRectangleColor.A / 3, _SelectedRectangleColor);
            Rectangle rect = Rectangle.Round(CanvasToControl(m_rect_select));
            using (var stroke = new SKPaint { Color = SkiaDrawingHelper.ToSKColor(_SelectedRectangleColor), Style = SKPaintStyle.Stroke, StrokeWidth = 1, IsAntialias = true })
            using (var bg = new SKPaint { Color = SkiaDrawingHelper.ToSKColor(fill), Style = SKPaintStyle.Fill, IsAntialias = true }) {
                m_canvas.DrawRect(rectf.Left, rectf.Top, rectf.Width, rectf.Height, stroke);
                m_canvas.DrawRect(rect.Left, rect.Top, rect.Width, rect.Height, bg);
            }
        }
        /// <summary>
        /// 绘制超出视觉区域的 Node 位置提示信息
        /// </summary>
        /// <param name="dt">绘制工具</param>
        /// <param name="sz">提示框边距</param>
        /// <param name="lstPts">超出视觉区域的 Node 位置信息</param>
        protected virtual void OnDrawNodeOutLocation(DrawingTools dt, Size sz, List<Point> lstPts) {
            if (m_canvas == null) return;
            using (var back = new SKPaint { Color = SkiaDrawingHelper.ToSKColor(_LocationBackColor), Style = SKPaintStyle.Fill, IsAntialias = false })
            using (var fore = new SKPaint { Color = SkiaDrawingHelper.ToSKColor(_LocationForeColor), Style = SKPaintStyle.Fill, IsAntialias = false }) {
                if (lstPts.Count == _Nodes.Count && _Nodes.Count != 0) {
                    Rectangle r = CanvasToControl(_CanvasValidBounds);
                    m_canvas.DrawRect(r.Left, r.Top, r.Width, r.Height, back);
                }
                m_canvas.DrawRect(0, 0, 4, sz.Height, back);
                m_canvas.DrawRect(sz.Width - 4, 0, 4, sz.Height, back);
                m_canvas.DrawRect(4, 0, sz.Width - 8, 4, back);
                m_canvas.DrawRect(4, sz.Height - 4, sz.Width - 8, 4, back);
                foreach (var v in lstPts) {
                    var pt = CanvasToControl(v);
                    if (pt.X < 0) pt.X = 0;
                    if (pt.Y < 0) pt.Y = 0;
                    if (pt.X > sz.Width) pt.X = sz.Width - 4;
                    if (pt.Y > sz.Height) pt.Y = sz.Height - 4;
                    m_canvas.DrawRect(pt.X, pt.Y, 4, 4, fore);
                }
            }
        }
        /// <summary>
        /// 绘制提示信息
        /// </summary>
        /// <param name="dt">绘制工具</param>
        /// <param name="rect">需要绘制区域</param>
        /// <param name="strText">需要绘制文本</param>
        /// <param name="foreColor">信息前景色</param>
        /// <param name="backColor">信息背景色</param>
        /// <param name="al">信息位置</param>
        protected virtual void OnDrawAlert(DrawingTools dt, Rectangle rect, string strText, Color foreColor, Color backColor, AlertLocation al) {
            if (m_alpha_alert == 0 || m_canvas == null) return;
            using (var bg = new SKPaint { Color = SkiaDrawingHelper.ToSKColor(backColor), Style = SKPaintStyle.Fill, IsAntialias = false })
            using (var border = new SKPaint { Color = SkiaDrawingHelper.ToSKColor(backColor), Style = SKPaintStyle.Stroke, StrokeWidth = 1, IsAntialias = false })
            using (var text = new SKPaint { Color = SkiaDrawingHelper.ToSKColor(foreColor), TextSize = Math.Max(10f, Font.Size), IsAntialias = true }) {
                m_canvas.DrawRect(rect.Left, rect.Top, rect.Width, rect.Height, bg);
                m_canvas.DrawRect(rect.Left, rect.Top, rect.Width - 1, rect.Height - 1, border);
                var fm = text.FontMetrics;
                float x = rect.Left + (rect.Width - text.MeasureText(strText)) / 2f;
                float y = rect.Top + (rect.Height - (fm.Descent - fm.Ascent)) / 2f - fm.Ascent;
                m_canvas.DrawText(strText, x, y, text);
            }
        }
        /// <summary>
        /// 获取提示信息需要绘制的矩形区域
        /// </summary>
        /// <param name="g">绘图表面</param>
        /// <param name="strText">需要绘制文本</param>
        /// <param name="al">信息位置</param>
        /// <returns>矩形区域</returns>
        protected virtual Rectangle GetAlertRectangle(string strText, AlertLocation al) {
            using (var text = new SKPaint { TextSize = Math.Max(10f, Font.Size), IsAntialias = true }) {
                var fm = text.FontMetrics;
                float w = text.MeasureText(m_str_alert);
                float h = fm.Descent - fm.Ascent;
                Size sz = new Size((int)Math.Round(w + 10), (int)Math.Round(h + 4));
                Rectangle rect = new Rectangle(4, Height - sz.Height - 4, sz.Width, sz.Height);

            switch (al) {
                case AlertLocation.Left:
                    rect.Y = (Height - sz.Height) >> 1;
                    break;
                case AlertLocation.Top:
                    rect.Y = 4;
                    rect.X = (Width - sz.Width) >> 1;
                    break;
                case AlertLocation.Right:
                    rect.X = Width - sz.Width - 4;
                    rect.Y = (Height - sz.Height) >> 1;
                    break;
                case AlertLocation.Bottom:
                    rect.X = (Width - sz.Width) >> 1;
                    break;
                case AlertLocation.Center:
                    rect.X = (Width - sz.Width) >> 1;
                    rect.Y = (Height - sz.Height) >> 1;
                    break;
                case AlertLocation.LeftTop:
                    rect.X = rect.Y = 4;
                    break;
                case AlertLocation.RightTop:
                    rect.Y = 4;
                    rect.X = Width - sz.Width - 4;
                    break;
                case AlertLocation.RightBottom:
                    rect.X = Width - sz.Width - 4;
                    break;
            }
                return rect;
            }
        }

        #endregion protected

        #region internal

        internal void BuildLinePath() {
            foreach (var v in m_dic_gp_info) v.Key.Dispose();
            m_dic_gp_info.Clear();
            m_is_buildpath = true;
            Invalidate();
        }

        internal void OnDrawAlert() {
            m_rect_alert = GetAlertRectangle(m_str_alert, m_al);
            Color clr_fore = Color.FromArgb((int)((float)m_alpha_alert / 255 * m_forecolor_alert.A), m_forecolor_alert);
            Color clr_back = Color.FromArgb((int)((float)m_alpha_alert / 255 * m_backcolor_alert.A), m_backcolor_alert);
            OnDrawAlert(m_drawing_tools, m_rect_alert, m_str_alert, clr_fore, clr_back, m_al);
        }

        internal void InternalAddSelectedNode(STNode node) {
            node.IsSelected = true;
            lock (m_hs_node_selected) m_hs_node_selected.Add(node);
        }

        internal void InternalRemoveSelectedNode(STNode node) {
            node.IsSelected = false;
            lock (m_hs_node_selected) m_hs_node_selected.Remove(node);
        }

        #endregion internal

        #region private -----------------------------------------------------------------------------------------------------

        private void MoveCanvasThread() {
            bool bRedraw;
            while (true) {
                bRedraw = false;
                if (m_real_canvas_x != _CanvasOffsetX) {
                    float nx = m_real_canvas_x - _CanvasOffsetX;
                    float n = Math.Abs(nx) / 10;
                    float nTemp = Math.Abs(nx);
                    if (nTemp <= 4) n = 1;
                    else if (nTemp <= 12) n = 2;
                    else if (nTemp <= 30) n = 3;
                    if (nTemp < 1) _CanvasOffsetX = m_real_canvas_x;
                    else
                        _CanvasOffsetX += nx > 0 ? n : -n;
                    bRedraw = true;
                }
                if (m_real_canvas_y != _CanvasOffsetY) {
                    float ny = m_real_canvas_y - _CanvasOffsetY;
                    float n = Math.Abs(ny) / 10;
                    float nTemp = Math.Abs(ny);
                    if (nTemp <= 4) n = 1;
                    else if (nTemp <= 12) n = 2;
                    else if (nTemp <= 30) n = 3;
                    if (nTemp < 1)
                        _CanvasOffsetY = m_real_canvas_y;
                    else
                        _CanvasOffsetY += ny > 0 ? n : -n;
                    bRedraw = true;
                }
                if (bRedraw) {
                    m_pt_canvas_old.X = _CanvasOffsetX;
                    m_pt_canvas_old.Y = _CanvasOffsetY;
                    Invalidate();
                    Thread.Sleep(30);
                } else {
                    Thread.Sleep(100);
                }
            }
        }

        private void ShowAlertThread() {
            while (true) {
                int nTime = m_time_alert - (int)DateTime.Now.Subtract(m_dt_alert).TotalMilliseconds;
                if (nTime > 0) {
                    Thread.Sleep(nTime);
                    continue;
                }
                if (nTime < -1000) {
                    if (m_alpha_alert != 0) {
                        m_alpha_alert = 0;
                        Invalidate();
                    }
                    Thread.Sleep(100);
                } else {
                    m_alpha_alert = (int)(255 - (-nTime / 1000F) * 255);
                    Invalidate(m_rect_alert);
                    Thread.Sleep(50);
                }
            }
        }

        private Image CreateBorderImage(Color clr) {
            using (var bitmap = new SKBitmap(12, 12, true))
            using (var canvas = new SKCanvas(bitmap))
            using (var paint = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Fill }) {
                var center = SkiaDrawingHelper.ToSKColor(Color.FromArgb(200, clr));
                var edge = SkiaDrawingHelper.ToSKColor(Color.FromArgb(10, clr));
                using (var shader = SKShader.CreateRadialGradient(new SKPoint(5.5f, 5.5f), 5.5f,
                    new[] { center, edge },
                    new float[] { 0f, 1f }, SKShaderTileMode.Clamp)) {
                    paint.Shader = shader;
                    canvas.DrawCircle(5.5f, 5.5f, 5.5f, paint);
                }
                using (var image = SKImage.FromBitmap(bitmap))
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                using (var ms = new MemoryStream(data.ToArray())) {
                    return Image.FromStream(ms);
                }
            }
        }

        private ConnectionStatus DisConnectionHover() {
            if (!m_dic_gp_info.ContainsKey(m_gp_hover)) return ConnectionStatus.DisConnected;
            ConnectionInfo ci = m_dic_gp_info[m_gp_hover];
            var ret = ci.Output.DisConnectOption(ci.Input);
            //this.OnOptionDisConnected(new STNodeOptionEventArgs(ci.Output, ci.Input, ret));
            if (ret == ConnectionStatus.DisConnected) {
                m_dic_gp_info.Remove(m_gp_hover);
                m_gp_hover.Dispose();
                m_gp_hover = null;
                Invalidate();
            }
            return ret;
        }

        private void StartConnect(STNodeOption op) {
            if (op.IsInput) {
                m_pt_dot_down.X = op.DotLeft;
                m_pt_dot_down.Y = op.DotTop + 5;
            } else {
                m_pt_dot_down.X = op.DotLeft + op.DotSize;
                m_pt_dot_down.Y = op.DotTop + 5;
            }
            m_ca = CanvasAction.ConnectOption;
            m_option_down = op;
        }

        private void MoveNode(Point pt) {
            int nX = (int)((pt.X - m_pt_down_in_control.X) / _CanvasScale);
            int nY = (int)((pt.Y - m_pt_down_in_control.Y) / _CanvasScale);
            lock (m_hs_node_selected) {
                foreach (STNode v in m_hs_node_selected) {
                    v.Left = m_dic_pt_selected[v].X + nX;
                    v.Top = m_dic_pt_selected[v].Y + nY;
                }
                if (_ShowMagnet) {
                    MagnetInfo mi = CheckMagnet(_ActiveNode);
                    if (mi.XMatched) {
                        foreach (STNode v in m_hs_node_selected) v.Left -= mi.OffsetX;
                    }
                    if (mi.YMatched) {
                        foreach (STNode v in m_hs_node_selected) v.Top -= mi.OffsetY;
                    }
                }
            }
            Invalidate();
        }

        protected internal virtual void BuildBounds() {
            if (_Nodes.Count == 0) {
                _CanvasValidBounds = ControlToCanvas(DisplayRectangle);
                return;
            }
            int x = int.MaxValue;
            int y = int.MaxValue;
            int r = int.MinValue;
            int b = int.MinValue;
            foreach (STNode n in _Nodes) {
                if (x > n.Left) x = n.Left;
                if (y > n.Top) y = n.Top;
                if (r < n.Right) r = n.Right;
                if (b < n.Bottom) b = n.Bottom;
            }
            _CanvasValidBounds.X = x - 60;
            _CanvasValidBounds.Y = y - 60;
            _CanvasValidBounds.Width = r - x + 120;
            _CanvasValidBounds.Height = b - y + 120;
        }

        private bool PointInRectangle(Rectangle rect, float x, float y) {
            if (x < rect.Left) return false;
            if (x > rect.Right) return false;
            if (y < rect.Top) return false;
            if (y > rect.Bottom) return false;
            return true;
        }

        private void BuildMagnetLocation() {
            m_lst_magnet_x.Clear();
            m_lst_magnet_y.Clear();
            foreach (STNode v in _Nodes) {
                if (v.IsSelected) continue;
                m_lst_magnet_x.Add(v.Left);
                m_lst_magnet_x.Add(v.Left + v.Width / 2);
                m_lst_magnet_x.Add(v.Left + v.Width);
                m_lst_magnet_y.Add(v.Top);
                m_lst_magnet_y.Add(v.Top + v.Height / 2);
                m_lst_magnet_y.Add(v.Top + v.Height);
            }
        }

        private MagnetInfo CheckMagnet(STNode node) {
            m_mi.XMatched = m_mi.YMatched = false;
            m_lst_magnet_mx.Clear();
            m_lst_magnet_my.Clear();
            m_lst_magnet_mx.Add(node.Left + node.Width / 2);
            m_lst_magnet_mx.Add(node.Left);
            m_lst_magnet_mx.Add(node.Left + node.Width);
            m_lst_magnet_my.Add(node.Top + node.Height / 2);
            m_lst_magnet_my.Add(node.Top);
            m_lst_magnet_my.Add(node.Top + node.Height);

            bool bFlag = false;
            foreach (var mx in m_lst_magnet_mx) {
                foreach (var x in m_lst_magnet_x) {
                    if (Math.Abs(mx - x) <= 5) {
                        bFlag = true;
                        m_mi.X = x;
                        m_mi.OffsetX = mx - x;
                        m_mi.XMatched = true;
                        break;
                    }
                }
                if (bFlag) break;
            }
            bFlag = false;
            foreach (var my in m_lst_magnet_my) {
                foreach (var y in m_lst_magnet_y) {
                    if (Math.Abs(my - y) <= 5) {
                        bFlag = true;
                        m_mi.Y = y;
                        m_mi.OffsetY = my - y;
                        m_mi.YMatched = true;
                        break;
                    }
                }
                if (bFlag) break;
            }
            return m_mi;
        }

        private void DrawBezier(SKCanvas canvas, SKPaint paint, PointF ptStart, PointF ptEnd, float f) {
            DrawBezier(canvas, paint, ptStart.X, ptStart.Y, ptEnd.X, ptEnd.Y, f);
        }

        private void DrawBezier(SKCanvas canvas, SKPaint paint, float x1, float y1, float x2, float y2, float f) {
            if (canvas == null || paint == null) return;
            float n = (Math.Abs(x1 - x2) * f);
            if (_Curvature != 0 && n < 30) n = 30;
            using (var path = new SKPath()) {
                path.MoveTo(x1, y1);
                path.CubicTo(x1 + n, y1, x2 - n, y2, x2, y2);
                canvas.DrawPath(path, paint);
            }
        }

        private GraphicsPath CreateBezierPath(float x1, float y1, float x2, float y2, float f) {
            GraphicsPath gp = new GraphicsPath();
            float n = (Math.Abs(x1 - x2) * f);
            if (_Curvature != 0 && n < 30) n = 30;
            gp.AddBezier(
                x1, y1,
                x1 + n, y1,
                x2 - n, y2,
                x2, y2
                );
            return gp;
        }

        private SKPath ToSKPath(GraphicsPath gp) {
            if (gp == null) return null;
            var points = gp.PathPoints;
            if (points == null || points.Length < 2) return null;
            var path = new SKPath();
            path.MoveTo(points[0].X, points[0].Y);
            int i = 1;
            while (i + 2 < points.Length) {
                path.CubicTo(points[i].X, points[i].Y, points[i + 1].X, points[i + 1].Y, points[i + 2].X, points[i + 2].Y);
                i += 3;
            }
            return path;
        }

        private void RenderBorder(SKCanvas canvas, Rectangle rect, Image img) {
            if (canvas == null || img == null) return;
            using (var bmp = SkiaDrawingHelper.ToSKBitmap(img)) {
                if (bmp == null) return;
                //填充四个角
                canvas.DrawBitmap(bmp, new SKRect(0, 0, 5, 5), new SKRect(rect.X - 5, rect.Y - 5, rect.X, rect.Y));
                canvas.DrawBitmap(bmp, new SKRect(img.Width - 5, 0, img.Width, 5), new SKRect(rect.Right, rect.Y - 5, rect.Right + 5, rect.Y));
                canvas.DrawBitmap(bmp, new SKRect(0, img.Height - 5, 5, img.Height), new SKRect(rect.X - 5, rect.Bottom, rect.X, rect.Bottom + 5));
                canvas.DrawBitmap(bmp, new SKRect(img.Width - 5, img.Height - 5, img.Width, img.Height), new SKRect(rect.Right, rect.Bottom, rect.Right + 5, rect.Bottom + 5));
                //四边
                canvas.DrawBitmap(bmp, new SKRect(0, 5, 5, img.Height - 10), new SKRect(rect.X - 5, rect.Y, rect.X, rect.Bottom));
                canvas.DrawBitmap(bmp, new SKRect(5, 0, img.Width - 10, 5), new SKRect(rect.X, rect.Y - 5, rect.Right, rect.Y));
                canvas.DrawBitmap(bmp, new SKRect(img.Width - 5, 5, img.Width, img.Height - 10), new SKRect(rect.Right, rect.Y, rect.Right + 5, rect.Bottom));
                canvas.DrawBitmap(bmp, new SKRect(5, img.Height - 5, img.Width - 10, img.Height), new SKRect(rect.X, rect.Bottom, rect.Right, rect.Bottom + 5));
            }
        }

        #endregion private

        #region public --------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 通过画布坐标进行寻找
        /// </summary>
        /// <param name="pt">画布中的坐标</param>
        /// <returns>寻找到的数据</returns>
        public NodeFindInfo FindNodeFromPoint(PointF pt) {
            m_find.Node = null; m_find.NodeOption = null; m_find.Mark = null;
            for (int i = _Nodes.Count - 1; i >= 0; i--) {
                if (!string.IsNullOrEmpty(_Nodes[i].Mark) && PointInRectangle(_Nodes[i].MarkRectangle, pt.X, pt.Y)) {
                    m_find.Mark = _Nodes[i].Mark;
                    m_find.MarkLines = _Nodes[i].MarkLines;
                    return m_find;
                }
                foreach (STNodeOption v in _Nodes[i].InputOptions) {
                    if (v == STNodeOption.Empty) continue;
                    if (PointInRectangle(v.DotRectangle, pt.X, pt.Y)) m_find.NodeOption = v;
                }
                foreach (STNodeOption v in _Nodes[i].OutputOptions) {
                    if (v == STNodeOption.Empty) continue;
                    if (PointInRectangle(v.DotRectangle, pt.X, pt.Y)) m_find.NodeOption = v;
                }
                if (PointInRectangle(_Nodes[i].Rectangle, pt.X, pt.Y)) {
                    m_find.Node = _Nodes[i];
                }
                if (m_find.NodeOption != null || m_find.Node != null) return m_find;
            }
            return m_find;
        }
        /// <summary>
        /// 获取已经被选择的 Node 集合
        /// </summary>
        /// <returns>Node 集合</returns>
        public STNode[] GetSelectedNode() {
            return m_hs_node_selected.ToArray();
        }
        /// <summary>
        /// 将画布坐标转换为控件坐标
        /// </summary>
        /// <param name="number">参数</param>
        /// <param name="isX">是否为 X 坐标</param>
        /// <returns>转换后的坐标</returns>
        public float CanvasToControl(float number, bool isX) {
            return (number * _CanvasScale) + (isX ? _CanvasOffsetX : _CanvasOffsetY);
        }
        /// <summary>
        /// 将画布坐标转换为控件坐标
        /// </summary>
        /// <param name="pt">坐标</param>
        /// <returns>转换后的坐标</returns>
        public PointF CanvasToControl(PointF pt) {
            pt.X = (pt.X * _CanvasScale) + _CanvasOffsetX;
            pt.Y = (pt.Y * _CanvasScale) + _CanvasOffsetY;
            //pt.X += this._CanvasOffsetX;
            //pt.Y += this._CanvasOffsetY;
            return pt;
        }
        /// <summary>
        /// 将画布坐标转换为控件坐标
        /// </summary>
        /// <param name="pt">坐标</param>
        /// <returns>转换后的坐标</returns>
        public Point CanvasToControl(Point pt) {
            pt.X = (int)(pt.X * _CanvasScale + _CanvasOffsetX);
            pt.Y = (int)(pt.Y * _CanvasScale + _CanvasOffsetY);
            //pt.X += (int)this._CanvasOffsetX;
            //pt.Y += (int)this._CanvasOffsetY;
            return pt;
        }
        /// <summary>
        /// 将画布坐标转换为控件坐标
        /// </summary>
        /// <param name="rect">矩形区域</param>
        /// <returns>转换后的矩形区域</returns>
        public Rectangle CanvasToControl(Rectangle rect) {
            rect.X = (int)((rect.X * _CanvasScale) + _CanvasOffsetX);
            rect.Y = (int)((rect.Y * _CanvasScale) + _CanvasOffsetY);
            rect.Width = (int)(rect.Width * _CanvasScale);
            rect.Height = (int)(rect.Height * _CanvasScale);
            //rect.X += (int)this._CanvasOffsetX;
            //rect.Y += (int)this._CanvasOffsetY;
            return rect;
        }
        /// <summary>
        /// 将画布坐标转换为控件坐标
        /// </summary>
        /// <param name="rect">矩形区域</param>
        /// <returns>转换后的矩形区域</returns>
        public RectangleF CanvasToControl(RectangleF rect) {
            rect.X = (rect.X * _CanvasScale) + _CanvasOffsetX;
            rect.Y = (rect.Y * _CanvasScale) + _CanvasOffsetY;
            rect.Width = (rect.Width * _CanvasScale);
            rect.Height = (rect.Height * _CanvasScale);
            //rect.X += this._CanvasOffsetX;
            //rect.Y += this._CanvasOffsetY;
            return rect;
        }
        /// <summary>
        /// 将控件坐标转换为画布坐标
        /// </summary>
        /// <param name="number">参数</param>
        /// <param name="isX">是否为 X 坐标</param>
        /// <returns>转换后的坐标</returns>
        public float ControlToCanvas(float number, bool isX) {
            return (number - (isX ? _CanvasOffsetX : _CanvasOffsetY)) / _CanvasScale;
        }
        /// <summary>
        /// 将控件坐标转换为画布坐标
        /// </summary>
        /// <param name="pt">坐标</param>
        /// <returns>转换后的坐标</returns>
        public Point ControlToCanvas(Point pt) {
            pt.X = (int)((pt.X - _CanvasOffsetX) / _CanvasScale);
            pt.Y = (int)((pt.Y - _CanvasOffsetY) / _CanvasScale);
            return pt;
        }
        /// <summary>
        /// 将控件坐标转换为画布坐标
        /// </summary>
        /// <param name="pt">坐标</param>
        /// <returns>转换后的坐标</returns>
        public PointF ControlToCanvas(PointF pt) {
            pt.X = ((pt.X - _CanvasOffsetX) / _CanvasScale);
            pt.Y = ((pt.Y - _CanvasOffsetY) / _CanvasScale);
            return pt;
        }
        /// <summary>
        /// 将控件坐标转换为画布坐标
        /// </summary>
        /// <param name="rect">矩形区域</param>
        /// <returns>转换后的区域</returns>
        public Rectangle ControlToCanvas(Rectangle rect) {
            rect.X = (int)((rect.X - _CanvasOffsetX) / _CanvasScale);
            rect.Y = (int)((rect.Y - _CanvasOffsetY) / _CanvasScale);
            rect.Width = (int)(rect.Width / _CanvasScale);
            rect.Height = (int)(rect.Height / _CanvasScale);
            return rect;
        }
        /// <summary>
        /// 将控件坐标转换为画布坐标
        /// </summary>
        /// <param name="rect">矩形区域</param>
        /// <returns>转换后的区域</returns>
        public RectangleF ControlToCanvas(RectangleF rect) {
            rect.X = ((rect.X - _CanvasOffsetX) / _CanvasScale);
            rect.Y = ((rect.Y - _CanvasOffsetY) / _CanvasScale);
            rect.Width = (rect.Width / _CanvasScale);
            rect.Height = (rect.Height / _CanvasScale);
            return rect;
        }
        /// <summary>
        /// 移动画布原点坐标到指定的控件坐标位置
        /// 当不存在 Node 时候 无法移动
        /// </summary>
        /// <param name="x">X 坐标</param>
        /// <param name="y">Y 坐标</param>
        /// <param name="bAnimation">移动过程中是否启动动画效果</param>
        /// <param name="ma">指定需要修改的坐标参数</param>
        public void MoveCanvas(float x, float y, bool bAnimation, CanvasMoveArgs ma) {
            if (_Nodes.Count == 0) {
                m_real_canvas_x = m_real_canvas_y = 10;
                return;
            }
            int l = (int)((_CanvasValidBounds.Left + 50) * _CanvasScale);
            int t = (int)((_CanvasValidBounds.Top + 50) * _CanvasScale);
            int r = (int)((_CanvasValidBounds.Right - 50) * _CanvasScale);
            int b = (int)((_CanvasValidBounds.Bottom - 50) * _CanvasScale);
            if (r + x < 0) x = -r;
            if (Width - l < x) x = Width - l;
            if (b + y < 0) y = -b;
            if (Height - t < y) y = Height - t;
            if (bAnimation) {
                if ((ma & CanvasMoveArgs.Left) == CanvasMoveArgs.Left)
                    m_real_canvas_x = x;
                if ((ma & CanvasMoveArgs.Top) == CanvasMoveArgs.Top)
                    m_real_canvas_y = y;
            } else {
                m_real_canvas_x = _CanvasOffsetX = x;
                m_real_canvas_y = _CanvasOffsetY = y;
            }
            OnCanvasMoved(EventArgs.Empty);
        }
        /// <summary>
        /// 缩放画布
        /// 当不存在 Node 时候 无法缩放
        /// </summary>
        /// <param name="f">缩放比例</param>
        /// <param name="x">缩放中心X位于控件上的坐标</param>
        /// <param name="y">缩放中心Y位于控件上的坐标</param>
        public void ScaleCanvas(float f, float x, float y) {
            if (_Nodes.Count == 0) {
                _CanvasScale = 1F;
                return;
            }
            if (_CanvasScale == f) return;
            if (f < 0.5) f = 0.5f; else if (f > 3) f = 3;
            float x_c = ControlToCanvas(x, true);
            float y_c = ControlToCanvas(y, false);
            _CanvasScale = f;
            _CanvasOffsetX = m_real_canvas_x -= CanvasToControl(x_c, true) - x;
            _CanvasOffsetY = m_real_canvas_y -= CanvasToControl(y_c, false) - y;
            OnCanvasScaled(EventArgs.Empty);
            Invalidate();
        }
        /// <summary>
        /// 获取当前已连接的 Option 对应关系
        /// </summary>
        /// <returns>连接信息集合</returns>
        public ConnectionInfo[] GetConnectionInfo() {
            return m_dic_gp_info.Values.ToArray();
        }
        /// <summary>
        /// 判断两个 Node 之间是否存在连接路径
        /// </summary>
        /// <param name="nodeStart">起始 Node</param>
        /// <param name="nodeFind">目标 Node</param>
        /// <returns>若存在路径返回true 否则false</returns>
        public static bool CanFindNodePath(STNode nodeStart, STNode nodeFind) {
            HashSet<STNode> hs = new HashSet<STNode>();
            return STNodeEditor.CanFindNodePath(nodeStart, nodeFind, hs);
        }
        private static bool CanFindNodePath(STNode nodeStart, STNode nodeFind, HashSet<STNode> hs) {
            foreach (STNodeOption op_1 in nodeStart.OutputOptions) {
                foreach (STNodeOption op_2 in op_1.ConnectedOption) {
                    if (op_2.Owner == nodeFind) return true;
                    if (hs.Add(op_2.Owner)) {
                        if (STNodeEditor.CanFindNodePath(op_2.Owner, nodeFind)) return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 获取画布中指定矩形区域图像
        /// </summary>
        /// <param name="rect">画布中指定的矩形区域</param>
        /// <returns>图像</returns>
        public Image GetCanvasImage(Rectangle rect) { return GetCanvasImage(rect, 1f); }
        /// <summary>
        /// 获取画布中指定矩形区域图像
        /// </summary>
        /// <param name="rect">画布中指定的矩形区域</param>
        /// <param name="fScale">缩放比例</param>
        /// <returns>图像</returns>
        public Image GetCanvasImage(Rectangle rect, float fScale) {
            if (fScale < 0.5) fScale = 0.5f; else if (fScale > 3) fScale = 3;
            using (var bitmap = new SKBitmap((int)(rect.Width * fScale), (int)(rect.Height * fScale), true))
            using (var canvas = new SKCanvas(bitmap)) {
                canvas.Clear(SkiaDrawingHelper.ToSKColor(BackColor));
                m_canvas = canvas;
                m_drawing_tools.Canvas = canvas;

                if (fScale != 1f) canvas.Scale(fScale, fScale);
                if (_ShowGrid) OnDrawGrid(m_drawing_tools, rect.Width, rect.Height);
                canvas.Translate(-rect.X, -rect.Y);
                OnDrawNode(m_drawing_tools, rect);
                OnDrawConnectedLine(m_drawing_tools);
                canvas.ResetMatrix();

                if (_ShowLocation) OnDrawNodeOutLocation(m_drawing_tools, new Size(bitmap.Width, bitmap.Height), m_lst_node_out);
                using (var image = SKImage.FromBitmap(bitmap))
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                using (var ms = new MemoryStream(data.ToArray())) {
                    m_canvas = null;
                    return Image.FromStream(ms);
                }
            }
        }
        /// <summary>
        /// 保存画布中的类容到文件中
        /// </summary>
        /// <param name="strFileName">文件路径</param>
        public void SaveCanvas(string strFileName) {
            using (FileStream fs = new FileStream(strFileName, FileMode.Create, FileAccess.Write)) {
                SaveCanvas(fs);
            }
        }
        /// <summary>
        /// 保存画布中的类容到数据流
        /// </summary>
        /// <param name="s">数据流对象</param>
        public void SaveCanvas(Stream s) {
            Dictionary<STNodeOption, long> dic = new Dictionary<STNodeOption, long>();
            s.Write(new byte[] { (byte)'S', (byte)'T', (byte)'N', (byte)'D' }, 0, 4); //file head
            s.WriteByte(1);                                                           //ver
            using (GZipStream gs = new GZipStream(s, CompressionMode.Compress)) {
                gs.Write(BitConverter.GetBytes(_CanvasOffsetX), 0, 4);
                gs.Write(BitConverter.GetBytes(_CanvasOffsetY), 0, 4);
                gs.Write(BitConverter.GetBytes(_CanvasScale), 0, 4);
                gs.Write(BitConverter.GetBytes(_Nodes.Count), 0, 4);
                foreach (STNode node in _Nodes) {
                    try {
                        byte[] byNode = node.GetSaveData();
                        gs.Write(BitConverter.GetBytes(byNode.Length), 0, 4);
                        gs.Write(byNode, 0, byNode.Length);
                        foreach (STNodeOption op in node.InputOptions) if (!dic.ContainsKey(op)) dic.Add(op, dic.Count);
                        foreach (STNodeOption op in node.OutputOptions) if (!dic.ContainsKey(op)) dic.Add(op, dic.Count);
                    } catch (Exception ex) {
                        throw new Exception("获取节点数据出错-" + node.Title, ex);
                    }
                }
                gs.Write(BitConverter.GetBytes(m_dic_gp_info.Count), 0, 4);
                foreach (var v in m_dic_gp_info.Values)
                    gs.Write(BitConverter.GetBytes(((dic[v.Output] << 32) | dic[v.Input])), 0, 8);
            }
        }
        /// <summary>
        /// 获取画布中内容二进制数据
        /// </summary>
        /// <returns>二进制数据</returns>
        public byte[] GetCanvasData() {
            using (MemoryStream ms = new MemoryStream()) {
                SaveCanvas(ms);
                return ms.ToArray();
            }
        }
        /// <summary>
        /// 加载程序集
        /// </summary>
        /// <param name="strFiles">程序集集合</param>
        /// <returns>存在STNode类型的文件的个数</returns>
        public int LoadAssembly(string[] strFiles) {
            int nCount = 0;
            foreach (var v in strFiles) {
                try {
                    if (LoadAssembly(v)) nCount++;
                } catch { }
            }
            return nCount;
        }
        /// <summary>
        /// 加载程序集
        /// </summary>
        /// <param name="strFile">指定需要加载的文件</param>
        /// <returns>是否加载成功</returns>
        public bool LoadAssembly(string strFile) {
            bool bFound = false;
            Assembly asm = Assembly.LoadFrom(strFile);
            if (asm == null) return false;
            foreach (var t in asm.GetTypes()) {
                if (t.IsAbstract) continue;
                if (t == m_type_node || t.IsSubclassOf(m_type_node)) {
                    if (m_dic_type.ContainsKey(t.GUID.ToString())) continue;
                    m_dic_type.Add(t.GUID.ToString(), t);
                    bFound = true;
                }
            }
            return bFound;
        }
        /// <summary>
        /// 获取当前编辑器中已加载的Node类型
        /// </summary>
        /// <returns>类型集合</returns>
        public Type[] GetTypes() {
            return m_dic_type.Values.ToArray();
        }
        /// <summary>
        /// 从文件中加载数据
        /// 注意: 此方法并不会清空画布中数据 而是数据叠加
        /// </summary>
        /// <param name="strFileName">文件路径</param>
        public void LoadCanvas(string strFileName) {
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(strFileName)))
                LoadCanvas(ms);
        }
        /// <summary>
        /// 从二进制加载数据
        /// 注意: 此方法并不会清空画布中数据 而是数据叠加
        /// </summary>
        /// <param name="byData">二进制数据</param>
        public void LoadCanvas(byte[] byData) {
            using (MemoryStream ms = new MemoryStream(byData))
                LoadCanvas(ms);
        }
        /// <summary>
        /// 从数据流中加载数据
        /// 注意: 此方法并不会清空画布中数据 而是数据叠加
        /// </summary>
        /// <param name="s">数据流对象</param>
        public void LoadCanvas(Stream s) {
            int nLen = 0;
            byte[] byLen = new byte[4];
            s.Read(byLen, 0, 4);
            if (BitConverter.ToInt32(byLen, 0) != BitConverter.ToInt32(new byte[] { (byte)'S', (byte)'T', (byte)'N', (byte)'D' }, 0))
                throw new InvalidDataException("无法识别的文件类型");
            if (s.ReadByte() != 1) throw new InvalidDataException("无法识别的文件版本号");
            using (GZipStream gs = new GZipStream(s, CompressionMode.Decompress)) {
                gs.Read(byLen, 0, 4);
                float x = BitConverter.ToSingle(byLen, 0);
                gs.Read(byLen, 0, 4);
                float y = BitConverter.ToSingle(byLen, 0);
                gs.Read(byLen, 0, 4);
                float scale = BitConverter.ToSingle(byLen, 0);
                gs.Read(byLen, 0, 4);
                int nCount = BitConverter.ToInt32(byLen, 0);
                Dictionary<long, STNodeOption> dic = new Dictionary<long, STNodeOption>();
                HashSet<STNodeOption> hs = new HashSet<STNodeOption>();
                byte[] byData = null;
                for (int i = 0; i < nCount; i++) {
                    gs.Read(byLen, 0, byLen.Length);
                    nLen = BitConverter.ToInt32(byLen, 0);
                    byData = new byte[nLen];
                    gs.Read(byData, 0, byData.Length);
                    STNode node = null;
                    try { node = GetNodeFromData(byData); } catch (Exception ex) {
                        throw new Exception("加载节点时发生错误可能数据已损坏\r\n" + ex.Message, ex);
                    }
                    try { _Nodes.Add(node); } catch (Exception ex) {
                        throw new Exception("加载节点出错-" + node.Title, ex);
                    }
                    foreach (STNodeOption op in node.InputOptions) if (hs.Add(op)) dic.Add(dic.Count, op);
                    foreach (STNodeOption op in node.OutputOptions) if (hs.Add(op)) dic.Add(dic.Count, op);
                }
                gs.Read(byLen, 0, 4);
                nCount = BitConverter.ToInt32(byLen, 0);
                byData = new byte[8];
                for (int i = 0; i < nCount; i++) {
                    gs.Read(byData, 0, byData.Length);
                    long id = BitConverter.ToInt64(byData, 0);
                    long op_out = id >> 32;
                    long op_in = (int)id;
                    dic[op_out].ConnectOption(dic[op_in]);
                }
                ScaleCanvas(scale, 0, 0);
                MoveCanvas(x, y, false, CanvasMoveArgs.All);
            }
            BuildBounds();
            foreach (STNode node in _Nodes) node.OnEditorLoadCompleted();
        }

        private STNode GetNodeFromData(byte[] byData) {
            int nIndex = 0;
            string strModel = Encoding.UTF8.GetString(byData, nIndex + 1, byData[nIndex]);
            nIndex += byData[nIndex] + 1;
            string strGUID = Encoding.UTF8.GetString(byData, nIndex + 1, byData[nIndex]);
            nIndex += byData[nIndex] + 1;

            int nLen = 0;

            Dictionary<string, byte[]> dic = new Dictionary<string, byte[]>();
            while (nIndex < byData.Length) {
                nLen = BitConverter.ToInt32(byData, nIndex);
                nIndex += 4;
                string strKey = Encoding.UTF8.GetString(byData, nIndex, nLen);
                nIndex += nLen;
                nLen = BitConverter.ToInt32(byData, nIndex);
                nIndex += 4;
                byte[] byValue = new byte[nLen];
                Array.Copy(byData, nIndex, byValue, 0, nLen);
                nIndex += nLen;
                dic.Add(strKey, byValue);
            }
            if (!m_dic_type.ContainsKey(strGUID)) throw new TypeLoadException("无法找到类型 {" + strModel.Split('|')[1] + "} 所在程序集 确保程序集 {" + strModel.Split('|')[0] + "} 已被编辑器正确加载 可通过调用LoadAssembly()加载程序集");
            Type t = m_dic_type[strGUID]; ;
            STNode node = (STNode)Activator.CreateInstance(t);
            node.OnLoadNode(dic);
            return node;
        }
        /// <summary>
        /// 在画布中显示提示信息
        /// </summary>
        /// <param name="strText">要显示的信息</param>
        /// <param name="foreColor">信息前景色</param>
        /// <param name="backColor">信息背景色</param>
        public void ShowAlert(string strText, Color foreColor, Color backColor) {
            ShowAlert(strText, foreColor, backColor, 1000, AlertLocation.RightBottom, true);
        }
        /// <summary>
        /// 在画布中显示提示信息
        /// </summary>
        /// <param name="strText">要显示的信息</param>
        /// <param name="foreColor">信息前景色</param>
        /// <param name="backColor">信息背景色</param>
        /// <param name="al">信息要显示的位置</param>
        public void ShowAlert(string strText, Color foreColor, Color backColor, AlertLocation al) {
            ShowAlert(strText, foreColor, backColor, 1000, al, true);
        }
        /// <summary>
        /// 在画布中显示提示信息
        /// </summary>
        /// <param name="strText">要显示的信息</param>
        /// <param name="foreColor">信息前景色</param>
        /// <param name="backColor">信息背景色</param>
        /// <param name="nTime">信息持续时间</param>
        /// <param name="al">信息要显示的位置</param>
        /// <param name="bRedraw">是否立即重绘</param>
        public void ShowAlert(string strText, Color foreColor, Color backColor, int nTime, AlertLocation al, bool bRedraw) {
            m_str_alert = strText;
            m_forecolor_alert = foreColor;
            m_backcolor_alert = backColor;
            m_time_alert = nTime;
            m_dt_alert = DateTime.Now;
            m_alpha_alert = 255;
            m_al = al;
            if (bRedraw) Invalidate();
        }
        /// <summary>
        /// 设置画布中活动的节点
        /// </summary>
        /// <param name="node">需要被设置为活动的节点</param>
        /// <returns>设置前的活动节点</returns>
        public STNode SetActiveNode(STNode node) {
            if (node != null && !_Nodes.Contains(node)) return _ActiveNode;
            STNode ret = _ActiveNode;
            if (_ActiveNode != node) {         //重置活动选择节点
                if (node != null) {
                    _Nodes.MoveToEnd(node);
                    node.IsActive = true;
                    node.SetSelected(true, false);
                    node.OnGotFocus(EventArgs.Empty);
                }
                if (_ActiveNode != null) {
                    _ActiveNode.IsActive /*= this._ActiveNode.IsSelected*/ = false;
                    _ActiveNode.OnLostFocus(EventArgs.Empty);
                }
                _ActiveNode = node;
                Invalidate();
                OnActiveChanged(EventArgs.Empty);
                //this.OnSelectedChanged(EventArgs.Empty);
            }
            return ret;
        }
        /// <summary>
        /// 向画布中添加一个被选中的节点
        /// </summary>
        /// <param name="node">需要被选中的节点</param>
        /// <returns>是否添加成功</returns>
        public bool AddSelectedNode(STNode node) {
            if (!_Nodes.Contains(node)) return false;
            bool b = !node.IsSelected;
            node.IsSelected = true;
            lock (m_hs_node_selected) return m_hs_node_selected.Add(node) || b;
        }
        /// <summary>
        /// 向画布中移除一个被选中的节点
        /// </summary>
        /// <param name="node">需要被移除的节点</param>
        /// <returns>是移除否成功</returns>
        public bool RemoveSelectedNode(STNode node) {
            if (!_Nodes.Contains(node)) return false;
            bool b = node.IsSelected;
            node.IsSelected = false;
            lock (m_hs_node_selected) return m_hs_node_selected.Remove(node) || b;
        }
        /// <summary>
        /// 向编辑器中添加默认数据类型颜色
        /// </summary>
        /// <param name="t">数据类型</param>
        /// <param name="clr">对应颜色</param>
        /// <returns>被设置后的颜色</returns>
        public Color SetTypeColor(Type t, Color clr) {
            return SetTypeColor(t, clr, false);
        }
        /// <summary>
        /// 向编辑器中添加默认数据类型颜色
        /// </summary>
        /// <param name="t">数据类型</param>
        /// <param name="clr">对应颜色</param>
        /// <param name="bReplace">若已经存在是否替换颜色</param>
        /// <returns>被设置后的颜色</returns>
        public Color SetTypeColor(Type t, Color clr, bool bReplace) {
            if (_TypeColor.ContainsKey(t)) {
                if (bReplace) _TypeColor[t] = clr;
            } else {
                _TypeColor.Add(t, clr);
            }
            return _TypeColor[t];
        }

        #endregion public
    }
}
