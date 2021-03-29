"use strict";

function replaceComment(comment) {
    let oldComment = document.querySelector(`.blog-comment[data-comment-id="${comment.dataset.commentId}"]`);
    oldComment.replaceWith(comment);
}

function createComment(comment) {
    let template = document.getElementById('templateBlogComment');

    let newComment;
    if (comment.requirements) {
        newComment = template.content.querySelector('.blog-comment.own-comment').cloneNode(true);
    } else {
        newComment = template.content.querySelector('.blog-comment.not-own-comment').cloneNode(true);
    }

    newComment.dataset.commentId = comment.commentId;

    if (comment.parentId) {
        newComment.dataset.parentId = comment.parentId;
        newComment.classList.add('parental');
        let parentComment = document.querySelector(`.blog-comment[data-comment-id="${comment.parentId}"]`);
        let position = parentComment.className.indexOf('parental-');
        let number = 0;
        let maxDepth = document.querySelector('.article-comments-collection').dataset.commentsDepth;
        if (position > 0) {
            number = parentComment.className.substring(position + 'parental-'.length);
            if (number > maxDepth)
                number = --maxDepth;
        }

        newComment.classList.add('parental-' + ++number);
    }

    let userAvatar = newComment.querySelector(".uk-comment-avatar");
    if (comment.avatar) {
        userAvatar.alt = comment.author;
        userAvatar.src = "data:image/jpeg;base64," + comment.avatar;
    }
    newComment.querySelector(".user-info-container").append(userAvatar);

    let origin = window.location.origin;
    let commentData = newComment.querySelector(".comment-username");
    commentData.querySelector("a").href = origin + "/users/" + comment.author;
    commentData.querySelector("a").textContent = comment.author;

    let textContainer = newComment.querySelector(".comment-container");
    let paragraphs = comment.text.split('\n');
    for (let i = 0; i < paragraphs.length; i++) {
        let paragraph = document.createElement('p');
        paragraph.textContent = paragraphs[i];
        textContainer.append(paragraph);
    }

    let created = newComment.querySelector('.time-info-created');
    let updated = newComment.querySelector('.time-info-updated');
    created.dateTime = comment.dateTime;
    if (comment.updatedDateTime) {
        updated.dateTime = comment.updatedDateTime;
    } else {
        let updatedText = newComment.querySelector(".datetime-updated-text");
        updatedText.remove();
    }
    
    insertCommentDate(newComment);

    return newComment;
}

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

function updateComment(updatedComment) {
    let comment = createComment(updatedComment);
    replaceComment(comment);
}

function deleteComment(comment) {
    let deletedComment = createComment(comment);

    let commentText = document.createElement('p');
    commentText.append(document.createElement('i'));
    commentText.querySelector('i').textContent = '[deleted]';

    let textContainer = deletedComment.querySelector('.comment-container');
    textContainer.append(commentText);
    let commentChangeActions = deletedComment.querySelector('.comment-change');
    commentChangeActions?.remove();

    replaceComment(deletedComment);
}
