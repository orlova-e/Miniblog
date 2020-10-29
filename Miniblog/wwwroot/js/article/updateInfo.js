"use strict";

function updateArticleLikesNumber(number) {
    let commentsNumber = document.getElementById("articleLikesNumber");
    commentsNumber.innerText = number;
}

function updateArticleBookmarksNumber(number) {
    let commentsNumber = document.getElementById("articleBookmarksNumber");
    commentsNumber.innerText = number;
}

function updateArticleCommentsNumber(number) {
    let commentsNumber = document.getElementById("articleCommentsNumber");
    commentsNumber.innerText = number;
}

function updateCommentLikesNumber(commentId, number) {
    let comment = document.getElementById(commentId);
    let likesNumberElem = comment.querySelector('span.hearts-counter.comment-hearts-count[name="commentLikesNumber"]');
    likesNumberElem.innerText = number;
}