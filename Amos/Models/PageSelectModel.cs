using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Amos.Models
{
    public class ChoosePageModel
    {
        public ChoosePageModel() { }
        public ChoosePageModel(int BookId, int ChapterId)
        {
            this.BookId = BookId;
            this.ChapterId = ChapterId;

            ApplicationDbContext db = new ApplicationDbContext();
            this.PageList = db.Pages.Where(x => x.BookId == 0).ToList();
        }
        public int BookId { get; set; }
        public int ChapterId { get; set; }
        public List<Page> PageList { get; set; }
    }
    public class PageSelectModel
    {
    }
}