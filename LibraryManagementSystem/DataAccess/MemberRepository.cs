// ============================================================
// MemberRepository.cs — Member Data Access
// Library Management System — Data Access Layer
// ============================================================
// Handles all database operations for the Members table.
// ============================================================

using System.Data.SQLite;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.DataAccess
{
    /// <summary>
    /// Repository class for Member CRUD operations.
    /// </summary>
    public class MemberRepository
    {
        // ---- READ Operations ----

        /// <summary>Retrieves all members from the database.</summary>
        public List<Member> GetAll()
        {
            var members = new List<Member>();

            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(
                "SELECT * FROM Members ORDER BY Name", connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                members.Add(MapReaderToMember(reader));
            }

            return members;
        }

        /// <summary>Retrieves a single member by ID.</summary>
        public Member? GetById(int memberId)
        {
            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(
                "SELECT * FROM Members WHERE MemberID = @MemberID", connection);
            command.Parameters.AddWithValue("@MemberID", memberId);

            using var reader = command.ExecuteReader();
            return reader.Read() ? MapReaderToMember(reader) : null;
        }

        /// <summary>Searches members by name, email, or phone.</summary>
        public List<Member> Search(string searchTerm)
        {
            var members = new List<Member>();
            string term = $"%{searchTerm}%";

            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(@"
                SELECT * FROM Members 
                WHERE Name LIKE @Term 
                   OR Email LIKE @Term 
                   OR Phone LIKE @Term
                   OR CAST(MemberID AS TEXT) LIKE @Term
                ORDER BY Name", connection);
            command.Parameters.AddWithValue("@Term", term);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                members.Add(MapReaderToMember(reader));
            }

            return members;
        }

        /// <summary>Gets the total count of members.</summary>
        public int GetTotalCount()
        {
            using var connection = DatabaseHelper.GetConnection();
            connection.Open();
            using var command = new SQLiteCommand(
                "SELECT COUNT(*) FROM Members", connection);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        /// <summary>
        /// Gets the number of books currently borrowed by a member.
        /// Used to enforce the maximum borrow limit (3 books).
        /// </summary>
        public int GetBorrowCount(int memberId)
        {
            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(@"
                SELECT COUNT(*) FROM IssuedBooks 
                WHERE MemberID = @MemberID AND ReturnDate IS NULL",
                connection);
            command.Parameters.AddWithValue("@MemberID", memberId);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        // ---- CREATE Operation ----

        /// <summary>Adds a new member to the database.</summary>
        public int Add(Member member)
        {
            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(@"
                INSERT INTO Members (Name, Email, Phone, Address, RegistrationDate)
                VALUES (@Name, @Email, @Phone, @Address, @RegDate);
                SELECT last_insert_rowid();",
                connection);

            command.Parameters.AddWithValue("@Name", member.Name);
            command.Parameters.AddWithValue("@Email", member.Email);
            command.Parameters.AddWithValue("@Phone", member.Phone);
            command.Parameters.AddWithValue("@Address", member.Address);
            command.Parameters.AddWithValue("@RegDate",
                member.RegistrationDate.ToString("yyyy-MM-dd"));

            return Convert.ToInt32(command.ExecuteScalar());
        }

        // ---- UPDATE Operation ----

        /// <summary>Updates an existing member in the database.</summary>
        public void Update(Member member)
        {
            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(@"
                UPDATE Members SET
                    Name = @Name,
                    Email = @Email,
                    Phone = @Phone,
                    Address = @Address
                WHERE MemberID = @MemberID",
                connection);

            command.Parameters.AddWithValue("@Name", member.Name);
            command.Parameters.AddWithValue("@Email", member.Email);
            command.Parameters.AddWithValue("@Phone", member.Phone);
            command.Parameters.AddWithValue("@Address", member.Address);
            command.Parameters.AddWithValue("@MemberID", member.MemberID);
            command.ExecuteNonQuery();
        }

        // ---- DELETE Operation ----

        /// <summary>Deletes a member from the database.</summary>
        public void Delete(int memberId)
        {
            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(
                "DELETE FROM Members WHERE MemberID = @MemberID", connection);
            command.Parameters.AddWithValue("@MemberID", memberId);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Checks if a member has any active (unreturned) issued books.
        /// Used to prevent deletion of members with active borrows.
        /// </summary>
        public bool HasActiveIssues(int memberId)
        {
            using var connection = DatabaseHelper.GetConnection();
            connection.Open();

            using var command = new SQLiteCommand(@"
                SELECT COUNT(*) FROM IssuedBooks 
                WHERE MemberID = @MemberID AND ReturnDate IS NULL",
                connection);
            command.Parameters.AddWithValue("@MemberID", memberId);
            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }

        // ---- Helper Methods ----

        /// <summary>Maps a data reader row to a Member object.</summary>
        private Member MapReaderToMember(SQLiteDataReader reader)
        {
            return new Member
            {
                MemberID = Convert.ToInt32(reader["MemberID"]),
                Name = reader["Name"].ToString()!,
                Email = reader["Email"].ToString()!,
                Phone = reader["Phone"].ToString()!,
                Address = reader["Address"].ToString()!,
                RegistrationDate = DateTime.Parse(reader["RegistrationDate"].ToString()!)
            };
        }
    }
}
