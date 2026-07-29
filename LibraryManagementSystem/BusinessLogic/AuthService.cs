// ============================================================
// AuthService.cs — Authentication Business Logic
// Library Management System — Business Logic Layer
// ============================================================
// Handles login, logout, and session management.
// Stores the currently logged-in user.
// ============================================================

using LibraryManagementSystem.DataAccess;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.BusinessLogic
{
    /// <summary>
    /// Service class for authentication and session management.
    /// Maintains a static reference to the currently logged-in user.
    /// </summary>
    public class AuthService
    {
        private readonly UserRepository _repository = new();

        /// <summary>
        /// The currently logged-in user. Null if no one is logged in.
        /// Static so it's accessible throughout the application.
        /// </summary>
        public static User? CurrentUser { get; private set; }

        /// <summary>
        /// Checks if a user is currently logged in.
        /// </summary>
        public static bool IsLoggedIn => CurrentUser != null;

        /// <summary>
        /// Attempts to log in with the provided credentials.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The plain text password (will be hashed).</param>
        /// <returns>Tuple with success flag and message.</returns>
        public (bool Success, string Message) Login(string username, string password)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(username))
                    return (false, "Username is required.");

                if (string.IsNullOrWhiteSpace(password))
                    return (false, "Password is required.");

                // Hash the password and authenticate
                string passwordHash = DatabaseHelper.HashPassword(password);
                var user = _repository.Authenticate(username, passwordHash);

                if (user != null)
                {
                    // Set the current user session
                    CurrentUser = user;
                    return (true, $"Welcome, {user.FullName}!");
                }

                return (false, "Invalid username or password.");
            }
            catch (Exception ex)
            {
                return (false, $"Login error: {ex.Message}");
            }
        }

        /// <summary>
        /// Logs out the current user by clearing the session.
        /// </summary>
        public static void Logout()
        {
            CurrentUser = null;
        }

        /// <summary>
        /// Changes the password for the current user.
        /// </summary>
        /// <param name="currentPassword">Current password for verification.</param>
        /// <param name="newPassword">New password to set.</param>
        /// <param name="confirmPassword">Confirmation of the new password.</param>
        public (bool Success, string Message) ChangePassword(
            string currentPassword, string newPassword, string confirmPassword)
        {
            try
            {
                if (CurrentUser == null)
                    return (false, "No user is logged in.");

                // Validate inputs
                if (string.IsNullOrWhiteSpace(currentPassword))
                    return (false, "Current password is required.");

                if (string.IsNullOrWhiteSpace(newPassword))
                    return (false, "New password is required.");

                if (newPassword.Length < 4)
                    return (false, "New password must be at least 4 characters.");

                if (newPassword != confirmPassword)
                    return (false, "New password and confirmation do not match.");

                // Verify current password
                string currentHash = DatabaseHelper.HashPassword(currentPassword);
                if (currentHash != CurrentUser.PasswordHash)
                    return (false, "Current password is incorrect.");

                // Update password
                string newHash = DatabaseHelper.HashPassword(newPassword);
                _repository.ChangePassword(CurrentUser.UserID, newHash);

                // Update the session
                CurrentUser.PasswordHash = newHash;

                return (true, "Password changed successfully!");
            }
            catch (Exception ex)
            {
                return (false, $"Error changing password: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the profile name for the current user.
        /// </summary>
        public (bool Success, string Message) UpdateProfile(string fullName)
        {
            try
            {
                if (CurrentUser == null)
                    return (false, "No user is logged in.");

                if (string.IsNullOrWhiteSpace(fullName))
                    return (false, "Full name is required.");

                _repository.UpdateProfile(CurrentUser.UserID, fullName);
                CurrentUser.FullName = fullName;

                return (true, "Profile updated successfully!");
            }
            catch (Exception ex)
            {
                return (false, $"Error updating profile: {ex.Message}");
            }
        }
    }
}
