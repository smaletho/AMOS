using Amos.Models;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;

namespace Amos.Controllers
{
    [Authorize]
    public class BuildController : Controller
    {
        // GET: Build
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListBooks()
        {
            ApplicationDbContext adb = new ApplicationDbContext();
            return View(adb.Books.ToList());
        }

        public ActionResult ListPages()
        {
            return View(new ApplicationDbContext().Pages.Where(x => x.BookId == 0).ToList());
        }

        public ActionResult Create()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            Book book = new Book();
            book.Create(User.Identity.Name);
            book.Name = "New Book";
            book.Published = false;
            book.Version = "1";

            db.Books.Add(book);
            db.SaveChanges();

            Module module = new Module();
            module.BookId = book.BookId;
            module.Name = "New Module";
            module.SortOrder = 10;
            module.Theme = "1";

            db.Modules.Add(module);
            db.SaveChanges();

            Section section = new Section();
            section.ModuleId = module.ModuleId;
            section.Name = "New Section";
            section.SortOrder = 10;

            db.Sections.Add(section);
            db.SaveChanges();

            Chapter chapter = new Chapter();
            chapter.SectionId = section.SectionId;
            chapter.Name = "New Chapter";
            chapter.SortOrder = 10;

            db.Chapters.Add(chapter);
            db.SaveChanges();

            Page page = new Page();
            page.ChapterId = chapter.ChapterId;
            page.BookId = book.BookId;
            page.Create(User.Identity.Name);
            page.SortOrder = 10;
            page.Title = "New Page";
            page.Type = "content";

            db.Pages.Add(page);
            db.SaveChanges();

            return RedirectToAction("Index", "Configure", new { id = book.BookId });
        }


        public ActionResult ImportBook(ImportBookModel model)
        {
            return View(new ImportBookModel());
        }

        public ActionResult DoImportBook(ImportBookModel model)
        {
            model.ResponseList = new List<string>();
            ApplicationDbContext cdb = new ApplicationDbContext();

            try
            {
                HttpPostedFileBase file = Request.Files[0];
                model.ResponseList = Action_Import(file.InputStream, "");
            }
            catch (NotImplementedException e)
            {
                model.ResponseList.Add("Couldn't open file. Exception: " + e.Message);
            }



            return View("ImportBook", model);
        }

