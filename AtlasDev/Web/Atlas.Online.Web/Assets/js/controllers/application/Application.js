/// <reference path="Application.js" />
(function (angular) {

  var module = angular.module('atlas.application', [
     'ngResource',

     'ui.date',

     'fixate.directives',

     'atlas.factories',
     'atlas.ui'
  ]);

  var _steps = [
      { id: 1, name: 'Personal Details', templateUrl: '/Application/PersonalDetails', controller: 'PersonalDetailsCtrl', path: '/PersonalDetails' },
      { id: 2, name: 'Employer Details', templateUrl: '/Application/EmployerDetails', controller: 'EmployerDetailsCtrl', path: '/EmployerDetails' },
      { id: 3, name: 'Income and Expenses', templateUrl: '/Application/IncomeExpenses', controller: 'IncomeExpensesCtrl', path: '/IncomeExpenses' },
      { id: 4, name: 'Confirm and Verify', templateUrl: '/Application/ConfirmVerify', controller: 'ConfirmVerifyCtrl', path: '/ConfirmVerify' },
      { id: 5, name: 'Verify', templateUrl: '/Application/Verify', controller: 'VerifyCtrl', path: '/Verify' },
  ];

  // API resource
  module.factory('ApplicationResource', ['$apiResource', function ($apiResource) {
    return $apiResource("/applicationstep/:method/:stepId", {}, {
      resume: { method: 'GET', params: { method: 'resume' } },
      load: { method: 'GET', params: { method: 'Get' } },
      next: { method: 'POST', params: { method: 'next', stepId: '@stepId' } },
      back: { method: 'GET', params: { method: 'back', stepId: '@stepId' } },
    });
  }]);

  // Dialog
  module.controller('ConfirmModalCtrl', ['$scope', 'dialog', function ($scope, dialog) {
    $scope.buttons = [
      { text: 'Go Back Without Saving', cssClass: 'btn-beta', result: true },
      { text: 'Stay Here', cssClass: 'btn-alpha', result: false },
    ];

    $scope.title = 'Unsaved changes';
    $scope.text = 'If you go back now, your changes on this page of your application will not be saved.<br> You will be able to resume this application later.<p><strong>Are you sure you want to continue without saving?</strong></p>';

    $scope.buttonClicked = function (e, button) {
      if (angular.isFunction(button.click) && button.click(e, button) === false) {
        return;
      }

      if (angular.isDefined(button.result)) {
        dialog.close(button.result);
      }
    };
  }]);

  // Application
  function Application($rootScope, Api) {
    var self = this;

    var validateDefault = function () {
      var form = self.getFormController();
      return !form || form.$valid;
    };

    var retNull = function () { return null; };

    this.Id = 0;

    this.controllerData = null;
    this.validateStep = validateDefault;

    // Current step form controller
    this.formController = null;

    this.stepPromise = null;

    this.steps = _steps;
    this.stepData = null;
    this.stepId = 0;
    this.applicationStepId = 0; // Current "server-side" application step

    this.backtrackedStepId = 0;
    this.backtrackedStepData = null;

    this.lastError = null;

    // Private methods
    var _resetStepState = function () {
      self.controllerData = null;
      self.validateStep = validateDefault;
      self.formController = retNull;
      self.stepPromise = null;
    };
    _resetStepState();

    var _aliasNamespaced = function (fn) {
      return function () {
        var args = Array.prototype.slice.call(arguments, 0);
        args[0] = 'application:' + args[0];
        return $rootScope[fn].apply($rootScope, args);
      };
    };

    var _stepChange_success = function(success) {
      return function (resp) {
        self.backtrackedStepId = self.stepId;
        self.backtrackedStepData = self.stepData;

        self.setStep(resp);

        self.emit('stepLoaded', self.currentStep());

        _resetStepState();

        angular.isFunction(success) && success.apply(self, [self.currentStep(), resp]);
      }
    };

    var _stepChange_error = function(error) {
      return function (e) {
        self.lastError = angular.isDefined(e.data) ? e.data : e;
        angular.isFunction(error) && error.apply(self, arguments);
      }
    };

    // Methods
    this.hasNextStep = function () { return self.steps.length > self.stepId; };

    this.hasPreviousStep = function () { return self.stepId > 1; };

    this.hasBacktrackedStep = function () { return self.backtrackedStepId > 0; };

    // Step scope helpers
    this.setupScope = function ($scope, callback) {
      self.currentScope = $scope;
      $scope.data = self.stepData;
      if (angular.isFunction(callback)) { callback($scope.data); }

      var unbind = $rootScope.$on('stepDataRefreshed', function () {
        $scope.data = self.stepData;
        if (angular.isFunction(callback)) { callback($scope.data); }
      });

      $scope.$on('$destroy', function () {
        unbind();
      });
    };

    // Event methods
    this.on = _aliasNamespaced('$on');
    this.off = _aliasNamespaced('$removeListener');
    this.emit = _aliasNamespaced('$emit');

    this.getFormController = function () {
      return angular.isFunction(self.formController) ? self.formController() : self.formController;
    };

    this.backtrackedStep = function () {
      if (!this.hasBacktrackedStep()) {
        return null;
      }

      return self.steps[this.backtrackedStepId - 1];
    };

    this.nextStep = function () {
      if (!self.hasNextStep()) {
        return null;
      }

      return self.steps[this.stepId];
    };

    this.previousStep = function () {
      if (!self.hasPreviousStep()) {
        return null;
      }

      return self.steps[this.stepId - 2];
    };

    this.currentStep = function () {
      return self.steps[self.stepId - 1];
    };

    this.getStep = function (stepId) {
      var index = stepId - 1;
      if (index > 0 && index < self.steps.length - 1) {
        return self.steps[index];
      }
    };

    this.backtrack = function () {
      if (this.backtrackedStepId > 0) {
        var tmpId = this.stepId;
        this.setStep(this.backtrackedStepData);
        this.backtrackedStepId = tmpId;
      }

      return this.currentStep();
    };

    this.load = function (stepId, success, error) {
      return Api.load({ stepId: stepId }, _stepChange_success(success), _stepChange_error(error));
    };

    this.next = function (success, error) {
      self.lastError = null;

      var controllerData = angular.isFunction(self.controllerData) ?
        self.controllerData() : (self.controllerData || {});

      var isDirty = (function() {
        var form = self.getFormController();
        if (!form) { return false; }

        return form.$dirty;
      })();

      angular.extend(controllerData, {
        // Tell the backend if anything was changed
        IsDirty: isDirty,
        Id: this.stepId
      });

      // Save current step and progress
      return Api.next(controllerData, _stepChange_success(success), _stepChange_error(error));
    };

    this.back = function (success, error) {
      self.lastError = null;
      return Api.back({stepId: self.stepId}, _stepChange_success(success), _stepChange_error(error));
    };

    this.setStep = function (step) {
      self.stepId = step.Id;
      self.stepData = step;

      if (self.stepId > self.applicationStepId) {
        self.applicationStepId = self.stepId;
      }

      self.Id = step.ApplicationId;

      return this;
    };

    this.resume = function (success, error) {
      var _success = function (resp) {
        self.setStep(resp);
        self.emit('stepLoaded', self.currentStep());
        angular.isFunction(success) && success.apply(self, [self.currentStep(), resp]);
      };

      var _error = function (e) {
        self.lastError = e.data;
        angular.isFunction(error) && error.apply(self, arguments);
      };

      return Api.resume(_success, _error);
    };
  }

  // Step resolver - Helps makes sure data is loaded before the view is displayed
  // $q is injected by factory below.
  function $StepResolver($q) {
    var self = this;

    self.deferred = {};

    self.register = function (name) {
      return self.deferred[name] = $q.defer();
    };

    self.resolve = function (name) {
      if (name in self.deferred) {
        var defer = self.deferred[name];
        defer.resolve();
        return defer;
      }

      return null;
    };

    self.reject = function (name) {
      if (name in self.deferred) {
        var defer = self.deferred[name];
        defer.reject();
        return defer;
      }

      return null;
    };

    self.get = function(name) {
      return (name in self.deferred)
        ? self.deferred[name]
        : null;
    };
  }

  module.factory('$stepResolver', ['$q', function ($q) {
    return $StepResolver._instance ||
      ($StepResolver._instance = new $StepResolver($q));
  }]);

  // Shared Application object
  module.factory('Application', ['$rootScope', 'ApplicationResource', function ($rootScope, ApplicationResource) {
    return Application._instance ||
      (Application._instance = new Application($rootScope, ApplicationResource));
  }]);

  // Simple shared object for UI
  module.factory("UI", function () {
    var UI = function () {
      this.loading = true;
      this.heading = "Apply Now";
      this.title   = '...';
      this.error = null;
      this.showButtons = true;
      this.showSteps = true;

      this.setError = function (message, retryHandler) {
        this.error = {
          message: message,
          retryHandler: retryHandler
        };

        return this;
      };

      this.clearError = function () {
        this.error = null;
      };
    };

    return UI._instance || (UI._instance = new UI());
  });

  // Route config and initialization
  module.config(['$routeProvider', function ($routeProvider) {
    var i = 0,
        len = _steps.length;

    // Setup the routes for the steps
    for (; i < len; i++) {
      var step = _steps[i];
      var r = appCtrl.stepResolver(step.controller);
      $routeProvider.when(step.path, {
        templateUrl: step.templateUrl,
        controller: step.controller,
        stepId: step.id,
        resolve: {
          loadStep: r
        }
      });
    }

    $routeProvider.otherwise({ redirectTo: '/' });
  }]);

  // Main controller
  var appCtrl = module.controller('ApplicationCtrl', [
    '$scope', '$window', '$q', '$element', '$location', '$stepResolver', '$timeout', '$rootScope', '$smoothScroll', 'UI', 'Application', 'confirmModal',
    function ($scope, $window, $q, $element, $location, $stepResolver, $timeout, $rootScope, $smoothScroll, UI, Application, confirmModal) {

      var viewDefer = $q.defer();
      var _hasClickedNavButton = false;

      $scope.submitted = false;
      $scope.booting = true;
      $scope.errorState = false;

      // Allow our views to use the Application singleton
      $scope.App = Application;
      $scope.UI = UI;

      // Register a StepResolver for all steps
      angular.forEach(Application.steps, function (step) {
        $stepResolver.register(step.controller);
      });

      $scope.$on('$viewContentLoaded', function () {
        viewDefer.resolve();
      });

      var _onError = function (e) {
        UI.loading = false;
      };

      var _stepResolved = function () {
        UI.loading = false;

        viewDefer = $q.defer();
      };

      $rootScope.$on('$routeChangeSuccess', function (event, current, previous) {
        // Back or forward has been pressed in the browser.
        // Not we need to load retrospectively
        if (current.$$route && Application.stepId != current.$$route.stepId) {
          // Load current step
          Application.load(current.$$route.stepId, function () {
            $location.path(Application.backtrack().path);
            $rootScope.$emit('stepDataRefreshed');
          });
        }

        var currentStep = Application.stepId > 0 ? Application.currentStep() : null;

        $scope.submitted = false;
        $rootScope.title = UI.title = currentStep ? currentStep.name : '';

        if (_hasClickedNavButton) {
          $smoothScroll.scrollTo($($element), { offsetY: -50 });
        }
      });

      $rootScope.$on('$routeChangeError', function (event, current, previous, rejection) {
        $scope.errorState = rejection;
        UI.loading = false;
        if (Application.hasBacktrackedStep()) {
          $location.path(Application.backtrack().path);
        }
      });

      Application.on('stepLoaded', function (e, step) {
        UI.clearError();
        UI.showSteps = true;
        Application.currentScope && (Application.currentScope.form.$submitted = false);

        // Resolve step data
        $stepResolver.resolve(step.controller);

        // Let controller run
        $timeout(function () {
          var promises = [viewDefer.promise];
          if (Application.stepPromise) {
            promises.push(Application.stepPromise);
          }

          $q.all(promises).then(_stepResolved, _onError);
        });
      });

      Application.resume(function (currentStep) {
        if (currentStep.path != $location.path()) {
          $location.path(currentStep.path);
        }

        $scope.booting = false;
      });

      $window.onbeforeunload = function () {
        var ctrl = Application.formController();
        if (ctrl !== null && ctrl.$dirty) {
          return 'If you navigate away now your changes on this page will not be saved. You will, however, be able to resume this application later.';
        }
      };

      $scope.next = function () {
        $scope.errorState = false;
        _hasClickedNavButton = true;
        Application.currentScope.form.$submitted = true;

        if (angular.isFunction(Application.validateStep) && !Application.validateStep()) {
          return false;
        }

        UI.loading = true;

        Application.next(function (currentStep) {
          $location.path(currentStep.path);
        }, function (e) {
          $scope.submitted = false;
          _onError(e);
        });
      };

      $scope.back = function () {
        _hasClickedNavButton = true;
        var _back = function () {
          UI.clearError();
          $scope.errorState = false;
          UI.loading = true;
          $scope.submitted = true;

          Application.back(function (currentStep) {
            $location.path(currentStep.path);
          }, _onError);
        };

        var ctrl = Application.formController();
        if (ctrl !== null && ctrl.$dirty) {
          confirmModal.open().then(function (cont) {
            if (cont) {
              _back();
            }
          });
        } else {
          _back();
        }
      };
    }]);

  appCtrl.stepResolver = function (controller) {
    var resolver = ['$stepResolver', function ($stepResolver) {
      var $r = $stepResolver.deferred[controller];

      // Attach promise object to array object so that we can access it
      return resolver.promise = $r.promise;
    }];

    return resolver;
  };

}(this.angular));
