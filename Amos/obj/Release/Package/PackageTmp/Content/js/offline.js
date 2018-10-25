function offlineLoadInit() {

    $("#loading").show();

    updateLoadingText('Loading file');
    ConfigXml = offline_ConfigXml;

    processOfflinePageContents();
}


function processOfflinePageContents() {
    // offline_PageContents is a string.
    // first, parse it into JSON
    PageContent = JSON.parse(offline_PageContents);

    // call to config2.js
    //  fills offline_PageGuts
    loadGuts();

    if (PageContent.length != offline_PageGuts.length) {
        alert("Fail!");
        return false;
    } else {
        // now loop through offline_PageGuts and update PageContent.content
        for (var i = 0; i < PageContent.length; i++) {
            PageContent[i].content = offline_PageGuts[i];
        }

        updateLoadingText('Finished processing book');

        secondInit();

        //setTimeout(function () {
        //    updateLoadingText('');
        //    $("#loading").hide();
        //    $("#main-window").css('display', 'table');

        //    secondInit();
        //}, 1000);
    }

}

//function processOfflineXml() {
//    // replace all "page" nodes with empty ones that reference other objects
//    PageContent = [];


//    $(ConfigXml).contents().each(function processNodes() {
        
//        // check for empty node
//        if (!blankTextNode(this)) {
//            if (this.nodeName.toLowerCase() === "page") {
//                // I'm cutting out all the stuff page contents when it can be sent from the server
//                var id = $(this).prop('id');

//                var parentChapter = $(this).parent("chapter");
//                var parentSection = $(parentChapter).parent("section");
//                var parentModule = $(parentSection).parent("module");

//                PageContent.push({
//                    Module: $(parentModule).prop('id'),
//                    Section: $(parentSection).prop('id'),
//                    Chapter: $(parentChapter).prop('id'),
//                    Page: id,
//                    content: this.cloneNode(true),
//                });

//                // make an empty page piece
//                var blankPage = document.createElement("page");
//                $(blankPage).prop('id', id);
//                this.parentNode.replaceChild(blankPage, this);

//            }

//            // recurse
//            $(this).contents().each(processNodes);
//        }
//    });

//    updateLoadingText('Finished processing book');

//    setTimeout(function () {
//        updateLoadingText('');
//        $("#loading").hide();
//        $("#main-window").css('display', 'table');

//        secondInit();
//    }, 1000);

//    // process the pages now
//    //  to: base.js
//    //processPages(PageContent);

    
//}