using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Properties;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32.TaskScheduler;
using System.Text.RegularExpressions;


namespace System
{
    public partial class Form1 : Form
    {
        string ConfigFilePath = "";
        static string BasicLogFilePath = "";
        BackgroundWorker bw = null;

        public Form1()
        {
            ConfigFilePath = Path.Combine(Application.StartupPath, "config");
            BasicLogFilePath = Path.Combine(Application.StartupPath, "log");

            bw = new BackgroundWorker();
            bw.RunWorkerCompleted += Bw_RunWorkerCompleted;
            bw.DoWork += Bw_DoWork;

            InitializeComponent();

            LoadSettings();
        }

        void LoadSettings()
        {
            using (TaskService ts = new TaskService())
            {
                // Check if the task already exists
                var existingTask = ts.GetTask("Auto Folder Backup");
                if (existingTask != null)
                {
                    var triggerTime = existingTask.Definition.Triggers[0].StartBoundary;
                    lbTaskSchedulerStatus.Text = $"installed ({(existingTask.Enabled ? "enabled" : "disabled")})";
                    nmTaskHour.Value = triggerTime.Hour;
                    nmTaskMinute.Value = triggerTime.Minute;
                }
                else
                {
                    lbTaskSchedulerStatus.Text = "none - not installed yet";
                    nmTaskHour.Value = 3;
                    nmTaskMinute.Value = 0;
                }
            }

            flowLayoutPanel1.Controls.Clear();

            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo drive in allDrives)
            {
                string driveLetter = drive.Name[0].ToString();

                if (driveLetter == "C" || driveLetter == "D")
                {
                    continue;
                }

                CheckBox cb = new CheckBox();
                cb.Width = 60;
                cb.Text = $"{driveLetter}:\\";

                flowLayoutPanel1.Controls.Add(cb);
            }

            if (File.Exists(ConfigFilePath))
            {
                var settings = GetSettings();

                txtFolder.Text = settings.TargetFolder;
                nmTotalDays.Value = settings.TotalDaysForFullBackup;

                string[] myDrives = settings.BackupDriveLetters.Split(new char[] { ',', '|' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var drive in myDrives)
                {
                    foreach (var c in flowLayoutPanel1.Controls)
                    {
                        if (c is CheckBox)
                        {
                            var cb = (CheckBox)c;

                            if (cb.Text[0].ToString() == drive)
                            {
                                cb.Checked = true;
                                continue;
                            }
                        }
                    }
                }
            }
        }

        public Settings GetSettings()
        {
            if (!File.Exists(ConfigFilePath))
            {
                return new Settings();
            }

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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return null;
        }

        public byte[] AES_Encrypt(byte[] bytesToBeEncrypted)
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

        public byte[] AES_Decrypt(byte[] bytesToBeDecrypted)
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

        private void btOpenLogFile_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo()
                {
                    FileName = BasicLogFilePath,
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btSelectFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtFolder.Text = dialog.SelectedPath;
            }
        }

        private void btReload_Click(object sender, EventArgs e)
        {
            LoadSettings();
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            StringBuilder backupDriveLetters = new StringBuilder();
            foreach (Control c in flowLayoutPanel1.Controls)
            {
                if (c is CheckBox cb && cb.Checked)
                {
                    if (backupDriveLetters.Length > 0)
                        backupDriveLetters.Append(',');
                    backupDriveLetters.Append(cb.Text[0]);
                }
            }

            string output = $"TargetFolder={txtFolder.Text}\r\nBackupDriveLetters={backupDriveLetters.ToString()}\r\nTotalDaysForFullBackup={(int)nmTotalDays.Value}";
            byte[] ba = Encoding.UTF8.GetBytes(output);
            ba = AES_Encrypt(ba);
            File.WriteAllBytes(ConfigFilePath, ba);

            MessageBox.Show("Saved");
        }

