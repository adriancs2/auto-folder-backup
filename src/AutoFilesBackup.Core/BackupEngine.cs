using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.AccessControl;

using System.Security.Principal;
using System.Threading;
using System.Web.Script.Serialization;

namespace AutoFilesBackup.Core
{
    // Also used by settings writes, so a running backup cannot have its state overwritten by the UI.
    public sealed class BackupOperationLock : IDisposable
    {
        private readonly Mutex mutex;
        public BackupOperationLock()
        {
            var security = new MutexSecurity();
            security.AddAccessRule(new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                MutexRights.Synchronize | MutexRights.Modify, AccessControlType.Allow));
            bool created;
            mutex = new Mutex(false, @"Global\AutoFilesBackup.Operation.v2", out created, security);
            bool acquired;
            try { acquired = mutex.WaitOne(0); }
            catch (AbandonedMutexException) { acquired = true; }
            if (!acquired) { mutex.Dispose(); throw new InvalidOperationException("Another backup or settings operation is running."); }
        }
        public void Dispose() { mutex.ReleaseMutex(); mutex.Dispose(); }
    }

    public interface IBackupStorage
    {
        bool IsReady(string letter);
        string Root(string letter);
        long FreeBytes(string letter);
        long CapacityBytes(string letter);
    }

    public sealed class WindowsBackupStorage : IBackupStorage
    {
        public bool IsReady(string letter) { return WmiDriveService.DriveExists(letter); }
        public string Root(string letter) { return letter + @":\"; }
        public long FreeBytes(string letter) { return WmiDriveService.GetFreeSpaceBytes(letter); }
        public long CapacityBytes(string letter) { return WmiDriveService.GetTotalSpaceBytes(letter); }
    }

    public sealed class BackupSetManifest
    {
        public string Format { get; set; } = "AutoFilesBackup.Set.v1";
        public string Source { get; set; }
        public DateTime CreatedUtc { get; set; }
        public bool FullCompleted { get; set; }
        public bool InProgress { get; set; }
        public DateTime? LastSuccessfulUtc { get; set; }
    }

    public class BackupEngine
    {
        public event Action<string> OnStatusMessage;
        public event Action<int, int> OnProgress;
        public BackupConfig Config { get; }
        public LogManager Logger { get; }
        private readonly IBackupStorage storage;
        private readonly Action<BackupConfig> save;
        private readonly Func<DateTime> utcNow;
        private const string SetsDirectory = "BackupSets";
        private sealed class BackupSet
        {
            public string Letter;
            public string Path;
            public BackupSetManifest Manifest;
            public string Data { get { return System.IO.Path.Combine(Path, "Data"); } }
        }

        public BackupEngine(BackupConfig config, LogManager logger)
            : this(config, logger, new WindowsBackupStorage(), ConfigManager.SaveConfig, () => DateTime.UtcNow) { }

        // Injectable storage and clock allow capacity / disconnect / weekly tests without formatting disks.
        public BackupEngine(BackupConfig config, LogManager logger, IBackupStorage storage,
            Action<BackupConfig> save, Func<DateTime> utcNow)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.storage = storage ?? throw new ArgumentNullException(nameof(storage));
            this.save = save ?? throw new ArgumentNullException(nameof(save));
            this.utcNow = utcNow ?? throw new ArgumentNullException(nameof(utcNow));
        }

        private void Status(string message, string details = "")
        {
            Logger.LogEvent(message, details);
            OnStatusMessage?.Invoke(message);
        }

        public static void ValidateConfiguration(BackupConfig config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            if (string.IsNullOrWhiteSpace(config.SourceFolder) || !Path.IsPathRooted(config.SourceFolder) || !Directory.Exists(config.SourceFolder))
                throw new DirectoryNotFoundException("Select an existing source folder.");
            string source = Canonical(config.SourceFolder);
            EnsureNoReparse(source);
            string sourceRoot = Path.GetPathRoot(source);
            string systemRoot = Path.GetPathRoot(Environment.SystemDirectory);
            string appRoot = Path.GetPathRoot(AppDomain.CurrentDomain.BaseDirectory);
            var letters = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string value in new[] { config.BackupDrive1, config.BackupDrive2 })
            {
                string letter = WmiDriveService.NormalizeDriveLetter(value);
                if (letter.Length == 0) continue;
                if (!letters.Add(letter)) throw new InvalidOperationException("Backup drives must be different.");
                string root = letter + @":\";
                if (string.Equals(root, sourceRoot, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(root, systemRoot, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(root, appRoot, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("A backup drive cannot contain the source, Windows, or this application.");
                var info = new DriveInfo(root);
                if (info.IsReady && info.DriveFormat != "NTFS")
                    throw new InvalidOperationException("Backup drives must use NTFS for safe file replacement.");
            }
            if (letters.Count == 0) throw new InvalidOperationException("Select at least one backup drive.");
            if (config.MaxIntervalDays < 1 || double.IsNaN(config.BufferMarginGb) ||
                double.IsInfinity(config.BufferMarginGb) || config.BufferMarginGb < 0 || config.BufferMarginGb > 1000000)
                throw new InvalidOperationException("Full backup interval must be positive and buffer must be a valid nonnegative size.");
            if (config.LogRetentionDays < 1) throw new InvalidOperationException("Log retention must be at least one day.");
            TimeSpan time;
            if (!TimeSpan.TryParseExact(config.DailyBackupTime, @"hh\:mm", CultureInfo.InvariantCulture, out time))
                throw new InvalidOperationException("Daily backup time must be HH:mm.");
        }

        private static string Canonical(string path) { return Path.GetFullPath(path).TrimEnd('\\', '/') + Path.DirectorySeparatorChar; }
        private static void EnsureNoReparse(string path)
        {
            string current = Path.GetFullPath(path);
            while (!string.IsNullOrEmpty(current))
            {
                if ((Directory.Exists(current) || File.Exists(current)) &&
                    (File.GetAttributes(current) & FileAttributes.ReparsePoint) != 0)
                    throw new IOException("Reparse points are not supported: " + current);
                current = Path.GetDirectoryName(current.TrimEnd('\\', '/'));
            }
        }

        // Both scans stream; any unreadable directory aborts preflight before recycling is considered.
        private IEnumerable<FileSystemInfo> Scan(string root)
        {
            var pending = new Stack<DirectoryInfo>();
            pending.Push(new DirectoryInfo(root));
            while (pending.Count > 0)
            {
                var dir = pending.Pop();
                EnsureNoReparse(dir.FullName);
                yield return dir;
                foreach (var file in dir.EnumerateFiles())
                {
                    EnsureNoReparse(file.FullName);
                    yield return file;
                }
                foreach (var child in dir.EnumerateDirectories())
                {
                    if (string.Equals(Canonical(dir.FullName), Path.GetPathRoot(dir.FullName), StringComparison.OrdinalIgnoreCase) &&
                        (child.Name.Equals("$RECYCLE.BIN", StringComparison.OrdinalIgnoreCase) ||
                        child.Name.Equals("System Volume Information", StringComparison.OrdinalIgnoreCase))) continue;
                    EnsureNoReparse(child.FullName);
                    pending.Push(child);
                }
            }
        }

        private List<BackupSet> ReadSets(IEnumerable<string> letters, string source)
        {
            var result = new List<BackupSet>();
            foreach (string letter in letters)
            {
                string root = Path.Combine(storage.Root(letter), SetsDirectory);
                EnsureNoReparse(root);
                if (!Directory.Exists(root)) continue;
                foreach (string dir in Directory.EnumerateDirectories(root, "set-*"))
                {
                    EnsureNoReparse(dir);
                    string marker = Path.Combine(dir, "set.json");
                    EnsureNoReparse(marker);
                    if (!File.Exists(marker)) continue;
                    // A damaged marker must not turn an existing backup into disposable data.
                    var manifest = new JavaScriptSerializer().Deserialize<BackupSetManifest>(File.ReadAllText(marker));
                    if (manifest == null || manifest.Format != "AutoFilesBackup.Set.v1") continue;
                    if (!string.Equals(manifest.Source, source, StringComparison.OrdinalIgnoreCase)) continue;
                    EnsureNoReparse(Path.Combine(dir, "Data"));
                    result.Add(new BackupSet { Letter = letter, Path = dir, Manifest = manifest });
                }
            }
            return result.OrderByDescending(s => s.Manifest.CreatedUtc).ToList();
        }

        private static void WriteManifest(BackupSet set)
        {
            ConfigManager.AtomicWrite(Path.Combine(set.Path, "Backup status.txt"),
                set.Manifest.InProgress ? "INCOMPLETE OR RUNNING\r\nSome files may be missing or out of date. Check the failed-files log.\r\n" :
                "COMPLETED\r\nRestore by copying the contents of the Data folder.\r\nLast successful update (UTC): " + set.Manifest.LastSuccessfulUtc + "\r\n");
            ConfigManager.AtomicWrite(Path.Combine(set.Path, "set.json"), new JavaScriptSerializer().Serialize(set.Manifest));
        }

        public void ExecuteBackup()
        {
            using (new BackupOperationLock()) ExecuteLocked();
        }

        private void ExecuteLocked()
        {
            ValidateConfiguration(Config);
            DateTime started = utcNow();
            string source = Canonical(Config.SourceFolder);
            var letters = new[] { Config.BackupDrive1, Config.BackupDrive2 }
                .Select(WmiDriveService.NormalizeDriveLetter).Where(s => s.Length > 0).ToList();
            var ready = letters.Where(storage.IsReady).ToList();
            if (ready.Count == 0) throw new IOException("No backup drive is available.");
            var sets = ReadSets(ready, source);
            var latest = sets.FirstOrDefault(s => s.Manifest.FullCompleted && Directory.Exists(s.Data));
            string active = latest == null ? WmiDriveService.NormalizeDriveLetter(Config.ActiveDriveLetter) : latest.Letter;
            // Do not treat an older drive as the current baseline after the active drive disappears.
            bool missingActive = !string.IsNullOrEmpty(Config.ActiveDriveLetter) &&
                letters.Contains(WmiDriveService.NormalizeDriveLetter(Config.ActiveDriveLetter)) &&
                !ready.Contains(WmiDriveService.NormalizeDriveLetter(Config.ActiveDriveLetter));
            bool full = latest == null || missingActive ||
                (started.ToLocalTime().Date - latest.Manifest.CreatedUtc.ToLocalTime().Date).TotalDays >= Config.MaxIntervalDays;
            long fullBytes = 0, updateBytes = 0;
            int fileCount = 0;
            Status("Scanning source", source);
            foreach (var entry in Scan(source))
            {
                var file = entry as FileInfo;
                if (file == null) continue;
                long allocation = checked(((file.Length + 4095) / 4096) * 4096);
                fullBytes = checked(fullBytes + allocation);
                fileCount++;
                if (!full && NeedsCopy(file.FullName))
                    updateBytes = checked(updateBytes + allocation);
            }
            long buffer = checked((long)(Config.BufferMarginGb * 1073741824.0));
            long requiredFull = checked(fullBytes + buffer);
            BackupSet destination = latest;
            if (!full && storage.FreeBytes(latest.Letter) < checked(updateBytes + buffer)) full = true;
            if (full)
            {
                var ordered = ready.OrderBy(l => l == active ? 0 : 1).ToList();
                string chosen = ordered.FirstOrDefault(l => storage.FreeBytes(l) >= requiredFull);
                if (chosen == null)
                {
                    // Protect the newest completed baseline. Never erase the only good set to make room.
                    foreach (string candidate in ordered.OrderBy(l => l == active ? 1 : 0))
                    {
                        if (storage.CapacityBytes(candidate) < requiredFull) continue;
                        var disposable = sets.Where(s => s.Letter == candidate && s != latest).Reverse().ToList();
                        long reclaimable = 0;
                        foreach (var old in disposable)
                            foreach (var entry in Scan(old.Path))
                                if (entry is FileInfo) reclaimable = checked(reclaimable + ((FileInfo)entry).Length);
                        if (checked(storage.FreeBytes(candidate) + reclaimable) < requiredFull) continue;
                        foreach (var old in disposable)
                        {
                            if (storage.FreeBytes(candidate) >= requiredFull) break;
                            // Scan immediately before deletion to reject links anywhere in the tree.
                            foreach (var entry in Scan(old.Path)) { }
                            Status("Recycling old backup set", old.Path);
                            Directory.Delete(old.Path, true);
                        }
                        if (storage.FreeBytes(candidate) >= requiredFull) { chosen = candidate; break; }
                    }
                }
                if (chosen == null) throw new IOException("Insufficient space for a full backup while preserving the latest completed set. Free space or add a backup drive.");
                string root = Path.Combine(storage.Root(chosen), SetsDirectory);
                EnsureNoReparse(root);
                string path = Path.Combine(root, "set-" + started.ToString("yyyy-MM-dd_HHmmss", CultureInfo.InvariantCulture) + "-" + Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(path);
                destination = new BackupSet { Letter = chosen, Path = path,
                    Manifest = new BackupSetManifest { Source = source, CreatedUtc = started } };
            }
            destination.Manifest.InProgress = true;
            WriteManifest(destination);
            Directory.CreateDirectory(destination.Data);
            Status(full ? "Full backup started" : "Daily update started", destination.Data);
            long successes = 0, failures = 0, skipped = 0, copiedBytes = 0;
            int processed = 0;
            try
            {
                foreach (var entry in Scan(source))
                {
                    string relative = entry.FullName.TrimEnd('\\').Length < source.Length ? "" : entry.FullName.Substring(source.Length);
                    string target = Path.Combine(destination.Data, relative);
                    try
                    {
                        EnsureNoReparse(target);
                        if (entry is DirectoryInfo) { Directory.CreateDirectory(target); continue; }
                        processed++;
                        OnProgress?.Invoke(processed, Math.Max(processed, fileCount));
                        if (!full && !NeedsCopy(entry.FullName))
                        {
                            skipped++;
                            Logger.LogSkippedFile(entry.FullName, "Archive attribute is not set");
                            continue;
                        }
                        Directory.CreateDirectory(Path.GetDirectoryName(target));
                        CopyFileSafely(entry.FullName, target);
                        successes++;
                        copiedBytes += new FileInfo(target).Length;
                        Logger.LogSuccessFile(entry.FullName);
                    }
                    catch (Exception ex) { failures++; Logger.LogFailedFile(entry.FullName, ex.Message); }
                }
            }
            catch (Exception ex) { failures++; Logger.LogFailedFile(source, "Source scan failed: " + ex.Message); }
            Logger.LogStatistic(processed, successes, failures, skipped, fullBytes, full ? 0 : copiedBytes,
                utcNow() - started, full ? "FULL" : "INCREMENTAL", destination.Letter);
            if (failures > 0)
            {
                Status("Backup incomplete", failures + " errors; previous backups retained. See failed_files.log.");
                throw new IOException("Backup incomplete: " + failures + " errors. The set remains marked InProgress.");
            }
            destination.Manifest.FullCompleted = true;
            destination.Manifest.InProgress = false;
            destination.Manifest.LastSuccessfulUtc = utcNow();
            WriteManifest(destination);
            Config.ActiveDriveLetter = destination.Letter;
            Config.LastFullBackupDate = destination.Manifest.CreatedUtc.ToLocalTime();
            Config.LastBackupRunDate = utcNow().ToLocalTime();
            save(Config);
            Status("Backup completed", destination.Data);
        }

        public static bool NeedsCopy(string source)
        {
            return (File.GetAttributes(source) & FileAttributes.Archive) != 0;
        }
        public static void CopyFileSafely(string source, string destination)
        {
            EnsureNoReparse(source);
            EnsureNoReparse(destination);
            string temporary = Path.Combine(Path.GetDirectoryName(destination), ".afb-" + Guid.NewGuid().ToString("N") + ".tmp");
            try
            {
                // Deny writers/deleters while reading. Locked files fail instead of producing a mixed snapshot.
                using (var input = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var modified = File.GetLastWriteTimeUtc(source);
                    var created = File.GetCreationTimeUtc(source);
                    using (var output = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                    {
                        input.CopyTo(output);
                        output.Flush(true);
                    }
                    File.SetCreationTimeUtc(temporary, created);
                    File.SetLastWriteTimeUtc(temporary, modified);
                    if (File.Exists(destination))
                    {
                        FileAttributes attributes = File.GetAttributes(destination);
                        bool readOnly = (attributes & FileAttributes.ReadOnly) != 0;
                        if (readOnly) File.SetAttributes(destination, attributes & ~FileAttributes.ReadOnly);
                        try { File.Replace(temporary, destination, null); }
                        catch
                        {
                            if (readOnly && File.Exists(destination)) File.SetAttributes(destination, attributes);
                            throw;
                        }
                    }
                    else File.Move(temporary, destination);
                    // Clear only after committing the destination, while the source still denies writers.
                    File.SetAttributes(source, File.GetAttributes(source) & ~FileAttributes.Archive);
                }
            }
            finally { if (File.Exists(temporary)) File.Delete(temporary); }
        }
    }
}

