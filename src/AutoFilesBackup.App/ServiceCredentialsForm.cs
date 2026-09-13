using System;
using System.Drawing;
using System.Windows.Forms;

namespace AutoFilesBackup.App
{
    public class ServiceCredentialsForm : Form
    {
        private RadioButton rbLocalSystem;
        private RadioButton rbCustomUser;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnOk;
        private Button btnCancel;

        public string Username => rbCustomUser.Checked ? txtUsername.Text.Trim() : null;
        public string Password => rbCustomUser.Checked ? txtPassword.Text : null;

        public ServiceCredentialsForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Windows Service Account Setup";
            this.Size = new Size(460, 290);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 9.5f);

            var lblDesc = new Label
            {
                Text = "Choose the Windows user account to run the background service:",
                Location = new Point(20, 15),
                Size = new Size(410, 35)
            };

            rbLocalSystem = new RadioButton
            {
                Text = "Run as Local System Account (Recommended for automated disk wipe)",
                Location = new Point(25, 55),
                Size = new Size(400, 24),
                Checked = true
            };
            rbLocalSystem.CheckedChanged += (s, e) => ToggleInputs();

            rbCustomUser = new RadioButton
            {
                Text = "Run as Custom User / Windows Administrator",
                Location = new Point(25, 85),
                Size = new Size(400, 24)
            };

            var lblUser = new Label { Text = "User (e.g. .\\admin):", Location = new Point(45, 120), AutoSize = true };
            txtUsername = new TextBox { Location = new Point(190, 117), Size = new Size(220, 25), Enabled = false };

            var lblPwd = new Label { Text = "Password:", Location = new Point(45, 155), AutoSize = true };
            txtPassword = new TextBox { Location = new Point(190, 152), Size = new Size(220, 25), UseSystemPasswordChar = true, Enabled = false };

            btnOk = new Button { Text = "Install Service", Location = new Point(200, 200), Size = new Size(110, 32), DialogResult = DialogResult.OK };
            btnCancel = new Button { Text = "Cancel", Location = new Point(320, 200), Size = new Size(90, 32), DialogResult = DialogResult.Cancel };

            this.Controls.AddRange(new Control[] {
                lblDesc, rbLocalSystem, rbCustomUser,
                lblUser, txtUsername, lblPwd, txtPassword,
                btnOk, btnCancel
            });

            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
        }

        private void ToggleInputs()
        {
            txtUsername.Enabled = rbCustomUser.Checked;
            txtPassword.Enabled = rbCustomUser.Checked;
        }
    }
}
