using System;
using System.IO;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Windows.Forms;
using AutoFilesBackup.Core;

namespace AutoFilesBackup.App
{
    static class Program
    {
        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;

        [DllImport("kernel32.dll")]
        static extern bool AllocConsole();

        [STAThread]
        static void Main(string[] args)
        {
            // Check arguments
            if (args != null && args.Length > 0)
            {
                string cmd = args[0].Trim().ToLowerInvariant();

                if (cmd == "--service" || cmd == "-s")
                {
                    // Run as Windows Service
                    ServiceBase.Run(new BackupWindowsService());
                    return;
                }

                if (cmd == "--run" || cmd == "-r" || cmd == "/run")
                {
                    // Attach or allocate console for CLI visibility
                    if (!AttachConsole(ATTACH_PARENT_PROCESS))
                    {
                        AllocConsole();
                    }

                    Console.WriteLine("=================================================");
                    Console.WriteLine("Auto Files Backup - Automated Execution Routine");
                    Console.WriteLine($"Starting at: {DateTime.Now}");
                    Console.WriteLine("=================================================");

                    try
                    {
                        var config = ConfigManager.LoadConfig();
                        var logger = new LogManager(retentionDays: config.LogRetentionDays);
                        var engine = new BackupEngine(config, logger);

                        engine.OnStatusMessage += (msg) => Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {msg}");
                        engine.ExecuteBackup();

                        Console.WriteLine("Backup executed successfully.");
                        Environment.Exit(0);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine("FATAL ERROR DURING BACKUP: " + ex);
                        Environment.Exit(1);
                    }
                    return;
                }

                if (cmd == "--install-task")
                {
                    var config = ConfigManager.LoadConfig();
                    TaskSchedulerHelper.CreateOrUpdateTask(Application.ExecutablePath, config.DailyBackupTime);
                    return;
                }

                if (cmd == "--remove-task")
                {
                    TaskSchedulerHelper.DeleteTask();
                    return;
                }

                if (cmd == "--install-service")
                {
                    WindowsServiceHelper.InstallService(Application.ExecutablePath);
                    return;
                }

                if (cmd == "--uninstall-service")
                {
                    WindowsServiceHelper.UninstallService();
                    return;
                }
            }

            // Default: Launch GUI WinForms
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}

