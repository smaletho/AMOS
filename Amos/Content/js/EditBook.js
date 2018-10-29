var errorList = [];
var moduleThemeElement = null;

$(function() {
    updateList();

    $(".themeBlock").on('click', function () {
        $(".themeBlock").removeClass("selected");
        updateTheme($(this).find(".themeImg").first().data("id"));
    });
});

function updateList() {
    transmitAction(URL_BuildList, function(data) {
        $(".main-list-container").empty();
        $(".main-list-container").append(data);
        unbindRebindListeners();
    }, function(data) {
        alert('error');
        console.log(data);
    }, "", model, true);
}

// init functions
function unbindRebindListeners() {
    $(".fa-caret-up").unbind();
    $(".fa-caret-down").unbind();
    $(".page-action").unbind();
    $("input[type=text]").unbind();
    $(".bookVersion").unbind();
    $(".bookName").unbind();
    $(".itemTitle").unbind();

    $(".bookVersion").blur(function () { validateBook(true); });
    $(".bookName").blur(function () { validateBook(true); });
    $(".itemTitle").blur(function () { validateBook(true); });

    initHoverActions();
    

    $(".fa-caret-up").on('click', function() { moveBlock(true, this); });
    $(".fa-caret-down").on('click', function() { moveBlock(false, this); });
    

    initializeBookEdit();
}
function initializeBookEdit() {
    // reset disabled options
    $('.fa').removeClass("disabled");

    // first of types
    $(".module").first().find(".fa-caret-up").first().addClass("disabled");
    $(".module").last().find(".fa-caret-down").first().addClass("disabled");

    $(".section").first().find(".fa-caret-up").first().addClass("disabled");
    $(".section").last().find(".fa-caret-down").first().addClass("disabled");

    $(".chapter").first().find(".fa-caret-up").first().addClass("disabled");
    $(".chapter").last().find(".fa-caret-down").first().addClass("disabled");

    $(".page").first().find(".fa-caret-up").first().addClass("disabled");
    $(".page").last().find(".fa-caret-down").first().addClass("disabled");

    $(".scrollTo").on('click', function() {
        var index = parseInt($(this).data('item'));
        var item = errorList[index].element;
        if (item != null) {
            $('html, body').animate({
                scrollTop: ($(item).offset().top)
            }, 500);
        }
    });

    validateBook(false);
}
function initHoverActions() {
    $(".sortControls, .lower-highlight").unbind();
    $(".upper-highlight-module").unbind();
    $(".upper-highlight-section").unbind();
    $(".upper-highlight-chapter").unbind();

    $(".sortControls, .lower-highlight").hover(function() {
        doLowerHighlightHover(true, this);
    }, function() {
        doLowerHighlightHover(false, this);
    });


    // highlighting UP stuff
    $(".upper-highlight-module").hover(function() {
        doUpperHighlight(true, this, "module");
    }, function() {
        doUpperHighlight(false, this, "module");
    });

    $(".upper-highlight-section").hover(function() {
        doUpperHighlight(true, this, "section");
    }, function() {
        doUpperHighlight(false, this, "section");
    });

    $(".upper-highlight-chapter").hover(function() {
        doUpperHighlight(true, this, "chapter");
    }, function() {
        doUpperHighlight(false, this, "chapter");
    });
}
function doUpperHighlight(isIn, element, type) {
    var color = "white";
    if (isIn) color = "#17a2b8";

    var row = $(element).closest('.row');
    $(row).css('background', color);
    $(row).prevAll("." + type).first().css('background', color);
}
function doLowerHighlightHover(isIn, element) {

    var color = "white";
    if (isIn) color = "#ebebeb";

    var row = $(element).closest(".row");
    var type = $(row).prop('class').split(' ')[0];

    $(row).css('background', color);
    $(row).nextUntil(getNextUntilForHover(type)).css('background', color);
}
function getNextUntilForHover(type) {
    switch (type) {
        case "module":
            return ".module";
        case "section":
            return ".section, .module";
        case "chapter":
            return ".chapter, .section, .module";
        case "page":
            return ".page, .chapter, .section, .module";
        default:
            return "";
    }
}



