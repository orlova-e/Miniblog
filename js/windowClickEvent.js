"use strict";

function windowClickEvent(event) {
    let targetElement = event.target;
    let maybeSortList = targetElement.closest(".drop-menu");
    if(!maybeSortList) {
        closeSortList();
    }
}