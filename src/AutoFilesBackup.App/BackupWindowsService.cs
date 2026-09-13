using System;
using System.ServiceProcess;
using System.Threading;
using AutoFilesBackup.Core;

namespace AutoFilesBackup.App
{
    public class BackupWindowsService : ServiceBase
    {
        private Timer timer;
        private LogManager logger;
        private int running;
        private DateTime lastAttempt = DateTime.MinValue;
        public BackupWindowsService()
        {
            ServiceName = WindowsServiceHelper.ServiceName;
            CanStop = true;
            AutoLog = true;
        }
        protected override void OnStart(string[] args)
        {
            timer = new Timer(Tick, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }
        private void Tick(object state)
        {
            if (Interlocked.CompareExchange(ref running, 1, 0) != 0) return;
            try
            {
                var config = ConfigManager.LoadConfig();
                logger = new LogManager(retentionDays: config.LogRetentionDays);
                TimeSpan scheduled;
                if (!TimeSpan.TryParseExact(config.DailyBackupTime, @"hh\:mm", System.Globalization.CultureInfo.InvariantCulture, out scheduled))
                    throw new InvalidOperationException("Invalid daily backup time.");
                var now = DateTime.Now;
                // Catch up after downtime and retry failures at most hourly.
                if (now.TimeOfDay < scheduled || now - lastAttempt < TimeSpan.FromHours(1)) return;
                if (config.LastBackupRunDate.HasValue && config.LastBackupRunDate.Value.Date == now.Date) return;
                lastAttempt = now;
                new BackupEngine(config, logger).ExecuteBackup();
            }
            catch (Exception ex) { (logger ?? new LogManager()).LogEvent("Service backup failed", ex.ToString()); }
            finally { Interlocked.Exchange(ref running, 0); }
        }
        protected override void OnStop()
        {
            // Wait for an active backup to finish rather than killing a replacement mid-run.
            if (timer != null)
            {
                using (var finished = new ManualResetEvent(false))
                {
                    timer.Dispose(finished);
                    while (!finished.WaitOne(1000)) RequestAdditionalTime(10000);
                }
            }
        }
    }
}
