function assignButton(buttonId, onPageId, toPageId, element) {
    $(element).closest("ul").prev("button").button("loading");
    $("a.dropdown-item").prop('disabled', true);
    transmitAction(URL_AssignButton, function (data) {
        //$("#button-table").empty();
        //$("#button-table").append(data);
        $(element).closest("ul").prev("button").button("reset");
        setTimeout(function () {
            $(element).closest("ul").prev("button").text(data);
            $("a.dropdown-item").prop('disabled', false);
        }, 100);
        
    }, enableDisable_fail, "", { buttonId: buttonId, onPageId: onPageId, toPageId: toPageId }, true);
}

function enableDisable_fail(data) {
    $("a.dropdown-item").prop('disabled', false);
    alert("Error");
    console.log("error", data);
}