        public static List<string> Action_Import(Stream s, string fileName)
        {
            List<string> retLs = new List<string>();
            ApplicationDbContext cdb = new ApplicationDbContext();
            using (ZipArchive zipArchive = new ZipArchive(s))
            {

                Dictionary<string, string> oldToNewPageNumbers = new Dictionary<string, string>();

                // TODO timer

                var config = zipArchive.Entries.Where(x => x.Name == "config.xml").FirstOrDefault();
                if (config == null)
                {
                    retLs.Add("Could not find config.xml. Aborting.");
                    //return RedirectToAction("ImportBook", model);
                    return retLs;
                }
                else
                {
                    retLs.Add("Found config.xml. Parsing in to book...");
                    using (var stream = config.Open())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            try
                            {
                                // turn it in to an XML Doc
                                XmlDocument xmlDoc = new XmlDocument();
                                xmlDoc.LoadXml(reader.ReadToEnd());

                                // loop through the xml doc to build book
                                foreach (XmlElement bookNode in xmlDoc.ChildNodes)
                                {
                                    // this is the book object
                                    Book book = new Book();
                                    book.CreatedBy = bookNode.Attributes["author"].Value;
                                    book.ModifiedBy = bookNode.Attributes["author"].Value;
                                    book.FileName = fileName;

                                    try { book.CreateDate = Convert.ToDateTime(bookNode.Attributes["createdate"].Value); }
                                    catch { book.CreateDate = DateTime.Now; }
                                    try { book.ModifyDate = Convert.ToDateTime(bookNode.Attributes["modifydate"].Value); }
                                    catch { book.ModifyDate = DateTime.Now; }

                                    book.Published = true;
                                    book.Version = bookNode.Attributes["version"].Value;
                                    book.Name = bookNode.Attributes["name"].Value;

                                    cdb.Books.Add(book);
                                    cdb.SaveChanges();
                                    retLs.Add(string.Format("Added book. Name: {0} Id: {1}", book.Name, book.BookId));

                                    int moduleSort = 10;

                                    foreach (XmlElement moduleNode in bookNode.ChildNodes)
                                    {
                                        // these are modules
                                        Module module = new Module();
                                        module.BookId = book.BookId;
                                        module.Name = moduleNode.Attributes["name"].Value;

                                        module.SortOrder = moduleSort;
                                        moduleSort += 10;

                                        module.Theme = moduleNode.Attributes["theme"].Value;
                                        cdb.Modules.Add(module);
                                        cdb.SaveChanges();

                                        int sectionSort = 10;

                                        foreach (XmlElement sectionNode in moduleNode.ChildNodes)
                                        {
                                            // these are sections
                                            Section section = new Section();
                                            section.ModuleId = module.ModuleId;
                                            section.Name = sectionNode.Attributes["name"].Value;

                                            section.SortOrder = sectionSort;
                                            sectionSort += 10;

                                            cdb.Sections.Add(section);
                                            cdb.SaveChanges();

                                            int chapterSort = 10;

                                            foreach (XmlElement chapterNode in sectionNode.ChildNodes)
                                            {
                                                // these are chapters
                                                Chapter chapter = new Chapter();
                                                chapter.Name = chapterNode.Attributes["name"].Value;

                                                chapter.SortOrder = chapterSort;
                                                chapterSort += 10;

                                                chapter.SectionId = section.SectionId;
                                                cdb.Chapters.Add(chapter);
                                                cdb.SaveChanges();

                                                int pageSort = 10;

                                                foreach (XmlElement pageNode in chapterNode.ChildNodes)
                                                {
                                                    // these are pages
                                                    Page page = new Page();
                                                    page.BookId = book.BookId;
                                                    page.ChapterId = chapter.ChapterId;

                                                    page.SortOrder = pageSort;
                                                    pageSort += 10;

                                                    page.Type = pageNode.Attributes["type"].Value;

                                                    page.Create("import");
                                                    try
                                                    {
                                                        page.Title = pageNode.Attributes["title"].Value;
                                                    }
                                                    catch (NullReferenceException)
                                                    {
                                                        page.Title = "New Page";
                                                    }

                                                    page.PageContent = "";

                                                    cdb.Pages.Add(page);
                                                    cdb.SaveChanges();

                                                    oldToNewPageNumbers.Add(pageNode.Attributes["id"].Value, "p_" + page.PageId);
                                                    pageNode.SetAttribute("id", "p_" + page.PageId);

                                                    var ls = pageNode.SelectNodes("text//a");
                                                    var ls2 = pageNode.SelectNodes("image");
                                                    var ls3 = pageNode.SelectNodes("button");

                                                    foreach (XmlElement contentNode in pageNode.SelectNodes("image"))
                                                    {
                                                        // find the image in the archive
                                                        string id = contentNode.Attributes["source"].Value;

                                                        string type = "";
                                                        string matchFileName = "";
                                                        try
                                                        {
                                                            type = contentNode.Attributes["type"].Value;
                                                            matchFileName = id + "." + type;
                                                        }
                                                        catch (NullReferenceException)
                                                        {
                                                            type = "jpg";
                                                            matchFileName = id + ".jpg";
                                                        }


                                                        var innerFile = zipArchive.Entries.Where(x => x.Name == matchFileName).FirstOrDefault();
                                                        if (innerFile != null)
                                                        {
                                                            using (var innerStream = innerFile.Open())
                                                            {
                                                                using (var innerReader = new StreamReader(innerStream))
                                                                {
                                                                    AmosFile f = new AmosFile();

                                                                    MemoryStream target = new MemoryStream();
                                                                    innerReader.BaseStream.CopyTo(target);
                                                                    f.Content = target.ToArray();


                                                                    f.FileName = matchFileName;
                                                                    switch (type)
                                                                    {
                                                                        case "jpg":
                                                                        case "png":
                                                                        case "bmp":
                                                                        case "gif":
                                                                            f.ContentType = "image/" + type;
                                                                            f.FileType = FileType.Photo;
                                                                            break;
                                                                        case "mp4":
                                                                            f.ContentType = "video/" + type;
                                                                            f.FileType = FileType.Video;
                                                                            break;
                                                                    }

                                                                    f.PageId = page.PageId;
                                                                    cdb.AmosFiles.Add(f);
                                                                    cdb.SaveChanges();

                                                                    // update ID in contentNode
                                                                    switch (f.FileType)
                                                                    {
                                                                        case FileType.Photo:
                                                                            contentNode.SetAttribute("source", "i_" + f.FileId);
                                                                            break;
                                                                        case FileType.Video:
                                                                            contentNode.SetAttribute("source", "v_" + f.FileId);
                                                                            break;
                                                                    }

                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            retLs.Add(string.Format("File not found. Page: {0} FileId: {1}", pageNode.GetAttribute("id"), matchFileName));
                                                        }
                                                    }
                                                    foreach (XmlElement contentNode in pageNode.SelectNodes("video"))
                                                    {
                                                        // find the image in the archive
                                                        string id = contentNode.Attributes["source"].Value;

                                                        string type = "";
                                                        string matchFileName = "";
                                                        try
                                                        {
                                                            type = contentNode.Attributes["type"].Value;
                                                            matchFileName = id + "." + type;
                                                        }
                                                        catch (NullReferenceException)
                                                        {
                                                            type = "jpg";
                                                            matchFileName = id + ".jpg";
                                                        }


                                                        var innerFile = zipArchive.Entries.Where(x => x.Name == matchFileName).FirstOrDefault();
                                                        if (innerFile != null)
                                                        {
                                                            using (var innerStream = innerFile.Open())
                                                            {
                                                                using (var innerReader = new StreamReader(innerStream))
                                                                {
                                                                    AmosFile f = new AmosFile();

                                                                    MemoryStream target = new MemoryStream();
                                                                    innerReader.BaseStream.CopyTo(target);
                                                                    f.Content = target.ToArray();


                                                                    f.FileName = matchFileName;
                                                                    switch (type)
                                                                    {
                                                                        case "jpg":
                                                                        case "png":
                                                                        case "bmp":
                                                                        case "gif":
                                                                            f.ContentType = "image/" + type;
                                                                            f.FileType = FileType.Photo;
                                                                            break;
                                                                        case "mp4":
                                                                            f.ContentType = "video/" + type;
                                                                            f.FileType = FileType.Video;
                                                                            break;
                                                                    }

                                                                    f.PageId = page.PageId;
                                                                    cdb.AmosFiles.Add(f);
                                                                    cdb.SaveChanges();

                                                                    // update ID in contentNode
                                                                    switch (f.FileType)
                                                                    {
                                                                        case FileType.Photo:
                                                                            contentNode.SetAttribute("source", "i_" + f.FileId);
                                                                            break;
                                                                        case FileType.Video:
                                                                            contentNode.SetAttribute("source", "v_" + f.FileId);
                                                                            break;
                                                                    }

                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            retLs.Add(string.Format("File not found. Page: {0} FileId: {1}", pageNode.GetAttribute("id"), matchFileName));
                                                        }
                                                    }
                                                    foreach (XmlElement contentNode in pageNode.SelectNodes("text//a"))
                                                    {
                                                        if (contentNode.HasAttribute("class") && contentNode.GetAttribute("class").Contains("dialogLink"))
                                                        {
                                                            // find the image in the archive
                                                            string id = contentNode.Attributes["data-content"].Value;

                                                            string type = "";
                                                            string matchFileName = "";
                                                            try
                                                            {
                                                                type = contentNode.Attributes["data-type"].Value;
                                                                matchFileName = id + "." + type;
                                                            }
                                                            catch (NullReferenceException)
                                                            {
                                                                type = "jpg";
                                                                matchFileName = id + ".jpg";
                                                            }


                                                            var innerFile = zipArchive.Entries.Where(x => x.Name == matchFileName).FirstOrDefault();
                                                            if (innerFile != null)
                                                            {
                                                                using (var innerStream = innerFile.Open())
                                                                {
                                                                    using (var innerReader = new StreamReader(innerStream))
                                                                    {
                                                                        AmosFile f = new AmosFile();

                                                                        MemoryStream target = new MemoryStream();
                                                                        innerReader.BaseStream.CopyTo(target);
                                                                        f.Content = target.ToArray();


                                                                        f.FileName = matchFileName;
                                                                        switch (type)
                                                                        {
                                                                            case "jpg":
                                                                            case "png":
                                                                            case "bmp":
                                                                            case "gif":
                                                                                f.ContentType = "image/" + type;
                                                                                f.FileType = FileType.Photo;
                                                                                break;
                                                                            case "mp4":
                                                                                f.ContentType = "video/" + type;
                                                                                f.FileType = FileType.Video;
                                                                                break;
                                                                        }

                                                                        f.PageId = page.PageId;
                                                                        cdb.AmosFiles.Add(f);
                                                                        cdb.SaveChanges();

                                                                        // update ID in contentNode
                                                                        switch (f.FileType)
                                                                        {
                                                                            case FileType.Photo:
                                                                                contentNode.SetAttribute("data-content", "i_" + f.FileId);
                                                                                break;
                                                                            case FileType.Video:
                                                                                contentNode.SetAttribute("data-content", "v_" + f.FileId);
                                                                                break;
                                                                        }

                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                retLs.Add(string.Format("File not found. Page: {0} FileId: {1}", pageNode.GetAttribute("id"), matchFileName));
                                                            }
                                                        }
                                                    }
                                                    foreach (XmlElement contentNode in pageNode.SelectNodes("button"))
                                                    {
                                                        if (contentNode.HasAttribute("class") && contentNode.GetAttribute("class").Contains("dialogLink"))
                                                        {
                                                            // find the image in the archive
                                                            string id = contentNode.Attributes["data-content"].Value;

                                                            string type = "";
                                                            string matchFileName = "";
                                                            try
                                                            {
                                                                type = contentNode.Attributes["data-type"].Value;
                                                                matchFileName = id + "." + type;
                                                            }
                                                            catch (NullReferenceException)
                                                            {
                                                                type = "jpg";
                                                                matchFileName = id + ".jpg";
                                                            }


                                                            var innerFile = zipArchive.Entries.Where(x => x.Name == matchFileName).FirstOrDefault();
                                                            if (innerFile != null)
                                                            {
                                                                using (var innerStream = innerFile.Open())
                                                                {
                                                                    using (var innerReader = new StreamReader(innerStream))
                                                                    {
                                                                        AmosFile f = new AmosFile();

                                                                        MemoryStream target = new MemoryStream();
                                                                        innerReader.BaseStream.CopyTo(target);
                                                                        f.Content = target.ToArray();


                                                                        f.FileName = matchFileName;
                                                                        switch (type)
                                                                        {
                                                                            case "jpg":
                                                                            case "png":
                                                                            case "bmp":
                                                                            case "gif":
                                                                                f.ContentType = "image/" + type;
                                                                                f.FileType = FileType.Photo;
                                                                                break;
                                                                            case "mp4":
                                                                                f.ContentType = "video/" + type;
                                                                                f.FileType = FileType.Video;
                                                                                break;
                                                                        }

                                                                        f.PageId = page.PageId;
                                                                        cdb.AmosFiles.Add(f);
                                                                        cdb.SaveChanges();

                                                                        // update ID in contentNode
                                                                        switch (f.FileType)
                                                                        {
                                                                            case FileType.Photo:
                                                                                contentNode.SetAttribute("data-content", "i_" + f.FileId);
                                                                                break;
                                                                            case FileType.Video:
                                                                                contentNode.SetAttribute("data-content", "v_" + f.FileId);
                                                                                break;
                                                                        }

                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                retLs.Add(string.Format("File not found. Page: {0} FileId: {1}", pageNode.GetAttribute("id"), matchFileName));
                                                            }
                                                        }
                                                    }

                                                    //foreach (XmlElement contentNode in pageNode.ChildNodes)
                                                    //{
                                                    //    if (contentNode.Name.ToLower() == "image" || contentNode.Name.ToLower() == "video")
                                                    //    {
                                                    //        // find the image in the archive
                                                    //        string id = contentNode.Attributes["source"].Value;

                                                    //        string type = "";
                                                    //        string matchFileName = "";
                                                    //        try
                                                    //        {
                                                    //            type = contentNode.Attributes["type"].Value;
                                                    //            matchFileName = id + "." + type;
                                                    //        }
                                                    //        catch (NullReferenceException)
                                                    //        {
                                                    //            type = "jpg";
                                                    //            matchFileName = id + ".jpg";
                                                    //        }


                                                    //        var innerFile = zipArchive.Entries.Where(x => x.Name == matchFileName).FirstOrDefault();
                                                    //        if (innerFile != null)
                                                    //        {
                                                    //            using (var innerStream = innerFile.Open())
                                                    //            {
                                                    //                using (var innerReader = new StreamReader(innerStream))
                                                    //                {
                                                    //                    AmosFile f = new AmosFile();

                                                    //                    MemoryStream target = new MemoryStream();
                                                    //                    innerReader.BaseStream.CopyTo(target);
                                                    //                    f.Content = target.ToArray();


                                                    //                    f.FileName = matchFileName;
                                                    //                    switch (type)
                                                    //                    {
                                                    //                        case "jpg":
                                                    //                        case "png":
                                                    //                        case "bmp":
                                                    //                        case "gif":
                                                    //                            f.ContentType = "image/" + type;
                                                    //                            f.FileType = FileType.Photo;
                                                    //                            break;
                                                    //                        case "mp4":
                                                    //                            f.ContentType = "video/" + type;
                                                    //                            f.FileType = FileType.Video;
                                                    //                            break;
                                                    //                    }

                                                    //                    f.PageId = page.PageId;
                                                    //                    cdb.AmosFiles.Add(f);
                                                    //                    cdb.SaveChanges();

                                                    //                    // update ID in contentNode
                                                    //                    switch (f.FileType)
                                                    //                    {
                                                    //                        case FileType.Photo:
                                                    //                            contentNode.SetAttribute("source", "i_" + f.FileId);
                                                    //                            break;
                                                    //                        case FileType.Video:
                                                    //                            contentNode.SetAttribute("source", "v_" + f.FileId);
                                                    //                            break;
                                                    //                    }

                                                    //                }
                                                    //            }
                                                    //        }
                                                    //        else
                                                    //        {
                                                    //            retLs.Add(string.Format("File not found. Page: {0} FileId: {1}", pageNode.Attributes["id"].Value, matchFileName));
                                                    //        }
                                                    //    }
                                                    //    if (contentNode.HasAttribute("class") && contentNode.GetAttribute("class").Contains("dialogLink"))
                                                    //    {
                                                    //        // find the image in the archive
                                                    //        string id = contentNode.Attributes["data-content"].Value;

                                                    //        string type = "";
                                                    //        string matchFileName = "";
                                                    //        try
                                                    //        {
                                                    //            type = contentNode.Attributes["data-type"].Value;
                                                    //            matchFileName = id + "." + type;
                                                    //        }
                                                    //        catch (NullReferenceException)
                                                    //        {
                                                    //            type = "jpg";
                                                    //            matchFileName = id + ".jpg";
                                                    //        }


                                                    //        var innerFile = zipArchive.Entries.Where(x => x.Name == matchFileName).FirstOrDefault();
                                                    //        if (innerFile != null)
                                                    //        {
                                                    //            using (var innerStream = innerFile.Open())
                                                    //            {
                                                    //                using (var innerReader = new StreamReader(innerStream))
                                                    //                {
                                                    //                    AmosFile f = new AmosFile();

                                                    //                    MemoryStream target = new MemoryStream();
                                                    //                    innerReader.BaseStream.CopyTo(target);
                                                    //                    f.Content = target.ToArray();


                                                    //                    f.FileName = matchFileName;
                                                    //                    switch (type)
                                                    //                    {
                                                    //                        case "jpg":
                                                    //                        case "png":
                                                    //                        case "bmp":
                                                    //                        case "gif":
                                                    //                            f.ContentType = "image/" + type;
                                                    //                            f.FileType = FileType.Photo;
                                                    //                            break;
                                                    //                        case "mp4":
                                                    //                            f.ContentType = "video/" + type;
                                                    //                            f.FileType = FileType.Video;
                                                    //                            break;
                                                    //                    }

                                                    //                    f.PageId = page.PageId;
                                                    //                    cdb.AmosFiles.Add(f);
                                                    //                    cdb.SaveChanges();

                                                    //                    // update ID in contentNode
                                                    //                    switch (f.FileType)
                                                    //                    {
                                                    //                        case FileType.Photo:
                                                    //                            contentNode.SetAttribute("data-content", "i_" + f.FileId);
                                                    //                            break;
                                                    //                        case FileType.Video:
                                                    //                            contentNode.SetAttribute("data-content", "v_" + f.FileId);
                                                    //                            break;
                                                    //                    }

                                                    //                }
                                                    //            }
                                                    //        }
                                                    //        else
                                                    //        {
                                                    //            retLs.Add(string.Format("File not found. Page: {0} FileId: {1}", pageNode.Attributes["id"].Value, matchFileName));
                                                    //        }
                                                    //    }
                                                    //}

                                                }
                                            }
                                        }
                                    }

                                }

                                // go back through all the page nodes, and update the other IDs (buttons, links, etc)
                                ///book/module/section/chapter/
                                foreach (XmlElement p in xmlDoc.SelectNodes("//page"))
                                {
                                    // update the buttons
                                    foreach (XmlElement button in p.SelectNodes("./button"))
                                    {
                                        try
                                        {
                                            if (button.HasAttribute("class") && button.GetAttribute("class").Contains("popupPage"))
                                            {
                                                // change the id from the old one to the matching new one
                                                //button.SetAttribute("data-page", oldToNewPageNumbers[button.Attributes["data-page"].Value]);
                                                if (oldToNewPageNumbers.TryGetValue("p_" + button.GetAttribute("data-page"), out string value))
                                                {
                                                    button.SetAttribute("data-page", value.Split('_')[1]);
                                                }
                                            }
                                            else 
                                            {
                                                // change the id from the old one to the matching new one
                                                //button.SetAttribute("id", oldToNewPageNumbers[button.Attributes["id"].Value]);
                                                if (oldToNewPageNumbers.TryGetValue(button.Attributes["id"].Value, out string value))
                                                {
                                                    button.SetAttribute("id", value);
                                                }
                                            }
                                        }
                                        catch (NullReferenceException) { }
                                    }

                                    // update the anchors
                                    foreach (XmlElement a in p.SelectNodes("./text//a"))
                                    {

                                        try
                                        {
                                            if (a.HasAttribute("class") && a.GetAttribute("class") == "navigateTo")
                                            {
                                                // change the id from the old one to the matching new one
                                                //a.SetAttribute("data-id", oldToNewPageNumbers[a.Attributes["data-id"].Value]);
                                                if (oldToNewPageNumbers.TryGetValue(a.GetAttribute("data-id"), out string value))
                                                {
                                                    a.SetAttribute("data-id", value);
                                                }
                                            }
                                            if(a.HasAttribute("class") && a.GetAttribute("class") == "popupPage")
                                            {
                                                // change the id from the old one to the matching new one
                                                //a.SetAttribute("data-page", oldToNewPageNumbers[a.Attributes["data-page"].Value]);
                                                if (oldToNewPageNumbers.TryGetValue("p_" + a.GetAttribute("data-page"), out string value))
                                                {
                                                    a.SetAttribute("data-page", value.Split('_')[1]);
                                                }
                                            }
                                        }
                                        catch (NullReferenceException) { }
                                    }

                                    // get the pageId from the node
                                    string pageId = p.Attributes["id"].Value;
                                    int newPageId = Convert.ToInt32(pageId.Split('_')[1]);

                                    var pageLookup = cdb.Pages.Where(x => x.PageId == newPageId).FirstOrDefault();
                                    if (pageLookup != null)
                                    {
                                        pageLookup.PageContent = p.OuterXml;
                                    }
                                }

                                cdb.SaveChanges();

                            }
                            catch (XmlException e)
                            {
                                retLs.Add("Bad xml in config.xml. Aborting. Exception: " + e.Message);
                            }
                            catch (ArgumentException e)
                            {
                                retLs.Add("Attribute exception. Message: " + e.Message);
                            }
                        }
                    }
                }



            }

            return retLs;
        }

        public ActionResult Delete(int id)
        {
            ApplicationDbContext adb = new ApplicationDbContext();

            var book = adb.Books.Where(x => x.BookId == id).FirstOrDefault();

            ResetBook(book.BookId);

            adb.Books.Remove(book);
            adb.SaveChanges();

            return RedirectToAction("ListBooks");
        }

        public ActionResult Export(int id)
        {

            CreateFolderIfNotThere();// delete all the old stuff
            DeleteOldFiles();


            ApplicationDbContext cdb = new ApplicationDbContext();
            BookModel bookModel = new BookModel(id);

            var pages = cdb.Pages.Where(x => x.BookId == id).ToList();

            var theBook = cdb.Books.Where(x => x.BookId == id).FirstOrDefault();

            string bookName = theBook.Name;
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                bookName = bookName.Replace(c, '-');
            }

            string dt = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss");
            string fullFileName = Server.MapPath("~/ZipDump/export_" + bookName + "_" + dt + ".zip");
            string fileName = "export_" + bookName + "_" + dt + ".zip";

            Dictionary<string, string> fileNameToExtension = new Dictionary<string, string>();

            // write each of these to a file
            using (ZipFile zip = new ZipFile())
            {
                foreach (var p in pages)
                {
                    // find all associated files with that page
                    var files = cdb.AmosFiles.Where(x => x.PageId == p.PageId).ToList();
                    int count = 1;
                    foreach (var f in files)
                    {
                        if (f.Content != null)
                        {
                            string fName = Server.MapPath("~/ZipDump/f_" + f.FileId.ToString() + "_" + dt);
                            string newFileName = "/Content/";
                            switch (f.FileType)
                            {
                                case FileType.Photo:
                                    switch (f.ContentType)
                                    {
                                        case "image/jpg":
                                        case "jpg":
                                        case "jpeg":
                                        case "image/jpeg":
                                            fName += ".jpg";
                                            newFileName += "i_" + f.FileId.ToString() + ".jpg";
                                            fileNameToExtension.Add("i_" + f.FileId.ToString(), "jpg");
                                            break;
                                        case "image/png":
                                            fName += ".png";
                                            newFileName += "i_" + f.FileId.ToString() + ".png";
                                            fileNameToExtension.Add("i_" + f.FileId.ToString(), "png");
                                            break;
                                        case "image/gif":
                                            fName += ".gif";
                                            newFileName += "i_" + f.FileId.ToString() + ".gif";
                                            fileNameToExtension.Add("i_" + f.FileId.ToString(), "gif");
                                            break;
                                        case "image/bmp":
                                            fName += ".bmp";
                                            newFileName += "i_" + f.FileId.ToString() + ".bmp";
                                            fileNameToExtension.Add("i_" + f.FileId.ToString(), "bmp");
                                            break;
                                    }
                                    break;
                                case FileType.Video:
                                    switch (f.ContentType)
                                    {
                                        case "mp4":
                                        case "video/mp4":
                                            fName += ".mp4";
                                            newFileName += "v_" + f.FileId.ToString() + ".mp4";
                                            fileNameToExtension.Add("v_" + f.FileId.ToString(), "mp4");
                                            break;
                                    }
                                    break;
                            }

                            // write the file to the stream
                            using (var tw = new StreamWriter(fName, true))
                            {
                                tw.BaseStream.Write(f.Content, 0, f.Content.Length);
                                zip.AddFile(fName).FileName = newFileName;
                            }
                            count++;
                        }
                    }
                }

                // write the XML to a file too
                string configFileName = Server.MapPath("~/ZipDump/config.xml");
                using (var tw = new StreamWriter(configFileName, true))
                {
                    // Images need to have the right file type associated with them. It's not always explicitly specified
                    XmlNodeList imageNodeList = bookModel.ConfigXml.SelectNodes("//image");
                    foreach (XmlElement node in imageNodeList)
                    {
                        if (node.Attributes["type"] == null)
                        {
                            try
                            {
                                string fileExtension = fileNameToExtension[node.Attributes["source"].Value];
                                node.SetAttribute("type", fileExtension);
                            }
                            catch
                            {

                            }

                        }
                    }
                    //XmlNodeList videoNodeList = bookModel.ConfigXml.SelectNodes("//video");


                    tw.Write(bookModel.ConfigXml.OuterXml);
                    zip.AddFile(configFileName).FileName = "config.xml";
                }

                zip.Save(fullFileName);

                return File(fullFileName, "application/zip", fileName);
            }
        }

        public void CreateFolderIfNotThere()
        {
            if (!Directory.Exists(Server.MapPath("~/ZipDump")))
                Directory.CreateDirectory(Server.MapPath("~/ZipDump"));
        }



        public ActionResult Download(int id)
        {
            CreateFolderIfNotThere();
            // delete all the old stuff
            DeleteOldFiles();



            // grab all the stuff
            ApplicationDbContext cdb = new ApplicationDbContext();
            BookModel model = new BookModel(id);
            var theBook = cdb.Books.Where(x => x.BookId == id).FirstOrDefault();


            // loop through model.PageContent, and separate out all the XML pieces
            List<string> contentLs = new List<string>();
            foreach (var item in model.PageContent)
            {
                contentLs.Add(item.content);
                item.content = "";
            }


            // change the offline load to read two different files
            string dt = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss");

            // make the name of the book safe for filenames
            string bookName = theBook.Name;
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                bookName = bookName.Replace(c, '-');
            }


            string fullFileName = Server.MapPath("~/ZipDump/" + bookName + "_" + dt + ".zip");
            string fileName = bookName + "_" + dt + ".zip";

            string configFile1 = Server.MapPath("~/ZipDump/1config_" + dt + ".js");
            string configFile2 = Server.MapPath("~/ZipDump/2config_" + dt + ".js");
            string homeFile = Server.MapPath("~/ZipDump/index.html");

            using (ZipFile zip = new ZipFile())
            {
                zip.AddDirectory(Server.MapPath("~/Content/"), "/Content/");
                zip.AddDirectory(Server.MapPath("~/fonts/"), "/fonts/");

                // Add Index.html
                //  I need to strip down this file, and include the variables "config1.js" and config2.js"

                using (var tw = new StreamWriter(homeFile, true))
                {
                    string text = System.IO.File.ReadAllText(Server.MapPath("~/Views/View/Index.cshtml"));

                    text = text.Replace("~/", "./");

                    string scriptText = "<script src=\"./Content/js/offline.js\"></script>";
                    scriptText += "<script src=\"./Content/js/config1.js\"></script>";
                    scriptText += "<script src=\"./Content/js/config2.js\"></script>";
                    text = text.Replace("<script src=\"./Content/js/offline.js\"></script>", scriptText);

                    tw.Write(text);
                    zip.AddFile(homeFile).FileName = "index.html";

                }

                // first get all the pages in the book
                //var bookPages = cdb.BookPages.Where(x => x.BookId == theBook.BookId).Select(x => x.PageId).ToList();
                var pages = cdb.Pages.Where(x => x.BookId == theBook.BookId).ToList();
                Dictionary<string, string> fileNameToExtension = new Dictionary<string, string>();
                foreach (var p in pages)
                {
                    // find all associated files with that page
                    var files = cdb.AmosFiles.Where(x => x.PageId == p.PageId).ToList();
                    int count = 1;
                    foreach (var f in files)
                    {
                        if (f.Content != null)
                        {
                            string fName = Server.MapPath("~/ZipDump/f_" + f.FileId.ToString() + "_" + dt);
                            string newFileName = "/Content/";
                            switch (f.FileType)
                            {
                                case FileType.Photo:
                                    switch (f.ContentType)
                                    {
                                        case "image/jpg":
                                        case "image/jpeg":
                                            fName += ".jpg";
                                            newFileName += "images/i_" + f.FileId.ToString() + ".jpg";
                                            fileNameToExtension.Add("i_" + f.FileId.ToString(), "jpg");
                                            break;
                                        case "image/png":
                                            fName += ".png";
                                            newFileName += "images/i_" + f.FileId.ToString() + ".png";
                                            fileNameToExtension.Add("i_" + f.FileId.ToString(), "png");
                                            break;
                                        case "image/gif":
                                            fName += ".gif";
                                            newFileName += "images/i_" + f.FileId.ToString() + ".gif";
                                            fileNameToExtension.Add("i_" + f.FileId.ToString(), "gif");
                                            break;
                                        case "image/bmp":
                                            fName += ".bmp";
                                            newFileName += "images/i_" + f.FileId.ToString() + ".bmp";
                                            fileNameToExtension.Add("i_" + f.FileId.ToString(), "bmp");
                                            break;
                                    }
                                    break;
                                case FileType.Video:
                                    switch (f.ContentType)
                                    {
                                        case "video/mp4":
                                            fName += ".mp4";
                                            newFileName += "images/v_" + f.FileId.ToString() + ".mp4";
                                            fileNameToExtension.Add("i_" + f.FileId.ToString(), "mp4");
                                            break;
                                    }
                                    break;
                            }

                            // write the file to the stream
                            using (var tw = new StreamWriter(fName, true))
                            {
                                tw.BaseStream.Write(f.Content, 0, f.Content.Length);

                                zip.AddFile(fName).FileName = newFileName;
                            }
                            count++;
                        }
                    }
                }

                using (var tw = new StreamWriter(configFile1, true))
                {
                    // create the config file
                    tw.Write("var offline_ConfigXml = $.parseXML(`");
                    tw.Write(model.ConfigXml.OuterXml);
                    tw.Write("`);");

                    zip.AddFile(configFile1).FileName = "/Content/js/config1.js";
                }

                using (var tw = new StreamWriter(configFile2, true))
                {
                    // create the config file
                    tw.Write("var offline_PageContents = `");

                    var jsonSerialiser = new JavaScriptSerializer();
                    var json = jsonSerialiser.Serialize(model.PageContent);

                    tw.Write(json);
                    tw.Write("`;");

                    tw.WriteLine(Environment.NewLine);
                    tw.WriteLine("var offline_PageGuts = [];");

                    tw.WriteLine(Environment.NewLine);
                    tw.WriteLine(Environment.NewLine);
                    tw.WriteLine("function loadGuts() {");

                    foreach (var page in contentLs)
                    {
                        // update the "type" attribute for images
                        try
                        {
                            XmlDocument x = new XmlDocument();
                            x.LoadXml(page);
                            XmlNodeList imageNodeList = x.SelectNodes("//image");
                            foreach (XmlElement node in imageNodeList)
                            {
                                if (node.Attributes["type"] == null)
                                {
                                    try
                                    {
                                        string fileExtension = fileNameToExtension[node.Attributes["source"].Value];
                                        node.SetAttribute("type", fileExtension);
                                    }
                                    catch { }
                                }
                            }
                        }
                        catch { }
                        tw.WriteLine("offline_PageGuts.push(`" + page + "`);");
                    }

                    tw.WriteLine("}");

                    zip.AddFile(configFile2).FileName = "/Content/js/config2.js";
                }

                // map the images too





                zip.Save(fullFileName);

                return File(fullFileName, "application/zip", fileName);
            }
        }
        public void DeleteOldFiles()
        {
            string sourceDir = Server.MapPath("~/ZipDump/");

            try
            {
                DirectoryInfo di = new DirectoryInfo(sourceDir);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }

                //string[] mp4List = Directory.GetFiles(sourceDir, "*.mp4");
                //string[] pngList = Directory.GetFiles(sourceDir, "*.png");
                //string[] jpgList = Directory.GetFiles(sourceDir, "*.jpg");
                //string[] zipList = Directory.GetFiles(sourceDir, "*.zip");
                //string[] jsList = Directory.GetFiles(sourceDir, "*.js");
                //string[] htmlList = Directory.GetFiles(sourceDir, "*.html");

                //// Delete source files
                //foreach (string f in zipList)
                //{
                //    System.IO.File.Delete(f);
                //}
                //foreach (string f in jsList)
                //{
                //    System.IO.File.Delete(f);
                //}
                //foreach (string f in htmlList)
                //{
                //    System.IO.File.Delete(f);
                //}
                //foreach (string f in jpgList)
                //{
                //    System.IO.File.Delete(f);
                //}
                //foreach (string f in pngList)
                //{
                //    System.IO.File.Delete(f);
                //}
                //foreach (string f in mp4List)
                //{
                //    System.IO.File.Delete(f);
                //}
            }
            catch (DirectoryNotFoundException dirNotFound)
            {
                Console.WriteLine(dirNotFound.Message);
            }
        }



        public ActionResult DeleteFile(int id)
        {
            int pageId = 0;

            ApplicationDbContext cdb = new ApplicationDbContext();
            var file = cdb.AmosFiles.Where(x => x.FileId == id).FirstOrDefault();
            pageId = file.PageId;
            cdb.AmosFiles.Remove(file);
            cdb.SaveChanges();

            return RedirectToAction("ViewAssets", new { id = pageId });
        }

        public ActionResult Edit(int id)
        {
            return View(new PageListModel(id));
        }

        public ActionResult ViewPage(int id)
        {

            Session["LastPageIdVisited"] = id;
            PageViewModel model = new PageViewModel();
            ApplicationDbContext cdb = new ApplicationDbContext();
            if (id == 0)
            {
                model.PageId = 0;
                model.PageName = "New Page";
                model.XmlContent = "<page type=\"content\"></page>";
                model.BookId = 0;
            }
            else
            {
                var page = cdb.Pages.Where(x => x.PageId == id).FirstOrDefault();

                model.PageId = id;
                model.XmlContent = page.PageContent;
                model.PageName = page.Title;
                model.BookId = page.BookId;

                if (IsBookPublished(page.BookId)) return RedirectToAction("NoAccess", new { id = 1 });
            }


            return View(model);
        }

        [HttpPost]
        public ActionResult GetPage(int id)
        {
            // Get page content by ID
            ApplicationDbContext cdb = new ApplicationDbContext();
            return Content(cdb.Pages.Where(x => x.PageId == id).Select(x => x.PageContent).FirstOrDefault());
        }

        [HttpPost]
        public ActionResult UploadAsset(AssetModel model)
        {
            ApplicationDbContext cdb = new ApplicationDbContext();
            // if file is there...
            AmosFile f = new AmosFile();

            // Convert the httppostedfilebase to a byte[]
            MemoryStream target = new MemoryStream();
            model.UploadedFile.InputStream.CopyTo(target);
            f.Content = target.ToArray();

            f.ContentType = model.UploadedFile.ContentType;
            f.FileName = model.UploadedFile.FileName;

            // check if comatable/etc
            //  use ContentType ("image/jpeg" etc)
            if (f.ContentType.Contains("image"))
                f.FileType = FileType.Photo;
            else if (f.ContentType.Contains("video"))
                f.FileType = FileType.Video;

            f.PageId = model.PageId;

            cdb.AmosFiles.Add(f);
            cdb.SaveChanges();
            return RedirectToAction("ViewAssets", new { id = model.PageId });
        }

        public ActionResult ViewEditor(int id)
        {
            

            Session["LastPageIdVisited"] = id;
            ApplicationDbContext cdb = new ApplicationDbContext();
            if (id == 0)
            {
                Page page = new Page();
                page.Create(User.Identity.Name);
                page.Title = "New Page";
                page.PageContent = "<page type=\"content\"></page>";
                page.Type = "content";
                page.SortOrder = 10;
                cdb.Pages.Add(page);
                cdb.SaveChanges();

                return RedirectToAction("ViewEditor", new { id = page.PageId });
            }
            else
            {
                PageViewModel model = new PageViewModel
                {
                    PageId = id,
                };

                var page = cdb.Pages.Where(x => x.PageId == id).FirstOrDefault();
                if (page != null)
                {
                    model.PageName = page.Title;
                    model.PageType = page.Type;
                    string oldContent = page.PageContent;
                    model.XmlContent = oldContent.Replace("href=\"javascript:void(0)\"", "href=\"#\"");
                    model.BookId = page.BookId;

                    XmlDocument doc = new XmlDocument();
                    try
                    {
                        doc.LoadXml(model.XmlContent);
                    }
                    catch (XmlException)
                    {
                        //sb.AppendLine("ERROR: Unable to parse the XML. Aborting. Exception: " + e.Message);
                        //return Content(sb.ToString());
                    }

                    StringBuilder sb = new StringBuilder();
                    var settings = new XmlWriterSettings();
                    settings.OmitXmlDeclaration = true;
                    settings.Indent = true;
                    settings.NewLineOnAttributes = false;
                    settings.NewLineChars = "\r\n\r\n";

                    using (var xmlWriter = XmlWriter.Create(sb, settings))
                    {
                        doc.Save(xmlWriter);
                    }

                    model.XmlContent = sb.ToString();
                }
                else
                {
                    model.PageName = "New Page";
                    model.XmlContent = "<page type=\"content\"></page>";
                    model.BookId = 0;
                }

                if (IsBookPublished(model.BookId)) return RedirectToAction("NoAccess", new { id = 1 });

                return View(model);
            }
        }

        public ActionResult ViewAssets(int id)
        {
            Session["LastPageIdVisited"] = id;
            var model = new AssetModel(id);

            ApplicationDbContext db = new ApplicationDbContext();

            var bookId = db.Pages.Find(model.PageId).BookId;
            if (IsBookPublished(bookId)) return RedirectToAction("NoAccess", new { id = 1 });

            return View("ViewAssets", model);
        }

        [HttpPost]
        public ActionResult SavePage(string xml,
            List<string> images,
            int pageId,
            string name,
            string type)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Beginning analysis of page...");
            bool foundErrors = false;

            ApplicationDbContext cdb = new ApplicationDbContext();

            XmlDocument doc = new XmlDocument();
            try
            {
                doc.LoadXml(xml);
            }
            catch (XmlException e)
            {
                sb.AppendLine("ERROR: Unable to parse the XML. Aborting. Exception: " + e.Message);
                return Content(sb.ToString());
            }

            var page = cdb.Pages.Where(x => x.PageId == pageId).FirstOrDefault();
            if (page == null)
            {
                sb.AppendLine("This looks like a new page. Creating one...");
                page = new Page(User.Identity.Name);
                cdb.Pages.Add(page);
            }
            else
            {
                sb.AppendLine("Found an existing page...");
                page.Modify(User.Identity.Name);
            }

            page.Title = name;

            page.Type = type;


            if (page.Type == "quiz")
            {
                sb.AppendLine("This appears to be a QUIZ page. Checking for proper quiz elements...");

                // check for answer to question
                string givenAnswer = "";
                var questionNode = doc.SelectSingleNode("//text[@class='quiz-question']");
                if (questionNode == null)
                {
                    sb.AppendLine("ERROR: Found quiz page, did not find required \"quiz-question\" element.");
                    foundErrors = true;
                }
                else
                {
                    sb.AppendLine("Found question node, now checking for a valid answer...");
                    try
                    {
                        givenAnswer = questionNode.Attributes["answer"].Value;
                        sb.AppendLine("Quiz question appears to have an answer. Validating...");
                    }
                    catch (ArgumentException)
                    {
                        sb.AppendLine("ERROR: Found quiz page, found \"quiz-question\" element, but did not find \"answer\" attribute on \"quiz-question\" element.");
                        foundErrors = true;
                    }

                    // now find all the question inputs
                    var inputNodes = doc.SelectNodes("//input[@name='quiz']");
                    if (inputNodes == null)
                    {
                        sb.AppendLine("ERROR: Found quiz page, didn't find any input[name=quiz] elements.");
                        foundErrors = true;
                    }
                    else
                    {
                        sb.AppendLine("Found quiz input nodes. Checking to see if the answers are valid...");
                        // make sure there are at least 2 options, and that 1 is the correct answer
                        if (inputNodes.Count <= 1)
                        {
                            sb.AppendLine("ERROR: Found quiz page, but there was only one answer found.");
                            foundErrors = true;
                        }
                        else
                        {
                            sb.AppendLine("Found more than one quiz input, searching for matching answer...");
                            bool foundCorrectAnswer = false;
                            foreach (XmlElement element in inputNodes)
                            {
                                // does it have a value?
                                try
                                {
                                    var foundAnswer = element.Attributes["value"].Value;
                                    if (foundAnswer == givenAnswer)
                                    {
                                        foundCorrectAnswer = true;
                                        sb.AppendLine("Found an answer matching the given question!");
                                    }
                                }
                                catch (ArgumentException)
                                {
                                    sb.AppendLine("ERROR: Found a quiz input, but couldn't find the value.\n\n" + element.OuterXml);
                                    foundErrors = true;
                                }
                            }

                            if (!foundCorrectAnswer)
                            {
                                sb.AppendLine("ERROR: Processing quiz page, but did not find an input matching the correct answer: " + givenAnswer);
                                foundErrors = true;
                            }
                        }
                    }
                }
                // does it have .post-quiz element?
                var postQuizNode = doc.SelectSingleNode("//text[@class='post-quiz']");
                if (postQuizNode == null)
                {
                    sb.AppendLine("ERROR: Coun't find a \"post-quiz\" element on the quiz page.");
                    foundErrors = true;
                }
                else
                {
                    sb.AppendLine("Found \"post-quiz\" elements to pop up after the quiz.");
                }
            }
            if (page.Type == "survey")
            {
                sb.AppendLine("This appears to be a SURVEY page. Checking for proper survey elements...");

                // check for .survey-question and 8 inputs (name=survey)
                var surveyQuestionNode = doc.SelectNodes("//input[@class='survey-question']");
                if (surveyQuestionNode == null)
                {
                    sb.AppendLine("ERROR: Found survey page, couldn't find a .survey-question object.");
                    foundErrors = true;
                }
                else
                {
                    sb.AppendLine("Found survey page, and found .survey-question object.");
                    // check for 8 inputs
                    var inputNodes = doc.SelectNodes("//input[@name='survey']");
                    if (inputNodes == null)
                    {
                        sb.AppendLine("ERROR: Found survey page, didn't find any radio button inputs.");
                        foundErrors = true;
                    }
                    else
                    {
                        sb.AppendLine("Found radio button inputs, counting them...");
                        if (inputNodes.Count != 5)
                        {
                            sb.AppendFormat("ERROR: Found {0} survey response elements, when there should be 8.", inputNodes.Count.ToString());
                            foundErrors = true;
                        }
                        else
                        {
                            sb.AppendLine("Found 5 radio button inputs.");
                            // check for comments box
                            var commentsBox = doc.SelectNodes("//textarea[@id='survey-comment']");
                            if (commentsBox == null)
                            {
                                sb.AppendLine("ERROR: Found survey page, but found no comments box.");
                                foundErrors = true;
                            }
                            else if (commentsBox.Count > 1)
                            {
                                sb.AppendLine("ERROR: Found more than one comments box on the page.");
                                foundErrors = true;
                            }
                            else sb.AppendLine("Found a survey comments box.");
                        }
                    }
                }
            }
            if (page.Title == "content")
            {
                // make sure it has no survey or quiz elements
                if (doc.SelectNodes("//input[@name='quiz']").Count > 0)
                {
                    sb.AppendLine("This page is tagged as \"content\", but a quiz input element was discovered.");
                    foundErrors = true;
                }
                if (doc.SelectNodes("//text[@class='quiz-question']").Count > 0)
                {
                    sb.AppendLine("This page is tagged as \"content\", but a quiz question element was discovered.");
                    foundErrors = true;
                }
                if (doc.SelectNodes("//text[@class='post-quiz']").Count > 0)
                {
                    sb.AppendLine("This page is tagged as \"content\", but a post-quiz element was discovered.");
                    foundErrors = true;
                }
                if (doc.SelectNodes("//input[@class='survey-question']").Count > 0)
                {
                    sb.AppendLine("This page is tagged as \"content\", but a survey-question element was discovered.");
                    foundErrors = true;
                }
                if (doc.SelectNodes("//input[@name='survey']").Count > 0)
                {
                    sb.AppendLine("This page is tagged as \"content\", but a survey input element was discovered.");
                    foundErrors = true;
                }
                if (doc.SelectNodes("///textarea[@id='survey-comment']").Count > 0)
                {
                    sb.AppendLine("This page is tagged as \"content\", but a survey-comment element was discovered.");
                    foundErrors = true;
                }

            }

            // TODO finish implementing validation for other inputs
            if (!foundErrors)
            {
                // everything is good so far, do some updating
                page.PageContent = "";
                cdb.SaveChanges();

                // now we'll iterate through the other nodes to update them.
                // these are page elements
                foreach (XmlElement node in doc.ChildNodes)
                {
                    // replace page id
                    string newPageId = "p_" + page.PageId.ToString();
                    node.SetAttribute("id", newPageId);

                    // process the image nodes further
                    //  update the page ids so they're all associated with that page
                    foreach (XmlElement child in node.ChildNodes)
                    {
                        if (child.Name.ToLower() == "text")
                        {
                            // needs more checking for inner content
                            foreach (var el in child.ChildNodes)
                            {
                                try
                                {
                                    XmlElement e = (XmlElement)el;
                                    if (e.Name.ToLower() == "a")
                                    {
                                        e.SetAttribute("href", "javascript:void(0)");
                                    }
                                }
                                catch { }
                            }
                        }
                        if (child.Name.ToLower() == "image")
                        {
                            // check for image source
                            // check for "type"

                            string fileId = child.Attributes["source"].Value;
                            try
                            {
                                int id = Convert.ToInt32(fileId.Split('_')[1]);
                                var dbImage = cdb.AmosFiles.Where(x => x.FileId == id && x.PageId == page.PageId).FirstOrDefault();
                                if (dbImage != null)
                                {
                                    dbImage.PageId = page.PageId;
                                }
                            }
                            catch
                            {

                            }
                        }

                        if (child.Name.ToLower() == "video")
                        {

                            // check for image source
                            // check for "type"
                        }

                        if (child.Name.ToLower() == "button")
                        {
                            // check if it has a class of "quiz-submit" or "survey-submit"
                            try
                            {
                                var classList = child.Attributes["class"].Value;
                                if (classList.Contains("quiz-submit"))
                                {

                                }
                                if (classList.Contains("survey-submit"))
                                {

                                }
                            }
                            catch (NullReferenceException)
                            {

                            }
                        }
                    }
                }

                page.PageContent = doc.OuterXml;
                cdb.SaveChanges();
            }

            return Content(sb.ToString());
        }


        public ActionResult Duplicate(int id)
        {
            ApplicationDbContext cdb = new ApplicationDbContext();
            var book = cdb.Books.Where(x => x.BookId == id).FirstOrDefault();

            var modules = cdb.Modules.Where(x => x.BookId == id).ToList();
            var moduleIds = modules.Select(x => x.ModuleId).ToList();

            var sections = cdb.Sections.Where(x => moduleIds.Contains(x.ModuleId)).ToList();
            var sectionIds = sections.Select(x => x.SectionId).ToList();

            var chapters = cdb.Chapters.Where(x => sectionIds.Contains(x.SectionId)).ToList();
            //var chapterIds = chapters.Select(x => x.ChapterId).ToList();

            var pages = cdb.Pages.Where(x => x.BookId == id).ToList();
            var pageIds = pages.Select(x => x.PageId).ToList();

            var files = cdb.AmosFiles.Where(x => pageIds.Contains(x.PageId)).ToList();


            // copy these elements
            Book newBook = new Book();
            newBook.Create(User.Identity.Name);
            newBook.Name = book.Name + " (copy)";
            newBook.Published = book.Published;
            newBook.Version = book.Version;

            cdb.Books.Add(newBook);
            cdb.SaveChanges();

            // Key: Old Page Id     Value: New Page Id
            Dictionary<int, int> PageIdPairs = new Dictionary<int, int>();
            Dictionary<int, int> oldToNewFileIds = new Dictionary<int, int>();

            foreach (var mod in modules)
            {
                Module newModule = new Module
                {
                    BookId = newBook.BookId,
                    Name = mod.Name,
                    SortOrder = mod.SortOrder,
                    Theme = mod.Theme
                };

                cdb.Modules.Add(newModule);
                cdb.SaveChanges();

                foreach (var sec in sections.Where(x => x.ModuleId == mod.ModuleId).ToList())
                {
                    Section newSection = new Section
                    {
                        ModuleId = newModule.ModuleId,
                        Name = sec.Name,
                        SortOrder = sec.SortOrder,
                    };
                    cdb.Sections.Add(newSection);
                    cdb.SaveChanges();

                    foreach (var cha in chapters.Where(x => x.SectionId == sec.SectionId).ToList())
                    {
                        Chapter newChapter = new Chapter
                        {
                            SectionId = newSection.SectionId,
                            Name = cha.Name,
                            SortOrder = cha.SortOrder
                        };
                        cdb.Chapters.Add(newChapter);
                        cdb.SaveChanges();

                        foreach (var pag in pages.Where(x => x.ChapterId == cha.ChapterId).ToList())
                        {
                            Page newPage = new Page
                            {
                                BookId = newBook.BookId,
                                ChapterId = newChapter.ChapterId,
                                PageContent = pag.PageContent,
                                SortOrder = pag.SortOrder,
                                Title = pag.Title,
                                Type = pag.Type
                            };
                            newPage.Create(User.Identity.Name);
                            cdb.Pages.Add(newPage);
                            cdb.SaveChanges();

                            PageIdPairs.Add(pag.PageId, newPage.PageId);

                            foreach (var file in files.Where(x => x.PageId == pag.PageId).ToList())
                            {
                                AmosFile newFile = new AmosFile
                                {
                                    Content = file.Content,
                                    ContentType = file.ContentType,
                                    FileName = file.FileName,
                                    FileType = file.FileType,
                                    PageId = newPage.PageId
                                };
                                cdb.AmosFiles.Add(newFile);
                                cdb.SaveChanges();

                                oldToNewFileIds.Add(file.FileId, newFile.FileId);
                            }
                        }
                    }
                }
            }

            // Fix all the page buttons
            ManagePagesModel managePagesModel = new ManagePagesModel(newBook.BookId);



            foreach (var page in managePagesModel.PageListModel.PageList)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(page.PageContent);
                foreach (XmlElement img in doc.SelectNodes("//image"))
                {
                    try
                    {
                        var oldSource = img.GetAttribute("source").Split('_');
                        int oldSourceId = Convert.ToInt32(oldSource[1]);

                        string newSource = "i_" + oldToNewFileIds[oldSourceId];
                        img.SetAttribute("source", newSource);
                    }
                    catch { }

                }

                foreach (XmlElement vid in doc.SelectNodes("//video"))
                {
                    try
                    {
                        var oldSource = vid.GetAttribute("source").Split('_');
                        int oldSourceId = Convert.ToInt32(oldSource[1]);

                        string newSource = "v_" + oldToNewFileIds[oldSourceId];
                        vid.SetAttribute("source", newSource);
                    }
                    catch { }

                }

                foreach (XmlElement dialogLink in doc.SelectNodes("//a[contains(concat(' ', @class, ' '), ' dialogLink ')]"))
                {
                    try
                    {
                        var oldSource = dialogLink.GetAttribute("data-content").Split('_');
                        int oldSourceId = Convert.ToInt32(oldSource[1]);

                        string newSource = oldSource[0] + "_" + oldToNewFileIds[oldSourceId];
                        dialogLink.SetAttribute("data-content", newSource);
                    }
                    catch { }
                }

                XmlElement p = (XmlElement)doc.SelectSingleNode("//page");
                p.SetAttribute("id", "p_" + page.PageId);

                cdb.Pages.Where(x => x.PageId == page.PageId).FirstOrDefault().PageContent = doc.OuterXml;

                cdb.SaveChanges();
            }

            foreach (var button in managePagesModel.pageButtons)
            {
                try
                {
                    AssignButton(button.ButtonId, button.PageId, PageIdPairs[button.NavPageId]);
                }
                catch { }

            }

            // copy over .dialogLink as well

            return RedirectToAction("ListBooks");
        }

        public ActionResult DuplicatePage(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var page = db.Pages.Where(x => x.PageId == id).FirstOrDefault();

            Page newPage = new Page();
            newPage.Create(User.Identity.Name);
            newPage.PageContent = page.PageContent;
            newPage.Title = page.Title + " - Copy";
            newPage.Type = "content";
            db.Pages.Add(newPage);

            db.SaveChanges();

            var files = db.AmosFiles.Where(x => x.PageId == id).ToList();
            foreach (var file in files)
            {
                AmosFile f = new AmosFile();
                f.PageId = newPage.PageId;
                f.Content = file.Content;
                f.ContentType = file.ContentType;
                f.FileName = file.FileName;
                f.FileType = file.FileType;

                db.AmosFiles.Add(f);
            }
            db.SaveChanges();


            return RedirectToAction("ListPages");
        }



        //[HttpPost]
        //public SaveBookModel SaveBook(SaveBookModel model)
        //{
        //    ApplicationDbContext adb = new ApplicationDbContext();
        //    Book newOrMatchedBook = new Book();
        //    newOrMatchedBook = adb.Books.Where(x => x.BookId == model.S_Book.BookId).FirstOrDefault();
        //    if (newOrMatchedBook == null)
        //    {
        //        // creating a new book
        //        newOrMatchedBook = new Book();
        //        newOrMatchedBook.Create(User.Identity.Name);
        //        newOrMatchedBook.Published = false;
        //        newOrMatchedBook.Name = model.S_Book.Name;
        //        newOrMatchedBook.Version = model.S_Book.Version;

        //        adb.Books.Add(newOrMatchedBook);
        //    }
        //    else
        //    {
        //        // we're updating a book.
        //        //  delete all modules, sections, chapters
        //        //  set all pageIds and fileIds == 0
        //        ResetBook(model.S_Book.BookId);

        //        newOrMatchedBook.Modify(User.Identity.Name);
        //        newOrMatchedBook.Name = model.S_Book.Name;
        //        newOrMatchedBook.Version = model.S_Book.Version;
        //    }

        //    adb.SaveChanges();



        //    // go through the modules

        //    // keep track of old and temp ids
        //    //  <TempId, ActualNewId>
        //    Dictionary<int, int> ModuleTempIdPairs = new Dictionary<int, int>();
        //    int moduleSortCount = 10;
        //    foreach (var module in model.s_Modules)
        //    {
        //        Module newOrMatchedModule = new Module();
        //        newOrMatchedModule = adb.Modules.Where(x => x.ModuleId == module.ModuleId).FirstOrDefault();
        //        bool addModuleFlag = false;
        //        if (newOrMatchedModule == null)
        //        {
        //            newOrMatchedModule = new Module();
        //            addModuleFlag = true;
        //        }

        //        newOrMatchedModule.Name = module.Name;
        //        newOrMatchedModule.BookId = newOrMatchedBook.BookId;
        //        newOrMatchedModule.SortOrder = moduleSortCount;
        //        moduleSortCount += 10;
        //        newOrMatchedModule.Theme = module.Theme;

        //        if (addModuleFlag) adb.Modules.Add(newOrMatchedModule);

        //        adb.SaveChanges();
        //        ModuleTempIdPairs.Add(module.ModuleTempId, newOrMatchedModule.ModuleId);

        //    }


        //    // go through sections

        //    // keep track of old and temp ids
        //    Dictionary<int, int> SectionTempIdPairs = new Dictionary<int, int>();
        //    int sectionSortCount = 10;
        //    int previousSectionId = 0;
        //    foreach (var section in model.s_Sections)
        //    {
        //        if (previousSectionId != section.SectionId)
        //        {
        //            moduleSortCount = 10;
        //            previousModuleId = section.SectionId;
        //        }
        //        else
        //        {
        //            sectionSortCount += 10;
        //            previousModuleId = section.SectionId;
        //        }
        //        Section newOrMatchedSection = new Section();
        //        newOrMatchedSection = adb.Sections.Where(x => x.SectionId == section.SectionId).FirstOrDefault();
        //        bool addSectionFlag = false;
        //        if (newOrMatchedSection == null)
        //        {
        //            newOrMatchedSection = new Section();
        //            addSectionFlag = true;
        //        }

        //        newOrMatchedSection.Name = section.Name;
        //        newOrMatchedSection.SortOrder = section.SortOrder;

        //        newOrMatchedSection.ModuleId = ModuleTempIdPairs[section.ModuleTempId];
        //        if (addSectionFlag) adb.Sections.Add(newOrMatchedSection);

        //        adb.SaveChanges();
        //        SectionTempIdPairs.Add(section.SectionTempId, newOrMatchedSection.SectionId);
        //    }


        //    // go through chapters

        //    // keep track of old and temp ids
        //    Dictionary<int, int> ChapterTempIdPairs = new Dictionary<int, int>();
        //    int chapterSortCount = 10;
        //    int previousChapterId = 0;
        //    foreach (var chapter in model.s_Chapters)
        //    {
        //        if (previousChapterId != chapter.ChapterId)
        //        {
        //            chapterSortCount = 10;
        //            previousChapterId = chapter.se;
        //        }
        //        else
        //        {
        //            chapterSortCount += 10;
        //            previousChapterId = chapter.SectionId;
        //        }
        //        Chapter newOrMatchedChapter = new Chapter();
        //        newOrMatchedChapter = adb.Chapters.Where(x => x.ChapterId == chapter.ChapterId).FirstOrDefault();
        //        bool addChapterFlag = false;
        //        if (newOrMatchedChapter == null)
        //        {
        //            newOrMatchedChapter = new Chapter();
        //            addChapterFlag = true;
        //        }

        //        newOrMatchedChapter.Name = chapter.Name;
        //        newOrMatchedChapter.SortOrder = chapterSortCount;

        //        newOrMatchedChapter.SectionId = SectionTempIdPairs[chapter.SectionTempId];
        //        if (addChapterFlag) adb.Chapters.Add(newOrMatchedChapter);

        //        adb.SaveChanges();
        //        ChapterTempIdPairs.Add(chapter.ChapterTempId, newOrMatchedChapter.ChapterId);
        //    }


        //    // go through pages

        //    // keep track of old and temp ids
        //    foreach (var page in model.s_Pages)
        //    {
        //        Page newOrMatchedPage = new Page();
        //        newOrMatchedPage = adb.Pages.Where(x => x.PageId == page.PageId).FirstOrDefault();
        //        bool addPageFlag = false;
        //        if (newOrMatchedPage == null)
        //        {
        //            newOrMatchedPage = new Page();
        //            newOrMatchedPage.PageContent = "<page id=\"{id}\" type=\"content\"></page>";
        //            newOrMatchedPage.Type = "content";
        //            addPageFlag = true;
        //        }

        //        newOrMatchedPage.Title = page.Name;
        //        newOrMatchedPage.SortOrder = page.SortOrder;
        //        newOrMatchedPage.BookId = newOrMatchedBook.BookId;

        //        newOrMatchedPage.ChapterId = ChapterTempIdPairs[page.ChapterTempId];
        //        if (addPageFlag)
        //        {
        //            newOrMatchedPage.Create(User.Identity.Name);
        //            adb.Pages.Add(newOrMatchedPage);
        //            adb.SaveChanges();
        //            newOrMatchedPage.PageContent = newOrMatchedPage.PageContent.Replace("{id}", "p_" + newOrMatchedPage.PageId);
        //            adb.SaveChanges();
        //            newOrMatchedBook.Published = false;
        //        }
        //        else
        //        {
        //            newOrMatchedPage.Modify(User.Identity.Name);
        //        }

        //    }



        //    adb.SaveChanges();


        //    // You're right here. Everything is fine, except your sort orders are whacky.
        //    //  I think I can go through each object now and check if it has a different parent.

        //    return new SaveBookModel(new PageListModel(newOrMatchedBook.BookId));
        //}
        public void ResetBook(int bookId)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var modules = db.Modules.Where(x => x.BookId == bookId).ToList();
            var moduleIds = modules.Select(x => x.ModuleId).ToList();
            var sections = db.Sections.Where(x => moduleIds.Contains(x.ModuleId)).ToList();
            var sectionIds = sections.Select(x => x.SectionId).ToList();
            var chapters = db.Chapters.Where(x => sectionIds.Contains(x.SectionId)).ToList();

            db.Chapters.RemoveRange(chapters);
            db.Sections.RemoveRange(sections);
            db.Modules.RemoveRange(modules);

            var pages = db.Pages.Where(x => x.BookId == bookId).ToList();
            var pageIds = pages.Select(x => x.PageId).ToList();

            var files = db.AmosFiles.Where(x => pageIds.Contains(x.PageId)).ToList();
            foreach (var file in files)
            {
                file.PageId = 0;
            }

            foreach (var page in pages)
            {
                page.Modify(User.Identity.Name);
                page.BookId = 0;
                page.ChapterId = 0;
                page.SortOrder = 0;
            }

            db.SaveChanges();
        }



