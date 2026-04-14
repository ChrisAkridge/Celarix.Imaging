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

            _imageCache = new ImageCache(this);
        }

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

        private void InfiniteCanvasControl_MouseMove(object? sender, MouseEventArgs e)
        {
            _hoverPosition = new SKPoint(e.X, e.Y);
            if (_showInfoPanel)
            {
                Invalidate(); // Redraw to update info panel with new hover position
            }
        }
    }
}
