// ============================================================
// IssueReturnService.cs — Issue & Return Business Logic
// Library Management System — Business Logic Layer
// ============================================================
// Handles the core business logic for issuing and returning books.
// Enforces rules: max 3 books per member, book availability,
// fine calculation (₹10 per late day).
// ============================================================

using LibraryManagementSystem.DataAccess;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.BusinessLogic
{
    /// <summary>
    /// Service class for issuing and returning books.
    /// Enforces all borrowing business rules.
    /// </summary>
    public class IssueReturnService
    {
        private readonly IssuedBookRepository _issuedRepo = new();
        private readonly BookRepository _bookRepo = new();
        private readonly MemberRepository _memberRepo = new();

        // ---- Constants ----
        /// <summary>Maximum number of books a member can borrow at once.</summary>
        private const int MaxBooksPerMember = 3;

        /// <summary>Number of days a book can be borrowed (loan period).</summary>
        private const int LoanDays = 14;

        /// <summary>Fine per day in rupees for late returns.</summary>
        private const decimal FinePerDay = 10m;

        // ---- Issue Operations ----

        /// <summary>
        /// Issues a book to a member with all business rule checks.
        /// </summary>
        /// <param name="bookId">ID of the book to issue.</param>
        /// <param name="memberId">ID of the member borrowing the book.</param>
        /// <returns>Tuple with success flag and message.</returns>
        public (bool Success, string Message) IssueBook(int bookId, int memberId)
        {
            try
            {
                // Step 1: Verify the book exists
                var book = _bookRepo.GetById(bookId);
                if (book == null)
                    return (false, "Book not found.");

                // Step 2: Check if book is available
                if (book.AvailableCopies <= 0)
                    return (false, $"'{book.Title}' is not available for borrowing.");

                // Step 3: Verify the member exists
                var member = _memberRepo.GetById(memberId);
                if (member == null)
                    return (false, "Member not found.");

                // Step 4: Check if member has reached the maximum borrow limit
                int currentBorrows = _memberRepo.GetBorrowCount(memberId);
                if (currentBorrows >= MaxBooksPerMember)
                    return (false,
                        $"Member '{member.Name}' has already borrowed {MaxBooksPerMember} books (maximum limit).");

                // Step 5: Calculate dates
                DateTime issueDate = DateTime.Now;
                DateTime dueDate = issueDate.AddDays(LoanDays);

                // Step 6: Create the issue record
                int issueId = _issuedRepo.IssueBook(bookId, memberId, issueDate, dueDate);

                // Step 7: Reduce available copies
                _bookRepo.UpdateAvailableCopies(bookId, -1);

                return (true,
                    $"Book '{book.Title}' issued to '{member.Name}' successfully!\n" +
                    $"Issue ID: {issueId}\n" +
                    $"Due Date: {dueDate:dd-MM-yyyy}");
            }
            catch (Exception ex)
            {
                return (false, $"Error issuing book: {ex.Message}");
            }
        }

        // ---- Return Operations ----

        /// <summary>
        /// Returns a borrowed book and calculates any applicable fine.
        /// </summary>
        /// <param name="issueId">The issue ID of the borrow record.</param>
        /// <returns>Tuple with success flag, message, and fine amount.</returns>
        public (bool Success, string Message, decimal Fine) ReturnBook(int issueId)
        {
            try
            {
                // Step 1: Get the issue record
                var allIssued = _issuedRepo.GetAll();
                var issued = allIssued.FirstOrDefault(i => i.IssueID == issueId);

                if (issued == null)
                    return (false, "Issue record not found.", 0);

                if (issued.IsReturned)
                    return (false, "This book has already been returned.", 0);

                // Step 2: Calculate return date and fine
                DateTime returnDate = DateTime.Now;
                int lateDays = 0;

                if (returnDate.Date > issued.DueDate.Date)
                {
                    lateDays = (returnDate.Date - issued.DueDate.Date).Days;
                }

                decimal fine = lateDays * FinePerDay;

                // Step 3: Update the issue record
                _issuedRepo.ReturnBook(issueId, returnDate, fine);

                // Step 4: Increase available copies
                _bookRepo.UpdateAvailableCopies(issued.BookID, +1);

                // Step 5: Build result message
                string message = $"Book '{issued.BookTitle}' returned successfully!";
                if (lateDays > 0)
                {
                    message += $"\n\nLate by {lateDays} day(s).\nFine: ₹{fine:N2}";
                }

                return (true, message, fine);
            }
            catch (Exception ex)
            {
                return (false, $"Error returning book: {ex.Message}", 0);
            }
        }

        // ---- Query Operations ----

        /// <summary>Gets all issued books.</summary>
        public List<IssuedBook> GetAllIssued() => _issuedRepo.GetAll();

        /// <summary>Gets currently issued (unreturned) books.</summary>
        public List<IssuedBook> GetCurrentlyIssued() =>
            _issuedRepo.GetCurrentlyIssued();

        /// <summary>Gets overdue books.</summary>
        public List<IssuedBook> GetOverdueBooks() => _issuedRepo.GetOverdueBooks();

        /// <summary>Gets books due today.</summary>
        public List<IssuedBook> GetBooksDueToday() => _issuedRepo.GetBooksDueToday();

        /// <summary>Gets books borrowed by a specific member.</summary>
        public List<IssuedBook> GetBooksByMember(int memberId) =>
            _issuedRepo.GetByMember(memberId);

        /// <summary>Gets borrowed book count.</summary>
        public int GetBorrowedCount() => _issuedRepo.GetBorrowedCount();

        /// <summary>Gets overdue book count.</summary>
        public int GetOverdueCount() => _issuedRepo.GetOverdueCount();

        /// <summary>Gets books due today count.</summary>
        public int GetDueTodayCount() => _issuedRepo.GetDueTodayCount();

        /// <summary>Searches issued books.</summary>
        public List<IssuedBook> SearchIssued(string searchTerm) =>
            _issuedRepo.Search(searchTerm);
    }
}
