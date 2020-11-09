"use strict";

//replyCommentForm
function getReplyCommentForm(btn) {
    let comment = btn.closest('.blog-comment');
    //let commentContainer = comment.querySelector('.text-comment-container');
    //let hasCommentForm = false;
    //for (let elem of commentContainer.children) {
    //    if (elem.className === "comment-answer") {
    //        hasCommentForm = true;
    //        break;
    //    }
    //}

    //if (!hasCommentForm)
    if (!comment.querySelector('.text-comment-container .comment-answer')) {
        let template = document.getElementById("commentResponseForm");
        let commentForm = template.content.cloneNode(true);
        let previousElement = comment.querySelector('.text-comment-container .comment-actions-container');
        previousElement.after(commentForm);
    }
}

function removeReplyCommentForm(btn) {
    let comment = btn.closest('.blog-comment');
    let replyForm = comment.querySelector('.comment-answer');
    if (replyForm) {
        replyForm.remove();
    }
}

function getUpdateCommentForm(btn) {
    let comment = btn.closest('div.blog-comment');
    if (!comment.querySelector('form.comment-update-form')) {
        let template = document.getElementById('commentChangeForm').content.cloneNode(true);
        let commentContainer = comment.querySelector('.comment-container');

        let oldContent = document.createElement('div');
        oldContent.className = 'comment-old-content';
        oldContent.hidden = true;
        oldContent.innerHTML = commentContainer.innerHTML;
        commentContainer.after(oldContent);
        commentContainer.innerHTML = '';

        for (let i = 0; i < oldContent.childNodes.length; i++) {
            if (oldContent.childNodes[i].tagName === 'P') {
                template.querySelector('.comment-input-textarea').textContent += oldContent.childNodes[i].textContent.trim();
                if (i !== oldContent.childNodes.length - 1) {
                    template.querySelector('.comment-input-textarea').textContent += '\n';
                }
            }
        }
        //template.querySelector('.comment-input-textarea').textContent = oldContent.innerText;

        commentContainer.append(template);

        // hide action buttons and date
        comment.querySelector('.comment-actions-container').hidden = true;
        comment.querySelector('.comment-time-info').hidden = true;
    }
}

function removeUpdateCommentForm(btn) {
    let comment = btn.closest('div.blog-comment');
    let commentUpdateForm = comment.querySelector('form.comment-update-form');
    if (commentUpdateForm) {
        commentUpdateForm.remove();
        let currentContent = comment.querySelector('.comment-old-content');
        let commentContainer = comment.querySelector('.comment-container');
        commentContainer.innerHTML = currentContent.innerHTML;

        // show action buttons and date
        comment.querySelector('.comment-actions-container').hidden = false;
        comment.querySelector('.comment-time-info').hidden = false;
    }
}
