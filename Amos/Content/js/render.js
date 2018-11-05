


function renderInit() {
    $(".navigateTo").on('click', function () {
        loadPage($(this).data('id'), "page");
        return false;
    });

    $(".definitionPopup").on('click', function () {
        var pos = $(this).offset();


        $("#definitionWindow").css('top', (pos.top - 125) + "px");
        $("#definitionWindow").css('left', pos.left + "px");

        $("#definitionWindow").show();
        $("#definitionWindow").text($(this).data('text'));
        return false;
    });

    $("#page-content").on('click', function () {
        $("#definitionWindow").hide();
        $("#definitionWindow").css('top', '0');
        $("#definitionWindow").css('left', '0');
        $("#definitionWindow").text('');
    });

    if (applicationMode != "viewer") {
        if ($("#page-content").find(".quiz-submit").length != 0) {
            // it's a quiz page
            quizInit();
        }

        if ($("#page-content").find(".survey-submit").length != 0) {
            // it's a survey page
            surveyInit();
        }
    }


}

function okayToNavigateAway() {
    if ($("#page-content").find(".quiz-submit").length != 0) {
        // it's a quiz page
        var question = $(".quiz-question").first().text();
        var answer = getQuizAnswer(question);
        if (answer == "") {
            openDialog("Please answer the quiz question before navigating to a different page.", "Wait");
            return false;
        } else return true;
    }

    else if ($("#page-content").find(".survey-submit").length != 0) {
        // it's a survey page
        var question = $(".survey-question").first().text();
        var answer = getSurveyAnswer(question);
        if (answer == "") {
            openDialog("Please answer the survey question before navigating to a different page.", "Wait");
            return false;
        } else return true;
    }

    else return true;
}

function surveyInit() {


    var question = $(".survey-question").first().text();
    var answer = getSurveyAnswer(question);
    if (answer == "") {
        $(".survey-submit").on('click', function () {
            var valueAnswer = $('input[name=survey]:checked').val();
            if (typeof (valueAnswer) === "undefined") {
                openDialog("Please answer the survey question before navigating to a different page.", "Survey");
            } else {
                addSurveyAnswer(question, {
                    value: valueAnswer,
                    comments: $("#survey-comment").val()
                });
                $(".survey-submit").hide();
                openDialog("Your response has been submitted.", "Submitted");
                nextPage();
            }
        });
    } else {
        // fill values
        $('input[name=survey][value=' + answer.value + ']').prop('checked', 'checked');
        $("#survey-comment").val(answer.comments);

        // hide survey submit
        $(".survey-submit").hide();
    }
}

function quizInit() {
    var question = $(".quiz-question").first().text();
    var answer = getQuizAnswer(question);
    if (answer == "") {
        // load normal, no answer
        $(".post-quiz").hide();
        $(".quiz-submit").on('click', function () {

            var userAnswer = $('input[name=quiz]:checked').val();
            var answer = $(".quiz-question").first().prop('answer');

            if (typeof (userAnswer) === "undefined") {
                openDialog("Please answer the quiz question before navigating to a different page.", "Quiz");
            } else {

                addQuizAnswer(question, userAnswer, answer);

                if (userAnswer == answer) {
                    openDialog("Your answer is correct!", "Correct!");
                    $(".post-quiz").addClass("correct");
                    $(".post-quiz").show();
                } else {
                    openDialog("Your answer is incorrect.", "Incorrect");
                    $(".post-quiz").addClass("incorrect");
                    $(".post-quiz").show();
                }

                $(".quiz-submit").hide();
            }
        });
    } else {
        // already answered, 
        $(".post-quiz").show();
        $('input[name=quiz][value="' + answer.UserAnswer + '"]').prop('checked', 'checked');
        $(".quiz-submit").hide();

        if (answer.UserAnswer == answer.CorrectAnswer) $(".post-quiz").addClass("correct");
        else $(".post-quiz").addClass('incorrect');
    }

}

function renderElement(element) {
    var newNode;
    switch (element.nodeName.toLowerCase()) {
        case "text":
            newNode = textNode(element);
            break;
        case "img":
        case "image":
            newNode = imageNode(element);
            break;
        case "button":
            newNode = buttonNode(element);
            break;
        case "video":
            newNode = videoNode(element);
    }


    for (var i = 0; i < element.attributes.length; i++) {
        switch (element.attributes[i].nodeName.toLowerCase()) {
            case "left": {
                $(newNode).css('left', element.attributes[i].value + "px");
                break;
            }
            case "top": {
                $(newNode).css('top', element.attributes[i].value + "px");
                break;
            }
            case "width": {
                $(newNode).width(element.attributes[i].value);
                break;
            }
            case "height": {
                $(newNode).height(element.attributes[i].value);
                break;
            }
            case "class": {
                var classes = element.attributes[i].value.split(' ');
                for (var ii = 0; ii < classes.length; ii++) {
                    $(newNode).addClass(classes[ii]);
                }
                break;
            }
            case "style": {
                newNode = textStyleMap(newNode, element.attributes[i].value);
                break;
            }
            case "answer": {
                $(newNode).prop("answer", element.attributes[i].value);
                break;
            }
            default:
                var a = element.attributes[i];
                var b = 0;
                break;
        }
    }

    $("#page-content").append(newNode);
}

