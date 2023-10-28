namespace System
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btSave = new System.Windows.Forms.Button();
            this.btReload = new System.Windows.Forms.Button();
            this.btOpenLogFile = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btSelectFolder = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.txtFolder = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.nmTotalDays = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btRun = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmTotalDays)).BeginInit();
            this.SuspendLayout();
            // 
            // btSave
            // 
            this.btSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btSave.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.btSave.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.btSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btSave.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.btSave.Location = new System.Drawing.Point(12, 12);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(157, 35);
            this.btSave.TabIndex = 0;
            this.btSave.Text = "save settings";
            this.btSave.UseVisualStyleBackColor = false;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // btReload
            // 
            this.btReload.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btReload.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.btReload.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.btReload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btReload.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.btReload.Location = new System.Drawing.Point(175, 12);
            this.btReload.Name = "btReload";
            this.btReload.Size = new System.Drawing.Size(171, 35);
            this.btReload.TabIndex = 1;
            this.btReload.Text = "reload settings";
            this.btReload.UseVisualStyleBackColor = false;
            this.btReload.Click += new System.EventHandler(this.btReload_Click);
            // 
            // btOpenLogFile
            // 
            this.btOpenLogFile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btOpenLogFile.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.btOpenLogFile.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.btOpenLogFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btOpenLogFile.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.btOpenLogFile.Location = new System.Drawing.Point(352, 12);
            this.btOpenLogFile.Name = "btOpenLogFile";
            this.btOpenLogFile.Size = new System.Drawing.Size(155, 35);
            this.btOpenLogFile.TabIndex = 2;
            this.btOpenLogFile.Text = "open log file";
            this.btOpenLogFile.UseVisualStyleBackColor = false;
            this.btOpenLogFile.Click += new System.EventHandler(this.btOpenLogFile_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Lucida Console", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(385, 564);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(430, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "https://github.com/adriancs2/auto-folder-backup";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 108);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(277, 160);
            this.label2.TabIndex = 4;
            this.label2.Text = "target folder:\r\n \r\n\r\n\r\n\r\ntotal days for full backup:\r\n\r\n\r\n\r\ndestination drives:";
            // 
            // btSelectFolder
            // 
            this.btSelectFolder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btSelectFolder.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.btSelectFolder.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.btSelectFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btSelectFolder.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.btSelectFolder.Location = new System.Drawing.Point(12, 65);
            this.btSelectFolder.Name = "btSelectFolder";
            this.btSelectFolder.Size = new System.Drawing.Size(302, 35);
            this.btSelectFolder.TabIndex = 5;
            this.btSelectFolder.Text = "select a folder to backup";
            this.btSelectFolder.UseVisualStyleBackColor = false;
            this.btSelectFolder.Click += new System.EventHandler(this.btSelectFolder_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Location = new System.Drawing.Point(210, 247);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(605, 47);
            this.flowLayoutPanel1.TabIndex = 7;
            // 
            // txtFolder
            // 
            this.txtFolder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.txtFolder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFolder.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.txtFolder.Location = new System.Drawing.Point(162, 106);
            this.txtFolder.Multiline = true;
            this.txtFolder.Name = "txtFolder";
            this.txtFolder.ReadOnly = true;
            this.txtFolder.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtFolder.Size = new System.Drawing.Size(653, 52);
            this.txtFolder.TabIndex = 8;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::System.Properties.Resources.logo_100x100;
            this.pictureBox1.Location = new System.Drawing.Point(711, 434);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(104, 108);
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            // 
            // nmTotalDays
            // 
            this.nmTotalDays.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.nmTotalDays.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.nmTotalDays.Location = new System.Drawing.Point(292, 186);
            this.nmTotalDays.Name = "nmTotalDays";
            this.nmTotalDays.Size = new System.Drawing.Size(68, 23);
            this.nmTotalDays.TabIndex = 10;
            this.nmTotalDays.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(366, 188);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(377, 32);
            this.label3.TabIndex = 11;
            this.label3.Text = "(zero = always perform full backup)\r\n(else, it will do incremental backup)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Lucida Console", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(12, 317);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(664, 195);
            this.label5.TabIndex = 13;
            this.label5.Text = resources.GetString("label5.Text");
            // 
            // btRun
            // 
            this.btRun.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btRun.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.btRun.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.btRun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btRun.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.btRun.Location = new System.Drawing.Point(513, 12);
            this.btRun.Name = "btRun";
            this.btRun.Size = new System.Drawing.Size(178, 35);
            this.btRun.TabIndex = 14;
            this.btRun.Text = "run backup now";
            this.btRun.UseVisualStyleBackColor = false;
            this.btRun.Click += new System.EventHandler(this.btRun_Click);
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(832, 588);
            this.Controls.Add(this.btRun);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.nmTotalDays);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.txtFolder);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.btSelectFolder);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btOpenLogFile);
            this.Controls.Add(this.btReload);
            this.Controls.Add(this.btSave);
            this.Font = new System.Drawing.Font("Lucida Console", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Setup - Simple Auto Folder Backup - beta 0.1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmTotalDays)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Windows.Forms.Button btSave;
        private Windows.Forms.Button btReload;
        private Windows.Forms.Button btOpenLogFile;
        private Windows.Forms.Label label1;
        private Windows.Forms.Label label2;
        private Windows.Forms.Button btSelectFolder;
        private Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private Windows.Forms.TextBox txtFolder;
        private Windows.Forms.PictureBox pictureBox1;
        private Windows.Forms.NumericUpDown nmTotalDays;
        private Windows.Forms.Label label3;
        private Windows.Forms.Label label5;
        private Windows.Forms.Button btRun;
    }
}