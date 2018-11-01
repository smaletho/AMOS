var selectedId = null;
var selectedType = null;
//var selectedName = null;
var loadingActive = false;
initPage();




function initPage() {

    updateItemActionsPosition();
    updateOutlineInteractions();

    $('#ItemActions .name-update').click(function () {
        showLoading();
        var data = {};
        data.Type = selectedType;
        data.Id = selectedId;
        data.Name = $('#ItemActions [data-type="' + selectedType + '"] .name-input').val();
        data.pageQueryModel = pageQueryModel;
        //alert(data.Type + ' ' + data.Id + ' ' + data.Name);
        transmitAction("UpdateName", updateOutlineResponse, null, 'html', data);
        return false;
    });

    $('#ItemActions .version-update').click(function () {
        showLoading();
        var data = {};
        data.Id = selectedId;
        data.Version = $('#ItemActions [data-type="' + selectedType + '"] .version-input').val();
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
        data.TargetParentId = $('#ItemActions [data-type="' + selectedType + '"] .move-select').val();
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
        data.Theme = theme;
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

//function updateNameResponse(data) {
//    $('#BookOutline').html(data);
//    updateOutlineInteractions();
//    hiliteSelectedItem();
//}

function updateOutlineInteractions() {
    $('#BookOutline .item').click(function () {
        selectedType = $(this).data('type');
        selectedId = $(this).data('id');

        hiliteSelectedItem();
        showActionsForSelectedItem();
    });
}


//function itemSelected(type, id, name) {
//    selectedId = id;
//    selectedType = type;
//    selectedName = name;
//    hiliteSelectedItem();
//    showActionsForSelectedItem();
//}

function hiliteSelectedItem() {
    $('#BookOutline .item').removeClass('selectedItem');
    $('#BookOutline .item[data-type="' + selectedType + '"][data-id="' + selectedId + '"]').addClass('selectedItem');
}

function showActionsForSelectedItem() {
    //alert(selectedType + ' ' + selectedId);
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


function updateOutline() {
    transmitAction("BookOutline", updateOutlineResponse, null, 'html', pageQueryModel);
}

function updateOutlineResponse(data) {
    $('#BookOutline').html(data);
    updateOutlineInteractions();
    hiliteSelectedItem();
    hideLoading();
}


$(window).scroll(function () {
    updateItemActionsPosition();
});
$(window).resize(function () {
    updateItemActionsPosition();
});

function updateItemActionsPosition() {
    var scrollY = $(window).scrollTop();
    var outlineX = $('#BookOutline').position().left;
    var outlineY = $('#BookOutline').position().top;
    var outlineWidth = $('#BookOutline').outerWidth();
    $('#ItemActions').css('position', 'fixed');
    $('#ItemActions').css('left', outlineX + outlineWidth);
    $('#ItemActions').css('top', Math.max(outlineY - scrollY, 0));
}

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