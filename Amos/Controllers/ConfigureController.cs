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
        // ------------------------------------------------------------------------------------------
        // INITAL LOAD of this page: (Occurs once.)
        // ------------------------------------------------------------------------------------------

        public ActionResult Index(int id)
        {
            int bookId = id;

            var pageInitModel = new PageInitModel();
            pageInitModel.PageQueryModel = new PageQueryModel();
            pageInitModel.PageQueryModel.BookId = bookId;

            if (Session["ShowPageContent"] == null)
            {
                pageInitModel.PageQueryModel.ShowPageContent = false;
            } else
            {
                pageInitModel.PageQueryModel.ShowPageContent = (bool)Session["ShowPageContent"];
            }

            return View("Index", pageInitModel);
        }

        // ------------------------------------------------------------------------------------------
        // LOAD AND RENDER the Book Outline that is displayed on the left side of the page. (This is reloaded and rendered each time a change is made.) 
        // ------------------------------------------------------------------------------------------

        public ActionResult BookOutline(PageQueryModel PageQueryModel)
        {
            //System.Threading.Thread.Sleep(250 * (DateTime.Now.Second % 10));
            Session["ShowPageContent"] = PageQueryModel.ShowPageContent;
            
            var bookOutlineModel = new BookOutlineModel();
            bookOutlineModel.PageQueryModel = PageQueryModel;

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
                                                   Name = s.Name,
                                                   Theme = s.Theme,
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

            if (PageQueryModel.ShowPageContent)
            {
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
            } else
            {
                bookOutlineModel.OutlinePages = (from s in db.Pages
                                                 where s.BookId == PageQueryModel.BookId
                                                 orderby s.SortOrder
                                                 select new OutlinePage
                                                 {
                                                     PageId = s.PageId,
                                                     ChapterId = s.ChapterId,
                                                     Title = s.Title,
                                                     Type = s.Type,
                                                 }).ToList();
            }


            return View("BookOutline", bookOutlineModel);
        }

        // ------------------------------------------------------------------------------------------
        // The remaining ACTIONS are executed in response to the various actions occuring on the right side of the page:
        // ------------------------------------------------------------------------------------------

        // Users changes the NAME/TITLE of any object (book/module/section/chapter/page):
        public ActionResult UpdateName(ActionRequest UpdateNameRequest)
        {
            var db = new ApplicationDbContext();
            switch (UpdateNameRequest.Type)
            {
                case "book":
                    var book = db.Books.Find(UpdateNameRequest.Id);
                    book.Name = UpdateNameRequest.Text;
                    break;
                case "module":
                    var module = db.Modules.Find(UpdateNameRequest.Id);
                    module.Name = UpdateNameRequest.Text;
                    break;
                case "section":
                    var section = db.Sections.Find(UpdateNameRequest.Id);
                    section.Name = UpdateNameRequest.Text;
                    break;
                case "chapter":
                    var chapter = db.Chapters.Find(UpdateNameRequest.Id);
                    chapter.Name = UpdateNameRequest.Text;
                    break;
                case "page":
                    var page = db.Pages.Find(UpdateNameRequest.Id);
                    page.Title = UpdateNameRequest.Text;
                    break;
            }
            db.SaveChanges();
            return BookOutline(UpdateNameRequest.PageQueryModel);
        }

        // User changes VERSION of a book: 
        public ActionResult UpdateBookVersion(ActionRequest UpdateBookVersionRequest)
        {
            var db = new ApplicationDbContext();
            var book = db.Books.Find(UpdateBookVersionRequest.Id);
            book.Version = UpdateBookVersionRequest.Text;
            db.SaveChanges();
            return BookOutline(UpdateBookVersionRequest.PageQueryModel);
        }

        // User changes the THEME of a module:
        public ActionResult UpdateTheme(ActionRequest UpdateThemeRequest)
        {
            var db = new ApplicationDbContext();
            var module = db.Modules.Find(UpdateThemeRequest.Id);
            module.Theme = UpdateThemeRequest.Text;
            db.SaveChanges();
            return BookOutline(UpdateThemeRequest.PageQueryModel);
        }

        // User ADDS a new module, section, chapter, or page:
        public ActionResult AddItem(ActionRequest AddItemRequest)
        {
            var db = new ApplicationDbContext();
            switch (AddItemRequest.Type)
            {
                case "book": // new modules are added at TOP
                    var lowestExistingModuleSortOrder = (from s in db.Modules where s.BookId == AddItemRequest.Id select s.SortOrder).DefaultIfEmpty(0).Min();
                    var newModule = new Module();
                    newModule.SortOrder = lowestExistingModuleSortOrder - 1;
                    newModule.Name = "Untitled Module";
                    newModule.Theme = "1";
                    newModule.BookId = AddItemRequest.Id;
                    db.Modules.Add(newModule);
                    break;
                case "module": // new sections are added at BOTTOM
                    var highestExistingSectionSortOrder = (from s in db.Sections where s.ModuleId == AddItemRequest.Id select s.SortOrder).DefaultIfEmpty(0).Max();
                    var newSection = new Section();
                    newSection.SortOrder = highestExistingSectionSortOrder + 1;
                    newSection.Name = "Untitled Section";
                    newSection.ModuleId = AddItemRequest.Id;
                    db.Sections.Add(newSection);
                    break;
                case "section": // new chapters are added at BOTTOM
                    var highestExistingChapterSortOrder = (from s in db.Chapters where s.SectionId == AddItemRequest.Id select s.SortOrder).DefaultIfEmpty(0).Max();
                    var newChapter = new Chapter();
                    newChapter.SortOrder = highestExistingChapterSortOrder + 1;
                    newChapter.Name = "Untitled Chapter";
                    newChapter.SectionId = AddItemRequest.Id;
                    db.Chapters.Add(newChapter);
                    break;
                case "chapter": // new pages are added at BOTTOM
                    var highestExistingPageSortOrder = (from s in db.Pages where s.ChapterId == AddItemRequest.Id select s.SortOrder).DefaultIfEmpty(0).Max();
                    var newPage = new Page();
                    newPage.SortOrder = highestExistingPageSortOrder + 1;
                    newPage.Title = "Untitled Page";
                    newPage.ChapterId = AddItemRequest.Id;
                    newPage.BookId = AddItemRequest.PageQueryModel.BookId;
                    newPage.PageContent = "";
                    newPage.CreatedBy = User.Identity.Name;
                    newPage.CreateDate = DateTime.Now;
                    newPage.ModifiedBy = User.Identity.Name;
                    newPage.ModifyDate = newPage.CreateDate;
                    newPage.Type = "content";
                    db.Pages.Add(newPage);
                    break;
            }
            db.SaveChanges();
            return BookOutline(AddItemRequest.PageQueryModel);
        }

        // User REMOVES any module, section, chapter, or page:
        public ActionResult RemoveItem(ActionRequest RemoveItemRequest)
        {
            var db = new ApplicationDbContext();
            switch (RemoveItemRequest.Type)
            {
                case "module":
                    var module = db.Modules.Find(RemoveItemRequest.Id);
                    db.Modules.Remove(module);
                    var sections = (from s in db.Sections where s.ModuleId == RemoveItemRequest.Id select s);
                    var sectionIds = from s in sections select s.SectionId;
                    db.Sections.RemoveRange(sections);
                    var chapters2 = (from s in db.Chapters where sectionIds.Contains(s.SectionId) select s);
                    var chapterIds2 = from s in chapters2 select s.ChapterId;
                    db.Chapters.RemoveRange(chapters2);
                    var pages3 = (from s in db.Pages where chapterIds2.Contains(s.ChapterId) select s);
                    foreach (var page3 in pages3)
                    {
                        page3.BookId = 0;
                        page3.ChapterId = 0;
                        page3.SortOrder = 0;
                    }
                    break;
                case "section":
                    var section = db.Sections.Find(RemoveItemRequest.Id);
                    db.Sections.Remove(section);
                    var chapters = (from s in db.Chapters where s.SectionId == RemoveItemRequest.Id select s);
                    var chapterIds = from s in chapters select s.ChapterId;
                    db.Chapters.RemoveRange(chapters);
                    var pages2 = (from s in db.Pages where chapterIds.Contains(s.ChapterId) select s);
                    foreach (var page2 in pages2)
                    {
                        page2.BookId = 0;
                        page2.ChapterId = 0;
                        page2.SortOrder = 0;
                    }
                    break;
                case "chapter":
                    var chapter = db.Chapters.Find(RemoveItemRequest.Id);
                    db.Chapters.Remove(chapter);
                    var pages = (from s in db.Pages where s.ChapterId == RemoveItemRequest.Id select s);
                    foreach (var page2 in pages)
                    {
                        page2.BookId = 0;
                        page2.ChapterId = 0;
                        page2.SortOrder = 0;
                    }
                    break;
                case "page":
                    var page = db.Pages.Find(RemoveItemRequest.Id);
                    page.BookId = 0;
                    page.ChapterId = 0;
                    page.SortOrder = 0;
                    break;
            }
            db.SaveChanges();
            return BookOutline(RemoveItemRequest.PageQueryModel);
        }

        // User MOVEs an item using the select menu. (relocates a page to another chapter, etc.):
        public ActionResult MoveItem(ActionRequest MoveItemRequest)
        {
            var db = new ApplicationDbContext();
            switch (MoveItemRequest.Type)
            {
                case "section":
                    var highestExistingSectionSortOrder = (from s in db.Sections where s.ModuleId == MoveItemRequest.TargetId select s.SortOrder).DefaultIfEmpty(0).Max();
                    var section = db.Sections.Find(MoveItemRequest.Id);
                    section.ModuleId = MoveItemRequest.TargetId;
                    section.SortOrder = highestExistingSectionSortOrder + 1;
                    break;
                case "chapter":
                    var highestExistingChapterSortOrder = (from s in db.Chapters where s.SectionId == MoveItemRequest.TargetId select s.SortOrder).DefaultIfEmpty(0).Max();
                    var chapter = db.Chapters.Find(MoveItemRequest.Id);
                    chapter.SectionId = MoveItemRequest.TargetId;
                    chapter.SortOrder = highestExistingChapterSortOrder + 1;
                    break;
                case "page":
                    var highestExistingPageSortOrder = (from s in db.Pages where s.ChapterId == MoveItemRequest.TargetId select s.SortOrder).DefaultIfEmpty(0).Max();
                    var page = db.Pages.Find(MoveItemRequest.Id);
                    page.ChapterId = MoveItemRequest.TargetId;
                    page.SortOrder = highestExistingPageSortOrder + 1;
                    break;
            }
            db.SaveChanges();
            return BookOutline(MoveItemRequest.PageQueryModel);
        }

        // User reorders an item UP or DOWN within the outline:
        // (This code is lengthy and complex to account for the capability to move items beyond their parent into adjacent parents.)
        // Example: a page can be moved past the beginning or end of a chapter and into an adjacent chapter.
        public ActionResult ReorderItem(ActionRequest ReorderItemRequest)
        {
            var db = new ApplicationDbContext();
            switch (ReorderItemRequest.Type)
            {
                case "module":
                    if (ReorderItemRequest.Action == "up")
                    {
                        var module = db.Modules.Find(ReorderItemRequest.Id);
                        var adjacentModule = (from s in db.Modules where s.BookId == module.BookId && s.SortOrder < module.SortOrder orderby s.SortOrder descending select s).FirstOrDefault();
                        if (adjacentModule != null) // there IS an adjacent module within this book to swap with:
                        {
                            int swapSortOrder = adjacentModule.SortOrder;
                            adjacentModule.SortOrder = module.SortOrder;
                            module.SortOrder = swapSortOrder;
                        }
                    }
                    if (ReorderItemRequest.Action == "down")
                    {
                        var module = db.Modules.Find(ReorderItemRequest.Id);
                        var adjacentModule = (from s in db.Modules where s.BookId == module.BookId && s.SortOrder > module.SortOrder orderby s.SortOrder select s).FirstOrDefault();
                        if (adjacentModule != null) // there IS an adjacent module within this book to swap with:
                        {
                            int swapSortOrder = adjacentModule.SortOrder;
                            adjacentModule.SortOrder = module.SortOrder;
                            module.SortOrder = swapSortOrder;
                        }
                    }
                    break;
                case "section":
                    if (ReorderItemRequest.Action == "up")
                    {
                        var section = db.Sections.Find(ReorderItemRequest.Id);
                        var adjacentSection = (from s in db.Sections where s.ModuleId == section.ModuleId && s.SortOrder < section.SortOrder orderby s.SortOrder descending select s).FirstOrDefault();
                        if (adjacentSection != null) // there IS an adjacent section within this module to swap with:
                        {
                            int swapSortOrder = adjacentSection.SortOrder;
                            adjacentSection.SortOrder = section.SortOrder;
                            section.SortOrder = swapSortOrder;
                        }
                        else // there is NOT an adjacent section, we will need to move the section to the bottom of an adjacent module:
                        {
                            var allModules = (from s in db.Modules orderby s.SortOrder select s.ModuleId).ToList();
                            int moduleIndex = allModules.IndexOf(section.ModuleId);
                            if (moduleIndex > 0) // there IS an adjacent module to move this section to:
                            {
                                int targetModuleId = allModules[moduleIndex - 1];
                                var highestSectionSortOrderInTargetModule = (from s in db.Sections where s.ModuleId == targetModuleId select s.SortOrder).DefaultIfEmpty(0).Max();
                                section.ModuleId = targetModuleId;
                                section.SortOrder = highestSectionSortOrderInTargetModule + 1;
                            } // otherwise, there is no adjacent section AND no adjacent module... the section can be moved no further: take no action.
                        }
                    }
                    if (ReorderItemRequest.Action == "down")
                    {
                        var section = db.Sections.Find(ReorderItemRequest.Id);
                        var adjacentSection = (from s in db.Sections where s.ModuleId == section.ModuleId && s.SortOrder > section.SortOrder orderby s.SortOrder select s).FirstOrDefault();
                        if (adjacentSection != null) // there IS an adjacent section within this module to swap with:
                        {
                            int swapSortOrder = adjacentSection.SortOrder;
                            adjacentSection.SortOrder = section.SortOrder;
                            section.SortOrder = swapSortOrder;
                        }
                        else // there is NOT an adjacent section, we will need to move the section to the top of an adjacent module:
                        {
                            var allModules = (from s in db.Modules orderby s.SortOrder descending select s.ModuleId).ToList();
                            int moduleIndex = allModules.IndexOf(section.ModuleId);
                            if (moduleIndex > 0) // there IS an adjacent module to move this section to:
                            {
                                int targetModuleId = allModules[moduleIndex - 1];
                                var lowestSectionSortOrderInTargetModule = (from s in db.Sections where s.ModuleId == targetModuleId select s.SortOrder).DefaultIfEmpty(0).Min();
                                section.ModuleId = targetModuleId;
                                section.SortOrder = lowestSectionSortOrderInTargetModule - 1;
                            } // otherwise, there is no adjacent section AND no adjacent module... the section can be moved no further: take no action.
                        }
                    }
                    break;
                case "chapter":
                    if (ReorderItemRequest.Action == "up")
                    {
                        var chapter = db.Chapters.Find(ReorderItemRequest.Id);
                        var adjacentChapter = (from s in db.Chapters where s.SectionId == chapter.SectionId && s.SortOrder < chapter.SortOrder orderby s.SortOrder descending select s).FirstOrDefault();
                        if (adjacentChapter != null) // there IS an adjacent chapter within this chapter to swap with:
                        {
                            int swapSortOrder = adjacentChapter.SortOrder;
                            adjacentChapter.SortOrder = chapter.SortOrder;
                            chapter.SortOrder = swapSortOrder;
                        }
                        else // there is NOT an adjacent chapter, we will need to move the chapter to the bottom of an adjacent section:
                        {
                            var allSections = (from s in db.Sections orderby s.Module.SortOrder, s.SortOrder select s.SectionId).ToList();
                            int sectionIndex = allSections.IndexOf(chapter.SectionId);
                            if (sectionIndex > 0) // there IS an adjacent section to move this chapter to:
                            {
                                int targetSectionId = allSections[sectionIndex - 1];
                                var highestChapterSortOrderInTargetSection = (from s in db.Chapters where s.SectionId == targetSectionId select s.SortOrder).DefaultIfEmpty(0).Max();
                                chapter.SectionId = targetSectionId;
                                chapter.SortOrder = highestChapterSortOrderInTargetSection + 1;
                            } // otherwise, there is no adjacent chapter AND no adjacent section... the chapter can be moved no further: take no action.
                        }
                    }
                    if (ReorderItemRequest.Action == "down")
                    {
                        var chapter = db.Chapters.Find(ReorderItemRequest.Id);
                        var adjacentChapter = (from s in db.Chapters where s.SectionId == chapter.SectionId && s.SortOrder > chapter.SortOrder orderby s.SortOrder select s).FirstOrDefault();
                        if (adjacentChapter != null) // there IS an adjacent chapter within this chapter to swap with:
                        {
                            int swapSortOrder = adjacentChapter.SortOrder;
                            adjacentChapter.SortOrder = chapter.SortOrder;
                            chapter.SortOrder = swapSortOrder;
                        }
                        else // there is NOT an adjacent chapter, we will need to move the chapter to the top of an adjacent section:
                        {
                            var allSections = (from s in db.Sections orderby s.Module.SortOrder descending, s.SortOrder descending select s.SectionId).ToList();
                            int sectionIndex = allSections.IndexOf(chapter.SectionId);
                            if (sectionIndex > 0) // there IS an adjacent section to move this chapter to:
                            {
                                int targetSectionId = allSections[sectionIndex - 1];
                                var lowestChapterSortOrderInTargetSection = (from s in db.Chapters where s.SectionId == targetSectionId select s.SortOrder).DefaultIfEmpty(0).Min();
                                chapter.SectionId = targetSectionId;
                                chapter.SortOrder = lowestChapterSortOrderInTargetSection - 1;
                            } // otherwise, there is no adjacent chapter AND no adjacent section... the chapter can be moved no further: take no action.
                        }
                    }
                    break;
                case "page":
                    if (ReorderItemRequest.Action == "up")
                    {
                        var page = db.Pages.Find(ReorderItemRequest.Id);
                        var adjacentPage = (from s in db.Pages where s.ChapterId == page.ChapterId && s.SortOrder < page.SortOrder orderby s.SortOrder descending select s).FirstOrDefault();
                        if (adjacentPage != null) // there IS an adjacent page within this chapter to swap with:
                        {
                            int swapSortOrder = adjacentPage.SortOrder;
                            adjacentPage.SortOrder = page.SortOrder;
                            page.SortOrder = swapSortOrder;
                        }
                        else // there is NOT an adjacent page, we will need to move the page to the bottom of an adjacent chapter:
                        {
                            var allChapters = (from s in db.Chapters orderby s.Section.Module.SortOrder, s.Section.SortOrder, s.SortOrder select s.ChapterId).ToList();
                            int chapterIndex = allChapters.IndexOf(page.ChapterId);
                            if (chapterIndex > 0) // there IS an adjacent chapter to move this page to:
                            {
                                int targetChapterId = allChapters[chapterIndex - 1];
                                var highestPageSortOrderInTargetChapter = (from s in db.Pages where s.ChapterId == targetChapterId select s.SortOrder).DefaultIfEmpty(0).Max();
                                page.ChapterId = targetChapterId;
                                page.SortOrder = highestPageSortOrderInTargetChapter + 1;
                            } // otherwise, there is no adjacent page AND no adjacent chapter... the page can be moved no further: take no action.
                        }
                    }
                    if (ReorderItemRequest.Action == "down")
                    {
                        var page = db.Pages.Find(ReorderItemRequest.Id);
                        var adjacentPage = (from s in db.Pages where s.ChapterId == page.ChapterId && s.SortOrder > page.SortOrder orderby s.SortOrder select s).FirstOrDefault();
                        if (adjacentPage != null) // there IS an adjacent page within this chapter to swap with:
                        {
                            int swapSortOrder = adjacentPage.SortOrder;
                            adjacentPage.SortOrder = page.SortOrder;
                            page.SortOrder = swapSortOrder;
                        }
                        else // there is NOT an adjacent page, we will need to move the page to the top of an adjacent chapter:
                        {
                            var allChapters = (from s in db.Chapters orderby s.Section.Module.SortOrder descending, s.Section.SortOrder descending, s.SortOrder descending select s.ChapterId).ToList();
                            int chapterIndex = allChapters.IndexOf(page.ChapterId);
                            if (chapterIndex > 0) // there IS an adjacent chapter to move this page to:
                            {
                                int targetChapterId = allChapters[chapterIndex - 1];
                                var lowestPageSortOrderInTargetChapter = (from s in db.Pages where s.ChapterId == targetChapterId select s.SortOrder).DefaultIfEmpty(0).Min();
                                page.ChapterId = targetChapterId;
                                page.SortOrder = lowestPageSortOrderInTargetChapter - 1;
                            } // otherwise, there is no adjacent page AND no adjacent chapter... the page can be moved no further: take no action.
                        }
                    }
                    break;
            }
            db.SaveChanges();
            return BookOutline(ReorderItemRequest.PageQueryModel);
        } // End of (giant) action method that performs UP/DOWN page re-ordering.

    }  // END OF CONTROLLER CLASS

    // ------------------------------------------------------------------------------------------
    //                                  MODELS AND CLASSES
    // ------------------------------------------------------------------------------------------

    // Model used for the inital page load: (populated only once.)
    public class PageInitModel
    {
        public PageQueryModel PageQueryModel { get; set; }
    }

    // Model that persists through the page lifecycle. This is included with each action request.
    public class PageQueryModel
    {
        public int BookId { get; set; }
        public bool ShowPageContent { get; set; }
    }

    // Model used to render the book outline. This is re-created every time a change is made.
    public class BookOutlineModel
    {
        public PageQueryModel PageQueryModel { get; set; }
        public OutlineBook OutlineBook { get; set; }
        public List<OutlineModule> OutlineModules { get; set; }
        public List<OutlineSection> OutlineSections { get; set; }
        public List<OutlineChapter> OutlineChapters { get; set; }
        public List<OutlinePage> OutlinePages { get; set; }
    }

    // Components making up the book outline:
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
        public string Theme { get; set; }
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

    // Action Request Object. These are created in Javascript and passed to the server side Action methods whenever the user makes a modification to book outline.
    public class ActionRequest
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Action { get; set; }
        public string Text { get; set; }
        public int TargetId { get; set; }
        public PageQueryModel PageQueryModel { get; set; }
    }
    

}