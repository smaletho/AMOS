using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Amos.Controllers.Configuration
{
    public class ConfigureController : Controller
    {
        // GET: Configure
        public ActionResult Index(int id)
        {
            int bookId = id;

            var pageInitModel = new PageInitModel();
            pageInitModel.PageQueryModel = new PageQueryModel();

            return View("Index", pageInitModel);
        }

        public ActionResult BookOutline(PageQueryModel PageQueryModel)
        {
            var bookOutlineModel = new BookOutlineModel();

            return View("BookOutline",bookOutlineModel);
        }
    }

    public class PageInitModel
    {
        public PageQueryModel PageQueryModel { get; set; }
    }

    public class PageQueryModel
    {

    }

    public class BookOutlineModel
    {

    }
}