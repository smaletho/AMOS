var selectedId = null;
var selectedType = null;
var selectedName = null;
initPage();




function initPage() {

    updateItemActionsPosition();
    updateOutlineInteractions();

    $('#ItemActions .name-update').click(function () {
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
        var data = {};
        data.Id = selectedId;
        data.Version = $('#ItemActions [data-type="' + selectedType + '"] .version-input').val();
        data.pageQueryModel = pageQueryModel;
        transmitAction("UpdateBookVersion", updateOutlineResponse, null, 'html', data);
        return false;
    });

    $('#ItemActions .add-button').click(function () {
        var data = {};
        data.Type = selectedType;
        data.Id = selectedId;
        data.pageQueryModel = pageQueryModel;
        transmitAction("AddItem", updateOutlineResponse, null, 'html', data);
        return false;
    });

    $('#ItemActions .remove-button').click(function () {
        if (!confirm("Are you sure that you want to remove this " + selectedType +"?")) return false;
        var data = {};
        data.Type = selectedType;
        data.Id = selectedId;
        data.pageQueryModel = pageQueryModel;
        transmitAction("RemoveItem", updateOutlineResponse, null, 'html', data);
        return false;
    });

    $('#ItemActions .reorder-button').click(function () {
        var data = {};
        data.Type = selectedType;
        data.Id = selectedId;
        data.Action = $(this).data('action');
        data.pageQueryModel = pageQueryModel;
        transmitAction("ReorderItem", updateOutlineResponse, null, 'html', data);
        return false;
    });



}

//function updateNameResponse(data) {
//    $('#BookOutline').html(data);
//    updateOutlineInteractions();
//    hiliteSelectedItem();
//}

function updateOutlineInteractions() {
    $('#BookOutline .item').click(function () {
        var selectModuleHtml = $('#SelectModule').html();
        var selectSectionHtml = $('#SelectSection').html();
        var selectChapterHtml = $('#SelectChapter').html();
        $('#SectionActions .move-input').html(selectModuleHtml);
        $('#ChapterActions .move-input').html(selectSectionHtml);
        $('#PageActions .move-input').html(selectChapterHtml);

        selectedType = $(this).data('type');
        selectedId = $(this).data('id');
        selectedName = $(this).find('.name').text();

        var parentId = $(this).data('parent');
        switch (selectedType) {
            case "section":
                $('#SectionActions .move-select').val(parentId);
                break;
            case "chapter":
                $('#ChapterActions .move-select').val(parentId);
                break;
            case "page":
                $('#PageActions .move-select').val(parentId);
                break;
        }

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
    $('#ItemActions .actions-set').hide();
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
            $('#SectionActions').show();
            break;
        case "chapter":
            $('#ChapterActions').show();
            break;
        case "page":
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