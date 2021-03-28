"use strict";

function deleteComment(comment) {
    let deletedComment = createComment(comment);

    let commentText = document.createElement('p');
    commentText.append(document.createElement('i'));
    commentText.querySelector('i').textContent = '[deleted]';

    let textContainer = deletedComment.querySelector('.comment-container');
    textContainer.append(commentText);
    let commentChangeActions = deletedComment.querySelector('.comment-change');
    commentChangeActions.remove();

    replaceComment(deletedComment);
}
