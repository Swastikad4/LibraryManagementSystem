using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Web.Services;

namespace LibraryManagementSystem.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly BookService _bookService;
        private readonly MemberService _memberService;
        private readonly IssueReturnService _issueReturnService;
        private readonly MagazineService _magazineService;
        private readonly NewspaperService _newspaperService;

        public DashboardController(BookService bookService, MemberService memberService, IssueReturnService issueReturnService, MagazineService magazineService, NewspaperService newspaperService)
        {
            _bookService = bookService;
            _memberService = memberService;
            _issueReturnService = issueReturnService;
            _magazineService = magazineService;
            _newspaperService = newspaperService;
        }

        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
                return RedirectToAction("Login", "Account");

            ViewData["Title"] = "Dashboard";
            ViewData["ActivePage"] = "Dashboard";

            ViewBag.TotalBooks = _bookService.GetTotalCount();
            ViewBag.AvailableBooks = _bookService.GetAvailableCount();
            ViewBag.BorrowedBooks = _issueReturnService.GetBorrowedCount();
            ViewBag.TotalMembers = _memberService.GetTotalCount();
            ViewBag.DueToday = _issueReturnService.GetDueTodayCount();
            ViewBag.OverdueBooks = _issueReturnService.GetOverdueCount();

            ViewBag.TotalMagazines = _magazineService.GetTotalCount();
            ViewBag.AvailableMagazines = _magazineService.GetAvailableCount();

            ViewBag.TotalNewspapers = _newspaperService.GetTotalCount();
            ViewBag.AvailableNewspapers = _newspaperService.GetAvailableCount();

            ViewBag.FullName = HttpContext.Session.GetString("FullName") ?? "Admin";

            return View();
        }
    }
}
