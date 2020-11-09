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


articleHubConnection.on("ArticleLikeIsChanged", function (state) {
    let btn = document.querySelector('.article-user-actions .like-action button');
    const inactiveClass = 'article-action-inactive';
    let checked = btn.querySelector('.article-action-checked');
    let unchecked = btn.querySelector('.article-action-unchecked');
    if (state) {
        checked.classList.remove(inactiveClass);
        unchecked.classList.add(inactiveClass);
    } else {
        checked.classList.add(inactiveClass);
        unchecked.classList.remove(inactiveClass);
    }
});

articleHubConnection.on("ArticleLikesCounted", function (currentTitle, number) {
    if (currentTitle === title) {
        let articleLikesNumber = document.getElementById("articleLikesNumber");
        articleLikesNumber.textContent = number;
        let likesInfo = document.getElementById("articleLikesInfo");
        likesInfo.textContent = number;
    }
});

articleHubConnection.on("ArticleBookmarkIsChanged", function (state) {
    let btn = document.querySelector('.article-user-actions .bookmark-action button');
    const inactiveClass = 'article-action-inactive';
    let checked = btn.querySelector('.article-action-checked');
    let unchecked = btn.querySelector('.article-action-unchecked');
    if (state) {
        checked.classList.remove(inactiveClass);
        unchecked.classList.add(inactiveClass);
    } else {
        checked.classList.add(inactiveClass);
        unchecked.classList.remove(inactiveClass);
    }
});

articleHubConnection.on("ArticleBookmarksCounted", function (currentTitle, number) {
    if (currentTitle === title) {
        let articleBookmarksNumber = document.getElementById("articleBookmarksNumber");
        articleBookmarksNumber.textContent = number;
        let bookmarksInfo = document.getElementById("articleBookmarksInfo");
        bookmarksInfo.textContent = number;
    }
})

articleHubConnection.on("CommentLikeIsChanged", function (commentId, state) {
    let comment = document.querySelector('.blog-comment[data-comment-id="' + commentId + '"]');
    let btn = comment.querySelector('button.heart-button');
    const inactiveClass = 'comment-action-inactive';
    let checked = btn.querySelector('.comment-action-checked');
    let unchecked = btn.querySelector('.comment-action-unchecked');
    if (state) {
        checked.classList.remove(inactiveClass);
        unchecked.classList.add(inactiveClass);
    } else {
        checked.classList.add(inactiveClass);
        unchecked.classList.remove(inactiveClass);
    }
});

articleHubConnection.on("CommentLikesCounted", function (commentId, number) {
    let comment = document.querySelector('.blog-comment[data-comment-id="' + commentId + '"]');
    let likesNumberElem = comment.querySelector('.hearts-counter.comment-hearts-count[name="commentLikesNumber"]');
    likesNumberElem.innerText = number;
});

articleHubConnection.start();
