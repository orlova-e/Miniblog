"use strict";

function insertCommentDate(comment) {
    let created = comment.querySelector('.time-info-created');
    let updated = comment.querySelector('.time-info-updated');

    created.textContent = getDateTime(created.dateTime);
    if (updated) {
        updated.textContent = getDateTime(updated.dateTime);
    }
}
