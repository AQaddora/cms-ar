mergeInto(LibraryManager.library, {
  GetInitData: function () {
    if (window["openModelLoader"] == undefined)
      openModelLoader = false;
    var returnStr = JSON.stringify({
        lats: lats, 
        lons: lons
    });
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
  },
  HideLoading: function() {
    loading.style.display = "none";
  },
  ShowLoading: function() {
    loading.style.display = "";
  }
});