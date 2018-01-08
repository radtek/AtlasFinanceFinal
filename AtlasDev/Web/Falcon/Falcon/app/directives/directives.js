(function(angular) {
    'use strict';
    var module = angular.module('falcon.directives', []);

    /* directive to disable / enable buttons */
    module.directive('disabler', ['$compile',
        function($compile) {
            return {
                link: function(scope, elm, attrs) {
                    var btnContents = $compile(elm.contents())(scope);
                    scope.$watch(attrs.ngModel, function(value) {
                        if (value) {
                            elm.html(scope.$eval(attrs.disabler));
                            elm.attr('disabled', true);
                        } else {
                            elm.html('').append(btnContents);
                            elm.attr('disabled', false);
                        }
                    });
                }
            };
        }
    ]);

    module.directive('tooltip', function () {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                $(element).hover(function () {
                    // on mouseenter
                    $(element).tooltip('show');
                }, function () {
                    // on mouseleave
                    $(element).tooltip('hide');
                });
            }
        };
    });

    module.directive('eatClick', function() {
        return function(scope, element, attrs) {
            $(element).click(function(event) {
                event.preventDefault();
            });
        };
    });

    module.directive('ngFocus', [

        function() {
            var FOCUS_CLASS = "ng-focused";
            return {
                restrict: 'A',
                require: 'ngModel',
                link: function(scope, element, attrs, ctrl) {
                    ctrl.$focused = false;
                    element.bind('focus', function(evt) {
                        element.addClass(FOCUS_CLASS);
                        scope.$apply(function() {
                            ctrl.$focused = true;
                        });
                    }).bind('blur', function(evt) {
                        element.removeClass(FOCUS_CLASS);
                        scope.$apply(function() {
                            ctrl.$focused = false;
                        });
                    });
                }
            };
        }
    ]);

    // Use:
    // apply to label with condition
    // condition must be in the form of a valid css class
    // i.e. label-danger, label-info
    module.directive('falconLabel', function() {
        return {
            restrict: 'A',
            scope: {
                'condition': '='
            },
            link: function(scope, element, attrs) {
                scope.$watch('condition', function(condition) {
                    if (condition)
                        element.addClass(condition);
                });
            }
        };
    });

    module.directive('uniqueUsername', ['$http',
        function($http) {
            return {
                require: 'ngModel',
                link: function(scope, elem, attrs, ctrl) {
                    scope.busy = false;
                    scope.$watch(attrs.ngModel, function(value) {

                        // hide old error messages
                        ctrl.$setValidity('isTaken', true);
                        ctrl.$setValidity('invalidChars', true);

                        if (!value) {
                            // don't send undefined to the server during dirty check
                            // empty username is caught by required directive
                            return;
                        }

                        scope.busy = true;

                        var url = '/api/username/?id=' + value;
                        $http.post(url).success(function(data, status, headers, config) {
                            $scope.busy = false;
                        }).error(function(data) {
                            if (data.isTaken) {
                                ctrl.$setValidity('isTaken', false);
                            } else if (data.invalidChars) {
                                ctrl.$setValidity('invalidChars', false);
                            }
                            scope.busy = false;
                        });
                    });
                }
            };
        }
    ]);

    module.directive('formGroup', function() {
        return {
            restrict: 'C',
            require: '?form',
            link: function(scope, element, attrs, formController) {
                if (!formController)
                    return;
                scope.$watch(function() {
                    return formController.$valid;
                }, function(valid) {
                    if (valid)
                        element.removeClass('has-error');
                    else
                        element.addClass('has-error');
                });
            }
        };
    });

    module.directive('blurFocus', function() {
        return {
            restrict: 'E',
            require: '?ngModel',
            link: function(scope, elm, attr, ctrl) {
                if (!ctrl) {
                    return;
                }

                elm.on('focus', function() {
                    elm.addClass('has-focus');

                    scope.$apply(function() {
                        ctrl.hasFocus = true;
                    });
                });

                elm.on('blur', function() {
                    elm.removeClass('has-focus');
                    elm.addClass('has-visited');

                    scope.$apply(function() {
                        ctrl.hasFocus = false;
                        ctrl.hasVisited = true;
                    });
                });

                elm.closest('form').on('submit', function() {
                    elm.addClass('has-visited');

                    scope.$apply(function() {
                        ctrl.hasFocus = false;
                        ctrl.hasVisited = true;
                    });
                });

            }
        };
    });

    module.directive('passwordMatch', ['$parse',
        function($parse) {

            var directive = {
                link: link,
                restrict: 'A',
                require: '?ngModel'
            };
            return directive;

            function link(scope, elem, attrs, ctrl) {
                // if ngModel is not defined, we don't need to do anything
                if (!ctrl) return;
                if (!attrs['passwordMatch']) return;

                var firstPassword = $parse(attrs['passwordMatch']);

                var validator = function(value) {
                    var temp = firstPassword(scope),
                        v = value === temp;
                    ctrl.$setValidity('match', v);
                    return value;
                }

                ctrl.$parsers.unshift(validator);
                ctrl.$formatters.push(validator);
                attrs.$observe('passwordMatch', function() {
                    validator(ctrl.$viewValue);
                });

            }
        }
    ]);

    module.directive("progressbar", function() {
        return {
            restrict: "A",
            scope: {
                total: "=",
                current: "="
            },
            link: function(scope, element) {

                scope.$watch("current", function(value) {
                    element.css("width", scope.current / scope.total * 100 + "%");
                });
                scope.$watch("total", function(value) {
                    element.css("width", scope.current / scope.total * 100 + "%");
                });
            }
        };
    });

    module.directive('charLimit', function() {
        return {
            restrict: 'A',
            link: function($scope, $element, $attributes) {
                var limit = $attributes.charLimit;

                $element.bind('keyup', function(event) {
                    var element = $element.parent().parent();

                    element.toggleClass('warning', limit - $element.val().length <= 10);
                    element.toggleClass('error', $element.val().length > limit);
                });

                $element.bind('keypress', function(event) {
                    // Once the limit has been met or exceeded, prevent all keypresses from working
                    if ($element.val().length >= limit) {
                        // Except backspace
                        if (event.keyCode != 8) {
                            event.preventDefault();
                        }
                    }
                });
            }
        };
    });

    module.directive('numbersOnly', function() {
        return {
            require: 'ngModel',
            link: function(scope, element, attrs, modelCtrl) {
                modelCtrl.$parsers.push(function(inputValue) {

                    if (inputValue === undefined) return '';
                    var transformedInput = inputValue.replace(/[^0-9]/g, '');
                    if (transformedInput != inputValue) {
                        modelCtrl.$setViewValue(transformedInput);
                        modelCtrl.$render();
                    }

                    return transformedInput;
                });
            }
        };
    });

    module.directive('popover', function() {
        return function(scope, elem) {
            elem.popover();
        };
    });

    // Used to copy server side rendered values into assignable model fields.
    module.directive('copyValue', ['$parse',
        function($parse) {
            return function(scope, element, attrs) {
                if (attrs.ngModel) {
                    $parse(attrs.ngModel).assign(scope, element.val());
                }
            };
        }
    ]);

    // Used to apply a dismiss function to a bootstrap modal, 
    // this allows progamtic closing of opened modal dialogues
    module.directive('bootModal', function() {
        return {
            restrict: 'A',
            link: function(scope, element, attr) {
                scope.dismiss = function() {
                    element.modal('hide');
                };
            }
        };
    });

    module.directive('ngEnter', function() {
        return function(scope, element, attrs) {
            element.bind("keydown keypress", function(event) {
                if (event.which === 13) {
                    scope.$apply(function() {
                        scope.$eval(attrs.ngEnter);
                    });

                    event.preventDefault();
                }
            });
        };
    });

    module.directive('selectMultiple', function() {
        return {
            link: function(scope, elem, attrs, ctrl) {
                elem.select2({
                    placeholder: attrs.placeholder,
                    selectableOptgroup: true
                });

                return elem[0].value;
            }
        };
    });

    module.directive('documentSwitchOff', [
        '$parse',
        '$timeout',
        function($parse, $timeout) {
            return function(scope, element, attrs) {
                var getter = $parse(attrs.documentSwitchOff);
                var setter = getter.assign;
                var clickInsideElement = false;

                function elementClickHandler() {
                    clickInsideElement = true;
                }

                function documentClickHandler() {
                    if (!clickInsideElement) {
                        scope.$apply(function() {
                            setter(scope, false);
                        });
                    }
                    clickInsideElement = false;
                }
                var bound = false;
                scope.$watch(attrs.documentSwitchOff, function(newVal) {
                    if (angular.isDefined(newVal)) {
                        if (newVal) {
                            $timeout(function() {
                                bound = true;
                                element.bind("click", elementClickHandler);
                                var doc = angular.element(document)
                                doc.bind('click', documentClickHandler);
                            }, 0);
                        } else {
                            if (bound) {
                                element.unbind("click", elementClickHandler);
                                angular.element(document).unbind('click', documentClickHandler);
                                bound = false;
                            }

                        }
                    }
                });

                scope.$on("$destroy", function() {
                    if (bound) {
                        element.unbind("click", elementClickHandler);
                        angular.element(document).unbind('click', documentClickHandler);
                        bound = false;
                    }
                });
            }
        }
    ]);

    module.directive('bsSelect', function() {
        return {
            link: function(scope, elem, attrs, ctrl) {
                elem.selectpicker({
                    iconBase: 'fa',
                    tickIcon: 'fa-check'
                });

                return elem[0].value;
            }
        };
    });

    module.directive('currencyInput', ['$filter',
        function($filter) {
            return {
                require: '?ngModel',
                link: function(scope, elem, attrs, ctrl) {

                    if (!ctrl) return;

                    ctrl.$parsers.unshift(function(viewValue) {
                        elem.priceFormat({
                            prefix: 'R ',
                            centsSeparator: '.',
                            thousandsSeparator: ' '
                        });

                        return elem[0].value;
                    });
                }
            };
        }
    ]);

    module.directive('ngCurrency', function ($filter, $locale) {
        return {
            require: 'ngModel',
            scope: {
                min: '=min',
                max: '=max',
                ngRequired: '=ngRequired'
            },
            link: function (scope, element, attrs, ngModel) {

                function decimalRex(dChar) {
                    return RegExp("\\d|\\" + dChar, 'g')
                }

                function clearRex(dChar) {
                    return RegExp("((\\" + dChar + ")|([0-9]{1,}\\" + dChar + "?))&?[0-9]{0,2}", 'g');
                }

                function decimalSepRex(dChar) {
                    return RegExp("\\" + dChar, "g")
                }

                function clearValue(value) {
                    value = String(value);
                    var dSeparator = $locale.NUMBER_FORMATS.DECIMAL_SEP;
                    var clear = null;

                    if (value.match(decimalSepRex(dSeparator))) {
                        clear = value.match(decimalRex(dSeparator))
                            .join("").match(clearRex(dSeparator));
                        clear = clear ? clear[0].replace(dSeparator, ".") : null;
                    }
                    else if (value.match(decimalSepRex("."))) {
                        clear = value.match(decimalRex("."))
                            .join("").match(clearRex("."));
                        clear = clear ? clear[0] : null;
                    }
                    else {
                        clear = value.match(/\d/g);
                        clear = clear ? clear.join("") : null;
                    }

                    return clear;
                }

                ngModel.$parsers.push(function (viewValue) {
                    cVal = clearValue(viewValue);
                    return parseFloat(cVal);
                });

                element.on("blur", function () {
                    element.val($filter('currency')(ngModel.$modelValue));
                });

                ngModel.$formatters.unshift(function (value) {
                    return $filter('currency')(value);
                });

                scope.$watch(function () {
                    return ngModel.$modelValue
                }, function (newValue, oldValue) {
                    runValidations(newValue)
                })

                function runValidations(cVal) {
                    if (!scope.ngRequired && isNaN(cVal)) {
                        return
                    }
                    if (scope.min) {
                        var min = parseFloat(scope.min)
                        ngModel.$setValidity('min', cVal >= min)
                    }
                    if (scope.max) {
                        var max = parseFloat(scope.max)
                        ngModel.$setValidity('max', cVal <= max)
                    }
                }
            }
        }
    });

    module.directive("buttonLoading", function() {
        return function(scope, element, attrs) {
            scope.$watch(function() {
                return scope.$eval(attrs.ngDisabled);
            }, function(newVal) {
                if (newVal) {
                    return;
                } else {
                    return scope.$watch(function() {
                            return scope.$eval(attrs.buttonLoading);
                        },
                        function(loading) {
                            if (loading)
                                return element.button("loading");
                            element.button("reset");
                        });
                }
            });
        };
    });

    /*jshint -W069 */
    module.directive('ngSize', function() {
        return {
            restrict: 'A',
            link: function(scope, elem, attrs) {
                scope.$watch(attrs['ngSize'], function(size) {
                    angular.element(elem).attr('size', size);
                });
            }
        };
    });

    module.directive('scrollGlue', function() {
        return {
            priority: 1,
            require: ['?ngModel'],
            restrict: 'A',
            link: function(scope, $el, attrs, ctrls) {
                var el = $el[0],
                    ngModel = ctrls[0];

                function scrollBottom() {
                    el.scrollTop = el.scrollHeight;
                }

                function activateScroll() {
                    return el.scrollTop + el.clientHeight + 1 >= el.scrollHeight;
                }

                scope.$watch(function() {
                    if (ngModel.$viewValue) {
                        scrollBottom();
                    }
                });

                $el.bind('scroll', function() {
                    scope.$apply(ngModel.$setViewValue.bind(ngModel, activateScroll()));
                });
            }
        };
    });

    module.directive('optionsClass', function($parse) {
        return {
            require: 'select',
            link: function(scope, elem, attrs, ngSelect) {
                // get the source for the items array that populates the select.
                var optionsSourceStr = attrs.ngOptions.split(' ').pop(),
                    // use $parse to get a function from the options-class attribute
                    // that you can use to evaluate later.
                    getOptionsClass = $parse(attrs.optionsClass);

                scope.$watch(optionsSourceStr, function(items) {
                    // when the options source changes loop through its items.
                    angular.forEach(items, function(item, index) {
                        // evaluate against the item to get a mapping object for
                        // for your classes.
                        var classes = getOptionsClass(item),
                            // also get the option you're going to need. This can be found
                            // by looking for the option with the appropriate index in the
                            // value attribute.
                            option = elem.find('option[value=' + index + ']');

                        // now loop through the key/value pairs in the mapping object
                        // and apply the classes that evaluated to be truthy.
                        angular.forEach(classes, function(add, className) {
                            if (add) {
                                angular.element(option).addClass(className);
                            }
                        });
                    });
                });
            }
        };
    });

    //module.directive('datePicker', function() {
    //    return function(scope, element, attrs) {
    //        if (element.datepicker) {
    //            element.datepicker({
    //                rtl: Metronic.isRTL(),
    //                format: attrs.datePicker,
    //                autoclose: true
    //            });
    //            element.removeClass('modal-open'); // fix bug when inline picker is used in modal
    //        }
    //    };
    //});

    module.directive('datePicker', function() {
        return {

            restrict: 'A',
            // Always use along with an ng-model
            require: '?ngModel',

            link: function(scope, element, attrs, ngModel) {
                if (!ngModel) return;

                ngModel.$render = function() { //This will update the view with your model in case your model is changed by another code.
                    element.datepicker('update', ngModel.$viewValue || '');
                };

                element.datepicker().on("changeDate", function(event) {
                    scope.$apply(function() {
                        ngModel.$setViewValue(event.date); //This will update the model property bound to your ng-model whenever the datepicker's date changes.
                    });
                });


            }
        };
    });

    /* temp until i find a place to put it */
    Date.prototype.addDays = function(days) {
        var date = new Date(this.valueOf());
        date.setDate(date.getDate() + days);
        return date;
    }

    module.directive('compDatepickerTrigger', function() {
        return function(scope, element, attrs) {
            element.datepicker({
                rtl: Metronic.isRTL(),
                orientation: "left",
                autoclose: true,
                format: 'yyyy-mm-dd',
                startDate: attrs.disablePreviousDates ? attrs.onlyFutureDates ? new Date().addDays(1)  : new Date() : null,
                daysOfWeekDisabled: attrs.disableWeekends ? [0,6] : null
            });
        };
    });

    //module.directive('compDateTimePickerWatchDaysAhead', function () {
    //    return {
    //        link: function (scope, element, attrs) {
    //            scope.$watch("dateDaysAhead", function (newValue, oldValue) {
    //                if (newValue != oldValue) {
    //                    scope.$parent.$apply(element.datetimepicker({
    //                        autoclose: true,
    //                        isRTL: Metronic.isRTL(),
    //                        format: attrs.dateForm,
    //                        pickerPosition: (Metronic.isRTL() ? "bottom-right" : "bottom-left"),
    //                        startDate: attrs.disablePreviousDates ? new Date() : null,
    //                        endDate: newValue ? new Date().addDays(parseInt(newValue)) : null,
    //                        daysOfWeekDisabled: attrs.disableWeekends ? attrs.disableSundays ? [0] : [0, 6] : null
    //                    })
    //                    );
    //                }
    //            });
    //        }
    //    }
    //});

    module.directive('compDateTimePicker', function () {
        return function (scope, element, attrs) {
            element.datetimepicker({
                autoclose: true,
                isRTL: Metronic.isRTL(),
                format: attrs.dateForm,
                pickerPosition: (Metronic.isRTL() ? "bottom-right" : "bottom-left"),
                startDate: attrs.disablePreviousDates ? new Date() : null,
                endDate: attrs.daysAhead ? new Date().addDays(parseInt(attrs.daysAhead)) : null,
                daysOfWeekDisabled: attrs.disableWeekends ? attrs.disableSundays ? [0] : [0, 6] : null
            });
        };
    });

    module.directive('tagInput', function() {
        return function(scope, element, attrs) {
            element.tagsInput({
                'width': '100%',
                'defaultText': ''
            });
        }
    });

    module.directive('anchorPreventPropagation', function() {
        return {
            restrict: 'A',
            link: function(scope, elem, attrs) {
                elem.on('click', function(e) {
                    e.preventDefault();
                });
            }
        };
    });

    module.directive('tableExpando', function() {
        return {
            restrict: 'A',
            link: function(scope, elem, attrs) {

                $("td[colspan=4]").find("p").hide();
                elem.on('click', function(event) {
                    event.stopPropagation();
                    var $target = $(event.target);
                    if ($target.closest("td").attr("colspan") > 1) {
                        $target.slideUp();
                        $target.closest("tr").prev().find("td:first").html("+");
                    } else {
                        $target.closest("tr").next().find("p").slideToggle();
                        if ($target.closest("tr").find("td:first").html() == "+")
                            $target.closest("tr").find("td:first").html("-");
                        else
                            $target.closest("tr").find("td:first").html("+");
                    }
                });
            }
        };
    });

    module.directive('modalStack', function () {
        return {
            restrict: 'A',
            link: function (scope, elem, attrs) {

                $('.modal').on('hidden.bs.modal', function (event) {
                    $(this).removeClass('fv-modal-stack');
                    $('body').data('fv_open_modals', $('body').data('fv_open_modals') - 1);
                });
                $('.modal').on('shown.bs.modal', function (event) {
                    if (typeof ($('body').data('fv_open_modals')) == 'undefined') {
                        $('body').data('fv_open_modals', 0);
                    }
                    if ($(this).hasClass('fv-modal-stack')) {
                        return;
                    }
                    $(this).addClass('fv-modal-stack');
                    $('body').data('fv_open_modals', $('body').data('fv_open_modals') + 1);
                    $(this).css('z-index', 1040 + (10 * $('body').data('fv_open_modals')));
                    $('.modal-backdrop').not('.fv-modal-stack')
                        .css('z-index', 1039 + (10 * $('body').data('fv_open_modals')));
                    $('.modal-backdrop').not('fv-modal-stack')
                        .addClass('fv-modal-stack');
                });
            }
        };
    });
})(this.angular);