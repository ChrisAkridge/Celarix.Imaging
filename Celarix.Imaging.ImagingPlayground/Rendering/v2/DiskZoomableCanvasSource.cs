using Celarix.Imaging.Utilities;
using Serilog;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground.Rendering.v2
{
    internal sealed class DiskZoomableCanvasSource : IZoomableCanvasSource
    {
        private readonly string _zoomableCanvasFolderPath;
        private readonly Image<Rgba32>? _blankTile;

        public string Name { get; }

        public System.Drawing.Size TilePixelSize { get; private set; }

        public System.Drawing.Size Level0TileCount { get; private set; }

        public DiskZoomableCanvasSource(string zoomableCanvasFolderPath)
        {
            FolderValidOrThrow(zoomableCanvasFolderPath);

            _zoomableCanvasFolderPath = zoomableCanvasFolderPath;
            Name = Path.GetFileName(Path.TrimEndingDirectorySeparator(zoomableCanvasFolderPath));
            var presumedTileFiles = Directory.EnumerateFiles(zoomableCanvasFolderPath, "*.png", SearchOption.AllDirectories);
            string? firstTileFilePath = null;
            foreach (var tileFile in presumedTileFiles)
            {
                var fileName = Path.GetFileNameWithoutExtension(tileFile);
                var folderPath = Path.GetDirectoryName(tileFile)
                    ?? throw new ArgumentException($"The folder at {zoomableCanvasFolderPath} is not a zoomable canvas; it contains a tile file at {tileFile} that is not within a zoom level folder.");
                var relativeFolderPath = Path.GetRelativePath(zoomableCanvasFolderPath, folderPath);
                var folderNames = relativeFolderPath.Split(Path.DirectorySeparatorChar);

                if (folderNames.All(n => int.TryParse(n, out _)) && int.TryParse(fileName, out _))
                {
                    firstTileFilePath = tileFile;
                    break;
                }
            }

            if (firstTileFilePath == null)
            {
                throw new ArgumentException($"The folder at {zoomableCanvasFolderPath} is not a zoomable canvas; it contains no tile files within zoom level folders.");
            }

            Image<Rgba32>? tile = null;
            try
            {
                tile = SixLabors.ImageSharp.Image.Load<Rgba32>(firstTileFilePath);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"The folder at {zoomableCanvasFolderPath} contains a tile file at {firstTileFilePath} that could not be loaded as an image.", ex);
            }
            if (tile == null)
            {
                throw new ArgumentException($"The folder at {zoomableCanvasFolderPath} contains a tile file at {firstTileFilePath} that could not be loaded as an image.");
            }

            TilePixelSize = new System.Drawing.Size(tile.Width, tile.Height);
            Level0TileCount = ComputeLevel0TileCount(zoomableCanvasFolderPath);

            Log.Debug("Created DiskZoomableCanvasSource with name {Name} at path {Path} with {TilePixelSize} tiles and {Level0TileCount} tiles at level 0",
                Name, zoomableCanvasFolderPath, TilePixelSize, Level0TileCount);

            _blankTile = new Image<Rgba32>(TilePixelSize.Width, TilePixelSize.Height, SixLabors.ImageSharp.Color.White);
            tile?.Dispose();
        }

        public async Task<SKImage> LoadTileAsync(FactoryOptions options)
        {
            if (options is not ZoomableCanvasFactoryOptions canvasOptions)
            {
                throw new ArgumentException(nameof(options));
            }

            Log.Debug("Loading tile from disk: {Options}", options.ToString());

            var tileFilePath = Path.Combine(
                _zoomableCanvasFolderPath,
                canvasOptions.ZoomLevel.ToString(),
                canvasOptions.TileY.ToString(),
                $"{canvasOptions.TileX}.png");
            if (!File.Exists(tileFilePath))
            {
                return await Helpers.CreateSkImageFromImageSharp(_blankTile!, options.CancellationToken);
            }

            var loadResult = await ImageLoader.LoadImage(tileFilePath, options.CancellationToken);
            if (loadResult.Result == ImageLoadAttemptResult.Success && loadResult.LoadedImage != null)
            {
                SKImage sKImage = await Helpers.CreateSkImageFromImageSharp(loadResult.LoadedImage, options.CancellationToken);
                loadResult.LoadedImage.Dispose();
                return sKImage;
            }
            else
            {
                throw loadResult.Exception ?? new Exception($"Failed to load tile image at {tileFilePath} for unknown reasons.");
            }
        }

        private static void FolderValidOrThrow(string folderPath)
        {
            var topLevelFolders = Directory.GetDirectories(folderPath, "*", SearchOption.TopDirectoryOnly);
            if (topLevelFolders.Length == 0)
            {
                throw new ArgumentException($"The folder at {folderPath} is not a zoomable canvas; it has no folders within it.");
            }

            var topLevelFolderNames = topLevelFolders.Select(f => Path.GetFileName(f)).ToArray();
            if (!topLevelFolderNames.Any(n => int.TryParse(n, out _)))
            {
                throw new ArgumentException($"The folder at {folderPath} is not a zoomable canvas; at least one folder must be named with an integer representing the zoom level.");
            }

            var zoomLevels = ParseOnlyNumbers(topLevelFolderNames)
                .OrderBy(n => n)
                .ToArray();
            for (var i = 0; i < zoomLevels.Length; i++)
            {
                var level = zoomLevels[i];
                if (level != i)
                {
                    throw new ArgumentException($"The folder at {folderPath} is not a zoomable canvas; the zoom level folders are {string.Join(", ", zoomLevels.Select(l => l.ToString()))}, which is missing {i}.");
                }
            }
        }

        private static System.Drawing.Size ComputeLevel0TileCount(string zoomableCanvasFolderPath)
        {
            var level0FolderPath = Path.Combine(zoomableCanvasFolderPath, "0");
            if (!Directory.Exists(level0FolderPath))
            {
                throw new ArgumentException($"The folder at {zoomableCanvasFolderPath} is not a zoomable canvas; it has no zoom level 0 folder.");
            }

            var level0TileStripes = Directory
                .GetDirectories(level0FolderPath, "*", SearchOption.TopDirectoryOnly);
            var maxX = 0;
            var maxY = 0;
            foreach (var stripe in level0TileStripes)
            {
                var folderName = Path.GetFileName(stripe);
                if (!int.TryParse(folderName, out var y))
                {
                    continue;
                }

                if (y > maxY)
                {
                    maxY = y;
                }

                var tileFilePaths = ParseOnlyNumbers(Directory
                    .GetFiles(stripe, "*.png", SearchOption.TopDirectoryOnly)
                    .Select(p => Path.GetFileNameWithoutExtension(p)));
                var maxXInStripe = tileFilePaths.Any() ? tileFilePaths.Max() : -1;
                if (maxXInStripe > maxX)
                {
                    maxX = maxXInStripe;
                }
            }

            return new System.Drawing.Size(maxX + 1, maxY + 1);
        }

        private static IEnumerable<int> ParseOnlyNumbers(IEnumerable<string> values)
        {
            ArgumentNullException.ThrowIfNull(values);

            foreach (var value in values)
            {
                if (int.TryParse(value, out var number))
                {
                    yield return number;
                }
            }
        }
    }
}
