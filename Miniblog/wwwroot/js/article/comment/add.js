"use strict";

function addComment(comment) {
    let newComment = createComment(comment);
    if (comment.parentId) {
        let parentComment = document.querySelector('.blog-comment[data-comment-id="' + comment.parentId + '"]');
        parentComment.after(newComment);

        let commentForm = document.querySelector(`form[data-parent-id="${comment.parentId}"]`);
        commentForm.remove();
    } else {
        document.querySelector(".article-comments-collection").append(newComment);
        let noCommentsMessage = document.getElementById('noCommentsMessage');
        if (noCommentsMessage) {
            let commentsMessage = document.createElement('h3');
            commentsMessage.id = 'commentsAreExistMessage';
            commentsMessage.textContent = 'Comments';
            noCommentsMessage.replaceWith(commentsMessage);
        }
        let commentAnswerForm = document.querySelector('.root-answer-form');
        if (commentAnswerForm) {
            commentAnswerForm.querySelector('textarea').value = '';
        }
    }
}
