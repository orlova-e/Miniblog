"use strict";

function openList(btn) {
    let maybeMenuList = btn.closest("li.blog-menu-list-point");
    if(maybeMenuList) {
        let innerMenuList = maybeMenuList.querySelector("ul.blog-links-list");
        if(innerMenuList.display.style === "none")
            innerMenuList.style.display = "block";
        else
            innerMenuList.style.display = "none";
    }
}