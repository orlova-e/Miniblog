"use strict";

function sortArticlesList() {
    let dropdownList = document.getElementById("sortListDropdown");
    if(dropdownList.style.display !== "block"){
        dropdownList.style.display = "block";
    }
    else {
        dropdownList.style.display = "none";
    }
}