(function (angular) {

  var module = angular.module('atlas.application');

  module.controller('IncomeExpensesCtrl', ['$scope', 'Application', 'LoanCalculator', 'LoanResource', 'Holidays', 'UI',
  function ($scope, Application, LoanCalculator, LoanApi, Holidays, UI) {

    Application.setupScope($scope, function (data) {
      if (data.Loan) {
        data.Loan.RepaymentDate = new Date(data.Loan.RepaymentDate);
      }
    });

    Application.controllerData = function () {
      return $scope.data;
    };

    Application.formController = function () {
      return $scope.form;
    };
        
    // Expenses section
    $scope.showExpenses = false;

    $scope.toggleExpenses = function ($event) {
      $event.preventDefault();
      $scope.showExpenses = !$scope.showExpenses;
      if ($scope.showExpenses) {
        for (e in $scope.expense) {
          $scope.expense[e] = 0;
        }
      }
    };

    $scope.expensesToggleButtonText = function () {
      return $scope.showExpenses ? 'Hide expenses' : 'Help with expenses?';
    };

    // Total expense fields
    $scope.$watch(function () {
      if (!$scope.showExpenses) {
        return $scope.data.TotalExpenses;
      }

      var e, total = 0.0;

      for (e in $scope.expense) {
        total += +$scope.expense[e];
      }

      // If nothing has been entered keep the input value
      if (total == 0) {
        return $scope.data.TotalExpenses;
      }

      return total;
    }, function (total) {
      $scope.data.TotalExpenses = total;
    });    
  }]);

}(this.angular));