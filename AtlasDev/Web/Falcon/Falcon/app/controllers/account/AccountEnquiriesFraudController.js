(function(angular) {
    'use strict';
    var app = angular.module('falcon');

    app.controller('AccountEnquiriesFraudController', ['$scope', 'FalconTable', '$FalconDataProvider', '$FalconChangeSelectionProvider', 'AccountResource', 'toaster',
        function($scope, FalconTable, $fdp, $fcsp, AccountResource, toaster) {

            $scope.changeSelection = function(targetRow, sourceRow, dataSource) {
                $fcsp.changeSelection(targetRow, sourceRow, dataSource);
                $scope.row = $fcsp.getSelectedItem('fraudEnquiries');

                if ($scope.row) {
                    _setFraudReasons($scope.row.Reasons);
                } else {
                    _setFraudReasons([]);
                }
            };

            var _update = function() {
                var data = $fdp.data('AccountData');

                if (data) {
                    $scope.fraudEnquiries = data.FraudEnquiries;

                    $fcsp.registerDataSource('fraudEnquiries', $scope.fraudEnquiries);

                    // Setup params for tables
                    if ($scope.fraudParams) {
                        $scope.fraudParams.reload();
                    } else {
                        $scope.fraudParams = FalconTable.new($scope, 'fraudEnquiries', 10, [10]);
                    }
                }
            };

            var _setFraudReasons = function(reasons) {
                $scope.fraudReasons = reasons;
                if ($scope.fraudReasonParams) {
                    $scope.fraudReasonParams.reload();
                } else {
                    $scope.fraudReasonParams = FalconTable.new($scope, 'fraudReasons', 10, [10]);
                }

            };

            $scope.fraudOverrideScore = function(pId) {
                $scope.is_processing = true;
                AccountResource.overrideFraudScore($scope.row.FraudScoreId, pId, $scope.token, $scope.fraud.OverrideReason).then(function(result) {

                    toaster.pop('success', "Override", "Override saved!");

                    $fdp.promptUpdateRegistars();
                    $scope.fraud.OverrideReason = null;
                    $scope.is_processing = false;
                    $scope.dismiss();
                });
            };
            // Setup listener
            $fdp.registerListenerCallback(_update);
        }
    ]);
}(this.angular));