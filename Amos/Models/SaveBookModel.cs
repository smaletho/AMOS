using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Amos.Models
{
    public class SaveBookModel
    {
        public SaveBookModel() { }
        public SaveBookModel(PageListModel model)
        {
            this.S_Book = new S_Book(model.GetBook);
            this.s_Modules = new List<S_Module>();
            this.s_Sections = new List<S_Section>();
            this.s_Chapters = new List<S_Chapter>();
            this.s_Pages = new List<S_Page>();
            foreach (var mod in model.Modules)
            {
                this.s_Modules.Add(new S_Module(mod));
            }
            foreach (var sec in model.Sections)
            {
                this.s_Sections.Add(new S_Section(sec));
            }
            foreach (var cha in model.Chapters)
            {
                this.s_Chapters.Add(new S_Chapter(cha));
            }
            foreach (var pa in model.PageList)
            {
                this.s_Pages.Add(new S_Page(pa));
            }
        }
        public S_Book S_Book { get; set; }
        public List<S_Module> s_Modules { get; set; }
        public List<S_Section> s_Sections { get; set; }
        public List<S_Chapter> s_Chapters { get; set; }
        public List<S_Page> s_Pages { get; set; }
    }

    public class S_Book
    {
        public S_Book() { }
        public S_Book(Book book)
        {
            this.BookId = book.BookId;
            this.Name = book.Name;
            this.Version = book.Version;
        }
        public int BookId { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
    }

    public class S_Module
    {
        public S_Module() { }
        public S_Module(Module module)
        {
            this.ModuleId = module.ModuleId;
            this.ParentId = module.BookId;
            this.Name = module.Name;
            this.SortOrder = module.SortOrder;
            this.Theme = module.Theme;
        }
        public int ModuleId { get; set; }
        public int ParentId { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public string Theme { get; set; }
    }

    public class S_Section
    {
        public S_Section() { }
        public S_Section(Section section)
        {
            this.Name = section.Name;
            this.SectionId = section.SectionId;
            this.ParentId = section.ModuleId;
            this.SortOrder = section.SortOrder;
        }
        public int SectionId { get; set; }
        public int ParentId { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
    }

    public class S_Chapter
    {
        public S_Chapter() { }
        public S_Chapter(Chapter chapter)
        {
            this.ChapterId = chapter.ChapterId;
            this.Name = chapter.Name;
            this.ParentId = chapter.SectionId;
            this.SortOrder = chapter.SortOrder;
        }
        public int ChapterId { get; set; }
        public int ParentId { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
    }

    public class S_Page
    {
        public S_Page() { }
        public S_Page(Page page)
        {
            this.ParentId = page.ChapterId;
            this.Name = page.Title;
            this.PageId = page.PageId;
            this.SortOrder = page.SortOrder;
        }
        public int PageId { get; set; }
        public int ParentId { get; set; }
        public int SortOrder { get; set; }
        public string Name { get; set; }
    }
}