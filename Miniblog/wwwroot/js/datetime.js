"use strict";

const currentLanguage = "language";

function getLanguage() {
	let language;
	if (navigator.languages != undefined) {
		if (navigator.languages[0] === 'ru-RU' || navigator.languages[0] === 'ru')
			language = 'ru';

	} else {
		if (navigator.language === 'ru-RU' || navigator.language === 'ru')
			language = 'ru';
	}
	if (!language)
		language = 'en';
	sessionStorage.setItem(currentLanguage, language);
	//return language;
}

window.addEventListener("load", getLanguage);

function getDateTime(date) {
	let language = sessionStorage.getItem(currentLanguage);
	let formatter = new Intl.DateTimeFormat(language, {
		timeStyle: 'short',
		dateStyle: 'short'
	});
	let dt = new Date(Date.parse(date));
	return formatter.format(dt);
}

function getDate(date) {
	let language = sessionStorage.getItem(currentLanguage);
	let formatter = new Intl.DateTimeFormat(language, {
		dateStyle: 'short'
	});
	let dt = new Date(Date.parse(date));
	return formatter.format(dt);
}

function getTime(date) {
	let language = sessionStorage.getItem(currentLanguage);
	let formatter = new Intl.DateTimeFormat(language, {
		timeStyle: 'short'
	});
	let dt = new Date(Date.parse(date));
	return formatter.format(dt);
}
