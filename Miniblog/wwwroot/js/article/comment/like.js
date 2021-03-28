"use strict";

function likeComment(btn) {
    let commentId = btn.closest('.blog-comment').dataset.commentId;

    articleHubConnection.invoke("LikeComment", commentId);
}
