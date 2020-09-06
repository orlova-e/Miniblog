"use strict";

function hideOptions(event) {
    let type = event.target.tagName;
    if (type.toLowerCase() !== "input") {
        return;
    }
    if (event.target.hasAttribute("data-display-switcher") && event.target.checked === true) {
        let switcherValue = event.target.dataset.displaySwitcher;
        if (switcherValue) {
            let field = event.target.closest("fieldset");
            let dependentObjectContainer = field.querySelector(`div[data-display-dependent-object=${switcherValue}]`);
            let inputs = dependentObjectContainer.querySelectorAll("*");
            for (let i = 0; i < inputs.length; i++) {
                if (inputs[i].tagName.toLowerCase() === "input" || inputs[i].tagName.toLowerCase() === "select") {
                    if (inputs[i].hasAttribute("disabled")) {
                        inputs[i].removeAttribute("disabled");
                    }
                }
            }
        }
    }
    else if (!event.target.closest("div[data-display-dependent-object]") && !event.target.closest("div[data-display-dependent-object=ignore]")) {
        let field = event.target.closest("fieldset");
        let dependentObjectContainer = field.querySelector("div[data-display-dependent-object]");
        let inputs = dependentObjectContainer.querySelectorAll("*");
        for (let i = 0; i < inputs.length; i++) {
            if ((inputs[i].tagName.toLowerCase() === "input" || inputs[i].tagName.toLowerCase() === "select") && !inputs[i].closest("div[data-display-dependent-object=ignore]")) {
                if (!inputs[i].hasAttribute("disabled")) {
                    inputs[i].setAttribute("disabled", true);
                }
            }
        }
    }
}