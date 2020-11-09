"use strict";

function updateArticleLikesNumber(number) {
    let commentsNumber = document.getElementById("articleLikesNumber");
    commentsNumber.textContent = number;
}

function updateArticleBookmarksNumber(number) {
    let commentsNumber = document.getElementById("articleBookmarksNumber");
    commentsNumber.textContent = number;
}

function updateArticleCommentsNumber(number) {
    let commentsNumber = document.getElementById("articleCommentsNumber");
    commentsNumber.textContent = number;
    let commentsInfo = document.getElementById("articleCommentsInfo");
    commentsInfo.textContent = number;
}

function insertArticleDate() {
    let dtCreatedStr = document.querySelector('.time-info-utc.time-info-created').textContent;
    if (dtCreatedStr) {
        let date = getDateTime(dtCreatedStr);
        document.querySelector('.article-time-info').textContent = date;
    }
}