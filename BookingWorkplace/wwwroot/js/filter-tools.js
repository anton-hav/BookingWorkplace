$(function () {
    $('[data-toggle="tooltip"]').tooltip()
})

function selectAllReserveButton() {
    let selects = document.getElementsByName("reserve-button");
    return selects;
}

function selectAllTooltipSpans() {
    let selects = document.getElementsByClassName("d-inline-block");
    return selects;
}

function disableAllReserveButton() {
    let buttons = selectAllReserveButton();
    let tooltips = selectAllTooltipSpans();

    for (let i = 0; i < buttons.length; i++) {
        buttons[i].setAttribute('disabled', '');
        tooltips[i].setAttribute('data-toggle', 'tooltip');
        tooltips[i].setAttribute('data-placement', 'top');
        tooltips[i].setAttribute('title', 'Press Submit to apply the filter settings.');
        $('[data-toggle="tooltip"]').tooltip();
    }
}