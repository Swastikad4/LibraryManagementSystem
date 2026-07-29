// ============================================================
// IconHelper.cs — Unicode Icon Constants
// Library Management System — UI Helpers
// ============================================================
// Provides Unicode emoji/symbol constants for menu icons.
// No external icon library needed — works natively in WinForms.
// ============================================================

namespace LibraryManagementSystem.UI.Helpers
{
    /// <summary>
    /// Provides Unicode icon constants for the application UI.
    /// These work natively in Windows Forms with Segoe UI Emoji font.
    /// </summary>
    public static class IconHelper
    {
        // ---- Sidebar Menu Icons ----
        public const string Dashboard = "📊";
        public const string Books = "📚";
        public const string Members = "👥";
        public const string IssueBook = "📤";
        public const string ReturnBook = "📥";
        public const string Search = "🔍";
        public const string Reports = "📋";
        public const string Settings = "⚙️";
        public const string Logout = "🚪";
        public const string Profile = "👤";

        // ---- Status Icons ----
        public const string Available = "✅";
        public const string Borrowed = "📖";
        public const string Overdue = "⚠️";
        public const string Returned = "✔️";

        // ---- Action Icons ----
        public const string Add = "➕";
        public const string Edit = "✏️";
        public const string Delete = "🗑️";
        public const string Save = "💾";
        public const string Cancel = "❌";
        public const string Print = "🖨️";
        public const string Export = "📁";
        public const string Refresh = "🔄";

        // ---- Dashboard Icons ----
        public const string TotalBooks = "📚";
        public const string AvailableBooks = "📗";
        public const string BorrowedBooks = "📕";
        public const string TotalMembers = "👥";
        public const string DueToday = "📅";
        public const string OverdueBooks = "⏰";

        // ---- Notification Icons ----
        public const string Success = "✅";
        public const string Error = "❌";
        public const string Warning = "⚠️";
        public const string Info = "ℹ️";

        // ---- Theme Icons ----
        public const string LightMode = "☀️";
        public const string DarkMode = "🌙";

        // ---- Other Icons ----
        public const string Lock = "🔒";
        public const string Key = "🔑";
        public const string Backup = "💿";
        public const string Restore = "📂";
        public const string Clock = "🕐";
        public const string Calendar = "📅";
        public const string Money = "💰";
        public const string Library = "🏛️";
    }
}
