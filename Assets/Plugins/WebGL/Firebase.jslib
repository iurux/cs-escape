mergeInto(LibraryManager.library, {

  SendUnityAnalytics: function (eventNamePtr, jsonPtr) {

    const eventName = UTF8ToString(eventNamePtr);
    const jsonString = UTF8ToString(jsonPtr);

    let data = {};

    try {
        data = JSON.parse(jsonString);
    } catch (e) {
        console.error("JSON parse error:", e);
    }

    firebase.analytics().logEvent(eventName, data);
  }

});