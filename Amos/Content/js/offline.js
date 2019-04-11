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
    csv += "User\tModule\tQuestion\tUser Answer\tCorrect Answer\tIs Correct?\tDate Answered\tTime Answered\n";
    for (i = 0; i < UserTracker.QuizResponses.length; i++) {
        csv += "\"" + UserTracker.Email + "\"\t" +
            "\"" + UserTracker.QuizResponses[i].Module + "\"\t" +
            "\"" + UserTracker.QuizResponses[i].Question + "\"\t" +
            "\"" + UserTracker.QuizResponses[i].UserAnswer + "\"\t" +
            "\"" + UserTracker.QuizResponses[i].CorrectAnswer + "\"\t";
        if (UserTracker.QuizResponses[i].UserAnswer === UserTracker.QuizResponses[i].CorrectAnswer) {
            csv += "True\t";
        } else {
            csv += "False\t";
        }

        // date
        csv += UserTracker.QuizResponses[i].Time.getMonth() + "/" + UserTracker.QuizResponses[i].Time.getDate() + "/" + UserTracker.QuizResponses[i].Time.getFullYear() + "\t";
        //time
        csv += UserTracker.QuizResponses[i].Time.getUTCHours() + ":" + UserTracker.QuizResponses[i].Time.getUTCMinutes() + ":" + UserTracker.QuizResponses[i].Time.getUTCSeconds() + "\n";
    }

    csv += "\nSurvey Responses\n";
    csv += "User\tModule\tQuestion\tUser Answer\tComments\tDate Answered\tTime Answered\n";
    for (i = 0; i < UserTracker.SurveyResponses.length; i++) {
        csv += "\"" + UserTracker.Email + "\"\t" +
            "\"" + UserTracker.SurveyResponses[i].Module + "\"\t" +
            "\"" + UserTracker.SurveyResponses[i].Question + "\"\t" +
            "\"" + UserTracker.SurveyResponses[i].UserAnswer.value + "\"\t" +
            "\"" + UserTracker.SurveyResponses[i].UserAnswer.comments + "\"\t";

        // date
        csv += UserTracker.SurveyResponses[i].Time.getMonth() + "/" + UserTracker.SurveyResponses[i].Time.getDate() + "/" + UserTracker.SurveyResponses[i].Time.getFullYear() + "\t";
        //time
        csv += UserTracker.SurveyResponses[i].Time.getUTCHours() + ":" + UserTracker.SurveyResponses[i].Time.getUTCMinutes() + ":" + UserTracker.SurveyResponses[i].Time.getUTCSeconds() + "\n";
    }

    csv += "\nActivity Tracker\n";
    csv += "User\tTo\tFrom\tDescription\Date\tTime\n";
    for (i = 0; i < UserTracker.ActivityTracking.length; i++) {
        csv += "\"" + UserTracker.Email + "\"\t" +
            "\"" + UserTracker.ActivityTracking[i].to + "\"\t" +
            "\"" + UserTracker.ActivityTracking[i].from + "\"\t" +
            "\"" + UserTracker.ActivityTracking[i].description + "\"\t";

        // date
        csv += UserTracker.ActivityTracking[i].time.getMonth() + "/" + UserTracker.ActivityTracking[i].time.getDate() + "/" + UserTracker.ActivityTracking[i].time.getFullYear() + "\t";
        //time
        csv += UserTracker.ActivityTracking[i].time.getUTCHours() + ":" + UserTracker.ActivityTracking[i].time.getUTCMinutes() + ":" + UserTracker.ActivityTracking[i].time.getUTCSeconds() + "\n";

    }

    var hiddenElement = document.createElement('a');
    hiddenElement.href = 'data:text/csv;charset=utf-8,' + encodeURI(csv);
    hiddenElement.target = '_blank';
    hiddenElement.download = 'export.csv';
    hiddenElement.click();

    openDialog("A file containing the user data has been exported. Feel free to close the browser window to complete the training.", "Thank you!");
}