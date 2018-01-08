(function(angular) {
  'use strict';

  var module = angular.module('atlas.myaccount', [
    'ngResource',

    'ui.date',

    'fixate.directives',
    'fixate.filters',

    'atlas.factories',
    'atlas.ui'
  ]);

  // Route config
  module.config(['$routeProvider',
    function($routeProvider) {
      $routeProvider
        .when('/', {
          name: 'CurrentLoan',
          templateUrl: '/MyAccount/CurrentLoan',
          controller: 'CurrentLoanCtrl',
          disableTemplateCache: true
        })
        .when('/CurrentLoan/Settlements/:applicationId', {
          name: 'LoanSettlements',
          templateUrl: '/MyAccount/LoanSettlements',
          controller: 'LoanSettlementsCtrl'
        })
        .when('/LoanHistory', {
          name: 'LoanHistory',
          templateUrl: '/MyAccount/LoanHistory',
          controller: 'LoanHistoryCtrl',
          disableTemplateCache: true
        })
        .when('/PersonalDetails', {
          name: 'PersonalDetails',
          templateUrl: '/MyAccount/PersonalDetails',
          controller: 'PersonalDetailsCtrl'
        })
        .when('/BankDetails', {
          name: 'BankDetails',
          templateUrl: '/MyAccount/BankDetails',
          controller: 'BankDetailsCtrl'
        })
        .when('/Login', {
          name: 'Login',
          templateUrl: '/MyAccount/LoginDetails',
          controller: 'LoginCtrl'
        })
        .otherwise({
          redirectTo: '/'
        });
    }
  ]);

  // API resource
  module.factory('AccountResource', ['$resource',
    function($resource) {
      return $resource("/myaccount/:user");
    }
  ]);

  // Simple shared object for UI
  module.factory("UI", function() {
    var UI = function() {
      var self = this;
      this.loading = true;
      this.title = '';
      this.error = null;
      this.data = {};
      this.appScope = null;
      this.message = null;

      this.getUpdateFunction = function(Resource, $scope, fields, options) {
        options = options || {};
        return function($event) {
          self.loading = true;
          $event.preventDefault();
          var details = new Resource({
            clientId: self.appScope.clientId
          });

          angular.extend(details, (function() {
            var obj = {};
            angular.forEach(fields, function(f) {
              obj[f] = $scope[f];
            });
            return obj;
          }()));

          self.clearMessage();
          self.clearError();

          details.$save(function(resp) {
            self.loading = false;
            if (options.showMessage !== false) {
              self.setMessage("Changes successfully saved.");
            }
            angular.isFunction(options.success) && options.success(resp);
          }, function(e) {
            self.loading = false;
            self.setError(e.data.Message || 'Unknown error.');
            angular.isFunction(options.error) && options.error(e);
          });
        };
      };

      this.setMessage = function(text) {
        this.message = {
          text: text
        };
        return this;
      };

      this.clearMessage = function() {
        this.message = null;
        return this;
      };

      this.setError = function(message, retryHandler) {
        this.error = {
          message: message,
          retryHandler: retryHandler
        };

        return this;
      };

      this.clearError = function() {
        this.error = null;
      };
    };

    return UI._instance || (UI._instance = new UI());
  });

  // Main MyAccount controller
  module.controller('MyAccountController', ['$scope', '$templateCache', '$rootScope', '$window', 'UI',
    function($scope, $templateCache, $rootScope, $window, UI) {
      $scope.UI = UI;

      UI.appScope = $scope;

      $scope.navClass = function(nav) {
        var route = $scope.currentRoute;
        if (!route) {
          return;
        }

        return {
          'fc-beta': route.name == nav
        };
      };

      $rootScope.$on('$routeChangeSuccess', function(event, current, previous) {
        UI.loading = false;
        UI.clearMessage().clearError();
        var route = $scope.currentRoute = current.$$route;

        if (route.disableTemplateCache) {
          $templateCache.remove(route.templateUrl);
        }
      });

      $rootScope.$on('$routeChangeError', function(event, current, previous) {
        UI.loading = false;
        UI.title = '';
        UI.setError("Unfortunately this page failed to load.", function() {
          $window.location.reload();
        });
      });

      $rootScope.$on('$routeChangeStart', function(event, current, previous) {
        UI.loading = true;
        UI.clearError();
      });
    }
  ]);

  module.factory('$Application', ['$apiResource',
    function($apiResource) {
      return $apiResource('/Application/:method/:id', {
        id: "@id"
      }, {
        current: {
          method: 'GET',
          params: {
            method: 'current'
          }
        },
        submitSettle: {
          method: 'POST',
          params: {
            method: 'SubmitSettlement',
            id: '@id'
          }
        },
        getRepayment: {
          method: 'GET',
          params: {
            method: 'Repayment',
            id: '@id'
          }
        }
      });
    }
  ]);

  // Current loan controller
  module.controller('LoanSettlementsCtrl', ['$scope', 'UI', '$routeParams', '$window', '$Application', 'Holidays', 'DateExtension',
    function($scope, UI, $routeParams, $window, $Application, Holidays, DateExtension) {
      UI.title = "Loan History - Settlements";

      UI.loading = true;

      $scope.applicationId = $routeParams.applicationId;

      $Application.getRepayment({
        id: $routeParams.applicationId
      }, function(data) {
        UI.loading = false;
        $scope.hasSettlement = data.HasSettlement;
        $scope.repaymentAmount = data.RepaymentAmount;
        $scope.repaymentDate = new Date(data.RepaymentDate);
        $scope.dateOptions = {
          yearRange: '1900:-0',
          dateFormat: 'yy-mm-dd',
          minDate: new Date(),
          maxDate: new Date(data.OrigRepaymentDate)
        };
      }, function(e) {
        UI.loading = false;
        UI.setError('Unfortunately an unknown error has occurred.', function() {
          $window.location.reload();
        });
      });

      $scope.isWorkingDay = true;
      $scope.$watch('newRepaymentDate', function(value) {
        if (value) {
          $scope.isWorkingDay = Holidays.isWorkingDay(value);
        }
      });

      $scope.isValid = function() {
        return $scope.newRepaymentDate && $scope.isWorkingDay;
      };

      $scope.submit = function($event) {
        if (!$scope.isValid()) {
          return;
        }
        $event.preventDefault();
        UI.loading = true;
        UI.clearError();
        UI.clearMessage();

        $Application.submitSettle({
          id: $routeParams.applicationId,
          newDate: DateExtension.getTimeZoneDate($scope.newRepaymentDate)
        }, function(resp) {
          $scope.newRepaymentAmount = resp.RepaymentAmount;
          $scope.hasSettlement = true;
          UI.loading = false;
          UI.setMessage("Your new settlement date was successfully set. Click back to return to Loan History.");
        }, function(e) {
          UI.loading = false;
          UI.setError('Unfortunately an unknown error has occurred.');
        });
      };
    }
  ]);

  // Current loan controller
  module.controller('CurrentLoanCtrl', ['$scope', 'UI', '$filter', 'atlasEnums',
    function($scope, UI, $filter, atlasEnums) {
      UI.title = '';

      var AppStatus = atlasEnums.APP_STATUS;
      //   options;
      $scope.selectedAppId = null;

      $scope.preventClick = function($event) {
        var el = $event.currentTarget;
        if (angular.element(el).hasClass('disabled')) {
          $event.preventDefault();
          $event.stopPropagation();
        }
      };

      $scope.init = function(status, applicationId) {
        //options = angular.fromJson(data);

        $scope.selectedRow = true;
        $scope.selectedAppId = applicationId;
        $scope.canSettle = status >= AppStatus.Open &&
          status < AppStatus.Closed;
        $scope.canContract = status >= AppStatus.PreApproved;
        $scope.canPaidUp = status == AppStatus.Closed;
        $scope.canStatement = status == AppStatus.Open ||
          status == AppStatus.Closed;
      };
    }
  ]);

  // Loan history controller
  module.controller('LoanHistoryCtrl', ['$scope', 'UI', 'atlasEnums',
    function($scope, UI, atlasEnums) {
      UI.title = "Loan History";
      var AppStatus = atlasEnums.APP_STATUS;
      $scope.selectedAppId = null;

      $scope.preventClick = function($event) {
        var el = $event.currentTarget;
        if (angular.element(el).hasClass('disabled')) {
          $event.preventDefault();
          $event.stopPropagation();
        }
      };

      $scope.selectRow = function(i, appId, status) {
        $scope.canContract = status == AppStatus.Closed;
        $scope.canPaidUp = status == AppStatus.Closed;
        $scope.canStatement = status == AppStatus.Closed;

        if ($scope.selectedRow === i) {
          $scope.selectedRow = $scope.selectedAppId = null;
        } else {
          $scope.selectedRow = i;
          $scope.selectedAppId = appId;
        }
      };
    }
  ]);

  // Personal Details controller
  module.controller('PersonalDetailsCtrl', ['$scope', 'UI', '$apiResource',
    function($scope, UI, $apiResource) {
      UI.title = "Personal Details";
      var PersDetails = $apiResource('/MyAccount/UpdatePersonalDetails/:clientId', {
        clientId: "@clientId"
      });
      var oldCell;

      $scope.update = UI.getUpdateFunction(PersDetails, $scope, ['CellNo', 'Address1', 'Address2', 'Address3', 'City', 'Province', 'PostalCode']);

      $scope.$watch('CellNo', function(value) {
        if (value) {
          $scope.cellNoChanged = (oldCell && oldCell != value);
          if (!oldCell) {
            oldCell = value;
          }
        }
      });
    }
  ]);

  // Bank Details controller
  module.controller('BankDetailsCtrl', ['$scope', 'UI', '$apiResource', 'AvsResource', 'atlasEnums', '$templateCache',
    function($scope, UI, $apiResource, AvsResource, atlasEnums, $templateCache) {
      UI.title = "Banking Details";
      var BankDetails = $apiResource('/MyAccount/UpdateBankDetails/:clientId', {
        clientId: "@clientId"
      });
      var AVS_RESULT = $scope.AVS_RESULT = atlasEnums.AVS_RESULT;

      UI.loading = true;
      AvsResource.get(function(resp) {
        var status = +resp.Status;
        $scope.allowBankChanges = (
          // Disable if AVS has started but we don't yet have a result back.
          status !== AVS_RESULT.NORESULT && status !== AVS_RESULT.LOCKED
        );

        UI.loading = false;
        $scope.avsResult = status;
      });

      $scope.bankReadOnly = function() {
        return $scope.avsResult == AVS_RESULT.NORESULT || $scope.avsResult == AVS_RESULT.LOCKED;
      };

      $scope.update = UI.getUpdateFunction(BankDetails, $scope, ['BankName', 'AccountHolder', 'AccountType', 'AccountNo', 'BankPeriod'], {
        showMessage: false,
        success: function(resp) {
          $templateCache.remove("/MyAccount/BankDetails");
          $scope.avsResult = resp.AvsStatus
        }
      });
    }
  ]);

  // Login controller
  module.controller('LoginCtrl', ['$scope', 'UI', '$apiResource', '$window', '$localStorage',
    function($scope, UI, $apiResource, $window, $localStorage) {
      UI.title = "Login Details";
      var LoginDetails = $apiResource('/MyAccount/UpdateLoginDetails/:clientId', {
        clientId: "@clientId"
      });

      // In case of token refresh this hack will still display success message
      if ($localStorage.getItem('myAccount_showSuccess')) {
        UI.setMessage("Changes successfully saved");
        $localStorage.removeItem('myAccount_showSuccess');
      }

      $scope.update = UI.getUpdateFunction(LoginDetails, $scope, ['Email', 'CurrentPassword', 'Password', 'PasswordConfirm'], {
        success: function(resp) {
          $scope.Password = $scope.CurrentPassword = $scope.PasswordConfirm = '';
          if (resp.reissueToken) {
            // Refresh to reissue javascript security token
            $localStorage.setItem('myAccount_showSuccess', true);
            $window.location.reload();
          }
        }
      });
    }
  ]);

}(this.angular));