"use strict";

function updateComment(updatedComment) {
    let comment = createComment(updatedComment);
    replaceComment(comment);

    //let comment = document.querySelector('div.blog-comment[data-comment-id="' + updatedComment.commentId + '"]');

    ////img
    //if (updatedComment.avatar) {
    //    comment.querySelector('.user-info-container img').src = 'data:image/jpeg;base64,' + updatedComment.avatar;
    //    comment.querySelector('.user-info-container img').alt = updatedComment.author;
    //}
    ////username
    //comment.querySelector('.blog-comment-meta .comment-username').textContent = updatedComment.author;
    //comment.querySelector('.blog-comment-meta .comment-username').href = window.location.origin + /users/ + updatedComment.author;
    ////date and time
    //comment.querySelector('.comment-time-info').hidden = false;
    //comment.querySelector('.comment-time-info').innerHTML = '';
    //comment.querySelector('.comment-time-info').textContent = updatedComment.date + ' ' + updatedComment.time
    //    + ' (updated at ' + updatedComment.updatedDate + ' ' + updatedComment.updatedTime + ')';
    ////text
    //comment.querySelector('.comment-container').innerHTML = '';
    //let paragraphsArray = updatedComment.text.split('\n');
    //for (let i = 0; i < paragraphsArray.length; i++) {
    //    let paragraph = document.createElement('p');
    //    paragraph.textContent = paragraphsArray[i];
    //    comment.querySelector('.comment-container').append(paragraph);
    //}
    ////action buttons visibility
    //comment.querySelector('.comment-actions-container').hidden = false;
}


