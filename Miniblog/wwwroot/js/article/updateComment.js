"use strict";

function updateComment(updatedComment) {
    //let updatedComment = createComment(comment);
    //replaceComment(updatedComment);

    let comment = document.querySelector('div.blog-comment[data-comment-id="' + updatedComment.commentId + '"]');

    //img
    if (updatedComment.avatar) {
        comment.querySelector('.user-info-container img').src = 'data:image/jpeg;base64,' + updatedComment.avatar;
        comment.querySelector('.user-info-container img').alt = updatedComment.author;
    }
    //username
    comment.querySelector('.blog-comment-meta .comment-username').textContent = updatedComment.author;
    comment.querySelector('.blog-comment-meta .comment-username').href = window.location.origin + /users/ + updatedComment.author;
    //date and time
    comment.querySelector('.comment-time-info').hidden = false;
    comment.querySelector('.comment-time-info').innerHTML = '';
    comment.querySelector('.comment-time-info').textContent = updatedComment.date + ' ' + updatedComment.time
        + ' (updated at ' + updatedComment.updatedDate + ' ' + updatedComment.updatedTime + ')';
    //text
    comment.querySelector('.comment-container').innerHTML = '';
    let paragraphsArray = updatedComment.text.split('\n');
    for (let i = 0; i < paragraphsArray.length; i++) {
        let paragraph = document.createElement('p');
        paragraph.textContent = paragraphsArray[i];
        comment.querySelector('.comment-container').append(paragraph);
    }
    //action buttons visibility
    comment.querySelector('.comment-actions-container').hidden = false;
}

function getUpdateCommentForm(btn) {
    let comment = btn.closest('div.blog-comment');
    if (!comment.querySelector('form.comment-update-form')) {
        let template = document.getElementById('commentChangeForm').content.cloneNode(true);
        let commentContainer = comment.querySelector('.comment-container');

        let oldContent = document.createElement('div');
        oldContent.className = 'comment-old-content';
        oldContent.innerHTML = commentContainer.innerHTML;
        commentContainer.innerHTML = '';

        template.querySelector('.comment-input-textarea').textContent = oldContent.innerText;
        
        commentContainer.append(template);

        // hide action buttons and date
        comment.querySelector('.comment-actions-container').hidden = true;
        comment.querySelector('.comment-time-info').hidden = true;
    }
}

function invokeCommentUpdating(btn) {
    let comment = btn.closest('div.blog-comment');
    let commentId = comment.dataset.commentId;
    let commentElements = comment.querySelector('textarea.comment-input-textarea').value;
    let text = String(commentElements);
    articleHubConnection.invoke("UpdateComment", title, text, commentId);
}

function hideUpdateCommentForm(btn) {
    let comment = btn.closest('div.blog-comment');
    let commentUpdateForm = comment.querySelector('form.comment-update-form');
    if (commentUpdateForm) {
        commentUpdateForm.remove();
        let currentContent = comment.querySelector('.comment-old-content');
        let commentContainer = comment.querySelector('.comment-container');
        commentContainer.innerHTML = currentContent.innerHTML;

        // show action buttons and date
        comment.querySelector('.comment-actions-container').hidden = false;
        comment.querySelector('.comment-time-info').hidden = false;
    }
}