using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Celarix.Imaging.Utilities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
// ReSharper disable AccessToDisposedClosure

namespace Celarix.Imaging.ZoomableCanvas
{
	public static class ZoomLevelGenerator
	{
        public static bool TryCombineImagesForNextZoomLevel(string inputFolderPath,
            string outputFolderPath,
            int nextZoomLevel,
            IProgress<string> progress)
        {
            var cellSize = new Size(LibraryConfiguration.Instance.ZoomableCanvasTileEdgeLength);
            var paddingImage = new Image<Rgba32>(cellSize.Width, cellSize.Height, Rgba32.ParseHex("ffffffff"));
            var files = LoadFilesFromZoomLevel(inputFolderPath, progress);
            if (files.Count <= 1) { return false; }
            
            Directory.CreateDirectory(Path.Combine(outputFolderPath, $"{nextZoomLevel}"));

            var levelWidth = files.Max(kvp => kvp.Key.X) + 1;
            var levelHeight = files.Max(kvp => kvp.Key.Y) + 1;

            for (var y = 0; y < levelHeight; y += 2)
            {
                progress?.Report($"Beginning row {y}...");
                
                Directory.CreateDirectory(Path.Combine(outputFolderPath, $"{nextZoomLevel}", $"{y / 2}"));
                for (var x = 0; x < levelWidth; x += 2)
                {
                    try
                    {
                        var outputFilePath = Path.Combine(outputFolderPath, $"{nextZoomLevel}", $"{y / 2}", $"{x / 2}.png");

                        if (File.Exists(outputFilePath))
                        {
                            progress?.Report($"Saved level {nextZoomLevel} tile ({x / 2}, {y / 2})");

                            continue;
                        }

                        var topLeftCell = new Point(x, y);
                        var topRightCell = new Point(x + 1, y);
                        var bottomLeftCell = new Point(x, y + 1);
                        var bottomRightCell = new Point(x + 1, y + 1);
                        
                        var topLeftExists = files.ContainsKey(topLeftCell);
                        var topRightExists = files.ContainsKey(topRightCell);
                        var bottomLeftExists = files.ContainsKey(bottomLeftCell);
                        var bottomRightExists = files.ContainsKey(bottomRightCell);

                        if (!topLeftExists && !topRightExists && !bottomLeftExists && !bottomRightExists)
                        {
                            continue;
                        }

                        var topLeft = topLeftExists ? Image.Load(files[topLeftCell]) : paddingImage;
                        var topRight = topRightExists ? Image.Load(files[topRightCell]) : paddingImage;
                        var bottomLeft = bottomLeftExists ? Image.Load(files[bottomLeftCell]) : paddingImage;
                        var bottomRight = bottomRightExists ? Image.Load(files[bottomRightCell]) : paddingImage;

                        var canvas = new Image<Rgba32>(cellSize.Width * 2, cellSize.Height * 2, Rgba32.ParseHex("ffffffff"));
                        canvas.Mutate(c => DrawImagesOnCell(c, cellSize, topLeft, topRight, bottomLeft, bottomRight));
                        canvas.SaveAsPng(outputFilePath);

                        if (topLeft != paddingImage) { topLeft.Dispose(); }
                        if (topRight != paddingImage) { topRight.Dispose(); }
                        if (bottomLeft != paddingImage) { bottomLeft.Dispose(); }
                        if (bottomRight != paddingImage) { bottomRight.Dispose(); }
                        canvas.Dispose();

                        progress?.Report($"Saved level {nextZoomLevel} tile ({x / 2}, {y / 2}). Bounds: {Math.Ceiling(levelWidth / 2d)} by {Math.Ceiling(levelHeight / 2d)} tiles.");
                    }
                    catch (Exception ex)
                    {
                        File.WriteAllText(Path.Combine(outputFolderPath, $"exception_{x}_{y}.txt"),
                            Helpers.FormatException(ex));
                    }
                }
            }

            return true;
        }

        private static Dictionary<Point, string> LoadFilesFromZoomLevel(string inputFolderPath,
            IProgress<string> logger)
        {
            var foundFileCount = 0;
            var files = new Dictionary<Point, string>();

            foreach (var file in Directory.EnumerateFiles(inputFolderPath, "*.png", SearchOption.AllDirectories))
            {
                foundFileCount += 1;

                if (foundFileCount % 10000 == 0) { logger?.Report($"Found {foundFileCount} files in {inputFolderPath}, most recent is {file}"); }
                
                var xTileNumber = Path.GetFileNameWithoutExtension(file);
                var yTileNumber = Path.GetFileName(Path.GetDirectoryName(file));
                files.Add(new Point(int.Parse(xTileNumber), int.Parse(yTileNumber!)), file);
            }

            return files;
        }

        private static void DrawImagesOnCell(IImageProcessingContext c, Size cellSize, Image topLeft, Image topRight,
            Image bottomLeft, Image bottomRight)
        {
            c.DrawImage(topLeft, Point.Empty, 1f);
            c.DrawImage(topRight, new Point(cellSize.Width, 0), 1f);
            c.DrawImage(bottomLeft, new Point(0, cellSize.Height), 1f);
            c.DrawImage(bottomRight, new Point(cellSize.Width, cellSize.Height), 1f);
            c.Resize(cellSize);
        }
    }
}
