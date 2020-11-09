"use strict";

function insertCommentDate(comment) {
    let dtCreatedStr = comment.querySelector('.time-info-utc.time-info-created').textContent;
    let dtUpdatedStr = comment.querySelector('.time-info-utc.time-info-updated').textContent;
    let date = getDateTime(dtCreatedStr);
    if (dtUpdatedStr) {
        date += ' (updated at ' + getDateTime(dtUpdatedStr) + ')';
    }
    comment.querySelector('.comment-time-info.time-info').textContent = date;
}
