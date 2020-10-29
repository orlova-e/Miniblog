"use strict";

function addComment(comment) {
    let newComment = createComment(comment);
    if (comment.parentId) {
        let currentDepth = 0;
        let parentId = comment.parentId;
        let parentComment = document.querySelector('div.blog-comment[data-comment-id="' + parentId + '"]');
        let currentComment = parentComment;
        while (currentDepth < depth) {
            currentComment = currentComment.closest('div.blog-comment[data-comment-id="' + parentId + '"]');
            if (!currentComment?.dataset.parentId) {
                break;
            }
            parentId = currentComment.dataset.parentId;
            ++currentDepth;
        }
        if (currentDepth < depth) {
            let commentsCollection = parentComment.querySelector('.users-comment-answer-collection');
            commentsCollection.append(newComment);
        } else {
            let parentCommentsContainer = document.querySelector('.users-comment-answer-collection[data-collection-of-parent="' + comment.parentId + '"]');
            if (!parentCommentsContainer) {
                parentCommentsContainer = document.createElement('div');
                parentCommentsContainer.dataset.collectionOfParent = comment.parentId;
                parentCommentsContainer.className = 'users-comment-answer-collection';
                parentComment.after(parentCommentsContainer);
            }
            parentCommentsContainer.append(newComment);
            //parentComment.after(newComment);
        }
    }
    else {
        document.querySelector(".article-comments-collection").append(newComment);
    }
}
