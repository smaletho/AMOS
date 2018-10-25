var newModuleStr = '<li class="module theme_1" data-id="0"> <div class="info-block"> <i class="fa fa-minus"></i> <div class="arrow-block"> <i class="fa fa-chevron-circle-up"></i> <i class="fa fa-chevron-circle-down"></i> </div> <div class="something">M</div> <input type="text" value="New Module" class="partName" /> <div class="action-menu"> <div class="dropdown"> <a class="btn btn-secondary dropdown-toggle" href="#" role="button" id="dropdownMenuLink" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"> action </a> <div class="dropdown-menu" aria-labelledby="dropdownMenuLink"> <button class="dropdown-item" data-type="module" onclick="performActionMenu(this)">delete</button> <button class="dropdown-item" data-type="module" onclick="performActionMenu(this)">change theme</button> </div> </div> </div> </div> <ul class="sections"> <li class="new"> <a class="newItem" data-type="section" href="javascript:void(0)">new section</a> </li> </ul> </li> ';
var newSectionStr = '<li class="section" data-id="0"> <div class="info-block"> <i class="fa fa-minus"></i> <div class="arrow-block"> <i class="fa fa-chevron-circle-up"></i> <i class="fa fa-chevron-circle-down"></i> </div> <div class="something">S</div> <input type="text" value="New Section" class="partName" /> <div class="action-menu"> <div class="dropdown"> <a class="btn btn-secondary dropdown-toggle" href="#" role="button" id="dropdownMenuLink" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"> action </a> <div class="dropdown-menu" aria-labelledby="dropdownMenuLink"> <button class="dropdown-item" data-type="section" onclick="performActionMenu(this)">delete</button> </div> </div> </div> </div> <ul class="chapters"> <li class="new"> <a class="newItem" data-type="chapter" href="javascript:void(0)">new chapter</a> </li> </ul> </li>';
var newChapterStr = ' <li class="chapter" data-id="0"> <div class="info-block"> <i class="fa fa-minus"></i> <div class="arrow-block"> <i class="fa fa-chevron-circle-up"></i> <i class="fa fa-chevron-circle-down"></i> </div> <div class="something">C</div> <input type="text" value="New Chapter" class="partName" /> <div class="action-menu"> <div class="dropdown"> <a class="btn btn-secondary dropdown-toggle" href="#" role="button" id="dropdownMenuLink" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"> action </a> <div class="dropdown-menu" aria-labelledby="dropdownMenuLink"> <button class="dropdown-item" data-type="chapter" onclick="performActionMenu(this)">delete</button> </div> </div> </div> </div> <ul class="pages"> <li class="new"> <a class="newItem" data-type="page" href="javascript:void(0)">new page</a> </li> </ul> </li>';
var newPageStr = '<li class="page" data-id="@pa.PageId"> <div class="info-block"> <div class="arrow-block"> <i class="fa fa-chevron-circle-up"></i> <i class="fa fa-chevron-circle-down"></i> </div> <div class="something">P</div> <input type="text" value="New Page" class="partName" /> <div class="action-menu"> <div class="dropdown"> <a class="btn btn-secondary dropdown-toggle" href="#" role="button" id="dropdownMenuLink" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"> action </a> <div class="dropdown-menu" aria-labelledby="dropdownMenuLink" data-id="0"> <button class="dropdown-item" data-type="page" onclick="performActionMenu(this)">import</button> <button class="dropdown-item" data-type="page" onclick="performActionMenu(this)">delete</button> </div> </div> </div> </div> </li>';

var errorList = [];
var replaceElement;

var unusedPageData = [];
var applicationMode = "online";

var moduleThemeElement = null;

$(function () {


    unbindRebindListeners();

    $(".bookValid").on("click", function () {
        console.log($(".error-list").css('display'));
        if ($(".error-list").css('display') == "block") {
            $(".error-list").slideUp('400');
        } else {
            $(".error-list").slideDown('400');
        }
    });

    $("#saveButton").on('click', saveBook);

    $(".themeBlock").on('click', function () {
        $(".themeBlock").removeClass("selected");
        updateTheme($(this).find(".themeImg").first().data("id"));
    });

    $(window).bind('beforeunload', function () {
        if ($("#saveButton").hasClass("btn-warning"))
            return "You have unsaved changes. Would you like to discard them?";
    });

});

