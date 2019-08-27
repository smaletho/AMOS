function initNavigation() {
    unbindNavigation();

    $("#next-button").on('click', function () {
        nextPage("next button click");
    });
    $("#previous-button").on('click', function () {
        previousPage("previous button click");
    });

    $(".chapter-item").on('click', function () {
        var id = $(this).data('id');
        loadPage(id, "chapter", "chapter click (" + $(this).text() + ")");
    });
    $(".section-item").on('click', function () {
        var id = $(this).data('id');
        loadPage(id, "section", "section click (" + $(this).text() + ")");
    });
    $(".dot").on('click', function () {
        var id = $(this).data('page');
        loadPage(id, "page", "navigation dot click");
    });
    bindKeyboard();
}

function bindKeyboard() {
    $(document).keydown(function (event) {
        idleTime = 0;
        if (event.which === 39) {
            // going forwards
            nextPage("arrow key navigation");
            event.preventDefault();
        } else if (event.which === 37) {
            // going backwards
            previousPage("arrow key navigation");
            event.preventDefault();
        } else if (event.which === 32) {
            // make sure the focused element isn't an input or textarea
            if (!$("input[type=text]").is(":focus") && !$("textarea").is(":focus")) {
                nextPage("spacebar navigation");
                event.preventDefault();
            }
                
        }
    });
}

function unbindNavigation() {
    $("#next-button").unbind();
    $("#previous-button").unbind();
    $(".chapter-item").unbind();
    $(".section-item").unbind();
    $(".dot").unbind();
    $(document).unbind();
}


function nextPage(type) {
    // CurrentLocation
    window.scrollTo(0, 0);

    var allPages = $(ConfigXml).find('page');

    for (var i = 0; i < allPages.length; i++) {
        var id = $(allPages[i])[0].attributes["id"].value;
        if (id === CurrentLocation.Page) {
            if (i + 1 >= allPages.length) {
                exitBook("You have reached the end of the book. Would you like to exit?");
            } else {
                if ($(allPages[i+1]).hasClass("hide-page")) {
                    // it's a hidden page

                    var anotherPageExists = false;
                    for (var ii = i + 1; ii < allPages.length; ii++) {
                        // find next not hidden page
                        if (!$(allPages[ii]).hasClass("hide-page")) {
                            anotherPageExists = true;
                            loadPage($(allPages[ii])[0].attributes["id"].value, "page", type);
                            break;
                        }
                    } 

                    if (!anotherPageExists)
                        exitBook("You have reached the end of the book. Would you like to exit?");
                } else {
                    loadPage($(allPages[i + 1])[0].attributes["id"].value, "page", type);
                }
            } 
            break;
        }
    }
}
function previousPage(type) {
    window.scrollTo(0, 0);
    var allPages = $(ConfigXml).find('page');

    for (var i = 0; i < allPages.length; i++) {
        var id = $(allPages[i])[0].attributes["id"].value;
        if (id === CurrentLocation.Page) {
            if (i - 1 < 0) {
                openDialog("You are currently at the beginning of the module.", "Warning");
            } else {
                if ($(allPages[i - 1]).hasClass("hide-page")) {
                    // it's a hidden page, go to the last non-hidden page
                    var lastNonHidden = $(allPages[i - 1]).prevAll("page").not(".hide-page").first();
                    if (lastNonHidden.length !== 0)
                        loadPage($(lastNonHidden)[0].attributes["id"].value, "page", type);
                    else
                        openDialog("You are currently at the beginning of the module.", "Warning");
                } else {
                    loadPage($(allPages[i - 1])[0].attributes["id"].value, "page", type);
                }
            }
            break;
        }
    }
}

function goToTheBeginning() {
    var newId = $(ConfigXml).find('page').first();

    loadPage(newId[0].attributes["id"].value, "page", "go to the beginning");

    showInitialHelp();
}


