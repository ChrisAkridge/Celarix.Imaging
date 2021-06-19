using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Celarix.Imaging.JobRecovery;
using Celarix.Imaging.Progress;
using Celarix.Imaging.ZoomableCanvas;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Celarix.Imaging.Packing
{
	public static class ImagePacker
	{
        public static void Pack(PackingOptions options,
            CancellationToken cancellationToken,
            IProgress<string> progress)
        {
            var job = new PackingJob { Options = options };
            JobManager.SaveJobFile(JobSources.Packer, job);
            
            PackImplementation(job, cancellationToken, progress);
        }

        public static void ResumePack(CancellationToken cancellationToken,
            IProgress<string> progress)
        {
            JobManager.TryLoadLatestJobFromFile<PackingJob>(JobSources.Packer, out PackingJob job);

            PackImplementation(job, cancellationToken, progress);
        }

        private static void PackImplementation(PackingJob job,
            CancellationToken cancellationToken,
            IProgress<string> progress)
        {
            if (job.ImagesAndSizes == null)
            {
                var files = GetFiles(job.Options.InputFolderPath, job.Options.Recursive);
                job.ImagesAndSizes = GetImageSizes(files, progress, cancellationToken);
                JobManager.SaveJobFile(JobSources.Packer, job);
            }

            job.Blocks ??= job.ImagesAndSizes
                .OrderByDescending(kvp => kvp.Value.Width)
                .Select(kvp => new Block { ImageFilePath = kvp.Key, Size = kvp.Value })
                .ToList();

            var packer = new Packer();
            packer.Fit(job.Blocks, progress);

            if (!job.Options.Multipicture)
            {
                DrawImage(job.Blocks.Where(b => b.Fit != null).ToList(),
                    packer.Root.Size,
                    job.Options.OutputPath,
                    cancellationToken,
                    progress);
            }
            else
            {
                var images = job.Blocks
                    .Where(b => b.Fit != null)
                    .Select(b => new PositionedImage
                    {
                        ImageFilePath = b.ImageFilePath, Position = b.Fit.Location, Size = b.Size
                    });

                CanvasGenerator.Generate(images, new Size(256, 256), job.Options.OutputPath, cancellationToken,
                    progress);
            }
            
            JobManager.CompleteJob(JobSources.Packer);
        }

        private static IEnumerable<string> GetFiles(string inputFolderPath, bool recursive)
        {
            if (!Directory.Exists(inputFolderPath))
            {
                throw new DirectoryNotFoundException($"Invalid path {inputFolderPath}: path does not exist");
            }

            var files = Directory.EnumerateFiles(inputFolderPath,
                "*",
                new EnumerationOptions
                {
                    RecurseSubdirectories = recursive,
                    IgnoreInaccessible = true
                });
            var filteredFiles = files.Where(f => f.EndsWith("gif", StringComparison.InvariantCultureIgnoreCase)
                || f.EndsWith("jpg", StringComparison.InvariantCultureIgnoreCase)
                || f.EndsWith("jpeg", StringComparison.InvariantCultureIgnoreCase)
                || f.EndsWith("png", StringComparison.InvariantCultureIgnoreCase));

            return filteredFiles;
        }

        private static Dictionary<string, Size> GetImageSizes(IEnumerable<string> imageFilePaths,
            IProgress<string> progress,
            CancellationToken cancellationToken)
        {
            var filesAndSizes = new Dictionary<string, Size>();

            foreach (var file in imageFilePaths)
            {
                try
                {
                    var size = GetImageSize(file);
                    filesAndSizes.Add(file, size);

                    if (filesAndSizes.Count % 100 == 0)
                    {
                        progress.Report($"Loaded sizes for {filesAndSizes.Count} files...");
                    }
                }
                catch { continue; }

                if (cancellationToken.IsCancellationRequested) { throw new TaskCanceledException(); }
            }

            return filesAndSizes;
        }
        
        private static void DrawImage(IList<Block> blocks,
            Size rootSize,
            string outputFilePath,
            CancellationToken cancellationToken,
            IProgress<string> progress)
        {
            var (width, height) = rootSize;
            var canvas = new Image<Rgba32>(width, height, Color.White);

			for (var i = 0; i < blocks.Count; i++)
            {
				var block = blocks[i];
				using var image = Image.Load(block.ImageFilePath);
                canvas.Mutate(c => c.DrawImage(image, block.Fit.Location, 1f));

                if (cancellationToken.IsCancellationRequested) { throw new TaskCanceledException(); }
                progress?.Report($"Packed {i + 1} of {blocks.Count} images");
            }

            canvas.SaveAsPng(outputFilePath);
        }

        private static Size GetImageSize(string imageFilePath)
        {
            if (ImageSizeLoader.TryGetSize(imageFilePath, out Size size))
            {
                if (size.Width < 1048576 && size.Height < 1048576)
                {
                    return size;
                }
            }

            using var image = Image.Load(imageFilePath);
            var (width, height) = image.Size();

            if (width < 1048576 && height < 1048576)
            {
                return image.Size();
            }
            else
            {
                return new Size(1);
            }
        }
    }
}
