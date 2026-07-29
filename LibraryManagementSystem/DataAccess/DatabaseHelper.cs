// ============================================================
// DatabaseHelper.cs — Database Connection & Initialization
// Library Management System — Data Access Layer
// ============================================================
// Manages the SQLite database connection.
// Creates tables and seeds default data on first run.
// ============================================================

using System.Data.SQLite;
using System.Security.Cryptography;
using System.Text;

namespace LibraryManagementSystem.DataAccess
{
    /// <summary>
    /// Provides database connection management and initialization.
    /// Creates the SQLite database file, tables, and seed data on first run.
    /// </summary>
    public static class DatabaseHelper
    {
        // ---- Database file path (stored in the application directory) ----
        private static readonly string DbFileName = "library.db";
        private static readonly string DbPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, DbFileName);

        /// <summary>
        /// Connection string for the SQLite database.
        /// </summary>
        public static string ConnectionString =>
            $"Data Source={DbPath};Version=3;";

        /// <summary>
        /// Creates and returns a new SQLite database connection.
        /// Always use this inside a 'using' statement to ensure proper disposal.
        /// </summary>
        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(ConnectionString);
        }

        /// <summary>
        /// Initializes the database by creating tables and seeding data.
        /// Safe to call multiple times — uses IF NOT EXISTS.
        /// </summary>
        public static void InitializeDatabase()
        {
            try
            {
                // Create the database file if it doesn't exist
                if (!File.Exists(DbPath))
                {
                    SQLiteConnection.CreateFile(DbPath);
                }

                using var connection = GetConnection();
                connection.Open();

                // Create all tables
                CreateTables(connection);

                // Seed default data (only if tables are empty)
                SeedDefaultData(connection);
            }
            catch (Exception ex)
            {
                // Show error to user if database initialization fails
                MessageBox.Show(
                    $"Failed to initialize database:\n{ex.Message}",
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Creates all required database tables if they don't exist.
        /// </summary>
        private static void CreateTables(SQLiteConnection connection)
        {
            string createTablesSQL = @"
                -- Books table: stores all book information
                CREATE TABLE IF NOT EXISTS Books (
                    BookID          INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title           TEXT NOT NULL,
                    Author          TEXT NOT NULL,
                    ISBN            TEXT NOT NULL UNIQUE,
                    Category        TEXT NOT NULL,
                    Publisher       TEXT NOT NULL,
                    Quantity        INTEGER NOT NULL DEFAULT 1,
                    AvailableCopies INTEGER NOT NULL DEFAULT 1,
                    ShelfNo         TEXT NOT NULL,
                    PublishedYear   INTEGER NOT NULL
                );

                -- Members table: stores library member information
                CREATE TABLE IF NOT EXISTS Members (
                    MemberID         INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name             TEXT NOT NULL,
                    Email            TEXT NOT NULL,
                    Phone            TEXT NOT NULL,
                    Address          TEXT NOT NULL,
                    RegistrationDate TEXT NOT NULL DEFAULT (date('now'))
                );

                -- IssuedBooks table: tracks book borrowing transactions
                CREATE TABLE IF NOT EXISTS IssuedBooks (
                    IssueID    INTEGER PRIMARY KEY AUTOINCREMENT,
                    BookID     INTEGER NOT NULL,
                    MemberID   INTEGER NOT NULL,
                    IssueDate  TEXT NOT NULL DEFAULT (date('now')),
                    DueDate    TEXT NOT NULL,
                    ReturnDate TEXT,
                    Fine       REAL DEFAULT 0,
                    FOREIGN KEY (BookID)   REFERENCES Books(BookID),
                    FOREIGN KEY (MemberID) REFERENCES Members(MemberID)
                );

                -- Users table: stores admin login credentials
                CREATE TABLE IF NOT EXISTS Users (
                    UserID       INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username     TEXT NOT NULL UNIQUE,
                    PasswordHash TEXT NOT NULL,
                    FullName     TEXT NOT NULL,
                    Role         TEXT NOT NULL DEFAULT 'Admin'
                );
            ";

            using var command = new SQLiteCommand(createTablesSQL, connection);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Seeds default data into the database if tables are empty.
        /// Includes: default admin user and sample books from the existing HTML site.
        /// </summary>
        private static void SeedDefaultData(SQLiteConnection connection)
        {
            // ---- Seed default admin user ----
            SeedAdminUser(connection);

            // ---- Seed sample books (from existing HTML site) ----
            SeedSampleBooks(connection);

            // ---- Seed sample members ----
            SeedSampleMembers(connection);
        }

        /// <summary>
        /// Creates the default admin account if no users exist.
        /// Default credentials: admin / admin123
        /// </summary>
        private static void SeedAdminUser(SQLiteConnection connection)
        {
            // Check if any users exist
            using var checkCmd = new SQLiteCommand(
                "SELECT COUNT(*) FROM Users", connection);
            long count = (long)checkCmd.ExecuteScalar();

            if (count == 0)
            {
                // Hash the default password
                string hashedPassword = HashPassword("admin123");

                using var insertCmd = new SQLiteCommand(@"
                    INSERT INTO Users (Username, PasswordHash, FullName, Role)
                    VALUES (@Username, @PasswordHash, @FullName, @Role)",
                    connection);

                insertCmd.Parameters.AddWithValue("@Username", "admin");
                insertCmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                insertCmd.Parameters.AddWithValue("@FullName", "Administrator");
                insertCmd.Parameters.AddWithValue("@Role", "Admin");
                insertCmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Seeds sample books from the existing HTML site if no books exist.
        /// </summary>
        private static void SeedSampleBooks(SQLiteConnection connection)
        {
            // Check if any books exist
            using var checkCmd = new SQLiteCommand(
                "SELECT COUNT(*) FROM Books", connection);
            long count = (long)checkCmd.ExecuteScalar();

            if (count == 0)
            {
                // Sample books matching the existing HTML/CSS site
                var books = new[]
                {
                    ("The Alchemist", "Paulo Coelho", "978-0062315007", "Fiction", "HarperOne", 3, 2, "A-01", 1988),
                    ("Atomic Habits", "James Clear", "978-0735211292", "Self Help", "Avery", 5, 5, "B-03", 2018),
                    ("Clean Code", "Robert C. Martin", "978-0132350884", "Programming", "Pearson", 4, 4, "C-02", 2008),
                    ("Introduction to Algorithms", "Thomas H. Cormen", "978-0262033848", "Computer Science", "MIT Press", 2, 2, "C-05", 2009),
                    ("Rich Dad Poor Dad", "Robert Kiyosaki", "978-1612680194", "Finance", "Plata Publishing", 3, 3, "D-01", 1997)
                };

                foreach (var book in books)
                {
                    using var insertCmd = new SQLiteCommand(@"
                        INSERT INTO Books (Title, Author, ISBN, Category, Publisher, 
                                          Quantity, AvailableCopies, ShelfNo, PublishedYear)
                        VALUES (@Title, @Author, @ISBN, @Category, @Publisher,
                                @Quantity, @AvailableCopies, @ShelfNo, @PublishedYear)",
                        connection);

                    insertCmd.Parameters.AddWithValue("@Title", book.Item1);
                    insertCmd.Parameters.AddWithValue("@Author", book.Item2);
                    insertCmd.Parameters.AddWithValue("@ISBN", book.Item3);
                    insertCmd.Parameters.AddWithValue("@Category", book.Item4);
                    insertCmd.Parameters.AddWithValue("@Publisher", book.Item5);
                    insertCmd.Parameters.AddWithValue("@Quantity", book.Item6);
                    insertCmd.Parameters.AddWithValue("@AvailableCopies", book.Item7);
                    insertCmd.Parameters.AddWithValue("@ShelfNo", book.Item8);
                    insertCmd.Parameters.AddWithValue("@PublishedYear", book.Item9);
                    insertCmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Seeds sample members if no members exist.
        /// </summary>
        private static void SeedSampleMembers(SQLiteConnection connection)
        {
            using var checkCmd = new SQLiteCommand(
                "SELECT COUNT(*) FROM Members", connection);
            long count = (long)checkCmd.ExecuteScalar();

            if (count == 0)
            {
                var members = new[]
                {
                    ("Rahul Sharma", "rahul@email.com", "9876543210", "Mumbai, Maharashtra"),
                    ("Priya Patel", "priya@email.com", "9123456789", "Delhi, India"),
                    ("Amit Kumar", "amit@email.com", "9988776655", "Bangalore, Karnataka"),
                    ("Sneha Reddy", "sneha@email.com", "9112233445", "Hyderabad, Telangana"),
                    ("Vikram Singh", "vikram@email.com", "9001122334", "Jaipur, Rajasthan")
                };

                foreach (var member in members)
                {
                    using var insertCmd = new SQLiteCommand(@"
                        INSERT INTO Members (Name, Email, Phone, Address, RegistrationDate)
                        VALUES (@Name, @Email, @Phone, @Address, @RegDate)",
                        connection);

                    insertCmd.Parameters.AddWithValue("@Name", member.Item1);
                    insertCmd.Parameters.AddWithValue("@Email", member.Item2);
                    insertCmd.Parameters.AddWithValue("@Phone", member.Item3);
                    insertCmd.Parameters.AddWithValue("@Address", member.Item4);
                    insertCmd.Parameters.AddWithValue("@RegDate",
                        DateTime.Now.ToString("yyyy-MM-dd"));
                    insertCmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Hashes a password using SHA256 for secure storage.
        /// </summary>
        /// <param name="password">Plain text password to hash.</param>
        /// <returns>Hex string of the SHA256 hash.</returns>
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                sb.Append(b.ToString("x2")); // Convert each byte to hex
            }
            return sb.ToString();
        }

        /// <summary>
        /// Gets the full path to the database file.
        /// Used for backup and restore operations.
        /// </summary>
        public static string GetDatabasePath() => DbPath;
    }
}
