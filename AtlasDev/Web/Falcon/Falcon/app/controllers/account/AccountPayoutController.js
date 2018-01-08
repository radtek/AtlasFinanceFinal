(function(angular) {
    'use strict';
    var app = angular.module('falcon');

    app.controller('AccountPayoutController', ['$scope', 'AccountResource', 'FalconTable', '$FalconDataProvider', '$FalconChangeSelectionProvider', 'PayoutResource', 'toaster',
        function($scope, AccountResource, FalconTable, $fdp, $fcsp, PayoutResource, toaster) {

            $scope.changeSelection = function(targetRow, sourceRow, dataSource) {
                $fcsp.changeSelection(targetRow, sourceRow, dataSource);
                $scope.poRowSelected = $fcsp.getSelectedItem(dataSource);
            };

            var update = function() {
                var data = $fdp.data('AccountData');

                if (data) {
                    $scope.payouts = data.Payouts;

                    // Register data source to grid selection provider.
                    $fcsp.registerDataSource('payouts', $scope.payouts);

                    // Setup params for tables
                    if ($scope.payoutParams)
                        $scope.payoutParams.reload();
                    else
                        $scope.payoutParams = FalconTable.new($scope, 'payouts', 10, [10]);
                }
            };

            $scope.payoutHold = function(payout) {
                PayoutResource.holdPayment(payout, $scope.token).then(function (result) {
                    $scope.dismiss();
                    if (result.PayoutId) {
                        toaster.pop('success', "Hold Payout", "Payout has been placed on hold!");
                        payout = result;
                    } else {
                        toaster.pop('danger', "Hold Payout", result);
                    }
                });
            };

            $scope.payoutRelease = function(payout) {
                PayoutResource.releasePayment(payout, $scope.token).then(function (result) {
                    $scope.dismiss();
                    if (result.PayoutId) {
                        toaster.pop('success', "Release Payout", "Payout has been released!");
                        payout = result;
                    } else {
                        toaster.pop('danger', "Release Payout", result);
                    }
                });
            };

            // Setup listener
            $fdp.registerListenerCallback(update);

        }
    ]);
}(this.angular));