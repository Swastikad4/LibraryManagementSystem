using System.Data.SQLite;
using LibraryManagementSystem.Web.Models;
using LibraryManagementSystem.Web.DataAccess;

namespace LibraryManagementSystem.Web.Services
{
    public class BookService
    {
        public List<Book> GetAllBooks()
        {
            var books = new List<Book>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT * FROM Books ORDER BY Title", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read()) books.Add(MapBook(r));
            return books;
        }

        public Book? GetBookById(int id)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT * FROM Books WHERE BookID = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();
            return r.Read() ? MapBook(r) : null;
        }

        public List<Book> SearchBooks(string term)
        {
            var books = new List<Book>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand(@"
                SELECT * FROM Books 
                WHERE Title LIKE @t OR Author LIKE @t OR ISBN LIKE @t OR Category LIKE @t
                ORDER BY Title", conn);
            cmd.Parameters.AddWithValue("@t", $"%{term}%");
            using var r = cmd.ExecuteReader();
            while (r.Read()) books.Add(MapBook(r));
            return books;
        }

        public List<Book> GetBooksByCategory(string cat)
        {
            var books = new List<Book>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT * FROM Books WHERE Category = @c ORDER BY Title", conn);
            cmd.Parameters.AddWithValue("@c", cat);
            using var r = cmd.ExecuteReader();
            while (r.Read()) books.Add(MapBook(r));
            return books;
        }

        public List<string> GetCategories()
        {
            var cats = new List<string>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT DISTINCT Category FROM Books ORDER BY Category", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read()) cats.Add(r["Category"].ToString()!);
            return cats;
        }

        public int GetTotalCount()
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT COALESCE(SUM(Quantity), 0) FROM Books", conn);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public int GetAvailableCount()
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT COALESCE(SUM(AvailableCopies), 0) FROM Books", conn);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public (bool Success, string Message) AddBook(Book book)
        {
            var errors = book.Validate();
            if (errors.Count > 0) return (false, string.Join(" ", errors));

            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            // Check ISBN
            using (var check = new SQLiteCommand("SELECT COUNT(*) FROM Books WHERE ISBN = @isbn", conn))
            {
                check.Parameters.AddWithValue("@isbn", book.ISBN);
                if (Convert.ToInt32(check.ExecuteScalar()) > 0) return (false, "ISBN already exists.");
            }

            book.AvailableCopies = book.Quantity;
            using var cmd = new SQLiteCommand(@"
                INSERT INTO Books (Title, Author, ISBN, Category, Publisher, Quantity, AvailableCopies, ShelfNo, PublishedYear)
                VALUES (@Title, @Author, @ISBN, @Category, @Publisher, @Quantity, @AvailableCopies, @ShelfNo, @PublishedYear)", conn);
            AddParams(cmd, book);
            cmd.ExecuteNonQuery();
            return (true, "Book added successfully!");
        }

        public (bool Success, string Message) UpdateBook(Book book)
        {
            var errors = book.Validate();
            if (errors.Count > 0) return (false, string.Join(" ", errors));

            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            using (var check = new SQLiteCommand("SELECT COUNT(*) FROM Books WHERE ISBN = @isbn AND BookID != @id", conn))
            {
                check.Parameters.AddWithValue("@isbn", book.ISBN);
                check.Parameters.AddWithValue("@id", book.BookID);
                if (Convert.ToInt32(check.ExecuteScalar()) > 0) return (false, "ISBN used by another book.");
            }

            using var cmd = new SQLiteCommand(@"
                UPDATE Books SET Title=@Title, Author=@Author, ISBN=@ISBN, Category=@Category, Publisher=@Publisher,
                Quantity=@Quantity, AvailableCopies=@AvailableCopies, ShelfNo=@ShelfNo, PublishedYear=@PublishedYear
                WHERE BookID=@BookID", conn);
            AddParams(cmd, book);
            cmd.Parameters.AddWithValue("@BookID", book.BookID);
            cmd.ExecuteNonQuery();
            return (true, "Book updated successfully!");
        }

        public (bool Success, string Message) DeleteBook(int id)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand("DELETE FROM Books WHERE BookID = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
            return (true, "Book deleted successfully.");
        }

        private Book MapBook(SQLiteDataReader r) => new Book
        {
            BookID = Convert.ToInt32(r["BookID"]),
            Title = r["Title"].ToString()!,
            Author = r["Author"].ToString()!,
            ISBN = r["ISBN"].ToString()!,
            Category = r["Category"].ToString()!,
            Publisher = r["Publisher"].ToString()!,
            Quantity = Convert.ToInt32(r["Quantity"]),
            AvailableCopies = Convert.ToInt32(r["AvailableCopies"]),
            ShelfNo = r["ShelfNo"].ToString()!,
            PublishedYear = Convert.ToInt32(r["PublishedYear"])
        };

        private void AddParams(SQLiteCommand cmd, Book b)
        {
            cmd.Parameters.AddWithValue("@Title", b.Title);
            cmd.Parameters.AddWithValue("@Author", b.Author);
            cmd.Parameters.AddWithValue("@ISBN", b.ISBN);
            cmd.Parameters.AddWithValue("@Category", b.Category);
            cmd.Parameters.AddWithValue("@Publisher", b.Publisher);
            cmd.Parameters.AddWithValue("@Quantity", b.Quantity);
            cmd.Parameters.AddWithValue("@AvailableCopies", b.AvailableCopies);
            cmd.Parameters.AddWithValue("@ShelfNo", b.ShelfNo);
            cmd.Parameters.AddWithValue("@PublishedYear", b.PublishedYear);
        }
    }

    public class MemberService
    {
        public List<Member> GetAllMembers()
        {
            var members = new List<Member>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT * FROM Members ORDER BY Name", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read()) members.Add(MapMember(r));
            return members;
        }

        public Member? GetMemberById(int id)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT * FROM Members WHERE MemberID = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();
            return r.Read() ? MapMember(r) : null;
        }

        public List<Member> SearchMembers(string term)
        {
            var members = new List<Member>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT * FROM Members WHERE Name LIKE @t OR Email LIKE @t OR Phone LIKE @t ORDER BY Name", conn);
            cmd.Parameters.AddWithValue("@t", $"%{term}%");
            using var r = cmd.ExecuteReader();
            while (r.Read()) members.Add(MapMember(r));
            return members;
        }

        public int GetTotalCount()
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Members", conn);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public (bool Success, string Message) AddMember(Member m)
        {
            var errors = m.Validate();
            if (errors.Count > 0) return (false, string.Join(" ", errors));

            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand(@"
                INSERT INTO Members (Name, Email, Phone, Address, RegistrationDate)
                VALUES (@Name, @Email, @Phone, @Address, @RegDate)", conn);
            cmd.Parameters.AddWithValue("@Name", m.Name);
            cmd.Parameters.AddWithValue("@Email", m.Email);
            cmd.Parameters.AddWithValue("@Phone", m.Phone);
            cmd.Parameters.AddWithValue("@Address", m.Address);
            cmd.Parameters.AddWithValue("@RegDate", DateTime.Now.ToString("yyyy-MM-dd"));
            cmd.ExecuteNonQuery();
            return (true, "Member registered successfully!");
        }

        public (bool Success, string Message) UpdateMember(Member m)
        {
            var errors = m.Validate();
            if (errors.Count > 0) return (false, string.Join(" ", errors));

            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand(@"
                UPDATE Members SET Name=@Name, Email=@Email, Phone=@Phone, Address=@Address WHERE MemberID=@MemberID", conn);
            cmd.Parameters.AddWithValue("@Name", m.Name);
            cmd.Parameters.AddWithValue("@Email", m.Email);
            cmd.Parameters.AddWithValue("@Phone", m.Phone);
            cmd.Parameters.AddWithValue("@Address", m.Address);
            cmd.Parameters.AddWithValue("@MemberID", m.MemberID);
            cmd.ExecuteNonQuery();
            return (true, "Member updated successfully!");
        }

        public (bool Success, string Message) DeleteMember(int id)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            using (var check = new SQLiteCommand("SELECT COUNT(*) FROM IssuedBooks WHERE MemberID = @id AND ReturnDate IS NULL", conn))
            {
                check.Parameters.AddWithValue("@id", id);
                if (Convert.ToInt32(check.ExecuteScalar()) > 0) return (false, "Cannot delete member with active borrowed books.");
            }

            using var cmd = new SQLiteCommand("DELETE FROM Members WHERE MemberID = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
            return (true, "Member deleted successfully.");
        }

        private Member MapMember(SQLiteDataReader r) => new Member
        {
            MemberID = Convert.ToInt32(r["MemberID"]),
            Name = r["Name"].ToString()!,
            Email = r["Email"].ToString()!,
            Phone = r["Phone"].ToString()!,
            Address = r["Address"].ToString()!,
            RegistrationDate = DateTime.Parse(r["RegistrationDate"].ToString()!)
        };
    }

    public class IssueReturnService
    {
        public List<IssuedBook> GetAllIssued()
        {
            var list = new List<IssuedBook>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand(@"
                SELECT i.*, b.Title AS BookTitle, m.Name AS MemberName
                FROM IssuedBooks i
                INNER JOIN Books b ON i.BookID = b.BookID
                INNER JOIN Members m ON i.MemberID = m.MemberID
                ORDER BY i.IssueDate DESC", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(MapIssued(r));
            return list;
        }

        public List<IssuedBook> GetCurrentlyIssued()
        {
            var list = new List<IssuedBook>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand(@"
                SELECT i.*, b.Title AS BookTitle, m.Name AS MemberName
                FROM IssuedBooks i
                INNER JOIN Books b ON i.BookID = b.BookID
                INNER JOIN Members m ON i.MemberID = m.MemberID
                WHERE i.ReturnDate IS NULL ORDER BY i.DueDate ASC", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(MapIssued(r));
            return list;
        }

        public List<IssuedBook> GetOverdueBooks()
        {
            var list = new List<IssuedBook>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand(@"
                SELECT i.*, b.Title AS BookTitle, m.Name AS MemberName
                FROM IssuedBooks i
                INNER JOIN Books b ON i.BookID = b.BookID
                INNER JOIN Members m ON i.MemberID = m.MemberID
                WHERE i.ReturnDate IS NULL AND date(i.DueDate) < date('now')
                ORDER BY i.DueDate ASC", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(MapIssued(r));
            return list;
        }

        public int GetBorrowedCount()
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT COUNT(*) FROM IssuedBooks WHERE ReturnDate IS NULL", conn);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public int GetOverdueCount()
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT COUNT(*) FROM IssuedBooks WHERE ReturnDate IS NULL AND date(DueDate) < date('now')", conn);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public int GetDueTodayCount()
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT COUNT(*) FROM IssuedBooks WHERE ReturnDate IS NULL AND date(DueDate) = date('now')", conn);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public (bool Success, string Message) IssueBook(int bookId, int memberId)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            // Availability check
            using (var bCmd = new SQLiteCommand("SELECT AvailableCopies FROM Books WHERE BookID=@id", conn))
            {
                bCmd.Parameters.AddWithValue("@id", bookId);
                var avail = bCmd.ExecuteScalar();
                if (avail == null || Convert.ToInt32(avail) <= 0) return (false, "Book is unavailable.");
            }

            // Member borrow limit check (max 3)
            using (var mCmd = new SQLiteCommand("SELECT COUNT(*) FROM IssuedBooks WHERE MemberID=@id AND ReturnDate IS NULL", conn))
            {
                mCmd.Parameters.AddWithValue("@id", memberId);
                if (Convert.ToInt32(mCmd.ExecuteScalar()) >= 3) return (false, "Member already has 3 books (max limit).");
            }

            DateTime issueDate = DateTime.Now;
            DateTime dueDate = issueDate.AddDays(14);

            using var cmd = new SQLiteCommand(@"
                INSERT INTO IssuedBooks (BookID, MemberID, IssueDate, DueDate)
                VALUES (@BookID, @MemberID, @IssueDate, @DueDate)", conn);
            cmd.Parameters.AddWithValue("@BookID", bookId);
            cmd.Parameters.AddWithValue("@MemberID", memberId);
            cmd.Parameters.AddWithValue("@IssueDate", issueDate.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@DueDate", dueDate.ToString("yyyy-MM-dd"));
            cmd.ExecuteNonQuery();

            // Decrement copies
            using var decCmd = new SQLiteCommand("UPDATE Books SET AvailableCopies = AvailableCopies - 1 WHERE BookID=@id", conn);
            decCmd.Parameters.AddWithValue("@id", bookId);
            decCmd.ExecuteNonQuery();

            return (true, $"Book issued successfully! Due date: {dueDate:dd-MM-yyyy}");
        }

        public (bool Success, string Message, decimal Fine) ReturnBook(int issueId)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            int bookId = 0;
            DateTime dueDate = DateTime.Now;
            bool alreadyReturned = false;

            using (var getCmd = new SQLiteCommand("SELECT BookID, DueDate, ReturnDate FROM IssuedBooks WHERE IssueID=@id", conn))
            {
                getCmd.Parameters.AddWithValue("@id", issueId);
                using var r = getCmd.ExecuteReader();
                if (r.Read())
                {
                    bookId = Convert.ToInt32(r["BookID"]);
                    dueDate = DateTime.Parse(r["DueDate"].ToString()!);
                    alreadyReturned = r["ReturnDate"] != DBNull.Value;
                }
                else
                {
                    return (false, "Issue record not found.", 0);
                }
            }

            if (alreadyReturned) return (false, "Book is already returned.", 0);

            DateTime returnDate = DateTime.Now;
            int lateDays = (returnDate.Date - dueDate.Date).Days;
            if (lateDays < 0) lateDays = 0;
            decimal fine = lateDays * 10m;

            using var updCmd = new SQLiteCommand(@"
                UPDATE IssuedBooks SET ReturnDate=@ReturnDate, Fine=@Fine WHERE IssueID=@IssueID", conn);
            updCmd.Parameters.AddWithValue("@ReturnDate", returnDate.ToString("yyyy-MM-dd"));
            updCmd.Parameters.AddWithValue("@Fine", fine);
            updCmd.Parameters.AddWithValue("@IssueID", issueId);
            updCmd.ExecuteNonQuery();

            using var incCmd = new SQLiteCommand("UPDATE Books SET AvailableCopies = AvailableCopies + 1 WHERE BookID=@id", conn);
            incCmd.Parameters.AddWithValue("@id", bookId);
            incCmd.ExecuteNonQuery();

            string msg = lateDays > 0 ? $"Book returned! Late by {lateDays} day(s). Fine: ₹{fine:N2}" : "Book returned on time!";
            return (true, msg, fine);
        }

        private IssuedBook MapIssued(SQLiteDataReader r) => new IssuedBook
        {
            IssueID = Convert.ToInt32(r["IssueID"]),
            BookID = Convert.ToInt32(r["BookID"]),
            MemberID = Convert.ToInt32(r["MemberID"]),
            IssueDate = DateTime.Parse(r["IssueDate"].ToString()!),
            DueDate = DateTime.Parse(r["DueDate"].ToString()!),
            ReturnDate = r["ReturnDate"] != DBNull.Value ? DateTime.Parse(r["ReturnDate"].ToString()!) : null,
            Fine = r["Fine"] != DBNull.Value ? Convert.ToDecimal(r["Fine"]) : 0,
            BookTitle = r["BookTitle"].ToString()!,
            MemberName = r["MemberName"].ToString()!
        };
    }

    public class AuthService
    {
        public User? Authenticate(string username, string password)
        {
            string hash = DatabaseHelper.HashPassword(password);
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT * FROM Users WHERE Username=@u AND PasswordHash=@p", conn);
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@p", hash);
            using var r = cmd.ExecuteReader();
            if (r.Read())
            {
                return new User
                {
                    UserID = Convert.ToInt32(r["UserID"]),
                    Username = r["Username"].ToString()!,
                    PasswordHash = r["PasswordHash"].ToString()!,
                    FullName = r["FullName"].ToString()!,
                    Role = r["Role"].ToString()!
                };
            }
            return null;
        }

        public (bool Success, string Message) ChangePassword(int userId, string currPass, string newPass)
        {
            string currHash = DatabaseHelper.HashPassword(currPass);
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            using (var check = new SQLiteCommand("SELECT COUNT(*) FROM Users WHERE UserID=@id AND PasswordHash=@p", conn))
            {
                check.Parameters.AddWithValue("@id", userId);
                check.Parameters.AddWithValue("@p", currHash);
                if (Convert.ToInt32(check.ExecuteScalar()) == 0) return (false, "Current password is incorrect.");
            }

            string newHash = DatabaseHelper.HashPassword(newPass);
            using var cmd = new SQLiteCommand("UPDATE Users SET PasswordHash=@p WHERE UserID=@id", conn);
            cmd.Parameters.AddWithValue("@p", newHash);
            cmd.Parameters.AddWithValue("@id", userId);
            cmd.ExecuteNonQuery();
            return (true, "Password changed successfully.");
        }
    }

    public class MagazineService
    {
        public List<Magazine> GetAllMagazines()
        {
            var magazines = new List<Magazine>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT * FROM Magazines ORDER BY IssueDate DESC", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read()) magazines.Add(MapMagazine(r));
            return magazines;
        }

        public Magazine? GetMagazineById(int id)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT * FROM Magazines WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();
            return r.Read() ? MapMagazine(r) : null;
        }

        public List<Magazine> SearchMagazines(string term)
        {
            var magazines = new List<Magazine>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand(@"
                SELECT * FROM Magazines
                WHERE Title LIKE @t OR Publisher LIKE @t OR Category LIKE @t OR Language LIKE @t
                ORDER BY IssueDate DESC", conn);
            cmd.Parameters.AddWithValue("@t", $"%{term}%");
            using var r = cmd.ExecuteReader();
            while (r.Read()) magazines.Add(MapMagazine(r));
            return magazines;
        }

        public List<Magazine> GetMagazinesByPublisher(string publisher)
        {
            var magazines = new List<Magazine>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT * FROM Magazines WHERE Publisher = @p ORDER BY IssueDate DESC", conn);
            cmd.Parameters.AddWithValue("@p", publisher);
            using var r = cmd.ExecuteReader();
            while (r.Read()) magazines.Add(MapMagazine(r));
            return magazines;
        }

        public List<Magazine> GetMagazinesByStatus(string status)
        {
            var magazines = new List<Magazine>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT * FROM Magazines WHERE Status = @s ORDER BY IssueDate DESC", conn);
            cmd.Parameters.AddWithValue("@s", status);
            using var r = cmd.ExecuteReader();
            while (r.Read()) magazines.Add(MapMagazine(r));
            return magazines;
        }

        public List<string> GetPublishers()
        {
            var publishers = new List<string>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT DISTINCT Publisher FROM Magazines ORDER BY Publisher", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read()) publishers.Add(r["Publisher"].ToString()!);
            return publishers;
        }

        public List<string> GetCategories()
        {
            var categories = new List<string>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT DISTINCT Category FROM Magazines ORDER BY Category", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read()) categories.Add(r["Category"].ToString()!);
            return categories;
        }

        public int GetTotalCount()
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Magazines", conn);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public int GetAvailableCount()
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Magazines WHERE Status = 'Available'", conn);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public (bool Success, string Message) AddMagazine(Magazine magazine)
        {
            var errors = magazine.Validate();
            if (errors.Count > 0) return (false, string.Join(" ", errors));

            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand(@"
                INSERT INTO Magazines (Title, Publisher, IssueDate, Language, Category, Description, Status, CreatedAt)
                VALUES (@Title, @Publisher, @IssueDate, @Language, @Category, @Description, @Status, @CreatedAt)", conn);
            AddParams(cmd, magazine);
            cmd.ExecuteNonQuery();
            return (true, "Magazine added successfully!");
        }

        public (bool Success, string Message) UpdateMagazine(Magazine magazine)
        {
            var errors = magazine.Validate();
            if (errors.Count > 0) return (false, string.Join(" ", errors));

            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand(@"
                UPDATE Magazines SET Title=@Title, Publisher=@Publisher, IssueDate=@IssueDate, Language=@Language,
                Category=@Category, Description=@Description, Status=@Status WHERE Id=@Id", conn);
            AddParams(cmd, magazine);
            cmd.Parameters.AddWithValue("@Id", magazine.Id);
            cmd.ExecuteNonQuery();
            return (true, "Magazine updated successfully!");
        }

        public (bool Success, string Message) DeleteMagazine(int id)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand("DELETE FROM Magazines WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
            return (true, "Magazine deleted successfully.");
        }

        private Magazine MapMagazine(SQLiteDataReader r) => new Magazine
        {
            Id = Convert.ToInt32(r["Id"]),
            Title = r["Title"].ToString()!,
            Publisher = r["Publisher"].ToString()!,
            IssueDate = DateTime.Parse(r["IssueDate"].ToString()!),
            Language = r["Language"].ToString()!,
            Category = r["Category"].ToString()!,
            Description = r["Description"] != DBNull.Value ? r["Description"].ToString()! : string.Empty,
            Status = r["Status"].ToString()!,
            CreatedAt = DateTime.Parse(r["CreatedAt"].ToString()!)
        };

        private void AddParams(SQLiteCommand cmd, Magazine m)
        {
            cmd.Parameters.AddWithValue("@Title", m.Title);
            cmd.Parameters.AddWithValue("@Publisher", m.Publisher);
            cmd.Parameters.AddWithValue("@IssueDate", m.IssueDate.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@Language", m.Language);
            cmd.Parameters.AddWithValue("@Category", m.Category);
            cmd.Parameters.AddWithValue("@Description", m.Description);
            cmd.Parameters.AddWithValue("@Status", m.Status);
            cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now.ToString("yyyy-MM-dd"));
        }
    }

    public class NewspaperService
    {
        public List<Newspaper> GetAllNewspapers()
        {
            var newspapers = new List<Newspaper>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT * FROM Newspapers ORDER BY PublishedDate DESC", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read()) newspapers.Add(MapNewspaper(r));
            return newspapers;
        }

        public Newspaper? GetNewspaperById(int id)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT * FROM Newspapers WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();
            return r.Read() ? MapNewspaper(r) : null;
        }

        public List<Newspaper> SearchNewspapers(string term)
        {
            var newspapers = new List<Newspaper>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand(@"
                SELECT * FROM Newspapers
                WHERE Title LIKE @t OR Publisher LIKE @t OR Edition LIKE @t OR Language LIKE @t
                ORDER BY PublishedDate DESC", conn);
            cmd.Parameters.AddWithValue("@t", $"%{term}%");
            using var r = cmd.ExecuteReader();
            while (r.Read()) newspapers.Add(MapNewspaper(r));
            return newspapers;
        }

        public List<Newspaper> GetNewspapersByPublisher(string publisher)
        {
            var newspapers = new List<Newspaper>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT * FROM Newspapers WHERE Publisher = @p ORDER BY PublishedDate DESC", conn);
            cmd.Parameters.AddWithValue("@p", publisher);
            using var r = cmd.ExecuteReader();
            while (r.Read()) newspapers.Add(MapNewspaper(r));
            return newspapers;
        }

        public List<Newspaper> GetNewspapersByStatus(string status)
        {
            var newspapers = new List<Newspaper>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT * FROM Newspapers WHERE Status = @s ORDER BY PublishedDate DESC", conn);
            cmd.Parameters.AddWithValue("@s", status);
            using var r = cmd.ExecuteReader();
            while (r.Read()) newspapers.Add(MapNewspaper(r));
            return newspapers;
        }

        public List<string> GetPublishers()
        {
            var publishers = new List<string>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT DISTINCT Publisher FROM Newspapers ORDER BY Publisher", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read()) publishers.Add(r["Publisher"].ToString()!);
            return publishers;
        }

        public List<string> GetEditions()
        {
            var editions = new List<string>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT DISTINCT Edition FROM Newspapers ORDER BY Edition", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read()) editions.Add(r["Edition"].ToString()!);
            return editions;
        }

        public int GetTotalCount()
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Newspapers", conn);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public int GetAvailableCount()
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Newspapers WHERE Status = 'Available'", conn);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public (bool Success, string Message) AddNewspaper(Newspaper newspaper)
        {
            var errors = newspaper.Validate();
            if (errors.Count > 0) return (false, string.Join(" ", errors));

            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand(@"
                INSERT INTO Newspapers (Title, Publisher, PublishedDate, Language, Edition, Description, Status, CreatedAt)
                VALUES (@Title, @Publisher, @PublishedDate, @Language, @Edition, @Description, @Status, @CreatedAt)", conn);
            AddParams(cmd, newspaper);
            cmd.ExecuteNonQuery();
            return (true, "Newspaper added successfully!");
        }

        public (bool Success, string Message) UpdateNewspaper(Newspaper newspaper)
        {
            var errors = newspaper.Validate();
            if (errors.Count > 0) return (false, string.Join(" ", errors));

            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand(@"
                UPDATE Newspapers SET Title=@Title, Publisher=@Publisher, PublishedDate=@PublishedDate, Language=@Language,
                Edition=@Edition, Description=@Description, Status=@Status WHERE Id=@Id", conn);
            AddParams(cmd, newspaper);
            cmd.Parameters.AddWithValue("@Id", newspaper.Id);
            cmd.ExecuteNonQuery();
            return (true, "Newspaper updated successfully!");
        }

        public (bool Success, string Message) DeleteNewspaper(int id)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new SQLiteCommand("DELETE FROM Newspapers WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
            return (true, "Newspaper deleted successfully.");
        }

        private Newspaper MapNewspaper(SQLiteDataReader r) => new Newspaper
        {
            Id = Convert.ToInt32(r["Id"]),
            Title = r["Title"].ToString()!,
            Publisher = r["Publisher"].ToString()!,
            PublishedDate = DateTime.Parse(r["PublishedDate"].ToString()!),
            Language = r["Language"].ToString()!,
            Edition = r["Edition"].ToString()!,
            Description = r["Description"] != DBNull.Value ? r["Description"].ToString()! : string.Empty,
            Status = r["Status"].ToString()!,
            CreatedAt = DateTime.Parse(r["CreatedAt"].ToString()!)
        };

        private void AddParams(SQLiteCommand cmd, Newspaper n)
        {
            cmd.Parameters.AddWithValue("@Title", n.Title);
            cmd.Parameters.AddWithValue("@Publisher", n.Publisher);
            cmd.Parameters.AddWithValue("@PublishedDate", n.PublishedDate.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@Language", n.Language);
            cmd.Parameters.AddWithValue("@Edition", n.Edition);
            cmd.Parameters.AddWithValue("@Description", n.Description);
            cmd.Parameters.AddWithValue("@Status", n.Status);
            cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now.ToString("yyyy-MM-dd"));
        }
    }
}
