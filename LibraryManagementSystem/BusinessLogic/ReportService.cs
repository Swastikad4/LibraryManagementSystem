// ============================================================
// ReportService.cs — Report Business Logic
// Library Management System — Business Logic Layer
// ============================================================
// Generates report data and handles export to PDF and Excel.
// ============================================================

using LibraryManagementSystem.DataAccess;
using LibraryManagementSystem.Models;
using System.Data;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LibraryManagementSystem.BusinessLogic
{
    /// <summary>
    /// Service class for generating reports and exporting data.
    /// </summary>
    public class ReportService
    {
        private readonly ReportRepository _reportRepo = new();
        private readonly BookRepository _bookRepo = new();
        private readonly MemberRepository _memberRepo = new();
        private readonly IssuedBookRepository _issuedRepo = new();

        // ---- Chart Data ----

        /// <summary>Gets books grouped by category for charts.</summary>
        public Dictionary<string, int> GetBooksByCategory() =>
            _reportRepo.GetBooksByCategory();

        /// <summary>Gets monthly borrowing data for charts.</summary>
        public Dictionary<string, int> GetMonthlyBorrowing() =>
            _reportRepo.GetMonthlyBorrowing();

        /// <summary>Gets most borrowed books for charts.</summary>
        public Dictionary<string, int> GetMostBorrowedBooks() =>
            _reportRepo.GetMostBorrowedBooks();

        // ---- Report Data ----

        /// <summary>Gets available books for the report.</summary>
        public List<Book> GetAvailableBooksReport() =>
            _bookRepo.GetAvailableBooks();

        /// <summary>Gets all books for the report.</summary>
        public List<Book> GetAllBooksReport() =>
            _bookRepo.GetAll();

        /// <summary>Gets currently borrowed books for the report.</summary>
        public List<IssuedBook> GetBorrowedBooksReport() =>
            _issuedRepo.GetCurrentlyIssued();

        /// <summary>Gets overdue books for the report.</summary>
        public List<IssuedBook> GetOverdueBooksReport() =>
            _issuedRepo.GetOverdueBooks();

        /// <summary>Gets all members for the report.</summary>
        public List<Member> GetMembersReport() =>
            _memberRepo.GetAll();

        // ---- Notification Data ----

        /// <summary>Gets books with low stock.</summary>
        public List<(string Title, int Available)> GetLowStockBooks() =>
            _reportRepo.GetLowStockBooks();

        /// <summary>Gets books with approaching due dates.</summary>
        public List<(string BookTitle, string MemberName, DateTime DueDate)>
            GetApproachingDueBooks() => _reportRepo.GetApproachingDueBooks();

        // ---- Export Methods ----

        /// <summary>
        /// Creates a DataTable from a list of books for use in exports and grids.
        /// </summary>
        public DataTable BooksToDataTable(List<Book> books)
        {
            var dt = new DataTable("Books");
            dt.Columns.Add("BookID", typeof(int));
            dt.Columns.Add("Title", typeof(string));
            dt.Columns.Add("Author", typeof(string));
            dt.Columns.Add("ISBN", typeof(string));
            dt.Columns.Add("Category", typeof(string));
            dt.Columns.Add("Publisher", typeof(string));
            dt.Columns.Add("Quantity", typeof(int));
            dt.Columns.Add("Available", typeof(int));
            dt.Columns.Add("Shelf", typeof(string));
            dt.Columns.Add("Year", typeof(int));

            foreach (var book in books)
            {
                dt.Rows.Add(book.BookID, book.Title, book.Author, book.ISBN,
                    book.Category, book.Publisher, book.Quantity,
                    book.AvailableCopies, book.ShelfNo, book.PublishedYear);
            }

            return dt;
        }

        /// <summary>
        /// Creates a DataTable from a list of issued books.
        /// </summary>
        public DataTable IssuedBooksToDataTable(List<IssuedBook> issues)
        {
            var dt = new DataTable("IssuedBooks");
            dt.Columns.Add("IssueID", typeof(int));
            dt.Columns.Add("Book", typeof(string));
            dt.Columns.Add("Member", typeof(string));
            dt.Columns.Add("Issue Date", typeof(string));
            dt.Columns.Add("Due Date", typeof(string));
            dt.Columns.Add("Return Date", typeof(string));
            dt.Columns.Add("Status", typeof(string));
            dt.Columns.Add("Fine (₹)", typeof(decimal));

            foreach (var issue in issues)
            {
                dt.Rows.Add(
                    issue.IssueID,
                    issue.BookTitle,
                    issue.MemberName,
                    issue.IssueDate.ToString("dd-MM-yyyy"),
                    issue.DueDate.ToString("dd-MM-yyyy"),
                    issue.ReturnDate?.ToString("dd-MM-yyyy") ?? "Not Returned",
                    issue.Status,
                    issue.Fine);
            }

            return dt;
        }

        /// <summary>
        /// Creates a DataTable from a list of members.
        /// </summary>
        public DataTable MembersToDataTable(List<Member> members)
        {
            var dt = new DataTable("Members");
            dt.Columns.Add("MemberID", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Email", typeof(string));
            dt.Columns.Add("Phone", typeof(string));
            dt.Columns.Add("Address", typeof(string));
            dt.Columns.Add("Registered", typeof(string));

            foreach (var member in members)
            {
                dt.Rows.Add(member.MemberID, member.Name, member.Email,
                    member.Phone, member.Address,
                    member.RegistrationDate.ToString("dd-MM-yyyy"));
            }

            return dt;
        }

        /// <summary>
        /// Exports a DataTable to an Excel file using ClosedXML.
        /// </summary>
        /// <param name="data">The data table to export.</param>
        /// <param name="filePath">Output file path.</param>
        /// <param name="sheetName">Name of the worksheet.</param>
        public (bool Success, string Message) ExportToExcel(
            DataTable data, string filePath, string sheetName = "Report")
        {
            try
            {
                using var workbook = new ClosedXML.Excel.XLWorkbook();
                var worksheet = workbook.Worksheets.Add(data, sheetName);

                // Style the header row
                var headerRow = worksheet.Row(1);
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Fill.BackgroundColor =
                    ClosedXML.Excel.XLColor.FromHtml("#8E6A6A");
                headerRow.Style.Font.FontColor =
                    ClosedXML.Excel.XLColor.White;

                // Auto-fit column widths
                worksheet.Columns().AdjustToContents();

                workbook.SaveAs(filePath);
                return (true, $"Exported to Excel successfully!\n{filePath}");
            }
            catch (Exception ex)
            {
                return (false, $"Export error: {ex.Message}");
            }
        }

        /// <summary>
        /// Exports a DataTable to a PDF file using QuestPDF.
        /// </summary>
        public (bool Success, string Message) ExportToPdf(
            DataTable data, string filePath, string title = "Library Report")
        {
            try
            {
                // Set the QuestPDF license to Community (free)
                QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

                QuestPDF.Fluent.Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(QuestPDF.Helpers.PageSizes.A4.Landscape());
                        page.Margin(30);
                        page.DefaultTextStyle(x => x.FontSize(9));

                        // Header
                        page.Header().Column(col =>
                        {
                            col.Item().Text(title)
                                .FontSize(18).Bold()
                                .FontColor(QuestPDF.Helpers.Colors.Grey.Darken3);
                            col.Item().Text($"Generated: {DateTime.Now:dd-MM-yyyy HH:mm}")
                                .FontSize(8)
                                .FontColor(QuestPDF.Helpers.Colors.Grey.Medium);
                            col.Item().PaddingBottom(10).LineHorizontal(1)
                                .LineColor(QuestPDF.Helpers.Colors.Grey.Lighten2);
                        });

                        // Table content
                        page.Content().Table(table =>
                        {
                            // Define columns
                            table.ColumnsDefinition(columns =>
                            {
                                for (int i = 0; i < data.Columns.Count; i++)
                                    columns.RelativeColumn();
                            });

                            // Header cells
                            foreach (DataColumn col in data.Columns)
                            {
                                table.Cell().Background("#8E6A6A")
                                    .Padding(5)
                                    .Text(col.ColumnName)
                                    .FontColor("#FFFFFF")
                                    .Bold();
                            }

                            // Data cells
                            foreach (DataRow row in data.Rows)
                            {
                                foreach (var item in row.ItemArray)
                                {
                                    table.Cell()
                                        .BorderBottom(1)
                                        .BorderColor("#EEEEEE")
                                        .Padding(5)
                                        .Text(item?.ToString() ?? "");
                                }
                            }
                        });

                        // Footer
                        page.Footer().AlignCenter()
                            .Text(x =>
                            {
                                x.Span("Library Management System — Page ");
                                x.CurrentPageNumber();
                                x.Span(" of ");
                                x.TotalPages();
                            });
                    });
                }).GeneratePdf(filePath);

                return (true, $"Exported to PDF successfully!\n{filePath}");
            }
            catch (Exception ex)
            {
                return (false, $"Export error: {ex.Message}");
            }
        }
    }
}
