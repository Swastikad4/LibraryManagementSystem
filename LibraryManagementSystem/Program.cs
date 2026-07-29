// ============================================================
// Program.cs — Application Entry Point
// Library Management System
// ============================================================
// This is the starting point of the application.
// It configures high-DPI support, sets the default font,
// and launches the Login form.
// ============================================================

namespace LibraryManagementSystem
{
    /// <summary>
    /// Main entry point for the Library Management System application.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// Configures visual styles, DPI settings, and launches the login form.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Enable modern visual styles for controls
            Application.EnableVisualStyles();

            // Use compatible text rendering for better font display
            Application.SetCompatibleTextRenderingDefault(false);

            // Enable high-DPI support for sharp rendering on modern displays
            Application.SetHighDpiMode(HighDpiMode.SystemAware);

            // Set the default font for the entire application
            // Using Segoe UI which is the standard modern Windows font
            Application.SetDefaultFont(new Font("Segoe UI", 10F, FontStyle.Regular));

            // Initialize the database on first run
            // This creates tables and seeds default data if they don't exist
            DataAccess.DatabaseHelper.InitializeDatabase();

            // Launch the Login form as the starting window
            Application.Run(new UI.Forms.LoginForm());
        }
    }
}