function updateTheme(id) {
    $(moduleThemeElement).removeClass("theme_1");
    $(moduleThemeElement).removeClass("theme_2");
    $(moduleThemeElement).removeClass("theme_3");
    $(moduleThemeElement).removeClass("theme_4");
    $(moduleThemeElement).removeClass("theme_5");
    $(moduleThemeElement).removeClass("theme_6");
    $(moduleThemeElement).removeClass("theme_7");
    $(moduleThemeElement).removeClass("theme_8");
    $(moduleThemeElement).removeClass("theme_9");
    $(moduleThemeElement).addClass("theme_" + id);
    moduleThemeElement = null;
    $(".overlay").hide();
    $(".themeSelector").hide();
}

function createNewItem(element, type) {
    var d = {};
    var url = "";

    switch (type) {
        case "page":
            url = URL_New_Page;
            d = { m: 0, s: 0, c: 0, p: 0 };
            break;
        case "chapter":
            url = URL_New_Chapter;
            break;
        case "section":
            url = URL_New_Section;
            break;
        case "module":
            url = URL_New_Module;
            break;
    }

    if (type == "page") {
        transmitAction(url, function (data) {
            addItemContent_Success(element, type, data);
        }, addItemContent_Fail, "", d, true);
    } else {
        transmitAction(url, function (data) {
            addItemContent_Success(element, type, data);
        }, addItemContent_Fail, "", null, true);
    }
    
}

function addItemContent_Success(element, type, data) {
    $(data).insertBefore($(element).closest('li'));
    modifiedPage();
    unbindRebindListeners();
}
function addItemContent_Fail(data) {
    alert("Error");
    console.log('error', data);
}


function unbindRebindListeners() {
    $(".fa-sort-up").unbind();
    $(".fa-sort-down").unbind();
    $(".fa-plus").unbind();
    $(".fa-minus").unbind();
    $(".newItem").unbind();
    $(".page-action").unbind();
    $(".chapter-action").unbind();
    $(".section-action").unbind();
    $(".module-action").unbind();
    $("input[type=text]").unbind();



    $("input[type=text]").on('keyup', modifiedPage);
    
    $(".fa-sort-up").on('click', function () { moveBlock(true, this); });
    $(".fa-sort-down").on('click', function () { moveBlock(false, this); });
    $(".fa-plus").on('click', function () {
        openCollapseSection(false, this);
    });
    $(".fa-minus").on('click', function () {
        openCollapseSection(true, this);
    });
    $(".newItem").on('click', function () { createNewItem(this, $(this).data('type')); });

    initializeBookEdit();
}


function performActionMenu(element) {
    var type = $(element).data('type');
    var val = $(element).text();
    
    if (type == "page" && val == "edit") {
        saveBook();
        window.location = URL_EditPage.replace("99999", $(element).data('id'));
    } else if (type == "page" && val == "preview") {
        saveBook();
        window.location = URL_ViewPage.replace("99999", $(element).data('id'));
    } else if (type == "page" && val == "import") {
        replaceElement = $(element).closest("li");
        showPageSelect();
    } else if (val == "delete") {
        if (confirm("Would you like to remove this " + type + "?")) {
            $(element).closest("li").remove();
            modifiedPage();
        }
    } else if (val == "change theme") {
        openInitThemeSelector($(element).closest("li"));
    }
    validateBook();
    
}

function getParentType(type) {
    switch (type) {
        case "page":
            return "chapter";
        case "chapter":
            return "section";
        case "section":
            return "module";
        default:
            return "";
    }
}

