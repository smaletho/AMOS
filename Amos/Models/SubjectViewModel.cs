using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Amos.Models
{
    public class SubjectListModel
    {
        public SubjectListModel() { }
        public SubjectListModel(int id)
        {
            ApplicationDbContext cdb = new ApplicationDbContext();
            this.Book = cdb.Books.Where(x => x.BookId == id).FirstOrDefault();
            this.UserTrackers = new List<UserTracker>();

            string bookId = "b_" + id;
            this.UserTrackers = cdb.UserTrackers.Where(x => x.BookId == bookId).ToList();

        }
        public List<UserTracker> UserTrackers { get; set; }
        public Book Book { get; set; }
    }

    public class SubjectBookListModel
    {
        public SubjectBookListModel()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            this.books = db.Books.Where(x => x.Published).ToList();
            this.userTrackers = db.UserTrackers.ToList();
        }
        public List<UserTracker> userTrackers { get; set; }
        public List<Book> books { get; set; }
    }

    public class SubjectViewModel
    {
        public SubjectViewModel(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            this.userTracker = db.UserTrackers.Where(x => x.UserTrackerId == id).FirstOrDefault();

            string bookId = this.userTracker.BookId;
            int newBookId = 0;
            try
            {
                newBookId = Convert.ToInt32(bookId.Split('_')[1]);
                this.book = db.Books.Where(x => x.BookId == newBookId).FirstOrDefault();
            }
            catch { this.book = new Book(); }
        }
        public Book book { get; set; }
        public UserTracker userTracker { get; set; }
    }
}