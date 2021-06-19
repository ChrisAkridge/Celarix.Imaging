using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Celarix.Imaging.Tiling
{
	public sealed class Tiler
	{
        public static Image<TPixel> Tile<TPixel>(TileOptions tileOptions,
            IEnumerable<Image<TPixel>> inputImages,
            int imageCount,
            CancellationToken cancellationToken,
            IProgress<int> progress) where TPixel : unmanaged, IPixel<TPixel>
        {
            var images = inputImages;
            var imagesOnCanvas = Utilities.GetSizeFromCount(imageCount);
            var canvas = CreateCanvas<TPixel>(imageCount, new Size(tileOptions.TileWidth, tileOptions.TileHeight));
            var (aspectWidth, aspectHeight) = GetAspectRatio(tileOptions.TileWidth, tileOptions.TileHeight);

            var x = 0;
            var y = 0;
            var widthInImages = imagesOnCanvas.Width;

            foreach (var image in images)
            {
                var cropRect = GetImageCropRect(image.Width,
                    image.Height, aspectWidth,
                    aspectHeight);
                var cropped = CropImage(image, cropRect);
                var resized = ResizeImage(cropped, new Size(tileOptions.TileWidth, tileOptions.TileHeight));
                OverlayImage(canvas, resized, x, y, new Size(tileOptions.TileWidth, tileOptions.TileHeight));

                if (x < widthInImages - 1) { x++; }
                else
                {
                    x = 0;
                    y++;
                }

                if (cancellationToken.IsCancellationRequested) { throw new TaskCanceledException(); }

                progress?.Report((y * widthInImages) + x);
            }

            return canvas;
        }

        private static Rectangle GetImageCropRect(int imageWidth, int imageHeight, int tileAspectWidth, int tileAspectHeight)
        {
            var (imageAspectWidth, imageAspectHeight) = GetAspectRatio(imageWidth, imageHeight);

            if (imageAspectWidth == tileAspectWidth && imageAspectHeight == tileAspectHeight)
            {
                return new Rectangle(0, 0, imageWidth, imageHeight);
            }

            var oneByAspect = (double)tileAspectWidth / tileAspectHeight;

            var (centerX, centerY) = GetImageCenter(new Size(imageWidth, imageHeight));

            var cropSizeBasedOnHeight = new Size((int)(imageHeight * oneByAspect), imageHeight);
            var cropSizeBasedOnWidth = new Size(imageWidth, (int)(imageWidth / oneByAspect));
            var (width, height) = WillSizeFit(cropSizeBasedOnHeight, new Size(imageWidth, imageHeight))
                ? cropSizeBasedOnHeight
                : cropSizeBasedOnWidth;
            var (x, y) = new Point(centerX - (width / 2), centerY - (height / 2));
            return new Rectangle(x, y, width, height);
        }

        private static Image CropImage(Image original, Rectangle cropRect) => original.Clone(i => i.Crop(cropRect));

        private static Image ResizeImage(Image image, Size newSize) => image.Clone(i => i.Resize(newSize));

        private static Point GetImageCenter(Size imageSize) => new Point(imageSize.Width / 2, imageSize.Height / 2);

        private static Size GetCanvasSize(int imageCount, Size imageSize)
        {
            var (tilesAcross, tilesDown) = Utilities.GetSizeFromCount(imageCount);
            var (width, height) = imageSize;
            return new Size(tilesAcross * width, tilesDown * height);
        }

        private static Image<TPixel> CreateCanvas<TPixel>(int imageCount, Size tileSize) where TPixel : unmanaged, IPixel<TPixel>
        {
            var (canvasWidth, canvasHeight) = GetCanvasSize(imageCount, tileSize);
            return new Image<TPixel>(canvasWidth, canvasHeight, Color.White.ToPixel<TPixel>());
        }

        private static void OverlayImage(Image canvas, Image image, int x, int y, Size imageSize)
        {
            var (width, height) = imageSize;
            var imageOverlayPosition = new Point(x * width, y * height);
            canvas.Mutate(c => c.DrawImage(image, imageOverlayPosition, 1f));
        }

        private static bool WillSizeFit(Size a, Size b) => a.Width <= b.Width && a.Height <= b.Height;

        private static Tuple<int, int> GetAspectRatio(int width, int height)
        {
            var gcd = GetGCD(height, width);
            var a = height / gcd;
            var b = width / gcd;
            return new Tuple<int, int>(b, a);
        }

        // http://stackoverflow.com/a/2565188/2709212
        private static int GetGCD(int a, int b)
        {
            while (true)
            {
                if (b == 0)
                {
                    return a;
                }

                var c = a;
                a = b;
                b = c % b;
            }
        }
    }
}
