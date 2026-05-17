using Celarix.Imaging.BinaryDrawing.v2;
using RenderingV2 = Celarix.Imaging.ImagingPlayground.Rendering.v2;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.ComponentModel;
using System.Drawing;

namespace Celarix.Imaging.ImagingPlayground.Rendering
{
    public sealed class InfiniteCanvasControl : SKGLControl
    {
        private const float MinZoomScale = 1f / 64f;
        private const float MaxZoomScale = 64f;
        private const float InertiaDampening = 0.75f;

        private SKPoint translation = SKPoint.Empty;
        private float zoomScale = 1f;
        private bool showInfoPanel;
        private SKPoint hoverPosition = SKPoint.Empty;

        private RenderingV2.IImageCache? imageCache;
        private List<RenderingV2.ImageEntry> visibleImageEntries = new();

        private readonly System.Windows.Forms.Timer animationTimer;
        private SKPoint? dragStartPoint;
        private SKPoint? dragVelocity;
        private SKPoint? dragInertia;
        private bool isDragging;

        private readonly ContextMenuStrip contextMenu;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowInfoPanel
        {
            get => showInfoPanel;
            set
            {
                if (showInfoPanel == value)
                {
                    return;
                }

                showInfoPanel = value;
                Invalidate();
            }
        }

        public InfiniteCanvasControl()
        {
            SetStyle(ControlStyles.Opaque, true);

            animationTimer = new System.Windows.Forms.Timer
            {
                Interval = 33,
                Enabled = true
            };
            animationTimer.Tick += OnAnimationFrame;

            MouseMove += InfiniteCanvasControl_MouseMove;
            MouseDown += InfiniteCanvasControl_MouseDown;
            MouseUp += InfiniteCanvasControl_MouseUp;
            MouseWheel += InfiniteCanvasControl_MouseWheel;

            contextMenu = new ContextMenuStrip();
            var resetTranslationMenuItem = new ToolStripMenuItem("Reset Translation");
            resetTranslationMenuItem.Click += (_, _) =>
            {
                translation = SKPoint.Empty;
                NotifyViewportChanged();
            };
            contextMenu.Items.Add(resetTranslationMenuItem);

            var resetZoomMenuItem = new ToolStripMenuItem("Reset Zoom");
            resetZoomMenuItem.Click += (_, _) =>
            {
                zoomScale = 1f;
                NotifyViewportChanged();
            };
            contextMenu.Items.Add(resetZoomMenuItem);

            contextMenu.Items.Add(new ToolStripSeparator());

            var resetAllMenuItem = new ToolStripMenuItem("Reset All");
            resetAllMenuItem.Click += (_, _) => ResetView();
            contextMenu.Items.Add(resetAllMenuItem);
            ContextMenuStrip = contextMenu;
        }

        public void LoadSingleImage(string filePath)
        {
            var canvasImage = RenderingV2.CanvasImage.FromFile(filePath, 0, 0);
            var cache = new RenderingV2.SingleImageCache(this, ClientRectangle, canvasImage);
            SetImageCache(cache);
            ResetView();
        }

        public void LoadInMemoryImage(Image<Rgba32> image)
        {
            ArgumentNullException.ThrowIfNull(image);

            var canvasImage = RenderingV2.CanvasImage.FromImageSharpImage(image, 0, 0);
            var cache = new RenderingV2.SingleImageCache(this, ClientRectangle, canvasImage);
            SetImageCache(cache);
            ResetView();
        }

        public void LoadStripedImages(IReadOnlyList<System.Drawing.Rectangle> stripeRects,
            Func<RenderingV2.FactoryOptions, int, Task<SKImage>> stripeFactory)
        {
            ArgumentNullException.ThrowIfNull(stripeRects);
            ArgumentNullException.ThrowIfNull(stripeFactory);

            var cache = new RenderingV2.StripedImageCache(this, ClientRectangle, stripeRects, stripeFactory);
            SetImageCache(cache);
            ResetView();
        }

        public void LoadZoomableCanvas(RenderingV2.IZoomableCanvasSource source)
        {
            ArgumentNullException.ThrowIfNull(source);

            var cache = new RenderingV2.ZoomableCanvasImageCache(this, ClientRectangle, source);
            SetImageCache(cache);
            ResetView();
        }

