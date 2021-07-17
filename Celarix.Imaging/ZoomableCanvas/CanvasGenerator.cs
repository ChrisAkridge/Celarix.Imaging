using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Celarix.Imaging.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Celarix.Imaging.ZoomableCanvas
{
    public static class CanvasGenerator
    {
        private static readonly ImageCache cache = new ImageCache(2);
        private const string exceptionsFilePath = "exceptions.txt";
        
        public static void Generate(string imageFilePath,
            Size cellSize,
            string outputFolderPath,
            CancellationToken cancellationToken,
            IProgress<string> progress,
            bool createHigherZoomLevels = true)
        {
            var imageSize = Utilities.GetImageSize(imageFilePath);
            Generate(new List<PositionedImage>
                {
                    new PositionedImage
                    {
                        ImageFilePath = imageFilePath,
                        Position = Point.Empty,
                        Size = imageSize
                    }
                },
                cellSize,
                outputFolderPath,
                cancellationToken,
                progress,
                createHigherZoomLevels);
        }

        public static void Generate(IEnumerable<PositionedImage> images,
            Size cellSize, 
            string outputFolderPath,
            CancellationToken cancellationToken,
            IProgress<string> progress,
            bool createHigherZoomLevels = true)
        {
            var level0Tiles = BuildTileMap(images, cellSize);
            int tilesComplete = 0;
            int totalTiles = level0Tiles.Count;
            Directory.CreateDirectory(Path.Combine(outputFolderPath, "0"));

            foreach (var (position, tileImages) in level0Tiles)
            {
                SaveLevel0CellImage(position, cellSize, tileImages, outputFolderPath, (double)tilesComplete / totalTiles, progress);
                if (cancellationToken.IsCancellationRequested) { throw new TaskCanceledException(); }

                tilesComplete++;
            }

            var currentZoomLevel = 0;

            if (!createHigherZoomLevels) { return; }

            string folderToCombine;
            do
            {
                folderToCombine = Path.Combine(outputFolderPath, $"{currentZoomLevel}");
                if (cancellationToken.IsCancellationRequested) { throw new TaskCanceledException(); }
            } while (
                ZoomLevelGenerator.TryCombineImagesForNextZoomLevel(cellSize, folderToCombine, outputFolderPath, ++currentZoomLevel, progress));
        }

        private static Dictionary<Point, List<PositionedImage>> BuildTileMap(IEnumerable<PositionedImage> images,
            Size cellSize)
        {
            var level0Tiles = new Dictionary<Point, List<PositionedImage>>();

            foreach (var image in images)
            {
                var corners = new List<Point>
                {
                    new Point(image.Position.X, image.Position.Y),
                    new Point((image.Position.X + image.Size.Width) - 1, image.Position.Y),
                    new Point(image.Position.X, (image.Position.Y + image.Size.Height) - 1),
                    new Point((image.Position.X + image.Size.Width) - 1, (image.Position.Y + image.Size.Height) - 1)
                };

                var (cellWidth, cellHeight) = cellSize;
                var cells = corners.Select(corner => new Point(corner.X / cellWidth, corner.Y / cellHeight)).ToList();
                var minCellX = cells.Min(c => c.X);
                var minCellY = cells.Min(c => c.Y);
                var maxCellX = cells.Max(c => c.X);
                var maxCellY = cells.Max(c => c.Y);

                for (var y = minCellY; y <= maxCellY; y++)
                {
                    for (var x = minCellX; x <= maxCellX; x++)
                    {
                        var cell = new Point(x, y);
                        if (!level0Tiles.ContainsKey(cell)) { level0Tiles.Add(cell, new List<PositionedImage>()); }

                        var (cellOriginX, cellOriginY) = new Point(cell.X * cellWidth, cell.Y * cellHeight);
                        var positionInCell =
                            new Point(image.Position.X - cellOriginX, image.Position.Y - cellOriginY);

                        level0Tiles[cell]
                            .Add(new PositionedImage
                            {
                                ImageFilePath = image.ImageFilePath,
                                Position = positionInCell,
                                Size = image.Size
                            });
                    }
                }
            }

            return level0Tiles;
        }

        private static void SaveLevel0CellImage(Point cellNumber,
            Size cellSize,
            IEnumerable<PositionedImage> images,
            string outputFolderPath,
            double percentCompleted,
            IProgress<string> progress)
        {     
            var (cellNumberX, cellNumberY) = cellNumber;
            var (cellWidth, cellHeight) = cellSize;
            var outputFilePath = Path.Combine(outputFolderPath, "0", $"{cellNumberY}", $"{cellNumberX}.png");
            var cellImage = new Image<Rgba32>(cellWidth, cellHeight);

            Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath));

            if (File.Exists(outputFilePath))
            {
                progress?.Report($"Saved tile ({cellNumberX}, {cellNumberY}), {percentCompleted * 100d:F2}% complete");

                return;
            }

            foreach (var image in images)
            {
                try
                {
                    var imageToDraw = cache.Load(image.ImageFilePath);
                    cellImage.Mutate(ci => ci.DrawImage(imageToDraw, image.Position, 1f));
                }
                catch (Exception ex)
                {
                    File.WriteAllText(Path.Combine(outputFolderPath, $"exception_{cellNumberX}_{cellNumberY}.txt"), Utilities.FormatException(ex));
                }
            }

            cellImage.SaveAsPng(outputFilePath);
            cellImage.Dispose();

            progress?.Report($"Saved tile ({cellNumberX}, {cellNumberY}), {percentCompleted * 100d:F2}% complete");
        }

        internal static void SaveLevel0CellImage(Point cellNumber,
            Image<Rgba32> image,
            string outputFolderPath)
        {
            var (cellNumberX, cellNumberY) = cellNumber;
            var outputFilePath = Path.Combine(outputFolderPath, "0", $"{cellNumberY}", $"{cellNumberX}.png");

            Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath));

            if (File.Exists(outputFilePath))
            {
                return;
            }
            
            image.SaveAsPng(outputFilePath);
            image.Dispose();
        }
    }
}
