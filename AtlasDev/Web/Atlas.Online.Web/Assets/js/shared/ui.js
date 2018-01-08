(function (window, angular) {
	'use strict';
	var module = angular.module('atlas.ui', [
		'fixate.services',

    'ui.bootstrap.dialog'
	]);

  // Confirm Modal
	module.factory('confirmModal', ['$dialog', function ($dialog) {
	  var t =
	    '<div class="modal-header cf bxsh-down-dk bgi-gr-tb-alpha">' +
		    '<h6>{{ title }}</h6>' +
	    '</div>' +
	    '<div class="modal-body" ng-bind-html-unsafe="text"></div>' +
	    '<div class="modal-footer">' +
		    '<button class="btn mb-milli {{ button.cssClass }}" ng-click="buttonClicked($event,button)" ng-repeat="button in buttons">{{ button.text }}</button>' +
	    '</div>';

	  return $dialog.dialog({
	    backdrop: true,
	    keyboard: true,
	    backdropClick: true,
	    template: t,
	    controller: 'ConfirmModalCtrl'
	  });
	}]);

  // Two textboxes with a common value
	module.directive('atlTwoTextboxes', ['$keyCode', function ($keyCode) {
	  return {
	    replace: true,
	    require: '?ngModel',
	    template:
        '<div class="row row-ib-cols cf">' +
	        '<div class="col col-3 pr-micro">' +
		        '<input type="text" class="d-block">' +
          '</div>' +
	        '<div class="col col-9">' +
		        '<input type="text" class="d-block">' +
	        '</div>' +
        '</div>',
	    link: function (scope, element, attrs, ctrl) {	      
	      var inputs = element.find('input'),
	          input1 = inputs.eq(0),
	          input2 = inputs.eq(1);

	      var isValid = function () {
	        return input1.val() !== "" && input2.val() !== "";
	      };

	      var validate = function (viewValue) {
          if (!angular.isDefined(attrs.required)) { return viewValue; }
	        if (isValid()) {
	          ctrl.$setValidity('required', true);
	          return viewValue;
	        } else {
	          ctrl.$setValidity('required', false);
	          return undefined;
	        }
	      };

	      if (attrs.prefixLength) {
	        input1.on('keydown', function (e) {
	          var charCode = e.which || e.keyCode;
	          if (!$keyCode.isEditer(e) && this.value.length >= +attrs.prefixLength) {
	            var selected = this.selectionStart !== this.selectionEnd;
	            if (!selected) {
	              input2.focus();
	            }
	          }
	        });

	        input2.on('keydown', function (e) {
	          if (!this.value && $keyCode.isDeleter(e)) {
	            input1.focus();
	          }
	        }); 
	      }

	      scope.$watch(function () {
	        return (ctrl.$dirty && 'a') + (ctrl.$invalid && 'b');
	      }, function (value) {
	        if (ctrl.$invalid) {
	          input1.add(input2).addClass('ng-invalid');
	        } else {
	          input1.add(input2).removeClass('ng-invalid');
	        }

	        if (ctrl.$dirty) {
	          input1.add(input2).addClass('ng-dirty').removeClass('ng-pristine');
	        } else {
	          input1.add(input2).removeClass('ng-dirty').addClass('ng-pristine');
	        }
	      });

	      // Support $blur
	      if (attrs.name) {
	        input1.add(input2).on('blur', function () {
	          var field = scope.form[attrs.name];
	          if (field) {
	            field.$blur = true;
	          }
	        });
	      }

	      input2.on('blur', function (e) {
	        element.trigger('blur', e);
	      });

	      input1.on('change', function () {
	        var val = this.value,
	          prefixLen = +attrs.prefixLength;

	        if (!val || val.length <= prefixLen || !angular.isDefined(attrs.prefixLength)) { return; }

	        input1.val(val.substr(0, prefixLen))
	        input2.val(val.substr(prefixLen - 1, val.length - 1));
	      });

	      scope.$watch(function () {
	        return input1.val() + (attrs.delimiter || '') + input2.val();
	      }, function (value) {
	        validate(value);
	        !ctrl.$error.required && patternValidator && patternValidator(value);
	        if (ctrl.$modelValue !== value && ctrl.$valid) {
	          ctrl.$setViewValue(value);
	        }
	      });

	      scope.$on('$destroy', function () {
          // Clean up handlers
	        input1.off('keydown blur change');
	        input2.off('keydown blur');
	      });

	      // pattern validator
	      var pattern = attrs.ngPattern,
            patternValidator;

	      var patternValidate = function (regexp, value) {
	        if (value === "" || regexp.test(value)) {
	          ctrl.$setValidity('pattern', true);
	          return value;
	        } else {
	          ctrl.$setValidity('pattern', false);
	          return undefined;
	        }
	      };

	      if (pattern) {
	        if (pattern.match(/^\/(.*)\/$/)) {
	          pattern = new RegExp(pattern.substr(1, pattern.length - 2));
	          patternValidator = function (value) {
	            return patternValidate(pattern, value)
	          };
	        } else {
	          patternValidator = function (value) {
	            var patternObj = scope.$eval(pattern);

	            if (!patternObj || !patternObj.test) {
	              throw new Error('Expected ' + pattern + ' to be a RegExp but was ' + patternObj);
	            }
	            return patternValidate(patternObj, value);
	          };
	        }

	        ctrl.$formatters.push(patternValidator);
	        ctrl.$parsers.push(patternValidator);
	      }

	      ctrl.$render = function () {
	        var value = ctrl.$modelValue;
	        if (!value) {
	          if (value = attrs.value) {
	            ctrl.$setViewValue(value);
	          } else {
	            return;
	          }
	        }

	        var inputs = element.find('input'), values;

	        if (attrs.delimiter) {
	          values = value.split(attrs.delimiter, 2);
	        } else if (attrs.prefixLength) {
	          values = [value.substr(0, +attrs.prefixLength), value.substr(+attrs.prefixLength)];
	        }

	        if (values.length < 2) { return; }

	        input1.val(values[0]);
	        input2.val(values[1]);

	        ctrl.$pristine = true;
	        ctrl.$dirty = false;
	      };
	    }
	  };
	}]);

	// Slider
	module.directive('atlSliderBar', function() {
		var calcPercentage = function(value, min, max) {
			return (value - min) / (max - min) * 100;
		};

		var calculateValue = function(perc, min, max) {
			return (perc / 100) * (max - min) + min;
		};

		return {
			restrict: 'EA',
			scope: {
				min: '@',
				max: '@',
				model: '=ngModel',
				onMouseUp: '&',
				onMouseDown: '&',
        onChange: '&change'
			},
			template: '<a href="javascript:void(0);" class="slider-container">'+
					'<i class="slider-progress" ng-style="{width: percentage+\'%\'}"></i>'+
					'<i class="slider-handle" ng-style="{left: percentage+\'%\'}"></i>'+
				'</a>',

			link: function(scope, element, attrs) {
				var $bar          = element.first(),
						barOffset     = null,
						barWidth      = null,
						$body         = angular.element(document.body),
						$window       = angular.element(window),
						resizeTimeout = null;

				scope.$watch('model', function (model) {
					var perc = scope.percentage = checkPercentage(
						calcPercentage(scope.model, scope.min, scope.max)
					);
				});

				// Functions 
				var setModel = function (value) {
					scope.model && (scope.model = value); 
					scope.$apply();
				};

				var bindMouse = function() {
					$body.on({
						mousemove: mouseMove,
						mouseup: mouseUp,
            mousedown: mouseDown,
						mouseenter : mouseEnter
					});
				};

				var unbindMouse = function() {
					$body.off({
						mousemove: mouseMove,
						mouseup: mouseUp,
						mousedown: mouseDown,
						mouseenter: mouseEnter
					});
				};

				var checkPercentage = function(perc) {
					if (perc > 100) {
						perc = 100;
					} else if (perc < 0) {
						perc = 0;
					}

					return perc;
				};

			  // Events
				var resize = function (e) {
				  barOffset = $bar.offset();
					barWidth = $bar.width();
				};

				var mouseEnter = function(e) {
					if (e.button === 0) {
						// Update model and unbind 
						// Handles case when entering from off the screen
						mouseUp.apply(this, e);
						return;
					}
				};

				var mouseMove = function(e) {
					var perc = calcPercentage(e.clientX, barOffset.left, barOffset.left + barWidth);

					scope.percentage = perc = checkPercentage(perc);
					setModel(calculateValue(perc, +scope.min, +scope.max));

					angular.isFunction(scope.onChange) && scope.onChange({ percentage: perc });
				};

				var mouseUp = function(e) {
				  unbindMouse();
				  scope.onMouseUp();
				};
        
				var mouseDown = function (e) {
				  scope.onMouseDown();
				};

				$bar.on({
					mousedown: function(e) {
						e.preventDefault();
						// Update when clicking on bar
						barWidth = $(this).width();
						mouseMove.call(this, e);
						bindMouse();
					}
				});

				$window.on('resize', function throttledResize() {
				  if (resizeTimeout) {
				    clearTimeout(resizeTimeout);
				  }

				  resizeTimeout = setTimeout(function () {
				    resizeTimeout = null;
				    resize();
				  }, 50);
				});
				resize();

				// Clean up events
				scope.$on('$destroy', function() {
					unbindMouse();
					$bar.off('mousedown mouseup');
          $window.off('resize', throttledResize);
					$body = $bar = null;
				});

			}
		};
	});

}(this, this.angular));
