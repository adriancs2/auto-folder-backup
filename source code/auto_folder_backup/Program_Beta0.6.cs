using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Data;
using System.Security.Cryptography;
using System.Management;
using System.Runtime.InteropServices;
using System.Diagnostics;


// Version: Beta 0.6

namespace System
{
    internal class Program_Beta06
    {
        static Settings config = null;

        static int TotalLogFiles = 300;

        static bool IsFullBackup;

        static string BasicLogFilePath;
        static string statLogPath;
        static string successLogPath;
        static string failLogPath;

        static DirectoryInfo SourceFolder = null;

        static long TotalIncrementBackupSize = 0L;
        static long TotalIncrementBackupBufferSize = 0L;
        static long TotalIncrementBackupAndBufferSize = 0L;

        static long TotalFullBackupSize = 0L;
        static long TotalFullBackupBufferSize = 0L;
        static long TotalFullBackupAndBufferSize = 0L;

        static long TotalOldArchiveSize = 0L;

        static long TotalDiskSpace = 0L;
        static long TotalDiskFreeSpace = 0L;
        static long RemainingFreeSpace = 0L;

        static long TotalDiskFreeSpaceAfterBackup = 0L;

        static long TotalArchiveFiles = 0L;
        static long TotalFiles = 0L;
        static long TotalSubFolders = 0L;
        static long TotalSuccess = 0L;
        static long TotalFailed = 0L;
        static long TotalSkipped = 0L;

        static List<string> AvailableDriveLetters = null;
        static List<DriveInfo> AvailableDrives = null;

