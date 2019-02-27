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
            sb.AppendLine("User, Module, Question, User Answer, Correct Answer, Is Correct?, Date Answered, Time Answered");
            foreach (var user in trackerList)
            {
                dynamic ob = JsonConvert.DeserializeObject(user.TrackerContent);
                foreach (var quiz in ob.QuizResponses)
                {
                    DateTime dt = Convert.ToDateTime(quiz.Time);
                    string isCorrect = "";
                    if (quiz.UserAnswer == quiz.CorrectAnswer) isCorrect = "Correct";
                    else isCorrect = "Incorrect";
                    sb.AppendFormat("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}" + Environment.NewLine, user.Email, quiz.Module, quiz.Question.Replace(",", ""), quiz.UserAnswer.Replace(",", ""), quiz.CorrectAnswer, isCorrect, dt.ToShortDateString(), dt.ToString("HH:mm"));
                }
            }

            sb.AppendLine("");


            sb.AppendLine("Survey Responses");
            sb.AppendLine("User, Module, Question, User Answer, Comments, Date Answered, Time Answered");
            foreach (var user in trackerList)
            {
                dynamic ob = JsonConvert.DeserializeObject(user.TrackerContent);
                foreach (var survey in ob.SurveyResponses)
                {
                    DateTime dt = Convert.ToDateTime(survey.Time);
                    sb.AppendFormat("{0}, {1}, {2}, {3}, {4}, {5}, {6}" + Environment.NewLine, user.Email, survey.Module, survey.Question.Replace(",", ""), GetTextFromSurveyValue(survey.UserAnswer.value), survey.UserAnswer.comments.Replace(",", ""), dt.ToShortDateString(), dt.ToString("HH:mm"));
                }
            }

            sb.AppendLine("");


            sb.AppendLine("Activity Tracker");
            sb.AppendLine("User, To, From, Description, Date, Time");
            foreach (var user in trackerList)
            {
                dynamic ob = JsonConvert.DeserializeObject(user.TrackerContent);
                foreach (var activity in ob.ActivityTracking)
                {
                    DateTime dt = Convert.ToDateTime(activity.Time);
                    sb.AppendFormat("{0}, {1}, {2}, {3}, {4}, {5}" + Environment.NewLine, user.Email, activity.to, activity.from, activity.description, dt.ToShortDateString(), dt.ToString("HH:mm"));
                }
            }


            return File(new UTF8Encoding().GetBytes(sb.ToString()), "text/csv", "export.csv");
        }

        public static string GetTextFromSurveyValue(int value)
        {
            switch (value)
            {
                case 1: return "Strongly Disagree";
                case 2: return "Disagree";
                case 3: return "Neither Agree or Disagree";
                case 4: return "Agree";
                case 5: return "Strongly Agree";
                default: return "";
            }
        }
    }
}