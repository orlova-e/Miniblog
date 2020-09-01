"use strict";

function logOut() {

    localStorage.removeItem(tokenKey);
    localStorage.removeItem(tokenExpiration);
    localStorage.removeItem(refreshTokenKey);
}