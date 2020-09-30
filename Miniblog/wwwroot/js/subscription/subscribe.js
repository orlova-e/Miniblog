"use strict";

const subscribeHubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/subscription")
    .build();

subscribeHubConnection.serverTimeoutInMilliseconds = 1000 * 60 * 2 * 15;

function subscribe() {
    let authorName = document.getElementById("author_username").innerText;
    subscribeHubConnection.invoke("Subscribe", authorName);
}

subscribeHubConnection.on("Subscribed", function (statusCode) {
    if (statusCode === 200) {
        let btn = document.getElementById("subscribeButton");
        btn.classList.add("usual-button-reverse");
        btn.innerText = "Subscribed";
        btn.onclick = unsubscribe;
        count();
    }
});

function unsubscribe() {
    let authorName = document.getElementById("author_username").innerText;
    subscribeHubConnection.invoke("Unsubscribe", authorName);
}

subscribeHubConnection.on("Unsubscribed", function (statusCode) {
    if (statusCode === 200) {
        let btn = document.getElementById("subscribeButton");
        btn.classList.remove("usual-button-reverse");
        btn.innerText = "+ Subscribe";
        btn.onclick = subscribe;
        count();
    }
});

function count() {
    let authorName = document.getElementById("author_username").innerText;
    subscribeHubConnection.invoke("Count", authorName);
}

subscribeHubConnection.on("Counted", function (number) {
    let subscribers = document.getElementById("subscribersNumber");
    subscribers.innerText = number;
});

subscribeHubConnection.start();