"use strict";

const usedLanguage = "en";
const shortStyle = "short";

function getDateTime(date) {
	let formatter = new Intl.DateTimeFormat(usedLanguage, {
		timeStyle: shortStyle,
		dateStyle: shortStyle
	});
	let dt = new Date(Date.parse(date));
	return formatter.format(dt);
}

function getDate(date) {
	let formatter = new Intl.DateTimeFormat(usedLanguage, {
		dateStyle: shortStyle
	});
	let dt = new Date(Date.parse(date));
	return formatter.format(dt);
}

function getTime(date) {
	let formatter = new Intl.DateTimeFormat(usedLanguage, {
		timeStyle: shortStyle
	});
	let dt = new Date(Date.parse(date));
	return formatter.format(dt);
}
