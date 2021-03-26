"use strict";

const subscribeHubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/subscription")
    .build();

subscribeHubConnection.serverTimeoutInMilliseconds = 1000 * 60 * 2 * 15;

subscribeHubConnection.on("Counted", function (number) {
    let subscribers = document.getElementById("subscribersNumber");
    subscribers.textContent = number;
});

subscribeHubConnection.start();