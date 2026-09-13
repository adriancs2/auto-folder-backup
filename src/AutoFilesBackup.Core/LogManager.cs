using System;
using System.IO;
using System.Text;

namespace AutoFilesBackup.Core
{
    public class LogManager
    {
        private readonly string _logRootDir;
        private readonly string _todayFolder;
        private readonly object _lockObj = new object();

        public string EventsLogPath { get; }
        public string StatisticLogPath { get; }
        public string SuccessFilesLogPath { get; }
        public string FailedFilesLogPath { get; }
        public string SkippedFilesLogPath { get; }

        public LogManager(string baseDir = null, int retentionDays = 120)
        {
            if (string.IsNullOrEmpty(baseDir))
            {
                baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            }
            _logRootDir = baseDir;
            _todayFolder = Path.Combine(_logRootDir, DateTime.Now.ToString("yyyy-MM-dd"));

            if (!Directory.Exists(_todayFolder))
            {
                Directory.CreateDirectory(_todayFolder);
            }

            EventsLogPath = Path.Combine(_todayFolder, "events.log");
            StatisticLogPath = Path.Combine(_todayFolder, "statistic.log");
            SuccessFilesLogPath = Path.Combine(_todayFolder, "success_files.log");
            FailedFilesLogPath = Path.Combine(_todayFolder, "failed_files.log");
            SkippedFilesLogPath = Path.Combine(_todayFolder, "skipped_files.log");

            PruneOldLogs(retentionDays);
        }

        public void LogEvent(string message, string details = "")
        {
            lock (_lockObj)
            {
                string line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}";
                if (!string.IsNullOrEmpty(details))
                {
                    line += $" | Details: {details}";
                }
                File.AppendAllText(EventsLogPath, line + Environment.NewLine, Encoding.UTF8);
            }
        }

        public void LogStatistic(long totalFiles, long totalSuccess, long totalFailed, long totalSkipped, 
            long totalBackupBytes, long totalIncrementalBytes, TimeSpan duration, string backupType, string driveLetter)
        {
            lock (_lockObj)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"--- Backup Run Statistics [{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ---");
                sb.AppendLine($"Backup Type           : {backupType}");
                sb.AppendLine($"Target Drive          : {driveLetter}");
                sb.AppendLine($"Duration              : {duration}");
                sb.AppendLine($"Total Files Processed : {totalFiles}");
                sb.AppendLine($"Total Success         : {totalSuccess}");
                sb.AppendLine($"Total Failed          : {totalFailed}");
                sb.AppendLine($"Total Skipped         : {totalSkipped}");
                sb.AppendLine($"Total Full Size       : {totalBackupBytes:N0} bytes ({totalBackupBytes / (1024.0 * 1024.0):N2} MB)");
                sb.AppendLine($"Total Incremental Size: {totalIncrementalBytes:N0} bytes ({totalIncrementalBytes / (1024.0 * 1024.0):N2} MB)");
                sb.AppendLine(new string('=', 60));
                File.AppendAllText(StatisticLogPath, sb.ToString(), Encoding.UTF8);
            }
        }

        public void LogSuccessFile(string originalFilePath)
        {
            lock (_lockObj)
            {
                File.AppendAllText(SuccessFilesLogPath, $"[{DateTime.Now:HH:mm:ss}] {originalFilePath}{Environment.NewLine}", Encoding.UTF8);
            }
        }

        public void LogFailedFile(string originalFilePath, string errorMessage)
        {
            lock (_lockObj)
            {
                File.AppendAllText(FailedFilesLogPath, $"[{DateTime.Now:HH:mm:ss}] {originalFilePath} | Error: {errorMessage}{Environment.NewLine}", Encoding.UTF8);
            }
        }

        public void LogSkippedFile(string originalFilePath, string reason = "Archive attribute clear")
        {
            lock (_lockObj)
            {
                File.AppendAllText(SkippedFilesLogPath, $"[{DateTime.Now:HH:mm:ss}] {originalFilePath} | Reason: {reason}{Environment.NewLine}", Encoding.UTF8);
            }
        }

        private void PruneOldLogs(int retentionDays)
        {
            try
            {
                if (!Directory.Exists(_logRootDir)) return;
                var subDirs = Directory.GetDirectories(_logRootDir);
                DateTime cutoffDate = DateTime.Today.AddDays(-retentionDays);

                foreach (var dir in subDirs)
                {
                    string dirName = Path.GetFileName(dir);
                    if (DateTime.TryParse(dirName, out DateTime dirDate))
                    {
                        if (dirDate < cutoffDate)
                        {
                            try
                            {
                                Directory.Delete(dir, true);
                            }
                            catch { }
                        }
                    }
                }
            }
            catch { }
        }
    }
}