// action menu options
function performActionMenu(element) {
    var type = $(element).data('type');
    var val = $(element).text();

    if (type == "page" && val == "edit") {
        window.location = URL_EditPage.replace("99999", $(element).data('id'));
    } else if (type == "page" && val == "preview") {
        window.location = URL_ViewPage.replace("99999", $(element).data('id'));
    } else if (type == "page" && val == "import") {
        replaceElement = $(element).closest("li");
        showPageSelect();
    } else if (val == "delete") {
        if (confirm("Would you like to remove this " + type + "?")) {
            var id = $(element).closest('.row').data('myid');
            modifiedPage(false);
            transmitAction(URL_DeleteItem, function (data) {
                // success
                var row = $(element).closest('.row');
                var allRows = $(row).nextUntil(getNextUntilForHover(type));
                $(allRows).remove();
                $(row).remove();
                modifiedPage(true);
            }, function (data) {
                alert("error");
                console.log(data);
                }, "",
                {
                    id: id,
                    type: type
            }, true);
        }
    } else if (val == "change theme") {
        openInitThemeSelector($(element).closest(".row"));
    }
    validateBook();

}

// call this when anything is modified to note "not saved"
function modifiedPage(isDone) {
    if (!isDone) {
        $("#saveButton").removeClass("btn-success");
        $("#saveButton").removeClass("btn-warning");
        $("#saveButton").addClass("btn-warning");
        $("#saveButton").text("NOT Saved");
    } else {
        $("#saveButton").removeClass("btn-success");
        $("#saveButton").removeClass("btn-warning");
        $("#saveButton").addClass("btn-success");
        $("#saveButton").text("saved");
    }
}


function moveBlock(isUp, element) {
    var row = $(element).closest('.row');
    var type = $(row).prop('class').split(' ')[0];

    var allRows = $(row).nextUntil(getNextUntilForHover(type));

    if (isUp) {
        var prevRow = $(row).prev();
        var prevType = $(prevRow).prop('class').split(' ')[0];
        // prevUntil row is same type
        var until = $(prevRow).prevUntil("." + prevType);
        $(row).insertBefore($(until).first());
        $(allRows).insertAfter($(row));
    } else {
        // grab everythin that ".lower-highlight" would
        var nextRow = $(allRows).last().next();
        if (nextRow.length == 0) nextRow = $(row).next();
        var nextFullBlock = $(nextRow).nextUntil(getNextUntilForHover(type));

        if (nextFullBlock.length == 0) {
            $(row).insertAfter($(nextRow));
        } else {
            $(row).insertAfter($(nextFullBlock).last());
        }

        $(allRows).insertAfter($(row));

    }


    validateBook(true);
}



function scrollToElement(element) {
    $('html, body').animate({
        scrollTop: ($(element).offset().top - 150)
    }, 500, function() {
        flashItem(element);
    });
}

// highlights/flashes a specific item
function flashItem(element) {
    $(element).effect("highlight", {}, 1000);
    //$(element).effect("highlight", {}, 1000);
}









