"use strict";

function createComment(comment) {
    let template = document.getElementById('templateBlogComment');
    let newComment = template.content.querySelector('div.blog-comment').cloneNode(true);

    newComment.dataset.commentId = comment.commentId;

    if (comment.parentId) {
        newComment.dataset.parentId = comment.parentId;
    }

    let userAvatar = document.createElement('img');
    if (comment.avatar) {
        userAvatar.alt = comment.author;
        userAvatar.src = "data:image/jpeg;base64," + comment.avatar;
    } else {
        userAvatar.alt = "User";
        let imgTemplate = document.getElementById('anonymousUserImgTemplate').content.querySelector('img');
        userAvatar.src = imgTemplate.getAttribute('src');
    }
    newComment.querySelector(".user-info-container").append(userAvatar);

    let origin = window.location.origin;
    let commentData = newComment.querySelector("div.blog-comment-meta");
    commentData.querySelector('a.comment-username').href = origin + "/account/" + comment.author;

    commentData.querySelector('a.comment-username').innerText = comment.author;

    commentData.querySelector('.comment-time-info').innerText = comment.time + " " + comment.date;

    let textContainer = newComment.querySelector(".comment-container");
    let paragraphs = comment.text.split('\n');
    for (let i = 0; i < paragraphs.length; i++) {
        let par = document.createElement('p');
        par.innerText = paragraphs[i];
        textContainer.append(par);
    }

    return newComment;
}
