// ============================================================
// IssuedBookRepository.cs — Issued Books Data Access
// Library Management System — Data Access Layer
// ============================================================
// Handles all database operations for the IssuedBooks table.
// Includes JOIN queries to get book titles and member names.
// ============================================================

using System.Data.SQLite;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.DataAccess
{
    /// <summary>
    /// Repository class for IssuedBook operations.
    /// Manages book issuing and returning transactions.
    /// </summary>
    public class IssuedBookRepository
    {
        // ---- READ Operations ----

        /// <summary>
        /// Gets all issued books with book titles and member names (JOIN query).
        /// </summary>
        public List<IssuedBook> GetAll()
        {
            var issues = new List<IssuedBook>();

            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(@"
                SELECT i.*, b.Title AS BookTitle, m.Name AS MemberName
                FROM IssuedBooks i
                INNER JOIN Books b ON i.BookID = b.BookID
                INNER JOIN Members m ON i.MemberID = m.MemberID
                ORDER BY i.IssueDate DESC", connection);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                issues.Add(MapReaderToIssuedBook(reader));
            }

            return issues;
        }

        /// <summary>
        /// Gets all currently issued (not returned) books.
        /// </summary>
        public List<IssuedBook> GetCurrentlyIssued()
        {
            var issues = new List<IssuedBook>();

            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(@"
                SELECT i.*, b.Title AS BookTitle, m.Name AS MemberName
                FROM IssuedBooks i
                INNER JOIN Books b ON i.BookID = b.BookID
                INNER JOIN Members m ON i.MemberID = m.MemberID
                WHERE i.ReturnDate IS NULL
                ORDER BY i.DueDate ASC", connection);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                issues.Add(MapReaderToIssuedBook(reader));
            }

            return issues;
        }

        /// <summary>
        /// Gets all overdue books (past due date and not returned).
        /// </summary>
        public List<IssuedBook> GetOverdueBooks()
        {
            var issues = new List<IssuedBook>();

            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(@"
                SELECT i.*, b.Title AS BookTitle, m.Name AS MemberName
                FROM IssuedBooks i
                INNER JOIN Books b ON i.BookID = b.BookID
                INNER JOIN Members m ON i.MemberID = m.MemberID
                WHERE i.ReturnDate IS NULL 
                  AND date(i.DueDate) < date('now')
                ORDER BY i.DueDate ASC", connection);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                issues.Add(MapReaderToIssuedBook(reader));
            }

            return issues;
        }

        /// <summary>
        /// Gets books due today.
        /// </summary>
        public List<IssuedBook> GetBooksDueToday()
        {
            var issues = new List<IssuedBook>();

            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(@"
                SELECT i.*, b.Title AS BookTitle, m.Name AS MemberName
                FROM IssuedBooks i
                INNER JOIN Books b ON i.BookID = b.BookID
                INNER JOIN Members m ON i.MemberID = m.MemberID
                WHERE i.ReturnDate IS NULL 
                  AND date(i.DueDate) = date('now')
                ORDER BY i.DueDate ASC", connection);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                issues.Add(MapReaderToIssuedBook(reader));
            }

            return issues;
        }

        /// <summary>
        /// Gets all books currently borrowed by a specific member.
        /// </summary>
        public List<IssuedBook> GetByMember(int memberId)
        {
            var issues = new List<IssuedBook>();

            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(@"
                SELECT i.*, b.Title AS BookTitle, m.Name AS MemberName
                FROM IssuedBooks i
                INNER JOIN Books b ON i.BookID = b.BookID
                INNER JOIN Members m ON i.MemberID = m.MemberID
                WHERE i.MemberID = @MemberID AND i.ReturnDate IS NULL
                ORDER BY i.DueDate ASC", connection);
            command.Parameters.AddWithValue("@MemberID", memberId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                issues.Add(MapReaderToIssuedBook(reader));
            }

            return issues;
        }

        /// <summary>
        /// Gets the total count of currently borrowed books.
        /// </summary>
        public int GetBorrowedCount()
        {
            using var connection = DatabaseHelper.GetConnection();
            connection.Open();
            using var command = new SQLiteCommand(
                "SELECT COUNT(*) FROM IssuedBooks WHERE ReturnDate IS NULL",
                connection);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        /// <summary>
        /// Gets the count of overdue books.
        /// </summary>
        public int GetOverdueCount()
        {
            using var connection = DatabaseHelper.GetConnection();
            connection.Open();
            using var command = new SQLiteCommand(@"
                SELECT COUNT(*) FROM IssuedBooks 
                WHERE ReturnDate IS NULL AND date(DueDate) < date('now')",
                connection);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        /// <summary>
        /// Gets the count of books due today.
        /// </summary>
        public int GetDueTodayCount()
        {
            using var connection = DatabaseHelper.GetConnection();
            connection.Open();
            using var command = new SQLiteCommand(@"
                SELECT COUNT(*) FROM IssuedBooks 
                WHERE ReturnDate IS NULL AND date(DueDate) = date('now')",
                connection);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        // ---- ISSUE (CREATE) Operation ----

        /// <summary>
        /// Issues a book to a member (creates a new borrow record).
        /// </summary>
        public int IssueBook(int bookId, int memberId, DateTime issueDate, DateTime dueDate)
        {
            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(@"
                INSERT INTO IssuedBooks (BookID, MemberID, IssueDate, DueDate)
                VALUES (@BookID, @MemberID, @IssueDate, @DueDate);
                SELECT last_insert_rowid();",
                connection);

            command.Parameters.AddWithValue("@BookID", bookId);
            command.Parameters.AddWithValue("@MemberID", memberId);
            command.Parameters.AddWithValue("@IssueDate",
                issueDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@DueDate",
                dueDate.ToString("yyyy-MM-dd"));

            return Convert.ToInt32(command.ExecuteScalar());
        }

        // ---- RETURN (UPDATE) Operation ----

        /// <summary>
        /// Records the return of a book and updates the fine.
        /// </summary>
        public void ReturnBook(int issueId, DateTime returnDate, decimal fine)
        {
            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(@"
                UPDATE IssuedBooks SET
                    ReturnDate = @ReturnDate,
                    Fine = @Fine
                WHERE IssueID = @IssueID",
                connection);

            command.Parameters.AddWithValue("@ReturnDate",
                returnDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@Fine", fine);
            command.Parameters.AddWithValue("@IssueID", issueId);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Searches issued books by book title or member name.
        /// </summary>
        public List<IssuedBook> Search(string searchTerm)
        {
            var issues = new List<IssuedBook>();
            string term = $"%{searchTerm}%";

            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(@"
                SELECT i.*, b.Title AS BookTitle, m.Name AS MemberName
                FROM IssuedBooks i
                INNER JOIN Books b ON i.BookID = b.BookID
                INNER JOIN Members m ON i.MemberID = m.MemberID
                WHERE b.Title LIKE @Term 
                   OR m.Name LIKE @Term
                   OR CAST(i.IssueID AS TEXT) LIKE @Term
                ORDER BY i.IssueDate DESC", connection);
            command.Parameters.AddWithValue("@Term", term);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                issues.Add(MapReaderToIssuedBook(reader));
            }

            return issues;
        }

        // ---- Helper Methods ----

        /// <summary>Maps a data reader row to an IssuedBook object.</summary>
        private IssuedBook MapReaderToIssuedBook(SQLiteDataReader reader)
        {
            var issued = new IssuedBook
            {
                IssueID = Convert.ToInt32(reader["IssueID"]),
                BookID = Convert.ToInt32(reader["BookID"]),
                MemberID = Convert.ToInt32(reader["MemberID"]),
                IssueDate = DateTime.Parse(reader["IssueDate"].ToString()!),
                DueDate = DateTime.Parse(reader["DueDate"].ToString()!),
                Fine = reader["Fine"] != DBNull.Value
                    ? Convert.ToDecimal(reader["Fine"]) : 0,
                BookTitle = reader["BookTitle"].ToString()!,
                MemberName = reader["MemberName"].ToString()!
            };

            // ReturnDate can be null if book hasn't been returned
            if (reader["ReturnDate"] != DBNull.Value)
            {
                issued.ReturnDate = DateTime.Parse(reader["ReturnDate"].ToString()!);
            }

            return issued;
        }
    }
}
