using LibraryManagementSystem.BusinessLogic;
using LibraryManagementSystem.UI.Helpers;
using LibraryManagementSystem.UI.Controls;

namespace LibraryManagementSystem.UI.Forms
{
    public class LoginForm : Form
    {
        private readonly AuthService _authService = new();
        private RoundedTextBox? txtUsername;
        private RoundedTextBox? txtPassword;
        private CheckBox? chkShowPassword;
        private CheckBox? chkRememberMe;
        private RoundedButton? btnLogin;
        private Label? lblTitle;
        private Label? lblSubtitle;
        private Label? lblError;

        public LoginForm()
        {
            InitializeComponent();
            ThemeManager.ThemeChanged += OnThemeChanged;
        }

        private void InitializeComponent()
        {
            this.Size = new Size(460, 560);
            this.Text = "Login - Library Management System";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = ThemeManager.BgColor;

            var panel = new RoundedPanel
            {
                Size = new Size(400, 480),
                Location = new Point(22, 20),
                BackColor = ThemeManager.CardColor,
                CornerRadius = 16,
                BorderWidth = 1,
                BorderColor = ThemeManager.BorderColor
            };

            var lblLogo = new Label
            {
                Text = "🏛️",
                Font = new Font("Segoe UI Emoji", 28F),
                AutoSize = true,
                Location = new Point(175, 20)
            };
            panel.Controls.Add(lblLogo);

            lblTitle = new Label
            {
                Text = "Library Management",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = ThemeManager.TextColor,
                AutoSize = true,
                Location = new Point(90, 80)
            };
            panel.Controls.Add(lblTitle);

            lblSubtitle = new Label
            {
                Text = "Sign in to manage your system",
                Font = new Font("Segoe UI", 9F),
                ForeColor = ThemeManager.TextSecondaryColor,
                AutoSize = true,
                Location = new Point(105, 112)
            };
            panel.Controls.Add(lblSubtitle);

            var lblUsername = new Label
            {
                Text = "Username",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = ThemeManager.TextColor,
                Location = new Point(30, 150),
                AutoSize = true
            };
            panel.Controls.Add(lblUsername);

            txtUsername = new RoundedTextBox
            {
                Location = new Point(30, 172),
                Size = new Size(340, 40),
                PlaceholderText = "admin"
            };
            txtUsername.Text = "admin";
            panel.Controls.Add(txtUsername);

            var lblPassword = new Label
            {
                Text = "Password",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = ThemeManager.TextColor,
                Location = new Point(30, 222),
                AutoSize = true
            };
            panel.Controls.Add(lblPassword);

            txtPassword = new RoundedTextBox
            {
                Location = new Point(30, 244),
                Size = new Size(340, 40),
                IsPassword = true,
                PlaceholderText = "•••••"
            };
            txtPassword.Text = "admin123";
            panel.Controls.Add(txtPassword);

            chkShowPassword = new CheckBox
            {
                Text = "Show Password",
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = ThemeManager.TextSecondaryColor,
                Location = new Point(30, 295),
                AutoSize = true,
                Cursor = Cursors.Hand
            };
            chkShowPassword.CheckedChanged += (s, e) =>
            {
                txtPassword.IsPassword = !chkShowPassword.Checked;
            };
            panel.Controls.Add(chkShowPassword);

            chkRememberMe = new CheckBox
            {
                Text = "Remember Me",
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = ThemeManager.TextSecondaryColor,
                Location = new Point(230, 295),
                AutoSize = true,
                Cursor = Cursors.Hand
            };
            panel.Controls.Add(chkRememberMe);

            lblError = new Label
            {
                ForeColor = ThemeManager.DangerColor,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Italic),
                Location = new Point(30, 330),
                Size = new Size(340, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };
            panel.Controls.Add(lblError);

            btnLogin = new RoundedButton
            {
                Text = "Login",
                BgColor = ThemeManager.PrimaryColor,
                HoverColor = ThemeManager.HoverColor,
                Location = new Point(30, 365),
                Size = new Size(340, 45)
            };
            btnLogin.Click += BtnLogin_Click;
            panel.Controls.Add(btnLogin);

            this.Controls.Add(panel);
            txtUsername.InnerTextBox.KeyDown += Txt_KeyDown;
            txtPassword.InnerTextBox.KeyDown += Txt_KeyDown;

            OnThemeChanged();
        }

        private void Txt_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BtnLogin_Click(this, EventArgs.Empty);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            if (txtUsername == null || txtPassword == null || lblError == null) return;

            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            var result = _authService.Login(username, password);
            if (result.Success)
            {
                lblError.Text = "";
                this.Hide();
                var mainForm = new MainForm();
                mainForm.FormClosed += (s, args) => this.Close();
                mainForm.Show();
            }
            else
            {
                lblError.Text = result.Message;
                NotificationToast.Show(this, result.Message, NotificationType.Error);
            }
        }

        private void OnThemeChanged()
        {
            this.BackColor = ThemeManager.BgColor;
            ThemeManager.ApplyTheme(this);
            if (txtUsername != null) txtUsername.UpdateTheme();
            if (txtPassword != null) txtPassword.UpdateTheme();
        }
    }
}
