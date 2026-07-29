// ============================================================
// ThemeManager.cs — Dark/Light Mode Theme Manager
// Library Management System — UI Helpers
// ============================================================
// Manages the application's color theme.
// Supports Light Mode (Dusty Rose) and Dark Mode.
// Recursively applies colors to all controls.
// ============================================================

namespace LibraryManagementSystem.UI.Helpers
{
    /// <summary>
    /// Manages the application's visual theme (Light/Dark mode).
    /// Uses the Dusty Rose color palette specified in the design requirements.
    /// </summary>
    public static class ThemeManager
    {
        // ---- Theme State ----
        /// <summary>Whether dark mode is currently active.</summary>
        public static bool IsDarkMode { get; private set; } = false;

        // ============================================================
        // LIGHT MODE — Dusty Rose Palette (Default)
        // ============================================================
        public static class Light
        {
            public static readonly Color Background = ColorTranslator.FromHtml("#FFF8F5");   // Soft Beige
            public static readonly Color Sidebar = ColorTranslator.FromHtml("#8E6A6A");      // Dusty Rose
            public static readonly Color SidebarText = Color.White;
            public static readonly Color Primary = ColorTranslator.FromHtml("#B67C7C");      // Rose
            public static readonly Color Hover = ColorTranslator.FromHtml("#9B5F5F");        // Mauve
            public static readonly Color Card = Color.White;                                  // White
            public static readonly Color Accent = ColorTranslator.FromHtml("#F5E6E8");       // Cream
            public static readonly Color Text = ColorTranslator.FromHtml("#343434");         // Dark Grey
            public static readonly Color TextSecondary = ColorTranslator.FromHtml("#7A7A7A");
            public static readonly Color Border = ColorTranslator.FromHtml("#E8D5D5");
            public static readonly Color GridHeader = ColorTranslator.FromHtml("#8E6A6A");
            public static readonly Color GridHeaderText = Color.White;
            public static readonly Color GridAltRow = ColorTranslator.FromHtml("#FDF2EF");
            public static readonly Color Success = ColorTranslator.FromHtml("#4CAF50");
            public static readonly Color Danger = ColorTranslator.FromHtml("#E53935");
            public static readonly Color Warning = ColorTranslator.FromHtml("#FF9800");
            public static readonly Color Info = ColorTranslator.FromHtml("#2196F3");
        }

        // ============================================================
        // DARK MODE
        // ============================================================
        public static class Dark
        {
            public static readonly Color Background = ColorTranslator.FromHtml("#0A0A0A");   // Deep Black
            public static readonly Color Sidebar = ColorTranslator.FromHtml("#4A3535");      // Dark Rose
            public static readonly Color SidebarText = ColorTranslator.FromHtml("#E5E5E5");
            public static readonly Color Primary = ColorTranslator.FromHtml("#8E6A6A");      // Muted Rose
            public static readonly Color Hover = ColorTranslator.FromHtml("#6B4A4A");        // Deep Mauve
            public static readonly Color Card = ColorTranslator.FromHtml("#141414");         // Dark Card
            public static readonly Color Accent = ColorTranslator.FromHtml("#2A2020");       // Dark Cream
            public static readonly Color Text = ColorTranslator.FromHtml("#E5E5E5");         // Light Grey
            public static readonly Color TextSecondary = ColorTranslator.FromHtml("#A3A3A3");
            public static readonly Color Border = ColorTranslator.FromHtml("#262626");
            public static readonly Color GridHeader = ColorTranslator.FromHtml("#4A3535");
            public static readonly Color GridHeaderText = ColorTranslator.FromHtml("#E5E5E5");
            public static readonly Color GridAltRow = ColorTranslator.FromHtml("#1A1A1A");
            public static readonly Color Success = ColorTranslator.FromHtml("#66BB6A");
            public static readonly Color Danger = ColorTranslator.FromHtml("#EF5350");
            public static readonly Color Warning = ColorTranslator.FromHtml("#FFA726");
            public static readonly Color Info = ColorTranslator.FromHtml("#42A5F5");
        }

        // ============================================================
        // Current Theme Color Accessors
        // ============================================================

        /// <summary>Gets the current background color based on theme.</summary>
        public static Color BgColor => IsDarkMode ? Dark.Background : Light.Background;

        /// <summary>Gets the current sidebar color.</summary>
        public static Color SidebarColor => IsDarkMode ? Dark.Sidebar : Light.Sidebar;

        /// <summary>Gets the current sidebar text color.</summary>
        public static Color SidebarTextColor => IsDarkMode ? Dark.SidebarText : Light.SidebarText;

        /// <summary>Gets the current primary color.</summary>
        public static Color PrimaryColor => IsDarkMode ? Dark.Primary : Light.Primary;

        /// <summary>Gets the current hover color.</summary>
        public static Color HoverColor => IsDarkMode ? Dark.Hover : Light.Hover;

        /// <summary>Gets the current card background color.</summary>
        public static Color CardColor => IsDarkMode ? Dark.Card : Light.Card;

        /// <summary>Gets the current accent color.</summary>
        public static Color AccentColor => IsDarkMode ? Dark.Accent : Light.Accent;

        /// <summary>Gets the current primary text color.</summary>
        public static Color TextColor => IsDarkMode ? Dark.Text : Light.Text;

        /// <summary>Gets the current secondary text color.</summary>
        public static Color TextSecondaryColor => IsDarkMode ? Dark.TextSecondary : Light.TextSecondary;

