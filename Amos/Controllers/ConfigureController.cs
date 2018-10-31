using Amos.Models;
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
            pageInitModel.PageQueryModel.BookId = bookId;

            return View("Index", pageInitModel);
        }

        public ActionResult BookOutline(PageQueryModel PageQueryModel)
        {
            var bookOutlineModel = new BookOutlineModel();

            var db = new ApplicationDbContext();

            bookOutlineModel.OutlineBook = (from s in db.Books
                                            where s.BookId == PageQueryModel.BookId
                                            select new OutlineBook
                                            {
                                                BookId = s.BookId,
                                                Version = s.Version,
                                                Name = s.Name,
                                            }).FirstOrDefault();

            bookOutlineModel.OutlineModules = (from s in db.Modules
                                               where s.BookId == PageQueryModel.BookId
                                               orderby s.SortOrder
                                               select new OutlineModule
                                               {
                                                   ModuleId = s.ModuleId,
                                                   Name = s.Name
                                               }).ToList();
            var moduleIdList = (from s in bookOutlineModel.OutlineModules select s.ModuleId);

            bookOutlineModel.OutlineSections = (from s in db.Sections
                                               where moduleIdList.Contains(s.ModuleId)
                                                orderby s.SortOrder
                                               select new OutlineSection
                                               {
                                                   SectionId = s.SectionId,
                                                   ModuleId = s.ModuleId,
                                                   Name = s.Name
                                               }).ToList();
            var sectionIdList = (from s in bookOutlineModel.OutlineSections select s.SectionId);

            bookOutlineModel.OutlineChapters = (from s in db.Chapters
                                               where sectionIdList.Contains(s.SectionId)
                                                orderby s.SortOrder
                                               select new OutlineChapter
                                               {
                                                   ChapterId = s.ChapterId,
                                                   SectionId = s.SectionId,
                                                   Name = s.Name
                                               }).ToList();

            bookOutlineModel.OutlinePages = (from s in db.Pages
                                               where s.BookId == PageQueryModel.BookId
                                               orderby s.SortOrder
                                               select new OutlinePage
                                               {
                                                   PageId = s.PageId,
                                                   ChapterId = s.ChapterId,
                                                   Title = s.Title,
                                                   Type = s.Type,
                                                   PageContent = s.PageContent
                                               }).ToList();

            return View("BookOutline",bookOutlineModel);
        }

        public ActionResult UpdateName(UpdateNameRequest UpdateNameRequest)
        {
            var db = new ApplicationDbContext();
            switch (UpdateNameRequest.Type)
            {
                case "book":
                    var book = db.Books.Find(UpdateNameRequest.Id);
                    book.Name = UpdateNameRequest.Name;
                    break;
                case "module":
                    var module = db.Modules.Find(UpdateNameRequest.Id);
                    module.Name = UpdateNameRequest.Name;
                    break;
                case "section":
                    var section = db.Sections.Find(UpdateNameRequest.Id);
                    section.Name = UpdateNameRequest.Name;
                    break;
                case "chapter":
                    var chapter = db.Chapters.Find(UpdateNameRequest.Id);
                    chapter.Name = UpdateNameRequest.Name;
                    break;
                case "page":
                    var page = db.Pages.Find(UpdateNameRequest.Id);
                    page.Title = UpdateNameRequest.Name;
                    break;
            }
            db.SaveChanges();
            return BookOutline(UpdateNameRequest.PageQueryModel);
        }

        public ActionResult UpdateBookVersion(UpdateBookVersionRequest UpdateBookVersionRequest)
        {
            var db = new ApplicationDbContext();
            var book = db.Books.Find(UpdateBookVersionRequest.Id);
            book.Version = UpdateBookVersionRequest.Version;
            db.SaveChanges();
            return BookOutline(UpdateBookVersionRequest.PageQueryModel);
        }

        public ActionResult AddItem(RemoveItemRequest AddItemRequest)
        {
            var db = new ApplicationDbContext();

            return BookOutline(AddItemRequest.PageQueryModel);
        }

        public ActionResult RemoveItem(RemoveItemRequest RemoveItemRequest)
        {
            var db = new ApplicationDbContext();

            return BookOutline(RemoveItemRequest.PageQueryModel);
        }

        public ActionResult ReorderItem(RemoveItemRequest ReorderItemRequest)
        {
            var db = new ApplicationDbContext();

            return BookOutline(ReorderItemRequest.PageQueryModel);
        }
    }

    public class PageInitModel
    {
        public PageQueryModel PageQueryModel { get; set; }
    }

    public class PageQueryModel
    {
        public int BookId { get; set; }
    }

    public class BookOutlineModel
    {
        public OutlineBook OutlineBook { get; set; }
        public List<OutlineModule> OutlineModules { get; set; }
        public List<OutlineSection> OutlineSections { get; set; }
        public List<OutlineChapter> OutlineChapters { get; set; }
        public List<OutlinePage> OutlinePages { get; set; }
    }
    public class OutlineBook
    {
        public int BookId { get; set; }
        public string Version { get; set; }
        public string Name { get; set; }
    }
    public class OutlineModule
    {
        public int ModuleId { get; set; }
        public string Name { get; set; }
    }

    public class OutlineSection
    {
        public int SectionId { get; set; }
        public int ModuleId { get; set; }
        public string Name { get; set; }
    }

    public class OutlineChapter
    {
        public int ChapterId { get; set; }
        public int SectionId { get; set; }
        public string Name { get; set; }
    }

    public class OutlinePage
    {
        public int PageId { get; set; }
        public int ChapterId { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string PageContent { get; set; }
    }

    public class UpdateNameRequest
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public PageQueryModel PageQueryModel { get; set; }
    }

    public class UpdateBookVersionRequest
    {
        public int Id { get; set; }
        public string Version { get; set; }
        public PageQueryModel PageQueryModel { get; set; }
    }

    public class AddItemRequest
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public PageQueryModel PageQueryModel { get; set; }
    }

    public class RemoveItemRequest
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public PageQueryModel PageQueryModel { get; set; }
    }

    public class ReorderItemRequest
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Action { get; set; }
        public PageQueryModel PageQueryModel { get; set; }
    }

}