"use strict";

function hideOptions(btn) {
    let id = btn.dataset.targetId;
    let options = document.getElementById(id);
    let optionsLabel = btn.querySelector(`span.uk-icon`);
    if (options.style.display === "none") {
        options.style.display = "block";
        optionsLabel.setAttribute("uk-icon", "icon: chevron-down");
    } else {
        options.style.display = "none";
        optionsLabel.setAttribute("uk-icon", "icon: chevron-up");
    }
}