"use strict";

function createComment(comment) {
    let template = document.getElementById('templateBlogComment');
    let newComment = template.content.querySelector('div.blog-comment').cloneNode(true);

    newComment.dataset.commentId = comment.commentId;

    if (comment.parentId) {
        newComment.dataset.parentId = comment.parentId;
        newComment.classList.add('parental');
        let parentComment = document.querySelector('.blog-comment[data-comment-id="' + comment.parentId + '"]');
        let parentCommentClasses = parentComment.className;
        let position = parentCommentClasses.indexOf('parental-');
        let number = 0;
        let maximumDepth = document.querySelector('.article-comments-collection').dataset.commentsDepth;
        if (position > 0 && position < maximumDepth) {
            number = parentCommentClasses.substring(position + 'parental-'.length);
        } else if (position >= maximumDepth) {
            number = maximumDepth - 1;
        }
        newComment.classList.add('parental-' + ++number);
    }

    //avatar
    let userAvatar = document.createElement('img');
    if (comment.avatar) {
        userAvatar.alt = comment.author;
        userAvatar.src = "data:image/jpeg;base64," + comment.avatar;
    } else {
        userAvatar.alt = "User";
        let imgTemplate = document.getElementById('anonymousUserImgTemplate').content.querySelector('img');
        let srcValue = imgTemplate.getAttribute('src');
        userAvatar.setAttribute('src', srcValue);
    }
    newComment.querySelector(".user-info-container").append(userAvatar);

    //username
    let origin = window.location.origin;
    let commentData = newComment.querySelector("div.blog-comment-meta");
    commentData.querySelector('a.comment-username').href = origin + "/account/" + comment.author;
    commentData.querySelector('a.comment-username').innerText = comment.author;

    //content
    let textContainer = newComment.querySelector(".comment-container");
    let paragraphs = comment.text.split('\n');
    for (let i = 0; i < paragraphs.length; i++) {
        let par = document.createElement('p');
        par.innerText = paragraphs[i];
        textContainer.append(par);
    }

    //datetime
    newComment.querySelector('.time-info-utc.time-info-created').textContent = comment.dateTime;
    newComment.querySelector('.time-info-utc.time-info-updated').textContent = comment.updatedDateTime ? comment.updatedDateTime : '';
    insertCommentDate(newComment);

    return newComment;
}
