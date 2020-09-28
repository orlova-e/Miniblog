"use strict";

function selectTheme(btn) {
    let themeSwitcher = document.getElementById("DisplayOptions_ColorTheme");
    let val = String(btn.dataset.themeValue);
    themeSwitcher.setAttribute("value", val);

    let circles = btn.closest("div.article-circles");
    let circlesCollection = circles.querySelectorAll("button");
    for (let i = 0; i < circlesCollection.length; i++) {
        if (circlesCollection[i].classList.contains("selected-circle"))
            circlesCollection[i].classList.remove("selected-circle");
    }
    btn.classList.add("selected-circle");
}