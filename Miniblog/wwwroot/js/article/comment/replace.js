"use strict";

function replaceComment(comment) {
    let oldComment = document.querySelector(`.blog-comment[data-comment-id="${comment.dataset.commentId}"]`);
    oldComment.replaceWith(comment);
}