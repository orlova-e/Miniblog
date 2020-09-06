"use strict";

function openList(btn) {
    let maybeMenuList = btn.closest("li.blog-menu-list-point");
    if (maybeMenuList) {
        let innerMenuList = maybeMenuList.querySelector("ul.blog-links-list");
        if (innerMenuList.style.display === "none")
            innerMenuList.style.display = "block";
        else
            innerMenuList.style.display = "none";
    }
}