function moveBlock(isUp, element) {
    // grab the block we're moving
    var blockToMove = $(element).closest("li");
    // check what kind it is
    var type = $(blockToMove).prop('class');
    var parentType = getParentType(type);

    var parent = $(blockToMove).closest("ul").first();
    var prevElement;
    var similarParents = $(document).find("." + type + "s");



    // find all similar blocks on the page, and loop through
    var similarElements = $(document).find("." + type);
    for (var i = 0; i < similarElements.length; i++) {
        // if this element (in the loop) is the same as the one we're moving...
        if (similarElements[i] === blockToMove.get(0)) {

            if (isUp) {
                prevElement = similarElements[i - 1];

                // if the parent of the previous element is NOT equal to the parent
                //  of the block to move, we're jumping up a group. Put the element 
                //  to move AFTER this, not before

                if ($(prevElement).parent("ul").get(0) === $(blockToMove).parent("ul").get(0)) {
                    // same parent, normal move
                    blockToMove.insertBefore(prevElement);
                } else if (typeof prevElement === "undefined") {
                    // we're moving into a new container

                    // get the parent container, and find the previous one of the same type
                    for (var i = 0; i < similarParents.length; i++) {
                        // if the parents are the same, use the previous element
                        if (similarParents[i] === parent.get(0)) {
                            blockToMove.insertBefore($(similarParents[i - 1]).find(".new").first());
                            break;
                        }
                    }

                } else {
                    // different parent, insert after
                    blockToMove.insertAfter(prevElement);
                }


                break;
            } else {
                prevElement = similarElements[i + 1];

                if ($(prevElement).parent("ul").get(0) === $(blockToMove).parent("ul").get(0)) {
                    // same parent, normal move
                    blockToMove.insertAfter(prevElement);
                } else if (typeof prevElement === "undefined") {
                    // we're moving into a new container

                    // get the parent container, and find the previous one of the same type
                    for (var i = 0; i < similarParents.length; i++) {
                        // if the parents are the same, use the previous element
                        if (similarParents[i] === parent.get(0)) {
                            blockToMove.insertBefore($(similarParents[i + 1]).find(".new").first());
                            break;
                        }
                    }
                } else {
                    // different parent, insert after
                    blockToMove.insertBefore(prevElement);
                }

                break;
            }
        }
    }
    scrollToElement(blockToMove);
    modifiedPage();
    initializeBookEdit();
}

function openCollapseSection(isClose, element) {
    var newIcon = "";
    var parent = $(element).closest("li");
    var hideShow = $(parent).find("ul").first();
    if (isClose) {
        newIcon = "<i class='fa fa-plus'></i>"
        $(element).replaceWith("<i class='fa fa-plus'></i>");
        $(hideShow).hide();
    } else {
        $(element).replaceWith("<i class='fa fa-minus'></i>");
        $(hideShow).show();
    }

    unbindRebindListeners();
}

function initializeBookEdit() {
    // reset disabled options
    $('.fa').removeClass("disabled");

    // first of types
    $(".module").first().find(".fa-sort-up").first().addClass("disabled");
    $(".module").last().find(".fa-sort-down").first().addClass("disabled");


    $(".module").first().find(".section").first().find(".fa-sort-up").first().addClass("disabled");
    $(".section").first().find(".chapter").first().find(".fa-sort-up").first().addClass("disabled");
    $(".chapter").first().find(".page").first().find(".fa-sort-up").first().addClass("disabled");

    $(".module").last().find(".section").last().find(".fa-sort-down").first().addClass("disabled");
    $(".section").last().find(".chapter").last().find(".fa-sort-down").first().addClass("disabled");
    $(".chapter").last().find(".page").last().find(".fa-sort-down").first().addClass("disabled");

    $(".scrollTo").on('click', function () {
        var index = parseInt($(this).data('item'));
        var item = errorList[index].element;
        if (item != null) {
            $('html, body').animate({
                scrollTop: ($(item).offset().top)
            }, 500);
        }
    });
    
    validateBook();
}


