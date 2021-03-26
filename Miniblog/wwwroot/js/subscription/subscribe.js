"use strict";

const subscribeHubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/subscription")
    .build();

subscribeHubConnection.serverTimeoutInMilliseconds = 1000 * 60 * 2 * 15;

function subscribe() {
    let authorName = document.getElementById("authorUsername").textContent;
    subscribeHubConnection.invoke("Subscribe", authorName);
}

subscribeHubConnection.on("Subscribed", function (wasSubscribed) {
    if (!wasSubscribed)
        return;

    let btn = document.getElementById("subscribeButton");
    btn.classList.remove("uk-button-secondary");
    btn.textContent = "Subscribed";
    btn.onclick = unsubscribe;
    count();
});

function unsubscribe() {
    let authorName = document.getElementById("authorUsername").textContent;
    subscribeHubConnection.invoke("Unsubscribe", authorName);
}

subscribeHubConnection.on("Unsubscribed", function (wasUnsubscribed) {
    if (!wasUnsubscribed)
        return;

    let btn = document.getElementById("subscribeButton");
    btn.classList.add("uk-button-secondary");
    btn.textContent = "+ Subscribe";
    btn.onclick = subscribe;
    count();
});

function count() {
    let authorName = document.getElementById("authorUsername").textContent;
    subscribeHubConnection.invoke("Count", authorName);
}

subscribeHubConnection.on("Counted", function (number) {
    let subscribers = document.getElementById("subscribersNumber");
    subscribers.textContent = number;
});

subscribeHubConnection.start();