//"use strict";

////commentSmallForm (btn, replyForm = true)
//function changeCommentForm(btn) {
//    let comment = btn.closest('.blog-comment');
//    //let commentContainer = comment.querySelector('.text-comment-container');
//    //let hasCommentForm = false;
//    //for (let elem of commentContainer.children) {
//    //    if (elem.className === "comment-answer") {
//    //        hasCommentForm = true;
//    //        break;
//    //    }
//    //}

//    /*if (!hasCommentForm)*/
//    if (!comment.querySelector('.text-comment-container .comment-answer')) {
//        let template = document.getElementById("commentResponseForm");
//        let commentForm = template.content.cloneNode(true);
//        let previousElement = comment.querySelector('.text-comment-container .comment-actions-container');
//        previousElement.after(commentForm);
//    }
//}