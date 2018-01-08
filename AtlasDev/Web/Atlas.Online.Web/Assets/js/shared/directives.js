(function (angular) {
  'use strict';

  var PRISTINE_CLASS = 'ng-pristine',
      DIRTY_CLASS = 'ng-dirty';

  var module = angular.module('fixate.directives', [
    'ngResource',
    'fixate.filters',
    'fixate.services'
  ]);

  // Tooltip
  module.directive('f8Tooltip', function () {
    return {
      restrict: 'EAC',
      scope: {
        title: '@'
      },
      link: function (scope, element, attrs) {
        var options = scope.$parent.$eval(attrs.f8Tooltip),
            tooltip = element.tooltipster(options);

        scope.$watch('title', function (value) {
          tooltip.tooltipster('update', value);
        });
      }
    };
  });


  // Form extensions
  module.directive('form', function () {
    return {
      restrict: 'E',
      require: 'form',
      link: function (scope, element, attrs, form) {
        form.$submitted = false;
        var _submit = function () {
          form.$submitted = true;
        };

        element.on('submit', _submit);

        scope.$on('$destroy', function () {
          element.off('submit', _submit);
        });
      }
    };
  });

  // Used to validate a field using a remote resource call
  module.directive('validateRemotely', ['$resource', '$timeout', function ($resource, $timeout) {    
    var validationTypes = ['pending', 'unchecked', 'failed'];    
    var setValidity = function (ngModel, remoteValidity, other) {
      other = other || {};

      if (angular.isDefined(other.all) && other.all) {
        other = {
          pending: other.all,
          unchecked: other.all,
          failed: other.all
        };
      }
              
      angular.forEach(validationTypes, function (v) {
        var key = 'remotevalidity' + v;
        if (remoteValidity != null) {
          ngModel.$setValidity('remotevalidity', remoteValidity);
        }

        if (angular.isDefined(other[v])) {
          ngModel.$setValidity(key, other[v]);
        }
      });
    };

    return {
      restrict: 'AC',
      require: ['validateRemotely', 'ngModel'],
      controller: ['$scope', function ($scope) {
        this.revalidate = function () {
          return this.validate();
        };

        this.validatedValue = null;
      }],
      compile: function () {
        return {
          pre: function (scope, element, attrs, controllers) {

            if (!angular.isDefined(attrs.endpoint)) {
              throw "Enpoint not supplied.";
            }

            var validateRemotely = controllers[0];
            var ngModel = controllers[1];

            var resource = $resource(unescape(attrs.endpoint));
           
            // Returns true if the model is valid apart from the remotevalidity
            var modelValid = function () {
              var errors = angular.copy(ngModel.$error), e;

              delete errors.remotevalidity;
              angular.forEach(validationTypes, function (v) {
                delete errors['remotevalidity' + v];
              });

              for (e in errors) {
                if (errors[e]) { return false; }
              }

              return true;
            };

            var validate = validateRemotely.validate = function () {
              var val = ngModel.$modelValue || element.val();

              if (attrs.readonly) {
                return;
              }

              if (!val) {
                setValidity(ngModel, true, { all: true });
                validateRemotely.validatedValue = val;
                return;
              };

              if (!modelValid()) {
                setValidity(ngModel, true, { all: true });
                return;
              }

              var data = { value: val };
              if (attrs.params) {
                var params = scope.$eval(attrs.params);
                if (params === false) {
                  setValidity(ngModel, true, { pending: true, unchecked: false });
                  return;
                }
                angular.extend(data, params);
              }

              // Check that the value has changed
              if (validateRemotely.validatedValue && angular.equals(validateRemotely.validatedValue, data)) {
                setValidity(ngModel, null, { unchecked: true });
                return;
              }

              scope.working = true;
              setValidity(ngModel, true, { pending: false, unchecked: true, failed: true });
            
              resource.get(data, function (resp) {
                validateRemotely.validatedValue = data;
                scope.working = false;
                setValidity(ngModel, resp.Valid, { pending: true, unchecked: true });
              }, function (e) {
                setValidity(ngModel, null, { pending: true, unchecked: false, failed: false });
                scope.working = false;
                scope.error = {
                  message: e.message
                };
              });
            };

            var alias = attrs.name || attrs.validateRemotely;
            if (alias) {
              scope['_'+alias] = angular.extend(scope['_'+alias] || {}, {
                validateRemotely: validateRemotely
              });
            }

            scope.$on('$destroy', function () {
              element.off('blur keyup');
              if (alias) {
                scope[alias] = undefined;
              }
            });
          },
          post: function (scope, element, attrs, controllers) {
            var validateRemotely = controllers[0];

            var ngModel = controllers[1];

            element.on('blur', function () {
              $timeout(function () {
                validateRemotely.validate();
              });
            });
            element.on('keyup', function () {
              if (validateRemotely.validatedValue != this.value) {
                setValidity(ngModel, true, { pending: true, unchecked: false });
              }
            });

            // Run this on startup once, to check once controllers fill value in initially
            if (!attrs.validateonstartup || attrs.validateonstartup.toLowerCase() !== 'false') {
              var $clearModelListener = scope.$watch(function () { return ngModel.$modelValue }, function (value) {
                if (typeof value !== 'undefined') {
                  $clearModelListener();
                  validateRemotely.validate();
                }
              });
            }
          }
        }
      }
    };
  }]);

  module.directive('f8LimitLength', ['$keyCode', function ($keyCode) {
    return {
      restrict: 'A',
      require: 'ngModel',
      link: function (scope, element, attrs, ctrl) {
        var limit = +attrs.f8LimitLength;

        var keypress = function (event) {
          if (element.val().length >= limit) {
            // Except editer keys 
            if (!$keyCode.isEditer(event)) {
              event.preventDefault();
            }
          }
        };

        element.bind('keypress', keypress);

        scope.$on('$destroy', function () {
          element.unbind('keypress', keypress);
        });
      }
    };
  }]);

  // Some additional event models
  module.directive(['focus', 'blur', 'keyup', 'keydown', 'keypress'].reduce(function (container, name) {
    var directiveName = 'ng' + name[0].toUpperCase() + name.substr(1);

    container[directiveName] = ['$parse', function ($parse) {
      return function (scope, element, attr) {
        var fn = $parse(attr[directiveName]);
        element.bind(name, function (event) {
          scope.$apply(function () {
            fn(scope, {
              $event: event
            });
          });
        });
      };
    }];

    return container;
  }, {}));

  // Format models
  module.directive('ngModelFormatter', ['$filter', function ($filter) {
    return {
      require: 'ngModel',
      restrict: 'A',
      link: function (scope, element, attrs, controller) {
        var formatter = attrs.ngModelFormatter,
            args,
            parts = formatter.split(':');

        formatter = parts.shift();
        args = parts;

        var filter = $filter(formatter);

        if (!filter) { return; }

        controller.$formatters.unshift(function (value) {
          var argsCopy = angular.copy(args);
          argsCopy.unshift(value);
          return filter.apply(this, argsCopy);
        });
      }
    };
  }]);

  // Validate equal to (confirm password etc.)
  module.directive('f8Compare', [function () {
    return {
      require: 'ngModel',      
      link: function (scope, element, attrs, controller) {
        var validate = function (viewValue) {
          var origin = scope[attrs.f8Compare];
          if (origin !== viewValue) {
            controller.$setValidity("equalto", false);
            return undefined;
          } else {
            controller.$setValidity("equalto", true);
            return viewValue;
          }
        };

        // Watch compare target
        scope.$watch(function () { return scope[attrs.f8Compare]; }, function () {
          validate(controller.$viewValue || "");
        });

        controller.$parsers.unshift(validate);
      }
    };
  }]);

  // Hacky fix to get javascript to recognise autocompleted fields
  // https://github.com/angular/angular.js/issues/1460#issuecomment-19546862
  module.directive('input', ['$timeout', function ($timeout) {
    return {
      restrict: 'E',
      require: '?ngModel',
      link: function (scope, element, attrs, controller) {
        var intervalHnd,
          INTERVAL = 500,
          type = element.attr('type'),
          oldVal;

        if (!controller || attrs.disableAutocompletePing) { return; }
        if (type == 'checkbox' || type == 'radio') { return; }

        // Maybe autocomplete is disabled on the parent form, so this hack isn't even necessary
        if (element.closest('form').attr('autocomplete') == 'off') { return; }
        
        scope.$watch('init', function () {
          oldVal = controller.$modelValue || '';

          // Start timer
          intervalHnd = $timeout(function timer() {
            var val = element.val();
            if (controller.$pristine && val !== oldVal) {
              oldVal = val;
              controller.$setViewValue(val);
            }

            intervalHnd = $timeout(timer, INTERVAL);
          }, INTERVAL);
        });

        // Kill timer
        scope.$on('$destroy', function () {
          $timeout.cancel(intervalHnd);
        });
      }
    };
  }
  ]);

  var blurify = function () {
    return {
      restrict: 'E',
      link: function (scope, element, attrs) {
        if (!attrs.name) { return; }

        element.on('blur', function () {
          if (!scope.form) { return; }

          var field = scope.form[attrs.name];
          if (field) {
            field.$blur = true;
          }
        });

        scope.$on('$destroy', function () {
          element.off('blur');
        });
      }
    };
  }

  // Add $blur flag to form inputs
  module.directive('input', blurify);
  module.directive('select', blurify);
  module.directive('blurify', blurify);

  // Initialize model from form
  module.directive('f8InitializeModel', [function () {
    return {
      restrict: 'A',
      compile: function (element, attrs, transclude) {
        var formData = element.serializeArray();
        return function (scope) {
          // Now that the scope is setup, lets populate it from the form
          angular.forEach(formData, function (data) {
            if (angular.isDefined(element.find('[name="' + data.name + '"]').attr('ng-model'))) {
              scope[data.name] = data.value;
            }
          });
        }
      }
    };
  }]);

  // Dropkeeck-pants dropdowns
  module.factory('$dropkick', function () { return $.fn.dropkick; });
  module.directive('select', ['$dropkick', function ($dropkick) {
    if (!$dropkick) { return null; }

    return {
      restrict: 'E',
      terminal: true,
      require: '?ngModel',
      link: function (scope, element, attrs, controller) {
        if (attrs.multiline || !controller) { return; }

        if (element.val()) {
          controller.$setViewValue(element.val());
        }

        var dk = element.dropkick({
          change: function (value) {
            if (controller.$modelValue+"" !== value)
            {
              controller.$setViewValue(value);
              if (attrs.onChange) {
                scope.$eval(attrs.onChange);
              }
            }
            // Prevent unnecessary $digest phase
            if (scope.$$phase != '$digest') { scope.$apply(); }

            if (attrs.onSelect) {
              scope.$eval(attrs.onSelect);
            }
          },
          blur: function () {
            if (!attrs.name || !scope.form) { return; }
            var field = scope.form[attrs.name];
            if (field) {
              field.$blur = true;
            }
          }
        }).data('dropkick');

        // Support show
        if (attrs.show) {
          scope.$watch(attrs.show, function (value) {            
            // Have to explicitly say display: block for dropkick
            dk.$dk.css('display', !!value ? 'block' : 'none');
          });
        }

        // Support ngReadonly
        if (attrs.ngReadonly) {
          scope.$watch(attrs.ngReadonly, function (value) {
            element.dropkick('disabled', !!value);
          });
        }

        controller.$render = function () {
          element.dropkick('value', controller.$modelValue);
        };

        if (attrs.required) {
          controller.$setValidity('required', !!+element.val());
          var requiredValidator = function(value) {
            if (!angular.isDefined(value) || !value) {
              controller.$setValidity('required', false);
              return;
            } else {
              controller.$setValidity('required', true);
              return +value;
            }
          };
          controller.$formatters.push(requiredValidator);
          controller.$parsers.unshift(requiredValidator);
        }

        // Make select fluid
        if (element.hasClass('d-block')) {
          element.siblings('.dk_container').css('float', 'none');
          element.siblings('.dk_toggle').css('width', 'auto').addClass('d-block');
        }
      }
    };
  }]);

  module.directive('rangedSelect', function () {
    return {
      restrict: 'AC',
      priority: 100,
      scope: {
        min: '@',
        max: '@',
        step: '@'
      },
      compile: function (element, attrs) {
        var step = +attrs.step || 1;
        for (var i = +attrs.min || 0; i < (+attrs.max || 100) ; i += step) {
          element.append('<option value=' + i + '>' + i + '</option>');
        }
      }
    };
  });

  // Currency specific formatter
  module.directive('f8CurrencyField', ['$filter', '$locale', '$keyCode', function ($filter, $locale, $keyCode) {
    var formats = $locale.NUMBER_FORMATS;

    return {
      require: 'ngModel',
      restrict: 'A',
      link: function (scope, element, attrs, controller) {
        var options = scope.$eval(attrs.f8CurrencyField),
            filter = $filter('f8currency'),
            rendered = false,
            oldVal = '',
            parentForm = element.inheritedData('$formController');

        options = angular.extend({
          symbol: '$',
          decimals: 2,
        }, options);

        var removeFormatting = function (value) {
          if (!value || !value.substr || value.substr(0, options.symbol.length) != options.symbol) {
            return value;
          }

          value = value.substr(options.symbol.length);
          value = value.replace(new RegExp(formats.GROUP_SEP, 'g'), '');
          return value;
        };

        var addFormatting = function (value) {
          return filter(value, options.symbol, options.decimals);
        };

        var bindOnKey = function () {
          element.on('keydown', function (e) {
            // Hard enforce numbers only since this directive is for currency    
            if (!$keyCode.isNumberEdit(e)) {
              return false;
            }
            controller.$setViewValue(this.value);
          });
        };

        var unbindOnKey = function () {
          element.off('keydown');
        };

        var $setDirty = function (dirty) {
          controller.$dirty = dirty;
          controller.$pristine = !dirty;
          if (dirty) {
            element.addClass(DIRTY_CLASS).removeClass(PRISTINE_CLASS);
          } else {
            element.removeClass(DIRTY_CLASS).addClass(PRISTINE_CLASS);
          }

          parentForm.$setDirty();
        };

        controller.$parsers.push(function ($viewValue) {

          var val = removeFormatting($viewValue);
          if (angular.isDefined(attrs.required)) {
            controller.$setValidity('required', +val);
          }
          return val || 0;
        });

        controller.$render = function () {
          oldVal = controller.$modelValue || 0;
          element.val(addFormatting(oldVal));
          controller.$setViewValue(oldVal);
          rendered = true;          
        };

        element.on('focus', function () {
          if (attrs.readonly) { return; }
          // Remove the currency formatting on the control only
          this.value = removeFormatting(this.value);
          if (!+this.value) { this.value = ''; }
          rendered = false;
          bindOnKey();
        });

        element.on('blur', function () {
          if (!rendered) {
            // Add formatting but only if $render hasn't already added it.
            var val = addFormatting(this.value);
            this.value = val || (function () {
              var dirty = controller.$dirty;

              // If not valid, reset to old value
              controller.$setViewValue(oldVal);
              // Set dirty back to what is was before the change
              $setDirty(dirty);

              scope.$apply();
              return addFormatting(oldVal);
            }());
          }
          unbindOnKey();
        });
      }
    };
  }]);

  module.directive('alert', ['$timeout', function ($timeout) {
    return {
      restrict: 'C',
      link: function (scope, element, attrs) {
        var timeoutPromise = null,
          CLASSES = 'anim-margin-top-hide anim-slide-top-hide';

        var close = function () {
          timeoutPromise && $timeout.cancel(timeoutPromise);

          element.addClass(CLASSES);
        };

        element.parent().removeClass(CLASSES);
        element.find('.close').on('click', close);

        if (attrs.timeout) {
          timeoutPromise = $timeout(close, +attrs.timeout);
        }

        scope.$on('$destroy', function () {
          element.find('.close').off('click');
          timeoutPromise && $timeout.cancel(timeoutPromise);
        });
      }
    };
  }]);

  // Incomplete
  //module.directive('f8Modal', ['$dialog', function ($dialog) {
  //  var t = '<div class="modal fade" role="dialog" ng-show="show" ng-class="{in: show, hide: !show}">' +
  //    '<div class="modal-header cf bxsh-down-dk bgi-gr-tb-alpha">' +
  //	    '<h6>{{title}}</h6>' +
  //    '</div>' +
  //    '<div class="modal-body">{{ text }}</div>' +
  //    '<div class="modal-footer">' +
  //	    '<button class="btn mb-milli {{ button.cssClass }}" ng-click="buttonClicked($event,button)" ng-repeat="button in buttons">{{ button.text }}</button>' +
  //    '</div>'
  //  '</div>';

  //  return {
  //    restrict: 'EAC',
  //    replace: true,
  //    template: t,
  //    scope: {
  //      f8Modal: '@',
  //      buttons: '&',
  //      title: '@',
  //      text: '@',
  //      show: '='
  //    },
  //    require: 'f8Modal',
  //    controller: ['$scope', function ($scope) {
  //      this.dialog = null;

  //      this.open = function () {
  //        return this.dialog.open();
  //      };

  //      this.close = function (result) {
  //        this.dialog.close(result);
  //      };
  //    }],
  //    link: function (scope, element, attrs, ctrl) {
  //      if (attrs.f8Modal) {
  //        scope.options = scope.$eval(attrs.f8Modal);
  //      };

  //      scope.options = angular.extend(scope.options || {}, {
  //        backdrop: true,
  //        keyboard: true,
  //        backdropClick: true,          
  //      });

  //      var dialog = ctrl.dialog = $dialog.dialog(scope.options);

  //      scope.$watch('show', function (value) {
  //        if (value === true) {
  //          dialog.open();            
  //        } else if (value === false) {
  //          dialog.close();
  //        }
  //      });

  //      scope.buttonClicked = function (e, button) {
  //        if (angular.isFunction(button.click) && button.click.call(ctrl, e) === false) {
  //          return;
  //        }

  //        if (button.close) {
  //          ctrl.close(button.result);
  //        }
  //      };
  //    }
  //  };
  //}]);

})(this.angular);