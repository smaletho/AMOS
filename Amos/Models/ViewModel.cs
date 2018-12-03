using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace Amos.Models
{
    public class BookModel
    {
        public BookModel() { }
        public BookModel(int id)
        {
            // Load the book. 
            //  I'll be passing 2 variables to the page:
            //      1. ConfigXml - outlines of the chapters, content, etc.
            //                      does not include *page* content
            //      2. PageContent - contains strictly the pages and what's on them (text, etc.)

            this.ConfigXml = new XmlDocument();
            this.PageContent = new List<PageContentItem>();

            ApplicationDbContext cdb = new ApplicationDbContext();
            var book = cdb.Books.Where(x => x.BookId == id).FirstOrDefault();

            var modules = cdb.Modules.Where(x => x.BookId == book.BookId).ToList();
            var moduleIds = modules.Select(x => x.ModuleId).ToList();

            var sections = cdb.Sections.Where(x => moduleIds.Contains(x.ModuleId)).ToList();
            var sectionIds = sections.Select(x => x.SectionId).ToList();

            var chapters = cdb.Chapters.Where(x => sectionIds.Contains(x.SectionId)).ToList();
            var chapterIds = chapters.Select(x => x.ChapterId).ToList();

            //var bookPages = cdb.BookPages.Where(x => chapterIds.Contains(x.ChapterId)).ToList();
            //var bookPageIds = bookPages.Select(x => x.PageId).ToList();

            var pages = cdb.Pages.Where(x => x.BookId == id).ToList();

            // configXml.DocumentElement --> book
            XmlElement root = this.ConfigXml.CreateElement("book");
            root.SetAttribute("id", "b_" + book.BookId);
            root.SetAttribute("name", book.Name);
            root.SetAttribute("version", book.Version);
            root.SetAttribute("author", book.CreatedBy);
            root.SetAttribute("createdate", book.CreateDate.ToString("yyyyMMdd"));
            root.SetAttribute("modifydate", book.ModifyDate.ToString("yyyyMMdd"));
            this.ConfigXml.AppendChild(root);

            int moduleCount = 1;
            int sectionCount = 1;
            int chapterCount = 1;


            foreach (var m in modules.OrderBy(x => x.SortOrder).ToList())
            {
                XmlElement mod = this.ConfigXml.CreateElement("module");
                mod.SetAttribute("name", m.Name);
                mod.SetAttribute("theme", m.Theme);
                mod.SetAttribute("id", "m_" + moduleCount);


                root.AppendChild(mod);

                foreach (var s in sections.Where(x => x.ModuleId == m.ModuleId).OrderBy(x => x.SortOrder).ToList())
                {
                    XmlElement sec = this.ConfigXml.CreateElement("section");
                    sec.SetAttribute("name", s.Name);
                    sec.SetAttribute("id", "s_" + sectionCount);


                    mod.AppendChild(sec);

                    foreach (var c in chapters.Where(x => x.SectionId == s.SectionId).OrderBy(x => x.SortOrder).ToList())
                    {
                        XmlElement cha = this.ConfigXml.CreateElement("chapter");
                        cha.SetAttribute("name", c.Name);
                        cha.SetAttribute("id", "c_" + chapterCount);


                        sec.AppendChild(cha);

                        foreach (var p in pages.Where(x => x.ChapterId == c.ChapterId).OrderBy(x => x.SortOrder).ToList())
                        {
                            //XmlDocumentFragment xmlDocumentFragment = this.ConfigXml.CreateDocumentFragment();
                            //xmlDocumentFragment.InnerXml = p.PageContent;

                            //cha.AppendChild(xmlDocumentFragment);

                            //xmlDocumentFragment.SetAttribute("id", "p_" + p.PageId);
                            //xmlDocumentFragment.SetAttribute("type", p.Type);

                            //XmlElement bpa = this.ConfigXml.CreateElement()

                            XmlElement bpa = this.ConfigXml.CreateElement("page");
                            bpa.SetAttribute("id", "p_" + p.PageId);
                            bpa.SetAttribute("type", p.Type);
                            bpa.SetAttribute("title", p.Title);

                            XmlDocument doc = new XmlDocument();
                            try
                            {
                                doc.LoadXml(p.PageContent);
                                string classes = doc.ChildNodes[0].Attributes["class"].Value;
                                if (classes.Contains("hide-page"))
                                {
                                    bpa.SetAttribute("class", "hide-page");
                                }
                            }
                            catch { }

                            // these elements are pretty weird, and I can't edit the outerXML, so I'm parsing the string
                            string tempContent = p.PageContent;
                            int firstTagIndex = tempContent.IndexOf('>');
                            string secondContent = tempContent.Substring(firstTagIndex+1);
                            secondContent = secondContent.Replace("</page>", "");




                            bpa.InnerXml = secondContent;
                            cha.AppendChild(bpa);

                            XmlDocument xml_page = new XmlDocument();
                            try
                            {

                                PageContentItem newPage = new PageContentItem
                                {
                                    Chapter = "c_" + chapterCount,

                                    content = p.PageContent,
                                    
                                    Module = "m_" + moduleCount,
                                    Page = "p_" + p.PageId,
                                    Section = "s_" + sectionCount,
                                    Book = "b_" + book.BookId
                                };

                                this.PageContent.Add(newPage);
                            }
                            catch
                            {

                            }
                        }

                        chapterCount++;
                    }
                    sectionCount++;
                }
                moduleCount++;
            }
        }
        public XmlDocument ConfigXml { get; set; }
        public List<PageContentItem> PageContent { get; set; }
    }


    public class PageContentItem
    {
        public string Book { get; set; }
        public string Module { get; set; }
        public string Section { get; set; }
        public string Chapter { get; set; }
        public string Page { get; set; }
        public string content { get; set; }
    }
}