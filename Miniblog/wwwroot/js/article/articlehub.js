"use strict";

const title = new URL(window.location.href).searchParams.get("title");
const depth = document.querySelector('div.article-comments-collection').dataset.commentsDepth;

const articleHubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/articlehub")
    .build();

articleHubConnection.serverTimeoutInMilliseconds = 1000 * 60 * 2 * 30;

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

articleHubConnection.start();
