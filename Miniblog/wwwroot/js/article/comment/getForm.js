"use strict";

function getReplyCommentForm(btn) {
    let comment = btn.closest('.blog-comment');

    let template = document.getElementById("commentResponseForm");
    let commentForm = template.content.querySelector("form").cloneNode(true);

    if (!document.querySelector(`form[data-parent-id="${comment.dataset.commentId}"]`)) {
        commentForm.dataset.parentId = comment.dataset.commentId;
        commentForm.className += " " + comment.className;
        if (!commentForm.classList.contains("parental"))
            commentForm.classList.add("parental");

        if (commentForm.className.includes("parental-")) {
            let position = commentForm.className.indexOf('parental-');
            let maxDepth = document.querySelector('.article-comments-collection').dataset.commentsDepth;
            let number = 0;
            if (position > 0) {
                number = commentForm.className.substring(position + 'parental-'.length);
                if (number >= maxDepth)
                    number = --maxDepth;
                commentForm.classList.remove("parental-" + number);
            }
            
            commentForm.classList.add("parental-" + ++number);
        } else {
            commentForm.classList.add("parental-1");
        }

        comment.after(commentForm);
    }
}

function removeReplyCommentForm(btn) {
    let replyForm = btn.closest('form');
    replyForm?.remove();
}

function getUpdateCommentForm(btn) {
    let comment = btn.closest('.blog-comment');
    if (!comment.querySelector('form.comment-update-form')) {
        let template = document.getElementById('commentChangeForm').content.querySelector("form").cloneNode(true);
        let textContainer = comment.querySelector('.comment-container');
        let formText = template.querySelector("textarea");
        formText.value = "";

        for (let i = 0; i < textContainer.childNodes.length; i++) {
            if (textContainer.childNodes[i].tagName === 'P') {
                formText.value += textContainer.childNodes[i].textContent;
            }
        }

        textContainer.after(template);
        textContainer.style.display = "none";

        comment.querySelector(".comment-time-info").style.visibility = "collapse";
        comment.querySelector('.comment-actions-container').style.visibility = "collapse";
    }
}

function removeUpdateCommentForm(btn) {
    let comment = btn.closest('.blog-comment');
    let commentUpdateForm = comment.querySelector('form.comment-update-form');
    if (commentUpdateForm) {
        commentUpdateForm.remove();

        comment.querySelector(".comment-time-info").style.visibility = "visible";
        comment.querySelector('.comment-actions-container').style.visibility = "visible";

        let textContainer = comment.querySelector('.comment-container');
        textContainer.style.display = "block";
    }
}