        /// <summary>Gets the current border color.</summary>
        public static Color BorderColor => IsDarkMode ? Dark.Border : Light.Border;

        /// <summary>Gets the current grid header color.</summary>
        public static Color GridHeaderColor => IsDarkMode ? Dark.GridHeader : Light.GridHeader;

        /// <summary>Gets the current grid header text color.</summary>
        public static Color GridHeaderTextColor => IsDarkMode ? Dark.GridHeaderText : Light.GridHeaderText;

        /// <summary>Gets the current grid alternating row color.</summary>
        public static Color GridAltRowColor => IsDarkMode ? Dark.GridAltRow : Light.GridAltRow;

        /// <summary>Gets success color.</summary>
        public static Color SuccessColor => IsDarkMode ? Dark.Success : Light.Success;

        /// <summary>Gets danger color.</summary>
        public static Color DangerColor => IsDarkMode ? Dark.Danger : Light.Danger;

        /// <summary>Gets warning color.</summary>
        public static Color WarningColor => IsDarkMode ? Dark.Warning : Light.Warning;

        /// <summary>Gets info color.</summary>
        public static Color InfoColor => IsDarkMode ? Dark.Info : Light.Info;

        // ============================================================
        // Theme Toggle
        // ============================================================

        /// <summary>
        /// Event raised when the theme changes.
        /// Subscribe to this in forms to update their appearance.
        /// </summary>
        public static event Action? ThemeChanged;

        /// <summary>
        /// Toggles between light and dark mode.
        /// </summary>
        public static void ToggleTheme()
        {
            IsDarkMode = !IsDarkMode;
            ThemeChanged?.Invoke();
        }

        /// <summary>
        /// Sets the theme explicitly.
        /// </summary>
        public static void SetTheme(bool darkMode)
        {
            IsDarkMode = darkMode;
            ThemeChanged?.Invoke();
        }

        // ============================================================
        // Apply Theme to Controls
        // ============================================================

        /// <summary>
        /// Applies the current theme to a form and all its child controls recursively.
        /// </summary>
        /// <param name="form">The form to apply the theme to.</param>
        public static void ApplyTheme(Form form)
        {
            form.BackColor = BgColor;
            form.ForeColor = TextColor;
            ApplyThemeToControls(form.Controls);
        }

        /// <summary>
        /// Applies the current theme to a UserControl and all its children.
        /// </summary>
        public static void ApplyTheme(UserControl control)
        {
            control.BackColor = BgColor;
            control.ForeColor = TextColor;
            ApplyThemeToControls(control.Controls);
        }

        /// <summary>
        /// Recursively applies theme colors to a collection of controls.
        /// </summary>
        private static void ApplyThemeToControls(Control.ControlCollection controls)
        {
            foreach (Control ctrl in controls)
            {
                // Skip controls tagged with "no-theme" to preserve custom colors
                if (ctrl.Tag?.ToString() == "no-theme")
                    continue;

                // Apply based on control type
                switch (ctrl)
                {
                    case DataGridView dgv:
                        StyleDataGridView(dgv);
                        break;

                    case TextBox tb:
                        tb.BackColor = CardColor;
                        tb.ForeColor = TextColor;
                        tb.BorderStyle = BorderStyle.FixedSingle;
                        break;

                    case ComboBox cb:
                        cb.BackColor = CardColor;
                        cb.ForeColor = TextColor;
                        break;

                    case Panel panel when panel.Tag?.ToString() == "card":
                        panel.BackColor = CardColor;
                        break;

                    case Panel panel when panel.Tag?.ToString() == "sidebar":
                        panel.BackColor = SidebarColor;
                        break;

                    case Label lbl when lbl.Tag?.ToString() == "secondary":
                        lbl.ForeColor = TextSecondaryColor;
                        break;

                    case Label lbl:
                        lbl.ForeColor = TextColor;
                        break;
                }

                // Recursively apply to child controls
                if (ctrl.Controls.Count > 0)
                {
                    ApplyThemeToControls(ctrl.Controls);
                }
            }
        }

        /// <summary>
        /// Applies theme styling to a DataGridView control.
        /// Creates a professional look with themed headers and alternating rows.
        /// </summary>
        public static void StyleDataGridView(DataGridView dgv)
        {
            // General settings
            dgv.BackgroundColor = CardColor;
            dgv.GridColor = BorderColor;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.EnableHeadersVisualStyles = false;

            // Header styling
            dgv.ColumnHeadersDefaultCellStyle.BackColor = GridHeaderColor;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = GridHeaderTextColor;
            dgv.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 6, 8, 6);
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = GridHeaderColor;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersHeight = 45;

            // Row styling
            dgv.DefaultCellStyle.BackColor = CardColor;
            dgv.DefaultCellStyle.ForeColor = TextColor;
            dgv.DefaultCellStyle.SelectionBackColor = AccentColor;
            dgv.DefaultCellStyle.SelectionForeColor = TextColor;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dgv.DefaultCellStyle.Padding = new Padding(8, 4, 8, 4);

            // Alternating row styling
            dgv.AlternatingRowsDefaultCellStyle.BackColor = GridAltRowColor;

            // Row dimensions
            dgv.RowTemplate.Height = 42;
            dgv.RowHeadersVisible = false;

            // Selection mode
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = true;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
    }
}
