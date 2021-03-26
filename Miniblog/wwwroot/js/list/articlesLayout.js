"use strict";

const layoutType = "ArticlesLayoutType";
function articlesLayoutVisibility(btn) {    
    let btnImg = btn.querySelector('img');
    let container = document.getElementById("articlesContainer");

    if (btn.id === "RowsArticlesLayout") {
        btnImg.src = "/img/buttons/rows.png";
        let anotherBtn = document.getElementById("GridArticlesLayout");
        let anotherBtnImg = anotherBtn.querySelector('img');
        anotherBtnImg.src = "/img/buttons/grid_nonactive.png";

        if (container.classList.contains("uk-child-width-1-2@m")) {
            container.classList.remove("uk-child-width-1-2@m");
        }
        container.classList.add("uk-child-width-1-1@m");

        if (localStorage.getItem(layoutType) !== "Rows")
            localStorage.setItem(layoutType, "Rows");
    }
    else if (btn.id === "GridArticlesLayout") {
        btnImg.src = "/img/buttons/grid.png";
        let anotherBtn = document.getElementById("RowsArticlesLayout");
        let anotherBtnImg = anotherBtn.querySelector('img');
        anotherBtnImg.src = "/img/buttons/rows_nonactive.png";

        if (container.classList.contains("uk-child-width-1-1@m")) {
            container.classList.remove("uk-child-width-1-1@m");
        }
        container.classList.add("uk-child-width-1-2@m");

        if(localStorage.getItem(layoutType) !== "Grid")
            localStorage.setItem(layoutType, "Grid");
    }
}

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
