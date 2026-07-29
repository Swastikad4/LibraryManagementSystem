using LibraryManagementSystem.UI.Helpers;
using LibraryManagementSystem.UI.Controls;
using LibraryManagementSystem.BusinessLogic;

namespace LibraryManagementSystem.UI.Forms
{
    public class MainForm : Form
    {
        private SidebarMenu? sidebar;
        private Panel? contentPanel;
        private Label? lblPageTitle;
        private Label? lblDateTime;
        private System.Windows.Forms.Timer? clockTimer;
        private RoundedButton? btnThemeToggle;
        private RoundedButton? btnProfile;
        private Label? lblStatusBar;

        // Current active panel
        private UserControl? currentActiveControl;

        public MainForm()
        {
            InitializeComponent();
            ThemeManager.ThemeChanged += OnThemeChanged;
        }

        private void InitializeComponent()
        {
            this.Size = new Size(1200, 750);
            this.MinimumSize = new Size(1000, 650);
            this.Text = "LMS - Library Management System";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.KeyPreview = true; // For global hotkeys

            // ============================================================
            // LAYOUT HIERARCHY (controls added in reverse dock order):
            //   Form
            //     ├── sidebar (Dock=Left, Width=220)
            //     └── rightContainerPanel (Dock=Fill)
            //           ├── topPanel (Dock=Top, Height=70)
            //           ├── statusStrip (Dock=Bottom)
            //           └── contentPanel (Dock=Fill, AutoScroll)
            // ============================================================

            // --- Sidebar Menu (Dock Left) ---
            sidebar = new SidebarMenu();
            sidebar.SetItems(new List<SidebarMenuItem>
            {
                new() { Icon = IconHelper.Dashboard, Text = "Dashboard", Key = "dashboard" },
                new() { Icon = IconHelper.Books, Text = "Books Collection", Key = "books" },
                new() { Icon = IconHelper.Members, Text = "Members Registry", Key = "members" },
                new() { Icon = IconHelper.IssueBook, Text = "Issue Book", Key = "issue" },
                new() { Icon = IconHelper.ReturnBook, Text = "Return Book", Key = "return" },
                new() { Icon = IconHelper.Search, Text = "Global Search", Key = "search" },
                new() { Icon = IconHelper.Reports, Text = "Reports & Export", Key = "reports" },
                new() { Icon = IconHelper.Settings, Text = "Settings & Backup", Key = "settings" },
                new() { Icon = IconHelper.Logout, Text = "Logout", Key = "logout" }
            });
            sidebar.MenuItemClicked += Sidebar_MenuItemClicked;

            // --- Right Container Panel (Dock Fill) ---
            var rightContainerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ThemeManager.BgColor
            };

            // --- Top Header Bar (inside rightContainerPanel, Dock Top) ---
            var topPanel = new Panel
            {
                Height = 70,
                Dock = DockStyle.Top,
                BackColor = ThemeManager.CardColor,
                BorderStyle = BorderStyle.None
            };
            topPanel.Paint += (s, e) =>
            {
                using var pen = new Pen(ThemeManager.BorderColor, 1);
                e.Graphics.DrawLine(pen, 0, topPanel.Height - 1, topPanel.Width, topPanel.Height - 1);
            };

            lblPageTitle = new Label
            {
                Text = "Dashboard",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = ThemeManager.TextColor,
                Location = new Point(20, 20),
                AutoSize = true
            };
            topPanel.Controls.Add(lblPageTitle);

            // Clock/Date display — anchored to top-right
            lblDateTime = new Label
            {
                Text = DateTime.Now.ToString("dd-MM-yyyy  HH:mm:ss"),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = ThemeManager.TextSecondaryColor,
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            // Position will be set in topPanel.Resize
            topPanel.Controls.Add(lblDateTime);

            // Clock Timer
            clockTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            clockTimer.Tick += (s, e) =>
            {
                if (lblDateTime != null)
                {
                    lblDateTime.Text = DateTime.Now.ToString("dd-MM-yyyy  HH:mm:ss");
                }
            };
            clockTimer.Start();

            // Profile Button — anchored to top-right
            btnProfile = new RoundedButton
            {
                Text = AuthService.CurrentUser?.FullName ?? "Profile",
                Icon = IconHelper.Profile,
                BgColor = ThemeManager.PrimaryColor,
                TextColor = Color.White,
                HoverColor = ThemeManager.HoverColor,
                Width = 160,
                Height = 36,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                CornerRadius = 8
            };
            btnProfile.Click += BtnProfile_Click;
            topPanel.Controls.Add(btnProfile);

            // Theme Toggle Button — anchored to top-right
            btnThemeToggle = new RoundedButton
            {
                Text = "Mode",
                Icon = IconHelper.DarkMode,
                BgColor = ThemeManager.AccentColor,
                TextColor = ThemeManager.TextColor,
                HoverColor = ThemeManager.BorderColor,
                Width = 100,
                Height = 36,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                CornerRadius = 8
            };
            btnThemeToggle.Click += (s, e) => ThemeManager.ToggleTheme();
            topPanel.Controls.Add(btnThemeToggle);

            // Position right-anchored controls on resize
            topPanel.Resize += (s, e) =>
            {
                int rightMargin = 15;
                int y = 17;

                if (btnProfile != null)
                {
                    btnProfile.Location = new Point(topPanel.Width - btnProfile.Width - rightMargin, y);
                }
                if (btnThemeToggle != null)
                {
                    btnThemeToggle.Location = new Point(topPanel.Width - btnProfile!.Width - btnThemeToggle.Width - rightMargin - 10, y);
                }
                if (lblDateTime != null)
                {
                    lblDateTime.Location = new Point(topPanel.Width - btnProfile!.Width - btnThemeToggle!.Width - lblDateTime.Width - rightMargin - 30, 24);
                }
            };

            // --- Status Bar at the bottom (inside rightContainerPanel) ---
            var statusStrip = new StatusStrip
            {
                BackColor = ThemeManager.CardColor,
                ForeColor = ThemeManager.TextSecondaryColor,
                Font = new Font("Segoe UI", 9F)
            };
            lblStatusBar = new Label
            {
                Text = $"System Ready. Logged in as admin. Database: SQLite Local",
                AutoSize = true,
                BackColor = Color.Transparent,
                Padding = new Padding(5)
            };
            statusStrip.Items.Add(new ToolStripControlHost(lblStatusBar));

            // --- Content Panel (inside rightContainerPanel, Dock Fill) ---
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ThemeManager.BgColor,
                Padding = new Padding(20),
                AutoScroll = true
            };