function validateBook() {
    var isValid = true;
    var moduleCount = 0;
    var sectionCount = 0;
    var chapterCount = 0;
    var pageCount = 0;
    errorList = [];

    // all modules have sections?
    $(".module").each(function (k, v) {
        moduleCount++;
        sectionCount = 0;
        $(this).find(".section").each(function (k1, v1) {
            sectionCount++;
            chapterCount = 0;
            $(this).find(".chapter").each(function (k2, v2) {
                chapterCount++;
                pageCount = 0;
                $(this).find(".page").each(function (k3, v3) {
                    pageCount++;
                    var txt = moduleCount + "_" + sectionCount + "_" + chapterCount + "_" + pageCount;
                    var t = $(this);
                    var fi = $(this).find(".page-identifier").first();
                    $(this).find(".page-identifier").first().text(moduleCount + "_" + sectionCount + "_" + chapterCount + "_" + pageCount);
                });
                if (pageCount == 0) {
                    errorList.push({ value: "No pages found in a chapter", element: this });
                    isValid = false;
                }
            });
            if (chapterCount == 0) {
                errorList.push({ value: "No chapters found in a section", element: this });
                isValid = false;
            }
        });
        if (sectionCount == 0) {
            errorList.push({ value: "No sections found in a module", element: this });
            isValid = false;
        }
    });
    if (moduleCount == 0) {
        errorList.push({ value: "No modules found in the book", element: this });
        isValid = false;
    }

    $("#configurationButton").removeClass("btn-success");
    $("#configurationButton").removeClass("btn-warning");

    if (isValid) {
        $("#configurationButton").addClass("btn-success");
        $("#configurationButton").text("Valid Configuration");
        errorList.push({ value: "No errors found", element: null });
    } else {
        $("#configurationButton").addClass("btn-warning");
        $("#configurationButton").text("Invalid Configuration");
    }


    $("#errors").empty();
    for (var i = 0; i < errorList.length; i++) {
        var appStr = "";
        if (errorList[i].element == null) {
            appStr = '<div class="dropdown-item">' + errorList[i].value + '</div>';
        } else {
            appStr = '<div class="dropdown-item">' + errorList[i].value + '<a href="javascript:void(0)" class="scrollTo" data-item="' + i + '">find</a></div>';
        }
        $("#errors").append(appStr);
    }
    $(".scrollTo").unbind();
    $(".scrollTo").on('click', function () {
        var index = parseInt($(this).data('item'));
        var item = errorList[index].element;
        if (item != null) {
            scrollToElement(item);
            $(".error-list").slideUp('400');
        }
    });

    return isValid;
}

function scrollToElement(element) {
    $('html, body').animate({
        scrollTop: ($(element).offset().top - 150)
    }, 500, function () {
        flashItem(element);
    });
}

function modifiedPage() {
    $("#saveButton").removeClass("btn-success");
    $("#saveButton").removeClass("btn-warning");
    $("#saveButton").addClass("btn-warning");
    $("#saveButton").text("NOT Saved (click)");
}


function flashItem(element) {
    $(element).effect("highlight", {}, 1000);
    //$(element).effect("highlight", {}, 1000);
}


function saveBook() {
    if (confirm("Would you like to save this book?")) {
        var isValid = validateBook();

        var book = {
            S_Book: {
                BookId: $("#GetBook_BookId").val(),
                Name: $(".bookName").val(),
                Version: $(".bookVersion").val()
            },
            s_Modules: [],
            s_Sections: [],
            s_Chapters: [],
            s_Pages: []
        };

        var mId = 1;
        var sId = 1;
        var cId = 1;
        var pId = 1;
        var mSort = 10;
        var sSort = 10;
        var cSort = 10;
        var pSort = 10;

        // TODO validate that they all have names

        // modules
        $(".module").each(function () {
            book.s_Modules.push({
                ModuleId: $(this).data('id'),
                ModuleTempId: mId,
                Name: $(this).find(".partName").first().val(),
                SortOrder: mSort,
                // TODO fix
                Theme: getTheme(this)
            });

            // sections
            $(this).find(".section").each(function () {
                book.s_Sections.push({
                    SectionId: $(this).data('id'),
                    SectionTempId: sId,
                    Name: $(this).find(".partName").first().val(),
                    SortOrder: sSort,
                    ModuleTempId: mId
                });

                // chapters
                $(this).find(".chapter").each(function () {
                    book.s_Chapters.push({
                        ChapterId: $(this).data('id'),
                        ChapterTempId: cId,
                        Name: $(this).find(".partName").first().val(),
                        SortOrder: cSort,
                        SectionTempId: sId
                    });

                    // pages
                    $(this).find(".page").each(function () {
                        book.s_Pages.push({
                            PageId: $(this).data('id'),
                            ChapterTempId: cId,
                            Name: $(this).find(".partName").first().val(),
                            SortOrder: pSort,
                        });
                        pId++;
                        pSort += 10;
                    });
                    cId++;
                    cSort += 10;
                    pSort = 10;
                });
                sId++;
                sSort += 10;
                cSort = 10;
            });
            mId++;
            mSort += 10;
            sSort = 10;
        });

        transmitAction(URL_SaveBook, bookSaveSuccess, bookSaveFail, "", { model: book }, true);
    }


}

