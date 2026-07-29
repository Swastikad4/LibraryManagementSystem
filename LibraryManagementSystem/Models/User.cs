// ============================================================
// User.cs — Admin User Data Model
// Library Management System
// ============================================================
// Represents an administrator user for the system.
// Used for login authentication and profile management.
// ============================================================

namespace LibraryManagementSystem.Models
{
    /// <summary>
    /// Represents an admin user of the library system.
    /// Used for authentication and profile management.
    /// </summary>
    public class User
    {
        // ---- Primary Key ----
        /// <summary>Auto-generated unique identifier for the user.</summary>
        public int UserID { get; set; }

        // ---- User Details ----
        /// <summary>Login username (unique).</summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>SHA256 hashed password.</summary>
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>Full display name of the user.</summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>Role of the user (currently only "Admin").</summary>
        public string Role { get; set; } = "Admin";
    }
}
