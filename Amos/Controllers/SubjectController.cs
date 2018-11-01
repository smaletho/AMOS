using Amos.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Amos.Controllers
{
    [Authorize]
    public class SubjectController : Controller
    {
        // GET: Subject
        public ActionResult Index()
        {
            return View(new SubjectBookListModel());
        }

        public ActionResult ViewSubjects(int id)
        {
            return View(new SubjectListModel(id));
        }
        

        public ActionResult ExportSubject(int id)
        {
            // if userId == 0, export all subjects
            StringBuilder sb = new StringBuilder();
            //sb.AppendLine("User: ")

            List<UserTracker> trackerList = new List<UserTracker>();
            ApplicationDbContext cdb = new ApplicationDbContext();
            Book book = cdb.Books.Where(x => x.BookId == id).FirstOrDefault();

            string strBookId = "b_" + id;

            trackerList = cdb.UserTrackers.Where(x => x.BookId == strBookId).ToList();
            
            sb.AppendLine("Book: " + book.Name);
            sb.AppendLine("");


            sb.AppendLine("Quiz Responses");
            sb.AppendLine("User, Question, User Answer, Correct Answer, Time Answered");
            foreach (var user in trackerList)
            {
                dynamic ob = JsonConvert.DeserializeObject(user.TrackerContent);
                foreach (var quiz in ob.QuizResponses)
                {
                    sb.AppendFormat("\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\"" + Environment.NewLine, user.Email, quiz.Question, quiz.UserAnswer, quiz.CorrectAnswer, quiz.Time);
                }
            }

            sb.AppendLine("");


            sb.AppendLine("Survey Responses");
            sb.AppendLine("User, Question, User Answer, Comments, Time Answered");
            foreach (var user in trackerList)
            {
                dynamic ob = JsonConvert.DeserializeObject(user.TrackerContent);
                foreach (var survey in ob.SurveyResponses)
                {
                    sb.AppendFormat("\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\"" + Environment.NewLine, user.Email, survey.Question, survey.UserAnswer.value, survey.UserAnswer.comments, survey.Time);
                }
            }

            sb.AppendLine("");


            sb.AppendLine("Activity Tracker");
            sb.AppendLine("User, To, From, Description, Time");
            foreach (var user in trackerList)
            {
                dynamic ob = JsonConvert.DeserializeObject(user.TrackerContent);
                foreach (var activity in ob.ActivityTracking)
                {
                    sb.AppendFormat("\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\"" + Environment.NewLine, user.Email, activity.to, activity.from, activity.description, activity.time);
                }
            }


            return File(new System.Text.UTF8Encoding().GetBytes(sb.ToString()), "text/csv", "export.csv");
        }
    }
}