        [HttpPost]
        public ActionResult SaveBook(SaveBookModel model)
        {
            // This book is coming in properly ordered and numbered. 
            //  The looping doesn't have to count the sort order


            ApplicationDbContext db = new ApplicationDbContext();

            var book = db.Books.Where(x => x.BookId == model.S_Book.BookId).FirstOrDefault();
            bool addBook = false;
            if (book == null)
            {
                book = new Book();
                book.Create(User.Identity.Name);
                addBook = true;
            }
            else
            {
                book.Modify(User.Identity.Name);

            }
            try { book.Name = model.S_Book.Name; }
            catch { book.Name = "New Book"; }

            book.Published = false;

            try { book.Version = model.S_Book.Version; }
            catch { book.Version = "1"; }

            if (addBook) db.Books.Add(book);
            db.SaveChanges();

            // update the model as well
            model.S_Book.BookId = book.BookId;
            model.S_Book.Name = book.Name;
            model.S_Book.Version = book.Version;

            // MODULES
            foreach (var mod in model.s_Modules)
            {
                var module = db.Modules.Where(x => x.ModuleId == mod.ModuleId).FirstOrDefault();
                bool addModule = false;
                if (module == null)
                {
                    module = new Module();
                    addModule = true;
                }

                module.BookId = book.BookId;

                try { module.Name = mod.Name; }
                catch { module.Name = "New Module"; }

                try { module.Theme = mod.Theme; }
                catch { module.Theme = "1"; }

                module.SortOrder = mod.SortOrder;

                if (addModule) db.Modules.Add(module);

                db.SaveChanges();

                // update the model
                mod.ModuleId = module.ModuleId;
                mod.Name = module.Name;
                mod.ParentId = book.BookId;
                mod.SortOrder = module.SortOrder;
                mod.Theme = module.Theme;
            }

            // SECTIONS
            foreach (var sec in model.s_Sections)
            {
                var section = db.Sections.Where(x => x.SectionId == sec.SectionId).FirstOrDefault();
                bool addSection = false;
                if (section == null)
                {
                    section = new Section();
                    addSection = true;
                }

                section.ModuleId = sec.ParentId;

                try { section.Name = sec.Name; }
                catch { section.Name = "New Section"; }

                section.SortOrder = sec.SortOrder;

                if (addSection) db.Sections.Add(section);

                db.SaveChanges();

                // update the model
                sec.Name = section.Name;
                sec.ParentId = section.ModuleId;
                sec.SectionId = section.SectionId;
                sec.SortOrder = section.SortOrder;
            }

            // CHAPTERS
            foreach (var cha in model.s_Chapters)
            {
                var chapter = db.Chapters.Where(x => x.ChapterId == cha.ChapterId).FirstOrDefault();
                bool addChapter = false;
                if (chapter == null)
                {
                    chapter = new Chapter();
                    addChapter = true;
                }

                chapter.SectionId = cha.ParentId;

                try { chapter.Name = cha.Name; }
                catch { chapter.Name = "New Chapter"; }

                chapter.SortOrder = cha.SortOrder;

                if (addChapter) db.Chapters.Add(chapter);

                db.SaveChanges();

                // update the model
                cha.ChapterId = chapter.ChapterId;
                cha.Name = chapter.Name;
                cha.ParentId = chapter.SectionId;
                cha.SortOrder = chapter.SortOrder;
            }

            // PAGES
            foreach (var pa in model.s_Pages)
            {
                var page = db.Pages.Where(x => x.PageId == pa.PageId).FirstOrDefault();
                bool addPage = false;
                if (page == null)
                {
                    page = new Page();
                    addPage = true;
                    page.Create(User.Identity.Name);
                    page.PageContent = "<page id=\"{id}\" type=\"content\"></page>";
                }
                else page.Modify(User.Identity.Name);

                page.BookId = book.BookId;
                page.Type = "content";


                page.ChapterId = pa.ParentId;

                try { page.Title = pa.Name; }
                catch { page.Title = "New Page"; }

                page.SortOrder = pa.SortOrder;

                if (addPage)
                {
                    db.Pages.Add(page);
                    db.SaveChanges();
                    page.PageContent = page.PageContent.Replace("{id}", "p_" + page.PageId);
                }

                db.SaveChanges();

                // update the model
                pa.Name = page.Title;
                pa.PageId = page.PageId;
                pa.ParentId = page.ChapterId;
                pa.SortOrder = page.SortOrder;
            }

            db.SaveChanges();

            return View("BuildList", model);
        }

