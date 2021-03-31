"use strict";

function showLabel(input) {
    let label = document.querySelector(`label[for="${input.id}"]`);
    if (input.value.length > 0) {
        label.style.visibility = "visible";
    } else {
        label.style.visibility = "collapse";
    }
}