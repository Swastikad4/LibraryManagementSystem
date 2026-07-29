using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Web.Services;

namespace LibraryManagementSystem.Web.Controllers
{
    public class IssueController : Controller
    {
        private readonly BookService _bookService;
        private readonly MemberService _memberService;
        private readonly IssueReturnService _issueReturnService;

        public IssueController(BookService bookService, MemberService memberService, IssueReturnService issueReturnService)
        {
            _bookService = bookService;
            _memberService = memberService;
            _issueReturnService = issueReturnService;
        }

        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
                return RedirectToAction("Login", "Account");

            ViewData["Title"] = "Issue Book";
            ViewData["ActivePage"] = "Issue";

            ViewBag.Books = _bookService.GetAllBooks().Where(b => b.AvailableCopies > 0).ToList();
            ViewBag.Members = _memberService.GetAllMembers();

            return View();
        }

        [HttpPost]
        public IActionResult Submit(int bookId, int memberId)
        {
            var result = _issueReturnService.IssueBook(bookId, memberId);
            TempData["Message"] = result.Message;
            TempData["Success"] = result.Success;
            return RedirectToAction("Index");
        }
    }
}
