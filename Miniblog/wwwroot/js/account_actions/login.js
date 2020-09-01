"use strict";

async function Login() {

    let validationDiv = document.getElementsByClassName("validation-summary-valid")[0];
    if(!validationDiv)
        return;
    if(validationDiv.querySelector("li").innerText)
        return;
    
    let form = document.querySelector(".registration-container form.usual-form");
    let username = form.querySelector("input#Username").value;
    let password = form.querySelector("input#Password").value;

    let verToken = document.getElementById("RequestVerificationToken");

    const response = await fetch("/api/jwtaccount/signin", {
        method: "POST",
        credentials: "include",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json",
            "X-CSRF-TOKEN": verToken.value
        },
        body: form
    });

    if(response.status === 200) {
        localStorage.setItem(tokenKey, data.access_token);
        localStorage.setItem(tokenExpiration, data.access_token_expiration);
        localStorage.setItem(refreshTokenKey, data.refresh_token);
    }

    form.submit();
}

let loginForm = document.querySelector(".signin-container form.usual-form");
loginForm.addEventListener("submit", Login);