function getTheme(element) {
    var ret = "";
    var classArr = $(element).prop('class').split(' ');
    $(classArr).each(function () {
        if (this.indexOf("theme") != -1) {
            // Be careful of putting the return statement in here.
            //  return from inside the $.each() function only returns to 
            //  itself, not the actual function.
            //      The return here is only to break the iteration.
            ret = this.replace('theme_', '');
            return;
        }
    });
    return ret;
}

function bookSaveSuccess(data) {
    $("#saveButton").removeClass("btn-warning");
    $("#saveButton").removeClass("btn-success");
    $("#saveButton").addClass("btn-success");
    $("#saveButton").text("Saved");
    console.log(data);
}
function bookSaveFail(data) {
    $(".bookStatus").removeClass("in-valid");
    $(".bookStatus").removeClass("valid");
    $(".bookStatus").addClass("in-valid");
    $(".bookStatus").text("Error");
    console.log(data);
}




function openInitThemeSelector(element) {
    
    moduleThemeElement = element;
    var id = 0;
    var arr = $(element).prop("class").split(' ');
    $(arr).each(function () {
        if (this.indexOf("theme") != -1) {
            id = parseInt(this.replace('theme_', ''));
        }
    });
    $(".overlay").show();
    $(".themeSelector").show();
    $(".themeImg").each(function () {
        if ($(this).data('id') == id) {
            $(this).closest('.themeBlock').addClass("selected");
            $(this).closest('.themeBlock')[0].scrollIntoView();
            return;
        }
    });
}







function showPageSelect() {
    transmitAction(URL_GetAvailablePages,
        function (data) {
            $(".overlay").show();
            $(".pageSelector").show();
            unusedPageData = data;
            buildUnusedPageSelect();
        },
        function (data) {
            console.log('error', data);
            alert("There was an error.");
        },
        "json", null, true);

    
}

function closePageImport() {
    $(".pageList").empty();
    $(".unusedPage").removeClass("selected");
    $(".pagePreview").empty();
    $(".overlay").hide();
    $(".pageSelector").hide();
}

function buildUnusedPageSelect() {

    var htmlDiv = "";
    for (var i = 0; i < unusedPageData.length; i++) {
        htmlDiv += '<div class="unusedPage" data-index="' + i + '"><div class="name">';
        htmlDiv += unusedPageData[i].Title + '</div><div class="user">' + unusedPageData[i].ModifiedBy + '</div>';
        htmlDiv += "</div>";
    }
    $(".pageList").empty().append(htmlDiv);

    $(".unusedPage").on('click', function () {
        $(".unusedPage").removeClass("selected");
        $(this).addClass('selected');

        // load a preview of the page
        var arrIndex = $(this).data('index');
        var item = unusedPageData[arrIndex];

        $(".pagePreview").empty();

        $(item.PageContent).contents().each(function () {
            if (!blankTextNode(this)) {

                var innerPage = $.trim(this.textContent);

                if (innerPage.indexOf('<text>') === -1 && innerPage.indexOf('</text>') === -1) {
                    // this is a normal, empty node
                    tempRenderElement(this);
                } else {
                    //openDialog("There appears to be a formatting issue with this book. Please contact a system administrator.", "Error");
                }
            }
        });

        
    });
}


// This is a copy over version of rendering from render.js
//  I had to rebuild/reuse because the context is different
function tempRenderElement(element) {
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

    $(".pagePreview").append(newNode);
}



function importPage() {
    // find the selected page
    var item = $(".unusedPage.selected").first();
    var index = $(item).data('index');

    var pageToAdd = unusedPageData[index];

    // create page object based on item above
    var newElement = importExistingPageStr.replace("{0}", pageToAdd.PageId);
    newElement = newElement.replace("{1}", pageToAdd.Title);
    newElement = newElement.replace("{2}", pageToAdd.PageId);

    $(replaceElement).replaceWith(newElement);
    closePageImport();
    unbindRebindListeners();
}

var importExistingPageStr = '<li class="page" data-id="{0}"> <div class="info-block"> <div class="arrow-block"> <i class="fa fa-sort-up"></i> <i class="fa fa-sort-down"></i> </div> <div class="something">P</div> <input type="text" value="{1}" class="partName" /> <div class="action-menu"> <select class="page-action" data-id="{2}"> <option value="null">-action-</option> <option value="preview">preview</option> <option value="edit">edit</option> <option value="delete">delete</option> </select> </div> </div> </li>';