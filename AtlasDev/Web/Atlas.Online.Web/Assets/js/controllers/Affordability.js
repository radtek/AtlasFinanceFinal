(function (angular) {
  'use strict';

  var module = angular.module('atlas.affordability', [
    'atlas.config',

    'atlas.factories'
  ]);

  // Simple shared object for UI 
  module.factory("UI", function () {
    var UI = function () {
      this.loading = true;
      this.heading = "Apply Now";
      this.title = '...';
      this.error = null;
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

  module.controller('AffordabilityCtrl', ['$scope', '$window', 'UI', '$apiResource', function ($scope, $window, UI, $apiResource) {
    
    $scope.init = function(options) {
      var Affordibility = $apiResource('/Affordability/:method/:id', {}, {
        accept: { method: 'POST', params: { id: options.AppId, method: 'Accept' } },
        reject: { method: 'POST', params: { id: options.AppId, method: 'Reject' } }
      });

      $scope.UI = UI;

      UI.title = 'Affordability';

      $scope.App = {
        stepId: 4
      };

      UI.loading = false;

      $scope.accept = function () {
        UI.loading = true;
        UI.clearError();
        Affordibility.accept(angular.noop, function (e) {
          var data = e.data || {};
          UI.setError(data.ExceptionMessage || 'An error has occurred.', $scope.accept);
        });
      };

      $scope.reject = function () {
        UI.loading = true;
        UI.clearError();
        Affordibility.reject(angular.noop, function (e) {
          var data = e.data || {};
          UI.setError(data.ExceptionMessage || 'An error has occurred.', $scope.reject);
        });
      };
    };
  }]);
}(this.angular));