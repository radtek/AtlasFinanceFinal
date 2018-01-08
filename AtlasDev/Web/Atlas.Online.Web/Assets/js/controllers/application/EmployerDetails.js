(function (angular) {

  var module = angular.module('atlas.application');

  module.controller('EmployerDetailsCtrl', ['$scope', 'Application', function ($scope, Application) {

    Application.setupScope($scope);

    var SALARY_FREQ = $scope.SALARY_FREQ = {
      NOTSET: 0,
      MONTHLY: 1,
      BIWEEKLY: 2,
      WEEKLY: 3
    };

    Application.formController = function () {
      return $scope.form;
    };

    Application.controllerData = function () {
      var data = $scope.data;
      if (+data.SalaryFrequency == SALARY_FREQ.MONTHLY) {
        delete data.SalaryPayDay;
      } else {
        delete data.SalaryPayDayNumber;
      }
      return $scope.data;
    };

    Application.validateStep = function () {
      var data = $scope.data;
      return $scope.form.$valid && (
        (+data.SalaryFrequency == SALARY_FREQ.MONTHLY && data.SalaryPayDayNumber) ||
        (+data.SalaryFrequency != SALARY_FREQ.MONTHLY && data.SalaryPayDay)
      );
    };
  }]);
}(this.angular));