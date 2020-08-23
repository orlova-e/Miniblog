"use strict";

function closeSortList() {
    let sortList = document.getElementById("sortListDropdown");
    if(sortList.style.display !== "none") {
        sortList.style.display = "none";
    }
}