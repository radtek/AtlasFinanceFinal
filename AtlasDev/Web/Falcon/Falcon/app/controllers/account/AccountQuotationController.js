(function(angular) {
    'use strict';
    var app = angular.module('falcon');

    app.controller('AccountQuotationController', ['$scope', 'FalconTable', '$FalconDataProvider', '$FalconChangeSelectionProvider', 'UniversalResource', 'toaster', 'QuotationResource', 'AccountResource', '$filter',
        function($scope, FalconTable, $fdp, $fcsp, UniversalResource, toaster, QuotationResource, AccountResource, $filter) {
            $scope.changeSelection = function(targetRow, sourceRow, dataSource) {
                $fcsp.changeSelection(targetRow, sourceRow, dataSource);

                $scope[targetRow] = $fcsp.getSelectedItem(dataSource);
            };

            var update = function() {
                var data = $fdp.data('AccountData');
                if (data) {
                    $scope.account = data;
                    $scope.quotations = data.Quotations;

                    $fcsp.registerDataSource('quotations', $scope.quotations);

                    if ($scope.quotationParams)
                        $scope.quotationParams.reload();
                    else
                        $scope.quotationParams = FalconTable.new($scope, 'quotations', 10, [10]);

                    $scope.appliedAmount = $filter('currency')($scope.account.LoanAmount).replace(",", ".");
                    $scope.period = $scope.account.Period;
                    $scope.repaymentDate = $filter('date')(new Date($scope.account.FirstInstalmentDate), 'yyyy-MM-dd');

                    getPublicHolidays();
                }
            };
            // Setup listener
            $fdp.registerListenerCallback(update);

            $scope.rejectQuotation = function() {
                QuotationResource.reject($scope.account.AccountId, $scope.quOptionRowSelected.QuotationId).then(function(resp) {
                    if (result.status === 200) {
                        $scope.dismiss();
                        toaster.pop('success', "Reject Quotation", "Quotation has been rejected!");
                        $fdp.promptUpdateRegistars();
                    } else {
                        toaster.pop('danger', "Reject Quotation", result);
                    }
                });
            };

            var getPublicHolidays = function() {
                UniversalResource.getPublicHolidaysFromToday($scope.token).then(function(resp) {
                    if (resp.status === 200) {
                        $scope.publicHolidays = resp.data.publicHolidays;
                    } else {
                        toaster.pop('danger', "Loading Public Holidays", "There was a problem loading Public Holidays!");
                        console.error(resp);
                    }
                });
            };

            $scope.adjustQuotation = function(loanAmount, period) {
                AccountResource.adjustAccount($scope.account.AccountId, loanAmount.replace("R", ""), period, $scope.token).then(function(result) {
                    if (result.status === 200) {
                        $scope.dismiss();
                        toaster.pop('success', "Adjust Account", "Account has been adjusted!");
                        $fdp.promptUpdateRegistars();
                    } else {
                        toaster.pop('danger', "Adjust Account", result);
                    }
                });
            };

            $scope.updateRepaymentDate = function(period) {
                var repaymentDate = new Date(new Date($scope.account.FirstInstalmentDate).valueOf());
                repaymentDate.setDate(repaymentDate.getDate() + (period - $scope.account.Period));
                $scope.repaymentDate = $filter('date')(repaymentDate, 'yyyy-MM-dd');

                $scope.naShowRepaymentDateValidation = !isValidRepaymentDate(repaymentDate);

                //if (repaymentDate.getDay() === 0) {
                //  $scope.naShowRepaymentDateValidation = true;
                //}
                //else {
                //  for (var i = 0; i < $scope.publicHolidays.length; i++) {
                //    if (repaymentDate === $scope.publicHolidays[i].Date) {
                //      $scope.naShowRepaymentDateValidation = true;
                //      break;
                //    }
                //  }
                //}
            };

            $scope.updatePeriod = function(repaymentDate) {
                var repaymentDateT = new Date(repaymentDate);
                var periodShift = Math.ceil((repaymentDateT.valueOf() - new Date($scope.account.FirstInstalmentDate).valueOf()) / (24 * 60 * 60 * 1000)); //hours * minutes * seconds * milliseconds

                $scope.period = $scope.account.Period + periodShift;

                $scope.naShowRepaymentDateValidation = !isValidRepaymentDate(repaymentDateT);
            };

            var isValidRepaymentDate = function(date) {
                if (date.getDay() === 0) {
                    return false;
                } else {
                    for (var i = 0; i < $scope.publicHolidays.length; i++) {
                        if (date === $scope.publicHolidays[i].Date) {
                            return false;
                        }
                    }
                }

                return true;
            };
        }
    ]);
}(this.angular));