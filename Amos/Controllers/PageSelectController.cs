using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Amos.Models;

namespace Amos.Controllers
{
    public class PageSelectController : Controller
    {
        // GET: PageSelect
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddPage(int BookId, int ChapterId)
        {
            return View(new ChoosePageModel(BookId, ChapterId));
        }

        [HttpPost]
        public ActionResult DoAddPage(int PageId, int BookId, int ChapterId)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            // update the page
            var page = db.Pages.Where(x => x.PageId == PageId).FirstOrDefault();
            page.BookId = BookId;
            page.ChapterId = ChapterId;

            // get the last page in that chapter, iterate the sort order
            var lastChapterPage = db.Pages.Where(x => x.BookId == BookId & x.ChapterId == ChapterId).OrderByDescending(x => x.SortOrder).FirstOrDefault();
            if (lastChapterPage == null) page.SortOrder = 10;
            else page.SortOrder = lastChapterPage.SortOrder + 10;

            db.SaveChanges();

            return Content("success");
        }
    }
}