"use strict";

function createComment(comment) {
    let template = document.getElementById('templateBlogComment');
    let newComment = template.content.querySelector('.blog-comment').cloneNode(true);

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
