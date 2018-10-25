using Amos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Amos.Controllers
{
    public class ViewController : Controller
    {
        // GET: View
        public ActionResult Index(int id)
        {
            ViewBag.BookId = id;
            return View();
        }

        [HttpPost]
        public JsonResult LoadBook(int id)
        {

            BookModel model = new BookModel(id);

            // model
            //  ConfigXml (document)
            //  PageContent (List<PageContentItem>)
            //      PageContentItem:
            //          "Module" (string)
            //          "Section" (string)
            //          "Chapter" (string)
            //          "Page" (string)
            //          "content" document
            //
            //
            //
            //

            return Json(new
            {
                ConfigXml = model.ConfigXml.OuterXml,
                PageContent = model.PageContent
            });
        }

        [HttpPost]
        public JsonResult LoadTracker(string email, string id)
        {
            ApplicationDbContext cdb = new ApplicationDbContext();
            var tracker = cdb.UserTrackers.Where(x => x.Email == email && x.BookId == id).FirstOrDefault();
            if (tracker == null) return Json("none");
            else
            {
                return Json(tracker.TrackerContent);
            }
        }

        [HttpPost]
        public ActionResult SaveTracker(string BookId, string email, string UserTracker)
        {
            ApplicationDbContext cdb = new ApplicationDbContext();
            var existingTracker = cdb.UserTrackers.Where(x => x.BookId == BookId && x.Email == email).FirstOrDefault();
            if (existingTracker == null)
            {
                UserTracker track = new UserTracker();
                track.BookId = BookId;
                track.Email = email;
                track.TrackerContent = UserTracker;
                cdb.UserTrackers.Add(track);
            }
            else
            {
                existingTracker.TrackerContent = UserTracker;
            }
            cdb.SaveChanges();
            return Content("success");
        }
    }
}