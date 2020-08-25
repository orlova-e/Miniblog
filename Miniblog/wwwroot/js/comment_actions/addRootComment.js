"use strict";

// ! after jQuery validation

async function addRootComment(btn) {
    let tokenKey = "accessToken";
    const token = localStorage.getItem(tokeyKey);

    let commentForm;
    let nameText;
    let emailText;
    let commentParagraphCollection;
    
    if(!token) {
        commentForm = btn.closest('form.comment-form');
        nameText = commentForm.querySelector('#nameInput').value;
        emailText = commentForm.querySelector('input[type=email]').value;
        commentParagraphCollection = commentForm.querySelector('textarea.comment-input-textarea').value.split("\n");
    }
    else {
        
    }
    



    let template = document.getElementsByClassName('template-blog-comment')[0];
    let newComment = document.createElement('div');
    newComment.className = "blog-comment";
    newComment.innerHTML = template.innerHTML;

    //newComment template editing

    let templateCommentText = newComment.querySelector('.comment-container');
        
    //let commentParagraphCollection = commentText.value.split("\n");
    for(let i = 0; i < commentParagraphCollection.length; i++) {
        let par = document.createElement('p');
        par.innerText = commentParagraphCollection[i];
        templateCommentText.append(par);
    }

    let dateTimeNow = new Date();
    let dateTimeSpan = newComment.querySelector('.comment-time-info');
    dateTimeSpan.innerText = dateTimeNow.getDate() + "." + (dateTimeNow.getMonth()+1) + "." + dateTimeNow.getFullYear() + " " + dateTimeNow.getHours() + ":" + dateTimeNow.getMinutes();

    let userNameMeta = newComment.querySelector('.blog-comment-meta .comment-username');
    userNameMeta.innerText = nameText;
    userNameMeta.setAttribute("href", "#");


    // TODO: SENDING to server
    // * ...

    // TODO: SERVER put link to anonymous user img INDEPENDENTLY

    let commentsContainer = document.querySelector('.blog-comments .article-comments-collection');
    commentsContainer.append(newComment);
}