            // Add to rightContainerPanel in correct dock order:
            // First added = last evaluated for docking → add Fill last
            rightContainerPanel.Controls.Add(contentPanel);   // Fill (added first, evaluated last)
            rightContainerPanel.Controls.Add(statusStrip);     // Bottom
            rightContainerPanel.Controls.Add(topPanel);        // Top

            // Add to form in correct dock order:
            // sidebar Left, rightContainerPanel Fill
            this.Controls.Add(rightContainerPanel);  // Fill (added first, evaluated last)
            this.Controls.Add(sidebar);               // Left

            // Load initial view
            LoadPanel("dashboard");

            // Keyboard Shortcuts
            this.KeyDown += MainForm_KeyDown;

            OnThemeChanged();
        }

        private void BtnProfile_Click(object? sender, EventArgs e)
        {
            using var profileForm = new ProfileForm();
            if (profileForm.ShowDialog() == DialogResult.OK)
            {
                if (btnProfile != null)
                {
                    btnProfile.Text = AuthService.CurrentUser?.FullName ?? "Profile";
                }
            }
        }

        private void Sidebar_MenuItemClicked(string key)
        {
            if (key == "logout")
            {
                var confirm = MessageBox.Show("Are you sure you want to logout?", "Logout",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                {
                    AuthService.Logout();
                    this.Hide();
                    var login = new LoginForm();
                    login.Show();
                }
                else
                {
                    // Reset selection
                    if (sidebar != null && currentActiveControl != null)
                    {
                        sidebar.SetActiveByKey(currentActiveControl.Name.Replace("Panel", "").ToLower());
                    }
                }
                return;
            }

            LoadPanel(key);
        }

        public void LoadPanel(string key)
        {
            if (contentPanel == null || lblPageTitle == null) return;

            // Clear old control
            if (currentActiveControl != null)
            {
                contentPanel.Controls.Remove(currentActiveControl);
                currentActiveControl.Dispose();
            }

            // Create new control based on key
            UserControl newControl = key switch
            {
                "dashboard" => new DashboardPanel(this),
                "books" => new BookManagementPanel(this),
                "members" => new MemberManagementPanel(),
                "issue" => new IssueBookPanel(),
                "return" => new ReturnBookPanel(),
                "search" => new SearchPanel(),
                "reports" => new ReportsPanel(),
                "settings" => new SettingsPanel(),
                _ => new DashboardPanel(this)
            };

            newControl.Name = key + "Panel";
            newControl.Dock = DockStyle.Fill;

            lblPageTitle.Text = key.ToUpper().Substring(0, 1) + key.Substring(1);

            contentPanel.Controls.Add(newControl);
            currentActiveControl = newControl;

            ThemeManager.ApplyTheme(newControl);
            AnimationHelper.FadeInControl(newControl);

            if (lblStatusBar != null)
            {
                lblStatusBar.Text = $"Active Section: {lblPageTitle.Text}. Shortcuts enabled.";
            }
        }

        private void MainForm_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.D: LoadPanel("dashboard"); sidebar?.SetActiveByKey("dashboard"); break;
                    case Keys.B: LoadPanel("books"); sidebar?.SetActiveByKey("books"); break;
                    case Keys.M: LoadPanel("members"); sidebar?.SetActiveByKey("members"); break;
                    case Keys.I: LoadPanel("issue"); sidebar?.SetActiveByKey("issue"); break;
                    case Keys.R: LoadPanel("return"); sidebar?.SetActiveByKey("return"); break;
                    case Keys.S: LoadPanel("search"); sidebar?.SetActiveByKey("search"); break;
                    case Keys.P: LoadPanel("reports"); sidebar?.SetActiveByKey("reports"); break;
                }
            }
        }

        private void OnThemeChanged()
        {
            this.BackColor = ThemeManager.BgColor;
            if (sidebar != null) sidebar.BackColor = ThemeManager.SidebarColor;
            if (contentPanel != null) contentPanel.BackColor = ThemeManager.BgColor;

            if (btnThemeToggle != null)
            {
                btnThemeToggle.Icon = ThemeManager.IsDarkMode ? IconHelper.LightMode : IconHelper.DarkMode;
                btnThemeToggle.Text = ThemeManager.IsDarkMode ? "Light" : "Dark";
                btnThemeToggle.BgColor = ThemeManager.AccentColor;
                btnThemeToggle.TextColor = ThemeManager.TextColor;
            }

            if (btnProfile != null)
            {
                btnProfile.BgColor = ThemeManager.PrimaryColor;
                btnProfile.HoverColor = ThemeManager.HoverColor;
            }

            ThemeManager.ApplyTheme(this);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            ThemeManager.ThemeChanged -= OnThemeChanged;
            clockTimer?.Stop();
            base.OnFormClosing(e);
        }
    }
}
