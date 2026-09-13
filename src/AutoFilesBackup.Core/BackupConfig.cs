using System;

namespace AutoFilesBackup.Core
{
    [Serializable]
    public class BackupConfig
    {
        public string SourceFolder { get; set; } = "";
        public string BackupDrive1 { get; set; } = @"E:\";
        public string BackupDrive2 { get; set; } = @"F:\";
        public int MaxIntervalDays { get; set; } = 7;
        public string DailyBackupTime { get; set; } = "03:00";
        public string ActiveDriveLetter { get; set; } = "E";
        public DateTime? LastFullBackupDate { get; set; } = null;
        public DateTime? LastBackupRunDate { get; set; } = null;
        public double BufferMarginGb { get; set; } = 5.0;
        public int LogRetentionDays { get; set; } = 120;
    }
}
