using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace auto_folder_backup_debug_helper
{
    public partial class Form1 : Form
    {
        int totalFiles = 0;
        int targetFiles = 0;
        int intervalcount = 0;
        int curindex = 0;

        string fileconfig
        {
            get
            {
                return Path.Combine(Application.StartupPath, "last_used_folder.txt");
            }
        }

        string percentConfig
        {
            get
            {
                return Path.Combine(Application.StartupPath, "last_used_percent.txt");
            }
        }

        string error = "";
        string targetFolder = "";
        int percent = 0;
        int totalFilesSet = 0;

        BackgroundWorker bw = new BackgroundWorker();

        public Form1()
        {
            InitializeComponent();

            if (File.Exists(fileconfig))
            {
                txtFolder.Text = File.ReadAllText(fileconfig);
            }
            
            if (File.Exists(percentConfig))
            {
                if (int.TryParse(File.ReadAllText(percentConfig), out int savedPercent))
                {
                    nmPercent.Value = savedPercent;
                }
            }

            bw.DoWork += Bw_DoWork;
            bw.RunWorkerCompleted += Bw_RunWorkerCompleted;
        }

        private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show(error + "\r\n\r\nTotal Files: " + totalFiles + "\r\nTotal Files Set: " + totalFilesSet);
        }

        private void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                totalFiles = 0;

                DirectoryInfo dir = new DirectoryInfo(targetFolder);

                try
                {
                    CountFiles(dir);
                }
                catch { }

                if (totalFiles == 0)
                    return;

                targetFiles = totalFiles * percent / 100;
                intervalcount = totalFiles / (totalFiles - targetFiles);
                if (intervalcount < 1)
                    intervalcount = 1;

                try
                {
                    RunFileAttribute(dir);
                }
                catch { }

                error = "done";
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
        }

        private void btFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog f = new FolderBrowserDialog();

            if (f.ShowDialog() == DialogResult.OK)
            {
                txtFolder.Text = f.SelectedPath;
                File.WriteAllText(fileconfig, txtFolder.Text);
            }
        }

        private void btRun_Click(object sender, EventArgs e)
        {
            targetFolder = txtFolder.Text;

            if (bw.IsBusy)
            {
                MessageBox.Show("busy, please wait");
                return;
            }

            totalFilesSet = 0;

            percent = (int)nmPercent.Value;
            SavePercentValue(percent);

            bw.RunWorkerAsync();
        }

        private void SavePercentValue(int value)
        {
            try
            {
                File.WriteAllText(percentConfig, value.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving percent value: " + ex.Message);
            }
        }

        void CountFiles(DirectoryInfo dir)
        {
            try
            {
                foreach (FileInfo file in dir.EnumerateFiles())
                {
                    try
                    {
                        totalFiles++;
                    }
                    catch { }
                }
            }
            catch { }

            foreach (DirectoryInfo subDir in dir.EnumerateDirectories())
            {
                try
                {
                    CountFiles(subDir);
                }
                catch { }
            }
        }

        void RunFileAttribute(DirectoryInfo dir)
        {
            foreach (FileInfo file in dir.EnumerateFiles())
            {
                try
                {
                    if (totalFilesSet >= targetFiles)
                        return;

                    curindex++;

                    if (curindex % intervalcount == 0)
                    {
                        if (rbSet.Checked)
                        {
                            file.Attributes |= FileAttributes.Archive;
                        }
                        else
                        {
                            file.Attributes &= ~FileAttributes.Archive;
                        }

                        totalFilesSet++;
                    }
                }
                catch { }
            }

            foreach (DirectoryInfo subDir in dir.EnumerateDirectories())
            {
                try
                {
                    RunFileAttribute(subDir);
                }
                catch { }
            }
        }
    }
}