        [HttpPost]
        public ActionResult BuildList(SaveBookModel model)
        {
            return View(model);
        }
        [HttpPost]
        public ActionResult DeleteItem(int id, string type)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            if (type == "module")
            {
                var mod = db.Modules.Where(x => x.ModuleId == id).FirstOrDefault();

                var sections = db.Sections.Where(x => x.ModuleId == id);
                var sectionIds = sections.Select(x => x.SectionId).ToList();

                var chapters = db.Chapters.Where(x => sectionIds.Contains(x.SectionId));
                var chapterIds = chapters.Select(x => x.ChapterId).ToList();

                foreach (var page in db.Pages.Where(x => chapterIds.Contains(x.ChapterId)).ToList())
                {
                    page.BookId = 0;
                    page.ChapterId = 0;
                    page.Modify(User.Identity.Name);
                    page.SortOrder = 0;
                }

                db.Chapters.RemoveRange(chapters);
                db.Sections.RemoveRange(sections);
                db.Modules.Remove(mod);
            }

            if (type == "section")
            {
                var section = db.Sections.Where(x => x.SectionId == id).FirstOrDefault();

                var chapters = db.Chapters.Where(x => x.SectionId == id);
                var chapterIds = chapters.Select(x => x.ChapterId).ToList();

                foreach (var page in db.Pages.Where(x => chapterIds.Contains(x.ChapterId)).ToList())
                {
                    page.BookId = 0;
                    page.ChapterId = 0;
                    page.Modify(User.Identity.Name);
                    page.SortOrder = 0;
                }

                db.Sections.Remove(section);
                db.Chapters.RemoveRange(chapters);
            }

