
var resizeTimer;
var idleTime = 0;
var secondIdleTime = 0;
var applicationMode = "online";

$(function () {
    firstLoadInit();

    //Increment the idle time counter every minute.
    var idleInterval = setInterval(timerIncrement, 60000); // 1 minute

    //Zero the idle timer on mouse movement.
    $("body").mousemove(function (e) {
        idleTime = 0;
        secondIdleTime = 0;
    });
    // This part is in navigation.js
    //$(document).keydown(function (e) {
    //    idleTime = 0;
    //    console.log('timer reset');
    //});

    // If location.hostname == "", it means that the app is running as a "file" not on a server
    //  therefore, we're offline, so do that load instead
    if (location.hostname === "") {
        // when this function finishes, it will call the remaining init
        applicationMode = "offline";
        offlineLoadInit();
    } else if (location.pathname.indexOf("Build") !== -1) {
        // using the page viewer.
        applicationMode = "viewer";
        viewerInit();
    } else {
        applicationMode = "online";
        //load the book
        $("#loading").show();

        updateLoadingText('Loading file');
        //loading
        transmitAction(URL_LoadBook, gotBookInfo, fail_BookInfo, "", null, true);
    }
});

function timerIncrement() {
    idleTime = idleTime + 1;

    if (idleTime > 9 && $(".ui-dialog").not(":visible")) { 
        idleTime = 0;
        exitBook("You have been inactive for 10 minutes. Would you like to exit?");
        secondIdleTime = 0;
        var secondIdleInterval = setInterval(function () {
            secondIdleTime = secondIdleTime + 1;

            if (secondIdleTime > 5 && $(".ui-dialog").not(":visible")) {
                UserTracker.ExitTime = new Date();
                saveTracker();

                if (applicationMode === "offline") {
                    displayUserData();
                } else {
                    window.localStorage.clear();
                    window.location = URL_GoHome;
                }
            }

        }, 60000);
    }
}

function gotBookInfo(data) {

    updateLoadingText('Creating presentation');

    ConfigXml = $.parseXML(data.ConfigXml);

    for (var i = 0; i < data.PageContent.length; i++) {
        var tempXml = data.PageContent[i].content;
        data.PageContent[i].content = $($.parseXML(tempXml)).contents();
    }

    PageContent = data.PageContent;
    secondInit();
}

function fail_BookInfo(data) {
    console.log('fail', data);
    updateLoadingText("Failed to load :(");
}

function secondInit() {
    updateLoadingText("Building content");
    buildTableOfContents();
    initTocLinks();
    

    $("#dialog").dialog({
        position: { my: "center top", at: "center top+100", of: window },
        autoOpen: false,
        height: "auto",
        width: 500,
        modal: true,
        close: closeDialog,
        closeText: "",
        buttons: [{
            text: "Okay",
            click: function () {
                $(this).dialog("close");
            }
        }]
    });

    $("#popup").dialog({
        position: { my: "center top", at: "center top+100", of: window },
        autoOpen: false,
        height: "auto",
        width: "auto",
        modal: true,
        close: function () {
            $("#popup-content").empty();
            $("#popup-text").empty();
        },
        open: function () {
            calculateAspectRatioFit();
            $(".ui-dialog").css('transform-origin', 'top left');
            $(".ui-dialog").css({
                '-webkit-transform': 'scale(' + ScaleRatio + ')',
                '-moz-transform': 'scale(' + ScaleRatio + ')',
                '-ms-transform': 'scale(' + ScaleRatio + ')',
                '-o-transform': 'scale(' + ScaleRatio + ')',
                'transform': 'scale(' + ScaleRatio + ')'
            });
            $(".ui-dialog-titlebar").hide();
            $('.ui-widget-overlay').bind('click', function () {
                $("#popup").dialog('close');
            });

            
        },
        closeText: "",
        buttons: [{
            text: "Close",
            click: function () {
                $(this).dialog("close");
            }
        }]
    });

    $("#subject-login").dialog({
        position: { my: "center top", at: "center top+100", of: window },
        autoOpen: false,
        height: "auto",
        width: 500,
        modal: true,
        closeText: "",
        title: "Login",
        closeOnEscape: false,
        open: function (event, ui) {
            $(".ui-dialog-titlebar-close", ui.dialog | ui).hide();
        },
        buttons: [{
            text: "Submit",
            click: subjectLogin
        }]
    });

    $("#confirm-dialog").dialog({
        position: { my: "center top", at: "center top+100", of: window },
        title: "Confirm",
        autoOpen: false,
        height: "auto",
        width: 500,
        modal: true,
        closeOnEscape: false,
        open: function (event, ui) {
            $(".ui-dialog-titlebar-close", ui.dialog | ui).hide();
        },
        closeText: ""
    });



    initTracking();
    
}

function thirdInit() {
    updateLoadingText('Done!');
    setTimeout(function () {
        updateLoadingText('');
        $("#loading").hide();
        $("#main-window").css('display', 'table');

        // find the first page, and load it
        loadPage();
    }, 1000);
}

function firstLoadInit() {
    console.log('base.js loaded');


    maintainAspectRatio();

    $(window).on('resize', function (e) {
        clearTimeout(resizeTimer);
        resizeTimer = setTimeout(function () {
            maintainAspectRatio();
        }, 1000);
    });
    
    $("#exit-button").on('click', function () { exitBook(); });
    $("#help-button").on('click', showInitialHelp);

    $("#menu-close").on('click', closeNav);
    $("#menu-open").on('click', openNav);
    $("#full-modal-close").on('click', function () {
        $("#full-modal").hide();
    });
    initNavigation();
}

