using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using System.Globalization;

using System.Diagnostics;
using System.Data;
using System.Security.Cryptography;

using System.Management;

// version beta 0.4

// when doing compiling, exclude this file from the source code
// this file is kept here for historical reference purpose

namespace System
{
    internal class Program_beta04
    {
        static Settings config = null;

        static string ConfigFilePath = "";
        static string BasicLogFilePath = "";

        static int TotalFiles = 0;
        static int TotalSubFolders = 0;
        static int TotalSuccess = 0;
        static int TotalFailed = 0;
        static int TotalSkipped = 0;

        static void Main_beta04(string[] args)
        {
            ConfigFilePath = Path.Combine(Application.StartupPath, "config");
            BasicLogFilePath = Path.Combine(Application.StartupPath, "log.txt");

            // Check if the log file size exceeded 1MB
            if (File.Exists(BasicLogFilePath))
            {
                FileInfo fileInfo = new FileInfo(BasicLogFilePath);

                if (fileInfo.Length > 1048576)  // 1MB in bytes 
                {
                    // Backup the log file
                    File.Copy(BasicLogFilePath, BasicLogFilePath.Replace("log.txt", "log-old.txt"), true);

                    // Clear the log
                    File.WriteAllText(BasicLogFilePath, "Old log's size exceeded 1MB, it is copied to log-old.txt. Begin new log file.\r\n\r\n");
                }
            }

            try
            {
                WriteLog("Process started");

                if (!File.Exists(ConfigFilePath))
                {
                    WriteLog("config.txt not existed, please run the setup.");
                    WriteLog("Program terminated.");
                    return;
                }

                config = GetSettings();

                if (string.IsNullOrEmpty(config.TargetFolder))
                {
                    WriteLog("config.txt - TargetFolder is not set. Please run the setup");
                    WriteLog("Program terminated.");
                    return;
                }

                if (string.IsNullOrEmpty(config.BackupDriveLetters))
                {
                    WriteLog("config.txt - BackupDrive is not specified. Please run the setup");
                    WriteLog("Program terminated.");
                    return;
                }

                string targetFolder = config.TargetFolder;

                WriteLog($"Acquired target backup folder: {targetFolder}");

                DirectoryInfo dirInfo = new DirectoryInfo(targetFolder);

                string[] myDrives = config.BackupDriveLetters.Split(new char[] { ',', '|' }, StringSplitOptions.RemoveEmptyEntries);

                // Checking prohibited drive
                foreach (var drive in myDrives)
                {
                    if (drive.ToUpper().Contains("C") || drive.ToUpper().Contains("D"))
                    {
                        WriteLog("Cannot backup to drive C or drive D. Program Terminated");
                        return;
                    }
                }

                DateTime timeBeforeGettingSize = DateTime.Now;

                long totalSize = GetDirectorySize(dirInfo);

                DateTime timeAfterGettingSize = DateTime.Now;

                TimeSpan tsTotalSecondsGettingSize = timeAfterGettingSize - timeBeforeGettingSize;

                double totalGB = totalSize / 1073741824.0;

                WriteLog($"Acquired total size to backup: {totalGB:#,##0.###} GB ({tsTotalSecondsGettingSize.TotalSeconds:0.000} seconds)");

                string destinationFolder = GetDestinationFolder(totalSize, myDrives);

                if (!Directory.Exists(destinationFolder))
                {
                    Directory.CreateDirectory(destinationFolder);
                }

                DateTime timeAfterGettingDestinationFolder = DateTime.Now;

                TimeSpan tsTotalSecondsGettingDestinationFolder = timeAfterGettingDestinationFolder - timeAfterGettingSize;

                WriteLog($"Acquired destination folder: {destinationFolder} ({tsTotalSecondsGettingDestinationFolder.TotalSeconds:0.000} seconds)");

                string timenow = DateTime.Now.ToString("yyyy-MM-dd_HHmmss");

                string statLogPath = $"{destinationFolder}\\log-({timenow})-stat.txt";
                string successLogPath = $"{destinationFolder}\\log-({timenow})-success.txt";
                string failLogPath = $"{destinationFolder}\\log-({timenow})-fail.txt";

                TotalFiles = 0;
                TotalSubFolders = 0;
                TotalSuccess = 0;
                TotalFailed = 0;
                TotalSkipped = 0;

                WriteLog("Backup process begin");

                using (StreamWriter successWriter = new StreamWriter(successLogPath, true))
                {
                    using (StreamWriter failWriter = new StreamWriter(failLogPath, true))
                    {
                        Directory.CreateDirectory(destinationFolder);

                        BackupDirectory(targetFolder, destinationFolder, successWriter, failWriter);
                    }
                }

                TimeSpan ts = DateTime.Now - timeAfterGettingDestinationFolder;

                WriteLog($"Backup process ended - {ts.Hours} h {ts.Minutes} m {ts.Seconds} s {ts.Milliseconds} ms");

                string statReport = $@"Total Files = {TotalFiles}
Total Sub Folders = {TotalSubFolders}
Total Success = {TotalSuccess}
Total Failed = {TotalFailed}
Total Skipped = {TotalSkipped}";

                File.WriteAllText(statLogPath, statReport);

                WriteLog("Exit program gracefully\r\n");
            }
            catch (Exception ex)
            {
                try
                {
                    WriteLog($"Unknown Error: {ex.Message}. Program terminated.");
                }
                catch
                {
                    // unable to write any log, ignoring task, exit program
                }
            }
        }

