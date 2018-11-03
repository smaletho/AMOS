// THESE VARIABLES ARE PRESISTED THROUGHOUT THE PAGE LIFE-CYCLE:
// (all other varibles are local to their functions)
var selectedId = null;
var selectedType = null;
var loadingActive = false;
// Note: there are additional javascript variables set within the HTML view. (These are found at the bottom of the "index.cshtml" view file.) These values are set up and in place BEFORE any of the following code executes.


// ======================================
// EXECUTED ONCE ON INITIAL PAGE LOAD:
// (1) misc setup
// (1) Attach functionality to controls.
// (2) If needed, restore the previously selcted item. (highlight item in outline; display associated controls)
// ======================================
initPage();

function initPage() {

    // set up window scroll behavior to lock right column elements in place: 
    $(window).scroll(function () {
        updateRightColumnPosition();
    });
    $(window).resize(function () {
        updateRightColumnPosition();
    });

    updateRightColumnPosition();

    // apply initial functionality to book outline line items:
    updateOutlineInteractions();

    // set up filter controls:
    $('#ShowPageContent').prop("checked", pageQueryModel.ShowPageContent);

    $('#ShowPageContent').change(function () {
        pageQueryModel.ShowPageContent = $('#ShowPageContent').prop('checked');
        transmitAction("BookOutline", updateOutlineResponse, null, 'html', pageQueryModel);
    });

    // ---- apply actions to all right-column page controls: ----

    $('#ItemActions .name-update').click(function () {
        showLoading();
        var data = {};
        data.Type = selectedType;
        data.Id = selectedId;
        data.Text = $('#ItemActions [data-type="' + selectedType + '"] .name-input').val();
        data.pageQueryModel = pageQueryModel;
        transmitAction("UpdateName", updateOutlineResponse, null, 'html', data);
        return false;
    });

    $('#ItemActions .version-update').click(function () {
        showLoading();
        var data = {};
        data.Id = selectedId;
        data.Text = $('#ItemActions [data-type="' + selectedType + '"] .version-input').val();
        data.pageQueryModel = pageQueryModel;
        transmitAction("UpdateBookVersion", updateOutlineResponse, null, 'html', data);
        return false;
    });

    $('#ItemActions .add-button').click(function () {
        showLoading();
        var data = {};
        data.Type = selectedType;
        data.Id = selectedId;
        data.pageQueryModel = pageQueryModel;
        transmitAction("AddItem", updateOutlineResponse, null, 'html', data);
        return false;
    });

    $('#ItemActions .remove-button').click(function () {
        if (!confirm("Are you sure that you want to remove this " + selectedType + "?")) return false;
        showLoading();
        var data = {};
        data.Type = selectedType;
        data.Id = selectedId;
        data.pageQueryModel = pageQueryModel;
        transmitAction("RemoveItem", updateOutlineResponse, null, 'html', data);
        return false;
    });

    $('#ItemActions .reorder-button').click(function () {
        showLoading();
        var data = {};
        data.Type = selectedType;
        data.Id = selectedId;
        data.Action = $(this).data('action');
        data.pageQueryModel = pageQueryModel;
        transmitAction("ReorderItem", updateOutlineResponse, null, 'html', data);
        return false;
    });

    $('#ItemActions .move-button').click(function () {
        showLoading();
        var data = {};
        data.Type = selectedType;
        data.Id = selectedId;
        data.TargetId = $('#ItemActions [data-type="' + selectedType + '"] .move-select').val();
        data.pageQueryModel = pageQueryModel;
        transmitAction("MoveItem", updateOutlineResponse, null, 'html', data);
        return false;
    });

    $('#ItemActions .theme-button').click(function () {
        var theme = $('#BookOutline .module[data-id="' + selectedId + '"]').data('theme');
        $('#ThemeSelector .themeBlock').removeClass('selected');
        $('#ThemeSelector .themeBlock[data-id="' + theme + '"]').addClass('selected');
        $('#ThemeSelector').show();
        return false;
    });
    $('#ThemeSelector .overlay').click(function () {
        $('#ThemeSelector').hide();
        return false;
    });
    $('#ThemeSelector .themeBlock').click(function () {
        var theme = $(this).data('id');
        $('#ThemeSelector').hide();
        showLoading();
        var data = {};
        data.Id = selectedId;
        data.Text = theme;
        data.pageQueryModel = pageQueryModel;
        transmitAction("UpdateTheme", updateOutlineResponse, null, 'html', data);
        return false;
    });

    $('#PreviewButton').click(function () {
        sessionStorage.setItem('ActivePageId', selectedId);
        window.location = viewPageUrl.replace('0', selectedId);
        return false;
    });
    $('#EditButton').click(function () {
        sessionStorage.setItem('ActivePageId', selectedId);
        window.location = editPageUrl.replace('0', selectedId);
        return false;
    });

    // ====================
    // restore selected item if returning from a page view/edit:
    var activePageId = sessionStorage.getItem('ActivePageId');
    if (activePageId !== null) {
        selectedId = activePageId;
        selectedType = "page";
        hiliteSelectedItem();
        showActionsForSelectedItem();
    }
    sessionStorage.removeItem('ActivePageId');
    // ====================
}


