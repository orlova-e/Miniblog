"use strict";

function scrollPageFunction() {
    var winScroll = document.body.scrollTop || document.documentElement.scrollTop;
    var height = document.documentElement.scrollHeight - document.documentElement.clientHeight;
    var scrolled = (winScroll / height) * 100;
    document.getElementById("readBar").style.width = scrolled + "%";
}

window.addEventListener("scroll", scrollPageFunction);