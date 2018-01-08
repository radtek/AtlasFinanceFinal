/// <reference path="QuoteAcceptance.js" />
(function(angular) {
  'use strict';
  var module = angular.module('atlas.quoteacceptance', [
    'fixate.directives',

    'atlas.factories',
    'atlas.ui'
  ]);

  // Simple shared object for UI 
  module.factory("UI", function() {
    var UI = function() {
      this.loading = true;
      this.error = null;
      this.showSteps = true;
      this.showButtons = true;

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

  // Dialog
  module.controller('ConfirmModalCtrl', ['$scope', 'dialog',
    function($scope, dialog) {
      $scope.buttons = [{
        text: 'Yes, reject this quotation.',
        cssClass: 'btn-beta',
        result: true
      }, {
        text: 'Cancel',
        cssClass: 'btn-alpha',
        result: false
      }, ];

      $scope.title = 'Reject quotation?';
      $scope.text = 'By rejecting this quotation you will need to restart the application from the beginning.<br>Are you sure you wish to continue?';

      $scope.buttonClicked = function(e, button) {
        if (angular.isFunction(button.click) && button.click(e, button) === false) {
          return;
        }

        if (angular.isDefined(button.result)) {
          dialog.close(button.result);
        }
      };
    }
  ]);

  module.controller('QuoteAcceptanceCtrl', ['$scope', '$apiResource', 'UI', 'confirmModal', '$http',
    function($scope, $apiResource, UI, confirmModal, $http) {
      var $QA, _applicationId;

      $scope.UI = UI;
      $scope.App = {
        stepId: 6
      };

      $scope.tooltipOptions = {
        position: 'right'
      };

      $scope.init = function(applicationId) {
        UI.loading = false;

        _applicationId = applicationId;

        /*$QA = $apiResource('/QuoteAcceptance/:method/:id', {
          id: '@id'
        }, {
          accept: {
            method: 'POST',
            params: {
              method: 'accept',
              id: applicationId
            }
          },
          reject: {
            method: 'POST',
            params: {
              method: 'reject',
              id: applicationId
            }
          }
        });*/
      };

      $scope.accept = function($event) {
        UI.loading = true;

        var el = $event.currentTarget;
        if (angular.element(el).hasClass('disabled')) {
          $event.preventDefault();
          $event.stopPropagation();
        }

        UI.clearError();

        $http.post('/api/QuoteAcceptance/accept/' + _applicationId).success(function(data, status, headers, config) {}).error(function(data, status, headers, config) {
          UI.loading = false;
          UI.setError('Unfortunately an error has occurred. Please try again later.');
        });
        /*$QA.accept(angular.noop, function(e) {
         
        });*/
      };

      $scope.reject = function($event) {
        UI.loading = true;

        var el = $event.currentTarget;
        if (angular.element(el).hasClass('disabled')) {
          $event.preventDefault();
          $event.stopPropagation();
        }

        UI.clearError();

        confirmModal.open().then(function(result) {
          if (result) {
            $QA.reject(angular.noop, function(e) {
              UI.loading = false;
              UI.setError('Unfortunately an error has occurred. Please try again later.');
            });
          } else {
            UI.loading = false;
          }
        });
      };

    }
  ]);
}(this.angular));