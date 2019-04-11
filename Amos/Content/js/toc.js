function initTocLinks() {
    $(".nav-link").on('click', function () {
        // navigate to the page/section/chapter/etc
        var id = $(this).data('id');
        var type = "";

        if ($(this).hasClass("page")) type = "page";
        else if ($(this).hasClass("chapter")) type = "chapter";
        else if ($(this).hasClass("section")) type = "section";
        else if ($(this).hasClass("module")) type = "module";

        loadPage(id, type, "menu navigation");

    });



    
}



function updateTableOfContents() {
    // Update the TOC highlighting and stuff

    $(".page").removeClass('activePage');
    $(".page[data-id='" + CurrentLocation.Page + "']").addClass('activePage');
    

}



function buildTableOfContents() {

    // make ConfigXml into an easier to deal with object to generate the 

    //console.log('all modules', $(ConfigXml).find('chapter').prop('id'));

    // traverse the tree and build the TOC
    var tocHtmlString = "";

    $(ConfigXml).contents().contents().each(function processNodes() {
        // if it's the main book, render different
        if (this.nodeName.toLowerCase() === "module") {

            var id = this.getAttribute('id');
            var type = this.nodeName.toLowerCase();


            var name = this.attributes.name.value;


            tocHtmlString += "<div data-id='" + id + "' class='nav-link " + type + " theme" + this.attributes.theme.value + "'>";

            var newContent = "";
            if (applicationMode !== "offline") {
                newContent = URL_Content;
            } else {
                newContent = "./";
            }

            tocHtmlString += '<div class="color-block"><div class="dark-box"><img src="' + newContent + 'Content/images/left-mask.png" width="20" height="40" /></div><div class="light-box"><img src="' + newContent + 'Content/images/right-mask.png" width="20" height="40" /></div></div>';
            tocHtmlString += "<div class='text'>" + name + "</div>";
            tocHtmlString += "</div>";
        }
    });

    $("#pageList").html(tocHtmlString);

    // subtracting 2 accounts for the border
    $("#toc").height($("#inner-window").height() - 2);
}