function showInitialHelp() {
    var tour = new Tour({
        backdrop: true,
        storage: false,
        keyboard: true,
        autoscroll: false,
        delay: 200,
        onStart: function () {
            $("#main-window").css('pointer-events', 'none');
            $(document).unbind("keydown");
        },
        onEnd: function () {
            window.scrollTo(0, 0);
            $("#main-window").css('pointer-events', 'auto');
            bindKeyboard();
        },
        steps: [
            {
                element: "#module-head",
                title: "Welcome!",
                content: "Welcome to the AMOS training and guidance application!<br /><br />The following tour will provide a brief explanation of this application's features. Feel free to end the tour at any time.",
                placement: "bottom",
                animation: false
            },
            {
                element: "#menu-open",
                title: "Menu",
                content: "Click this menu to view all the modules contained in this book.",
                onNext: function () {
                    openNav();
                },
                placement: "right",
                animation: false
            },
            {
                element: "#pageList",
                title: "Modules",
                content: "Click on a module here to navigate to that location.",
                onNext: function () {
                    closeNav();
                },
                onPrev: function () {
                    closeNav();
                },
                placement: "right",
                animation: false
            },
            {
                element: "#sections",
                title: "Sections",
                onPrev: function () {
                    openNav();
                },
                content: "This bar shows the second level of navigation, known as sections.<br /><br />You can click on these to navigate directly to the specified location.",
                placement: "bottom",
                animation: false
            },
            {
                element: "#chapters",
                title: "Chapters",
                content: "This bar shows the third level of navigation, known as chapters.<br /><br />You can click on these to navigate directly to the specified location.",
                placement: "bottom",
                animation: false
            },
            {
                element: "#dot-container",
                title: "Pages",
                content: "This piece shows all the pages contained in each module.<br /><br />You can navigate directly to a specific page by clicking on it.<br /><br />Pages you've already visited will be lightly highlighted to track your progress.<br /><br />Dark lines in between pages denote the end of a section.",
                placement: "top",
                animation: false
            },
            {
                element: "#next-button",
                title: "Next Page",
                content: "Click here to view the next page, use the RIGHT arrow key on your keyboard, or press the SPACEBAR.<br /><br />There are multiple pages per chapter; modules are designed to page through like a book.",
                placement: "left",
                animation: false
            },
            {
                element: "#previous-button",
                title: "Previous Page",
                content: "Click here to view the previous page, or use the LEFT arrow key on your keyboard.",
                placement: "right",
                animation: false
            },
            {
                element: "#page-content",
                title: "Content",
                content: "Here is where you will view and interact with the page content.<br /><br />Pages may have buttons and/or links, which will give popup definitions, or navigate you directly to a specific page.<br /><br />Click any image to enlarge, click anywhere on screen to close.<br /><br />Use on-screen video buttons to control video screen size and playback.",
                placement: "top",
                animation: false
            },
            {
                element: "#help-button",
                title: "Help",
                content: "Click here to view this tutorial again.",
                placement: "left",
                animation: false
            },
            {
                element: "#exit-button",
                title: "Exit",
                content: "Click here to exit the book. Your progress and data will be saved.",
                placement: "left",
                animation: false
            }
        ]
    });

    // Initialize the tour
    tour.init();

    // Start the tour
    tour.start(true);
}

function loadPage(id, type, navDescription) {
    
    if (typeof id === "undefined") {
        // an ID wasn't sent. check if the localStorage object knows where to go
        //  otherwise, go to the beginning


        // TODO will this work in all versions? (more testing)
        if (UserTracker.CurrentLocation === null || typeof UserTracker.CurrentLocation === "undefined")
            goToTheBeginning();
        else
            loadPage(UserTracker.CurrentLocation.Page, "page", "returning user");
        
    } else {

        // check if page is unanswered quiz or survey
        if (okayToNavigateAway()) {
            var targetPage;
            var tempElement;
            switch (type) {
                case "page":
                    // id is already a page
                    break;
                case "chapter":
                    tempElement = $(ConfigXml).find("#" + id).children("page").first();
                    id = tempElement[0].attributes["id"].value;
                    break;
                case "section":
                    tempElement = $(ConfigXml).find("#" + id).children("chapter").first().children("page").first();
                    id = tempElement[0].attributes["id"].value;
                    break;
                case "module":
                    tempElement = $(ConfigXml).find("#" + id).children("section").first().children("chapter").first().children("page").first();
                    id = tempElement[0].attributes["id"].value;
                    break;
            }

            $(PageContent).each(function () {
                if (this.Page === id) {
                    targetPage = this;
                    return false;
                }
            });

            $(".dot[data-page=" + id + "]").addClass("visited");

            // if page has content
            $("#page-content").empty();
            appendStandardContent();

            var previousLocation = CurrentLocation;
            CurrentLocation = targetPage;
            addUserNavigation(previousLocation, CurrentLocation, navDescription);

            closeNav();


            $(targetPage.content).contents().each(function recursivePageLoad() {

                if (!blankTextNode(this)) {

                    var innerPage = $.trim(this.textContent);

                    if (innerPage.indexOf('<text>') === -1 && innerPage.indexOf('</text>') === -1) {
                        // this is a normal, empty node
                        renderElement(this);
                    } else {
                        openDialog("There appears to be a formatting issue with this book. Please contact a system administrator.", "Error");
                    }
                }
            });
            renderInit();
        }
    }
}

function appendStandardContent() {
    $("#page-content").append("<div id='definitionWindow'></div>");
}

