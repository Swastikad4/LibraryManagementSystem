// ============================================================
// BookRepository.cs — Book Data Access
// Library Management System — Data Access Layer
// ============================================================
// Handles all database operations for the Books table.
// Uses parameterized SQL queries to prevent SQL injection.
// ============================================================

using System.Data.SQLite;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.DataAccess
{
    /// <summary>
    /// Repository class for Book CRUD operations.
    /// All methods use parameterized queries for security.
    /// </summary>
    public class BookRepository
    {
        // ---- READ Operations ----

        /// <summary>
        /// Retrieves all books from the database.
        /// </summary>
        public List<Book> GetAll()
        {
            var books = new List<Book>();

            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(
                "SELECT * FROM Books ORDER BY Title", connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                books.Add(MapReaderToBook(reader));
            }

            return books;
        }

        /// <summary>
        /// Retrieves a single book by its ID.
        /// </summary>
        /// <param name="bookId">The ID of the book to retrieve.</param>
        /// <returns>The book object, or null if not found.</returns>
        public Book? GetById(int bookId)
        {
            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(
                "SELECT * FROM Books WHERE BookID = @BookID", connection);
            command.Parameters.AddWithValue("@BookID", bookId);

            using var reader = command.ExecuteReader();
            return reader.Read() ? MapReaderToBook(reader) : null;
        }

        /// <summary>
        /// Searches books by title, author, or ISBN.
        /// </summary>
        /// <param name="searchTerm">The search term to match.</param>
        /// <returns>List of matching books.</returns>
        public List<Book> Search(string searchTerm)
        {
            var books = new List<Book>();
            string term = $"%{searchTerm}%";

            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(@"
                SELECT * FROM Books 
                WHERE Title LIKE @Term 
                   OR Author LIKE @Term 
                   OR ISBN LIKE @Term
                   OR Category LIKE @Term
                ORDER BY Title", connection);
            command.Parameters.AddWithValue("@Term", term);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                books.Add(MapReaderToBook(reader));
            }

            return books;
        }

        /// <summary>
        /// Retrieves books filtered by category.
        /// </summary>
        /// <param name="category">The category to filter by.</param>
        public List<Book> GetByCategory(string category)
        {
            var books = new List<Book>();

            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(
                "SELECT * FROM Books WHERE Category = @Category ORDER BY Title",
                connection);
            command.Parameters.AddWithValue("@Category", category);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                books.Add(MapReaderToBook(reader));
            }

            return books;
        }

        /// <summary>
        /// Retrieves all books that have available copies.
        /// </summary>
        public List<Book> GetAvailableBooks()
        {
            var books = new List<Book>();

            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(
                "SELECT * FROM Books WHERE AvailableCopies > 0 ORDER BY Title",
                connection);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                books.Add(MapReaderToBook(reader));
            }

            return books;
        }

        /// <summary>
        /// Gets all distinct categories from the Books table.
        /// Used to populate category filter dropdowns.
        /// </summary>
        public List<string> GetCategories()
        {
            var categories = new List<string>();

            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(
                "SELECT DISTINCT Category FROM Books ORDER BY Category",
                connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                categories.Add(reader["Category"].ToString()!);
            }

            return categories;
        }

        /// <summary>
        /// Gets the total count of all books.
        /// </summary>
        public int GetTotalCount()
        {
            using var connection = DatabaseHelper.GetConnection();
            connection.Open();
            using var command = new SQLiteCommand(
                "SELECT COALESCE(SUM(Quantity), 0) FROM Books", connection);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        /// <summary>
        /// Gets the total count of available copies.
        /// </summary>
        public int GetAvailableCount()
        {
            using var connection = DatabaseHelper.GetConnection();
            connection.Open();
            using var command = new SQLiteCommand(
                "SELECT COALESCE(SUM(AvailableCopies), 0) FROM Books", connection);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        /// <summary>
        /// Checks if an ISBN already exists in the database.
        /// Used for validation when adding new books.
        /// </summary>
        /// <param name="isbn">ISBN to check.</param>
        /// <param name="excludeBookId">Optional book ID to exclude (for edit mode).</param>
        public bool IsISBNExists(string isbn, int excludeBookId = 0)
        {
            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(@"
                SELECT COUNT(*) FROM Books 
                WHERE ISBN = @ISBN AND BookID != @ExcludeID",
                connection);
            command.Parameters.AddWithValue("@ISBN", isbn);
            command.Parameters.AddWithValue("@ExcludeID", excludeBookId);

            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }

        // ---- CREATE Operation ----

        /// <summary>
        /// Adds a new book to the database.
        /// </summary>
        /// <param name="book">The book to add.</param>
        /// <returns>The ID of the newly created book.</returns>
        public int Add(Book book)
        {
            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(@"
                INSERT INTO Books (Title, Author, ISBN, Category, Publisher,
                                   Quantity, AvailableCopies, ShelfNo, PublishedYear)
                VALUES (@Title, @Author, @ISBN, @Category, @Publisher,
                        @Quantity, @AvailableCopies, @ShelfNo, @PublishedYear);
                SELECT last_insert_rowid();",
                connection);

            AddBookParameters(command, book);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        // ---- UPDATE Operations ----

        /// <summary>
        /// Updates an existing book in the database.
        /// </summary>
        /// <param name="book">The book with updated values.</param>
        public void Update(Book book)
        {
            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(@"
                UPDATE Books SET
                    Title = @Title,
                    Author = @Author,
                    ISBN = @ISBN,
                    Category = @Category,
                    Publisher = @Publisher,
                    Quantity = @Quantity,
                    AvailableCopies = @AvailableCopies,
                    ShelfNo = @ShelfNo,
                    PublishedYear = @PublishedYear
                WHERE BookID = @BookID",
                connection);

            AddBookParameters(command, book);
            command.Parameters.AddWithValue("@BookID", book.BookID);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Updates the available copies count for a book.
        /// Called when books are issued or returned.
        /// </summary>
        /// <param name="bookId">The book ID.</param>
        /// <param name="change">The change amount (+1 for return, -1 for issue).</param>
        public void UpdateAvailableCopies(int bookId, int change)
        {
            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(@"
                UPDATE Books 
                SET AvailableCopies = AvailableCopies + @Change
                WHERE BookID = @BookID",
                connection);
            command.Parameters.AddWithValue("@Change", change);
            command.Parameters.AddWithValue("@BookID", bookId);
            command.ExecuteNonQuery();
        }

        // ---- DELETE Operation ----

        /// <summary>
        /// Deletes a book from the database.
        /// </summary>
        /// <param name="bookId">The ID of the book to delete.</param>
        public void Delete(int bookId)
        {
            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(
                "DELETE FROM Books WHERE BookID = @BookID", connection);
            command.Parameters.AddWithValue("@BookID", bookId);
            command.ExecuteNonQuery();
        }

        // ---- Helper Methods ----

        /// <summary>
        /// Maps a SQLite data reader row to a Book object.
        /// </summary>
        private Book MapReaderToBook(SQLiteDataReader reader)
        {
            return new Book
            {
                BookID = Convert.ToInt32(reader["BookID"]),
                Title = reader["Title"].ToString()!,
                Author = reader["Author"].ToString()!,
                ISBN = reader["ISBN"].ToString()!,
                Category = reader["Category"].ToString()!,
                Publisher = reader["Publisher"].ToString()!,
                Quantity = Convert.ToInt32(reader["Quantity"]),
                AvailableCopies = Convert.ToInt32(reader["AvailableCopies"]),
                ShelfNo = reader["ShelfNo"].ToString()!,
                PublishedYear = Convert.ToInt32(reader["PublishedYear"])
            };
        }

        /// <summary>
        /// Adds common book parameters to a SQLite command.
        /// Reduces code duplication between Add and Update methods.
        /// </summary>
        private void AddBookParameters(SQLiteCommand command, Book book)
        {
            command.Parameters.AddWithValue("@Title", book.Title);
            command.Parameters.AddWithValue("@Author", book.Author);
            command.Parameters.AddWithValue("@ISBN", book.ISBN);
            command.Parameters.AddWithValue("@Category", book.Category);
            command.Parameters.AddWithValue("@Publisher", book.Publisher);
            command.Parameters.AddWithValue("@Quantity", book.Quantity);
            command.Parameters.AddWithValue("@AvailableCopies", book.AvailableCopies);
            command.Parameters.AddWithValue("@ShelfNo", book.ShelfNo);
            command.Parameters.AddWithValue("@PublishedYear", book.PublishedYear);
        }
    }
}
