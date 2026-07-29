using LibraryManagementSystem.BusinessLogic;
using LibraryManagementSystem.UI.Helpers;
using LibraryManagementSystem.UI.Controls;

namespace LibraryManagementSystem.UI.Forms
{
    public class ChangePasswordForm : Form
    {
        private readonly AuthService _authService = new();

        private RoundedTextBox? txtCurrentPassword;
        private RoundedTextBox? txtNewPassword;
        private RoundedTextBox? txtConfirmPassword;
        private RoundedButton? btnSave;
        private RoundedButton? btnCancel;

        public ChangePasswordForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(380, 270);
            this.Text = "Change Password";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = ThemeManager.BgColor;

            var panel = new RoundedPanel
            {
                Size = new Size(340, 195),
                Location = new Point(12, 12),
                BackColor = ThemeManager.CardColor,
                CornerRadius = 14,
                BorderWidth = 1,
                BorderColor = ThemeManager.BorderColor
            };

            CreateLabel(panel, "Current Pass", 15, 20);
            txtCurrentPassword = CreateTextBox(panel, 125, 15, 190, true);

            CreateLabel(panel, "New Pass", 15, 60);
            txtNewPassword = CreateTextBox(panel, 125, 55, 190, true);

            CreateLabel(panel, "Confirm Pass", 15, 100);
            txtConfirmPassword = CreateTextBox(panel, 125, 95, 190, true);

            btnSave = new RoundedButton
            {
                Text = "Change",
                BgColor = ThemeManager.PrimaryColor,
                HoverColor = ThemeManager.HoverColor,
                Location = new Point(70, 145),
                Size = new Size(110, 36),
                CornerRadius = 8
            };
            btnSave.Click += BtnSave_Click;
            panel.Controls.Add(btnSave);

            btnCancel = new RoundedButton
            {
                Text = "Cancel",
                BgColor = ThemeManager.AccentColor,
                TextColor = ThemeManager.TextColor,
                HoverColor = ThemeManager.BorderColor,
                Location = new Point(190, 145),
                Size = new Size(110, 36),
                CornerRadius = 8
            };
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            panel.Controls.Add(btnCancel);

            this.Controls.Add(panel);
            ThemeManager.ApplyTheme(this);
        }

        private void CreateLabel(Panel p, string text, int x, int y)
        {
            var lbl = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = ThemeManager.TextColor,
                Location = new Point(x, y + 8),
                AutoSize = true
            };
            p.Controls.Add(lbl);
        }

        private RoundedTextBox CreateTextBox(Panel p, int x, int y, int width, bool isPass)
        {
            var txt = new RoundedTextBox
            {
                Location = new Point(x, y),
                Size = new Size(width, 38),
                IsPassword = isPass
            };
            p.Controls.Add(txt);
            return txt;
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (txtCurrentPassword == null || txtNewPassword == null || txtConfirmPassword == null) return;

            string curr = txtCurrentPassword.Text;
            string newP = txtNewPassword.Text;
            string conf = txtConfirmPassword.Text;

            var result = _authService.ChangePassword(curr, newP, conf);
            MessageBox.Show(result.Message, result.Success ? "Success" : "Error");
            if (result.Success)
            {
                this.DialogResult = DialogResult.OK;
            }
        }
    }
}
