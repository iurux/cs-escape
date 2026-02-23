mergeInto(LibraryManager.library, {
  SendUnityAnalytics: function(eventNamePtr, jsonPtr) {
    var eventName = UTF8ToString(eventNamePtr);
    var jsonData = UTF8ToString(jsonPtr);

    if (window.sendUnityEvent) {
      window.sendUnityEvent(eventName, jsonData);
    }
  }
});