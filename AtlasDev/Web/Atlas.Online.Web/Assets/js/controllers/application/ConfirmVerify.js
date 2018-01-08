(function (angular) {
  var module = angular.module('atlas.application');  

  module.controller('ConfirmVerifyCtrl',
    ['$scope', '$templateCache', 'Application', 'atlasEnums', 'LoanCalculator',
    function ($scope, $templateCache, Application, atlasEnums, LoanCalculator) {
      Application.setupScope($scope);      

      LoanCalculator.initialize({ capital: $scope.data.Loan.Amount, period: $scope.data.Loan.Period });
      $scope.repaymentAmount = LoanCalculator.repaymentAmount();

      $scope.showBankingResubmit = true;

      var AVS_RESULT = atlasEnums.AVS_RESULT;

      Application.validateStep = function () {
        return ($scope.avsResult != AVS_RESULT.FAILED);
      };

      $scope.bankReadOnly = function () {
        return $scope.avsResult == AVS_RESULT.NORESULT ||
          $scope.avsResult == AVS_RESULT.PASSED ||
          $scope.avsResult == AVS_RESULT.PASSEDWARNINGS;
      };

      // Invalidate template cache as server rendered values may change
      // TODO: It would be more efficient if we invalidated this only when certain values
      //       are changed or render values from frontend data.
      $templateCache.remove(Application.currentStep().templateUrl);
  }]);
}(this.angular));