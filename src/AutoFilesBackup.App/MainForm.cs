using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutoFilesBackup.Core;

namespace AutoFilesBackup.App
{
    public class MainForm : Form
    {
        private TextBox txtSourceFolder;
        private Button btnBrowseSource;
        private ComboBox cmbDrive1;
        private ComboBox cmbDrive2;
        private NumericUpDown numMaxIntervalDays;
        private DateTimePicker dtpDailyTime;
        private NumericUpDown numBufferGb;

        private Button btnSaveConfig;
        private Button btnRunNow;
        private Button btnInstallTask;
        private Button btnRemoveTask;
        private Button btnInstallService;
        private Button btnRemoveService;
        private Button btnOpenLogs;

        private ProgressBar progressBar;
        private RichTextBox rtbLog;
        private Label lblStatus;

        private BackupConfig _config;
        private bool backupRunning;

        public MainForm()
        {
            InitializeComponents();
            FormClosing += (sender, e) => { if (backupRunning) { e.Cancel = true; MessageBox.Show("Wait for the backup to finish before closing."); } };
            try { LoadConfiguration(); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Configuration error", MessageBoxButtons.OK, MessageBoxIcon.Error); _config = new BackupConfig(); }
            RefreshStatus();
        }

        private void InitializeComponents()
        {
            this.Text = "Auto Files Backup - Enterprise File & Drive Automation";
            this.Size = new Size(880, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(820, 650);

            // Load Application Icon if available
            string icoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.ico");
            if (!File.Exists(icoPath))
            {
                icoPath = @"D:\aifiles\Auto-Files-Backup\app.ico";
            }
            if (File.Exists(icoPath))
            {
                try { this.Icon = new Icon(icoPath); } catch { }
            }

            var fontRegular = new Font("Segoe UI", 9.5f, FontStyle.Regular);
            var fontBold = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            this.Font = fontRegular;

            // Header Banner
            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(28, 44, 74)
            };
            var lblTitle = new Label
            {
                Text = "AUTO FILES BACKUP - WEEKLY SETS & DAILY UPDATES",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                Location = new Point(18, 16),
                AutoSize = true
            };
            pnlHeader.Controls.Add(lblTitle);
            this.Controls.Add(pnlHeader);

            // Main Container
            var mainContainer = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15)
            };
            this.Controls.Add(mainContainer);

            // GroupBox: Configuration
            var grpConfig = new GroupBox
            {
                Text = "Backup & Drive Settings",
                Font = fontBold,
                Location = new Point(15, 70),
                Size = new Size(835, 175)
            };
            mainContainer.Controls.Add(grpConfig);

            // Source Folder
            var lblSource = new Label { Text = "Source Folder:", Font = fontRegular, Location = new Point(15, 28), AutoSize = true };
            txtSourceFolder = new TextBox { Location = new Point(135, 25), Size = new Size(570, 26), Font = fontRegular };
            btnBrowseSource = new Button { Text = "Browse...", Location = new Point(715, 24), Size = new Size(100, 28), Font = fontRegular };
            btnBrowseSource.Click += (s, e) =>
            {
                using (var fbd = new FolderBrowserDialog())
                {
                    fbd.SelectedPath = txtSourceFolder.Text;
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        txtSourceFolder.Text = fbd.SelectedPath;
                    }
                }
            };
            grpConfig.Controls.AddRange(new Control[] { lblSource, txtSourceFolder, btnBrowseSource });

            // Drives
            var lblDrive1 = new Label { Text = "Backup Drive 1:", Font = fontRegular, Location = new Point(15, 65), AutoSize = true };
            cmbDrive1 = new ComboBox { Location = new Point(135, 62), Size = new Size(120, 26), DropDownStyle = ComboBoxStyle.DropDownList, Font = fontRegular };
            var lblDrive2 = new Label { Text = "Backup Drive 2:", Font = fontRegular, Location = new Point(290, 65), AutoSize = true };
            cmbDrive2 = new ComboBox { Location = new Point(410, 62), Size = new Size(120, 26), DropDownStyle = ComboBoxStyle.DropDownList, Font = fontRegular };
            
