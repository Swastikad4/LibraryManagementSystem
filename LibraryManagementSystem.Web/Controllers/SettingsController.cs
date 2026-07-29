using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Web.DataAccess;

namespace LibraryManagementSystem.Web.Controllers
{
    public class SettingsController : Controller
    {
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
                return RedirectToAction("Login", "Account");

            ViewData["Title"] = "Settings & Backup";
            ViewData["ActivePage"] = "Settings";

            return View();
        }

        [HttpGet]
        public IActionResult Backup()
        {
            string dbPath = DatabaseHelper.GetDatabasePath();
            if (!System.IO.File.Exists(dbPath)) return NotFound();

            byte[] bytes = System.IO.File.ReadAllBytes(dbPath);
            return File(bytes, "application/octet-stream", $"library_backup_{DateTime.Now:yyyyMMdd_HHmmss}.db");
        }

        [HttpPost]
        public IActionResult Restore(IFormFile dbFile)
        {
            if (dbFile != null && dbFile.Length > 0)
            {
                string target = DatabaseHelper.GetDatabasePath();
                using (var stream = new FileStream(target, FileMode.Create))
                {
                    dbFile.CopyTo(stream);
                }
                TempData["Message"] = "Database restored successfully!";
                TempData["Success"] = true;
            }
            else
            {
                TempData["Message"] = "Please select a valid .db file.";
                TempData["Success"] = false;
            }
            return RedirectToAction("Index");
        }
    }
}
