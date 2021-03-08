"use strict";

const pagePathName = window.location.pathname;
const queueList = pagePathName.substring(pagePathName.lastIndexOf('/') + 1);
const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl(`/verificationhub?queueList=${queueList}`)
    .build();

hubConnection.serverTimeoutInMilliseconds = 1000 * 60 * 30 * 2;
hubConnection.start();

function acceptOrDelete(btn, action) {
    let item = btn.closest(".verify-result");
    let id = item.dataset.itemId;

    let verifyList = document.getElementById("verifyResultsList");
    let elements = verifyList.getElementsByClassName("verify-result");
    let existElementsIds = [];
    for (let i = 0; i < elements.length; i++) {
        existElementsIds[i] = elements[i].dataset.itemId;
    }
    let index = existElementsIds.indexOf(id);
    existElementsIds.splice(index, 1);

    hubConnection.invoke(action, id, existElementsIds);
}

function acceptEntity(btn) {
    acceptOrDelete(btn, "Accept");
}

function deleteEntity(btn) {
    acceptOrDelete(btn, "Delete");
}

function searchEntities(input) {
    let query = input.value;
    if (query?.length >= 4) {
        hubConnection.invoke("Search", query);
    } else {
        let rows = document.getElementById("searchResultsList");
        rows.style.display = "none";
        let tables = document.getElementById("verifyResultsList");
        tables.style.display = "block";
    }
}

function changeRole(select) {
    let userId = select.closest(".verify-result").dataset.itemId;
    let type = Number(select.value);

    hubConnection.send("ChangeRole", userId, type);
}

hubConnection.on("AcceptedOrDeleted", function (id, next) {
    let items = document.querySelectorAll(`.verify-result[data-item-id="${id}"]`);
    for (let i = 0; i < items.length; i++) {
        items[i].remove();
    }
    if (!next)
        return;

    let pathname = window.location.pathname;
    let queueList = pathname.substring(pathname.lastIndexOf('/') + 1);

    if (queueList == "articles" || queueList == "pages" || queueList == "users" || queueList == "comments") {
        addItemToTable(next);
    } else {
        addItemToList(next, false);
    }
});

hubConnection.on("SearchResults", function (results) {
    let rows = document.getElementById("searchResultsList");
    rows.style.display = "block";
    rows.innerHTML = "";
    for (let result of results) {
        addItemToList(result, true);
    }
});

function addItemToTable(item) {
    let template = document.getElementById("verifyResultsItemsTemplate");
    let row = template.content.querySelector("tr").cloneNode(true);

    row.dataset.itemId = item.id;

    let userLink = row.querySelector("a.verify-user-link");
    userLink.href = window.location.origin + '/' + item.username;
    userLink.textContent = item.username;

    let itemLink = row.querySelector("a.verify-item-link");
    itemLink.href = window.location.origin + '/' + item.link;
    itemLink.querySelector("b").textContent = item.name;

    let matches = row.querySelector(".verify-matches");
    matches.textContent = item.matches;

    let rows = document.getElementById("verifyResultsList").querySelector("tbody");
    rows.append(row);
}

function addItemToList(item, isSearchItem = true) {
    let template = document.getElementById("searchResultsItemsTemplate");
    let row = template.content.querySelector("li").cloneNode(true);

    row.dataset.itemId = item.id;

    let linkTemplate = document.getElementById("resultsLinksTemplates");
    let requiredClass = ".usual-result-item";
    let info;

    let selectElement = row.querySelector(".user-role-select");
    if (!item.role) {
        selectElement?.remove();
    }

    if (!isSearchItem) {
        info = linkTemplate.content.querySelector("a" + requiredClass).cloneNode(true);
        info.href = window.location.origin + '/' + item.link;
        info.querySelector(".item-name").textContent = item.name;
    } else if (isSearchItem) {
        if (queueList == "users") {
            requiredClass = ".user-result-item";

            info = linkTemplate.content.querySelector("a" + requiredClass).cloneNode(true);
            info.href = "/users/" + item.author;

            let img = info.querySelector(".search-item-user-img");
            if (item.avatar)
                img.src = "data:image/jpeg;base64," + item.avatar;
            info.querySelector(".search-item-username").textContent = item.author;

            if (item.role) {
                let options = selectElement.children;
                for (let i = 0; i < options.length; i++) {
                    if (options[i].textContent == item.role) {
                        options[i].setAttribute("selected", true);
                        break;
                    }
                }
            }
        } else if (queueList == "articles") {
            requiredClass = ".article-result-item";

            info = linkTemplate.content.querySelector("a" + requiredClass).cloneNode(true);
            info.href = window.location.origin + '/' + item.link;

            info.querySelector(".article-info-header b").textContent = item.value;

            info.querySelector(".item-name").textContent = item.author;
        } else if (queueList == "comments") {
            info = linkTemplate.content.querySelector("a" + requiredClass).cloneNode(true);

            info.href = window.location.origin + '/' + item.link;

            let text = (item.author ? item.author : "deleted") + ": ";
            if (item.value.length > 50) {
                item.value = item.value.substring(0, 50);
            }
            text += item.value;

            info.querySelector(".item-name").textContent = text;

        } else {
            requiredClass = ".usual-result-item";
            info = linkTemplate.content.querySelector("a" + requiredClass).cloneNode(true);

            if (item.value.length > 50) {
                item.value = item.value.substring(0, 50);
            }

            info.href = window.location.origin + '/' + item.link;
            info.querySelector(".item-name").textContent = item.value;
        }
    }

    row.prepend(info);

    let rows;
    if (isSearchItem) {
        rows = document.getElementById("searchResultsList");
    } else {
        rows = document.getElementById("verifyResultsList").querySelector("tbody");
    }

    rows.append(row);
}
