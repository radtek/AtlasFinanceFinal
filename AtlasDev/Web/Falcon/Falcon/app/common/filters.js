(function(angular) {
    'use strict';

    var module = angular.module('falcon.filters', []);

    module.filter('capitalize', function() {
        return function(input, scope) {
            if (input !== null)
                return input.substring(0, 1).toUpperCase() + input.substring(1);
        };
    });

    module.filter('array', function() {
        return function(items) {
            var filtered = [];
            angular.forEach(items, function(item) {
                filtered.push(item);
            });
            return filtered;
        };
    });

    module.filter('padleft', function() {
        return paddingFunc(function(value, char) {
            return char + value;
        });
    });

    module.filter('padright', function() {
        return paddingFunc(function(value, char) {
            return value + char;
        });
    });

    module.filter('titlecase', function() {
        return function(input) {
            if (input) {
                var words = input.split(' ');
                for (var i = 0; i < words.length; i++) {
                    words[i] = words[i].toLowerCase(); // lowercase everything to get rid of weird casing issues
                    words[i] = words[i].charAt(0).toUpperCase() + words[i].slice(1);
                }
                return words.join(' ');
            }
        };
    });

    // Use:
    // {{var|truncate:5}
    module.filter('truncate', function() {
        return function(text, length, end) {
            if (isNaN(length))
                length = 10;

            if (end === undefined)
                end = "...";

            if (text.length <= length || text.length - end.length <= length) {
                return text;
            } else {
                return String(text).substring(0, length - end.length) + end;
            }

        };
    });

    module.filter('percentage', ['$filter', function ($filter) {
        return function (input, decimals) {
            return $filter('number')(input * 100, decimals) + '%';
        };
    }]);

    var paddingFunc = function(padFn) {
        return function(value, char, num) {
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

    module.filter('falconCurrency', ['$filter', '$locale',
        function($filter, $locale) {
            var currencyFilter = $filter('currency'),
                formats = $locale.NUMBER_FORMATS;

            return function(amount, currencySymbol, decimalPlaces) {
                if (angular.isUndefined(decimalPlaces)) {
                    decimalPlaces = 2;
                }

                var value = currencyFilter(amount, currencySymbol),
                    sep = value.indexOf(formats.DECIMAL_SEP);

                if (decimalPlaces > 0) {
                    decimalPlaces++;
                }

                return value.substring(0, sep + decimalPlaces);
            };
        }
    ]);
    module.filter('isArray', function() {
        return function(input) {
            return angular.isArray(input);
        };
    });

}(this.angular));