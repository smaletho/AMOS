function offlineLoadInit() {

    $("#loading").show();

    updateLoadingText('Loading file');
    ConfigXml = offline_ConfigXml;

    processOfflinePageContents();
}


function processOfflinePageContents() {
    // offline_PageContents is a string.
    // first, parse it into JSON
    PageContent = JSON.parse(offline_PageContents);

    // call to config2.js
    //  fills offline_PageGuts
    loadGuts();

    if (PageContent.length !== offline_PageGuts.length) {
        alert("Fail!");
        return false;
    } else {
        // now loop through offline_PageGuts and update PageContent.content
        for (var i = 0; i < PageContent.length; i++) {
            PageContent[i].content = offline_PageGuts[i];
        }

        updateLoadingText('Finished processing book');

        secondInit();

        //setTimeout(function () {
        //    updateLoadingText('');
        //    $("#loading").hide();
        //    $("#main-window").css('display', 'table');

        //    secondInit();
        //}, 1000);
    }

}

function displayUserData() {
    //  Exporting subject data when using offline version

    var csv = "";
    var i = 0;

    // Quiz responses
    csv += "Quiz Responses\n";
    csv += "User, Question, User Answer, Correct Answer, Time Answered\n";
    for (i = 0; i < UserTracker.QuizResponses.length; i++) {
        csv += "\"" + UserTracker.Email + "\", " +
            "\"" + UserTracker.QuizResponses[i].Question + "\", " +
            "\"" + UserTracker.QuizResponses[i].UserAnswer + "\", " +
            "\"" + UserTracker.QuizResponses[i].CorrectAnswer + "\", " +
            "\"" + UserTracker.QuizResponses[i].Time + "\"\n";
    }

    csv += "\nSurvey Responses\n";
    csv += "User, Question, Time Answered, User Answer, Comments\n";
    for (i = 0; i < UserTracker.SurveyResponses.length; i++) {
        csv += UserTracker.Email + ", " +
            "\"" + UserTracker.SurveyResponses[i].Question + "\", " +
            "\"" + UserTracker.SurveyResponses[i].Time + "\", " +
            "\"" + UserTracker.SurveyResponses[i].UserAnswer.value + "\", " +
            "\"" + UserTracker.SurveyResponses[i].UserAnswer.comments + "\"\n";
    }

    csv += "\nActivity Tracker\n";
    csv += "User, To, From, Description, Time\n";
    for (i = 0; i < UserTracker.ActivityTracking.length; i++) {
        csv += UserTracker.Email + ", " +
            "\"" + UserTracker.ActivityTracking[i].to + "\", " +
            "\"" + UserTracker.ActivityTracking[i].from + "\", " +
            "\"" + UserTracker.ActivityTracking[i].description + "\", " +
            "\"" + UserTracker.ActivityTracking[i].time + "\"\n";
    }

    var hiddenElement = document.createElement('a');
    hiddenElement.href = 'data:text/csv;charset=utf-8,' + encodeURI(csv);
    hiddenElement.target = '_blank';
    hiddenElement.download = 'export.csv';
    hiddenElement.click();
}