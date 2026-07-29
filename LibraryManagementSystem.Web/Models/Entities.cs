namespace LibraryManagementSystem.Web.Models
{
    public class Book
    {
        public int BookID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public int Quantity { get; set; } = 1;
        public int AvailableCopies { get; set; } = 1;
        public string ShelfNo { get; set; } = string.Empty;
        public int PublishedYear { get; set; }

        public string Status => AvailableCopies > 0 ? "Available" : "Borrowed";
        public bool IsAvailable => AvailableCopies > 0;

        public List<string> Validate()
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(Title)) errors.Add("Book title is required.");
            if (string.IsNullOrWhiteSpace(Author)) errors.Add("Author name is required.");
            if (string.IsNullOrWhiteSpace(ISBN)) errors.Add("ISBN is required.");
            if (string.IsNullOrWhiteSpace(Category)) errors.Add("Category is required.");
            if (string.IsNullOrWhiteSpace(Publisher)) errors.Add("Publisher is required.");
            if (string.IsNullOrWhiteSpace(ShelfNo)) errors.Add("Shelf number is required.");
            if (Quantity < 0) errors.Add("Quantity cannot be negative.");
            if (AvailableCopies < 0) errors.Add("Available copies cannot be negative.");
            if (AvailableCopies > Quantity) errors.Add("Available copies cannot exceed total quantity.");
            if (PublishedYear < 1000 || PublishedYear > DateTime.Now.Year + 1) errors.Add("Published year is not valid.");
            return errors;
        }
    }

    public class Member
    {
        public int MemberID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        public List<string> Validate()
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(Name)) errors.Add("Member name is required.");
            if (string.IsNullOrWhiteSpace(Email)) errors.Add("Email address is required.");
            if (string.IsNullOrWhiteSpace(Phone)) errors.Add("Phone number is required.");
            if (string.IsNullOrWhiteSpace(Address)) errors.Add("Address is required.");
            if (!string.IsNullOrWhiteSpace(Email) && !System.Text.RegularExpressions.Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                errors.Add("Email format is invalid.");
            if (!string.IsNullOrWhiteSpace(Phone) && System.Text.RegularExpressions.Regex.Replace(Phone, @"[^\d]", "").Length != 10)
                errors.Add("Phone number must be exactly 10 digits.");
            return errors;
        }
    }

    public class IssuedBook
    {
        public int IssueID { get; set; }
        public int BookID { get; set; }
        public int MemberID { get; set; }
        public DateTime IssueDate { get; set; } = DateTime.Now;
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public decimal Fine { get; set; } = 0;

        public string BookTitle { get; set; } = string.Empty;
        public string MemberName { get; set; } = string.Empty;

        public bool IsOverdue => ReturnDate == null && DateTime.Now.Date > DueDate.Date;
        public bool IsReturned => ReturnDate != null;
        public int LateDays
        {
            get
            {
                DateTime checkDate = ReturnDate ?? DateTime.Now;
                int days = (checkDate.Date - DueDate.Date).Days;
                return days > 0 ? days : 0;
            }
        }
        public decimal CalculatedFine => LateDays * 10m;
        public string Status => IsReturned ? "Returned" : (IsOverdue ? "Overdue" : "Issued");
    }

    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = "Admin";
    }

    public class Magazine
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; } = DateTime.Now;
        public string Language { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Available";
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsAvailable => Status == "Available";

        public List<string> Validate()
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(Title)) errors.Add("Magazine title is required.");
            if (string.IsNullOrWhiteSpace(Publisher)) errors.Add("Publisher is required.");
            if (string.IsNullOrWhiteSpace(Language)) errors.Add("Language is required.");
            if (string.IsNullOrWhiteSpace(Category)) errors.Add("Category is required.");
            if (IssueDate > DateTime.Now) errors.Add("Issue date cannot be in the future.");
            if (!new[] { "Available", "Issued" }.Contains(Status)) errors.Add("Status must be 'Available' or 'Issued'.");
            return errors;
        }
    }

    public class Newspaper
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public DateTime PublishedDate { get; set; } = DateTime.Now;
        public string Language { get; set; } = string.Empty;
        public string Edition { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Available";
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsAvailable => Status == "Available";

        public List<string> Validate()
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(Title)) errors.Add("Newspaper title is required.");
            if (string.IsNullOrWhiteSpace(Publisher)) errors.Add("Publisher is required.");
            if (string.IsNullOrWhiteSpace(Language)) errors.Add("Language is required.");
            if (string.IsNullOrWhiteSpace(Edition)) errors.Add("Edition is required.");
            if (PublishedDate > DateTime.Now) errors.Add("Published date cannot be in the future.");
            if (!new[] { "Available", "Issued" }.Contains(Status)) errors.Add("Status must be 'Available' or 'Issued'.");
            return errors;
        }
    }
}
