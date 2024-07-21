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
        BackgroundWorker bw2 = new BackgroundWorker();

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

            bw2.DoWork += Bw2_DoWork;
            bw2.RunWorkerCompleted += Bw2_RunWorkerCompleted;
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
                MessageBox.Show("I'm busy, please wait");
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

        private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show(error + "\r\n\r\nTotal Files: " + totalFiles + "\r\nExpected Target Files: " + targetFiles + "\r\nTotal Files Set: " + totalFilesSet+"\r\nInterval Count: "+intervalcount);
        }

        static readonly string[] recycleBinNames = { "$RECYCLE.BIN", "$Recycle.Bin", "System Volume Information" };

        static bool IsRecycleBinPath(string path)
        {
            string normalizedPath = Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar);
            string[] pathParts = normalizedPath.Split(Path.DirectorySeparatorChar);

            return pathParts.Any(part => recycleBinNames.Contains(part, StringComparer.OrdinalIgnoreCase));
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
                intervalcount = totalFiles / targetFiles;
                curindex = 0;

                AttributeFolderFiles(dir);

                error = "done";
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
        }

        void AttributeFolderFiles(DirectoryInfo dir)
        {
            foreach(var subdir in dir.EnumerateDirectories())
            {
                if (IsRecycleBinPath(subdir.FullName))
                {
                    continue;
                }

                AttributeFolderFiles(subdir);
            }

            foreach (var file in dir.EnumerateFiles())
            {
                curindex++;

                if (curindex >= intervalcount)
                {
                    curindex = 0;

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
        }

        void CountFiles(DirectoryInfo dir)
        {
            try
            {
                foreach(var subdir in dir.EnumerateDirectories())
                {
                    if (IsRecycleBinPath(subdir.FullName))
                        continue;

                    CountFiles(subdir);
                }

                foreach (FileInfo file in dir.EnumerateFiles())
                {
                    totalFiles++;
                }
            }
            catch { }
        }


        int totalRandomFiles = 0;

        private void btRandomFiles_Click(object sender, EventArgs e)
        {
            if (bw2.IsBusy)
            {
                MessageBox.Show("I'm busy generating files. Hold on.");
                return;
            }

            targetFolder = txtFolder.Text;
            totalRandomFiles = (int)nmRandomFiles.Value;

            bw2.RunWorkerAsync();
        }

        private void Bw2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("done");
        }

        private void Bw2_DoWork(object sender, DoWorkEventArgs e)
        {
            DirectoryInfo folder = new DirectoryInfo(targetFolder);

            string foldername = "random-" + DateTime.Now.ToString("yyyyMMddHHmmss");

            var subDir = folder.CreateSubdirectory(foldername);

            for(int i = 0; i < totalRandomFiles; i ++)
            {
                string filename = i.ToString().PadLeft(8, '0');

                string filepath = Path.Combine(subDir.FullName, $"{filename}.txt");

                File.WriteAllText(filepath, filename);
            }
        }

    }
}
