using Amos.Models;
using Hangfire;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Web;

namespace Amos
{
    public class ScheduledJobs
    {
        public static string ImportBookDirectory = "C:/Users/rktcreative/Desktop/AMOS_Content/Import";
        public static string ExportSubjectDataDirectory = "C:/Users/rktcreative/Desktop/AMOS_Content/SubjectData/";

        public static void Initialize()
        {
            RecurringJob.AddOrUpdate("Check for new books",() => FileCheck(), Cron.Daily);
        }
        
        [DisableConcurrentExecution(600)]
        public static void FileCheck()
        {
            // this function needs ultra-robust error checking

            Amos.Models.
            ApplicationDbContext db = new ApplicationDbContext();

            // Check for books waiting to be imported
            var existingBooks = db.Books.Select(x => x.FileName).ToList();

            string sourceDir = ImportBookDirectory;

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

                sb.AppendLine("Book: " + book.Name);
                sb.AppendLine("");

                sb.AppendLine("Quiz Responses");
                sb.AppendLine("User, Question, User Answer, Correct Answer, Time Answered");

                dynamic ob = JsonConvert.DeserializeObject(tracker.TrackerContent);
                foreach (var quiz in ob.QuizResponses)
                {
                    sb.AppendFormat("\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\"" + Environment.NewLine, tracker.Email, quiz.Question, quiz.UserAnswer, quiz.CorrectAnswer, quiz.Time);
                }

                sb.AppendLine("");
                sb.AppendLine("Survey Responses");
                sb.AppendLine("User, Question, User Answer, Comments, Time Answered");
                
                foreach (var survey in ob.SurveyResponses)
                {
                    sb.AppendFormat("\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\"" + Environment.NewLine, tracker.Email, survey.Question, survey.UserAnswer.value, survey.UserAnswer.comments, survey.Time);
                }

                sb.AppendLine("");
                sb.AppendLine("Activity Tracker");
                sb.AppendLine("User, To, From, Description, Time");

                foreach (var activity in ob.ActivityTracking)
                {
                    sb.AppendFormat("\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\"" + Environment.NewLine, tracker.Email, activity.to, activity.from, activity.description, activity.time);
                }

                // create file with sb.tostring as data
                try
                {
                    File.WriteAllText(ExportSubjectDataDirectory + DateTime.Now.Ticks + "_export.csv", sb.ToString());
                    ScheduledJobTracker log = new ScheduledJobTracker();
                    log.Action = "Exported subject data";
                    log.ExecutionTime = DateTime.Now;
                    log.ExtraData = "File: " + ExportSubjectDataDirectory + DateTime.Now.Ticks + "_export.csv";
                    db.ScheduledJobTrackers.Add(log);
                }
                catch (Exception e)
                {
                    ScheduledJobTracker log = new ScheduledJobTracker();
                    log.Action = "UNABLE to export data";
                    log.ExecutionTime = DateTime.Now;
                    log.ExtraData = "File: " + ExportSubjectDataDirectory + DateTime.Now.Ticks + "_export.csv. " + e.InnerException;
                    db.ScheduledJobTrackers.Add(log);
                }

                // set the "exported" bit to true
                tracker.Exported = true;
                db.SaveChanges();

                // sleeping here prevents files from having identical names
                System.Threading.Thread.Sleep(50);
            }
        }
    }
}