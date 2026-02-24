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

    data.event_name = eventName;
    data.server_timestamp = firebase.firestore.FieldValue.serverTimestamp();

    firebase.firestore()
        .collection("events")
        .add(data)
        .then(() => {
            console.log("Event written:", eventName);
        })
        .catch((error) => {
            console.error("Firestore write failed:", error);
        });
  }

});