            if (type == "chapter")
            {
                var chapter = db.Chapters.Where(x => x.ChapterId == id).FirstOrDefault();

                foreach (var page in db.Pages.Where(x => x.ChapterId == id).ToList())
                {
                    page.BookId = 0;
                    page.ChapterId = 0;
                    page.Modify(User.Identity.Name);
                    page.SortOrder = 0;
                }

                db.Chapters.Remove(chapter);
            }

            if (type == "page")
            {
                var page = db.Pages.Where(x => x.PageId == id).FirstOrDefault();
                page.BookId = 0;
                page.ChapterId = 0;
                page.Modify(User.Identity.Name);
                page.SortOrder = 0;
            }


            db.SaveChanges();
            return Content("success");
        }

        [HttpPost]
        public JsonResult GetAvailablePages()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var unusedPages = db.Pages.Where(x => x.BookId == 0).ToList();

            return Json(unusedPages);
        }


        [HttpPost]
        public ActionResult ChangePublishStatus(int BookId, bool IsPublish)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            if (!IsPublish)
            {
                db.Books.Where(x => x.BookId == BookId).FirstOrDefault().Published = false;
                db.SaveChanges();
                return Content("success");
            }

            else
            {
                // validate
                bool foundErrors = false;
                StringBuilder sb = new StringBuilder();

                // Step 1:
                //  Ensure the module, section, chapter, and page configuration is correct
                ManagePagesModel model = new ManagePagesModel(BookId);

                // check the book properties
                if (string.IsNullOrWhiteSpace(model.PageListModel.GetBook.Name))
                {
                    foundErrors = true;
                    sb.AppendLine("The book must have a name.");
                }
                if (string.IsNullOrWhiteSpace(model.PageListModel.GetBook.Version))
                {
                    foundErrors = true;
                    sb.AppendLine("The book must have a version.");
                }

                // check the modules
                int moduleCount = 0;
                foreach (var mod in model.PageListModel.Modules)
                {
                    moduleCount++;

                    if (string.IsNullOrWhiteSpace(mod.Name))
                    {
                        foundErrors = true;
                        sb.AppendLine("Module #" + mod.ModuleId + " found without a name.");
                    }
                    if (string.IsNullOrWhiteSpace(mod.Theme))
                    {
                        foundErrors = true;
                        sb.AppendLine("Module #" + mod.ModuleId + " found without a theme.");
                    }

                    // check the sections
                    int sectionCount = 0;
                    foreach (var sec in model.PageListModel.Sections.Where(x => x.ModuleId == mod.ModuleId).ToList())
                    {
                        sectionCount++;

                        if (string.IsNullOrWhiteSpace(sec.Name))
                        {
                            foundErrors = true;
                            sb.AppendLine("Section #" + sec.SectionId + " found without a name.");
                        }

                        // check the chapters
                        int chapterCount = 0;
                        foreach (var cha in model.PageListModel.Chapters.Where(x => x.SectionId == sec.SectionId).ToList())
                        {
                            chapterCount++;

                            if (string.IsNullOrWhiteSpace(cha.Name))
                            {
                                foundErrors = true;
                                sb.AppendLine("Chapter #" + cha.ChapterId + " found without a name.");
                            }

                            // check the pages
                            int pageCount = 0;
                            foreach (var pa in model.PageListModel.PageList.Where(x => x.ChapterId == cha.ChapterId).ToList())
                            {
                                pageCount++;

                                if (string.IsNullOrWhiteSpace(pa.Type))
                                {
                                    foundErrors = true;
                                    sb.AppendLine("Page #" + pa.PageId + " found without a proper 'type.'");
                                }

                                if (string.IsNullOrWhiteSpace(pa.Title))
                                {
                                    foundErrors = true;
                                    sb.AppendLine("Page #" + pa.PageId + " found without a 'title.'");
                                }
                            }

                            if (pageCount == 0)
                            {
                                foundErrors = true;
                                sb.AppendLine("No pages found in chapter: " + cha.Name);
                            }
                        }

                        if (chapterCount == 0)
                        {
                            foundErrors = true;
                            sb.AppendLine("No chapters found in section: " + sec.Name);
                        }
                    }

                    if (sectionCount == 0)
                    {
                        foundErrors = true;
                        sb.AppendLine("No sections found in module: " + mod.Name);
                    }
                }

                if (moduleCount == 0)
                {
                    foundErrors = true;
                    sb.AppendLine("No modules found in this book.");
                }

                // check if any sections, chapters, or pages are unlinked
                var moduleIds = model.PageListModel.Modules.Select(x => x.ModuleId).ToList();
                var sectionIds = model.PageListModel.Sections.Select(x => x.SectionId).ToList();
                var chapterIds = model.PageListModel.Chapters.Select(x => x.ChapterId).ToList();
                var pageIds = model.PageListModel.PageList.Select(x => x.PageId).ToList();

                var unlinkedSections = model.PageListModel.Sections.Where(x => !moduleIds.Contains(x.ModuleId)).ToList();
                if (unlinkedSections.Count != 0)
                {
                    foundErrors = true;
                    sb.AppendLine("Found " + unlinkedSections.Count + " sections NOT linked to a module.");
                }

                var unlinkedChapters = model.PageListModel.Chapters.Where(x => !sectionIds.Contains(x.SectionId)).ToList();
                if (unlinkedChapters.Count != 0)
                {
                    foundErrors = true;
                    sb.AppendLine("Found " + unlinkedChapters.Count + " chapters NOT linked to a section.");
                }

                var unlinkedPages = model.PageListModel.PageList.Where(x => !chapterIds.Contains(x.ChapterId)).ToList();
                if (unlinkedPages.Count != 0)
                {
                    foundErrors = true;
                    sb.AppendLine("Found " + unlinkedPages.Count + " pages NOT linked to a chapter.");
                }

                if (!foundErrors)
                {
                    sb.AppendLine("Book configuration is valid. Checking page buttons...");
                }

                List<int> currentPageIds = model.PageListModel.PageList.Select(x => x.PageId).ToList();
                foreach (var btn in model.pageButtons)
                {
                    if (btn.NavPageId == 0)
                    {
                        foundErrors = true;
                        sb.AppendLine("Unlinked page button found. (Page: " + btn.getPage.Title + " - Button Text: " + btn.ButtonText + ")");
                    }
                    else if (!currentPageIds.Contains(btn.NavPageId))
                    {
                        foundErrors = true;
                        sb.AppendLine("Button links to a page not included in the book. Current Page #" + btn.getPage.Title);
                    }
                }

                foreach (var page in model.PageListModel.PageList)
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    try
                    {
                        xmlDoc.LoadXml(page.PageContent);
                    }
                    catch
                    {
                        foundErrors = true;
                        sb.AppendLine("Unable to parse page XML. (PageId: " + page.Title + ")");
                    }
                    foreach (XmlElement contentNode in xmlDoc.SelectNodes("//image"))
                    {
                        if (!contentNode.HasAttribute("type"))
                        {
                            foundErrors = true;
                            sb.AppendLine("Image found without attribute 'type'. (PageId: " + page.Title + ", ImageId: " + contentNode.GetAttribute("source") + ")");
                        }
                    }
                    foreach (XmlElement contentNode in xmlDoc.SelectNodes("//video"))
                    {
                        if (!contentNode.HasAttribute("type"))
                        {
                            foundErrors = true;
                            sb.AppendLine("Video found without attribute 'type'. (PageId: " + page.Title + ", VideoId: " + contentNode.GetAttribute("source") + ")");
                        }
                    }
                }


                if (!foundErrors)
                {
                    Book book = db.Books.Where(x => x.BookId == BookId).FirstOrDefault();
                    book.Published = true;
                    book.Modify(User.Identity.Name);
                    sb.AppendLine("No errors found, the book has been saved and published.");

                    db.SaveChanges();
                    return Content("success");
                }
                else
                {
                    return Json(new { Errors = foundErrors, Details = sb.ToString() });
                }
            }
        }

        public ActionResult BuildButtonList(ManagePagesModel model)
        {
            return View("BuildButtonList", model);
        }

        public ActionResult Manage(int id)
        {
            return View(new ManagePagesModel(id));
        }



        [HttpPost]
        public ActionResult AssignButton(int buttonId, int onPageId, int toPageId)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var onPage = db.Pages.Where(x => x.PageId == onPageId).FirstOrDefault();
            var toPage = db.Pages.Where(x => x.PageId == toPageId).FirstOrDefault();
            
            // buttonId is the count of numbers on one page. works with indexes
            //  ex: find the 4th button on pageId == onPageId

            // parse the page
            XmlDocument xmlDoc = new XmlDocument();
            List<PageButton> pageButtonList = new List<PageButton>();
            int buttonCount = 1;
            try
            {
                xmlDoc.LoadXml(onPage.PageContent);

                foreach (XmlElement contentNode in xmlDoc.SelectNodes("//button"))
                {
                    try
                    {
                        string classList = contentNode.Attributes["class"].Value;
                        if (!classList.Contains("quiz-submit") && !classList.Contains("survey-submit"))
                        {
                            if (buttonCount == buttonId)
                                contentNode.SetAttribute("id", "p_" + toPageId.ToString());
                        }

                    }
                    catch
                    {
                        // This is here on purpose. Buttons don't always have an attribute "class" so failing is good, because it
                        //  means it's not a quiz or survey button
                        if (buttonCount == buttonId)
                            contentNode.SetAttribute("id", "p_" + toPageId.ToString());
                    }
                    buttonCount++;
                }

                // find all anchor elements with class="navigateTo"
                foreach (XmlElement contentNode in xmlDoc.SelectNodes("//a[contains(concat(' ', @class, ' '), ' navigateTo ')]"))
                {
                    if (buttonCount == buttonId)
                        contentNode.SetAttribute("data-id", "p_" + toPageId.ToString());
                    buttonCount++;
                }

                // find all anchor elements with class="popupPage"
                foreach (XmlElement contentNode in xmlDoc.SelectNodes("//a[contains(concat(' ', @class, ' '), ' popupPage ')]"))
                {
                    if (buttonCount == buttonId)
                        contentNode.SetAttribute("data-page", toPageId.ToString());
                    buttonCount++;
                }

                onPage.PageContent = xmlDoc.OuterXml;
                db.SaveChanges();

            }
            catch { }

            return Content(toPage.Title);
        }


        [HttpPost]
        public ActionResult GetContent()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            return Json(db.Pages.Where(x => x.BookId == 0).Select(x => new { x.PageId, x.PageContent }));
        }

        [HttpPost]
        public ActionResult DeletePage(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var page = db.Pages.Where(x => x.PageId == id).FirstOrDefault();
            db.Pages.Remove(page);
            db.SaveChanges();
            return Content("success");
        }



        public ActionResult ManageButtons(int id)
        {
            if (IsBookPublished(id)) return RedirectToAction("NoAccess", new { id=1 });
            return View(new ManagePagesModel(id));
        }








        public ActionResult ConvertXmlToHtml()
        {
            ApplicationDbContext db = new ApplicationDbContext();

            foreach (var page in db.Pages.ToList())
            {
                var oldContent = page.PageContent;
                var xmlDoc = new XmlDocument();
                try
                {
                    xmlDoc.Load(oldContent);
                }
                catch { }
            }

            return Content("success");
        }




        public static bool IsBookPublished(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var book = db.Books.Find(id);
            return book.Published;
        }
        public ActionResult NoAccess(int id)
        {
            // id == 0 --> unpublished book. (Used for when hitting "View" controller)
            // id == 1 --> published book. (used for not editing published)

            if (id == 0) ViewBag.Message = "This book has been removed, and cannot be accessed. Please contact the system administrator.";
            else ViewBag.Message = "This book is currently published, and cannot be edited. Please unpublish the book before making any changes.";

            return View();
        }
    }


}