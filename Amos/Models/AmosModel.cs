using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Amos.Models
{
    public class AmosFile
    {
        [Key]
        public int FileId { get; set; }
        [StringLength(255)]
        public string FileName { get; set; }
        [StringLength(100)]
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
        public FileType FileType { get; set; }

        public int PageId { get; set; }
    }

    public class Book
    {
        public void Modify(string user)
        {
            this.ModifiedBy = user;
            this.ModifyDate = DateTime.Now;
        }
        public void Create(string user)
        {
            this.ModifiedBy = user;
            this.ModifyDate = DateTime.Now;
            this.CreatedBy = user;
            this.CreateDate = DateTime.Now;
        }


        [Key]
        public int BookId { get; set; }

        public bool Published { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifyDate { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }
        public string Version { get; set; }
    }

    public class Module
    {

        [Key]
        public int ModuleId { get; set; }

        public int SortOrder { get; set; }
        public string Name { get; set; }
        public string Theme { get; set; }

        public int BookId { get; set; }
        //public virtual Book GetBook { get; set; }
    }

    public class Section
    {

        [Key]
        public int SectionId { get; set; }

        public int SortOrder { get; set; }
        public string Name { get; set; }

        public int ModuleId { get; set; }
        //public virtual Module GetModule { get; set; }

        public virtual Module Module { get; set; } // added by Ryan

    }

    public class Chapter
    {

        [Key]
        public int ChapterId { get; set; }

        public string Name { get; set; }
        public int SortOrder { get; set; }

        public int SectionId { get; set; }
        //public virtual Section GetSection { get; set; }

        public virtual Section Section { get; set; } // added by Ryan
    }


    public class Page
    {
        public Page() { }
        public Page(string user)
        {
            this.CreateDate = DateTime.Now;
            this.CreatedBy = user;
            this.ModifiedBy = user;
            this.ModifyDate = DateTime.Now;
        }
        public Page(string user, string content)
        {
            this.CreateDate = DateTime.Now;
            this.CreatedBy = user;
            this.ModifiedBy = user;
            this.ModifyDate = DateTime.Now;

            this.PageContent = content;
        }
        public void Modify(string user)
        {
            this.ModifiedBy = user;
            this.ModifyDate = DateTime.Now;
        }

        public void Create(string user)
        {
            this.ModifiedBy = user;
            this.ModifyDate = DateTime.Now;
            this.CreatedBy = user;
            this.CreateDate = DateTime.Now;
        }

        [Key]
        public int PageId { get; set; }
        public string PageContent { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifyDate { get; set; }

        public string Type { get; set; }
        public string Title { get; set; }

        public int SortOrder { get; set; }
        public int ChapterId { get; set; }
        public int BookId { get; set; }

        //public virtual Chapter Chapter { get; set; } // added by Ryan
    }



    public class UserTracker
    {

        [Key]
        public int UserTrackerId { get; set; }
        public string Email { get; set; }
        public string BookId { get; set; }
        public string TrackerContent { get; set; }

        //var UserTracker = {
        //    Email: null,
        //    CurrentLocation: null,
        //    QuizResponses: [{
        //        Question: "",
        //        UserAnswer: "",
        //        CorrectAnswer: "",
        //        Time: ""
        //    }],
        //    SurveyResponses: [{
        //        Question: "",
        //        UserAnswer: {
        //            value: "",
        //            comments: ""
        //        },
        //        Time: ""
        //    }],
        //    StartTime: null,
        //    EndTime: null,
        //    ActivityTracking: [],
        //    VisitedPages: []
        //};

    }
}