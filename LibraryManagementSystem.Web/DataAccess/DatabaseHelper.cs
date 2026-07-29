using System.Data.SQLite;
using System.Security.Cryptography;
using System.Text;
using LibraryManagementSystem.Web.Models;

namespace LibraryManagementSystem.Web.DataAccess
{
    public static class DatabaseHelper
    {
        private static readonly string DbFileName = "library.db";
        private static readonly string DbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DbFileName);

        public static string ConnectionString => $"Data Source={DbPath};Version=3;";

        public static SQLiteConnection GetConnection() => new SQLiteConnection(ConnectionString);

        public static void InitializeDatabase()
        {
            if (!File.Exists(DbPath))
            {
                SQLiteConnection.CreateFile(DbPath);
            }

            using var connection = GetConnection();
            connection.Open();

            string createTablesSQL = @"
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

                CREATE TABLE IF NOT EXISTS Members (
                    MemberID         INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name             TEXT NOT NULL,
                    Email            TEXT NOT NULL,
                    Phone            TEXT NOT NULL,
                    Address          TEXT NOT NULL,
                    RegistrationDate TEXT NOT NULL DEFAULT (date('now'))
                );

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

                CREATE TABLE IF NOT EXISTS Users (
                    UserID       INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username     TEXT NOT NULL UNIQUE,
                    PasswordHash TEXT NOT NULL,
                    FullName     TEXT NOT NULL,
                    Role         TEXT NOT NULL DEFAULT 'Admin'
                );

                CREATE TABLE IF NOT EXISTS Magazines (
                    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title       TEXT NOT NULL,
                    Publisher   TEXT NOT NULL,
                    IssueDate   TEXT NOT NULL,
                    Language    TEXT NOT NULL,
                    Category    TEXT NOT NULL,
                    Description TEXT,
                    Status      TEXT NOT NULL DEFAULT 'Available',
                    CreatedAt   TEXT NOT NULL DEFAULT (date('now'))
                );

                CREATE TABLE IF NOT EXISTS Newspapers (
                    Id           INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title        TEXT NOT NULL,
                    Publisher    TEXT NOT NULL,
                    PublishedDate TEXT NOT NULL,
                    Language     TEXT NOT NULL,
                    Edition      TEXT NOT NULL,
                    Description  TEXT,
                    Status       TEXT NOT NULL DEFAULT 'Available',
                    CreatedAt    TEXT NOT NULL DEFAULT (date('now'))
                );
            ";

            using var command = new SQLiteCommand(createTablesSQL, connection);
            command.ExecuteNonQuery();

            SeedDefaultData(connection);
        }

