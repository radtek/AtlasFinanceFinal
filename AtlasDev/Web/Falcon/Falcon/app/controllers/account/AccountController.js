;(function(angular) {
  'use strict';
  var app = angular.module('falcon');

  app.controller('AccountController', ['$scope', 'AccountResource', 'xSocketsProxy', '$FalconDataProvider',
    function($scope, AccountResource, xSocketsProxy, $fdp) {

      var _detail = function() {
        AccountResource.get($scope.accountId, $scope.token).then(function(result) {
          // Share data across bounds with service
          $scope.account = result;
          $fdp.setData('AccountData', result);

          $scope.isLoaded = true;
        });
      };

      $scope.init = function(id, personId) {
        $scope.isLoaded = false;
        $scope.personId = personId;
        $scope.accountId = id;
        _detail();
        //_setupSocket(id);
      };

      $fdp.registerUpdateCallback(_detail);
    }
  ]);
}(this.angular));