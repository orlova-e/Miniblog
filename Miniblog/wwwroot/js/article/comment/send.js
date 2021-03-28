"use strict";

function sendComment(btn) {
    let text = null;
    let parentId = null;
    if (btn.closest('.root-answer-form')) {
        let commentForm = btn.closest('.root-answer-form');
        text = commentForm.querySelector('textarea').value;
    } else {
        let commentForm = btn.closest('form');
        text = commentForm.querySelector('textarea').value;
        parentId = commentForm.dataset.parentId;
    }

    if (!text)
        return;

    articleHubConnection.invoke("AddComment", text, parentId);
}

function sendCommentUpdates(btn) {
    let comment = btn.closest('.blog-comment');
    let commentId = comment.dataset.commentId;
    let commentElements = comment.querySelector('textarea').value;
    let text = String(commentElements);

    articleHubConnection.invoke("UpdateComment", text, commentId);
}

function deleteActions(btn) {
    let comment = btn.closest('.blog-comment');
    if (comment.querySelector('.comment-update-form')) {
        removeUpdateCommentForm(btn);
    } else {
        let commentId = comment.dataset.commentId;

        articleHubConnection.invoke("DeleteComment", commentId);
    }
}
