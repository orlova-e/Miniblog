"use strict";

if(localStorage.getItem(sortArticlesType) !== null) {
    let btns = document.querySelectorAll("#sortListDropdown button");
    for(let i = 0; i< btns.length; i++) {
        if(btns[i].innerText.includes(localStorage.getItem(sortArticlesType))) {
            btns[i].innerText = localStorage.getItem(sortArticlesType);
            window.addEventListener("load", () => sortArticles(btns[i]));
            break;
        }
    }
}

if(window.onload == null) {
    if(localStorage.getItem(layoutType) !== null) {
        if(localStorage.getItem(layoutType) === "grid") {
            let btn = document.getElementById("gridArticlesLayout");
            window.addEventListener("load", () => articlesLayoutVisibility(btn));
        }
        else if(localStorage.getItem(layoutType) === "row") {
            let btn = document.getElementById("rowArticlesLayout");
            window.addEventListener("load", () => articlesLayoutVisibility(btn));
        }
    }
}

window.addEventListener("scroll", scrollPageFunction);
window.addEventListener("scroll", closeSortList);

window.addEventListener("click", windowClickEvent.bind(event));