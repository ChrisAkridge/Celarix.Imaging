using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Celarix.Imaging.ImagingPlayground.Rendering
{
    public sealed class InfiniteCanvasControl : SKGLControl
    {
        // Camera state
        private SKPoint _translation = SKPoint.Empty;
        private float _zoomScale = 1.0f;

        private bool _showInfoPanel = false;
        private SKPoint _hoverPosition = SKPoint.Empty;

        // Image management
        private ImageCache _imageCache;

        // Animation
        private readonly System.Windows.Forms.Timer _animationTimer = new System.Windows.Forms.Timer();

        // Dragging
        private SKPoint? _dragStartPoint = null;
        private SKPoint? _dragEndPoint = null;
        private SKPoint? _dragVelocity = null;
        private SKPoint? _dragInerita = null;
        private bool _isDragging = false;

        // Zooming
        private const float MaxZoomScale = 64f; // Pixels can be no larger than 64x64
        private const float MaxZoomVelocity = 1f;
        private const float ZoomDampening = 0.9f;
        private float _zoomVelocity = 0f;

        // Context menu
        private ContextMenuStrip _contextMenu = new ContextMenuStrip();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        private Viewport Viewport
        {
            get
            {
                var controlSize = new SKSize(Width, Height);
                var viewportLocation = new SKPoint(-_translation.X / _zoomScale, -_translation.Y / _zoomScale);
                var viewportSize = new SKSize(controlSize.Width / _zoomScale, controlSize.Height / _zoomScale);
                return new Viewport(viewportLocation, viewportSize);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowInfoPanel
        {
            get => _showInfoPanel;
            set
            {
                if (_showInfoPanel != value)
                {
                    _showInfoPanel = value;
                    Invalidate(); // Redraw to show/hide info panel
                }
            }
        }

        public InfiniteCanvasControl()
        {
            SetStyle(ControlStyles.Opaque, true);
            MouseMove += InfiniteCanvasControl_MouseMove;

            // Animation setup
            _animationTimer = new System.Windows.Forms.Timer
            {
                Interval = 33, // ~30 FPS
                Enabled = true // temporary for debugging
            };
            _animationTimer.Tick += OnAnimationFrame;

            // Dragging setup
            MouseDown += InfiniteCanvasControl_MouseDown;
            MouseUp += InfiniteCanvasControl_MouseUp;

            // Zooming setup
            MouseWheel += InfiniteCanvasControl_MouseWheel;

            // Context menu
            var resetTranslationMenuItem = new ToolStripMenuItem("Reset Translation");
            resetTranslationMenuItem.Click += (s, e) => _translation = SKPoint.Empty;
            _contextMenu.Items.Add(resetTranslationMenuItem);
            var resetZoomMenuItem = new ToolStripMenuItem("Reset Zoom");
            resetZoomMenuItem.Click += (s, e) => _zoomScale = 1.0f;
            _contextMenu.Items.Add(resetZoomMenuItem);
            var separator = new ToolStripSeparator();
            _contextMenu.Items.Add(separator);
            var resetMenuItem = new ToolStripMenuItem("Reset All");
            resetMenuItem.Click += (s, e) =>
            {
                _translation = SKPoint.Empty;
                _zoomScale = 1.0f;
            };
            _contextMenu.Items.Add(resetMenuItem);
            ContextMenuStrip = _contextMenu;

            _imageCache = new ImageCache(this);
        }

        // Load images
        public void LoadSingleImage(string filePath)
        {
            _imageCache.Clear();
            var canvasImage = CanvasImage.FromFile(filePath, SKPoint.Empty);
            _imageCache.Add(canvasImage);
            _imageCache.UpdateVisibility(Viewport);
        }

        public void LoadZoomableCanvas(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                // TODO: log something here
                return;
            }

            var zoomLevelFolders = Directory.GetDirectories(folderPath, "*", SearchOption.TopDirectoryOnly)
                .Where(f => int.TryParse(f, out _))
                .OrderBy(f => f)
                .ToArray();
            if (zoomLevelFolders.Length == 0)
            {
                // TODO: log something here
                return;
            }

            var canvasImages = new List<CanvasImage>();
            foreach (var zoomLevelFolder in zoomLevelFolders)
            {
                var zoomLevel = int.Parse(Path.GetFileName(zoomLevelFolder));
                var tileEdgeLengthAtLevel = LibraryConfiguration.Instance.ZoomableCanvasTileEdgeLength * (1 << zoomLevel);
                var yFolders = Directory.GetDirectories(zoomLevelFolder, "*", SearchOption.TopDirectoryOnly)
                    .Where(f => int.TryParse(f, out _))
                    .OrderBy(f => f)
                    .ToArray();
                for (int y = 0; y < yFolders.Length; y++)
                {
                    var xFiles = Directory.GetFiles(yFolders[y], "*", SearchOption.TopDirectoryOnly)
                        .Where(f => int.TryParse(Path.GetFileNameWithoutExtension(f), out _))
                        .OrderBy(f => f)
                        .ToArray();
                    var yPos = y * tileEdgeLengthAtLevel;
                    for (int x = 0; x < xFiles.Length; x++)
                    {
                        var xPos = x * tileEdgeLengthAtLevel;
                        var canvasImage = CanvasImage.FromFile(xFiles[x], new SKPoint(xPos, yPos), zoomLevel);
                        canvasImages.Add(canvasImage);
                    }
                }
            }

            _imageCache.Clear();
            foreach (var canvasImage in canvasImages)
            {
                _imageCache.Add(canvasImage);
            }
            _imageCache.UpdateVisibility(Viewport);
        }

        public void LoadBandedTempImages(string folderPath, BandDirection direction)
        {
            if (!Directory.Exists(folderPath))
            {
                // TODO: log something here
                return;
            }

            var files = Directory.GetFiles(folderPath, "*", SearchOption.TopDirectoryOnly)
                .Where(f =>
                {
                    // File name format is {imageIndex}_{width}_{height}.png
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(f);
                    var parts = fileNameWithoutExtension.Split('_');
                    return parts.Length == 3
                        && int.TryParse(parts[0], out _)
                        && int.TryParse(parts[1], out _)
                        && int.TryParse(parts[2], out _);
                })
                .OrderBy(f => f)
                .ToArray();
            if (files.Length == 0)
            {
                // TODO: log something here
                return;
            }

            var canvasImages = new List<CanvasImage>();
            var currentLocation = SKPoint.Empty;
            foreach (var image in files)
            {
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(image);
                var parts = fileNameWithoutExtension.Split("_");
                var width = int.Parse(parts[0]);
                var height = int.Parse(parts[1]);

                var canvasImage = CanvasImage.FromFile(image, currentLocation);
                canvasImages.Add(canvasImage);
                if (direction == BandDirection.Horizontal)
                {
                    currentLocation.X += width;
                }
                else
                {
                    currentLocation.Y += height;
                }
            }

            _imageCache.Clear();
            foreach (var canvasImage in canvasImages)
            {
                _imageCache.Add(canvasImage);
            }
            _imageCache.UpdateVisibility(Viewport);
        }

        // Event handlers
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

            var camera = SKMatrix.CreateScaleTranslation(_zoomScale, _zoomScale, _translation.X, _translation.Y);
            canvas.SetMatrix(camera);

            // Draw stuff here
            var visibleImages = _imageCache.Cull(Viewport);
            foreach (var image in visibleImages)
            {
                if (image.SKImage == null) { continue; }
                canvas.DrawImage(image.SKImage, image.Position);
            }

            canvas.ResetMatrix();

            // Draw info panel if enabled

            // Debugging
            var text = $"Zoom: {_zoomScale:F2}";
            using var paint = new SKPaint
            {
                Color = SKColors.Green,
                IsAntialias = true
            };
            var font = new SKFont(SKTypeface.Default, 16);
            canvas.DrawText(text, new SKPoint(32, 32), SKTextAlign.Left, font, paint);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (DesignMode)
            {
                e.Graphics.Clear(Color.WhiteSmoke);
                return;
            }
            base.OnPaint(e);
        }

        private void OnAnimationFrame(object? sender, EventArgs e)
        {
            const float Dampening = 0.75f;

            Invalidate();

            if (_dragInerita.HasValue)
            {
                _translation += _dragInerita.Value;
                _dragInerita = new SKPoint(_dragInerita.Value.X * Dampening, _dragInerita.Value.Y * Dampening); // Dampen inertia
                if (_dragInerita.Value.Length < 0.1f)
                {
                    _dragInerita = null; // Stop inertia when velocity is low
                }
            }
        }

        private void InfiniteCanvasControl_MouseMove(object? sender, MouseEventArgs e)
        {
            var lastHoverPosition = _hoverPosition;
            _hoverPosition = new SKPoint(e.X, e.Y);
            if (_isDragging && _dragStartPoint.HasValue)
            {
                var currentPoint = new SKPoint(e.X, e.Y);
                _dragVelocity = currentPoint - _dragStartPoint.Value;
                _translation += _dragVelocity.Value;
                _dragStartPoint = currentPoint; // Update for smooth dragging
                Invalidate();
            }
        }

        private void InfiniteCanvasControl_MouseUp(object? sender, MouseEventArgs e)
        {
            _isDragging = false;
            _dragEndPoint = new SKPoint(e.X, e.Y);
            _dragInerita = _dragVelocity; // Start inertia with the last velocity
        }

        private void InfiniteCanvasControl_MouseDown(object? sender, MouseEventArgs e)
        {
            _isDragging = true;
            _dragInerita = null; // Stop any ongoing inertia
            _dragStartPoint = new SKPoint(e.X, e.Y);
            _dragEndPoint = null;
        }

        private void InfiniteCanvasControl_MouseWheel(object? sender, MouseEventArgs e)
        {
            var zoomAmount = e.Delta > 0 ? 1.1f : 0.9f;

            // Compute new zoom scale
            var newZoomScale = _zoomScale * zoomAmount;

            // Clamp zoom scale
            const float MinZoomScale = 0.01f;
            newZoomScale = Math.Clamp(newZoomScale, MinZoomScale, MaxZoomScale);

            // Calculate zoom focal point in world coordinates
            var mousePosition = new SKPoint(e.X, e.Y);
            var focalPointBeforeZoom = Divide(mousePosition - _translation, _zoomScale);
            // Apply zoom
            _zoomScale = newZoomScale;
            // Calculate new translation to keep focal point under the mouse
            var focalPointAfterZoom = Multiply(focalPointBeforeZoom, _zoomScale) + _translation;
            _translation += mousePosition - focalPointAfterZoom;
        }

        // Miscellaneous
        public void SetMaxMemoryBytes(long maxBytes)
        {
            _imageCache.MaxMemoryBytes = maxBytes;
        }

        private static SKPoint Multiply(SKPoint p1, float scale)
        {
            return new SKPoint(p1.X * scale, p1.Y * scale);
        }

        private static SKPoint Divide(SKPoint point, float divisor)
        {
            return new SKPoint(point.X / divisor, point.Y / divisor);
        }
    }
}