            PopulateDriveDropdowns();
            grpConfig.Controls.AddRange(new Control[] { lblDrive1, cmbDrive1, lblDrive2, cmbDrive2 });

            // Max Interval & Daily Time & Buffer
            var lblMaxInterval = new Label { Text = "Full Backup Interval (Days):", Font = fontRegular, Location = new Point(15, 102), AutoSize = true };
            numMaxIntervalDays = new NumericUpDown { Location = new Point(210, 100), Size = new Size(65, 26), Minimum = 1, Maximum = 365, Value = 7, Font = fontRegular };

            var lblDailyTime = new Label { Text = "Daily Backup Time:", Font = fontRegular, Location = new Point(290, 102), AutoSize = true };
            dtpDailyTime = new DateTimePicker { Location = new Point(430, 100), Size = new Size(100, 26), Format = DateTimePickerFormat.Custom, CustomFormat = "HH:mm", ShowUpDown = true, Font = fontRegular };

            var lblBuffer = new Label { Text = "Safety Buffer (GB):", Font = fontRegular, Location = new Point(555, 102), AutoSize = true };
            numBufferGb = new NumericUpDown { Location = new Point(690, 100), Size = new Size(60, 26), Minimum = 0, Maximum = 500, DecimalPlaces = 2, Increment = 0.1M, Value = 5, Font = fontRegular };

            btnSaveConfig = new Button { Text = "Save Settings", Location = new Point(135, 135), Size = new Size(140, 32), Font = fontBold, BackColor = Color.FromArgb(220, 235, 252) };
            btnSaveConfig.Click += BtnSaveConfig_Click;

            grpConfig.Controls.AddRange(new Control[] { lblMaxInterval, numMaxIntervalDays, lblDailyTime, dtpDailyTime, lblBuffer, numBufferGb, btnSaveConfig });

            // GroupBox: Automation & Launch Strategies
            var grpAuto = new GroupBox
            {
                Text = "Auto-Start Automation Strategies & Manual Trigger",
                Font = fontBold,
                Location = new Point(15, 255),
                Size = new Size(835, 105)
            };
            mainContainer.Controls.Add(grpAuto);

            btnInstallTask = new Button { Text = "1-Click Task Scheduler", Location = new Point(15, 28), Size = new Size(180, 32), Font = fontRegular };
            btnInstallTask.Click += BtnInstallTask_Click;

            btnRemoveTask = new Button { Text = "Remove Task", Location = new Point(15, 65), Size = new Size(180, 28), Font = fontRegular };
            btnRemoveTask.Click += BtnRemoveTask_Click;

            btnInstallService = new Button { Text = "1-Click Windows Service", Location = new Point(210, 28), Size = new Size(190, 32), Font = fontRegular };
            btnInstallService.Click += BtnInstallService_Click;

            btnRemoveService = new Button { Text = "Remove Service", Location = new Point(210, 65), Size = new Size(190, 28), Font = fontRegular };
            btnRemoveService.Click += BtnRemoveService_Click;

            btnRunNow = new Button { Text = "RUN BACKUP NOW", Location = new Point(430, 28), Size = new Size(180, 65), Font = fontBold, BackColor = Color.FromArgb(40, 167, 69), ForeColor = Color.White };
            btnRunNow.Click += BtnRunNow_Click;

            btnOpenLogs = new Button { Text = "Open Log Directory", Location = new Point(630, 28), Size = new Size(180, 65), Font = fontBold, BackColor = Color.FromArgb(240, 240, 240) };
            btnOpenLogs.Click += (s, e) =>
            {
                string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                if (!Directory.Exists(logDir)) Directory.CreateDirectory(logDir);
                System.Diagnostics.Process.Start("explorer.exe", logDir);
            };

