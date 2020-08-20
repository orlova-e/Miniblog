"use strict";
async function heartClick(btn) {
    let elem = btn;
    let comment = btn.closest('.blog-comment');
    let whiteHeart=null;
    let redHeart=null;
    if(elem.classList.contains("heart-not-clicked")) {
        whiteHeart = elem;
        redHeart = comment.querySelector('.comment-actions-container .heart-clicked');
    }
    else {
        redHeart = elem;
        whiteHeart = comment.querySelector('.comment-actions-container .heart-not-clicked');
    }
    let counterNumb = comment.querySelector('.comment-actions-container .hearts-counter');
    if(whiteHeart.style.display !== "none") {
        whiteHeart.style.display = "none";
        redHeart.style.display = "inline-block";
        let count = Number(counterNumb.innerText);
        count++;
        counterNumb.innerText = count;
    }
    else if (redHeart.style.display === "inline-block") {
        redHeart.style.display = "none";
        whiteHeart.style.display = "inline-block";
        let count2 = Number(counterNumb.innerText);
        count2--;
        counterNumb.innerText = count2;
  }

    //let tokenKey = "accessToken";
    // const token = localStorage.getItem(tokeyKey);
    // const response = await fetch("/token", {
    //   method: "POST",
    //   headers: {
    //     "Accept": "application/json",
    //     "Authorization": "Bearer " + token
    //     },
    //   body: counterNumb.innerText, comment.id // TODO: add jwt bearer tokens - user identity
    // });

    // if(response.ok === true) {

    // }
    // else if(response.status === "401") {

    // }
    // else {

    // }
}
