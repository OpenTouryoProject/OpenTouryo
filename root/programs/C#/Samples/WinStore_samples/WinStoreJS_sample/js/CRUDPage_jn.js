function DOMContentLoaded() {
    WinJS.UI.processAll();
}

(function () {
    "use strict";

    if (document.addEventListener) {
        document.addEventListener('DOMContentLoaded', DOMContentLoaded);
    }
})();
