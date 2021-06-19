using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Celarix.Imaging.JobRecovery
{
    public static class JobManager
    {
        public static void SaveJobFile(string jobSource, object job)
        {
            CompleteJob(jobSource);
            string fileName = $"{jobSource}_{DateTimeOffset.Now:yyyyMMddHHmmss}.json";
            string filePath = Path.Combine(GetApplicationJobFolder(), fileName);
            string jobJson = JsonSerializer.Serialize(job, new JsonSerializerOptions { MaxDepth = 1048576 });
            File.WriteAllText(filePath, jobJson, Encoding.UTF8);
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

            var jobJson = File.ReadAllText(latestJobFilePath, Encoding.UTF8);
            job = JsonSerializer.Deserialize<T>(jobJson, new JsonSerializerOptions { MaxDepth = 1048576 });

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
    }
}
