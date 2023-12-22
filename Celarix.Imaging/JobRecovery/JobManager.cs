using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Celarix.Imaging.Packing;

namespace Celarix.Imaging.JobRecovery
{
    public static class JobManager
    {
        public static void SaveJobFile(string jobSource, IBinaryJob job)
        {
            CompleteJob(jobSource);
            string fileName = $"{jobSource}_{DateTimeOffset.Now:yyyyMMddHHmmss}.json";
            string filePath = Path.Combine(GetApplicationJobFolder(), fileName);

            using var writer = new BinaryWriter(File.OpenWrite(filePath), Encoding.UTF8);
            job.Save(writer);
        }

        public static void CompleteJob(string jobSource)
        {
            var latestJobFilePath = GetLatestJobFilePath(jobSource);
            if (latestJobFilePath != null) { File.Delete(latestJobFilePath); }
        }

        public static bool TryLoadLatestJobFromFile<T>(string jobSource, out T job) where T : class
        {
            var latestJobFilePath = GetLatestJobFilePath(jobSource);

            if (latestJobFilePath == null)
            {
                job = null;

                return false;
            }

            if (JobFileIsVersion1(latestJobFilePath))
            {
                var jobJson = File.ReadAllText(latestJobFilePath, Encoding.UTF8);
                job = JsonSerializer.Deserialize<T>(jobJson, new JsonSerializerOptions
                {
                    MaxDepth = 1048576
                });

                return true;
            }

            var reader = new BinaryReader(File.OpenRead(latestJobFilePath), Encoding.UTF8);
            job = IBinaryJob.Load<T>(reader);
            return true;
        }

        public static string GetLatestJobFilePath(string jobSource)
        {
            CreateAppDataFolderIfNonExistent();
            return Directory
                .GetFiles(GetApplicationJobFolder(), "*.json", SearchOption.TopDirectoryOnly)
                .Where(f => f.Contains(jobSource, StringComparison.InvariantCultureIgnoreCase))
                .OrderBy(f => f)
                .LastOrDefault();
        }
        
        private static void CreateAppDataFolderIfNonExistent()
        {
            // https://stackoverflow.com/a/16500150/2709212
            var jobFolder = GetApplicationJobFolder();
            Directory.CreateDirectory(jobFolder);
        }

        private static string GetApplicationJobFolder()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(appData, "Celarix.Imaging");
        }

        private static bool JobFileIsVersion1(string jobFilePath)
        {
            using var streamReader = new StreamReader(jobFilePath, Encoding.UTF8);

            return (char)streamReader.Read() == '{';
        }
    }
}
