"use strict";

function likeArticle() {
    articleHubConnection.invoke("LikeArticle", title);
}

function bookmarkArticle() {
    articleHubConnection.invoke("BookmarkArticle", title);
}
