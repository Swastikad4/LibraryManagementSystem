// ============================================================
// IssuedBook.cs — Issued Book Data Model
// Library Management System
// ============================================================
// Represents a book borrowing transaction.
// Tracks issue date, due date, return date, and fine.
// ============================================================

namespace LibraryManagementSystem.Models
{
    /// <summary>
    /// Represents a book issue/borrow transaction.
    /// Tracks when a book was issued, when it's due, and any fines.
    /// </summary>
    public class IssuedBook
    {
        // ---- Primary Key ----
        /// <summary>Auto-generated unique identifier for this transaction.</summary>
        public int IssueID { get; set; }

        // ---- Foreign Keys ----
        /// <summary>ID of the borrowed book.</summary>
        public int BookID { get; set; }

        /// <summary>ID of the member who borrowed the book.</summary>
        public int MemberID { get; set; }

        // ---- Transaction Dates ----
        /// <summary>Date when the book was issued/borrowed.</summary>
        public DateTime IssueDate { get; set; } = DateTime.Now;

        /// <summary>Date when the book is due for return (14 days from issue).</summary>
        public DateTime DueDate { get; set; }

        /// <summary>Date when the book was actually returned. Null if not yet returned.</summary>
        public DateTime? ReturnDate { get; set; }

        /// <summary>Fine amount in rupees (₹10 per day late).</summary>
        public decimal Fine { get; set; } = 0;

        // ---- Navigation Properties (for display purposes) ----
        /// <summary>Title of the borrowed book (populated from JOIN queries).</summary>
        public string BookTitle { get; set; } = string.Empty;

        /// <summary>Name of the member who borrowed (populated from JOIN queries).</summary>
        public string MemberName { get; set; } = string.Empty;

        // ---- Computed Properties ----

        /// <summary>
        /// Checks if the book is currently overdue (past due date and not yet returned).
        /// </summary>
        public bool IsOverdue => ReturnDate == null && DateTime.Now.Date > DueDate.Date;

        /// <summary>
        /// Checks if the book has been returned.
        /// </summary>
        public bool IsReturned => ReturnDate != null;

        /// <summary>
        /// Calculates the number of days the book is/was late.
        /// Returns 0 if the book is not late.
        /// </summary>
        public int LateDays
        {
            get
            {
                // Use return date if returned, otherwise use today
                DateTime checkDate = ReturnDate ?? DateTime.Now;

                // Calculate days late (only if past due date)
                int days = (checkDate.Date - DueDate.Date).Days;
                return days > 0 ? days : 0;
            }
        }

        /// <summary>
        /// Calculates the fine amount based on late days.
        /// Fine rate: ₹10 per day.
        /// </summary>
        public decimal CalculatedFine => LateDays * 10m;

        /// <summary>
        /// Returns the status of this issue transaction.
        /// </summary>
        public string Status
        {
            get
            {
                if (IsReturned) return "Returned";
                if (IsOverdue) return "Overdue";
                return "Issued";
            }
        }
    }
}
