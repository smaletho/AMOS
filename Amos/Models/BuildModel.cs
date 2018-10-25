using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace Amos.Models
{
    public class AssetModel
    {
        public AssetModel() { }
        public AssetModel(int pageId)
        {
            ApplicationDbContext cdb = new ApplicationDbContext();
            this.FileList = cdb.AmosFiles.Where(x => x.PageId == pageId).ToList();
            this.PageId = pageId;
        }
        public List<AmosFile> FileList { get; set; }
        public int PageId { get; set; }

        [Required, FileExtensions(Extensions = "xml",
                     ErrorMessage = "Specify an XML file.")]
        public HttpPostedFileBase UploadedFile { get; set; }

    }

    public class ImportBookModel
    {
        [Required, FileExtensions(Extensions = "zip",
             ErrorMessage = "Specify a ZIP file.")]
        public HttpPostedFileBase File { get; set; }
    }

    public class PageViewModel
    {
        //[Required, FileExtensions(Extensions = "xml",
        //             ErrorMessage = "Specify an XML file.")]
        //public HttpPostedFileBase File { get; set; }

        [AllowHtml]
        public string XmlContent { get; set; }
        public int PageId { get; set; }
        public string PageName { get; set; }
        public string PageType { get; set; }
    }

    public class PageListModel
    {
        public PageListModel()
        {
            this.GetBook = new Book();
            this.Modules = new List<Module>();
            this.Sections = new List<Section>();
            this.Chapters = new List<Chapter>();
            this.PageList = new List<Page>();

            this.GetBook.BookId = 1;
            this.GetBook.Name = "New Book";
            this.GetBook.Published = false;
            this.GetBook.Version = "1";

            Module module = new Module
            {
                Name = "New Module",
                SortOrder = 10,
                Theme = "1"
            };
            this.Modules.Add(module);

            Section section = new Section
            {
                Name = "New Section",
                SortOrder = 10
            };
            this.Sections.Add(section);

            Chapter chapter = new Chapter
            {
                Name = "New Chapter",
                SortOrder = 10
            };
            this.Chapters.Add(chapter);

            Page page = new Page
            {
                SortOrder = 10,
                Title = "New Page",
                Type = "content"
            };
            this.PageList.Add(page);

        }
        public PageListModel(int bookId)
        {
            this.PageList = new List<Page>();
            ApplicationDbContext cdb = new ApplicationDbContext();
            if (bookId == 0)
            {
                // find all pages that are not in the previous list
                this.PageList = cdb.Pages.Where(x => x.BookId == 0).ToList();

                this.Modules = new List<Module>();
                this.Sections = new List<Section>();
                this.Chapters = new List<Chapter>();
                this.GetBook = new Book();
            }
            else
            {
                this.PageList = cdb.Pages.Where(x => x.BookId == bookId).ToList();

                this.GetBook = cdb.Books.Where(x => x.BookId == bookId).FirstOrDefault();

                this.Modules = cdb.Modules.Where(x => x.BookId == bookId).ToList();
                var moduleIdList = this.Modules.Select(x => x.ModuleId).ToList();

                this.Sections = cdb.Sections.Where(x => moduleIdList.Contains(x.ModuleId)).ToList();
                var sectionIdList = this.Sections.Select(x => x.SectionId).ToList();

                this.Chapters = cdb.Chapters.Where(x => sectionIdList.Contains(x.SectionId)).ToList();
            }
        }
        public List<Page> PageList { get; set; }
        //public List<BookPage> BookPages { get; set; }
        public Book GetBook { get; set; }
        public List<Section> Sections { get; set; }
        public List<Module> Modules { get; set; }
        public List<Chapter> Chapters { get; set; }
    }



    public class ManagePagesModel
    {
        public ManagePagesModel() { }
        public ManagePagesModel(int id)
        {
            this.PageListModel = new PageListModel(id);
            this.pageButtons = new List<PageButton>();

            foreach (var page in this.PageListModel.PageList)
            {
                // get the content, and turn into XML
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(page.PageContent);

                foreach (XmlElement contentNode in xmlDoc.ChildNodes[0].ChildNodes)
                {
                    if (contentNode.LocalName == "button")
                    {
                        try
                        {
                            string classList = contentNode.Attributes["class"].Value;
                            if (!classList.Contains("quiz-submit") && !classList.Contains("survey-submit"))
                                this.pageButtons.Add(new PageButton(contentNode, this.PageListModel.PageList, page.PageId, true));
                        }
                        catch {
                            // This is here on purpose. Buttons don't always have an attribute "class" so failing is good, because it
                            //  means it's not a quiz or survey button
                            this.pageButtons.Add(new PageButton(contentNode, this.PageListModel.PageList, page.PageId, true));
                        }
                    }
                    else if (contentNode.LocalName == "text")
                    {
                        foreach (XmlNode textChild in contentNode.ChildNodes)
                        {
                            try
                            {
                                if (textChild.LocalName == "a" && textChild.Attributes["class"].Value.Contains("navigateTo"))
                                    this.pageButtons.Add(new PageButton(textChild, this.PageListModel.PageList, page.PageId, false));
                            }
                            catch { }

                        }

                    }
                }
            }
        }
        public PageListModel PageListModel { get; set; }
        public List<PageButton> pageButtons { get; set; }
    }

    public class PageButton
    {
        public PageButton() { }
        public PageButton(XmlNode xmlElement, List<Page> pages, int pageId, bool isButton)
        {
            try
            {
                this.PageId = pageId;
                this.getPage = pages.Find(x => x.PageId == this.PageId);
            }
            catch
            {
                this.PageId = 0;
            }
            this.ButtonText = xmlElement.InnerText;
            try
            {
                string ogId = "";
                if (isButton)
                {
                    ogId = xmlElement.Attributes["id"].Value;
                }
                else
                {
                    ogId = xmlElement.Attributes["data-id"].Value;
                }
                this.NavPageId = Convert.ToInt32(ogId.Split('_')[1]);
                this.getNavPage = pages.Find(x => x.PageId == this.NavPageId);
            }
            catch
            {
                this.NavPageId = 0;
            }
            this.isButton = isButton;
        }
        public int PageId { get; set; }
        public string ButtonText { get; set; }
        public int NavPageId { get; set; }

        public bool isButton { get; set; }
        // true == button

        public Page getPage { get; set; }

        public Page getNavPage { get; set; }
    }

    public class Build_PageModel
    {
        public Build_PageModel() { }
        public Build_PageModel(Page item, int m, int s, int c, int p)
        {
            this.GetPage = item;
            this.ModuleCount = m;
            this.SectionCount = s;
            this.ChapterCount = c;
            this.PageCount = p;
        }
        public Build_PageModel(int m, int s, int c, int p)
        {
            this.GetPage = new Page
            {
                Title = "New Page",
            };
            this.ModuleCount = m;
            this.SectionCount = s;
            this.ChapterCount = c;
            this.PageCount = p;
        }
        public Page GetPage { get; set; }
        public int ModuleCount { get; set; }
        public int SectionCount { get; set; }
        public int ChapterCount { get; set; }
        public int PageCount { get; set; }
    }
}