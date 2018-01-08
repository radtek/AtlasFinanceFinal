(function (angular, $) {
  'use strict';

  var module = angular.module('fixate.services', []);

  module.service('$keyCode', function () {
    function isArrow(e) {
      var charCode = e.which || e.keyCode;
      return (charCode >= 37 && charCode <= 40);
    }

    function isDeleter(e) {
      var charCode = e.which || e.keyCode;
      return charCode == 8 || charCode == 46;
    }

    function isTab(e) {
      var charCode = e.which || e.keyCode;
      return charCode == 9;
    }

    function isEditer(e) {
      return isArrow(e) || isDeleter(e) || isTab(e);
    }

    function isNumberEdit(e) {
      var charCode = e.which || e.keyCode;
      return !((charCode < 96 || charCode > 105) && // Numpad
            (charCode < 48 || charCode > 57) && // Numbers
            (charCode < 37 || charCode > 40) && // Arrow keys
            (charCode < 112 || charCode > 123) && // F keys
             charCode != 16 && charCode != 17 && charCode != 18 && // Modifiers              
             charCode != 8 && charCode != 9 && charCode != 46 ||   // Escape & tab
            e.shiftKey);
    }

    return {
      isArrow: isArrow,
      isEditer: isEditer,
      isTab: isTab,
      isDeleter: isDeleter,
      isNumberEdit: isNumberEdit
    };
  });

  module.service('$smoothScroll', function () {
    return {
      scrollTo: function (posY, options, selector) {
        selector || (selector = 'html, body');
        var a1type = typeof options; 
        if (a1type == 'string') {
          selector = options;
          options = { duration: 1000 };
        } else if (a1type == 'number') {
          options = { duration: options };
        }

        if (typeof posY == 'object') {
          posY = posY.offset ? posY.offset().top : posY.top;
        }

        if (options.offsetY) {
          posY += options.offsetY;
        }
        
        $(selector).animate({
          scrollTop: posY
        }, options.duration);
      }
    };
  });

}(this.angular, this.jQuery));