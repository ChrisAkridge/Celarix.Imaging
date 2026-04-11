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
