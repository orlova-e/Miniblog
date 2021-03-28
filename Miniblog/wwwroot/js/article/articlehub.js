"use strict";

const title = new URL(window.location.href).searchParams.get("title");
const depth = document.querySelector('.article-comments-collection').dataset?.commentsDepth;

const articleHubConnection = new signalR.HubConnectionBuilder()
    .withUrl(`/articlehub?title=${title}`)
    .build();

articleHubConnection.serverTimeoutInMilliseconds = 1000 * 60 * 2 * 30;

articleHubConnection.start();

articleHubConnection.on("AddedComment", function (newComment, number) {
    addComment(newComment);
    updateArticleCommentsNumber(number);
});

articleHubConnection.on("UpdatedComment", function (updatedComment) {
    updateComment(updatedComment);
});

articleHubConnection.on("DeletedComment", function (deletedComment) {
    deleteComment(deletedComment);
});


articleHubConnection.on("ArticleLikeIsChanged", function (state) {
    let img = document.querySelector(".article-user-actions button.like-action img");
    if (state) {
        img.src = "/img/buttons/heart red.png";
        img.alt = "liked";
    } else {
        img.src = "/img/buttons/heart.png";
        img.alt = "like";
    }
});

articleHubConnection.on("ArticleLikesCounted", function (number) {
    updateArticleLikesNumber(number);
});

articleHubConnection.on("ArticleBookmarkIsChanged", function (state) {
    let img = document.querySelector(".article-user-actions button.bookmark-action img");
    if (state) {
        img.src = "/img/buttons/bookmark-checked.png";
        img.alt = "bookmarked";
    } else {
        img.src = "/img/buttons/bookmark-unchecked.png";
        img.alt = "bookmark";
    }
});

articleHubConnection.on("ArticleBookmarksCounted", function (number) {
    updateArticleBookmarksNumber(number);
})

articleHubConnection.on("CommentLikeIsChanged", function (commentId, state) {
    let comment = document.querySelector('.blog-comment[data-comment-id="' + commentId + '"]');
    let img = comment.querySelector(".heart-button img");
    if (state) {
        img.src = "img/buttons/heart red.png";
        img.alt = "liked";
    } else {
        img.src = "img/buttons/heart.png";
        img.alt = "like";
    }
});

articleHubConnection.on("CommentLikesCounted", function (commentId, number) {
    let comment = document.querySelector(`.blog-comment[data-comment-id="${commentId}"]`);
    let likesNumber = comment.querySelector('.blog-comment-footer [name="commentLikesNumber"]');
    likesNumber.textContent = number;
});
