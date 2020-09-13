"use strict";

function selectTheme(btn) {
    let themeSwitcher = document.getElementById("ColorTheme");
    themeSwitcher.setAttribute("value", btn.id);
    let circles = btn.closest("div.article-circles");
    let circlesCollection = circles.querySelectorAll("button");
    for(let i = 0; i < circlesCollection.length; i++) {
        if(circlesCollection[i].classList.contains("selected-circle"))
            circlesCollection[i].classList.remove("selected-circle");
    }
    btn.classList.add("selected-circle");
}