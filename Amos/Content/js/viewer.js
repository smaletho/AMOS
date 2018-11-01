var pageXml;
var resizeTimer;

function viewerInit() {
    fillPageWithGrid();

    $(".grid-box").hover(function () {
        var x = $(this).data('x');
        var y = $(this).data('y');

        var str = "Top: " + (y*20) + ", Left: " + (x*25);
        $("#xy-coordinates").text(str);


    }, function () { });

    $("#grid-check").change(function () {
        if ($(this).is(':checked'))
            $("#grid-window").show();
        else
            $("#grid-window").hide();
    });

    $("#shell-assets").on('click', function () {
        // get all assets with PageId == 0
        window.location = URL_ViewAssets;
    });

    $("#editor").on('click', function () {
        window.location = URL_ViewEditor;
    });

    // keep aspect ratio
    maintainAspectRatio();

    $(window).on('resize', function (e) {
        clearTimeout(resizeTimer);
        resizeTimer = setTimeout(function () {
            maintainAspectRatio();
        }, 1000);
    });

    $("#submit-page").on('click', function () {
        if (confirm("Would you like to save this page?")) {

            var email = prompt("Please enter your email address for saving.");
            if (email !== null) {

                if ($("#PageName").val().trim() === "") {
                    alert("Please enter a page name.");
                    return false;
                } 


                //.item src
                var arr = $(".item").map(function () { return $(this).prop('src'); }).get();

                var s = new XMLSerializer();
                var str = s.serializeToString(xml);
                var pId = $("#PageId").val();
                var name = $("#PageName").val().trim();
                transmitAction(URL_SavePage, savePageSuccess, savePageFail, "", { xml: str, images: arr, email: email, pageId: pId, name: name }, true);
            }
        }
    });


    // transmit action to get xml
    transmitAction(URL_GetPage, gotPageContent_success, gotPageContent_fail, "xml", null, true);
}

function fillPageWithGrid() {
    for (var i = 0; i < 35; i++) {
        // rows
        for (var ii = 0; ii < 64; ii++) {
            $("#grid-window").append('<div class="grid-box" data-x="' + ii + '" data-y="' + i + '"></div>');
        }
    }

    
}

function gotPageContent_success(data) {
    pageXml = data;
    pageReceived_Render();


    // set the currentlocation variable
    //CurrentLocation.content = xml;
}

function gotPageContent_fail(data) {
    alert("Failed to get page content");
    console.log('failed to get page content', data);
}


function savePageSuccess(data) {
    alert("Page saved.");
    
}

function savePageFail(data) {
    alert("Fail :( Check the console");
    console.log('fail', data);
}


function pageReceived_Render() {
    $("#page-content").empty();
    appendStandardContent();

    // targetPage.content should be just the file contents from above
    $(pageXml).contents().contents().each(function recursivePageLoad() {

        if (!blankTextNode(this)) {
            //var innerPage = $(this).html().trim();
            var innerPage = $.trim(this.textContent);

            if ((innerPage.indexOf('<text>') === -1) && (innerPage.indexOf('</text>') === -1)) {
                // this is a normal, empty node

                renderElement(this);
            } else {
                alert("I found nodes inside of nodes, and I haven't accounted for that...");
                //processTextElement(this);
                //$(this).contents().each(recursivePageLoad);
            }
        }
    });
    renderInit();
    //$("#loading").hide();
}