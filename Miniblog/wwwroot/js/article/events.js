"use strict";

function insertArticleDate() {
    let articleDateTime = document.querySelector("time.article-time-info");
    articleDateTime.textContent = getDateTime(articleDateTime.dateTime);
}

function insertCommentsDates() {
    let comments = document.querySelectorAll('.blog-comment');
    for (let i = 0; i < comments.length; i++) {
        insertCommentDate(comments[i]);
    }
}

function insertCommentDate(comment) {
    let created = comment.querySelector('.time-info-created');
    let updated = comment.querySelector('.time-info-updated');

    created.textContent = getDateTime(created.dateTime);
    if (updated) {
        updated.textContent = getDateTime(updated.dateTime);
    }
}

window.addEventListener("load", insertArticleDate);
window.addEventListener("load", insertCommentsDates);
