
updateItemActionsPosition();

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