using Amos.Models;
using System.Linq;
using System.Web.Mvc;

namespace Amos.Controllers
{
    public class HomeController : Controller
    {
        

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