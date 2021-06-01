var Internalplugins = {
	SendSongResult: function (songResult) {
		var data = Pointer_stringify(songResult);
		SendSongData(data);
	},
	UnityToHtmlData: function (songResult) {
		var data = Pointer_stringify(songResult);
		UnityToHtmlData(data);
	},
};
mergeInto(LibraryManager.library, Internalplugins);