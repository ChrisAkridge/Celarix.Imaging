using System;
using System.Drawing;
using SkiaSharp;

namespace ST.Library.UI.NodeEditor {
    public static class SkiaDrawingHelper {
        public static SKColor ToSKColor(Color color) {
            return new SKColor(color.R, color.G, color.B, color.A);
        }

        public static SKRect ToSKRect(Rectangle rect) {
            return new SKRect(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }

        public static SKBitmap ToSKBitmap(Image image) {
            if (image == null) return null;
            using (var ms = new System.IO.MemoryStream()) {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;
                return SKBitmap.Decode(ms);
            }
        }

        public static void RenderToCanvas(SKCanvas canvas, Action<SKCanvas> renderAction) {
            if (canvas == null || renderAction == null) return;
            renderAction(canvas);
        }
    }
}
