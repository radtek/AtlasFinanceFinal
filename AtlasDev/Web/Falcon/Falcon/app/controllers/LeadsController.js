(function (angular) {

  var app = angular.module('falcon');

  app.controller('LeadsController', ['$scope',
    function ($scope) {
      $scope.leadsCount =1;
    }
  ]);
}(this.angular));