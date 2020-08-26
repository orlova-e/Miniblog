"use strict";
const layoutType = "ArticlesLayoutType";
function articlesLayoutVisibility(btn) {    
    let btnImg = btn.querySelector('img');
    let blogContainer = document.getElementById("articlesContainer");

    if(btn.id === "rowArticlesLayout") {
        btnImg.src = "../img/ico/rows2.png";
        let anotherBtn = document.getElementById("gridArticlesLayout");
        let anotherBtnImg = anotherBtn.querySelector('img');
        anotherBtnImg.src = "../img/ico/grid nonactive.png";
        blogContainer.style.gridTemplateColumns = "1fr";
        if(localStorage.getItem(layoutType) !== "row")
            localStorage.setItem(layoutType, "row");
    }
    else if (btn.id === "gridArticlesLayout") {
        btnImg.src = "../img/ico/grid.png";
        let anotherBtn = document.getElementById("rowArticlesLayout");
        let anotherBtnImg = anotherBtn.querySelector('img');
        anotherBtnImg.src = "../img/ico/rows2 nonactive.png";
        blogContainer.style.gridTemplateColumns = "1fr 1fr";
        if(localStorage.getItem(layoutType) !== "grid")
            localStorage.setItem(layoutType, "grid");
    }
}