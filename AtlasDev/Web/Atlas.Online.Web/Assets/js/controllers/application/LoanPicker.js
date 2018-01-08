(function(angular) {

  var module = angular.module('atlas.application');

  module.directive('atlLoanPicker', function() {
    return {
      restrict: 'AC',
      controller: ['$scope', 'LoanCalculator', 'LoanResource', 'Holidays',
        function($scope, LoanCalculator, LoanApi, Holidays) {
          // Loan section

          // Date picker
          $scope.dateOptions = {
            dateFormat: 'D, dd MM yy'
          };

          var calc = LoanCalculator,
            editing = false;
          $scope.isAmountEditing = false;
          $scope.workingDay = true;
          $scope.capitalOutOfBounds = false;

          var updateRepaymentDate = function() {
            var date = new Date();
            date.setDate(date.getDate() + +$scope.data.Loan.Period);
            $scope.data.Loan.RepaymentDate = date;

            return date;
          };

          var updateCalcs = function() {
            // Amount
            var amount = +$scope.data.Loan.Amount,
              amtTooSmall = (amount < $scope.minAmount),
              amtTooLarge = (amount > $scope.maxAmount);

            if (isNaN(amount)) {
              return;
            }

            if (amtTooSmall || amtTooLarge) {
              // If the user is editing the capital in the field
              // and they go out of range, just ignore it.
              // They are probably just trying to type in the whole value
              if ($scope.isAmountEditing) {
                return;
              }

              $scope.capitalOutOfBounds = true;

              $scope.data.Loan.Amount = amount = (amtTooSmall) ?
                $scope.minAmount :
                $scope.maxAmount;
            }

            // Period
            var period = +$scope.data.Loan.Period,
              prdToSmall = period < $scope.minPeriod,
              prdToLarge = period > $scope.maxPeriod;

            if (isNaN(period)) {
              return;
            }

            if (prdToSmall || prdToLarge) {
              if ($scope.isPeriodEditing) {
                return;
              }

              $scope.periodOutOfBounds = true;

              $scope.data.Loan.Period = period = (prdToSmall) ?
                $scope.minPeriod : Holidays.getNextWorkingDay($scope.maxPeriod);
            }

            calc.initialize({
              capital: amount,
              period: period
            });

            $scope.data.Loan.RepaymentAmount = calc.repaymentAmount();
          };

          $scope.$watch('data.Loan.Amount', function() {
            updateCalcs();

            $scope.periodOutOfBounds = false;
          });
          $scope.$watch('data.Loan.Period', function() {
            updateRepaymentDate();
            updateCalcs();

            $scope.capitalOutOfBounds = false;
            $scope.workingDay = $scope.periodOutOfBounds || Holidays.isWorkingDay($scope.data.Loan.RepaymentDate);
          });

          $scope.$watch('isAmountEditing', function(editing) {
            if (!editing) { // Basically onBlur
              $scope.capitalOutOfBounds = false;
              updateCalcs();
            }
          });

          $scope.$watch('isPeriodEditing', function(editing) {
            if (!editing) { // Basically onBlur
              $scope.periodOutOfBounds = false;
              updateCalcs();
            }
          });

          $scope.$watch('data.Loan.RepaymentDate', function(date) {
            var milliDiff = date - new Date();
            $scope.data.Loan.Period = Math.ceil(milliDiff / 86400000 /*1000 x 60 x 60 x 24*/ );
          });

          LoanApi.get(function(resp) {
            var Loan = $scope.data.Loan;
            $scope.lastError = null;

            $scope.maxAmount = resp.Rules.MaxLoanAmount;
            $scope.minAmount = resp.Rules.MinLoanAmount;

            $scope.maxPeriod = resp.Rules.MaxLoanPeriod;
            $scope.minPeriod = resp.Rules.MinLoanPeriod;

            updateRepaymentDate();
            updateCalcs();

            $scope.dateOptions.maxDate = '+' + $scope.maxPeriod + 'd';
            $scope.dateOptions.minDate = '+' + $scope.minPeriod + 'd';
          }, function(e) {
            $scope.lastError = e;
          });

        }
      ]
    };
  });

}(this.angular));