// ============================================================
// Book.cs — Book Data Model
// Library Management System
// ============================================================
// Represents a book in the library with all its properties.
// Includes validation methods for data integrity.
// ============================================================

namespace LibraryManagementSystem.Models
{
    /// <summary>
    /// Represents a book entity in the library system.
    /// Contains all book details and validation logic.
    /// </summary>
    public class Book
    {
        // ---- Primary Key ----
        /// <summary>Auto-generated unique identifier for the book.</summary>
        public int BookID { get; set; }

        // ---- Book Details ----
        /// <summary>Title of the book.</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>Author of the book.</summary>
        public string Author { get; set; } = string.Empty;

        /// <summary>International Standard Book Number (must be unique).</summary>
        public string ISBN { get; set; } = string.Empty;

        /// <summary>Category/Genre of the book (e.g., Fiction, Programming).</summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>Publisher of the book.</summary>
        public string Publisher { get; set; } = string.Empty;

        /// <summary>Total quantity of copies in the library.</summary>
        public int Quantity { get; set; } = 1;

        /// <summary>Number of copies currently available for borrowing.</summary>
        public int AvailableCopies { get; set; } = 1;

        /// <summary>Shelf number where the book is located.</summary>
        public string ShelfNo { get; set; } = string.Empty;

        /// <summary>Year the book was published.</summary>
        public int PublishedYear { get; set; }

        // ---- Computed Properties ----

        /// <summary>
        /// Returns the availability status of the book.
        /// </summary>
        public string Status => AvailableCopies > 0 ? "Available" : "Borrowed";

        /// <summary>
        /// Checks if the book is available for borrowing.
        /// </summary>
        public bool IsAvailable => AvailableCopies > 0;

        // ---- Validation ----

        /// <summary>
        /// Validates all book fields and returns a list of error messages.
        /// Returns an empty list if all fields are valid.
        /// </summary>
        public List<string> Validate()
        {
            var errors = new List<string>();

            // Check for empty required fields
            if (string.IsNullOrWhiteSpace(Title))
                errors.Add("Book title is required.");

            if (string.IsNullOrWhiteSpace(Author))
                errors.Add("Author name is required.");

            if (string.IsNullOrWhiteSpace(ISBN))
                errors.Add("ISBN is required.");

            if (string.IsNullOrWhiteSpace(Category))
                errors.Add("Category is required.");

            if (string.IsNullOrWhiteSpace(Publisher))
                errors.Add("Publisher is required.");

            if (string.IsNullOrWhiteSpace(ShelfNo))
                errors.Add("Shelf number is required.");

            // Quantity cannot be negative
            if (Quantity < 0)
                errors.Add("Quantity cannot be negative.");

            // Available copies cannot exceed total quantity
            if (AvailableCopies < 0)
                errors.Add("Available copies cannot be negative.");

            if (AvailableCopies > Quantity)
                errors.Add("Available copies cannot exceed total quantity.");

            // Published year should be reasonable
            if (PublishedYear < 1000 || PublishedYear > DateTime.Now.Year + 1)
                errors.Add("Published year is not valid.");

            return errors;
        }
    }
}
