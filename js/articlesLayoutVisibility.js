"use strict";

function articlesLayoutVisibility(btn) {
    let btnImg = btn.getElementsByTagName('img');
    let blogContainer = document.getElementById("articlesContainer");

    if(btn.id === "rowArticlesLayout") {
        btnImg.setAttribute("src", "img/ico/rows2.png");
        let anotherBtn = document.getElementById("gridArticlesLayout");
        let anotherBtnImg = anotherBtn.getElementsByTagName('img');
        anotherBtnImg.setAttribute("src", "img/ico/grid nonactive.png");
        blogContainer.style.gridTemplateColumns = "1fr";
    }
    else if (btn.id === "gridArticlesLayout") {
        btnImg.setAttribute("src", "img/ico/grid.png");
        let anotherBtn = document.getElementById("rowArticlesLayout");
        let anotherBtnImg = anotherBtn.getElementsByTagName('img');
        anotherBtnImg.setAttribute("src", "img/ico/rows2 nonactive.png");
        blogContainer.style.gridTemplateColumns = "2fr";
    }
}