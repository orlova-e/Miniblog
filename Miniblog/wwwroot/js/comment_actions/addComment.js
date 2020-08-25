"use strict";

// TODO: COMMENT gets ID from server and should include it to HTML

async function addComment(btn) {
    let formToSend = btn.closest('.answer-form.comment-form');
    let commentText = formToSend.querySelector('.comment-input-textarea');
    if(commentText.value.length === 0) {
        let errorMsg = formToSend.querySelector('span.comment-error-message');
        if(errorMsg.style.display === "none")
            errorMsg.style.display = "inline";
    }
    else {
        let errorMsg = formToSend.querySelector('span.comment-error-message');
        if(errorMsg.style.display === "inline")
            errorMsg.style.display = "none";
        
        let hiddenInput = formToSend.querySelector('input[type="hidden"]');
        let userName = hiddenInput.value;



        let newComment = document.createElement('div');
        newComment.className = "blog-comment";
        let template = document.getElementsByClassName('template-blog-comment')[0];
        newComment.innerHTML = template.innerHTML;

        // newComment editing
        let templateCommentText = newComment.querySelector('.comment-container');
        
        let paragraphCollection = commentText.value.split("\n");
        for(let i = 0; i < paragraphCollection.length; i++) {
            let par = document.createElement('p');
            par.innerText = paragraphCollection[i];
            templateCommentText.append(par);
        }

        let dateTimeNow = new Date();
        let dateTimeSpan = newComment.querySelector('.comment-time-info');
        dateTimeSpan.innerText = dateTimeNow.getDate() + "." + (dateTimeNow.getMonth()+1) + "." + dateTimeNow.getFullYear() + " " + dateTimeNow.getHours() + ":" + dateTimeNow.getMinutes();

        let userNameMeta = newComment.querySelector('.blog-comment-meta .comment-username');
        userNameMeta.innerText = userName;
        userNameMeta.setAttribute("href", "/accounts/" + userName);


        let commentContainer = formToSend.closest('.blog-comment .text-comment-container');
        let commentsCollection = commentContainer.querySelector('.users-comment-answer-collection');

        commentsCollection.append(newComment);

        // let tokenKey = "accessToken";
        // const token = localStorage.getItem(tokeyKey);
        // const response = await fetch("/api/addcomment", {
        //   method: "POST",
        //   headers: {
        //     "Accept": "application/json",
        //     "Authorization": "Bearer " + token
        //     },
        //   body: JSON.stringify(userName, commentText.innerText)
        //    // TODO: add jwt bearer tokens - user identity
        // });

        // if(response.ok === true) {

        // }
        // else if(response.status === "401") {

        // }
        // else {

        // }

        commentText.value = "";
    }
}