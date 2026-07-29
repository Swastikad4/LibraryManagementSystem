using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Web.Services;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LibraryManagementSystem.Web.Controllers
{
    public class ReportsController : Controller
    {
        private readonly BookService _bookService;
        private readonly MemberService _memberService;
        private readonly IssueReturnService _issueReturnService;

        public ReportsController(BookService bookService, MemberService memberService, IssueReturnService issueReturnService)
        {
            _bookService = bookService;
            _memberService = memberService;
            _issueReturnService = issueReturnService;
        }

        public IActionResult Index(string type = "AvailableBooks")
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
                return RedirectToAction("Login", "Account");

            ViewData["Title"] = "Reports & Export";
            ViewData["ActivePage"] = "Reports";
            ViewBag.CurrentType = type;

            return View();
        }

        [HttpGet]
        public IActionResult ExportExcel(string type)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Report");

            if (type == "AvailableBooks")
            {
                var books = _bookService.GetAllBooks().Where(b => b.AvailableCopies > 0).ToList();
                worksheet.Cell(1, 1).Value = "Book ID";
                worksheet.Cell(1, 2).Value = "Title";
                worksheet.Cell(1, 3).Value = "Author";
                worksheet.Cell(1, 4).Value = "ISBN";
                worksheet.Cell(1, 5).Value = "Available Copies";

                for (int i = 0; i < books.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = books[i].BookID;
                    worksheet.Cell(i + 2, 2).Value = books[i].Title;
                    worksheet.Cell(i + 2, 3).Value = books[i].Author;
                    worksheet.Cell(i + 2, 4).Value = books[i].ISBN;
                    worksheet.Cell(i + 2, 5).Value = books[i].AvailableCopies;
                }
            }
            else if (type == "Members")
            {
                var members = _memberService.GetAllMembers();
                worksheet.Cell(1, 1).Value = "Member ID";
                worksheet.Cell(1, 2).Value = "Name";
                worksheet.Cell(1, 3).Value = "Email";
                worksheet.Cell(1, 4).Value = "Phone";

                for (int i = 0; i < members.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = members[i].MemberID;
                    worksheet.Cell(i + 2, 2).Value = members[i].Name;
                    worksheet.Cell(i + 2, 3).Value = members[i].Email;
                    worksheet.Cell(i + 2, 4).Value = members[i].Phone;
                }
            }
            else
            {
                var issues = _issueReturnService.GetCurrentlyIssued();
                worksheet.Cell(1, 1).Value = "Issue ID";
                worksheet.Cell(1, 2).Value = "Book Title";
                worksheet.Cell(1, 3).Value = "Member Name";
                worksheet.Cell(1, 4).Value = "Due Date";

                for (int i = 0; i < issues.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = issues[i].IssueID;
                    worksheet.Cell(i + 2, 2).Value = issues[i].BookTitle;
                    worksheet.Cell(i + 2, 3).Value = issues[i].MemberName;
                    worksheet.Cell(i + 2, 4).Value = issues[i].DueDate.ToString("dd-MM-yyyy");
                }
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{type}_Report.xlsx");
        }

        [HttpGet]
        public IActionResult ExportPdf(string type)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var bytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(30);
                    page.Header().Text($"{type} Report").FontSize(20).Bold();
                    page.Content().PaddingTop(10).Text($"Library Management System Report - Generated on {DateTime.Now:dd-MM-yyyy HH:mm}");
                });
            }).GeneratePdf();

            return File(bytes, "application/pdf", $"{type}_Report.pdf");
        }
    }
}
