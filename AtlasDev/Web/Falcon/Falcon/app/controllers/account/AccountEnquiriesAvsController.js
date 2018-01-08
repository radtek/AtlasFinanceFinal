(function(angular) {
    'use strict';
    var app = angular.module('falcon');

    app.controller('AccountEnquiriesAvsController', ['$scope', 'AccountResource', 'FalconTable', '$FalconDataProvider', '$FalconChangeSelectionProvider', 'AvsResource', 'toaster',
        function($scope, AccountResource, FalconTable, $fdp, $fcsp, AvsResource, toaster) {

            $scope.changeSelection = function(targetRow, sourceRow, dataSource) {
                $fcsp.changeSelection(targetRow, sourceRow, dataSource);
                $scope.avRowSelected = $fcsp.getSelectedItem(dataSource);
            };

            var update = function() {
                var data = $fdp.data('AccountData');
                if (data) {
                    $scope.avsTransactions = data.AvsTransactions;

                    $fcsp.registerDataSource('avsTransactions', $scope.avsTransactions);

                    if ($scope.avsParams)
                        $scope.avsParams.reload();
                    else
                        $scope.avsParams = FalconTable.new($scope, 'avsTransactions', 10, [10]);
                }
            };

            $scope.avsResultHtml = function(avs) {
                return '<table class=\"table\"><tbody>' +
                    '<tr class=\"' + avs.ResponseAcceptsCredit + '\"><td><center>Accepts Credit</center></td></tr>' +
                    '<tr class=\"' + avs.ResponseAcceptsDebit + '\"><td><center>Accepts Debit</center></td></tr>' +
                    '<tr class=\"' + avs.ResponseAccountNumber + '\"><td><center>Account Number</center></td></tr>' +
                    '<tr class=\"' + avs.ResponseAccountOpen + '\"><td><center>Account Open</center></td></tr>' +
                    '<tr class=\"' + avs.ResponseIdNumber + '\"><td><center>ID Number</center></td></tr>' +
                    '<tr class=\"' + avs.ResponseInitials + '\"><td><center>Initials</center></td></tr>' +
                    '<tr class=\"' + avs.ResponseLastName + '\"><td><center>Last Name</center></td></tr>' +
                    '<tr class=\"' + avs.ResponseOpenThreeMonths + '\"><td><center>Open Three Months</center></td></tr>' +
                    '</tbody></table>' +
                    '<table class=\"table\"><thead><tr><td>Legend</td></tr></thead><tbody>' +
                    '<tr class=\"bg-green\"><td><center>Passed</center></td></tr>' +
                    '<tr class=\"bg-yellow\"><td><center>Warning</center></td></tr>' +
                    '<tr class=\"bg-red\"><td><center>Failed</center></td></tr>' +
                    '</tbody></table>';
            };

            $scope.cancelAvs = function(transactionId) {
                AvsResource.cancelAVS(transactionId, $scope.token).then(function (result) {
                    if (result.status === 200) {
                        $scope.dismiss();
                        toaster.pop('success', "Cancel AVS", "AVS has been cancelled!");
                        $scope.avRowSelected.StatusId = 4;
                        $scope.avRowSelected.Status = 'Cancelled';
                        $scope.avRowSelected.$selected = false;
                    } else {
                        toaster.pop('danger', "Cancel AVS", "There was a problem cancelling AVS!");
                    }
                });
            };

            $scope.resendAvs = function(transactionId) {
                AvsResource.resendAVS(transactionId, $scope.token).then(function(result) {
                    if (result.status === 200) {
                        $scope.dismiss();
                        toaster.pop('success', "Resend AVS", "AVS has been resent!");
                        $fdp.promptUpdateRegistars();
                    } else {
                        toaster.pop('danger', "Resend AVS", "There was a problem resending AVS!");
                    }
                });
            };

            // Setup listener
            $fdp.registerListenerCallback(update);
        }
    ]);
}(this.angular));