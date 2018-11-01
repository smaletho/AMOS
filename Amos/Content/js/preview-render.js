// copies of render.js
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

    return newNode;
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
                            case "jpg":
                                newSrc += ".jpg";
                                break;
                            case "png":
                                newSrc += ".png";
                                break;

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