"use strict";

function openCloseOption(event) {
    let switcher = event.target;
    let elementsFieldset = switcher.closest("fieldset");
    let maybeDependentObjects = elementsFieldset.querySelectorAll("input");
    if(switcher.dataset.displaySwitcher) {
        for(let i = 0; i < maybeDependentObjects.length; i++) {
            if(maybeDependentObjects[i].dataset.displayDependentObject === switcher.dataset.displaySwitcher) {
                if(maybeDependentObjects[i].getAttribute("disabled")) {
                    maybeDependentObjects[i].removeAttribute("disabled");
                }
            }
        }
    }
    else {
        for(let i = 0; i < maybeDependentObjects.length; i++) {
            if(maybeDependentObjects[i].dataset.displayDependentObject) {
                maybeDependentObjects[i].setAttribute("disabled", true);
            }
        }
    }
}

