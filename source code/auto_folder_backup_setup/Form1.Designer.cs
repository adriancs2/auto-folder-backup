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
            this.btSelectFolder = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.txtFolder = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.nmTotalDays = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btRun = new System.Windows.Forms.Button();
            this.btCreateTaskScheduler = new System.Windows.Forms.Button();
            this.btDeleteTaskScheduler = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.nmTaskHour = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.nmTaskMinute = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lbTaskSchedulerStatus = new System.Windows.Forms.Label();
            this.btClose = new System.Windows.Forms.Button();
            this.lbFormTitle = new System.Windows.Forms.Label();
            this.btMinimize = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmTotalDays)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmTaskHour)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmTaskMinute)).BeginInit();
            this.SuspendLayout();
            // 
            // btSave
            // 
            this.btSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btSave.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.btSave.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.btSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btSave.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.btSave.Location = new System.Drawing.Point(11, 35);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(144, 35);
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
            this.btReload.Location = new System.Drawing.Point(161, 35);
            this.btReload.Name = "btReload";
            this.btReload.Size = new System.Drawing.Size(163, 35);
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
            this.btOpenLogFile.Location = new System.Drawing.Point(330, 35);
            this.btOpenLogFile.Name = "btOpenLogFile";
            this.btOpenLogFile.Size = new System.Drawing.Size(149, 35);
            this.btOpenLogFile.TabIndex = 2;
            this.btOpenLogFile.Text = "open log file";
            this.btOpenLogFile.UseVisualStyleBackColor = false;
            this.btOpenLogFile.Click += new System.EventHandler(this.btOpenLogFile_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Lucida Console", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(419, 579);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(383, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "https://github.com/adriancs2/auto-folder-backup";
            // 
            // btSelectFolder
            // 
            this.btSelectFolder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btSelectFolder.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.btSelectFolder.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.btSelectFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btSelectFolder.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.btSelectFolder.Location = new System.Drawing.Point(11, 76);
            this.btSelectFolder.Name = "btSelectFolder";
            this.btSelectFolder.Size = new System.Drawing.Size(250, 35);
            this.btSelectFolder.TabIndex = 5;
            this.btSelectFolder.Text = "select a folder to backup";
            this.btSelectFolder.UseVisualStyleBackColor = false;
            this.btSelectFolder.Click += new System.EventHandler(this.btSelectFolder_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Location = new System.Drawing.Point(197, 164);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(605, 38);
            this.flowLayoutPanel1.TabIndex = 7;
            // 
            // txtFolder
            // 
            this.txtFolder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.txtFolder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFolder.Font = new System.Drawing.Font("Lucida Console", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFolder.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.txtFolder.Location = new System.Drawing.Point(267, 83);
            this.txtFolder.Name = "txtFolder";
            this.txtFolder.ReadOnly = true;
            this.txtFolder.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtFolder.Size = new System.Drawing.Size(523, 23);
            this.txtFolder.TabIndex = 8;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::System.Properties.Resources.logo_100x100;
            this.pictureBox1.Location = new System.Drawing.Point(686, 457);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(104, 108);
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            // 
            // nmTotalDays
            // 
            this.nmTotalDays.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.nmTotalDays.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.nmTotalDays.Location = new System.Drawing.Point(264, 127);
            this.nmTotalDays.Name = "nmTotalDays";
            this.nmTotalDays.Size = new System.Drawing.Size(68, 22);
            this.nmTotalDays.TabIndex = 10;
            this.nmTotalDays.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(338, 127);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(439, 30);
            this.label3.TabIndex = 11;
            this.label3.Text = "(zero = always perform full backup)\r\n(every other day, it will do incremental bac" +
    "kup)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Lucida Console", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(12, 416);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(663, 130);
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
            this.btRun.Location = new System.Drawing.Point(485, 35);
            this.btRun.Name = "btRun";
            this.btRun.Size = new System.Drawing.Size(166, 35);
            this.btRun.TabIndex = 14;
            this.btRun.Text = "run backup now";
            this.btRun.UseVisualStyleBackColor = false;
            this.btRun.Click += new System.EventHandler(this.btRun_Click);
            // 
            // btCreateTaskScheduler
            // 
            this.btCreateTaskScheduler.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btCreateTaskScheduler.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.btCreateTaskScheduler.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.btCreateTaskScheduler.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btCreateTaskScheduler.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.btCreateTaskScheduler.Location = new System.Drawing.Point(11, 278);
            this.btCreateTaskScheduler.Name = "btCreateTaskScheduler";
            this.btCreateTaskScheduler.Size = new System.Drawing.Size(224, 35);
            this.btCreateTaskScheduler.TabIndex = 15;
            this.btCreateTaskScheduler.Text = "create task scheduler";
            this.btCreateTaskScheduler.UseVisualStyleBackColor = false;
            this.btCreateTaskScheduler.Click += new System.EventHandler(this.btCreateTaskScheduler_Click);
            // 
            // btDeleteTaskScheduler
            // 
            this.btDeleteTaskScheduler.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btDeleteTaskScheduler.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.btDeleteTaskScheduler.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.btDeleteTaskScheduler.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btDeleteTaskScheduler.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.btDeleteTaskScheduler.Location = new System.Drawing.Point(241, 278);
            this.btDeleteTaskScheduler.Name = "btDeleteTaskScheduler";
            this.btDeleteTaskScheduler.Size = new System.Drawing.Size(224, 35);
            this.btDeleteTaskScheduler.TabIndex = 16;
            this.btDeleteTaskScheduler.Text = "delete task scheduler";
            this.btDeleteTaskScheduler.UseVisualStyleBackColor = false;
            this.btDeleteTaskScheduler.Click += new System.EventHandler(this.btDeleteTaskScheduler_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 129);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(250, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "total days for full backup:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 357);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(169, 15);
            this.label4.TabIndex = 17;
            this.label4.Text = "task trigger time:";
            // 
            // nmTaskHour
            // 
            this.nmTaskHour.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.nmTaskHour.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.nmTaskHour.Location = new System.Drawing.Point(183, 355);
            this.nmTaskHour.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.nmTaskHour.Name = "nmTaskHour";
            this.nmTaskHour.Size = new System.Drawing.Size(54, 22);
            this.nmTaskHour.TabIndex = 18;
            this.nmTaskHour.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nmTaskHour.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(241, 357);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(16, 15);
            this.label6.TabIndex = 19;
            this.label6.Text = "h";
            // 
            // nmTaskMinute
            // 
            this.nmTaskMinute.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.nmTaskMinute.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.nmTaskMinute.Location = new System.Drawing.Point(263, 355);
            this.nmTaskMinute.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.nmTaskMinute.Name = "nmTaskMinute";
            this.nmTaskMinute.Size = new System.Drawing.Size(54, 22);
            this.nmTaskMinute.TabIndex = 20;
            this.nmTaskMinute.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(323, 357);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(16, 15);
            this.label7.TabIndex = 21;
            this.label7.Text = "m";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 170);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(178, 15);
            this.label8.TabIndex = 22;
            this.label8.Text = "destination drives:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 325);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(205, 15);
            this.label9.TabIndex = 23;
            this.label9.Text = "task scheduler status:";
            // 
            // lbTaskSchedulerStatus
            // 
            this.lbTaskSchedulerStatus.AutoSize = true;
            this.lbTaskSchedulerStatus.Location = new System.Drawing.Point(222, 326);
            this.lbTaskSchedulerStatus.Name = "lbTaskSchedulerStatus";
            this.lbTaskSchedulerStatus.Size = new System.Drawing.Size(196, 15);
            this.lbTaskSchedulerStatus.TabIndex = 24;
            this.lbTaskSchedulerStatus.Text = "lbTaskSchedulerStatus";
            // 
            // btClose
            // 
            this.btClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btClose.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.btClose.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btClose.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.btClose.Location = new System.Drawing.Point(776, 5);
            this.btClose.Name = "btClose";
            this.btClose.Size = new System.Drawing.Size(35, 23);
            this.btClose.TabIndex = 26;
            this.btClose.Text = "X";
            this.btClose.UseVisualStyleBackColor = false;
            this.btClose.Click += new System.EventHandler(this.btClose_Click);
            // 
            // lbFormTitle
            // 
            this.lbFormTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.lbFormTitle.Font = new System.Drawing.Font("Lucida Console", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbFormTitle.Location = new System.Drawing.Point(2, 2);
            this.lbFormTitle.Name = "lbFormTitle";
            this.lbFormTitle.Padding = new System.Windows.Forms.Padding(6, 8, 0, 0);
            this.lbFormTitle.Size = new System.Drawing.Size(812, 30);
            this.lbFormTitle.TabIndex = 28;
            this.lbFormTitle.Text = "Auto Folder Backup (beta 0.3)";
            this.lbFormTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbFormTitle_MouseDown);
            this.lbFormTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lbFormTitle_MouseMove);
            this.lbFormTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lbFormTitle_MouseUp);
            // 
            // btMinimize
            // 
            this.btMinimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btMinimize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btMinimize.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.btMinimize.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(250)))));
            this.btMinimize.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(180)))));
            this.btMinimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btMinimize.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.btMinimize.Location = new System.Drawing.Point(738, 5);
            this.btMinimize.Name = "btMinimize";
            this.btMinimize.Size = new System.Drawing.Size(35, 23);
            this.btMinimize.TabIndex = 29;
            this.btMinimize.Text = "_";
            this.btMinimize.UseVisualStyleBackColor = false;
            this.btMinimize.Click += new System.EventHandler(this.btMinimize_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Lucida Console", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(12, 207);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(591, 39);
            this.label10.TabIndex = 30;
            this.label10.Text = "* warning! selected drives will be formatted. select the drive carefully.\r\n* sele" +
    "ct 2 or more drives\r\n* drive C and D are not allowed";
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.ClientSize = new System.Drawing.Size(816, 601);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.btMinimize);
            this.Controls.Add(this.btClose);
            this.Controls.Add(this.lbFormTitle);
            this.Controls.Add(this.lbTaskSchedulerStatus);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.nmTaskMinute);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.nmTaskHour);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btDeleteTaskScheduler);
            this.Controls.Add(this.btCreateTaskScheduler);
            this.Controls.Add(this.btRun);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.nmTotalDays);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.txtFolder);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.btSelectFolder);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btOpenLogFile);
            this.Controls.Add(this.btReload);
            this.Controls.Add(this.btSave);
            this.Font = new System.Drawing.Font("Lucida Console", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Auto Folder Backup (beta 0.3)";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmTotalDays)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmTaskHour)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmTaskMinute)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Windows.Forms.Button btSave;
        private Windows.Forms.Button btReload;
        private Windows.Forms.Button btOpenLogFile;
        private Windows.Forms.Label label1;
        private Windows.Forms.Button btSelectFolder;
        private Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private Windows.Forms.TextBox txtFolder;
        private Windows.Forms.PictureBox pictureBox1;
        private Windows.Forms.NumericUpDown nmTotalDays;
        private Windows.Forms.Label label3;
        private Windows.Forms.Label label5;
        private Windows.Forms.Button btRun;
        private Windows.Forms.Button btCreateTaskScheduler;
        private Windows.Forms.Button btDeleteTaskScheduler;
        private Windows.Forms.Label label2;
        private Windows.Forms.Label label4;
        private Windows.Forms.NumericUpDown nmTaskHour;
        private Windows.Forms.Label label6;
        private Windows.Forms.NumericUpDown nmTaskMinute;
        private Windows.Forms.Label label7;
        private Windows.Forms.Label label8;
        private Windows.Forms.Label label9;
        private Windows.Forms.Label lbTaskSchedulerStatus;
        private Windows.Forms.Button btClose;
        private Windows.Forms.Label lbFormTitle;
        private Windows.Forms.Button btMinimize;
        private Windows.Forms.Label label10;
    }
}