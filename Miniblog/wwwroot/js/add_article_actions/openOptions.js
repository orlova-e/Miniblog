"use strict";

function openOptions(btn) {
    let maybeOptionsFieldset = btn.closest("fieldset");
    if(maybeOptionsFieldset) {
        let displayOptions = maybeOptionsFieldset.querySelector("div.article-display-options");
        if(displayOptions.style.display === "none")
            displayOptions.style.display = "block";
        else
            displayOptions.style.display = "none";
    }
}