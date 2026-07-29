using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Web.Services;
using LibraryManagementSystem.Web.Models;

namespace LibraryManagementSystem.Web.Controllers
{
    public class NewspapersController : Controller
    {
        private readonly NewspaperService _newspaperService;

        public NewspapersController(NewspaperService newspaperService)
        {
            _newspaperService = newspaperService;
        }

        public IActionResult Index(string search, string publisher, string status)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
                return RedirectToAction("Login", "Account");

            ViewData["Title"] = "Newspapers Collection";
            ViewData["ActivePage"] = "Newspapers";

            List<Newspaper> newspapers;
            if (!string.IsNullOrEmpty(search))
            {
                newspapers = _newspaperService.SearchNewspapers(search);
            }
            else if (!string.IsNullOrEmpty(publisher) && publisher != "All")
            {
                newspapers = _newspaperService.GetNewspapersByPublisher(publisher);
            }
            else if (!string.IsNullOrEmpty(status) && status != "All")
            {
                newspapers = _newspaperService.GetNewspapersByStatus(status);
            }
            else
            {
                newspapers = _newspaperService.GetAllNewspapers();
            }

            ViewBag.Publishers = _newspaperService.GetPublishers();
            ViewBag.Editions = _newspaperService.GetEditions();
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentPublisher = publisher;
            ViewBag.CurrentStatus = status;

            return View(newspapers);
        }

        [HttpPost]
        public IActionResult Save(Newspaper newspaper)
        {
            var result = newspaper.Id == 0 ? _newspaperService.AddNewspaper(newspaper) : _newspaperService.UpdateNewspaper(newspaper);
            TempData["Message"] = result.Message;
            TempData["Success"] = result.Success;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var result = _newspaperService.DeleteNewspaper(id);
            TempData["Message"] = result.Message;
            TempData["Success"] = result.Success;
            return RedirectToAction("Index");
        }
    }
}
