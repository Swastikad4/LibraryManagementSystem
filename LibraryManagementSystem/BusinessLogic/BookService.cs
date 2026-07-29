// ============================================================
// BookService.cs — Book Business Logic
// Library Management System — Business Logic Layer
// ============================================================
// Validates book data and delegates to the repository.
// Acts as a bridge between UI and Data Access layers.
// ============================================================

using LibraryManagementSystem.DataAccess;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.BusinessLogic
{
    /// <summary>
    /// Service class that handles book business rules and validation.
    /// Sits between the UI layer and the data access layer.
    /// </summary>
    public class BookService
    {
        private readonly BookRepository _repository = new();

        /// <summary>Gets all books.</summary>
        public List<Book> GetAllBooks() => _repository.GetAll();

        /// <summary>Gets a book by ID.</summary>
        public Book? GetBookById(int bookId) => _repository.GetById(bookId);

        /// <summary>Searches books by title, author, or ISBN.</summary>
        public List<Book> SearchBooks(string searchTerm) =>
            _repository.Search(searchTerm);

        /// <summary>Gets books by category.</summary>
        public List<Book> GetBooksByCategory(string category) =>
            _repository.GetByCategory(category);

        /// <summary>Gets available books.</summary>
        public List<Book> GetAvailableBooks() => _repository.GetAvailableBooks();

        /// <summary>Gets all distinct categories.</summary>
        public List<string> GetCategories() => _repository.GetCategories();

        /// <summary>Gets total book count.</summary>
        public int GetTotalCount() => _repository.GetTotalCount();

        /// <summary>Gets available book count.</summary>
        public int GetAvailableCount() => _repository.GetAvailableCount();

        /// <summary>
        /// Adds a new book with validation.
        /// Returns a tuple: (success flag, error message or empty string).
        /// </summary>
        public (bool Success, string Message) AddBook(Book book)
        {
            try
            {
                // Step 1: Validate the book model
                var errors = book.Validate();
                if (errors.Count > 0)
                    return (false, string.Join("\n", errors));

                // Step 2: Check ISBN uniqueness
                if (_repository.IsISBNExists(book.ISBN))
                    return (false, "A book with this ISBN already exists.");

                // Step 3: Set available copies to quantity for new books
                book.AvailableCopies = book.Quantity;

                // Step 4: Add to database
                int newId = _repository.Add(book);
                return (true, $"Book added successfully! (ID: {newId})");
            }
            catch (Exception ex)
            {
                return (false, $"Error adding book: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing book with validation.
        /// </summary>
        public (bool Success, string Message) UpdateBook(Book book)
        {
            try
            {
                // Step 1: Validate the book model
                var errors = book.Validate();
                if (errors.Count > 0)
                    return (false, string.Join("\n", errors));

                // Step 2: Check ISBN uniqueness (excluding current book)
                if (_repository.IsISBNExists(book.ISBN, book.BookID))
                    return (false, "Another book with this ISBN already exists.");

                // Step 3: Update in database
                _repository.Update(book);
                return (true, "Book updated successfully!");
            }
            catch (Exception ex)
            {
                return (false, $"Error updating book: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a book by ID.
        /// </summary>
        public (bool Success, string Message) DeleteBook(int bookId)
        {
            try
            {
                _repository.Delete(bookId);
                return (true, "Book deleted successfully!");
            }
            catch (Exception ex)
            {
                return (false, $"Error deleting book: {ex.Message}");
            }
        }
    }
}
