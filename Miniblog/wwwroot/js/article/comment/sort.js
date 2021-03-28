"use strict";

function sortComments() {
    let commentsContainer = document.querySelector(".article-comments-collection");

    let sortingType = commentsContainer.dataset.sortingType;

    let comments = commentsContainer.children;

    comments = Match(sortingType, comments);

    commentsContainer.innerHTML = "";
    commentsContainer.append(comments);
}

function Match(sortingType, comments) {
    switch (sortingType) {

    }
}

function MostLiked(comments) {

}

function NewFirst(comments) {

}

function OldFirst(comments) {

}