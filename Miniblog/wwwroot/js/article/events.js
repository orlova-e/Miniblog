"use strict";

function insertCommentsDates() {
    let comments = document.querySelectorAll('.blog-comment');
    for (let i = 0; i < comments.length; i++) {
        insertCommentDate(comments[i]);
    }
}

window.addEventListener("load", insertArticleDate);
window.addEventListener("load", insertCommentsDates);
