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
    if (index >= 0) {
        existElementsIds.splice(index, 1);
    }

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
        rows.style.visibility = "collapse";
        let table = document.getElementById("verifyResultsList");
        table.style.visibility = "visible";
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

    if (queueList == "articles" || queueList == "pages" || queueList == "users" || queueList == "comments") {
        addItemToTable(next);
    } else {
        addItemToList(next, false);
    }
});

hubConnection.on("SearchResults", function (results) {
    let rows = document.getElementById("searchResultsList");
    if (results?.length > 0) {
        rows.style.visibility = "visible";
    } else {
        rows.style.visibility = "collapse";
    }
    let rowsTBody = rows.querySelector("tbody");
    rowsTBody.innerHTML = "";
    for (let result of results) {
        addItemToList(result, true);
    }
});

function addItemToTable(item) {
    let row = createTableRow(item);
    let rows = document.getElementById("verifyResultsList").querySelector("tbody");
    rows.append(row);
}

function createTableRow(item) {
    let template = document.getElementById("verifyResultsItemsTemplate");
    let row = template.content.querySelector("tr").cloneNode(true);

    row.dataset.itemId = item.id;

    let userLink = row.querySelector("a.verify-user-link");
    if (item.author) {
        userLink.href = window.location.origin + "/users/" + item.author;
        userLink.querySelector("b").textContent = item.author;
    } else {
        let italic = document.createElement("i");
        italic.textContent = "[deleted]";
        let userLinkInner = userLink.querySelector("b");
        userLinkInner.append(italic);
    }

    let itemLink = row.querySelector("a.verify-item-link");
    itemLink.href = window.location.origin + '/' + item.link;
    itemLink.textContent = item.value;

    let matches = row.querySelector(".verify-matches");
    matches.textContent = item.matches;

    return row;
}

function addItemToList(item, isSearchItem = true) {
    let row = createListRow(item);
    let id = isSearchItem ? "searchResultsList" : "verifyResultsList";
    let rows = document.getElementById(id).querySelector("tbody");
    rows.append(row);
}

function createListRow(item, isSearchItem = true) {
    let template = document.getElementById("searchResultsItemsTemplate");
    let row = template.content.querySelector("tr").cloneNode(true);

    row.dataset.itemId = item.id;

    let selectElement = row.querySelector(".user-role-select");
    if (!item.role) {
        selectElement?.remove();
    }

    let linkTemplate = document.getElementById("resultsLinksTemplates");
    let requiredClass = ".usual-result-item";
    let info = linkTemplate.content.querySelector("td" + requiredClass).cloneNode(true);
    let infoLink = info.querySelector("a");

    if (!isSearchItem) {
        infoLink.href = window.location.origin + '/' + item.link;
        infoLink.querySelector(".item-name").textContent = item.value;
    } else {
        if (queueList == "users") {
            requiredClass = ".user-result-item";
            info = linkTemplate.content.querySelector("td" + requiredClass).cloneNode(true);
            infoLink = info.querySelector("a");
            infoLink.href = "/users/" + item.author;

            if (item.avatar) {
                let img = infoLink.querySelector(".search-item-user-img");
                img.src = "data:image/jpeg;base64," + item.avatar;
            }

            infoLink.querySelector(".search-item-username").textContent = item.author;

            if (item.role) {
                let options = selectElement.querySelector("select").options;
                for (let i = 0; i < options.length; i++) {
                    if (options[i].textContent == item.role) {
                        options[i].setAttribute("selected", true);
                        break;
                    }
                }
            }
        } else if (queueList == "articles") {
            requiredClass = ".article-result-item";
            info = linkTemplate.content.querySelector("td" + requiredClass).cloneNode(true);
            infoLink = info.querySelector("a");
            infoLink.href = window.location.origin + '/' + item.link;

            infoLink.querySelector(".article-info-header").textContent = item.value;

            if (item.author) {
                infoLink.querySelector(".item-name").textContent = item.author;
            } else {
                let italic = document.createElement("i");
                italic.textContent = "[deleted]";
                let itemName = infoLink.querySelector(".item-name");
                itemName.innerHTML = "";
                itemName.append(italic);
            }
        } else if (queueList == "comments") {
            infoLink.href = window.location.origin + '/' + item.link;

            let text = (item.author ? item.author : "[deleted]") + ": ";
            if (item.value.length > 50) {
                item.value = item.value.substring(0, 50);
            }
            text += item.value;
            infoLink.textContent = text;
        } else {
            if (item.value.length > 50) {
                item.value = item.value.substring(0, 50);
            }
            infoLink.href = window.location.origin + (item.link ? "/" + item.link : "#");
            infoLink.textContent = item.value;
        }
    }

    row.prepend(info);
    return row;
}