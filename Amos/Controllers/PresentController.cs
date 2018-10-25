using Amos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Amos.Controllers
{
    public class PresentController : Controller
    {
        // GET: Present
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewBooks()
        {
            ApplicationDbContext adb = new ApplicationDbContext();
            return View(adb.Books.Where(x => x.Published).ToList());
        }
    }
}