(function (angular) {
  'use strict';

  var module = angular.module('atlas.slider', [
    'ngCookies',

    'ui.date',

    'fixate.directives',

    'atlas.factories',
    'atlas.ui',
  ]);

  // Slider controller
  module.controller('SliderController', [
    '$scope', '$window', '$cookies', 'Holidays', 'LoanResource', 'LoanCalculator',
      function ($scope, $window, $cookies, Holidays, LoanResource, LoanCalculator) {
    var calc = LoanCalculator;
    $scope.workingDay = true;
    $scope.canApply = false;

    $scope.capitalOutOfBounds = false;

    var updateRepaymentDate = function () {
      var date = new Date();
      date.setDate(date.getDate() + +$scope.period);
      $scope.repaymentDate = date;

      return date;
    };

    var handleUnauthorized = function (e) {
      // EDGE CASE: Use may have pressed the back button 
      // and so did not get issued a new XSRF token.
      // Lets reload the page to get a new one.
      if (e.status == 403) {
        $window.location.reload();
        return;
      }
    };

    var updateCalcs = function () {
      var capital = +$scope.capital,
          capTooSmall = (capital < $scope.minCapital),
          capTooLarge = (capital > $scope.maxCapital);

      if (isNaN(capital)) { return; }

      if (capTooSmall || capTooLarge) {
        // If the user is editing the capital in the field
        // and they go out of range, just ignore it.
        // They are probably just trying to type in the whole value
        if ($scope.isCapitalEditing) { return; }

        $scope.capitalOutOfBounds = true;

        $scope.capital = capital = (capTooSmall) ?
          $scope.minCapital :
          $scope.maxCapital;
      }

      calc.initialize({
        capital: capital,
        period: $scope.period
      });

      $scope.totalFee = calc.totalFee();
      $scope.repayment = calc.repaymentAmount();
    };

    $scope.dateOptions = {
      yearRange: '1900:-0',
      dateFormat: 'D, d M y'
    };

    $scope.tooltipOptions = {
      position: 'left',
      maxWidth: 300
    };

    $scope.loading = true;

    $scope.validateWorkingDay = function () {
      $scope.workingDay = Holidays.isWorkingDay($scope.repaymentDate);
      if ($scope.$$phase != '$apply') {
        $scope.$apply();
      }
    };

    $scope.setIsWorkingDay = function (value) {
      $scope.isWorkingDay = value;
      if ($scope.$$phase != '$apply') {
        $scope.$apply();
      }
    };

    $scope.$watch('capital', updateCalcs);
    $scope.$watch('period', function () {
      updateRepaymentDate();
      updateCalcs();
    });

    $scope.$watch('isCapitalEditing', function (editing) {
      if (!editing) { // Basically onBlur
        $scope.capitalOutOfBounds = false;
        updateCalcs();
      }
    });

    $scope.amountSliderChange = function () {
      $scope.capitalOutOfBounds = false;
    };

    $scope.submitSlider = function () {
      if (!$scope.workingDay || $scope.loading) { return; }

      $scope.loading = true;
      $scope.error = null;

      var result = new LoanResource();
      result.Amount = $scope.capital;
      result.Period = $scope.period;
      result.RepaymentAmount = $scope.repayment;
      result.RepaymentDate = $scope.repaymentDate;
      
      result.$save(angular.noop, function (e) {
        handleUnauthorized(e);
        $scope.loading = false;
        $scope.error = {
          message: 'An unknown error has occurred. Please try again in a few minutes. We apologise for any inconvenience caused.',
          retryHandler: $scope.submitSlider
        };
      });

      return false;
    };

    $scope.$watch('repaymentDate', function (date) {      
      var milliDiff = date - new Date();
      $scope.period = Math.ceil(milliDiff / 86400000 /*1000 x 60 x 60 x 24*/);
    });

    $scope.loadSlider = function () {
      $scope.loading = true;
      $scope.error = null;
      $scope.slider = LoanResource.get(null, function (slider) {
        $scope.loading = false;

        $scope.canApply = slider.CanApply;

        $scope.capital = slider.Rules.MaxLoanAmount;
        $scope.maxCapital = slider.Rules.MaxLoanAmount;
        $scope.minCapital = slider.Rules.MinLoanAmount;

        $scope.period = slider.Rules.MaxLoanPeriod;
        $scope.maxPeriod = slider.Rules.MaxLoanPeriod;
        $scope.minPeriod = slider.Rules.MinLoanPeriod;

        updateRepaymentDate();
        updateCalcs();

        $scope.dateOptions.maxDate = '+' + $scope.maxPeriod + 'd';
        $scope.dateOptions.minDate = '+' + $scope.minPeriod + 'd';

        // If slider cookie is present, use the slider values
        if ($cookies.LoanDto) {
          var result = angular.fromJson($cookies.LoanDto);
          angular.extend($scope, {
            capital: result.Amount,
            period: result.Period,
          });
        }
      }, function (e) {
        handleUnauthorized(e);
        $scope.loading = false;
        $scope.error = {
          message: 'There was an error while loading the slider. Click retry to try again.',
          retryHandler: $scope.loadSlider
        };
      });
    };
    
    $scope.loadSlider();
  }]);
}(this.angular));