function updateNavigation(previousLocation) {

    var parentChapter = $(ConfigXml).find("#" + CurrentLocation.Page).parent("chapter");
    var parentModule = $(ConfigXml).find("#" + CurrentLocation.Page).parent("module");
    var parentSection = $(ConfigXml).find("#" + CurrentLocation.Page).parent("section");

    // check if they're populated
    populateMenus();

    if (typeof previousLocation !== "undefined") {

        if (previousLocation.Module !== CurrentLocation.Module) {
            // changing modules
            $("#module-name").html('');
            
            $("#section-list").empty();

            $("#dot-container").empty();
        }
        if (previousLocation.Section !== CurrentLocation.Section) {
            // changing sections
            $("#chapter-list").empty();

            // get all chapters for this section
            $(".section-item").removeClass("selected");
        } if (previousLocation.Chapter !== CurrentLocation.Chapter) {
            // changing chapters
            
            $(".chapter-item").removeClass("selected");
        }

        populateMenus();
        $(".section-item[data-id='" + CurrentLocation.Section + "']").addClass("selected");
        $(".chapter-item[data-id='" + CurrentLocation.Chapter + "']").addClass("selected");

        // move the page arrow
        var mod = $(ConfigXml).find("#" + CurrentLocation.Module).first();
        var marginCount = 0;
        $(mod).find("page").each(function (k, v) {
            if ($(this).prop('id') === CurrentLocation.Page) {
                $("#arrow-pointer").css('margin-left', marginCount + "px");
                return false;
            }
            marginCount += 30;
        });
        
    } else {
        // use the first page

        // set default section selected
        $("#section-list").find(".section-item[data-id='" + CurrentLocation.Section + "']").first().addClass("selected");
        $("#chapter-list").find(".chapter-item[data-id='" + CurrentLocation.Chapter + "']").first().addClass("selected");
    }

    updateTableOfContents();
    initNavigation();
    selectPageDots();
}



function selectPageDots() {
    // set the current page to active
    $(".dot").removeClass("selected");

    var currentPage = CurrentLocation.Page;

    var page = $(ConfigXml).find("page#" + currentPage).first();
    try {
        if (page.hasClass("hide-page")) {
            var newElement = $(page).prevAll("page").not(".hide-page").first();
            $(".dot[data-page='" + $(newElement).prop('id') + "']").addClass("selected");
        } else {
            $(".dot[data-page='" + CurrentLocation.Page + "']").addClass("selected");
        }
    }
    catch (error) {
        $(".dot[data-page='" + CurrentLocation.Page + "']").addClass("selected");
    }

    

    
}



function populateMenus() {
    var mod = $(ConfigXml).find("#" + CurrentLocation.Module).first();
    var item;
    var arr;
    var i = 0;

    // if module banner is empty
    if ($("#module-name").html() === "") {
        
        var name = mod[0].attributes.name.value;
        var bookName = $(ConfigXml).find(":root").first();
        $("#book-name").html(bookName[0].attributes.name.value);
        $("#module-name").html(name);
        $("#content").removeClass();
        $("#content").addClass("theme" + mod[0].attributes.theme.value);
        //$("#top-bar").css('background-color', color);
        //$("#module-name").css('color', fontColor);
    }

    // check if there's anything in the box
    if ($('#section-list').is(':empty')) {
        // legit starting from scratch

        // find matching module
        arr = $(mod).find("section").map(function () {
            return {
                id: this.attributes.id.value,
                name: this.attributes.name.value
            };
        }).get();

        for (i = 0; i < arr.length; i++) {
            item = $("<div data-id='" + arr[i].id + "'></div>");
            $(item).addClass('section-item');
            $(item).html(arr[i].name);
            $("#section-list").append(item);
        }
    }

    // check if there's anything in the box
    if ($('#chapter-list').is(':empty')) {
        // legit starting from scratch

        // find matching section
        var sec = $(ConfigXml).find("#" + CurrentLocation.Section).first();
        arr = $(sec).find("chapter").map(function () {
            return {
                id: this.attributes.id.value,
                name: this.attributes.name.value
            };
        }).get();

        for (i = 0; i < arr.length; i++) {
            item = $("<div data-id='" + arr[i].id + "'></div>");
            $(item).addClass('chapter-item');
            $(item).html(arr[i].name);
            $("#chapter-list").append(item);
        }
    }

    // check if the page dots are there
    if ($("#dot-container").is(':empty')) {
        // get all pages in this module

        


        var currentSection = "";
        var pageCount = 1;
        $(mod).find("page").each(function (k, v) {

            // hide extra pages
            var classString = this.getAttribute("class");
            if (!classString || classString.indexOf('hide-page') === -1) {
                var dot = $("<div data-page='" + this.attributes.id.value + "' class='dot'></div>");

                // check visited pages
                if (typeof UserTracker.VisitedPages !== "undefined") {
                    if (UserTracker.VisitedPages.indexOf(this.attributes.id.value) !== -1)
                        $(dot).addClass("visited");
                }

                $(dot).html(pageCount);
                if ($(this).closest("section")[0].attributes["id"] !== currentSection) {
                    $(dot).addClass('leftborder');
                    currentSection = $(this).closest("section")[0].attributes["id"];
                }
                $("#dot-container").append(dot);
                pageCount++;
            } 
            
        });

        $("#dot-container").find('.dot').last().addClass('rightborder');

        // check number of pages
        var pageLen = $(".dot").length;
        if (pageLen < 70) {
            $(".dot").css('font-size', '12pt');
            $(".dot").css('width', '20px');
        }
        
    }
    
}

