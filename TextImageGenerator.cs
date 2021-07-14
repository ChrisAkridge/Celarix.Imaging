using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using LongFile = Pri.LongPath.File;
using LongFileInfo = Pri.LongPath.FileInfo;
using LongPath = Pri.LongPath.Path;
using LongDirectoryInfo = Pri.LongPath.DirectoryInfo;
using LongDirectory = Pri.LongPath.Directory;

namespace Celarix.IO.FileAnalysis.Analysis
{
    internal static class TextImageGenerator
    {
        private const int ImageWidth = 768;
        private const int ImageHeight = 768;
        private const int TileSize = 256;
        private const int ImageWidthInTiles = ImageWidth / TileSize;
        private const int ImageHeightInTiles = ImageHeight / TileSize;
        private const string TextImageFolderPath = "pageImages\\";

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly Font font = SystemFonts.CreateFont("Consolas", 12f);

        public static void SaveImageForTextFolder(string filePath)
        {
            logger.Info($"Generating images for pages generated for {filePath}...");
            
            var pageFilesPath = Utilities.Utilities.GetTextFilePagesFolderPath(filePath);
            var pageFilePaths = LongDirectory
                .GetFileSystemEntries(pageFilesPath, "*.txt", SearchOption.TopDirectoryOnly)
                .OrderBy(path => path)
                .ToList();
            var totalPages = pageFilePaths.Count;
            var canvasWidthInPages = (int)Math.Floor(Math.Sqrt(totalPages));
            var canvasHeightInPages = canvasWidthInPages;

            // 1000 iq math right here, boys
            while (canvasWidthInPages * canvasHeightInPages < totalPages)
            {
                canvasHeightInPages += 1;
            }
            
            logger.Info($"Canvas will be {canvasWidthInPages} by {canvasHeightInPages} pages, or {canvasWidthInPages * ImageWidthInTiles} by {canvasHeightInPages * ImageHeightInTiles} tiles");

            var pageTilesPath = LongPath.Combine(LongPath.GetDirectoryName(filePath),
                LongPath.GetFileNameWithoutExtension(filePath) + "_ext",
                TextImageFolderPath,
                "0");
            new LongDirectoryInfo(pageTilesPath).Create();

            for (var i = 0; i < pageFilePaths.Count; i++)
            {
                logger.Info($"Saving page {i + 1}");
                var pageFilePath = pageFilePaths[i];
                var pageIndices = GetPageIndices(i, canvasWidthInPages);
                
                using var image = CreateBlankImage();
                DrawPageOnImage(LongFile.ReadAllText(pageFilePath), image);

                for (int y = 0; y < ImageHeightInTiles; y++)
                {
                    var tileYOffset = y + (pageIndices.Y * ImageHeightInTiles);
                    var rowImagePath = LongPath.Combine(pageTilesPath,
                        tileYOffset.ToString());
                    new LongDirectoryInfo(rowImagePath).Create();

                    for (int x = 0; x < ImageWidthInTiles; x++)
                    {
                        var tileXOffset = x + (pageIndices.X * ImageWidthInTiles);
                        var tilePositionOnImage = new Point(-x * TileSize, -y * TileSize);
                        var tilePath = LongPath.Combine(rowImagePath, $"{tileXOffset}.png");
                        using var tile = CreateBlankTile();
                        tile.Mutate(t => t.DrawImage(image, tilePositionOnImage, 1f));
                        tile.SaveAsPng(tilePath);
                    }
                }
            }
            
            Utilities.Utilities.DrawZoomLevelsForLevel0CanvasTiles(pageTilesPath);
        }

        private static Image<Rgb24> CreateBlankImage() =>
            new Image<Rgb24>(ImageWidth, ImageHeight, Color.White);

        private static Image<Rgb24> CreateBlankTile() =>
            new Image<Rgb24>(TileSize, TileSize);

        private static void DrawPageOnImage(string pageText, Image<Rgb24> image)
        {
            image.Mutate(ctx => ctx.DrawPolygon(Color.Black, 1f, PointF.Empty,
                new PointF(ImageWidth - 1f, 0f),
                new PointF(ImageWidth - 1f, ImageHeight - 1f),
                new PointF(0f, ImageHeight - 1f),
                PointF.Empty));
            image.Mutate(ctx => ctx.DrawText(pageText, font, Color.Black, PointF.Empty));
        }

        private static Point GetPageIndices(int pageNumber, int canvasWidthInPages) =>
            new Point(pageNumber % canvasWidthInPages,
                pageNumber / canvasWidthInPages);
    }
}