// validation
function validateBook(doSave) {
    model.S_Book = {};
    model.s_Modules = [];
    model.s_Sections = [];
    model.s_Chapters = [];
    model.s_Pages = [];

    model.S_Book.BookId = $("#GetBook_BookId").val();
    model.S_Book.Name = $(".bookName").first().val();
    model.S_Book.Version = $(".bookVersion").first().val();

    var errorList = [];

    // Start looping through the page visually
    var isValid = true;
    var moduleCount = 10;
    var sectionCount = 10;
    var chapterCount = 10;
    var pageCount = 10;

    // TODO check if ParentId == 0

    // all modules have sections?
    $(".module").each(function (k, v) {

        model.s_Modules.push({
            ModuleId: $(v).data('myid'),
            Type: 'module',
            SortOrder: (moduleCount * 10),
            ParentId: $(v).data('parentid'),
            Name: $(v).find(".itemTitle").first().val(),
            Theme: $(v).prop('class').split(' ')[2].split('_')[1]
        });

        moduleCount++;
        sectionCount = 0;
        $(v).nextUntil(".module", ".section").each(function (k1, v1) {

            model.s_Sections.push({
                SectionId: $(v1).data('myid'),
                Type: 'section',
                SortOrder: (sectionCount * 10),
                ParentId: $(v).data('myid'),
                Name: $(v1).find(".itemTitle").first().val()
            });

            sectionCount++;
            chapterCount = 0;
            $(v1).nextUntil(".section, .module", ".chapter").each(function (k2, v2) {

                model.s_Chapters.push({
                    ChapterId: $(v2).data('myid'),
                    Type: 'chapter',
                    SortOrder: (chapterCount * 10),
                    ParentId: $(v1).data('myid'),
                    Name: $(v2).find(".itemTitle").first().val()
                });

                chapterCount++;
                pageCount = 0;
                $(v2).nextUntil(".chapter, .section, .module", ".page").each(function (k3, v3) {

                    model.s_Pages.push({
                        PageId: $(v).data('myid'),
                        Type: 'page',
                        SortOrder: (pageCount * 10),
                        ParentId: $(v2).data('myid'),
                        Name: $(v3).find(".itemTitle").first().val()
                    });

                    pageCount++;
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
            appStr = '<li><a>' + errorList[i].value + '</a></li>';
        } else {
            appStr = '<li><a href="javascript:void(0)" class="scrollTo" data-item="' + i + '">' + errorList[i].value + '</a></div>';
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

    if (doSave) {
        transmitAction(URL_SaveBook, function (data) {
            $(".main-list-container").empty();
            $(".main-list-container").append(data);
            unbindRebindListeners();
            modifiedPage(true);
        }, function (data) {
            alert('error');
            console.log(data);
        }, "", model, true);
    }
    
}

function addNewItem(type, element) {
    modifiedPage(false);

    if (type == "module") {
        transmitAction(URL_Build_Module, function (data) {
            // success
            $(data).insertBefore($(".module").first());
            validateBook(true);
        }, function (data) {
            // fail
            alert("Error");
            console.log(data);
            }, "", {
                item: {
                    ModuleId: 0,
                    ParentId: 0,
                    Name: "New Module",
                    SortOrder: 0,
                    Theme: "1"
                },
                m: 0
            }, true);
    }


    if (type == "section") {
        transmitAction(URL_Build_Section, function (data) {
            // success
            $(data).insertAfter($(element).closest('.row'));
            validateBook(true);
        }, function (data) {
            // fail
            alert("Error");
            console.log(data);
        }, "", {
                item: {
                    SectionId: 0,
                    ParentId: $(element).closest('.row').data('myid'),
                    Name: "New Section",
                    SortOrder: 0
                },
                s: 0
            }, true);
    }


    if (type == "chapter") {
        transmitAction(URL_Build_Chapter, function (data) {
            // success
            $(data).insertAfter($(element).closest('.row'));
            validateBook(true);
        }, function (data) {
            // fail
            alert("Error");
            console.log(data);
        }, "", {
                item: {
                    ChapterId: 0,
                    ParentId: $(element).closest('.row').data('myid'),
                    Name: "New Chapter",
                    SortOrder: 0
                },
                c: 0
            }, true);
    }


    if (type == "page") {
        transmitAction(URL_Build_Page, function (data) {
            // success
            $(data).insertAfter($(element).closest('.row'));
            validateBook(true);
        }, function (data) {
            // fail
            alert("Error");
            console.log(data);
        }, "", {
                item: {
                    PageId: 0,
                    ParentId: $(element).closest('.row').data('myid'),
                    Name: "New Page",
                    SortOrder: 0
                },
                m: 0, s:0, c:0, p:0
            }, true);
    }
    
}




// Theme functions START
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

    validateBook(true);
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


// importing pages
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