using System;
using System.Diagnostics;
using System.IO;

namespace AutoFilesBackup.Core
{
    public static class TaskSchedulerHelper
    {
        public const string TaskName = "AutoFilesBackupTask";

        public static bool CreateOrUpdateTask(string exePath, string dailyTime = "03:00")
        {
            if (string.IsNullOrEmpty(exePath) || !File.Exists(exePath))
            {
                throw new FileNotFoundException("Executable file not found: " + exePath);
            }

            // Using schtasks.exe with elevated highest run level
            // schtasks /create /tn "AutoFilesBackupTask" /tr "\"C:\path\AutoFilesBackup.exe\" --run" /sc daily /st 03:00 /rl HIGHEST /f
            TimeSpan time;
            if (!TimeSpan.TryParseExact(dailyTime, @"hh\:mm", System.Globalization.CultureInfo.InvariantCulture, out time))
                throw new ArgumentException("Daily time must be HH:mm.");
            string arguments = $"/create /tn \"{TaskName}\" /tr \"\\\"{exePath}\\\" --run\" /sc daily /st {dailyTime} /ru SYSTEM /rl HIGHEST /f";

            var psi = new ProcessStartInfo
            {
                FileName = "schtasks.exe",
                Arguments = arguments,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (var proc = Process.Start(psi))
            {
                var outputTask = proc.StandardOutput.ReadToEndAsync();
                var errorTask = proc.StandardError.ReadToEndAsync();
                proc.WaitForExit();
                string output = outputTask.Result;
                string err = errorTask.Result;
                if (proc.ExitCode != 0)
                {
                    throw new InvalidOperationException($"schtasks failed (Code {proc.ExitCode}): {err} {output}");
                }
                return true;
            }
        }

        public static bool DeleteTask()
        {
            var psi = new ProcessStartInfo
            {
                FileName = "schtasks.exe",
                Arguments = $"/delete /tn \"{TaskName}\" /f",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (var proc = Process.Start(psi))
            {
                var output = proc.StandardOutput.ReadToEndAsync();
                var error = proc.StandardError.ReadToEndAsync();
                proc.WaitForExit();
                System.Threading.Tasks.Task.WaitAll(output, error);
                return proc.ExitCode == 0;
            }
        }

        public static bool IsTaskScheduled()
        {
            var psi = new ProcessStartInfo
            {
                FileName = "schtasks.exe",
                Arguments = $"/query /tn \"{TaskName}\"",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (var proc = Process.Start(psi))
            {
                var output = proc.StandardOutput.ReadToEndAsync();
                var error = proc.StandardError.ReadToEndAsync();
                proc.WaitForExit();
                System.Threading.Tasks.Task.WaitAll(output, error);
                return proc.ExitCode == 0;
            }
        }
    }
}
