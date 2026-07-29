// ============================================================
// UserRepository.cs — User Data Access
// Library Management System — Data Access Layer
// ============================================================
// Handles authentication and user management operations.
// ============================================================

using System.Data.SQLite;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.DataAccess
{
    /// <summary>
    /// Repository class for User authentication and management.
    /// </summary>
    public class UserRepository
    {
        /// <summary>
        /// Authenticates a user by username and password hash.
        /// </summary>
        /// <param name="username">The username to check.</param>
        /// <param name="passwordHash">SHA256 hash of the password.</param>
        /// <returns>The User object if authenticated, null otherwise.</returns>
        public User? Authenticate(string username, string passwordHash)
        {
            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(@"
                SELECT * FROM Users 
                WHERE Username = @Username AND PasswordHash = @PasswordHash",
                connection);

            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@PasswordHash", passwordHash);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    UserID = Convert.ToInt32(reader["UserID"]),
                    Username = reader["Username"].ToString()!,
                    PasswordHash = reader["PasswordHash"].ToString()!,
                    FullName = reader["FullName"].ToString()!,
                    Role = reader["Role"].ToString()!
                };
            }

            return null;
        }

        /// <summary>
        /// Gets a user by their ID.
        /// </summary>
        public User? GetById(int userId)
        {
            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(
                "SELECT * FROM Users WHERE UserID = @UserID", connection);
            command.Parameters.AddWithValue("@UserID", userId);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    UserID = Convert.ToInt32(reader["UserID"]),
                    Username = reader["Username"].ToString()!,
                    PasswordHash = reader["PasswordHash"].ToString()!,
                    FullName = reader["FullName"].ToString()!,
                    Role = reader["Role"].ToString()!
                };
            }

            return null;
        }

        /// <summary>
        /// Changes a user's password.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="newPasswordHash">SHA256 hash of the new password.</param>
        public void ChangePassword(int userId, string newPasswordHash)
        {
            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(@"
                UPDATE Users SET PasswordHash = @PasswordHash 
                WHERE UserID = @UserID",
                connection);

            command.Parameters.AddWithValue("@PasswordHash", newPasswordHash);
            command.Parameters.AddWithValue("@UserID", userId);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Updates a user's full name.
        /// </summary>
        public void UpdateProfile(int userId, string fullName)
        {
            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(@"
                UPDATE Users SET FullName = @FullName 
                WHERE UserID = @UserID",
                connection);

            command.Parameters.AddWithValue("@FullName", fullName);
            command.Parameters.AddWithValue("@UserID", userId);
            command.ExecuteNonQuery();
        }
    }
}
