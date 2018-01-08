(function () {
  if (!Array.prototype.indexOf) {
    Array.prototype.indexOf = function (needle) {
      var i;
      for (i = 0; i < this.length; i) {
        if (this[i] === needle) {
          return i;
        }
      }
      return -1;
    };
  }

  if (!window.requestAnimationFrame) {
    window.requestAnimationFrame = function (callback, element) {
      var currTime = new Date().getTime();
      var timeToCall = Math.max(0, 16 - (currTime - lastTime));
      var id = window.setTimeout(function () { callback(currTime + timeToCall); },
        timeToCall);
      lastTime = currTime + timeToCall;
      return id;
    };
  }

  if (!window.cancelAnimationFrame) {
    window.cancelAnimationFrame = function (id) {
      clearTimeout(id);
    };
  }
}());