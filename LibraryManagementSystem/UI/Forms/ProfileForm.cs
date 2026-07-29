using LibraryManagementSystem.BusinessLogic;
using LibraryManagementSystem.UI.Helpers;
using LibraryManagementSystem.UI.Controls;

namespace LibraryManagementSystem.UI.Forms
{
    public class ProfileForm : Form
    {
        private readonly AuthService _authService = new();

        private RoundedTextBox? txtFullName;
        private RoundedButton? btnSave;
        private RoundedButton? btnChangePassword;
        private RoundedButton? btnCancel;

        public ProfileForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(380, 260);
            this.Text = "My Profile";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = ThemeManager.BgColor;

            var panel = new RoundedPanel
            {
                Size = new Size(340, 180),
                Location = new Point(12, 12),
                BackColor = ThemeManager.CardColor,
                CornerRadius = 14,
                BorderWidth = 1,
                BorderColor = ThemeManager.BorderColor
            };

            var lblName = new Label
            {
                Text = "Full Name:",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = ThemeManager.TextColor,
                Location = new Point(20, 25),
                AutoSize = true
            };
            panel.Controls.Add(lblName);

            txtFullName = new RoundedTextBox
            {
                Location = new Point(110, 20),
                Size = new Size(200, 38)
            };
            txtFullName.Text = AuthService.CurrentUser?.FullName ?? "";
            panel.Controls.Add(txtFullName);

            btnSave = new RoundedButton
            {
                Text = "Save Name",
                BgColor = ThemeManager.PrimaryColor,
                HoverColor = ThemeManager.HoverColor,
                Location = new Point(20, 80),
                Size = new Size(130, 36),
                CornerRadius = 8
            };
            btnSave.Click += BtnSave_Click;
            panel.Controls.Add(btnSave);

            btnChangePassword = new RoundedButton
            {
                Text = "Change Pass",
                BgColor = ThemeManager.AccentColor,
                TextColor = ThemeManager.TextColor,
                HoverColor = ThemeManager.BorderColor,
                Location = new Point(180, 80),
                Size = new Size(130, 36),
                CornerRadius = 8
            };
            btnChangePassword.Click += BtnChangePassword_Click;
            panel.Controls.Add(btnChangePassword);

            btnCancel = new RoundedButton
            {
                Text = "Close",
                BgColor = ThemeManager.AccentColor,
                TextColor = ThemeManager.TextColor,
                HoverColor = ThemeManager.BorderColor,
                Location = new Point(110, 130),
                Size = new Size(110, 32),
                CornerRadius = 8
            };
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            panel.Controls.Add(btnCancel);

            this.Controls.Add(panel);
            ThemeManager.ApplyTheme(this);
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (txtFullName == null) return;

            string name = txtFullName.Text.Trim();
            var result = _authService.UpdateProfile(name);
            MessageBox.Show(result.Message, result.Success ? "Success" : "Error");
            if (result.Success)
            {
                this.DialogResult = DialogResult.OK;
            }
        }

        private void BtnChangePassword_Click(object? sender, EventArgs e)
        {
            using var changePass = new ChangePasswordForm();
            changePass.ShowDialog();
        }
    }
}