        private static void SeedDefaultData(SQLiteConnection connection)
        {
            // Seed Admin User (admin / admin123)
            using (var checkCmd = new SQLiteCommand("SELECT COUNT(*) FROM Users", connection))
            {
                if (Convert.ToInt32(checkCmd.ExecuteScalar()) == 0)
                {
                    using var insertCmd = new SQLiteCommand(@"
                        INSERT INTO Users (Username, PasswordHash, FullName, Role)
                        VALUES (@Username, @PasswordHash, @FullName, @Role)", connection);
                    insertCmd.Parameters.AddWithValue("@Username", "admin");
                    insertCmd.Parameters.AddWithValue("@PasswordHash", HashPassword("admin123"));
                    insertCmd.Parameters.AddWithValue("@FullName", "Administrator");
                    insertCmd.Parameters.AddWithValue("@Role", "Admin");
                    insertCmd.ExecuteNonQuery();
                }
            }

            // Seed Sample Books
            using (var checkCmd = new SQLiteCommand("SELECT COUNT(*) FROM Books", connection))
            {
                if (Convert.ToInt32(checkCmd.ExecuteScalar()) == 0)
                {
                    var books = new[]
                    {
                        ("The Alchemist", "Paulo Coelho", "978-0062315007", "Fiction", "HarperOne", 3, 2, "A-01", 1988),
                        ("Atomic Habits", "James Clear", "978-0735211292", "Self Help", "Avery", 5, 5, "B-03", 2018),
                        ("Clean Code", "Robert C. Martin", "978-0132350884", "Programming", "Pearson", 4, 4, "C-02", 2008),
                        ("Introduction to Algorithms", "Thomas H. Cormen", "978-0262033848", "Computer Science", "MIT Press", 2, 2, "C-05", 2009),
                        ("Rich Dad Poor Dad", "Robert Kiyosaki", "978-1612680194", "Finance", "Plata Publishing", 3, 3, "D-01", 1997)
                    };

                    foreach (var b in books)
                    {
                        using var cmd = new SQLiteCommand(@"
                            INSERT INTO Books (Title, Author, ISBN, Category, Publisher, Quantity, AvailableCopies, ShelfNo, PublishedYear)
                            VALUES (@Title, @Author, @ISBN, @Category, @Publisher, @Quantity, @AvailableCopies, @ShelfNo, @PublishedYear)", connection);
                        cmd.Parameters.AddWithValue("@Title", b.Item1);
                        cmd.Parameters.AddWithValue("@Author", b.Item2);
                        cmd.Parameters.AddWithValue("@ISBN", b.Item3);
                        cmd.Parameters.AddWithValue("@Category", b.Item4);
                        cmd.Parameters.AddWithValue("@Publisher", b.Item5);
                        cmd.Parameters.AddWithValue("@Quantity", b.Item6);
                        cmd.Parameters.AddWithValue("@AvailableCopies", b.Item7);
                        cmd.Parameters.AddWithValue("@ShelfNo", b.Item8);
                        cmd.Parameters.AddWithValue("@PublishedYear", b.Item9);
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            // Seed Sample Members
            using (var checkCmd = new SQLiteCommand("SELECT COUNT(*) FROM Members", connection))
            {
                if (Convert.ToInt32(checkCmd.ExecuteScalar()) == 0)
                {
                    var members = new[]
                    {
                        ("Rahul Sharma", "rahul@email.com", "9876543210", "Mumbai, Maharashtra"),
                        ("Priya Patel", "priya@email.com", "9123456789", "Delhi, India"),
                        ("Amit Kumar", "amit@email.com", "9988776655", "Bangalore, Karnataka"),
                        ("Sneha Reddy", "sneha@email.com", "9112233445", "Hyderabad, Telangana"),
                        ("Vikram Singh", "vikram@email.com", "9001122334", "Jaipur, Rajasthan")
                    };

                    foreach (var m in members)
                    {
                        using var cmd = new SQLiteCommand(@"
                            INSERT INTO Members (Name, Email, Phone, Address, RegistrationDate)
                            VALUES (@Name, @Email, @Phone, @Address, @RegDate)", connection);
                        cmd.Parameters.AddWithValue("@Name", m.Item1);
                        cmd.Parameters.AddWithValue("@Email", m.Item2);
                        cmd.Parameters.AddWithValue("@Phone", m.Item3);
                        cmd.Parameters.AddWithValue("@Address", m.Item4);
                        cmd.Parameters.AddWithValue("@RegDate", DateTime.Now.ToString("yyyy-MM-dd"));
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            // Seed Sample Magazines
            using (var checkCmd = new SQLiteCommand("SELECT COUNT(*) FROM Magazines", connection))
            {
                if (Convert.ToInt32(checkCmd.ExecuteScalar()) == 0)
                {
                    var magazines = new[]
                    {
                        ("National Geographic", "National Geographic Partners", "2024-01-15", "English", "Science & Nature", "Monthly magazine featuring science, geography, and nature.", "Available"),
                        ("Forbes", "Forbes Media", "2024-02-01", "English", "Business", "Business magazine covering finance, investing, and technology.", "Available"),
                        ("Time Magazine", "Time USA", "2024-01-20", "English", "News", "Weekly news magazine covering current events.", "Available"),
                        ("Reader's Digest", "Trusted Media Brands", "2024-01-10", "English", "Lifestyle", "General interest family magazine.", "Available"),
                        ("PC World", "IDG Communications", "2024-02-05", "English", "Technology", "Computer and technology magazine.", "Available")
                    };

                    foreach (var m in magazines)
                    {
                        using var cmd = new SQLiteCommand(@"
                            INSERT INTO Magazines (Title, Publisher, IssueDate, Language, Category, Description, Status, CreatedAt)
                            VALUES (@Title, @Publisher, @IssueDate, @Language, @Category, @Description, @Status, @CreatedAt)", connection);
                        cmd.Parameters.AddWithValue("@Title", m.Item1);
                        cmd.Parameters.AddWithValue("@Publisher", m.Item2);
                        cmd.Parameters.AddWithValue("@IssueDate", m.Item3);
                        cmd.Parameters.AddWithValue("@Language", m.Item4);
                        cmd.Parameters.AddWithValue("@Category", m.Item5);
                        cmd.Parameters.AddWithValue("@Description", m.Item6);
                        cmd.Parameters.AddWithValue("@Status", m.Item7);
                        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now.ToString("yyyy-MM-dd"));
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            // Seed Sample Newspapers
            using (var checkCmd = new SQLiteCommand("SELECT COUNT(*) FROM Newspapers", connection))
            {
                if (Convert.ToInt32(checkCmd.ExecuteScalar()) == 0)
                {
                    var newspapers = new[]
                    {
                        ("The New York Times", "The New York Times Company", "2024-02-01", "English", "Daily", "American daily newspaper based in New York City.", "Available"),
                        ("The Guardian", "Guardian Media Group", "2024-02-01", "English", "Daily", "British daily newspaper.", "Available"),
                        ("The Washington Post", "Graham Holdings Company", "2024-02-01", "English", "Daily", "American daily newspaper published in Washington, D.C.", "Available"),
                        ("USA Today", "Gannett Company", "2024-02-01", "English", "Daily", "American nationally circulated newspaper.", "Available"),
                        ("The Daily Telegraph", "Telegraph Media Group", "2024-02-01", "English", "Daily", "British daily broadsheet newspaper.", "Available")
                    };

                    foreach (var n in newspapers)
                    {
                        using var cmd = new SQLiteCommand(@"
                            INSERT INTO Newspapers (Title, Publisher, PublishedDate, Language, Edition, Description, Status, CreatedAt)
                            VALUES (@Title, @Publisher, @PublishedDate, @Language, @Edition, @Description, @Status, @CreatedAt)", connection);
                        cmd.Parameters.AddWithValue("@Title", n.Item1);
                        cmd.Parameters.AddWithValue("@Publisher", n.Item2);
                        cmd.Parameters.AddWithValue("@PublishedDate", n.Item3);
                        cmd.Parameters.AddWithValue("@Language", n.Item4);
                        cmd.Parameters.AddWithValue("@Edition", n.Item5);
                        cmd.Parameters.AddWithValue("@Description", n.Item6);
                        cmd.Parameters.AddWithValue("@Status", n.Item7);
                        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now.ToString("yyyy-MM-dd"));
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var sb = new StringBuilder();
            foreach (byte b in bytes) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        public static string GetDatabasePath() => DbPath;
    }
}
