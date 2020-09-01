"use strict";

//function getCookie(cname) {
//    var name = cname + "=";
//    var decodedCookie = decodeURIComponent(document.cookie);
//    var ca = decodedCookie.split(';');
//    for (var i = 0; i < ca.length; i++) {
//        var c = ca[i];
//        while (c.charAt(0) == ' ') {
//            c = c.substring(1);
//        }
//        if (c.indexOf(name) == 0) {
//            return c.substring(name.length, c.length);
//        }
//    }
//    return "";
//}

async function Register(event) {
    event.preventDefault();

    let validationDiv = document.getElementsByClassName("validation-summary-valid")[0];
    if(!validationDiv)
        return;
    if(validationDiv.querySelector("li").innerText)
        return;

    let form = document.querySelector(".registration-container form.usual-form");
    let username = form.querySelector("input#Username").value;
    let email = form.querySelector("input#Email").value;
    let password = form.querySelector("input#Password").value;
    let passwordConfirmation = form.querySelector("input#PasswordConfirmation").value;

    //var csrfToken = getCookie("CSRF-TOKEN");
    let verToken = document.getElementById("RequestVerificationToken");

    const response = await fetch("/api/jwtaccount/signup", {
        method: "POST",
        credentials: "include",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json",
            "X-CSRF-TOKEN": verToken.value
        },
        body: JSON.stringify({
            Username: username,
            Email: email,
            Password: password,
            PasswordConfirmation: passwordConfirmation
        })
    });

    if (response.ok === true) {
        const data = await response.json();
        localStorage.setItem(tokenKey, data.access_token);
        localStorage.setItem(tokenExpiration, data.access_token_expiration);
        localStorage.setItem(refreshTokenKey, data.refresh_token);

        let hiddenInput = form.querySelector("input#userId");
        hiddenInput.value = data.id;
    }

    form.submit();
}

let registerForm = document.querySelector(".registration-container form.usual-form");
registerForm.addEventListener("submit", Register.bind(event));