namespace Amos.Migrations
{
    using Amos.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Amos.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(Amos.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "Admin" };

                manager.Create(role);
            }

            if (!context.Users.Any(u => u.UserName == "tsmale@rktcreative.com"))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var user = new ApplicationUser { UserName = "tsmale@rktcreative.com" };

                manager.Create(user, "Secret123!");
                manager.AddToRole(user.Id, "Admin");
            }

            if (!context.Users.Any(u => u.UserName == "rkinnen@rktcreative.com"))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var user = new ApplicationUser { UserName = "rkinnen@rktcreative.com" };

                manager.Create(user, "Secret123!");
                manager.AddToRole(user.Id, "Admin");
            }

            if (!context.Users.Any(u => u.UserName == "kcomerford@rktcreative.com"))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var user = new ApplicationUser { UserName = "kcomerford@rktcreative.com" };

                manager.Create(user, "Secret123!");
                manager.AddToRole(user.Id, "Admin");
            }

            if (!context.Users.Any(u => u.UserName == "admin@kbrwyle.com"))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var user = new ApplicationUser { UserName = "admin@kbrwyle.com" };

                manager.Create(user, "Secret123!");
                manager.AddToRole(user.Id, "Admin");
            }

            //context.Books.AddOrUpdate(x => x.BookId,
            //    new Book() { BookId = 1, CreateDate = DateTime.Now, ModifiedBy = "initial", CreatedBy = "initial", ModifyDate = DateTime.Now, Name = "COMfORT v1", Published = true, Version = "1" },
            //    new Book() { BookId = 2, CreateDate = new DateTime(1997, 6, 27), ModifiedBy = "J.K. Rowling", CreatedBy = "J.K. Rowling", ModifyDate = new DateTime(1998, 9, 1), Name = "Harry Potter and the Sorcerer's Stone", Published = true, Version = "1" },
            //    new Book() { BookId = 3, CreateDate = new DateTime(1996, 1, 1), ModifiedBy = "Shel Silverstein", CreatedBy = "Shel Silverstein", ModifyDate = new DateTime(1996, 1, 1), Name = "Falling Up", Published = true, Version = "1" },
            //    new Book() { BookId = 4, CreateDate = new DateTime(1961, 1, 1), ModifiedBy = "Norton Juster", CreatedBy = "Norton Juster", ModifyDate = new DateTime(1961, 1, 1), Name = "The Phantom Tollbooth", Published = true, Version = "1" },
            //    new Book() { BookId = 5, CreateDate = new DateTime(1998, 8, 20), ModifiedBy = "Louis Sachar", CreatedBy = "Louis Sachar", ModifyDate = new DateTime(1998, 8, 20), Name = "Holes", Published = true, Version = "1" },
            //    new Book() { BookId = 6, CreateDate = new DateTime(1990, 1, 22), ModifiedBy = "Dr. Suess", CreatedBy = "Dr. Suess", ModifyDate = new DateTime(1990, 1, 22), Name = "Oh The Places You'll Go", Published = true, Version = "1" },
            //    new Book() { BookId = 7, CreateDate = new DateTime(1969, 6, 3), ModifiedBy = "Eric Carle", CreatedBy = "Eric Carle", ModifyDate = new DateTime(1969, 6, 3), Name = "The Very Hungry Caterpillar", Published = true, Version = "1" },
            //    new Book() { BookId = 8, CreateDate = new DateTime(1992, 7, 1), ModifiedBy = "R.L. Stine", CreatedBy = "R.L. Stine", ModifyDate = new DateTime(1992, 7, 1), Name = "Goosebumps", Published = true, Version = "1" }
            //    );

            //context.Modules.AddOrUpdate(x => x.ModuleId,
            //    new Module() { ModuleId = 1, BookId = 1, Name = "Kidney & Bladder", SortOrder = 10, Theme = "3" }
            //    );

            //for (int i = 1; i < 7; i++)
            //{
            //    string name = "";
            //    switch (i)
            //    {
            //        case 1:
            //            name = "Objective";
            //            break;
            //        case 2:
            //            name = "Foundation";
            //            break;
            //        case 3:
            //            name = "Setup";
            //            break;
            //        case 4:
            //            name = "Exam Procedure";
            //            break;
            //        case 5:
            //            name = "Quiz";
            //            break;
            //        case 6:
            //            name = "Reference";
            //            break;
            //    }
            //    context.Sections.AddOrUpdate(x => x.SectionId,
            //        new Section() { SectionId = i, ModuleId = 1, Name = name, SortOrder = (i * 10) }
            //        );
            //}

            //context.Chapters.AddOrUpdate(x => x.ChapterId,
            //    new Chapter() { ChapterId = 1, Name = "Overview", SectionId = 1, SortOrder = 10 },
            //    new Chapter() { ChapterId = 2, Name = "Probe Handling", SectionId = 2, SortOrder = 10 },
            //    new Chapter() { ChapterId = 3, Name = "Concepts", SectionId = 2, SortOrder = 20 },
            //    new Chapter() { ChapterId = 4, Name = "Imaging", SectionId = 2, SortOrder = 30 },
            //    new Chapter() { ChapterId = 5, Name = "Normal Kidney", SectionId = 2, SortOrder = 40 },
            //    new Chapter() { ChapterId = 6, Name = "Normal Bladder", SectionId = 2, SortOrder = 50 },
            //    new Chapter() { ChapterId = 7, Name = "Pathology", SectionId = 2, SortOrder = 60 },
            //    new Chapter() { ChapterId = 8, Name = "Ultrasound Machine", SectionId = 3, SortOrder = 10 },
            //    new Chapter() { ChapterId = 9, Name = "Room & Subject", SectionId = 3, SortOrder = 20 },
            //    new Chapter() { ChapterId = 10, Name = "Right Kidney", SectionId = 4, SortOrder = 10 },
            //    new Chapter() { ChapterId = 11, Name = "Left Kidney", SectionId = 4, SortOrder = 20 },
            //    new Chapter() { ChapterId = 12, Name = "Bladder", SectionId = 4, SortOrder = 30 },
            //    new Chapter() { ChapterId = 13, Name = "Quiz", SectionId = 5, SortOrder = 10 },
            //    new Chapter() { ChapterId = 14, Name = "Scanning Reference", SectionId = 6, SortOrder = 10 },
            //    new Chapter() { ChapterId = 15, Name = "Troubleshooting", SectionId = 6, SortOrder = 20 }
            //    );



            //// 9 real pages
            //context.Pages.AddOrUpdate(x => x.PageId,
            //    new Page() { PageId = 1, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 1", Type = "content", PageContent = p1, BookId = 1, ChapterId = 1, SortOrder = 10 },
            //    new Page() { PageId = 2, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 2", Type = "content", PageContent = p2, BookId = 1, ChapterId = 2, SortOrder = 10 },
            //    new Page() { PageId = 3, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 3", Type = "content", PageContent = p3, BookId = 1, ChapterId = 2, SortOrder = 20 },
            //    new Page() { PageId = 4, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 4", Type = "content", PageContent = p4, BookId = 1, ChapterId = 2, SortOrder = 30 },
            //    new Page() { PageId = 5, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 5", Type = "content", PageContent = p5, BookId = 1, ChapterId = 2, SortOrder = 40 },
            //    new Page() { PageId = 6, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 6", Type = "content", PageContent = p6, BookId = 1, ChapterId = 3, SortOrder = 10 },
            //    new Page() { PageId = 7, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 7", Type = "content", PageContent = p7, BookId = 1, ChapterId = 3, SortOrder = 20 },
            //    new Page() { PageId = 8, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 8", Type = "content", PageContent = p8, BookId = 1, ChapterId = 3, SortOrder = 30 },
            //    new Page() { PageId = 9, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 9", Type = "content", PageContent = p9, BookId = 1, ChapterId = 4, SortOrder = 10 }
            //    );

            //context.Pages.AddOrUpdate(x => x.PageId,
            //    new Page() { PageId = 10, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 10", Type = "quiz", PageContent = p10, BookId = 1, ChapterId = 5, SortOrder = 10 },
            //    new Page() { PageId = 11, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 11", Type = "survey", PageContent = p11, BookId = 1, ChapterId = 6, SortOrder = 10 },
            //    new Page() { PageId = 12, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 12", Type = "content", PageContent = string.Format(genericText, "p_12", "12"), BookId = 1, ChapterId = 7, SortOrder = 10 },
            //    new Page() { PageId = 13, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 13", Type = "content", PageContent = string.Format(genericText, "p_13", "13"), BookId = 1, ChapterId = 7, SortOrder = 20 },
            //    new Page() { PageId = 14, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 14", Type = "content", PageContent = string.Format(genericText, "p_14", "14"), BookId = 1, ChapterId = 7, SortOrder = 30 },
            //    new Page() { PageId = 15, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 15", Type = "content", PageContent = string.Format(genericText, "p_15", "15"), BookId = 1, ChapterId = 7, SortOrder = 40 },
            //    new Page() { PageId = 16, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 16", Type = "content", PageContent = string.Format(genericText, "p_16", "16"), BookId = 1, ChapterId = 8, SortOrder = 10 },
            //    new Page() { PageId = 17, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 17", Type = "content", PageContent = string.Format(genericText, "p_17", "17"), BookId = 1, ChapterId = 9, SortOrder = 10 },
            //    new Page() { PageId = 18, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 18", Type = "content", PageContent = string.Format(genericText, "p_18", "18"), BookId = 1, ChapterId = 10, SortOrder = 10 },
            //    new Page() { PageId = 19, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 19", Type = "content", PageContent = string.Format(genericText, "p_19", "19"), BookId = 1, ChapterId = 10, SortOrder = 20 },
            //    new Page() { PageId = 20, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 20", Type = "content", PageContent = string.Format(genericText, "p_20", "20"), BookId = 1, ChapterId = 10, SortOrder = 30 },
            //    new Page() { PageId = 21, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 21", Type = "content", PageContent = string.Format(genericText, "p_21", "21"), BookId = 1, ChapterId = 11, SortOrder = 10 },
            //    new Page() { PageId = 22, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 22", Type = "content", PageContent = string.Format(genericText, "p_22", "22"), BookId = 1, ChapterId = 11, SortOrder = 20 },
            //    new Page() { PageId = 23, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 23", Type = "content", PageContent = string.Format(genericText, "p_23", "23"), BookId = 1, ChapterId = 11, SortOrder = 30 },
            //    new Page() { PageId = 24, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 24", Type = "content", PageContent = string.Format(genericText, "p_24", "24"), BookId = 1, ChapterId = 12, SortOrder = 10 },
            //    new Page() { PageId = 25, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 25", Type = "content", PageContent = string.Format(genericText, "p_25", "25"), BookId = 1, ChapterId = 12, SortOrder = 20 },
            //    new Page() { PageId = 26, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 26", Type = "content", PageContent = string.Format(genericText, "p_26", "26"), BookId = 1, ChapterId = 12, SortOrder = 30 },
            //    new Page() { PageId = 27, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 27", Type = "content", PageContent = string.Format(genericText, "p_27", "27"), BookId = 1, ChapterId = 12, SortOrder = 40 },
            //    new Page() { PageId = 28, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 28", Type = "content", PageContent = string.Format(genericText, "p_28", "28"), BookId = 1, ChapterId = 12, SortOrder = 50 },
            //    new Page() { PageId = 29, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 29", Type = "content", PageContent = string.Format(genericText, "p_29", "29"), BookId = 1, ChapterId = 12, SortOrder = 60 },
            //    new Page() { PageId = 30, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 30", Type = "content", PageContent = string.Format(genericText, "p_30", "30"), BookId = 1, ChapterId = 13, SortOrder = 10 },
            //    new Page() { PageId = 31, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 31", Type = "content", PageContent = string.Format(genericText, "p_31", "31"), BookId = 1, ChapterId = 13, SortOrder = 20 },
            //    new Page() { PageId = 32, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 32", Type = "content", PageContent = string.Format(genericText, "p_32", "32"), BookId = 1, ChapterId = 13, SortOrder = 30 },
            //    new Page() { PageId = 33, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 33", Type = "content", PageContent = string.Format(genericText, "p_33", "33"), BookId = 1, ChapterId = 14, SortOrder = 10 },
            //    new Page() { PageId = 34, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 34", Type = "content", PageContent = string.Format(genericText, "p_34", "34"), BookId = 1, ChapterId = 14, SortOrder = 20 },
            //    new Page() { PageId = 35, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 35", Type = "content", PageContent = string.Format(genericText, "p_35", "35"), BookId = 1, ChapterId = 14, SortOrder = 30 },
            //    new Page() { PageId = 36, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 36", Type = "content", PageContent = string.Format(genericText, "p_36", "36"), BookId = 1, ChapterId = 14, SortOrder = 40 },
            //    new Page() { PageId = 37, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 37", Type = "content", PageContent = string.Format(genericText, "p_37", "37"), BookId = 1, ChapterId = 14, SortOrder = 50 },
            //    new Page() { PageId = 38, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 38", Type = "content", PageContent = string.Format(genericText, "p_38", "38"), BookId = 1, ChapterId = 14, SortOrder = 60 },
            //    new Page() { PageId = 39, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 39", Type = "content", PageContent = string.Format(genericText, "p_39", "39"), BookId = 1, ChapterId = 14, SortOrder = 70 },
            //    new Page() { PageId = 40, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 40", Type = "content", PageContent = string.Format(genericText, "p_40", "40"), BookId = 1, ChapterId = 14, SortOrder = 80 },
            //    new Page() { PageId = 41, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 41", Type = "content", PageContent = string.Format(genericText, "p_41", "41"), BookId = 1, ChapterId = 14, SortOrder = 90 },
            //    new Page() { PageId = 42, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 42", Type = "content", PageContent = string.Format(genericText, "p_42", "42"), BookId = 1, ChapterId = 14, SortOrder = 100 },
            //    new Page() { PageId = 43, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 43", Type = "content", PageContent = string.Format(genericText, "p_43", "43"), BookId = 1, ChapterId = 14, SortOrder = 110 },
            //    new Page() { PageId = 44, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 44", Type = "content", PageContent = string.Format(genericText, "p_44", "44"), BookId = 1, ChapterId = 15, SortOrder = 10 },
            //    new Page() { PageId = 45, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 45", Type = "content", PageContent = string.Format(genericText, "p_45", "45"), BookId = 1, ChapterId = 15, SortOrder = 20 },
            //    new Page() { PageId = 46, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 46", Type = "content", PageContent = string.Format(genericText, "p_46", "46"), BookId = 1, ChapterId = 15, SortOrder = 30 },
            //    new Page() { PageId = 47, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 47", Type = "content", PageContent = string.Format(genericText, "p_47", "47"), BookId = 1, ChapterId = 15, SortOrder = 40 },
            //    new Page() { PageId = 48, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 48", Type = "content", PageContent = string.Format(genericText, "p_48", "48"), BookId = 1, ChapterId = 15, SortOrder = 50 },
            //    new Page() { PageId = 49, CreateDate = DateTime.Now, CreatedBy = "initial", ModifiedBy = "initial", ModifyDate = DateTime.Now, Title = "Page 49", Type = "content", PageContent = string.Format(genericText, "p_49", "49"), BookId = 1, ChapterId = 15, SortOrder = 60 }
            //    );
        }


        public static string p1 = "<page id=\"p_1\" type=\"content\"><text class=\"header\" left=\"\" top=\"\">Objective</text><text top=\"70\" width=\"735\"> A crewmember has medical complaints and <a href=\"javascript:void (0)\" class=\"navigateTo\" data-id=\"p_5\">ultrasound</a> images of <a href=\"javascript:void (0)\" class=\"definitionPopup\" data-text=\"Bean-shaped organs at the back of the abdominal cavity in humans, one on each side of the spinal column.They maintain water and electrolyte balance and filter waste products from the blood, which are excreted as urine.\">kidneys</a> and urinary bladder are necessary. <a href=\"javascript:void(0)\">Renal stone</a> with blockage of urine flow is suspected. </text><text top=\"167\" width=\"600\" style=\"font-weight:bold;\"> You will acquire ultrasound images of the right and left kidneys and two views of the urinary <a href=\"javascript:void (0)\">bladder.</a></text><text top=\"300\" width=\"500\"> Would you like to review the concepts and details of the procedure in the FOUNDATION tab? </text><text top=\"380\" width=\"500\"> OR </text><text top=\"440\" width=\"500\"> Go straight to the SETUP for the exam procedure? </text><image top=\"\" left=\"894\" source=\"i_1\" height=\"660\" /><button top=\"310\" left=\"600\" id=\"p_2\">FOUNDATION</button><button top=\"440\" left=\"600\" id=\"p_16\">GO TO SETUP</button></page>";
        public static string p2 = "<page id=\"p_2\" type=\"content\"><text class=\"header\" left=\"50\" top=\"30\">Probe Handling</text><text top=\"80\" left=\"50\" width=\"500\"> The ultrasound image shows a two dimensional slice of tissue under the <a href=\"javascript:void (0)\" class=\"definitionPopup\" data-text=\"popup information here.\">probe</a>. One edge of the probe has a (a raised grey stripe you can see and feel.) <br /><br />The <a href=\"javascript:void (0)\" class=\"definitionPopup\" data-text=\"popup information here.\">reference marker</a> corresponds to the ultrasound screen indicator. <br /><br />The other side of the probe is the non-marker edge. </text><image left=\"600\" source=\"i_4\" width=\"450\" top=\"80\" /><image left=\"1100\" source=\"i_3\" width=\"450\" top=\"80\" /><text top=\"465\" left=\"610\" width=\"900\" style=\"text-align:center;font-family:Fira Mono; font-size:10pt;\"> Placement of probe on subject's back. Indicator pointing toward the head. The resulting image is on the right. </text></page>";
        public static string p3 = "<page id=\"p_3\" type=\"content\"><text class=\"header\" left=\"50\" top=\"30\">Probe Handling: Kidney</text><text top=\"80\" left=\"50\" width=\"500\"> Correct <a href=\"javascript:void (0)\" class=\"definitionPopup\" data-text=\"popup information here.\">probe</a> orientation will ensure that the image is not reversed on the screen. </text><image left=\"600\" source=\"i_31\" width=\"420\" top=\"30\" /><image left=\"1035\" source=\"i_30\" width=\"420\" top=\"30\" /><image left=\"600\" source=\"i_29\" width=\"420\" top=\"360\" /><image left=\"1035\" source=\"i_28\" width=\"420\" top=\"360\" /><text top=\"285\" left=\"300\" width=\"290\" style=\"text-align:left;font-family:Fira Mono; font-size:10pt;\"><b>Horizontal Probe orientation</b>, the <a href=\"javascript:void (0)\" class=\"definitionPopup\" data-text=\"popup information here.\">reference marker</a> is pointed to the subject's right. </text><text top=\"600\" left=\"300\" width=\"290\" style=\"text-align:left;font-family:Fira Mono; font-size:10pt;\"><b>Vertical Probe orientation</b>, the <a href=\"javascript:void (0)\" class=\"definitionPopup\" data-text=\"popup information here.\">reference marker</a> is pointed up toward the subject's </text></page>";
        public static string p4 = "<page id=\"p_4\" type=\"content\"><text class=\"header\" left=\"50\" top=\"30\">Probe Handling: Bladder</text><text top=\"80\" left=\"50\" width=\"500\"> Correct <a href=\"javascript:void (0)\" class=\"definitionPopup\" data-text=\"popup information here.\">probe</a> orientation will ensure that the image is not reversed on the screen. </text><image left=\"600\" source=\"i_44\" width=\"420\" top=\"30\" /><image left=\"1035\" source=\"i_41\" width=\"420\" top=\"30\" /><image left=\"600\" source=\"i_43\" width=\"420\" top=\"360\" /><image left=\"1035\" source=\"i_42\" width=\"420\" top=\"360\" /><text top=\"285\" left=\"300\" width=\"290\" style=\"text-align:left;font-family:Fira Mono; font-size:10pt;\"><b>Horizontal Probe orientation</b>, the <a href=\"javascript:void (0)\" class=\"definitionPopup\" data-text=\"popup information here.\">reference marker</a> is pointed to the subject's right. </text><text top=\"600\" left=\"300\" width=\"290\" style=\"text-align:left;font-family:Fira Mono; font-size:10pt;\"><b>Vertical Probe orientation</b>, the <a href=\"javascript:void (0)\" class=\"definitionPopup\" data-text=\"popup information here.\">reference marker</a> is pointed up toward the subject's </text></page>";
        public static string p5 = "<page id=\"p_5\" type=\"content\"><text class=\"header\" left=\"50\" top=\"30\">Probe Handling</text><text top=\"70\" left=\"700\" width=\"700\"> Ultrasound requires <a href=\"javascript:void(0)\" class=\"definitionPopup\" data-text=\"popup information here.\">gel</a> as an acoustic conductor between the <a href=\"javascript:void(0)\" class=\"definitionPopup\" data-text=\"popup information here.\">probe</a> and the skin. If there is not enough gel, part of the image will be lost (black or artifactual). The probe requires consistent firm pressure to maintain contact with the body. This allows probe contact on the body even while tilting or panning. If your hand gets tired switch hands or use both hands. Hold the probe like a pencil while keeping part of the hand on the patient for stability, similar to a tripod. </text><image left=\"\" source=\"i_14\" width=\"600\" top=\"70\" /><image left=\"700\" source=\"i_13\" height=\"300\" top=\"320\" style=\"border:1px solid #000;\" /><image left=\"1150\" source=\"i_12\" width=\"420\" top=\"320\" style=\"border:1px solid #000;\" /></page>";
        public static string p6 = "<page id=\"p_6\" type=\"content\"><text class=\"header\" left=\"50\" top=\"30\">Concepts and What To Look For In An Image</text><text class=\"header\" left=\"0\" top=\"60\" style=\"background-color:#99ccff;padding: 5px 20px 5px 50px\">Right Kidney</text><text top=\"110\" left=\"50\" width=\"600\"> Watch the ultrasound screen as you control the <a href=\"javascript:void(0)\" class=\"definitionPopup\" data-text=\"popup information here.\">probe</a> to observe the kidney shape on the screen. <br /><br /> On the <b>right kidney</b> initial probe placement is near <b>right purple 2</b> . <br /><br /> Orient the probe to the kidney using small rotation and sliding. Small rotations affect the length and width of the kidney. <br /><br /> The best right kidney image is longest and widest with BOTH poles in view. </text><video type=\"mp4\" top=\"20\" left=\"700\" width=\"600\" height=\"336\" source=\"v_32\" /><text top=\"360\" left=\"700\" width=\"600\" style=\"text-align:left;font-family:Fira Mono;font-size:10pt;\">Press play to start video.</text><image left=\"50\" source=\"i_11\" height=\"340\" top=\"340\" style=\"border:1px solid #000;\" /><image left=\"700\" source=\"i_10\" height=\"300\" top=\"385\" style=\"border:1px solid #000;\" /><image left=\"1150\" source=\"i_9\" width=\"420\" top=\"385\" style=\"border:1px solid #000;\" /></page>";
        public static string p7 = "<page id=\"p_7\" type=\"content\"><text class=\"header\" left=\"50\" top=\"30\">Concepts and What To Look For In An Image</text><text class=\"header\" left=\"0\" top=\"60\" style=\"background-color:#99ccff;padding: 5px 20px 5px 50px\">Left Kidney</text><text top=\"110\" left=\"50\" width=\"600\"> On the <b>Left kidney</b> initial <a href=\"javascript:void(0)\" class=\"definitionPopup\" data-text=\"popup information here.\">probe</a> placement is near <b>left purple 2</b>. <br /><br />Orient the probe to the kidney using small rotation and sliding. Small rotations, <a href=\"javascript:void(0)\" class=\"definitionPopup\" data-text=\"popup information here.\">panning</a>, and <a href=\"javascript:void(0)\" class=\"definitionPopup\" data-text=\"popup information here.\">tilting</a> affect the length and width of the kidney. <br /><br />The best left Kidney image is longest and widest with BOTH <a href=\"javascript:void(0)\" class=\"definitionPopup\" data-text=\"popup information here.\">kidney poles</a> in view. </text><text top=\"20\" left=\"700\" width=\"600\" height=\"336\" style=\"background-color:gray;\"> Video Box </text><text top=\"360\" left=\"700\" width=\"600\" style=\"text-align:left;font-family:Fira Mono;font-size:10pt;\"> Press play to start video. </text><image left=\"50\" source=\"i_24\" height=\"340\" top=\"340\" style=\"border:1px solid #000;\" /><image left=\"700\" source=\"i_15\" height=\"300\" top=\"385\" style=\"border:1px solid #000;\" /><image left=\"1150\" source=\"i_16\" width=\"420\" top=\"385\" style=\"border:1px solid #000;\" /></page>";
        public static string p8 = "<page id=\"p_8\" type=\"content\"><text class=\"header\" left=\"50\" top=\"30\">Concepts and What To Look For In An Image</text><text class=\"header\" left=\"0\" top=\"60\" style=\"background-color:#99ccff;padding: 5px 20px 5px 50px\">Bladder</text><text top=\"110\" left=\"50\" width=\"500\"> The <b>urinary bladder</b> is a stretchable sac with a shape that can be variable. Without a lot of urine it can be rather flat; as urine accumulates it can be a trigonal to a teardrop shaped sac. </text><image left=\"50\" source=\"i_17\" width=\"448\" top=\"274\" /><image left=\"700\" source=\"i_26\" width=\"420\" top=\"30\" /><text top=\"265\" left=\"1130\" width=\"290\" style=\"text-align:left;font-family:Fira Mono;font-size:10pt;\"><b>In the vertical orientation, the reference marker is toward the patient's head. </b> The image tends to be almost square. </text><image left=\"700\" source=\"i_25\" width=\"420\" top=\"360\" /><text top=\"580\" left=\"1130\" width=\"290\" style=\"text-align:left;font-family:Fira Mono;font-size:10pt;\"><b>In the horizontal view the reference marker is towards the patient’s right.</b> The bladder's shape can be teardrop or linear depending on the volume in the sac. </text></page>";
        public static string p9 = "<page id=\"p_9\" type=\"content\"><text class=\"header\" left=\"50\" top=\"30\"> Image Acquisition Basics </text><text top=\"70\" left=\"80\" width=\"400\"> Observe the image screen as you control the ultrasound <a href=\"javascript:void (0)\" class=\"definitionPopup\" data-text=\"popup information here.\">probe</a>. </text><text top=\"150\" left=\"125\" width=\"300\" style=\"text-align:left;font-family:Fira Mono; font-size:10pt;\"><b>(1) </b>Press <a href=\"javascript:void (0)\" class=\"definitionPopup\" data-text=\"popup information here.\"><b>Text</b></a> once, to type onto the ultrasound screen and press Text once more to release. </text><text top=\"250\" left=\"125\" width=\"350\" style=\"text-align:left;font-family:Fira Mono; font-size:10pt;\"><b>(2) </b>Press <a href=\"javascript:void (0)\" class=\"definitionPopup\" data-text=\"popup information here.\"><b>Erase</b></a> once, to remove all text. </text><text top=\"300\" left=\"125\" width=\"300\" style=\"text-align:left;font-family:Fira Mono; font-size:10pt;\"><b>(3) </b><a href=\"javascript:void (0)\" class=\"definitionPopup\" data-text=\"popup information here.\"><b>iSCAN:</b></a><br />Over-brightness can be improved by pressing iSCAN once as the kidney is in view. </text><text top=\"450\" left=\"125\" width=\"350\" style=\"text-align:left;font-family:Fira Mono; font-size:10pt;\"><b>(4) </b><a href=\"javascript:void (0)\" class=\"definitionPopup\" data-text=\"popup information here.\"><b>AQUIRE:</b></a><br /> Stabilize the view and press Acquire to store one short video clip (cine-loop). Pressing the Acquire button digitally saves the images you are seeing. You will hear a beep after the image is saved. </text><text top=\"300\" left=\"1050\" width=\"350\" style=\"text-align:left;font-family:Fira Mono; font-size:10pt;\"><b>(5) </b><a href=\"javascript:void (0)\" class=\"definitionPopup\" data-text=\"popup information here.\"><b>DEPTH</b></a> FOCUS:<br /> Image depth can be increased or decreased using the depth knob. </text><text top=\"400\" left=\"1050\" width=\"350\" style=\"text-align:left;font-family:Fira Mono; font-size:10pt;\"><b>(6) </b><a href=\"javascript:void (0)\" class=\"definitionPopup\" data-text=\"popup information here.\"><b>GAIN:</b></a><br /> Under-bright images can be improved by turning up the gain (clock-wise rotation). </text><text top=\"500\" left=\"1050\" width=\"400\" style=\"text-align:left;font-family:Fira Mono; font-size:10pt;\"><b>(7) Freeze button:</b> when active the button is an amber color and the image moves as you move the ultrasound probe. When engaged the freeze button is bright green and the system is motionless. Press Freeze once to unfreeze and the button will be amber colored. </text><image left=\"500\" source=\"i_27\" width=\"525\" top=\"80\" style=\"border:1px solid #000;\" /><image left=\"1050\" source=\"i_19\" width=\"268\" top=\"25\" /></page>";
        public static string p10 = "<page type=\"quiz\" id=\"p_10\"> <text class=\"header\">Quiz Testing</text> <text class=\"quiz-question\" top=\"100\" left=\"\" answer=\"Tom\">What is my name?</text> <text top=\"140\" left=\"\"><input type=\"radio\" name=\"quiz\" value=\"Rob\" /> Rob <br /><input type=\"radio\" name=\"quiz\" value=\"Tom\" /> Tom <br /><input type=\"radio\" name=\"quiz\" value=\"Jim\" /> Jim </text> <button class=\"quiz-submit\" top=\"400\" left=\"\">Submit</button> <text class=\"post-quiz\" top=\"600\">this pops up after</text> </page>";
        public static string p11 = "<page type=\"survey\" id=\"p_11\"> <text class=\"header\">Survey Testing</text> <text class=\"survey-question\" top=\"100\" left=\"\">Please rate this app on the following scale:</text> <text top=\"140\" left=\"\"> <input type=\"radio\" name=\"survey\" value=\"0\" /> N/A <br /><input type=\"radio\" name=\"survey\" value=\"1\" /> Extremely Poor <br /><input type=\"radio\" name=\"survey\" value=\"2\" /> Very Poor <br /><input type=\"radio\" name=\"survey\" value=\"3\" /> Poor <br /><input type=\"radio\" name=\"survey\" value=\"4\" /> Neutral <br /><input type=\"radio\" name=\"survey\" value=\"5\" /> Good <br /><input type=\"radio\" name=\"survey\" value=\"6\" /> Very Good <br /><input type=\"radio\" name=\"survey\" value=\"7\" /> Excellent </text> <text top=\"340\"> Please provide any additional comments that you may have:<br /> <textarea id=\"survey-comment\"></textarea> </text> <button class=\"survey-submit\" top=\"500\" left=\"\">Submit</button> </page>";

        public static string genericText = "<page id=\"{0}\" type=\"content\"><text class=\"header\">New page {1}</text></page>";

    }
}
