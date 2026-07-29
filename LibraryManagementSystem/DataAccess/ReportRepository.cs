// ============================================================
// ReportRepository.cs — Report Data Access
// Library Management System — Data Access Layer
// ============================================================
// Provides data for reports and charts.
// Includes aggregated queries for dashboard charts.
// ============================================================

using System.Data.SQLite;

namespace LibraryManagementSystem.DataAccess
{
    /// <summary>
    /// Repository class for generating report data and chart statistics.
    /// </summary>
    public class ReportRepository
    {
        /// <summary>
        /// Gets the count of books grouped by category.
        /// Used for the "Books by Category" pie chart.
        /// </summary>
        /// <returns>Dictionary with category names as keys and counts as values.</returns>
        public Dictionary<string, int> GetBooksByCategory()
        {
            var result = new Dictionary<string, int>();

            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(@"
                SELECT Category, COUNT(*) as Count 
                FROM Books 
                GROUP BY Category 
                ORDER BY Count DESC", connection);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result[reader["Category"].ToString()!] =
                    Convert.ToInt32(reader["Count"]);
            }

            return result;
        }

        /// <summary>
        /// Gets monthly borrowing statistics for the current year.
        /// Used for the "Monthly Borrowing" bar chart.
        /// </summary>
        /// <returns>Dictionary with month names as keys and counts as values.</returns>
        public Dictionary<string, int> GetMonthlyBorrowing()
        {
            var result = new Dictionary<string, int>();

            // Initialize all months with 0
            string[] months = { "Jan", "Feb", "Mar", "Apr", "May", "Jun",
                               "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            foreach (var month in months)
            {
                result[month] = 0;
            }

            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(@"
                SELECT strftime('%m', IssueDate) as Month, COUNT(*) as Count
                FROM IssuedBooks
                WHERE strftime('%Y', IssueDate) = strftime('%Y', 'now')
                GROUP BY strftime('%m', IssueDate)
                ORDER BY Month", connection);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int monthNum = Convert.ToInt32(reader["Month"]);
                result[months[monthNum - 1]] = Convert.ToInt32(reader["Count"]);
            }

            return result;
        }

        /// <summary>
        /// Gets the top 5 most borrowed books.
        /// Used for the "Most Borrowed Books" chart.
        /// </summary>
        /// <returns>Dictionary with book titles as keys and borrow counts as values.</returns>
        public Dictionary<string, int> GetMostBorrowedBooks()
        {
            var result = new Dictionary<string, int>();

            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(@"
                SELECT b.Title, COUNT(*) as BorrowCount
                FROM IssuedBooks i
                INNER JOIN Books b ON i.BookID = b.BookID
                GROUP BY b.BookID, b.Title
                ORDER BY BorrowCount DESC
                LIMIT 5", connection);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result[reader["Title"].ToString()!] =
                    Convert.ToInt32(reader["BorrowCount"]);
            }

            return result;
        }

        /// <summary>
        /// Gets books with low stock (available copies less than or equal to threshold).
        /// Used for notifications.
        /// </summary>
        /// <param name="threshold">The threshold for low stock (default: 1).</param>
        public List<(string Title, int Available)> GetLowStockBooks(int threshold = 1)
        {
            var result = new List<(string, int)>();

            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(@"
                SELECT Title, AvailableCopies 
                FROM Books 
                WHERE AvailableCopies <= @Threshold
                ORDER BY AvailableCopies ASC", connection);
            command.Parameters.AddWithValue("@Threshold", threshold);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add((
                    reader["Title"].ToString()!,
                    Convert.ToInt32(reader["AvailableCopies"])
                ));
            }

            return result;
        }

        /// <summary>
        /// Gets books with approaching due dates (within 2 days).
        /// Used for notifications.
        /// </summary>
        public List<(string BookTitle, string MemberName, DateTime DueDate)>
            GetApproachingDueBooks()
        {
            var result = new List<(string, string, DateTime)>();

            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(@"
                SELECT b.Title, m.Name, i.DueDate
                FROM IssuedBooks i
                INNER JOIN Books b ON i.BookID = b.BookID
                INNER JOIN Members m ON i.MemberID = m.MemberID
                WHERE i.ReturnDate IS NULL
                  AND date(i.DueDate) BETWEEN date('now') AND date('now', '+2 days')
                ORDER BY i.DueDate ASC", connection);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add((
                    reader["Title"].ToString()!,
                    reader["Name"].ToString()!,
                    DateTime.Parse(reader["DueDate"].ToString()!)
                ));
            }

            return result;
        }
    }
}
