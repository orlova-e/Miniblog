"use strict";

let articlesLayoutBtn;
if (localStorage.getItem(layoutType) !== null) {
    let id = localStorage.getItem(layoutType) + "ArticlesLayout";
    articlesLayoutBtn = document.getElementById(id);
} else {
    let displayButtons = document.querySelector(".display-character");
    localStorage.setItem(layoutType, displayButtons.dataset.btnActive);
    let id = displayButtons.dataset.btnActive + "ArticlesLayout";
    articlesLayoutBtn = document.getElementById(id);
}

window.addEventListener("load", () => articlesLayoutVisibility(articlesLayoutBtn));
