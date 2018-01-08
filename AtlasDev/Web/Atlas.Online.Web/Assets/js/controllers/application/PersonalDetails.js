(function (angular) {

  var module = angular.module('atlas.application');

  module.controller('PersonalDetailsCtrl', ['$scope', 'atlasEnums', 'Application', 'UI',
    function ($scope, atlasEnums, Application, UI) {
      Application.setupScope($scope);

      Application.formController = function () {
        return $scope.form;
      };

      Application.controllerData = function () {
        return $scope.data;
      };

      $scope.idReadonly = !!$scope.data.IdNumber;

      $scope.init = function (currentStep) {
        $scope.bankReadOnly = function () {
          return $scope.avsResult == atlasEnums.AVS_RESULT.NORESULT || (
            $scope.avsResult != atlasEnums.AVS_RESULT.NORESULT && currentStep >= 4
          );
        };
      };

      $scope.showBankingResubmit = false;
    }]);
}(this.angular));