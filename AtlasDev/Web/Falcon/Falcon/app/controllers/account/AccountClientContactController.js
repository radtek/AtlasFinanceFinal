;(function(angular) {
  'use strict';
  var app = angular.module('falcon');

  app.controller('AccountClientContactController', ['$scope', '$FalconDataProvider',
    function($scope, $fdp) {
      var update = function() {
        $scope.account = $fdp.data('AccountData');
      };
      // Setup listener
      $fdp.registerListenerCallback(update);
    }
  ]);
}(this.angular));