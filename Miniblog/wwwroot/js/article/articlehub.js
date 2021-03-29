"use strict";

const currentArticleId = document.querySelector('article').id;
//const title = new URL(window.location.href).searchParams.get("title");
//const depth = document.querySelector('.article-comments-collection')?.dataset?.commentsDepth;

const articleHubConnection = new signalR.HubConnectionBuilder()
    .withUrl(`/articlehub?articleId=${currentArticleId}`)
    .build();

articleHubConnection.serverTimeoutInMilliseconds = 1000 * 60 * 2 * 30;

articleHubConnection.start();

articleHubConnection.on("AddedComment", function (newComment, number) {
    addComment(newComment);
    let commentsNumber = document.getElementById("articleCommentsNumber");
    commentsNumber.textContent = number;
});

articleHubConnection.on("UpdatedComment", function (updatedComment) {
    updateComment(updatedComment);
});

articleHubConnection.on("DeletedComment", function (deletedComment) {
    deleteComment(deletedComment);
});

function likeArticle() {
    articleHubConnection.invoke("LikeArticle");
}

function bookmarkArticle() {
    articleHubConnection.invoke("BookmarkArticle");
}

articleHubConnection.on("ArticleLikeIsChanged", function (articleId, state) {
    if (currentArticleId !== articleId)
        return;

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
    let likesNumber = document.getElementById("articleLikesNumber");
    likesNumber.textContent = number;
});

articleHubConnection.on("ArticleBookmarkIsChanged", function (articleId, state) {
    if (currentArticleId !== articleId)
        return;

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
    let bookmarksNumber = document.getElementById("articleBookmarksNumber");
    bookmarksNumber.textContent = number;
});

function likeComment(btn) {
    let commentId = btn.closest('.blog-comment').dataset.commentId;

    articleHubConnection.invoke("LikeComment", commentId);
}

articleHubConnection.on("CommentLikeIsChanged", function (articleId, commentId, state) {
    if (currentArticleId !== articleId)
        return;

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
