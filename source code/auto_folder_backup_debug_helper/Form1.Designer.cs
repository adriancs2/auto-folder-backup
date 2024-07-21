namespace auto_folder_backup_debug_helper
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.nmPercent = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.rbSet = new System.Windows.Forms.RadioButton();
            this.btFolder = new System.Windows.Forms.Button();
            this.txtFolder = new System.Windows.Forms.TextBox();
            this.rbRemove = new System.Windows.Forms.RadioButton();
            this.btRun = new System.Windows.Forms.Button();
            this.btRandomFiles = new System.Windows.Forms.Button();
            this.nmRandomFiles = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nmPercent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmRandomFiles)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 25);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(446, 60);
            this.label1.TabIndex = 0;
            this.label1.Text = "Auto Folder Backup Tester Helper\r\n\r\nThis program will intentionally set the files" +
    " attribute of \"Archive\"";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 209);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Set";
            // 
            // nmPercent
            // 
            this.nmPercent.Location = new System.Drawing.Point(67, 207);
            this.nmPercent.Name = "nmPercent";
            this.nmPercent.Size = new System.Drawing.Size(63, 26);
            this.nmPercent.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(136, 209);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 20);
            this.label3.TabIndex = 3;
            this.label3.Text = "% of files";
            // 
            // rbSet
            // 
            this.rbSet.AutoSize = true;
            this.rbSet.Checked = true;
            this.rbSet.Location = new System.Drawing.Point(31, 262);
            this.rbSet.Name = "rbSet";
            this.rbSet.Size = new System.Drawing.Size(307, 24);
            this.rbSet.TabIndex = 4;
            this.rbSet.TabStop = true;
            this.rbSet.Text = "Set \"Archive\" Attribute to Selected Files";
            this.rbSet.UseVisualStyleBackColor = true;
            // 
            // btFolder
            // 
            this.btFolder.Location = new System.Drawing.Point(31, 117);
            this.btFolder.Name = "btFolder";
            this.btFolder.Size = new System.Drawing.Size(430, 30);
            this.btFolder.TabIndex = 6;
            this.btFolder.Text = "Select the folder that you\'re currently doing backup test";
            this.btFolder.UseVisualStyleBackColor = true;
            this.btFolder.Click += new System.EventHandler(this.btFolder_Click);
            // 
            // txtFolder
            // 
            this.txtFolder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFolder.Location = new System.Drawing.Point(31, 153);
            this.txtFolder.Name = "txtFolder";
            this.txtFolder.ReadOnly = true;
            this.txtFolder.Size = new System.Drawing.Size(567, 26);
            this.txtFolder.TabIndex = 7;
            // 
            // rbRemove
            // 
            this.rbRemove.AutoSize = true;
            this.rbRemove.Location = new System.Drawing.Point(31, 301);
            this.rbRemove.Name = "rbRemove";
            this.rbRemove.Size = new System.Drawing.Size(359, 24);
            this.rbRemove.TabIndex = 8;
            this.rbRemove.Text = "Remove \"Archive\" Attribute from Selected Files";
            this.rbRemove.UseVisualStyleBackColor = true;
            // 
            // btRun
            // 
            this.btRun.Location = new System.Drawing.Point(31, 349);
            this.btRun.Name = "btRun";
            this.btRun.Size = new System.Drawing.Size(123, 48);
            this.btRun.TabIndex = 9;
            this.btRun.Text = "Run";
            this.btRun.UseVisualStyleBackColor = true;
            this.btRun.Click += new System.EventHandler(this.btRun_Click);
            // 
            // btRandomFiles
            // 
            this.btRandomFiles.Location = new System.Drawing.Point(160, 349);
            this.btRandomFiles.Name = "btRandomFiles";
            this.btRandomFiles.Size = new System.Drawing.Size(210, 48);
            this.btRandomFiles.TabIndex = 10;
            this.btRandomFiles.Text = "Generate Random Files";
            this.btRandomFiles.UseVisualStyleBackColor = true;
            this.btRandomFiles.Click += new System.EventHandler(this.btRandomFiles_Click);
            // 
            // nmRandomFiles
            // 
            this.nmRandomFiles.Location = new System.Drawing.Point(376, 361);
            this.nmRandomFiles.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nmRandomFiles.Name = "nmRandomFiles";
            this.nmRandomFiles.Size = new System.Drawing.Size(70, 26);
            this.nmRandomFiles.TabIndex = 11;
            this.nmRandomFiles.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(452, 363);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(107, 20);
            this.label4.TabIndex = 12;
            this.label4.Text = "Random Files";
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(624, 409);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.nmRandomFiles);
            this.Controls.Add(this.btRandomFiles);
            this.Controls.Add(this.btRun);
            this.Controls.Add(this.rbRemove);
            this.Controls.Add(this.txtFolder);
            this.Controls.Add(this.btFolder);
            this.Controls.Add(this.rbSet);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.nmPercent);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.nmPercent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmRandomFiles)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nmPercent;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton rbSet;
        private System.Windows.Forms.Button btFolder;
        private System.Windows.Forms.TextBox txtFolder;
        private System.Windows.Forms.RadioButton rbRemove;
        private System.Windows.Forms.Button btRun;
        private System.Windows.Forms.Button btRandomFiles;
        private System.Windows.Forms.NumericUpDown nmRandomFiles;
        private System.Windows.Forms.Label label4;
    }
}

