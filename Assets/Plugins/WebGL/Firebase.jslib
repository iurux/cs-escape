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
        .collection("events_v3")
        .add(data)
        .then(() => {
            console.log("Event written:", eventName);
        })
        .catch((error) => {
            console.error("Firestore write failed:", error);
        });
  },

  RegisterUnloadHandler: function (playerIDPtr, sessionIDPtr, gameStartPtr) {

    const playerID = UTF8ToString(playerIDPtr);
    const sessionID = UTF8ToString(sessionIDPtr);
    const gameStartTime = parseFloat(UTF8ToString(gameStartPtr));

    if (window._sessionHandlerRegistered) return;
    window._sessionHandlerRegistered = true;

    function sendSessionEnd() {

        const totalPlayTime = (performance.now() / 1000) - gameStartTime;

        const data = {
            event_name: "session_end",
            player_id: playerID,
            session_id: sessionID,
            total_play_time: totalPlayTime,
            closed_by: "browser_tab_close",
            server_timestamp: firebase.firestore.FieldValue.serverTimestamp()
        };

        firebase.firestore().collection("events_v3").add(data);
    }

    window.addEventListener("beforeunload", sendSessionEnd);
    window.addEventListener("pagehide", sendSessionEnd);

    document.addEventListener("visibilitychange", function () {
        if (document.visibilityState === "hidden") {
            sendSessionEnd();
        }
    });
  }

});