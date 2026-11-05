(function () {
  "use strict";

  const status = document.getElementById("status");

  function post(type, payload, requestId) {
    if (!window.chrome || !window.chrome.webview) {
      return;
    }

    window.chrome.webview.postMessage({
      type: type,
      requestId: requestId || null,
      payload: payload || {}
    });
  }

  function setStatus(text) {
    if (status) {
      status.textContent = text;
    }
  }

  window.byteViewObservatory = {
    sendOffset: function (byteOffset, bitIndex, requestId) {
      post("page.offset", {
        byteOffset: String(byteOffset),
        bitIndex: bitIndex
      }, requestId);
    },
    sendProgress: function (completed, total, isComplete, requestId) {
      post("page.progress", {
        completed: String(completed),
        total: String(total),
        isComplete: Boolean(isComplete)
      }, requestId);
    },
    sendError: function (message, requestId) {
      post("page.error", { message: String(message) }, requestId);
    }
  };

  if (window.chrome && window.chrome.webview) {
    window.chrome.webview.addEventListener("message", function (event) {
      const message = event.data || {};
      const payload = message.payload || {};

      switch (message.type) {
        case "host.hello":
          setStatus("WebView2 bridge ready");
          break;
        case "host.offset":
          setStatus("Offset " + String(payload.byteOffset || "0") + ":" + String(payload.bitIndex || 0));
          break;
        case "host.progress":
          setStatus("Progress " + String(payload.completed || "0") + " / " + String(payload.total || "0"));
          break;
        case "host.error":
          setStatus("Error: " + String(payload.message || "Unknown error"));
          break;
      }
    });
  }

  post("page.ready", { protocolVersion: 1 });
}());