        public void LoadBandedTempImages(string folderPath, BandDirection direction)
        {
            if (!Directory.Exists(folderPath))
            {
                return;
            }

            var files = Directory.GetFiles(folderPath, "*", SearchOption.TopDirectoryOnly)
                .Select(filePath =>
                {
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                    var parts = fileNameWithoutExtension.Split('_');
                    if (parts.Length != 3
                        || !int.TryParse(parts[0], out var imageIndex)
                        || !int.TryParse(parts[1], out _)
                        || !int.TryParse(parts[2], out _))
                    {
                        return null;
                    }

                    return new { FilePath = filePath, ImageIndex = imageIndex };
                })
                .Where(x => x != null)
                .OrderBy(x => x!.ImageIndex)
                .Select(x => x!.FilePath)
                .ToArray();
            if (files.Length == 0)
            {
                return;
            }

            var stripeRects = new List<System.Drawing.Rectangle>(files.Length);
            var stripePaths = new List<string>(files.Length);
            var currentX = 0;
            var currentY = 0;

            foreach (var file in files)
            {
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
                var parts = fileNameWithoutExtension.Split('_');
                var width = int.Parse(parts[1]);
                var height = int.Parse(parts[2]);
                stripeRects.Add(new System.Drawing.Rectangle(currentX, currentY, width, height));
                stripePaths.Add(file);

                if (direction == BandDirection.Horizontal)
                {
                    currentX += width;
                }
                else
                {
                    currentY += height;
                }
            }

            LoadStripedImages(stripeRects, (options, stripeIndex) =>
                RenderingV2.CanvasImage.FromFile(stripePaths[stripeIndex],
                    stripeRects[stripeIndex].X,
                    stripeRects[stripeIndex].Y).Factory(options));
        }

        public void SetSoftMemoryLimit(long bytes)
        {
            imageCache?.SetSoftMemoryLimit(bytes);
        }

        public void SetHardMemoryLimit(long bytes)
        {
            imageCache?.SetHardMemoryLimit(bytes);
        }

        protected override void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);
            var canvas = e.Surface.Canvas;

            if (DesignMode)
            {
                canvas.Clear(SKColors.WhiteSmoke);
                return;
            }

            canvas.Clear(SKColors.White);

            foreach (var imageEntry in visibleImageEntries)
            {
                if (imageEntry.Image == null)
                {
                    continue;
                }

                var canvasRectangle = new System.Drawing.Rectangle(imageEntry.CanvasImage.CanvasX,
                    imageEntry.CanvasImage.CanvasY,
                    imageEntry.CanvasImage.Width,
                    imageEntry.CanvasImage.Height);
                var controlRectangle = imageCache?.ControlRectangleForCanvasRectangle(canvasRectangle) ?? System.Drawing.Rectangle.Empty;
                if (controlRectangle.IsEmpty)
                {
                    continue;
                }

                var destination = new SKRect(controlRectangle.Left,
                    controlRectangle.Top,
                    controlRectangle.Right,
                    controlRectangle.Bottom);
                canvas.DrawImage(imageEntry.Image, destination);
            }

            using var paint = new SKPaint
            {
                Color = SKColors.Green,
                IsAntialias = true
            };
            var font = new SKFont(SKTypeface.Default, 16);
            var debugZoomLevel = MathF.Log2(1f / zoomScale);
            var debugText = $"Scale: {zoomScale:F3}  ZoomLevel: {debugZoomLevel:F2}  Visible: {visibleImageEntries.Count}";
            canvas.DrawText(debugText, new SKPoint(32, 32), SKTextAlign.Left, font, paint);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (DesignMode)
            {
                e.Graphics.Clear(System.Drawing.Color.WhiteSmoke);
                return;
            }

            base.OnPaint(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            NotifyViewportChanged();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                animationTimer.Tick -= OnAnimationFrame;
                animationTimer.Dispose();

                MouseMove -= InfiniteCanvasControl_MouseMove;
                MouseDown -= InfiniteCanvasControl_MouseDown;
                MouseUp -= InfiniteCanvasControl_MouseUp;
                MouseWheel -= InfiniteCanvasControl_MouseWheel;

                if (imageCache != null)
                {
                    imageCache.VisibleSetChanged -= ImageCache_VisibleSetChanged;
                    imageCache.Dispose();
                    imageCache = null;
                }

                contextMenu.Dispose();
            }

