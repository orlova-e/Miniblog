"use strict";

function sendComment(btn) {
    let text = null;
    let parentId = null;
    if (btn.closest('form.root-answer-form.comment-form')) {
        let commentForm = btn.closest('form.root-answer-form.comment-form');
        text = commentForm.querySelector('textarea.comment-input-textarea').value;
    } else {
        let commentForm = btn.closest('.answer-form.comment-form');
        text = commentForm.querySelector('textarea.comment-input-textarea').value;
        let queryString = 'div.blog-comment';
        parentId = commentForm.closest(queryString).dataset.commentId;
    }
    articleHubConnection.invoke("AddComment", title, text, parentId);
}