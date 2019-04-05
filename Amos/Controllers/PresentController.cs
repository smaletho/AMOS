using Amos.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Amos.Controllers
{
    public class PresentController : Controller
    {
        

        // GET: Present
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewBooks()
        {
            ApplicationDbContext adb = new ApplicationDbContext();
            return View(adb.Books.Where(x => x.Published).ToList());
        }

        public static void InitialImport()
        {
            string ImportBookDirectory = HostingEnvironment.MapPath("~/_FileTransfer/Import");
            string ExportSubjectDataDirectory = HostingEnvironment.MapPath("~/_FileTransfer/SubjectData/");

            var db = new ApplicationDbContext();


            // Check for books waiting to be imported
            var existingBooks = db.Books.Select(x => x.FileName).ToList();

            string sourceDir = ImportBookDirectory;

            string curlReturn = "";

            try
            {
                DirectoryInfo di = new DirectoryInfo(sourceDir);
                foreach (FileInfo file in di.GetFiles())
                {
                    if (!existingBooks.Contains(file.Name))
                    {
                        ScheduledJobTracker log = new ScheduledJobTracker();
                        try
                        {
                            // import this book
                            FileStream fs = file.OpenRead();

                            log.Action = "Found new book for import (" + file.Name + ")";
                            log.ExecutionTime = DateTime.Now;
                            List<string> results = Controllers.BuildController.Action_Import(fs, file.Name);
                            log.ExtraData = string.Join("\n", results);
                        }
                        catch (IOException e)
                        {
                            // Trouble reading the file
                            log.Action = "Found new book for import (" + file.Name + ")";
                            log.ExecutionTime = DateTime.Now;
                            log.ExtraData = "ERROR! IOException. " + e.InnerException;
                        }
                        catch (UnauthorizedAccessException e)
                        {
                            // Does not have read access to the file
                            log.Action = "Found new book for import (" + file.Name + ")";
                            log.ExecutionTime = DateTime.Now;
                            log.ExtraData = "ERROR! UnauthorizedAccessException. " + e.InnerException;
                        }
                        curlReturn += log.ExtraData + Environment.NewLine;
                        db.ScheduledJobTrackers.Add(log);
                    }
                }
            }
            catch (DirectoryNotFoundException e)
            {
                // Could not find directory
                ScheduledJobTracker log = new ScheduledJobTracker();
                log.Action = "Found new book for import (n/a)";
                log.ExecutionTime = DateTime.Now;
                log.ExtraData = "ERROR! DirectoryNotFoundException. " + e.InnerException;
                curlReturn += log.ExtraData + Environment.NewLine;
                db.ScheduledJobTrackers.Add(log);
            }

            db.SaveChanges();

        }

        public ActionResult ProcessScheduledJobs()
        {
            string ImportBookDirectory = Server.MapPath("~/_FileTransfer/Import");
            //string ImportBookDirectory = "C:/Users/rktcreative/Desktop/AMOS_Content/Import";

            string ExportSubjectDataDirectory = Server.MapPath("~/_FileTransfer/SubjectData/");



            //string ExportSubjectDataDirectory = "C:/Users/rktcreative/Desktop/AMOS_Content/SubjectData/";

            var db = new ApplicationDbContext();


            // Check for books waiting to be imported
            var existingBooks = db.Books.Select(x => x.FileName).ToList();

            string sourceDir = ImportBookDirectory;

            string curlReturn = "";

            try
            {
                DirectoryInfo di = new DirectoryInfo(sourceDir);
                foreach (FileInfo file in di.GetFiles())
                {
                    if (!existingBooks.Contains(file.Name))
                    {
                        ScheduledJobTracker log = new ScheduledJobTracker();
                        try
                        {
                            // import this book
                            FileStream fs = file.OpenRead();

                            log.Action = "Found new book for import (" + file.Name + ")";
                            log.ExecutionTime = DateTime.Now;
                            List<string> results = Controllers.BuildController.Action_Import(fs, file.Name);
                            log.ExtraData = string.Join("\n", results);
                        }
                        catch (IOException e)
                        {
                            // Trouble reading the file
                            log.Action = "Found new book for import (" + file.Name + ")";
                            log.ExecutionTime = DateTime.Now;
                            log.ExtraData = "ERROR! IOException. " + e.InnerException;
                        }
                        catch (UnauthorizedAccessException e)
                        {
                            // Does not have read access to the file
                            log.Action = "Found new book for import (" + file.Name + ")";
                            log.ExecutionTime = DateTime.Now;
                            log.ExtraData = "ERROR! UnauthorizedAccessException. " + e.InnerException;
                        }
                        curlReturn += log.ExtraData + Environment.NewLine;
                        db.ScheduledJobTrackers.Add(log);
                    }
                }
            }
            catch (DirectoryNotFoundException e)
            {
                // Could not find directory
                ScheduledJobTracker log = new ScheduledJobTracker();
                log.Action = "Found new book for import (n/a)";
                log.ExecutionTime = DateTime.Now;
                log.ExtraData = "ERROR! DirectoryNotFoundException. " + e.InnerException;
                curlReturn += log.ExtraData + Environment.NewLine;
                db.ScheduledJobTrackers.Add(log);
            }

            db.SaveChanges();




            // Check for non-exported datasets
            var untaggedUserTrackers = db.UserTrackers.Where(x => !x.Exported).ToList();
            foreach (var tracker in untaggedUserTrackers)
            {
                StringBuilder sb = new StringBuilder();

                int intBookId = Convert.ToInt32(tracker.BookId.Replace("b_", ""));

                Book book = db.Books.Where(x => x.BookId == intBookId).FirstOrDefault();
                if (book != null)
                {
                    sb.AppendLine("Book: " + book.Name);

                    dynamic ob = JsonConvert.DeserializeObject(tracker.TrackerContent);

                    try
                    {
                        sb.AppendLine("");
                        sb.AppendLine("Quiz Responses");
                        sb.AppendLine("User, Module, Question, User Answer, Correct Answer, Is Correct?, Date Answered, Time Answered");

                        foreach (var quiz in ob.QuizResponses)
                        {
                            DateTime dt = Convert.ToDateTime(quiz.Time);
                            string isCorrect = "";
                            if (quiz.UserAnswer == quiz.CorrectAnswer) isCorrect = "Correct";
                            else isCorrect = "Incorrect";
                            string q = quiz.Question;
                            q = q.Replace(",", "");

                            string a = quiz.UserAnswer;
                            a = a.Replace(",", "");

                            sb.AppendFormat("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}" + Environment.NewLine, tracker.Email, quiz.Module, q, a, quiz.CorrectAnswer, isCorrect, dt.ToShortDateString(), dt.ToString("HH:mm:ss"));
                        }
                    }
                    catch { }

                    try
                    {
                        sb.AppendLine("");
                        sb.AppendLine("Survey Responses");
                        sb.AppendLine("User, Module, Question, User Answer, Comments, Date Answered, Time Answered");

                        foreach (var survey in ob.SurveyResponses)
                        {
                            DateTime dt = Convert.ToDateTime(survey.Time);
                            string q = survey.Question;
                            q = q.Replace(",", "");

                            string a = "";
                            if (survey.UserAnswer.comments != null)
                            {
                                a = survey.UserAnswer.comments;
                                a = a.Replace(",", "");
                            }

                            string a2 = "";
                            try
                            {
                                a2 = SubjectController.GetTextFromSurveyValue((int)survey.UserAnswer.value);
                            }
                            catch { }


                            sb.AppendFormat("{0}, {1}, {2}, {3}, {4}, {5}, {6}" + Environment.NewLine, tracker.Email, survey.Module, q, a2, a, dt.ToShortDateString(), dt.ToString("HH:mm:ss"));
                        }

                    }
                    catch { }

                    try
                    {
                        sb.AppendLine("");
                        sb.AppendLine("Activity Tracker");
                        sb.AppendLine("User, To, From, Description, Date, Time");

                        foreach (var activity in ob.ActivityTracking)
                        {
                            DateTime dt = Convert.ToDateTime(activity.Time);
                            string desc = "";
                            if (activity.description != null)
                            {
                                desc = activity.description;
                                desc = desc.Replace(",", " ");
                            }

                            string to = "";
                            if (activity.to != null)
                            {
                                to = activity.to;
                                to = to.Replace(",", " ");
                            }

                            string from = "";
                            if (activity.from != null)
                            {
                                from = activity.from;
                                from = from.Replace(",", "");
                            }

                            sb.AppendFormat("{0}, {1}, {2}, {3}, {4}, {5}" + Environment.NewLine, tracker.Email, to, from, desc, dt.ToShortDateString(), dt.ToString("HH:mm:ss"));
                        }
                    }
                    catch { }



                    // create file with sb.tostring as data
                    try
                    {
                        System.IO.File.WriteAllText(ExportSubjectDataDirectory + DateTime.Now.Ticks + "_export.csv", sb.ToString());
                        ScheduledJobTracker log = new ScheduledJobTracker();
                        log.Action = "Exported subject data";
                        log.ExecutionTime = DateTime.Now;
                        log.ExtraData = "File: " + ExportSubjectDataDirectory + DateTime.Now.Ticks + "_export.csv";
                        curlReturn += log.ExtraData + Environment.NewLine;
                        db.ScheduledJobTrackers.Add(log);
                        tracker.Exported = true;
                    }
                    catch (Exception e)
                    {
                        ScheduledJobTracker log = new ScheduledJobTracker();
                        log.Action = "UNABLE to export data";
                        log.ExecutionTime = DateTime.Now;
                        log.ExtraData = "File: " + ExportSubjectDataDirectory + DateTime.Now.Ticks + "_export.csv. " + e.InnerException + " | " + e.Message;
                        curlReturn += log.ExtraData + Environment.NewLine;
                        db.ScheduledJobTrackers.Add(log);
                    }

                    // set the "exported" bit to true
                    db.SaveChanges();

                    // sleeping here prevents files from having identical names
                    System.Threading.Thread.Sleep(50);
                }

                
            }

            if (string.IsNullOrEmpty(curlReturn)) curlReturn = "No new books or new subjects found.";
            return Content(curlReturn);

        }
    }
}