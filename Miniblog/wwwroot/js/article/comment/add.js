"use strict";

function addComment(comment) {
    let newComment = createComment(comment);
    if (comment.parentId) {
        let parentComment = document.querySelector('.blog-comment[data-comment-id="' + comment.parentId + '"]');
        parentComment.after(newComment);
        if (parentComment.querySelector('.text-comment-container .comment-answer')) {
            let answerContainer = parentComment.querySelector('.text-comment-container .comment-answer');
            answerContainer.remove();
        }
    }
    else {
        document.querySelector(".article-comments-collection").append(newComment);
        let noCommentsMsg = document.getElementById('noCommentsMessage');
        if (noCommentsMsg) {
            let commentsMsg = document.createElement('h3');
            commentsMsg.id = 'commentsAreExistMessage';
            commentsMsg.textContent = 'Comments';
            noCommentsMsg.replaceWith(commentsMsg);
        }
        let commentAnswerForm = document.querySelector('.root-comment-answer.comment-answer');
        if (commentAnswerForm) {
            commentAnswerForm.querySelector('textarea').value = '';
        }
    }
}
