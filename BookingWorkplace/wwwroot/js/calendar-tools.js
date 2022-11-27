function selectTimeFrom() {
    let select = document.getElementById('time-from');
    return select;
}

function selectTimeTo() {
    let select = document.getElementById('time-to');
    return select;
}

function getValueFromTimeFrom() {
    let select = selectTimeFrom();
    return select.value;
}

function getValueFromTimeTo() {
    let select = selectTimeTo();
    return select.value;
}

function shiftValueForTimeTo() {
    let timeFrom = getValueFromTimeFrom();
    let timeTo = getValueFromTimeTo();

    let timeToSelect = selectTimeTo();
    console.log(timeFrom);
    
    timeToSelect.setAttribute('min', timeFrom);
    if (Date.parse(timeFrom) > Date.parse(timeTo)) {
        timeToSelect.value = timeFrom;
    }
    disableAllReserveButton();

}