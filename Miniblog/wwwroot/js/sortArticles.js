"use strict";
const sortArticlesType = "SortArticlesType";
function sortArticles(btn) {
    // let el = document.elementFromPoint(event.clientX, event.clientX);

    let blogContainer = document.getElementById("articlesContainer");
    let articlesCol = blogContainer.getElementsByTagName('article');
    let articlesCollection = Array.from(articlesCol);
    let sortMethod = btn.innerText;

    let spanText = document.getElementById("sortCharacter");
    spanText.innerText = btn.innerText;

    if(localStorage.getItem(sortArticlesType) !== btn.innerText) {
        localStorage.setItem(sortArticlesType, btn.innerText);
    }

    let articlesSortList = document.getElementById("sortListDropdown");
    if(articlesSortList.style.display !== "none") {
        articlesSortList.style.display = "none";
    }
    
    if(String(sortMethod).includes("First the")) {
        for(let j=articlesCollection.length-1; j>0;j--) {
            let wasPassed=false;
            for(let i = 0; i < j; i++) {
                let stringDate = articlesCollection[i].querySelector('.article-datetime a').innerText;
                let datestrings = stringDate.split(".");
                let newStringDate = datestrings[2] + "-" + datestrings[1] + "-" + datestrings[0];
                let date1 = Date.parse(newStringDate, "YYYY-MM-DD");

                let stringDate2 = articlesCollection[i+1].querySelector('.article-datetime a').innerText;
                let datestrings2 = stringDate2.split(".");
                let newStringDate2 = datestrings2[2] + "-" + datestrings2[1] + "-" + datestrings2[0];
                let date2 = Date.parse(newStringDate2, "YYYY-MM-DD");
                if(sortMethod.includes("new") && (date1 > date2)) {
                    wasPassed=true;
                    let temp = articlesCollection[i];
                    articlesCollection[i] = articlesCollection[i + 1];
                    articlesCollection[i + 1]=temp;
                }
                else if(sortMethod.includes("old") && (date1 < date2)) {
                    wasPassed=true;
                    let temp = articlesCollection[i];
                    articlesCollection[i] = articlesCollection[i + 1];
                    articlesCollection[i + 1]=temp;
                }
            }
            if(!wasPassed)
                break;
        }
    }
    else if(sortMethod.includes("Alphabetically")) {
        for(let j = articlesCollection.length - 1; j > 0; j--) {
            let wasPassed = false;
            for(let i = 0; i < j; i++) {
                let articleHeader1 = articlesCollection[i].querySelector('.article-header b').innerText;
                let articleHeader2 = articlesCollection[i + 1].querySelector('.article-header b').innerText;
                if(!sortMethod.includes("\u2193") && (articleHeader1.localeCompare(articleHeader2) < 0)) {
                    wasPassed = true;
                    let temp = articlesCollection[i];
                    articlesCollection[i] = articlesCollection[i + 1];
                    articlesCollection[i + 1] = temp;
                }
                else if(sortMethod.includes("\u2193") && (articleHeader1.localeCompare(articleHeader2) > 0)) {
                    wasPassed = true;
                    let temp = articlesCollection[i];
                    articlesCollection[i] = articlesCollection[i + 1];
                    articlesCollection[i + 1] = temp;
                }
            }
            if(!wasPassed)
                break;
        }
    }

    let newBlogContainer = document.createElement('div');
    newBlogContainer.className = "blog-container";
    blogContainer.replaceWith(newBlogContainer);
    newBlogContainer.id = "articlesContainer";

    for(let i = 0; i < articlesCollection.length; i++) {
        let newArticle = document.createElement('article');
        newArticle.classList.add('art', 'text-alegreya-style');
        newArticle.innerHTML = articlesCollection[i].innerHTML;
        newBlogContainer.prepend(newArticle);
    }

    if(localStorage.getItem(layoutType) !== null) {
        if(localStorage.getItem(layoutType) === "grid") {
            let btn = document.getElementById("gridArticlesLayout");
            articlesLayoutVisibility(btn);
        }
        else {
            let btn = document.getElementById("rowArticlesLayout");
            articlesLayoutVisibility(btn);
        }
    }
}