// ============================================================
// Member.cs — Member Data Model
// Library Management System
// ============================================================
// Represents a library member (student/reader) with
// contact details and validation logic.
// ============================================================

using System.Text.RegularExpressions;

namespace LibraryManagementSystem.Models
{
    /// <summary>
    /// Represents a library member entity.
    /// Contains member details and validation logic for email and phone.
    /// </summary>
    public class Member
    {
        // ---- Primary Key ----
        /// <summary>Auto-generated unique identifier for the member.</summary>
        public int MemberID { get; set; }

        // ---- Member Details ----
        /// <summary>Full name of the member.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Email address of the member.</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>Phone number of the member.</summary>
        public string Phone { get; set; } = string.Empty;

        /// <summary>Residential address of the member.</summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>Date when the member registered with the library.</summary>
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        // ---- Validation ----

        /// <summary>
        /// Validates all member fields and returns a list of error messages.
        /// Returns an empty list if all fields are valid.
        /// </summary>
        public List<string> Validate()
        {
            var errors = new List<string>();

            // Check for empty required fields
            if (string.IsNullOrWhiteSpace(Name))
                errors.Add("Member name is required.");

            if (string.IsNullOrWhiteSpace(Email))
                errors.Add("Email address is required.");

            if (string.IsNullOrWhiteSpace(Phone))
                errors.Add("Phone number is required.");

            if (string.IsNullOrWhiteSpace(Address))
                errors.Add("Address is required.");

            // Validate email format using regex
            if (!string.IsNullOrWhiteSpace(Email))
            {
                string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                if (!Regex.IsMatch(Email, emailPattern))
                    errors.Add("Email format is invalid. Example: user@example.com");
            }

            // Validate phone number (10 digits only)
            if (!string.IsNullOrWhiteSpace(Phone))
            {
                string phoneDigits = Regex.Replace(Phone, @"[^0-9]", "");
                if (phoneDigits.Length != 10)
                    errors.Add("Phone number must be exactly 10 digits.");
            }

            return errors;
        }
    }
}
