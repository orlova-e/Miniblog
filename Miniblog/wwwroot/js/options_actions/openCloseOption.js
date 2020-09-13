"use strict";

function openCloseOption(event) {

    let clicked = event.target.querySelector("input");
    if(clicked == null)
        return;

    let elementsFieldset = clicked.closest("fieldset");
    let maybeDependentObjects = elementsFieldset.querySelectorAll("*[data-display-dependent-object]");

    //|| clicked.dataset.displayDependentObject
    if((clicked.dataset.displaySwitcher || clicked.dataset.displayDependentObject)
    && !clicked.hasAttribute("checked")) {
        for(let i = 0; i < maybeDependentObjects.length; i++) {
            if(maybeDependentObjects[i].dataset.displayDependentObject === clicked.dataset.displaySwitcher) {
                if(maybeDependentObjects[i].hasAttribute("disabled") && clicked.dataset.displaySwitcher) {
                    // if(maybeDependentObjects[i].getAttribute())



                    maybeDependentObjects[i].removeAttribute("disabled");
                }
                else{
                    maybeDependentObjects[i].setAttribute("disabled", true);
                }
            }
        }
    }
    else if(!clicked.dataset.displayDependentObject) {
        for(let i = 0; i < maybeDependentObjects.length; i++) {
            if(maybeDependentObjects[i].dataset.displayDependentObject) {
                maybeDependentObjects[i].setAttribute("disabled", true);
            }
        }
    }
}



// function openCloseOption(event) {
//     let clicked = event.target;
//     let clicked;
//     let elementsFieldset = event.target.closest("fieldset");
//     let maybeDependentObjects = elementsFieldset.
//     if(clicked.querySelector("input").dataset.displaySwitcher) {
//         clicked = clicked;
//     }
//     else if(clicked.querySelector("input").dataset.displayDependentObject) {

//     }
//     else if (clicked.querySelector("input")){

//     }
//     let clicked = event.target.querySelector("input");
//     let switcherValue = clicked.dataset.displaySwitcher;
//     maybeDependentObjects = elementsFieldset.querySelectorAll("div[data-display-dependent-object=${`switcherValue`}");


//     if(clicked.dataset.displaySwitcher) {
//         for(let i = 0; i < maybeDependentObjects.length; i++) {
//             if(maybeDependentObjects[i].dataset.displayDependentObject === clicked.dataset.displaySwitcher) {
//                 if(maybeDependentObjects[i].getAttribute("disabled")) {
//                     maybeDependentObjects[i].removeAttribute("disabled");
//                 }
//             }
//         }
//     }
//     else {
//         for(let i = 0; i < maybeDependentObjects.length; i++) {
//             if(maybeDependentObjects[i].dataset.displayDependentObject) {
//                 maybeDependentObjects[i].setAttribute("disabled", true);
//             }
//         }
//     }
// }