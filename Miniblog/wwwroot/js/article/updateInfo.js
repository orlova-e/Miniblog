"use strict";

function updateArticleLikesNumber(number) {
    let likesNumber = document.getElementById("articleLikesNumber");
    likesNumber.textContent = number;
}

function updateArticleBookmarksNumber(number) {
    let bookmarksNumber = document.getElementById("articleBookmarksNumber");
    bookmarksNumber.textContent = number;
}

function updateArticleCommentsNumber(number) {
    let commentsNumber = document.getElementById("articleCommentsNumber");
    commentsNumber.textContent = number;
}

function insertArticleDate() {
    let articleDateTime = document.querySelector("time.article-time-info");
    articleDateTime.textContent = getDateTime(articleDateTime.dateTime);
}