            grpAuto.Controls.AddRange(new Control[] { btnInstallTask, btnRemoveTask, btnInstallService, btnRemoveService, btnRunNow, btnOpenLogs });

            // GroupBox: Progress & Live Logs
            var grpProgress = new GroupBox
            {
                Text = "Live Activity & Status (120-Day Retention Multi-Log)",
                Font = fontBold,
                Location = new Point(15, 370),
                Size = new Size(835, 290)
            };
            mainContainer.Controls.Add(grpProgress);

            lblStatus = new Label { Text = "Ready.", Location = new Point(15, 25), AutoSize = true, Font = fontRegular, ForeColor = Color.DarkBlue };
            progressBar = new ProgressBar { Location = new Point(15, 50), Size = new Size(805, 22) };
            rtbLog = new RichTextBox { Location = new Point(15, 80), Size = new Size(805, 195), ReadOnly = true, BackColor = Color.FromArgb(248, 249, 250), Font = new Font("Consolas", 9f) };

            grpProgress.Controls.AddRange(new Control[] { lblStatus, progressBar, rtbLog });
        }

        private void PopulateDriveDropdowns()
        {
            cmbDrive1.Items.Clear();
            cmbDrive2.Items.Clear();

            cmbDrive1.Items.Add("(None)");
            cmbDrive2.Items.Add("(None)");

            foreach (var d in DriveInfo.GetDrives())
            {
                if (d.DriveType == DriveType.Fixed || d.DriveType == DriveType.Removable)
                {
                    string item = $"{d.Name[0]}:";
                    cmbDrive1.Items.Add(item);
                    cmbDrive2.Items.Add(item);
                }
            }
        }

        private void LoadConfiguration()
        {
            _config = ConfigManager.LoadConfig();
            txtSourceFolder.Text = _config.SourceFolder;

            string d1 = WmiDriveService.NormalizeDriveLetter(_config.BackupDrive1);
            string d2 = WmiDriveService.NormalizeDriveLetter(_config.BackupDrive2);

            SelectDropdown(cmbDrive1, d1);
            SelectDropdown(cmbDrive2, d2);

            numMaxIntervalDays.Value = Math.Max(1, Math.Min(365, _config.MaxIntervalDays));
            if (DateTime.TryParseExact(_config.DailyBackupTime, "HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime parsedTime))
            {
                dtpDailyTime.Value = parsedTime;
            }
            numBufferGb.Value = (decimal)Math.Max((double)numBufferGb.Minimum, Math.Min((double)numBufferGb.Maximum, _config.BufferMarginGb));
        }

        private void SelectDropdown(ComboBox cmb, string letter)
        {
            if (string.IsNullOrEmpty(letter))
            {
                cmb.SelectedIndex = 0;
                return;
            }
            string target = $"{letter}:";
            int idx = cmb.Items.IndexOf(target);
            if (idx >= 0)
            {
                cmb.SelectedIndex = idx;
            }
            else
            {
                cmb.Items.Add(target);
                cmb.SelectedItem = target;
            }
        }

        private bool SaveSettings(bool showSuccess)
        {
            try
            {
                using (new BackupOperationLock())
                {
                    // Reload runtime state to avoid overwriting a service run with stale UI state.
                    var next = ConfigManager.LoadConfig();
                    string oldSource = next.SourceFolder;
                    string oldDrive1 = next.BackupDrive1;
                    string oldDrive2 = next.BackupDrive2;
                    next.SourceFolder = txtSourceFolder.Text.Trim();
                    next.BackupDrive1 = cmbDrive1.SelectedItem?.ToString() == "(None)" ? "" : cmbDrive1.SelectedItem?.ToString();
                    next.BackupDrive2 = cmbDrive2.SelectedItem?.ToString() == "(None)" ? "" : cmbDrive2.SelectedItem?.ToString();
                    next.MaxIntervalDays = (int)numMaxIntervalDays.Value;
                    next.DailyBackupTime = dtpDailyTime.Value.ToString("HH:mm");
                    next.BufferMarginGb = (double)numBufferGb.Value;
                    if (!string.Equals(oldSource, next.SourceFolder, StringComparison.OrdinalIgnoreCase) ||
                        WmiDriveService.NormalizeDriveLetter(oldDrive1) != WmiDriveService.NormalizeDriveLetter(next.BackupDrive1) ||
                        WmiDriveService.NormalizeDriveLetter(oldDrive2) != WmiDriveService.NormalizeDriveLetter(next.BackupDrive2))
                    {
                        next.LastBackupRunDate = null;
                        next.LastFullBackupDate = null;
                        next.ActiveDriveLetter = WmiDriveService.NormalizeDriveLetter(next.BackupDrive1);
                    }
                    ConfigManager.SaveConfig(next);
                    _config = next;
                }
                if (showSuccess) MessageBox.Show("Configuration saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving configuration: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void BtnSaveConfig_Click(object sender, EventArgs e) { SaveSettings(true); }
        private void RefreshStatus()
        {
            bool taskInstalled = TaskSchedulerHelper.IsTaskScheduled();
            btnInstallTask.Enabled = !taskInstalled;
            btnRemoveTask.Enabled = taskInstalled;

            bool serviceInstalled = WindowsServiceHelper.IsServiceInstalled();
            btnInstallService.Enabled = !serviceInstalled;
            btnRemoveService.Enabled = serviceInstalled;
        }

        private void BtnInstallTask_Click(object sender, EventArgs e)
        {
            try
            {
                if (!SaveSettings(false)) return;
                string exePath = Application.ExecutablePath;
                string time = dtpDailyTime.Value.ToString("HH:mm");
                TaskSchedulerHelper.CreateOrUpdateTask(exePath, time);
                MessageBox.Show($"Task Scheduler created successfully! Configured to run daily at {time}.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to create scheduled task: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRemoveTask_Click(object sender, EventArgs e)
        {
            try
            {
                if (TaskSchedulerHelper.DeleteTask())
                {
                    MessageBox.Show("Scheduled task removed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                RefreshStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to remove task: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnInstallService_Click(object sender, EventArgs e)
        {
            using (var credForm = new ServiceCredentialsForm())
            {
                if (credForm.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        if (!SaveSettings(false)) return;
                string exePath = Application.ExecutablePath;
                        WindowsServiceHelper.InstallService(exePath, credForm.Username, credForm.Password);
                        MessageBox.Show("Windows Service installed and started successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        RefreshStatus();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to install Windows Service: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnRemoveService_Click(object sender, EventArgs e)
        {
            try
            {
                if (WindowsServiceHelper.UninstallService())
                {
                    MessageBox.Show("Windows Service stopped and uninstalled successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                RefreshStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to uninstall service: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnRunNow_Click(object sender, EventArgs e)
        {
            btnRunNow.Enabled = false;
            rtbLog.Clear();
            progressBar.Value = 0;

            if (!SaveSettings(false)) { btnRunNow.Enabled = true; return; }

            var logger = new LogManager(retentionDays: _config.LogRetentionDays);
            var engine = new BackupEngine(_config, logger);

            engine.OnStatusMessage += (msg) =>
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        lblStatus.Text = msg;
                        rtbLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}\n");
                        rtbLog.ScrollToCaret();
                    }));
                }
                else
                {
                    lblStatus.Text = msg;
                    rtbLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}\n");
                    rtbLog.ScrollToCaret();
                }
            };

            engine.OnProgress += (curr, total) =>
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        progressBar.Maximum = total;
                        progressBar.Value = Math.Min(curr, total);
                    }));
                }
                else
                {
                    progressBar.Maximum = total;
                    progressBar.Value = Math.Min(curr, total);
                }
            };

            try
            {
                backupRunning = true;
                await Task.Run(() => engine.ExecuteBackup());
                MessageBox.Show("Backup process completed successfully!", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Backup encountered an error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                backupRunning = false;
                btnRunNow.Enabled = true;
            }
        }
    }
}


