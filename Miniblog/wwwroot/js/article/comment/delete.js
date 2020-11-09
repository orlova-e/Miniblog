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

    //let comment = document.querySelector('div.blog-comment[data-comment-id="' + deletedComment.commentId + '"]');

    ////avatar
    //if (deletedComment.avatar) {
    //    comment.querySelector('.user-info-container img').src = 'data:image/jpeg;base64,' + deletedComment.avatar;
    //    comment.querySelector('.user-info-container img').alt = deletedComment.author;
    //}
    ////username
    //comment.querySelector('.blog-comment-meta .comment-username').textContent = deletedComment.author;
    //comment.querySelector('.blog-comment-meta .comment-username').href = window.location.origin + /users/ + deletedComment.author;
    ////date and time
    //comment.querySelector('.comment-time-info').hidden = false;
    //comment.querySelector('.comment-time-info').innerHTML = '';
    //comment.querySelector('.comment-time-info').textContent = deletedComment.date + ' ' + deletedComment.time
    //    + '(updated at ' + deletedComment.updatedDate + ' ' + deletedComment.updatedTime;
    ////text
    //comment.querySelector('.comment-container').innerHTML = '';
    //let iElem = document.createElement('i');
    //i.textContent = 'Deleted';
    //let paragraph = document.createElement('p');
    //paragraph.append(iElem);
    //comment.querySelector('.comment-container').append(paragraph);
    ////action buttons visibility
    //comment.querySelector('.comment-actions-container').hidden = false;
    //comment.querySelector('.comment-actions-container .hearts button').onclick = null;
}
