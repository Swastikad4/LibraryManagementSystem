using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Web.Services;

namespace LibraryManagementSystem.Web.Controllers
{
    public class ReturnController : Controller
    {
        private readonly IssueReturnService _issueReturnService;

        public ReturnController(IssueReturnService issueReturnService)
        {
            _issueReturnService = issueReturnService;
        }

        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
                return RedirectToAction("Login", "Account");

            ViewData["Title"] = "Return Book";
            ViewData["ActivePage"] = "Return";

            var issued = _issueReturnService.GetCurrentlyIssued();
            return View(issued);
        }

        [HttpPost]
        public IActionResult Submit(int issueId)
        {
            var result = _issueReturnService.ReturnBook(issueId);
            TempData["Message"] = result.Message;
            TempData["Success"] = result.Success;
            return RedirectToAction("Index");
        }
    }
}
