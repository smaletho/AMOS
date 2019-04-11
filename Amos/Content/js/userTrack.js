var UserTracker = {
    Email: null,
    CurrentLocation: null,
    QuizResponses: [],
    SurveyResponses: [],
    StartTime: null,
    EndTime: null,
    ActivityTracking: [],
    VisitedPages: []
};

var modulesVisited = [];

function initTracking() {

    if (applicationMode !== "viewer") {
        
        setInterval(saveTracker, 30000);

        openSubjectDialog();
    } else {
        thirdInit();
    }
}

function subjectLogin() {
    var email = $("#subject-email").val().trim().toLowerCase();
    if (email === null || email === "") {
        openDialog("Please enter your email address.", "Error");
    } else {
        UserTracker.Email = email;
        UserTracker.StartTime = new Date();

        // check local storage on this machine
        var existingData = window.localStorage.getItem(email);

        if (existingData === null) {
            // nothing was found on this machine, if online, check the server
            if (applicationMode === "online") {
                checkOnlineTracking(email);
            } else {
                // running offline, and no matching email was found.
                // start fresh

                UserTracker.CurrentLocation = CurrentLocation;
                UserTracker.Email = email;
                UserTracker.StartTime = new Date();
                closeSubjectDialog();
                thirdInit();
            }
        } else {
            // check if existing data is for the same book we're viewing now
            var tempTracker = JSON.parse(existingData);
            if (tempTracker.CurrentLocation !== null) {
                if (getBookId() === tempTracker.CurrentLocation.Book) {
                    // same book. 
                    openConfirmationDialog("I found a matching user on this machine. Would you like to pick up where you left off, or start a new session? Click 'Yes' to continue, or 'No' to start over.", function () {
                        UserTracker = JSON.parse(existingData);
                        closeSubjectDialog();
                        thirdInit();
                    }, function () {
                        window.localStorage.clear();
                        closeSubjectDialog();
                        thirdInit();
                        });
                    
                } else {
                    // not the same book. remove the object
                    window.localStorage.clear();
                    closeSubjectDialog();
                    thirdInit();
                }
            } else {
                // something is off, start over
                window.localStorage.clear();
                closeSubjectDialog();
                thirdInit();
            }

        }
    }
}


function getSurveyAnswer(question, module) {
    if (UserTracker.SurveyResponses === null) UserTracker.SurveyResponses = [];
    for (var i = 0; i < UserTracker.SurveyResponses.length; i++) {
        if (UserTracker.SurveyResponses[i].Question === question && UserTracker.SurveyResponses[i].Module === getModuleNameFromId(module)) {
            return UserTracker.SurveyResponses[i].UserAnswer;
        }
    }
    return "";
}

function addSurveyAnswer(question, userAnswer) {
    if (UserTracker.QuizResponses === null) UserTracker.QuizResponses = [];
    UserTracker.SurveyResponses.push({
        Question: question,
        UserAnswer: userAnswer,
        Time: new Date(),
        Module: getModuleNameFromId(UserTracker.CurrentLocation.Module)
    });
    saveTracker();
}

function getQuizAnswer(question, module) {
    if (UserTracker.QuizResponses === null) UserTracker.QuizResponses = [];
    for (var i = 0; i < UserTracker.QuizResponses.length; i++) {
        if (UserTracker.QuizResponses[i].Question === question && UserTracker.QuizResponses[i].Module === getModuleNameFromId(module)) {
            return UserTracker.QuizResponses[i];
        }
    }
    return "";
}

function addQuizAnswer(question, userAnswer, correctAnswer) {
    if (UserTracker.QuizResponses === null) UserTracker.QuizResponses = [];
    UserTracker.QuizResponses.push({
        Question: question,
        UserAnswer: userAnswer,
        CorrectAnswer: correctAnswer,
        Time: new Date(),
        Module: getModuleNameFromId(UserTracker.CurrentLocation.Module)
    });
    saveTracker();
}


function checkOnlineTracking(email) {
    // check online
    transmitAction(URL_LoadTracker, checkOnlineTracking_success, checkOnlineTracking_fail, "json", { email: email, id: getBookId() }, false);
}

function checkOnlineTracking_success(data) {
    if (data !== "none") {
        openConfirmationDialog("I found a matching user on this machine. Would you like to pick up where you left off, or start a new session? Click 'Yes' to continue, or 'No' to start over.", function () {
            UserTracker = JSON.parse(data);
            closeSubjectDialog();
            thirdInit();
        }, function () {
            window.localStorage.clear();
            closeSubjectDialog();
            thirdInit();
        });
    } else {
        window.localStorage.clear();
        closeSubjectDialog();
        thirdInit();
    }
        
}

function checkOnlineTracking_fail(data) {
    openDialog("Failed to receive user tracking info.", "Error");
    console.log("checkOnlineTracking_fail()", data);
}

function UpdateCurrentLocation(loc) {
    UserTracker.CurrentLocation = loc;
}

function addUserActionLog(description) {

    var pageDes = getPageTitleFromId(UserTracker.CurrentLocation.Page);
    
    var navOb = {
        to: "",
        from: pageDes,
        description: description,
        time: new Date()
    };
    UserTracker.ActivityTracking.push(navOb);
    saveTracker();
}

function addUserNavigation(from, to, how) {
    
    var f = "";
    if (typeof from === "undefined") {
        f = "start";
        how = "first load";
    } else {
        f = getPageTitleFromId(from.Page);
    }

    var t = getPageTitleFromId(to.Page);
    var dt;

    if (modulesVisited.indexOf(to.Module) === -1)
        modulesVisited.push(to.Module);

    var navOb = {
        to: t,
        from: f,
        description: how,
        time: new Date()
    };
    UserTracker.ActivityTracking.push(navOb);

    if (UserTracker.VisitedPages.indexOf(to.Page) === -1) 
        UserTracker.VisitedPages.push(to.Page);
    
    UpdateCurrentLocation(to);
    updateNavigation(from);

    saveTracker();
}

function saveTracker() {
    if (applicationMode !== "offline")
        saveUserTracking();
    window.localStorage.setItem(UserTracker.Email, JSON.stringify(UserTracker));
}

function saveUserTracking() {
    transmitAction(URL_SaveTracker, function () {
        console.log('saved tracker at: ' + new Date());
    }, null, "",
        {
            BookId: getBookId(),
            email: UserTracker.Email,
            UserTracker: JSON.stringify(UserTracker)
        }, true);
}

function getPageTitleFromId(id) {
    var item = $(ConfigXml).find("#" + id).first();
    return item[0].getAttribute("title");
}

function getModuleNameFromId(id) {
    try {
        var item = $(ConfigXml).find("#" + id).first();
        return item[0].getAttribute("name");
    } catch (e) { return ""; }
}