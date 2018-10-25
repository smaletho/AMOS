using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Amos.Models
{
    public class SaveBookModel
    {
        public S_Book S_Book { get; set; }
        public List<S_Module> s_Modules { get; set; }
        public List<S_Section> s_Sections { get; set; }
        public List<S_Chapter> s_Chapters { get; set; }
        public List<S_Page> s_Pages { get; set; }
    }

    public class S_Book
    {
        public int BookId { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
    }

    public class S_Module
    {
        public int ModuleId { get; set; }
        public int ModuleTempId { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public string Theme { get; set; }
    }

    public class S_Section
    {
        public int SectionId { get; set; }
        public int SectionTempId { get; set; }
        public int ModuleTempId { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
    }

    public class S_Chapter
    {
        public int ChapterId { get; set; }
        public int ChapterTempId { get; set; }
        public int SectionTempId { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
    }

    public class S_Page
    {
        public int PageId { get; set; }
        public int ChapterTempId { get; set; }
        public int SortOrder { get; set; }
        public string Name { get; set; }
    }
}