        private void btRun_Click(object sender, EventArgs e)
        {
            if (bw.IsBusy)
            {
                MessageBox.Show("auto_folder_backup.exe is currently busy doing it's work, please hold on...", "Backup is running");
                return;
            }
            else
            {
                MessageBox.Show("auto_folder_backup.exe will now begin to run", "Start Backup");
                bw.RunWorkerAsync();
            }
        }

        private void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            string exefile = Path.Combine(Application.StartupPath, "auto_folder_backup.exe");

            using (Process process = new Process())
            {
                process.StartInfo.FileName = exefile;
                process.Start();
                process.WaitForExit();
            }
        }

        private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("The backup process has ended. You may [open log file] for the work result status (either success or fail).");
        }

        private void btDeleteTaskScheduler_Click(object sender, EventArgs e)
        {
            // Get the service on the local machine
            using (TaskService ts = new TaskService())
            {
                // Get the tasks that match the regex
                var tasks = ts.RootFolder.GetTasks(new Regex("Auto Folder Backup"));

                // Delete the tasks
                foreach (var task in tasks)
                {
                    ts.RootFolder.DeleteTask(task.Name);
                }
            }

            LoadSettings();

            MessageBox.Show("Task Scheduler deleted");
        }

        private void btCreateTaskScheduler_Click(object sender, EventArgs e)
        {
            // Get the service on the local machine
            using (TaskService ts = new TaskService())
            {
                // Check if the task already exists
                var existingTask = ts.GetTask("Auto Folder Backup");
                if (existingTask != null)
                {
                    var sb = existingTask.Definition.Triggers[0].StartBoundary;
                    MessageBox.Show($"The task 'Auto Folder Backup' already exists\r\n\r\nExecution Time: {sb.ToString("HH:mm")}.\r\n\r\nPlease delete it first before attempting to create it again.", "Task Exists", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Create a new task definition and assign properties
                TaskDefinition td = ts.NewTask();
                td.RegistrationInfo.Description = "Automated folder backup task";
                td.Principal.RunLevel = TaskRunLevel.Highest;  // Run with the highest privileges

                // Create a trigger that will fire every day at 3AM
                DateTime triggertime = DateTime.Today.AddHours((int)nmTaskHour.Value).AddMinutes((int)nmTaskMinute.Value);
                td.Triggers.Add(new DailyTrigger { StartBoundary = triggertime });

                // Create an action that will launch the program whenever the trigger fires
                td.Actions.Add(new ExecAction(@"D:\auto_folder_backup\auto_folder_backup.exe", null, null));

                // Register the task in the root folder
                ts.RootFolder.RegisterTaskDefinition(@"Auto Folder Backup", td,
                    TaskCreation.CreateOrUpdate,
                    "SYSTEM",   // Specify the "SYSTEM" user (or an administrator username)
                    null,       // No password is needed when using the SYSTEM user
                    TaskLogonType.ServiceAccount,
                    null);      // No SDDL-defined security descriptor needed
            }

            LoadSettings();

            MessageBox.Show("Done");
        }

        bool beginMoveForm = false;
        Point startMousePosition;
        Point oriFormPosition;

        private void lbFormTitle_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                beginMoveForm = true;
                startMousePosition = Control.MousePosition;  // Store the mouse position on screen at the time of the mouse down event
                oriFormPosition = this.Location;  // Store the original form position
            }
        }

        private void lbFormTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (beginMoveForm)
            {
                // Get the current mouse position on screen
                Point currentMousePosition = Control.MousePosition;

                // Calculate how much the mouse has moved
                int deltaX = currentMousePosition.X - startMousePosition.X;
                int deltaY = currentMousePosition.Y - startMousePosition.Y;

                // Compute the new form position
                this.Location = new Point(oriFormPosition.X + deltaX, oriFormPosition.Y + deltaY);
            }
        }

        private void lbFormTitle_MouseUp(object sender, MouseEventArgs e)
        {
            beginMoveForm = false;  // Stop moving the form when the mouse button is released
        }

        private void btMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

    public class Settings
    {
        public string TargetFolder = "";
        public string BackupDriveLetters = "";
        public int TotalDaysForFullBackup = 0;
    }
}