            base.Dispose(disposing);
        }

        private void SetImageCache(RenderingV2.IImageCache newImageCache)
        {
            ArgumentNullException.ThrowIfNull(newImageCache);

            if (imageCache != null)
            {
                imageCache.VisibleSetChanged -= ImageCache_VisibleSetChanged;
                imageCache.Dispose();
            }

            imageCache = newImageCache;
            imageCache.VisibleSetChanged += ImageCache_VisibleSetChanged;
            visibleImageEntries = imageCache.VisibleSet.ToList();
            NotifyViewportChanged();
        }

        private void ResetView()
        {
            dragInertia = null;
            dragVelocity = null;
            dragStartPoint = null;
            isDragging = false;
            translation = SKPoint.Empty;
            zoomScale = 1f;
            NotifyViewportChanged();
        }

        private void NotifyViewportChanged()
        {
            if (imageCache != null && Width > 0 && Height > 0)
            {
                imageCache.SetViewportControlCoordinates(ClientRectangle);
                imageCache.ViewportChanged(GetViewportCanvasCoordinates());
            }

            Invalidate();
        }

        private System.Drawing.Rectangle GetViewportCanvasCoordinates()
        {
            var canvasLeft = (int)MathF.Floor(-translation.X / zoomScale);
            var canvasTop = (int)MathF.Floor(-translation.Y / zoomScale);
            var canvasWidth = Math.Max(1, (int)MathF.Ceiling(Width / zoomScale));
            var canvasHeight = Math.Max(1, (int)MathF.Ceiling(Height / zoomScale));
            return new System.Drawing.Rectangle(canvasLeft, canvasTop, canvasWidth, canvasHeight);
        }

        private void OnAnimationFrame(object? sender, EventArgs e)
        {
            if (!dragInertia.HasValue)
            {
                return;
            }

            translation += dragInertia.Value;
            dragInertia = new SKPoint(dragInertia.Value.X * InertiaDampening,
                dragInertia.Value.Y * InertiaDampening);
            if (dragInertia.Value.Length < 0.1f)
            {
                dragInertia = null;
            }

            NotifyViewportChanged();
        }

        private void ImageCache_VisibleSetChanged(object? sender, EventArgs e)
        {
            visibleImageEntries = imageCache?.VisibleSet.ToList() ?? new List<RenderingV2.ImageEntry>();
            Invalidate();
        }

        private void InfiniteCanvasControl_MouseMove(object? sender, MouseEventArgs e)
        {
            hoverPosition = new SKPoint(e.X, e.Y);
            if (!isDragging || !dragStartPoint.HasValue)
            {
                return;
            }

            var currentPoint = new SKPoint(e.X, e.Y);
            dragVelocity = currentPoint - dragStartPoint.Value;
            translation += dragVelocity.Value;
            dragStartPoint = currentPoint;
            NotifyViewportChanged();
        }

        private void InfiniteCanvasControl_MouseUp(object? sender, MouseEventArgs e)
        {
            isDragging = false;
            dragInertia = dragVelocity;
        }

        private void InfiniteCanvasControl_MouseDown(object? sender, MouseEventArgs e)
        {
            isDragging = true;
            dragInertia = null;
            dragVelocity = null;
            dragStartPoint = new SKPoint(e.X, e.Y);
        }

        private void InfiniteCanvasControl_MouseWheel(object? sender, MouseEventArgs e)
        {
            var zoomAmount = e.Delta > 0 ? 1.1f : 0.9f;
            var newZoomScale = Math.Clamp(zoomScale * zoomAmount, MinZoomScale, MaxZoomScale);

            var mousePosition = new SKPoint(e.X, e.Y);
            var focalPointBeforeZoom = Divide(mousePosition - translation, zoomScale);
            zoomScale = newZoomScale;
            var focalPointAfterZoom = Multiply(focalPointBeforeZoom, zoomScale) + translation;
            translation += mousePosition - focalPointAfterZoom;
            NotifyViewportChanged();
        }

        private static SKPoint Multiply(SKPoint point, float scalar)
            => new(point.X * scalar, point.Y * scalar);

        private static SKPoint Divide(SKPoint point, float divisor)
            => new(point.X / divisor, point.Y / divisor);
    }
}
