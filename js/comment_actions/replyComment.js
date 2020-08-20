"use strict";

// TODO: change PHOTO

function replyComment(btn) {
    let comment = btn.closest('.blog-comment');
    let commentContainer = comment.querySelector('.text-comment-container');
    let hasCommentForm = false;
    for(let elem of commentContainer.children) {
        if(elem.className === "comment-answer") {
            hasCommentForm = true;
            break;
        }
    }
    // let hasCommentForm = comment.querySelector('.comment-answer');
    if(!hasCommentForm) {
        let commentForm = document.createElement('div');
        commentForm.className = "comment-answer";
        let template = document.getElementsByClassName('template-comment-answer')[0];
        commentForm.innerHTML = template.innerHTML;
        let previousElement = comment.querySelector('.text-comment-container .comment-actions-container');
        previousElement.after(commentForm);
    }

    // template.classList.remove('template-comment-answer');

    // let commentForm = comment.querySelector('.comment-answer');
    // commentForm.style.display = "grid";
}