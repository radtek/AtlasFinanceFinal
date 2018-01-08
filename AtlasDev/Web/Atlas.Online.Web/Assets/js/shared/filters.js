(function (angular) {
  'use strict';

  var module = angular.module('fixate.filters', []);

  // Extension of the currency filter to include param for number of decimal places. 
  module.filter('f8currency', ['$filter', '$locale', function ($filter, $locale) {
    var currencyFilter = $filter('currency'),
        formats = $locale.NUMBER_FORMATS;

    return function (amount, currencySymbol, decimalPlaces) {
      if (angular.isUndefined(decimalPlaces)) { decimalPlaces = 2; }

      var value = currencyFilter(amount, currencySymbol),
          sep = value.indexOf(formats.DECIMAL_SEP);

      if (decimalPlaces > 0) { decimalPlaces++; }

      return value.substring(0, sep + decimalPlaces);
    };
  }]);

  module.filter('f8NumberRange', function() {
    return function (value, min, max) {      
      if (value < min) {
        return min;
      }

      if (value > max) {
        return max;
      }

      return value;
    };
  });

  var paddingFunc = function (padFn) {
    return function (value, char, num) {
      num = num || 10;
      char = char || '0';
      value = value + '';

      num -= value.length;
      while ((num--) > 0) {
        value = padFn(value, char);
      }
      return value;
    };
  };

  module.filter('f8padleft', function () {
    return paddingFunc(function (value, char) { return char + value; });
  });

  module.filter('f8padright', function () {
    return paddingFunc(function (value, char) { return value + char; });
  });

}(this.angular));