function videoNode(element) {
    var newNode = $("<video></video>");
    $(newNode).addClass("item");
    $(newNode).prop('controls', 'controls');

    for (var i = 0; i < element.attributes.length; i++) {
        switch (element.attributes[i].nodeName.toLowerCase()) {
            case "source": {
                // "i_{id}"
                // find the associated file
                var src = element.attributes[i].value;
                var newSrc = "";

                // running offline
                if (applicationMode === "offline") {
                    // TODO: catch other types of videos

                    newSrc = "./Content/images/" + src;
                    if (element.hasAttribute("type")) {
                        switch (element.attributes["type"].value) {
                            case "mp4":
                                newSrc += ".mp4";
                                break;
                        }
                    } else {
                        newSrc += ".mp4";
                    }
                } else {
                    newSrc = URL_Content + "ImageManager.ashx?id=" + src;
                }



                $(newNode).prop('src', newSrc);
                break;
            }
            default:
                break;
        }
    }
    return newNode;
}

function buttonNode(element) {
    var newNode = $("<button></button>");
    $(newNode).addClass("item");

    //var html = new XMLSerializer().serializeToString($(element)[0]);

    $(newNode).html($.trim(element.textContent));



    if (typeof (element.classList) === "undefined" || element.classList.length == 0) {
        $(newNode).on('click', function () {
            loadPage(element.id, "page", "button click (" + $(this).text() + ")");
        });
    } else {
        for (var i = 0; i < element.classList.length; i++) {
            switch (element.classList[i]) {
                case "quiz-submit": {

                    break;
                }
                case "survey-submit": {

                    break;
                }
                case "active": {
                    $(element).addClass("active");
                    }
                    break;
                default:
                    $(newNode).on('click', function () {
                        loadPage(element.id, "page", "button click (" + $(this).text() + ")");
                    });
                    break;
            }
        }
    }



    return newNode;
}

function imageNode(element) {
    var newNode = $("<img></img>");
    $(newNode).addClass("item");

    for (var i = 0; i < element.attributes.length; i++) {
        switch (element.attributes[i].nodeName.toLowerCase()) {
            case "source": {
                // "i_{id}"
                // find the associated file
                var src = element.attributes[i].value;
                var newSrc = "";

                // running offline
                if (applicationMode == "offline") {
                    // TODO: catch other types of images

                    newSrc = "./Content/images/" + src;
                    if (element.hasAttribute("type")) {
                        switch (element.attributes["type"].value) {
                            default:
                            case "jpeg":
                            case "jpg":
                                newSrc += ".jpg";
                                break;
                            case "png":
                                newSrc += ".png";
                                break;
                            case "gif":
                                newSrc += ".gif"

                        }
                    } else {
                        newSrc += ".jpg";
                    }

                } else {
                    newSrc = URL_Content + "ImageManager.ashx?id=" + src;
                }



                $(newNode).prop('src', newSrc);
                break;
            }
            default:
                break;
        }
    }
    return newNode;
}



function textNode(element) {
    var newNode = $("<div></div>");
    $(newNode).addClass("item");
    $(newNode).addClass('inner-text');

    var html = new XMLSerializer().serializeToString($(element)[0]);
    var content = element.textContent;

    // IE made me do this
    //  The content will contain the outer "text" tags, and those must be removed
    //  It has to render with HTML inside because of links and buttons
    html = html.replace("</text>", "");
    var position_endOfTextTag = html.indexOf('>') + 1;
    var newHtml = html.substr(position_endOfTextTag, html.length);

    $(newNode).html(newHtml);


    // use this, and remove the outer text tags

    //$(newNode).html($.trim(html));

    //$(newNode).html($(element).html().trim());
    //var content = element.textContent;
    //$(newNode).html(content);

    return newNode;
}

function textStyleMap(element, styles) {
    var styleArr = styles.split(';');
    for (var i = 0; i < styleArr.length; i++) {
        if ($.trim(styleArr[i]) != "") {
            var block = styleArr[i].split(':');
            $(element).css($.trim(block[0]), $.trim(block[1]));
        }
    }
    return element;
}






