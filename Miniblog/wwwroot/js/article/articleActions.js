"use strict";

function likeArticle() {
    articleHubConnection.invoke("LikeArticle");
}

function bookmarkArticle() {
    articleHubConnection.invoke("BookmarkArticle");
}