// ======================================
// EXECUTED WHENEVER THE BOOK OUTLINE IS REFRESHED:
//  Attach functionality to every line item in the outline:
// ======================================
function updateOutlineInteractions() {
    $('#BookOutline .item').click(function () {
        selectedType = $(this).data('type');
        selectedId = $(this).data('id');

        hiliteSelectedItem();
        showActionsForSelectedItem();
    });
}


// ======================================
// Update visual highlight of selected item in book outline: 
// (Executed whenever the user clicks a line item in the book outline.)
// ======================================
function hiliteSelectedItem() {
    $('#BookOutline .item').removeClass('selectedItem');
    $('#BookOutline .item[data-type="' + selectedType + '"][data-id="' + selectedId + '"]').addClass('selectedItem');
}

// ======================================
// Set up and display controls in the right column that go with the currently selected item in the book outline:
// (Executed whenever the user clicks a line item in the book outline.)
// ======================================
function showActionsForSelectedItem() {
    $('#ItemActions .actions-set').hide();
    var selectedName = $('#BookOutline [data-type="' + selectedType + '"][data-id="' + selectedId + '"] .name').text();
    var parentId = $('#BookOutline [data-type="' + selectedType + '"][data-id="' + selectedId + '"]').data('parent');
    $('#ItemActions [data-type="' + selectedType + '"] .name-input').val(selectedName);
    switch (selectedType) {
        case "book":
            var version = $('#BookOutline .book .version span').text();
            $('#BookActions .version-input').val(version);
            $('#BookActions').show();
            break;
        case "module":
            $('#ModuleActions').show();
            break;
        case "section":
            var selectModuleHtml = $('#SelectModule').html();
            $('#SectionActions .move-input').html(selectModuleHtml);
            $('#SectionActions .move-select').val(parentId);
            $('#SectionActions').show();
            break;
        case "chapter":
            var selectSectionHtml = $('#SelectSection').html();
            $('#ChapterActions .move-input').html(selectSectionHtml);
            $('#ChapterActions .move-select').val(parentId);
            $('#ChapterActions').show();
            break;
        case "page":
            var selectChapterHtml = $('#SelectChapter').html();
            $('#PageActions .move-input').html(selectChapterHtml);
            $('#PageActions .move-select').val(parentId);
            $('#PageActions').show();
            break;
    }
}


// ======================================
// Executed at the conclusion of each user action... after the server has responded with a refreshed view of the book outline:
// ======================================
function updateOutlineResponse(data) {
    $('#BookOutlineView').html(data);
    updateOutlineInteractions();
    hiliteSelectedItem();
    hideLoading();
}


// ======================================
// Executed whenever the user changes the page scroll position or window shape. Display right column components locked in place:
// (When IE is dead, this can be done with pure CSS {position:sticky;})
// ======================================
function updateRightColumnPosition() {
    var scrollY = $(window).scrollTop();
    var outlineX = $('#BookOutline').position().left;
    var outlineY = $('#BookOutline').position().top;
    var outlineWidth = $('#BookOutline').outerWidth();
    $('#ItemActions').css('position', 'fixed');
    $('#ItemActions').css('left', outlineX + outlineWidth);
    $('#ItemActions').css('top', Math.max(outlineY - scrollY, 0));
}

// ======================================
// Shorthand function for our ajax transmissions to the server:
// ======================================
function transmitAction(action, successCallback, errorCallback, returnDataType, data) {
    $.ajax({
        url: actionTransmitUrl.replace('action', action),
        data: JSON.stringify(data),
        type: 'POST',
        dataType: returnDataType,
        contentType: "application/json",
        success: successCallback,
        error: errorCallback
    });
}

// ======================================
// Show and Hide the "UPDATING..." gray-out of the LEFT COLUMN whenever an update to the book outline has been requested.
// There is a slight delay, so this will only appear for longer refreshes.
// ======================================
function showLoading() {
    loadingActive = true;
    window.setTimeout(showLoadingDelay, 1500);
}
function showLoadingDelay() {
    if (loadingActive) {
        $('#BookOutline').addClass('loading-fade');
        $('#Loading').show();
    }
}
function hideLoading() {
    loadingActive = false;
    $('#BookOutline').removeClass('loading-fade');
    $('#Loading').hide();
}