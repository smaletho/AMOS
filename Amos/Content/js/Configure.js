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
        selectedName = $(this).find('.name').text();
        //itemSelected(type, id, name);
        //selectedId = id;
        //selectedType = type;
        //selectedName = name;
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