        static void WriteLog(string info)
        {
            File.AppendAllText(BasicLogFilePath, $"\r\n{DateTime.Now:yyyy-MM-dd HH:mm:ss}  {info}");
        }

        static long GetDirectorySize(DirectoryInfo dirInfo)
        {
            long size = 0;

            // Calculate size of all files in the directory.
            FileInfo[] files;
            try
            {
                files = dirInfo.GetFiles();
            }
            catch (UnauthorizedAccessException)
            {
                //Console.WriteLine($"Skipping folder {dirInfo.FullName}, access denied.");
                return 0;
            }
            catch (Exception e)
            {
                //Console.WriteLine($"Skipping folder {dirInfo.FullName}, error: {e.Message}");
                return 0;
            }

            foreach (FileInfo file in files)
            {
                try
                {
                    size += file.Length;
                }
                catch (UnauthorizedAccessException)
                {
                    //Console.WriteLine($"Skipping file {file.FullName}, access denied.");
                }
                catch (Exception e)
                {
                    //Console.WriteLine($"Skipping file {file.FullName}, error: {e.Message}");
                }
            }

            // Calculate size of all subdirectories.
            DirectoryInfo[] subDirs;
            try
            {
                subDirs = dirInfo.GetDirectories();
            }
            catch (UnauthorizedAccessException)
            {
                //Console.WriteLine($"Skipping folder {dirInfo.FullName}, access denied.");
                return size;
            }
            catch (Exception e)
            {
                //Console.WriteLine($"Skipping folder {dirInfo.FullName}, error: {e.Message}");
                return size;
            }

            foreach (DirectoryInfo subDir in subDirs)
            {
                size += GetDirectorySize(subDir);
            }

            return size;
        }

