var Internalplugins = {
	SendSongResult: function (songResult) {
		var data = Pointer_stringify(songResult);
		SendSongData(data);
	},
};
mergeInto(LibraryManager.library, Internalplugins);