        static void Main(string[] args)
        {
            try
            {
                DateTime timeBeforeGettingSize = DateTime.Now;

                ReadConfigInitializeLogFolder();

                GetAvailableDestinationDrives();

                WriteLog("Acquiring backup size...");

                // Calculate the total size of the target directory
                GetDirectorySize(SourceFolder);

                if (TotalFullBackupSize > 0L)
                {
                    // 20% buffer
                    TotalFullBackupBufferSize = (long)(TotalFullBackupSize * 0.2);

                    // Add 1 GB to the buffer
                    TotalFullBackupBufferSize += (long)(1024 * 1024 * 1024); // 1 GB in bytes
                }

                TotalFullBackupAndBufferSize = TotalFullBackupSize + TotalFullBackupBufferSize;

                if (TotalIncrementBackupSize > 0L)
                {
                    // 120% buffer
                    TotalIncrementBackupBufferSize = (long)(TotalIncrementBackupSize * 1.2);

                    // Add 1 GB to the buffer
                    TotalIncrementBackupBufferSize += (long)(1024 * 1024 * 1024); // 1 GB in bytes
                }

                TotalIncrementBackupAndBufferSize = TotalIncrementBackupSize + TotalIncrementBackupBufferSize;

                WriteLog($@"Acquired backup size.......

                     Total Full Backup Size            : {FormatGB(TotalFullBackupSize)} GB ({TotalFullBackupSize})
                     Total Full Backup Buffer          : {FormatGB(TotalFullBackupBufferSize)} GB ({TotalFullBackupBufferSize})
                     Total Full Backup + Buffer        : {FormatGB(TotalFullBackupAndBufferSize)} GB ({TotalFullBackupAndBufferSize})
                     Total Incremental Backup Size     : {FormatGB(TotalIncrementBackupSize)} GB ({TotalIncrementBackupSize})
                     Total Incremental Buffer Size     : {FormatGB(TotalIncrementBackupBufferSize)} GB ({TotalIncrementBackupBufferSize})
                     Total Incremental Backup + Buffer : {FormatGB(TotalIncrementBackupAndBufferSize)} GB ({TotalIncrementBackupAndBufferSize})
                     Total Achieve Size                : {FormatGB(TotalOldArchiveSize)} GB ({TotalOldArchiveSize})
");

                TotalIncrementBackupAndBufferSize = TotalIncrementBackupSize + TotalIncrementBackupBufferSize;

                DateTime timeAfterGettingSize = DateTime.Now;

                TimeSpan tsTotalSecondsGettingSize = timeAfterGettingSize - timeBeforeGettingSize;

                WriteLog($"Finished acquiring total backup size ({tsTotalSecondsGettingSize.TotalSeconds:0.000} seconds)");

                (DirectoryInfo destinationFolder, DriveInfo destinationDrive, IsFullBackup) = GetDestinationFolder();

                TotalDiskSpace = destinationDrive.TotalSize;
                TotalDiskFreeSpace = destinationDrive.AvailableFreeSpace;

                if (IsFullBackup)
                {
                    RemainingFreeSpace = TotalDiskFreeSpace;
                }
                else
                {
                    // This value has already been collected during GetDestinationFolder() for incremental backup
                    // RemainingFreeSpace = TotalDiskFreeSpace + TotalOldArchiveSize
                }

                DateTime timeAfterGettingDestinationFolder = DateTime.Now;

                TimeSpan tsTotalTimeGettingDestinationFolder = timeAfterGettingDestinationFolder - timeAfterGettingSize;

                WriteLog($"Acquired destination folder: \"{destinationFolder.FullName}\" ({tsTotalTimeGettingDestinationFolder.Minutes} m {tsTotalTimeGettingDestinationFolder.Seconds} s)");

                WriteLog("Backup process begin");

                if (!IsFullBackup && TotalArchiveFiles == 0L)
                {
                    WriteLog("There is no archived files to be backup.");
                    WriteLog("No backup is required");
                }
                else
                {
                    // Perform the backup operation
                    using (StreamWriter successWriter = new StreamWriter(successLogPath, true))
                    {
                        using (StreamWriter failWriter = new StreamWriter(failLogPath, true))
                        {
                            BackupDirectory(SourceFolder, destinationFolder, successWriter, failWriter);
                        }
                    }
                }

                // Backup operation completed

                DateTime timeEnd = DateTime.Now;

                TimeSpan tsBackupTime = timeEnd - timeBeforeGettingSize;

                WriteLog($"Backup process ended - {tsBackupTime.Hours} h {tsBackupTime.Minutes} m {tsBackupTime.Seconds} s {tsBackupTime.Milliseconds} ms");

                DriveInfo destinationDrive2 = new DriveInfo(destinationDrive.Name[0].ToString());

                // Generate a statistics report

                long _backupSize = IsFullBackup ? TotalFullBackupSize : TotalIncrementBackupSize;
                long _bufferSize = IsFullBackup ? TotalFullBackupBufferSize : TotalIncrementBackupBufferSize;
                long _backupBufferSize = IsFullBackup ? TotalFullBackupAndBufferSize : TotalIncrementBackupAndBufferSize;

                string statReport = $@"Backup Type = {(IsFullBackup ? "Full Backup" : "Incremental Backup")}

Time Start           = {timeBeforeGettingSize:yyyy-MM-dd HH:mm:ss}
Time End             = {timeEnd:yyyy-MM-dd HH:mm:ss}
Time Spent           = {tsBackupTime.Days} d {tsBackupTime.Hours} h {tsBackupTime.Minutes} m {tsBackupTime.Seconds} s

Backup Size          = {FormatGB(_backupSize)} GB
Buffer Size          = {FormatGB(_bufferSize)} GB
Backup + Buffer      = {FormatGB(_backupBufferSize)} GB

Old Archive Size     = {FormatGB(TotalOldArchiveSize)} GB

Disk Size            = {FormatGB(destinationDrive2.TotalSize)} GB
Free Space Before    = {FormatGB(TotalDiskFreeSpace)} GB
Free Space After     = {FormatGB(destinationDrive2.AvailableFreeSpace)} GB

Total Files          = {FormatNumber(TotalFiles)} Files
Total Archived Files = {FormatNumber(TotalArchiveFiles)} Files
Total Sub-Folders    = {FormatNumber(TotalSubFolders)} Folders

Total Success        = {FormatNumber(TotalSuccess)} Files 
Total Failed         = {FormatNumber(TotalFailed)} Files
Total Skipped        = {FormatNumber(TotalSkipped)} Files

";

                File.WriteAllText(statLogPath, statReport);

                WriteLog("Exit program gracefully");
            }
            catch (Exception ex)
            {
                try
                {
                    WriteLog($"{ex.Message}");
                    WriteLog("Program terminated");
                }
                catch
                {
                    // unable to write any log, ignoring task, exit program
                }
            }

            try
            {

            }
            catch { }
        }

        static void ReadConfigInitializeLogFolder()
        {
            // Create a timestamped folder for log files

            DirectoryInfo logFolder = new DirectoryInfo(Path.Combine(Application.StartupPath, $"log/{DateTime.Now:yyyy-MM-dd HHmmss}"));

            logFolder.Create();

            // Set the path for the log file
            BasicLogFilePath = Path.Combine(logFolder.FullName, "basic.txt");
            statLogPath = Path.Combine(logFolder.FullName, "statistic.txt");
            successLogPath = Path.Combine(logFolder.FullName, "success.txt");
            failLogPath = Path.Combine(logFolder.FullName, "fail.txt");

            // Write the initial log entry
            WriteLog("Log file created");

            // Clean up old log files
            if (TotalLogFiles > 0)
            {
                // Get a list of log directories sorted by creation time in descending order
                var logDirectories = logFolder.EnumerateDirectories("*", SearchOption.TopDirectoryOnly)
                                              .OrderByDescending(dir => dir.CreationTime);

                var directoriesToDelete = logDirectories.Skip(TotalLogFiles - 1);

                foreach (var dir in directoriesToDelete)
                {
                    try
                    {
                        dir.Delete(true);

                        //Console.WriteLine($"Deleted log directory: {dir.FullName}");
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine($"Failed to delete log directory: {dir.FullName}. Error: {ex.Message}");
                    }
                }
            }

            // Set the path for the configuration file
            var configFilePath = Path.Combine(Application.StartupPath, "config");

            // Check if the configuration file exists
            if (!File.Exists(configFilePath))
            {
                throw new Exception("Configuration data is not existed, please run the setup.");
            }

            // Load the configuration settings
            config = GetSettings(configFilePath);

            WriteLog("Configuration data loaded.");

            // Validate the target folder
            if (string.IsNullOrEmpty(config.TargetFolder))
            {
                throw new Exception("Configuration - TargetFolder is not set. Please run the setup");
            }

            // Validate the backup drive letters
            if (string.IsNullOrEmpty(config.BackupDriveLetters))
            {
                throw new Exception("Configuration - BackupDrive is not specified. Please run the setup");
            }

            SourceFolder = new DirectoryInfo(config.TargetFolder);

            if (!SourceFolder.Exists)
            {
                throw new Exception("Source folder is not existed. Please run the setup");
            }

            WriteLog($"Acquired source folder: {SourceFolder.FullName}");
        }

        static void GetAvailableDestinationDrives()
        {
            config.BackupDriveLetters = config.BackupDriveLetters.ToUpper();

            DriveInfo[] allDrives = DriveInfo.GetDrives();

            AvailableDrives = allDrives.Where(d => config.BackupDriveLetters.Contains(d.Name[0].ToString())).ToList();

            if (AvailableDrives == null || AvailableDrives.Count == 0)
            {
                throw new Exception("Destination drives are not set properly. Please run the setup.");
            }

            AvailableDriveLetters = new List<string>();

            foreach (DriveInfo d in AvailableDrives)
            {
                string driveLetter = d.Name[0].ToString().ToUpper();

                if (driveLetter == "C" || driveLetter == "D")
                {
                    throw new Exception($"Drive {driveLetter} is not allowed to used as destination backup drive. Please run the setup again.");
                }

                AvailableDriveLetters.Add(driveLetter);
            }
        }

        static string FormatGB(long bytes)
        {
            const long GB = 1_073_741_824; // 1024^3
            return bytes == 0 ? "0.000" : (bytes / (double)GB).ToString("N3");
        }

        static string FormatNumber(long number)
        {
            return number.ToString("N0");
        }

        static void WriteLog(string info)
        {
            File.AppendAllText(BasicLogFilePath, $"\r\n{DateTime.Now:yyyy-MM-dd HH:mm:ss}  {info}");
        }

        static void GetDirectorySize(DirectoryInfo folderInfo)
        {
            try
            {
                // Skip if the source folder is in the Recycle Bin
                if (IsRecycleBinPath(folderInfo.FullName))
                {
                    return;
                }

                // Check if the folder is accessible
                if (!folderInfo.Exists)
                {
                    WriteLog($"Folder does not exist: {folderInfo.FullName}");
                    return;
                }

                // Lazy loading the files
                foreach (FileInfo file in folderInfo.EnumerateFiles())
                {
                    try
                    {
                        var fileSize = GetFileAllocationSize(file);

                        TotalFullBackupSize += fileSize;

                        // File is marked as Archive, required for incremental backup
                        if (FileIsArchive(file))
                        {
                            TotalArchiveFiles++;
                            TotalIncrementBackupSize += fileSize;
                        }
                    }
                    catch (Exception exFile)
                    {
                        WriteLog(exFile.Message);
                    }
                }

                // Recursively process subfolders
                foreach (DirectoryInfo subFolder in folderInfo.EnumerateDirectories())
                {
                    GetDirectorySize(subFolder);
                }
            }
            catch (Exception exFolder)
            {
                WriteLog(exFolder.Message);
            }
        }

        static string[] recycleBinNames = { "$RECYCLE.BIN", "$Recycle.Bin", "System Volume Information" };

        static bool IsRecycleBinPath(string path)
        {
            foreach (string recycleBinName in recycleBinNames)
            {
                if (path.Contains(Path.DirectorySeparatorChar + recycleBinName + Path.DirectorySeparatorChar) ||
                    path.EndsWith(Path.DirectorySeparatorChar + recycleBinName))
                {
                    WriteLog($"GetDirectorySize - Skipping folder: {path}");
                    return true;
                }
            }

            return false;
        }

        // Windows API Call - Get the file allocation size
        [DllImport("kernel32.dll")]
        static extern uint GetCompressedFileSize(string lpFileName, out uint lpFileSizeHigh);

        // Get the error performed by last Windows API Call
        [DllImport("kernel32.dll")]
        static extern uint GetLastError();

        static long GetFileAllocationSize(FileInfo fileInfo)
        {
            try
            {
                // The following section is to obtain the file disk space or file allocation size
                // There is a difference between file size and file allocation size

                // FileInfo.Length refers to the file size, not the file allocation size (disk size)

                // Example: Disk Cluster size = 4KB

                // Example 1: File Size = 1KB
                // File Allocation Size = 4KB

                // Example 2: File Size = 4.1KB
                // File Allocation Size = 8KB

                // Obtaining the file allocation size (file clusters)
                uint high;
                uint low = GetCompressedFileSize(fileInfo.FullName, out high);

                // 0xFFFFFFFF equals the max value, exactly 4,294,967,295 bytes
                // The system will return this value to indicate there is an error reading the file
                if (low == 0xFFFFFFFF)
                {
                    // To further test if the system really encounter an error or
                    // the file exactly the file size of 4,294,967,295 bytes (which is highly unlikely)

                    // Getting error code from the last Windows API call (kernel32.dll)
                    uint error = GetLastError();

                    // No error
                    if (error == 0)
                    {
                        // File size is exactly 4,294,967,295 bytes
                        return 0xFFFFFFFF;
                    }
                    else
                    {
                        // An actual error occurred
                        // Do nothing
                    }
                }
                else
                {
                    // The GetCompressedFileSize function retrieves the compressed file size,
                    // with low representing the lower 32 bits and
                    // high representing the upper 32 bits of the 64-bit file size.
                    // The two parts are then combined to form the complete 64-bit file size.
                    return ((long)high << 32) | low;
                }
            }
            catch
            { }

            return 0L;
        }

        static (DirectoryInfo, DriveInfo, bool) GetDestinationFolder()
        {
            if (config.TotalDaysForFullBackup == 0)
            {
                // ===============================================
                // Condition 1: Always Full Backup only
                // ===============================================

                WriteLog("Condition 1: Program is set to always run full backup");
                WriteLog($"Total required backup size: {FormatGB(TotalFullBackupAndBufferSize)} GB");

                // Check is there any backup ever executed

                (DirectoryInfo lastDir, DriveInfo lastDrive) = GetLatestFolder();

                DriveInfo startDrive = (lastDrive == null) ? null : lastDrive;

                (DirectoryInfo dir, DriveInfo drv) = GetFullBackupFolder(startDrive);

                if (dir == null)
                {
                    throw new Exception("All drives do not have enough disk space");
                }

                return (dir, drv, true);
            }
            else
            {
                // ===========================================================
                // Condition 2: Incremental + Full Backup
                // ===========================================================

                WriteLog("Condition 2: Incremental + Full Backup");

                (DirectoryInfo lastDir, DriveInfo lastDrive) = GetLatestFolder();

                DriveInfo startDrive = (lastDrive == null) ? null : lastDrive;

                DateTime lastBackupDate = DateTime.MinValue;

                if (lastDir != null)
                {
                    DateTime.TryParseExact(lastDir.Name, "yyyy-MM-dd HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out lastBackupDate);
                }

                if (lastBackupDate == DateTime.MinValue)
                {
                    // ===========================================================
                    // Condition 3: No backup was ever executed - Running Full Backup
                    // ===========================================================

                    WriteLog("Condition 3: No backup was ever executed - Running Full Backup");

                    (DirectoryInfo dir, DriveInfo drv) = GetFullBackupFolder(startDrive);

                    if (dir == null)
                    {
                        throw new Exception("All drives do not have enough disk space");
                    }

                    return (dir, drv, true);
                }
                else
                {
                    // ===========================================================
                    // Condition 4: At least one backup was executed
                    // ===========================================================

                    WriteLog($"Condition 4: At least one backup was executed: {lastBackupDate:yyyy-MM-dd HHmmss}");

                    TimeSpan tsBackupDaysOld = DateTime.Now - lastBackupDate;

                    WriteLog($"Total days since last FULL backup: {tsBackupDaysOld.TotalDays:0.0}");
                    WriteLog($"Interval of days for full backup: {config.TotalDaysForFullBackup}");

                    if (tsBackupDaysOld.TotalDays < config.TotalDaysForFullBackup)
                    {
                        // ===========================================================
                        // Condition 5: The total number of days elapsed since the last backup is still within the predefined interval for full backups
                        // ===========================================================

                        // Attempt to perform Incremental Backup

                        WriteLog("Condition 5: Days since last backup still within full backup interval");
                        WriteLog("Attempt to find out if disk space enough for incremental backup");

                        WriteLog("Calculating old archive size from destination folder");

                        TotalOldArchiveSize = 0L;

                        GetOldBackupArchiveSize(SourceFolder, lastDir);

                        WriteLog($"Old archive size is {FormatGB(TotalOldArchiveSize)} GB ({TotalOldArchiveSize})");

                        WriteLog($"Free available space at destination Drive {startDrive.Name[0]} is {FormatGB(startDrive.AvailableFreeSpace)} GB ({startDrive.AvailableFreeSpace})");

                        RemainingFreeSpace = startDrive.AvailableFreeSpace + TotalOldArchiveSize;

                        WriteLog($"Total estimated available size: {FormatGB(RemainingFreeSpace)} GB ({RemainingFreeSpace}), Old archive size + free sapce");

                        WriteLog($"Total incremental size is {FormatGB(TotalIncrementBackupAndBufferSize)} GB ({TotalIncrementBackupAndBufferSize})");

                        bool enoughFreeSpace = RemainingFreeSpace > TotalIncrementBackupAndBufferSize;

                        if (enoughFreeSpace)
                        {
                            // proceed incremental backup
                            WriteLog($"Drive {startDrive.Name[0]} has enough space for incremental backup");
                            WriteLog("Executing incremental backup");
                            return (startDrive.RootDirectory.CreateSubdirectory($"{DateTime.Now:yyyy-MM-dd HHmmss}"), startDrive, false);
                        }
                        else
                        {
                            // insufficient free space
                            // cancel incremental backup
                            // switch to next drive and do new full backup

                            WriteLog($"Insufficient space at Drive {startDrive.Name[0]} for incremental backup");
                            WriteLog("Give up incremental backup, switch to next drive and attempt for full backup");

                            (DirectoryInfo dir, DriveInfo drv) = GetFullBackupFolder(startDrive);

                            if (dir == null)
                            {
                                throw new Exception("All drives do not have enough disk space");
                            }

                            return (dir, drv, true);
                        }
                    }
                    else
                    {
                        // ===========================================================
                        // Condition 6: The total number of days elapsed since the last backup has surpassed the predefined interval for full backups
                        // ===========================================================

                        // attempt to perform Full Backup

                        WriteLog("Condition 6: The total number of days elapsed since the last backup has surpassed the predefined interval for full backups");

                        (DirectoryInfo dir, DriveInfo drv) = GetFullBackupFolder(startDrive);

                        if (dir == null)
                        {
                            throw new Exception("All drives do not have enough disk space");
                        }

                        return (dir, drv, true);
                    }
                }
            }
        }

        static void GetOldBackupArchiveSize(DirectoryInfo sourceFolderInfo, DirectoryInfo destFolderInfo)
        {
            // Calculates the total size of old archive files in the destination directory
            // that correspond to the files in the source directory.

            try
            {
                foreach (var fileInfo in sourceFolderInfo.EnumerateFiles("*", SearchOption.TopDirectoryOnly))
                {
                    try
                    {
                        if (!FileIsArchive(fileInfo))
                            continue;

                        FileInfo destFileInfo = new FileInfo(Path.Combine(destFolderInfo.FullName, fileInfo.Name));

                        if (destFileInfo.Exists)
                        {
                            TotalOldArchiveSize += GetFileAllocationSize(destFileInfo);
                        }
                    }
                    catch { }
                }

                foreach (var sourceSubDir in sourceFolderInfo.EnumerateDirectories("*", SearchOption.TopDirectoryOnly))
                {
                    try
                    {
                        DirectoryInfo destSubFolder = GenerateDestinationFolder(destFolderInfo, sourceSubDir);

                        if (!destSubFolder.Exists)
                            continue;

                        GetOldBackupArchiveSize(sourceSubDir, destSubFolder);
                    }
                    catch { }
                }
            }
            catch { }
        }

        static DriveInfo GetDriveWithEnoughSpace(long requiredSpace, DriveInfo startDrive, bool checkFreeSpaceOnly, bool startFromNextDrive)
        {
            if (checkFreeSpaceOnly)
            {
                WriteLog("Identifying free space...");
            }
            else
            {
                WriteLog("Indentifying disk size...");
            }

            int startIndex = 0;

            if (startDrive != null)
            {
                for (int i = 0; i < AvailableDrives.Count; i++)
                {
                    if (AvailableDrives[i].Name == startDrive.Name)
                    {
                        if (startFromNextDrive)
                        {
                            startIndex = i + 1;
                        }
                        else
                        {
                            startIndex = i;
                        }
                        break;
                    }
                }
            }

            for (int i = 0; i < AvailableDrives.Count; i++)
            {
                int thisIndex = i + startIndex;

                if (thisIndex >= AvailableDrives.Count)
                {
                    thisIndex = thisIndex - AvailableDrives.Count;
                }

                long spacesize = checkFreeSpaceOnly ? AvailableDrives[thisIndex].AvailableFreeSpace : AvailableDrives[thisIndex].TotalSize;

                if (CheckDriveSpace(requiredSpace, AvailableDrives[thisIndex], checkFreeSpaceOnly, null))
                {
                    return AvailableDrives[thisIndex];
                }
            }

            return null;
        }

        static bool CheckDriveSpace(long requiredSpace, DriveInfo drv, bool checkFreeSpaceOnly, DirectoryInfo lastUsedBackupFolder)
        {
            if (checkFreeSpaceOnly)
            {
                // Checking free space

                if (drv.AvailableFreeSpace > requiredSpace)
                {
                    WriteLog($"Drive {drv.Name[0]} selected (Free space = {FormatGB(drv.AvailableFreeSpace)} GB, {drv.AvailableFreeSpace})");
                    return true;
                }
                else
                {
                    WriteLog($"Drive {drv.Name[0]} does not has enough free space ({FormatGB(drv.AvailableFreeSpace)} GB, {drv.AvailableFreeSpace})");
                }
            }
            else
            {
                // Checking disk size

                if (drv.TotalSize > requiredSpace)
                {
                    WriteLog($"Drive {drv.Name[0]} selected (Disk size = {FormatGB(drv.TotalSize)} GB, {drv.TotalSize})");
                    return true;
                }
                else
                {
                    WriteLog($"Drive {drv.Name[0]} does not has enough disk size ({FormatGB(drv.TotalSize)} GB, {drv.TotalSize})");
                }
            }

            return false;
        }

        static (DirectoryInfo, DriveInfo) GetLatestFolder()
        {
            DirectoryInfo folder = null;
            DriveInfo drive = null;

            DateTime latestDate = DateTime.MinValue;

            foreach (DriveInfo drv in AvailableDrives)
            {
                foreach (DirectoryInfo dir in drv.RootDirectory.EnumerateDirectories("*", SearchOption.TopDirectoryOnly))
                {
                    if (DateTime.TryParseExact(dir.Name, "yyyy-MM-dd HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime folderDate))
                    {
                        if (folderDate > latestDate)
                        {
                            latestDate = folderDate;
                            drive = drv;
                            folder = dir;
                        }
                    }
                }
            }

            return (folder, drive);
        }

        static (DirectoryInfo, DriveInfo) GetFullBackupFolder(DriveInfo startDrive)
        {
            // check for free space

            WriteLog("Preparing for full backup");
            WriteLog($"Full backup + buffer size: {(FormatGB(TotalFullBackupAndBufferSize))} GB, {TotalFullBackupAndBufferSize}");

            var freeDrive = GetDriveWithEnoughSpace(TotalFullBackupAndBufferSize, startDrive, true, false);

            if (freeDrive == null)
            {
                // None of the drive has enough free space
                // Require format
                // Get next drive for format

                WriteLog("None of the drive has enough FREE space");
                WriteLog("A format is require");

                DriveInfo diskDrive = GetDriveWithEnoughSpace(TotalFullBackupAndBufferSize, startDrive, false, true);

                if (diskDrive != null)
                {
                    // This disk's capacity is sufficient for the backup
                    // Format this drive

                    FormatDrive(diskDrive);

                    // Create a new folder. Use this folder as destination

                    WriteLog("Executing full backup");

                    return (diskDrive.RootDirectory.CreateSubdirectory($"{DateTime.Now:yyyy-MM-dd HHmmss}"), diskDrive);
                }
            }
            else
            {
                // This drive has enough free space
                // Create a new folder. Use this folder as destination

                WriteLog("Executing full backup");

                return (freeDrive.RootDirectory.CreateSubdirectory($"{DateTime.Now:yyyy-MM-dd HHmmss}"), freeDrive);
            }

            return (null, null);
        }

        static void FormatDrive(DriveInfo selectedDrive)
        {
            if (selectedDrive.Name[0] == 'C' || selectedDrive.Name[0] == 'D')
            {
                throw new Exception("You are not allowed to backup the files to Drive C or Drive D.");
            }

            WriteLog($"Begin formatting drive {selectedDrive.Name[0]}");

            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("\\\\.\\ROOT\\CIMV2", $"SELECT * FROM Win32_Volume WHERE DriveLetter = '{selectedDrive.Name[0]}:'");
                foreach (ManagementObject volume in searcher.Get())
                {
                    // The parameters are: file system, quick format, cluster size, label, enable compression
                    object[] formatArgs = { "NTFS", true, 4096, "Backup", false };
                    volume.InvokeMethod("Format", formatArgs);
                }

                WriteLog("Format success!");
            }
            catch (ManagementException ex)
            {
                throw ex;
            }
        }

        static bool FileIsArchive(FileInfo file)
        {
            return ((file.Attributes & FileAttributes.Archive) == FileAttributes.Archive);
        }

        static void BackupDirectory(DirectoryInfo sourceFolderInfo, DirectoryInfo destFolderInfo, StreamWriter successWriter, StreamWriter failWriter)
        {
            // There is a known bug in .NET when using the following code:

            // string[] folders = Directory.GetDirectories("/path");
            // string[] files = Directory.GetFiles("/path");

            // The issue arises when the file or folder names contain leading or trailing whitespace.
            // In such cases, the files and folders arrays store the paths as plain strings, including the whitespace.
            // Later, when attempting to copy these files, the system might throws a "file not found" exception.

            // To resolve this problem, it is recommended to use Directory.EnumerateFiles and Directory.EnumerateDirectories instead.
            // These methods return a list of FileInfo and DirectoryInfo objects, respectively, rather than plain string paths.

            // IEnumerable<DirectoryInfo> directories = Directory.EnumerateDirectories("/path");
            // IEnumerable<FileInfo> files = Directory.EnumerateFiles("/path");

            // Directory.EnumerateFiles and Directory.EnumerateDirectories employ lazy loading,
            // which means they retrieve items one at a time as needed, instead of loading all items at once.
            // This approach is more memory - efficient, particularly when dealing with hugh amount directories and files.

            // Use FileStream directly with the FileInfo object to perform Copy operation.
            // This ensures that the process always refers to the exact file it intends to work with.
            // This way we won't have to worry about the leading or trailing whitespace of filenames.

            // Additionally, FileStream is efficient for copying large files.

            // References:
            // -Stack Overflow: Directory.GetFiles throws exception folder name has spaces at the end not recognized
            // https://stackoverflow.com/questions/28888525/directory-getfiles-throws-exception-folder-name-has-spaces-at-the-end-not-re
            // -Microsoft Docs: How to enumerate directories and files
            // https://learn.microsoft.com/en-us/dotnet/standard/io/how-to-enumerate-directories-and-files?redirectedfrom=MSDN

            try
            {
                // Create the destination folder if it doesn't exist
                destFolderInfo.Create();
            }
            catch (Exception exCreateFolder)
            {
                // Log an error message if unable to create the destination folder
                WriteLog($"Cannot create target folder ({exCreateFolder.Message.Replace("\r\n", string.Empty)}): {destFolderInfo.FullName}");
                return;
            }

            // Iterate through each file in the source directory (non-recursive)
            foreach (FileInfo sourceFile in sourceFolderInfo.EnumerateFiles("*", SearchOption.TopDirectoryOnly))
            {
                try
                {

                    // Construct the destination file path
                    string destFilePath = Path.Combine(destFolderInfo.FullName, sourceFile.Name);
                    FileInfo destFileInfo = new FileInfo(destFilePath);

                    if (IsFullBackup)
                    {
                        // Full backup

                        if (CheckEnoughDiskSpaceUpdateAvailableDiskSpace(sourceFile))
                        {
                            CopyFile(sourceFile, destFileInfo, successWriter, failWriter);
                        }
                        else
                        {
                            WriteLog($"Program stopped at: {sourceFile.FullName}");
                            throw new Exception("Not enough disk space. Program terminated.");
                        }
                    }
                    else
                    {
                        // Incremental backup

                        if (FileIsArchive(sourceFile))
                        {
                            if (CheckEnoughDiskSpaceUpdateAvailableDiskSpace(sourceFile))
                            {
                                CopyFile(sourceFile, destFileInfo, successWriter, failWriter);
                            }
                            else
                            {
                                WriteLog($"Program stopped at: {sourceFile.FullName}");
                                throw new Exception("Not enough disk space. Program terminated.");
                            }
                        }
                    }

                    // Increment the total file count
                    TotalFiles++;
                }
                catch (UnauthorizedAccessException)
                {
                    // Log an error message if access is denied to the file and update failed count
                    failWriter.WriteLine($"Access Denied: {sourceFile.FullName}");
                    TotalFailed++;
                }
                catch (Exception e)
                {
                    // Log an error message for any other exception and update failed count
                    TotalFailed++;
                    failWriter.WriteLine($"{sourceFile.FullName}\r\n{e.Message}\r\n");
                }
            }

            // Recursively process subdirectories
            foreach (DirectoryInfo subDir in sourceFolderInfo.EnumerateDirectories("*", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    // Skip if the subdirectory is in the Recycle Bin
                    if (IsRecycleBinPath(subDir.FullName))
                    {
                        continue;
                    }

                    // Increment the total subfolder count
                    TotalSubFolders++;

                    // Generates sub-folder
                    DirectoryInfo destSubFolder = GenerateDestinationFolder(destFolderInfo, subDir);

                    // Recursively call the BackupDirectory method for the subdirectory
                    BackupDirectory(subDir, destSubFolder, successWriter, failWriter);
                }
                catch (UnauthorizedAccessException)
                {
                    // Log an error message if access is denied to the subdirectory
                    failWriter.WriteLine($"Access denied: {subDir.FullName}");
                }
                catch (Exception e)
                {
                    // Log an error message for any other exception
                    failWriter.WriteLine($"{subDir.FullName}\r\n{e.Message}\r\n");
                }
            }
        }

        static void CopyFile(FileInfo fileSource, FileInfo fileDest, StreamWriter successWriter, StreamWriter failWriter)
        {
            using (FileStream sourceStream = fileSource.OpenRead())
            {
                using (FileStream destStream = fileDest.OpenWrite())
                {
                    sourceStream.CopyTo(destStream);
                }
            }

            // Reset the archive attribute on the source file
            fileSource.Attributes &= ~FileAttributes.Archive;

            // Write the file information to the success log
            successWriter.WriteLine(fileSource.FullName);

            // Increment the success counter
            TotalSuccess++;
        }

        public static bool CheckEnoughDiskSpaceUpdateAvailableDiskSpace(FileInfo fileInfo)
        {
            if (IsFullBackup)
                return true;

            long fileSize = GetFileAllocationSize(fileInfo);

            if (RemainingFreeSpace > fileSize)
            {
                RemainingFreeSpace -= fileSize;

                return true;
            }

            WriteLog($"Remaining free disk space: {FormatGB(RemainingFreeSpace)} GB, Required disk space: {FormatGB(fileSize)} GB");
            return false;
        }

        static DirectoryInfo GenerateDestinationFolder(DirectoryInfo baseDestFolderInfo, DirectoryInfo sourceSubDir)
        {
            // Construct the destination subdirectory path
            string destSubFolderPath = Path.Combine(baseDestFolderInfo.FullName, sourceSubDir.Name);

            // Removes leading and trailing whitespace from every folder's name and sub-folder's name 
            destSubFolderPath = RemoveWhitespaceFromPath(destSubFolderPath);

            DirectoryInfo destSubFolder = new DirectoryInfo(destSubFolderPath);

            return destSubFolder;
        }

        public static string RemoveWhitespaceFromPath(string path)
        {
            string[] parts = path.Split(Path.DirectorySeparatorChar)
                .Select(part => part.Trim())
                .Where(part => !string.IsNullOrEmpty(part))
                .ToArray();

            return Path.Combine(parts);
        }

        public static Settings GetSettings(string configFilePath)
        {
            Settings settings = null;

            try
            {
                byte[] ba = File.ReadAllBytes(configFilePath);

                ba = AES_Decrypt(ba);

                string input = Encoding.UTF8.GetString(ba);

                settings = new Settings();
                var lines = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    var keyValue = line.Split('=');
                    if (keyValue.Length == 2)
                    {
                        var key = keyValue[0].Trim();
                        var value = keyValue[1].Trim();
                        switch (key)
                        {
                            case "TargetFolder":
                                settings.TargetFolder = value;
                                break;
                            case "BackupDriveLetters":
                                settings.BackupDriveLetters = value;
                                break;
                            case "TotalDaysForFullBackup":
                                if (int.TryParse(value, out int totalDays))
                                {
                                    settings.TotalDaysForFullBackup = totalDays;
                                }
                                break;
                        }
                    }
                }
            }
            catch
            {
                throw new Exception("Error reading config file. Program terminated.");
            }

            return settings;
        }

        public static byte[] AES_Encrypt(byte[] bytesToBeEncrypted)
        {
            byte[] encryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            byte[] passwordBytes = new byte[] { 21, 32, 34, 45, 56, 67, 78, 89, 90, 42, 35, 64, 86, 97 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 5);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        public static byte[] AES_Decrypt(byte[] bytesToBeDecrypted)
        {
            byte[] decryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            byte[] passwordBytes = new byte[] { 21, 32, 34, 45, 56, 67, 78, 89, 90, 42, 35, 64, 86, 97 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 5);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }
    }
}