using LibraryManagementSystem.DataAccess;
using LibraryManagementSystem.UI.Helpers;
using LibraryManagementSystem.UI.Controls;

namespace LibraryManagementSystem.UI.Forms
{
    public class SettingsPanel : UserControl
    {
        private RoundedButton? btnBackup;
        private RoundedButton? btnRestore;
        private CheckBox? chkDarkMode;

        public SettingsPanel()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.AutoScroll = true;

            var panel = new RoundedPanel
            {
                Size = new Size(500, 350),
                Location = new Point(15, 15),
                BackColor = ThemeManager.CardColor,
                CornerRadius = 12,
                BorderWidth = 1,
                BorderColor = ThemeManager.BorderColor
            };

            var lblHeader = new Label
            {
                Text = "System Configuration & Utility",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            panel.Controls.Add(lblHeader);

            // Dark Mode toggle
            chkDarkMode = new CheckBox
            {
                Text = "Enable Dark Mode",
                Font = new Font("Segoe UI", 10F),
                ForeColor = ThemeManager.TextColor,
                Location = new Point(25, 70),
                AutoSize = true,
                Checked = ThemeManager.IsDarkMode,
                Cursor = Cursors.Hand
            };
            chkDarkMode.CheckedChanged += ChkDarkMode_CheckedChanged;
            panel.Controls.Add(chkDarkMode);

            // Database utility label
            var lblDb = new Label
            {
                Text = "Database Control (Local SQLite file)",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Location = new Point(20, 130),
                AutoSize = true
            };
            panel.Controls.Add(lblDb);

            btnBackup = new RoundedButton
            {
                Text = "Backup Database",
                Icon = IconHelper.Backup,
                BgColor = ThemeManager.PrimaryColor,
                HoverColor = ThemeManager.HoverColor,
                Location = new Point(25, 170),
                Size = new Size(200, 42),
                CornerRadius = 8
            };
            btnBackup.Click += BtnBackup_Click;
            panel.Controls.Add(btnBackup);

            btnRestore = new RoundedButton
            {
                Text = "Restore Database",
                Icon = IconHelper.Restore,
                BgColor = ThemeManager.AccentColor,
                TextColor = ThemeManager.TextColor,
                HoverColor = ThemeManager.BorderColor,
                Location = new Point(25, 230),
                Size = new Size(200, 42),
                CornerRadius = 8
            };
            btnRestore.Click += BtnRestore_Click;
            panel.Controls.Add(btnRestore);

            this.Controls.Add(panel);
            ThemeManager.ApplyTheme(this);
        }

        private void ChkDarkMode_CheckedChanged(object? sender, EventArgs e)
        {
            if (chkDarkMode == null) return;
            ThemeManager.SetTheme(chkDarkMode.Checked);
        }

        private void BtnBackup_Click(object? sender, EventArgs e)
        {
            using var sfd = new SaveFileDialog
            {
                Filter = "SQLite Database (*.db)|*.db",
                FileName = $"library_backup_{DateTime.Now:yyyyMMdd_HHmmss}.db"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string source = DatabaseHelper.GetDatabasePath();
                    File.Copy(source, sfd.FileName, true);
                    MessageBox.Show("Database backed up successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Backup failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnRestore_Click(object? sender, EventArgs e)
        {
            var confirm = MessageBox.Show("Restoring will overwrite current data. Do you want to continue?", "Confirm Restore",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                using var ofd = new OpenFileDialog
                {
                    Filter = "SQLite Database (*.db)|*.db"
                };

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string target = DatabaseHelper.GetDatabasePath();
                        File.Copy(ofd.FileName, target, true);
                        MessageBox.Show("Database restored successfully! Restart the application to load new data.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Restore failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
