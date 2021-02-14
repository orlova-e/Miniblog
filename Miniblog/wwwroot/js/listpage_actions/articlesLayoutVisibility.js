"use strict";
const layoutType = "ArticlesLayoutType";
function articlesLayoutVisibility(btn) {    
    let btnImg = btn.querySelector('img');
    let blogContainer = document.getElementById("articlesContainer");

    if (btn.id === "RowsArticlesLayout") {
        btnImg.src = "../img/ico/rows.png";
        let anotherBtn = document.getElementById("GridArticlesLayout");
        let anotherBtnImg = anotherBtn.querySelector('img');
        anotherBtnImg.src = "../img/ico/grid_nonactive.png";
        blogContainer.style.gridTemplateColumns = "1fr";
        if (localStorage.getItem(layoutType) !== "Rows")
            localStorage.setItem(layoutType, "Rows");
    }
    else if (btn.id === "GridArticlesLayout") {
        btnImg.src = "../img/ico/grid.png";
        let anotherBtn = document.getElementById("RowsArticlesLayout");
        let anotherBtnImg = anotherBtn.querySelector('img');
        anotherBtnImg.src = "../img/ico/rows_nonactive.png";
        blogContainer.style.gridTemplateColumns = "1fr 1fr";
        if(localStorage.getItem(layoutType) !== "Grid")
            localStorage.setItem(layoutType, "Grid");
    }
}