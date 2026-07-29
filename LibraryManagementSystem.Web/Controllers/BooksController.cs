using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Web.Services;
using LibraryManagementSystem.Web.Models;

namespace LibraryManagementSystem.Web.Controllers
{
    public class BooksController : Controller
    {
        private readonly BookService _bookService;

        public BooksController(BookService bookService)
        {
            _bookService = bookService;
        }

        public IActionResult Index(string search, string category)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
                return RedirectToAction("Login", "Account");

            ViewData["Title"] = "Books Collection";
            ViewData["ActivePage"] = "Books";

            List<Book> books;
            if (!string.IsNullOrEmpty(search))
            {
                books = _bookService.SearchBooks(search);
            }
            else if (!string.IsNullOrEmpty(category) && category != "All")
            {
                books = _bookService.GetBooksByCategory(category);
            }
            else
            {
                books = _bookService.GetAllBooks();
            }

            ViewBag.Categories = _bookService.GetCategories();
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentCategory = category;

            return View(books);
        }

        [HttpPost]
        public IActionResult Save(Book book)
        {
            var result = book.BookID == 0 ? _bookService.AddBook(book) : _bookService.UpdateBook(book);
            TempData["Message"] = result.Message;
            TempData["Success"] = result.Success;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var result = _bookService.DeleteBook(id);
            TempData["Message"] = result.Message;
            TempData["Success"] = result.Success;
            return RedirectToAction("Index");
        }
    }
}
