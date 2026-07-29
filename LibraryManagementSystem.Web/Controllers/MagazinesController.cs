using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Web.Services;
using LibraryManagementSystem.Web.Models;

namespace LibraryManagementSystem.Web.Controllers
{
    public class MagazinesController : Controller
    {
        private readonly MagazineService _magazineService;

        public MagazinesController(MagazineService magazineService)
        {
            _magazineService = magazineService;
        }

        public IActionResult Index(string search, string publisher, string status)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
                return RedirectToAction("Login", "Account");

            ViewData["Title"] = "Magazines Collection";
            ViewData["ActivePage"] = "Magazines";

            List<Magazine> magazines;
            if (!string.IsNullOrEmpty(search))
            {
                magazines = _magazineService.SearchMagazines(search);
            }
            else if (!string.IsNullOrEmpty(publisher) && publisher != "All")
            {
                magazines = _magazineService.GetMagazinesByPublisher(publisher);
            }
            else if (!string.IsNullOrEmpty(status) && status != "All")
            {
                magazines = _magazineService.GetMagazinesByStatus(status);
            }
            else
            {
                magazines = _magazineService.GetAllMagazines();
            }

            ViewBag.Publishers = _magazineService.GetPublishers();
            ViewBag.Categories = _magazineService.GetCategories();
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentPublisher = publisher;
            ViewBag.CurrentStatus = status;

            return View(magazines);
        }

        [HttpPost]
        public IActionResult Save(Magazine magazine)
        {
            var result = magazine.Id == 0 ? _magazineService.AddMagazine(magazine) : _magazineService.UpdateMagazine(magazine);
            TempData["Message"] = result.Message;
            TempData["Success"] = result.Success;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var result = _magazineService.DeleteMagazine(id);
            TempData["Message"] = result.Message;
            TempData["Success"] = result.Success;
            return RedirectToAction("Index");
        }
    }
}
