"use strict";

function replaceComment(comment) {
    let oldComment = document.querySelector('div.blog-comment[data-comment-id="' + comment.dataset.commentId + '"]');
    oldComment.replaceWith(comment);
}