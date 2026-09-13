using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;

namespace AutoFilesBackup.Core
{
    public static class WindowsServiceHelper
    {
        public const string ServiceName = "AutoFilesBackupService";
        public const string DisplayName = "Auto Files Backup Automation Service";

        public static bool IsServiceInstalled()
        {
            try
            {
                using (var sc = new ServiceController(ServiceName))
                {
                    var status = sc.Status;
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool InstallService(string exePath, string username = null, string password = null)
        {
            if (string.IsNullOrEmpty(exePath) || !File.Exists(exePath))
            {
                throw new FileNotFoundException("Executable not found: " + exePath);
            }

            // sc.exe create AutoFilesBackupService binPath= "\"C:\path\app.exe\" --service" start= auto
            string binPath = $"\"{exePath}\" --service";
            string args = $"create \"{ServiceName}\" binPath= \"\\\"{exePath}\\\" --service\" DisplayName= \"{DisplayName}\" start= auto";

            if (!string.IsNullOrEmpty(username))
            {
                args += $" obj= \"{username}\"";
                if (!string.IsNullOrEmpty(password))
                {
                    args += $" password= \"{password}\"";
                }
            }

            RunSc(args);

            // Set description
            RunSc($"description \"{ServiceName}\" \"Automatic Enterprise Daily Incremental & Full Backup Service\"");

            // Start the service
            try
            {
                RunSc($"start \"{ServiceName}\"");
            }
            catch (Exception ex) { throw new InvalidOperationException("Service installed but failed to start.", ex); }

            return true;
        }

        public static bool UninstallService()
        {
            try
            {
                RunSc($"stop \"{ServiceName}\"");
            }
            catch { }

            try
            {
                RunSc($"delete \"{ServiceName}\"");
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void RunSc(string arguments)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "sc.exe",
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
                if (proc.ExitCode != 0 && proc.ExitCode != 1056) // 1056 = service already running
                {
                    throw new InvalidOperationException($"SC command failed ({proc.ExitCode}): {err} {output}");
                }
            }
        }
    }
}
