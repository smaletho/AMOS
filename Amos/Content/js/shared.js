function blankTextNode(element) {
    // This should never happen.
    //  It would be XML outsite of an element
    //  I had to check several times, so I moved it here
    //
    //  ex:     <chapter>
    //              <page>
    //                  <text>Good text here</text>
    //                  Bad text here
    //              </page>
    //          </chapter>
    //
    //  return true --> BAD
    //  return false --> GOOD



    if (element.nodeType === 3) {
        var trimmed = $.trim(element.wholeText);
        if (trimmed === "") {
            // it's actually pure text outside of 
            //  I'm pretty sure we'll never hit here
            return true;
        }
    }
    return false;
}



function maintainAspectRatio() {
    // the "17" is due to padding and stuff
    var maxHeight = window.innerHeight - 18;
    var maxWidth = window.innerWidth - 18;
    var srcHeight = $("#main-window").height();
    var srcWidth = $("#main-window").width();

    ScaleRatio = calculateAspectRatioFit(srcWidth, srcHeight, maxWidth, maxHeight);
    console.log("Set scale", ScaleRatio);

    $("#main-window").css({
        '-webkit-transform': 'scale(' + ScaleRatio + ')',
        '-moz-transform': 'scale(' + ScaleRatio + ')',
        '-ms-transform': 'scale(' + ScaleRatio + ')',
        '-o-transform': 'scale(' + ScaleRatio + ')',
        'transform': 'scale(' + ScaleRatio + ')'
    });

    $("#main-window").css('margin-left', '0');

}


function calculateAspectRatioFit(srcWidth, srcHeight, maxWidth, maxHeight) {

    return Math.min(maxWidth / srcWidth, maxHeight / srcHeight);
}



function transmitAction(url, successCallback, errorCallback, returnDataType, data, synchronous) {
    $.ajax({
        url: url,
        data: JSON.stringify(data),
        type: 'POST',
        dataType: returnDataType,
        contentType: "application/json",
        success: successCallback,
        error: errorCallback,
        async: synchronous
    });
}



function updateLoadingText(text) {
    $("#loading-text").text(text);
}




function openNav() {
    if ($("#toc").width() === 0) {
        $("#toc").css('width', '20%');
        $("#menu-open").css('margin-left', '20%');
    } else {
        closeNav();
    }
}

function closeNav() {
    $("#toc").width(0);
    $("#menu-open").css('margin-left', '-1px');
}




function openDialog(text, title) {
    $("#dialog").dialog("option", "title", title);
    $("#dialog-content").text(text);
    $("#dialog").dialog('open');
}

function closeDialog() {
    $("#dialog").dialog("option", "title", '');
    $("#dialog-content").empty();
    $("#dialog").dialog('close');
}

function openSubjectDialog() {
    $("#subject-login").keypress(function (e) {
        if (e.keyCode === $.ui.keyCode.ENTER) {
            subjectLogin();
        }
    });
    $("#subject-login").dialog('open');
}

function closeSubjectDialog() {
    $("#subject-email").val('');
    $("#subject-login").dialog('close');
}

function openConfirmationDialog(text, callback, cancelCallback) {
    $("#confirm-dialog").dialog("option", "buttons", [
        {
            text: "Okay",
            click: function () {
                $(this).dialog("close");
                callback();
            }
        },
        {
            text: "Cancel",
            click: function () {
                $(this).dialog("close");
                if (cancelCallback && typeof cancelCallback === "function") {
                    cancelCallback();
                }
            }
        }
    ]);

    $("#confirm-dialog-content").text(text);
    $("#confirm-dialog").dialog('open');
}





function getBookId() {
    var book = $(ConfigXml).find('book').first();
    try {
        return book[0].attributes["id"].value;
    } catch (e) {
        return 0;
    }
}

function exitBook(str) {
    // open confirmation
    if (typeof str === "undefined" || str === "") str = "Would you like to exit this book?";

    openConfirmationDialog(str, function () {
        UserTracker.ExitTime = new Date();
        saveTracker();

        if (applicationMode === "offline") {
            displayUserData();
        } else {
            window.localStorage.clear();
            window.location = URL_GoHome;
        }
    });
}

function openModalPopup() {
    $("#full-modal-content").empty();
    $("#full-modal-content").css('background-image', 'none');
    $("#full-modal").show();
}