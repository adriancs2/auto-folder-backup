using System;
using System.IO;

namespace AutoFilesBackup.Core
{
    public static class WmiDriveService
    {
        public static string NormalizeDriveLetter(string drivePath)
        {
            if (string.IsNullOrWhiteSpace(drivePath)) return string.Empty;
            string value = drivePath.Trim().ToUpperInvariant();
            if (value.Length == 1 && value[0] >= 'A' && value[0] <= 'Z') return value;
            if ((value.Length == 2 && value[1] == ':') ||
                (value.Length == 3 && value[1] == ':' && (value[2] == '\\' || value[2] == '/')))
            {
                if (value[0] >= 'A' && value[0] <= 'Z') return value.Substring(0, 1);
            }
            throw new ArgumentException("Expected a drive letter, for example L:\\; received: " + drivePath);
        }
        public static bool DriveExists(string letter)
        {
            letter = NormalizeDriveLetter(letter);
            if (letter.Length == 0) return false;
            try { return new DriveInfo(letter + @":\").IsReady; }
            catch (IOException) { return false; }
            catch (UnauthorizedAccessException) { return false; }
        }
        public static long GetFreeSpaceBytes(string letter)
        {
            return new DriveInfo(NormalizeDriveLetter(letter) + @":\").AvailableFreeSpace;
        }
        public static long GetTotalSpaceBytes(string letter)
        {
            return new DriveInfo(NormalizeDriveLetter(letter) + @":\").TotalSize;
        }
        // Whole-volume formatting was replaced by ownership-checked backup-set recycling.
        [Obsolete("Use managed backup-set recycling. Whole-volume formatting is disabled.")]
        public static bool FormatDriveWmi(string driveLetter, string volumeLabel = "Backup", LogManager logger = null)
        {
            throw new NotSupportedException("Whole-volume formatting is disabled. Only owned old backup sets may be recycled.");
        }
    }
}
