"use strict";

function deleteComment(comment) {
    let deletedComment = createComment(comment);

    //content
    let commentText = document.createElement('p');
    commentText.append(document.createElement('i'));
    commentText.querySelector('i').textContent = 'Deleted';
    let commentTextContainer = deletedComment.querySelector('.comment-container');
    commentTextContainer.append(commentText);
    let commentChangeActions = deletedComment.querySelector('.comment-change');
    commentChangeActions.replaceWith(document.createElement('div'));

    replaceComment(deletedComment);
}