        static string GetDestinationFolder(long totalSize, string[] lstMyDrive)
        {
            string timeNowStr = DateTime.Now.ToString("yyyy-MM-dd HHmmss");

            // Stage 1: Collecting all found drives that match with the settings

            DriveInfo[] allDrives = DriveInfo.GetDrives();
            List<DriveInfo> matchingDrives = allDrives.Where(d => lstMyDrive.Contains(d.Name[0].ToString())).ToList();

            // Stage 2: Get the latest backup date from folder's name

            Dictionary<string, DateTime> dicFolderDate = new Dictionary<string, DateTime>();

            foreach (var drive in matchingDrives)
            {
                string[] directories;
                try
                {
                    directories = Directory.GetDirectories(drive.Name);
                }
                catch (Exception)
                {
                    continue;
                }

                foreach (var dir in directories)
                {
                    string folderName = new DirectoryInfo(dir).Name;

                    if (DateTime.TryParseExact(folderName, "yyyy-MM-dd HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime folderDate))
                    {
                        dicFolderDate[dir] = folderDate;
                    }
                }
            }

            // Stage 3: Check if there is any folder existed

            // No folder, means no backup is ever executed.
            if (dicFolderDate.Count == 0)
            {
                // Begin full backup, save to the first drive
                WriteLog("Executing full backup");
                return $"{lstMyDrive[0]}:\\{timeNowStr}";
            }

            // Stage 4: Get the latest backup date and folder

            string latestBackupFolder = "";
            DateTime latestBackupDate = DateTime.MinValue;

            foreach (var keyValuePair in dicFolderDate)
            {
                if (keyValuePair.Value > latestBackupDate)
                {
                    latestBackupDate = keyValuePair.Value;
                    latestBackupFolder = keyValuePair.Key;
                }
            }

            // Stage 5: There is no recorded backup

            if (latestBackupDate == DateTime.MinValue)
            {
                // Begin full backup, save to the first drive
                WriteLog("Executing full backup");
                return $"{lstMyDrive[0]}:\\{timeNowStr}";
            }

            // Stage 6: Check if the total number of days since the last full backup exceeds the threshold

            var timespanTotalDaysOld = DateTime.Now - latestBackupDate;

            WriteLog($"Total days for full backup: {config.TotalDaysForFullBackup}, Days since last backup: {timespanTotalDaysOld.TotalDays.ToString("0.0")}");

            if (timespanTotalDaysOld.TotalDays >= (double)config.TotalDaysForFullBackup)
            {
                // Perform a full backup

                WriteLog("Executing full backup");

                bool requireFormat = false;

                string newTargetedDriveLetter = GetSuitableDrive(totalSize, lstMyDrive, out requireFormat);

                // Format the drive, a quick way to erase all files
                if (requireFormat)
                {
                    WriteLog("Disk drive fulled, switching to next drive with older version of backup");
                    FormatDrive(newTargetedDriveLetter);
                }

                return $"{newTargetedDriveLetter}:\\{timeNowStr}";
            }
            else
            {
                // Perform incremental backup, continue to use the latest used backup folder

                WriteLog("Executing incremental backup");

                return latestBackupFolder;
            }
        }

        static string GetSuitableDrive(long totalSize, string[] lstMyDrive, out bool requireFormat)
        {
            requireFormat = false;

            DriveInfo[] allDrives = DriveInfo.GetDrives();
            List<DriveInfo> matchingDrives = allDrives.Where(d => lstMyDrive.Contains(d.Name[0].ToString())).ToList();

            // Condition 1: Check available free space
            foreach (DriveInfo drive in matchingDrives)
            {
                if (drive.IsReady)
                {
                    bool enoughSpace = drive.AvailableFreeSpace > totalSize;
                    if (enoughSpace)
                    {
                        return drive.Name[0].ToString();
                    }
                }
            }

            // Condition 2: Check for latest folder

            requireFormat = true;

            DateTime latestDate = DateTime.MinValue;
            string latestDrive = "";
            foreach (DriveInfo drive in matchingDrives)
            {
                if (drive.IsReady)
                {
                    string[] directories;
                    try
                    {
                        directories = Directory.GetDirectories(drive.Name);
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    foreach (string dir in directories)
                    {
                        string folderName = new DirectoryInfo(dir).Name;
                        if (DateTime.TryParseExact(folderName, "yyyy-MM-dd HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime folderDate))
                        {
                            if (folderDate > latestDate)
                            {
                                latestDate = folderDate;
                                latestDrive = drive.Name[0].ToString();
                            }
                        }
                    }
                }
            }

            // Find the next drive after the one with the latest folder

            // Get the current position drive letter in the array (list)
            int latestDriveIndex = Array.IndexOf(lstMyDrive, latestDrive);

            // Move towards the next drive
            latestDriveIndex++;

            // The index position falls out the array (list)
            if (latestDriveIndex >= lstMyDrive.Length)
            {
                // Going back to the first drive
                latestDriveIndex = 0;
            }

            return lstMyDrive[latestDriveIndex];

        }

        static void FormatDrive(string driveLetter)
        {
            if (driveLetter == "C" || driveLetter == "D")
            {
                WriteLog("You are not allowed to backup the files to Drive C or Drive D. Program Terminated.");
                Environment.Exit(0);
            }

            WriteLog($"Begin formatting drive {driveLetter}:\\, for quickly erasing all files");

            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("\\\\.\\ROOT\\CIMV2", $"SELECT * FROM Win32_Volume WHERE DriveLetter = '{driveLetter}:'");
                foreach (ManagementObject volume in searcher.Get())
                {
                    // The parameters are: file system, quick format, cluster size, label, enable compression
                    volume.InvokeMethod("Format", new object[] { "NTFS", true, 4096, "Backup", false });
                    WriteLog("Format success.");
                }
            }
            catch (ManagementException ex)
            {
                // Handle exception
                WriteLog($"Error formatting drive {driveLetter}: {ex.Message}");
            }
        }

        static void BackupDirectory(string sourceFolder, string destFolder, StreamWriter successWriter, StreamWriter failWriter)
        {
            if (!Directory.Exists(destFolder))
            {
                Directory.CreateDirectory(destFolder);
                successWriter.WriteLine($"Create folder: {destFolder}");
            }

            // Copy all files from source to destination folder
            string[] files = null;
            try
            {
                files = Directory.GetFiles(sourceFolder);
            }
            catch (UnauthorizedAccessException)
            {
                failWriter.WriteLine($"{sourceFolder}\r\nAccess Denied\r\n");
            }
            catch (Exception e)
            {
                failWriter.WriteLine($"{sourceFolder}\r\n{e.Message}\r\n");
            }

            if (files != null && files.Length > 0)
            {
                TotalFiles += files.Length;

                foreach (string file in files)
                {
                    try
                    {
                        string name = Path.GetFileName(file);
                        string dest = Path.Combine(destFolder, name);

                        // Check if the file exists in the destination folder
                        if (File.Exists(dest))
                        {
                            // Compare last write times
                            DateTime srcWriteTime = File.GetLastWriteTime(file);
                            DateTime destWriteTime = File.GetLastWriteTime(dest);

                            // If the source file has a later write time, copy it over
                            if (srcWriteTime > destWriteTime)
                            {
                                // Overwrite if file already exists
                                File.Copy(file, dest, true);

                                FileInfo fileInfo = new FileInfo(file);
                                double fileSize = (double)fileInfo.Length / 1048576.0;  // Convert to MB
                                successWriter.WriteLine($"{fileSize:000.000} MB - (updated) {file}");
                                TotalSuccess++;
                            }
                            else
                            {
                                // Else, skip copying the file
                                TotalSkipped++;
                            }
                        }
                        else
                        {
                            // If the file doesn't exist in the destination, copy it over
                            // Overwrite if file already exists
                            File.Copy(file, dest, true);

                            FileInfo fileInfo = new FileInfo(file);
                            double fileSize = (double)fileInfo.Length / 1048576.0;  // Convert to MB
                            successWriter.WriteLine($"{fileSize:000.000} MB - {file}");
                            TotalSuccess++;
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        failWriter.WriteLine($"{file}\r\nAccess Denied\r\n");
                        TotalFailed++;
                    }
                    catch (Exception e)
                    {
                        TotalFailed++;
                        failWriter.WriteLine($"{file}\r\n{e.Message}\r\n");
                    }
                }
            }


            // Copy all subfolders from source to destination folder
            string[] folders = null;

            try
            {
                folders = Directory.GetDirectories(sourceFolder);
            }
            catch (UnauthorizedAccessException)
            {
                failWriter.WriteLine($"{sourceFolder}\r\nAccess denied\r\n");
            }
            catch (Exception e)
            {
                failWriter.WriteLine($"{sourceFolder}\r\nAccess {e.Message}\r\n");
            }

            if (folders != null && folders.Length > 0)
            {
                TotalSubFolders += folders.Length;

                foreach (string folder in folders)
                {
                    try
                    {
                        string name = Path.GetFileName(folder);
                        string dest = Path.Combine(destFolder, name);
                        BackupDirectory(folder, dest, successWriter, failWriter);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        failWriter.WriteLine($"{folder}\r\nAccess denied\r\n");
                    }
                    catch (Exception e)
                    {
                        failWriter.WriteLine($"{sourceFolder}\r\nAccess {e.Message}\r\n");
                    }
                }
            }
        }

        public static Settings GetSettings()
        {
            try
            {
                byte[] ba = File.ReadAllBytes(ConfigFilePath);

                ba = AES_Decrypt(ba);

                string input = Encoding.UTF8.GetString(ba);

                Settings settings = new Settings();
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
                return settings;
            }
            catch
            {
                WriteLog("Error reading config file. Program terminated.");
                Environment.Exit(0);
            }

            return null;
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
