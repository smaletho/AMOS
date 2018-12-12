******************************************
					AMOS
	Autonomous Medical Officer Support
******************************************

Developed by: RKT Creative Solutions and KBRWyle
Last Updated: 12/12/2018

Overview
--------
AMOS (Autonomous Medical Officer Support) was developed as a flexible, interactive content shell, used for displaying Just-In-Time 
training modules. This application serves content (images, video, text) from a web browser to assist in various training experiments.

A "book" refers to a collection of content pages, organized in an outline format, like below:

Book
-Module
--Section
---Chapter
----Page

Each parent (book, module, section, chapter) element must contain at least one or more children.
This is to say that all books must contain at least one module, all modules must contain at least one section,
all sections must contain at least on chapter, and all chapters must contain at least one page.

There are two types of users in this application, Subjects and Administrators.

* Subject Usage
Subjects can access the homepage, the list of published books, and any published book/training module.

Once a subject has selected a book to view, they are to click through the content in a mainly linear fashion.
Hyperlinks, definitions, videos, and images allow the user to get more information on any topics.

All subject avtivity is recorded for later analysis.

* Administrative Usage
Administrators are those with access to the backend of the software. Admins are able to un-publish a book with the purpose of
reorganizing, modifying, editing, deleting, or creating a new book.

@ Note: In this iteration of the project, all page content is developed solely by RKT Creative. The interface with which content
is generated was not designed with general users in mind, and requires in-depth knowledge of the application in order to properly
organize the content.

Once the content pages are created, they can be arranged in any manner by an administrator. Pages can also be removed from a book,
where they will remain as "unlinked" pages to be freely added to any book.


Resources
---------
Below is a list of frameworks and libraries used in this project:
- Bootstrap v3.3 (https://getbootstrap.com/docs/3.3/)
- jQuery v3.3.1 (https://jquery.com/)
- jQuery UI v1.12.1 (https://jqueryui.com/)
- DotNetZip v1.11.0 (https://github.com/haf/DotNetZip.Semverd)
- Bootstrap Tour (http://bootstraptour.com/)
- FontAwesome v4.7.0 (https://fontawesome.com/v4.7.0/)
- .NET v4.6.1
- SQL Server Express


Scheduled Jobs
--------------
There are two tasks which are run through an automated batch file that this application performs. One will search for exported books
in a given directory, while the the other will export all user data.

The file to run is "RunJobs.bat" and can be found in this solution. Executing this script through a job scheduler (like Windows 
Task Scheduler) will send a call to the server, notifying it to check for new books and to dump the users' data into a given directory.

The method the batch file hits is located in the PresentController.cs file. Within the "ProcessScheduledJobs()" method, there are two
static strings, which determine where the application should look for files, and where to dump the user data.



Deployment
----------


Notes
-----
This software is designed to run in most web browsers. Internet Explorer v9+ is supported.
There are issues viewing videos when this content is viewed through Safari.
Chrome and Firefox are entirely supported.

