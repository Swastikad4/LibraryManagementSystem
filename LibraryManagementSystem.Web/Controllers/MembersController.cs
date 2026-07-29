using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Web.Services;
using LibraryManagementSystem.Web.Models;

namespace LibraryManagementSystem.Web.Controllers
{
    public class MembersController : Controller
    {
        private readonly MemberService _memberService;

        public MembersController(MemberService memberService)
        {
            _memberService = memberService;
        }

        public IActionResult Index(string search)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
                return RedirectToAction("Login", "Account");

            ViewData["Title"] = "Members Registry";
            ViewData["ActivePage"] = "Members";

            var members = string.IsNullOrEmpty(search) ? _memberService.GetAllMembers() : _memberService.SearchMembers(search);
            ViewBag.CurrentSearch = search;

            return View(members);
        }

        [HttpPost]
        public IActionResult Save(Member member)
        {
            var result = member.MemberID == 0 ? _memberService.AddMember(member) : _memberService.UpdateMember(member);
            TempData["Message"] = result.Message;
            TempData["Success"] = result.Success;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var result = _memberService.DeleteMember(id);
            TempData["Message"] = result.Message;
            TempData["Success"] = result.Success;
            return RedirectToAction("Index");
        }
    }
}
