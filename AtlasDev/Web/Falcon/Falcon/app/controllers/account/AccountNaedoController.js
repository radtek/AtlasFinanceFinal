(function(angular) {
    'use strict';
    var app = angular.module('falcon');

    app.controller('AccountNaedoController', ['$scope', 'AccountResource', 'FalconTable', '$filter', '$FalconDataProvider', '$FalconChangeSelectionProvider', 'NaedoResource', 'toaster',
        function($scope, AccountResource, FalconTable, $filter, $fdp, $fcsp, NaedoResource, toaster) {

            var newdate = new Date();
            newdate.setDate(newdate.getDate() + 3);
            var date = $filter('date')(new Date(newdate), 'yyyy-MM-dd');
            $scope.additonalDebitActionDate = date;
            $scope.additonalDebitAmount = "R 0.00";
            $scope.accountId = '';

            $scope.changeSelection = function(targetRow, sourceRow, dataSource) {
                $fcsp.changeSelection(targetRow, sourceRow, dataSource);

                $scope[targetRow] = $fcsp.getSelectedItem(dataSource);

                if ($scope[targetRow] && dataSource === 'naedoControls') {
                    _naedoLoad($fcsp.getDataSourceCollection('naedo_' + $scope[targetRow].ControlId));
                }
            };

            $scope.transactionNgShow = function() {
                return ($scope.ncRowSelected && $scope.ncRowSelected.$selected);
            };

            var update = function() {
                var data = $fdp.data('AccountData');
                if (data) {
                    $scope.naedoControls = data.DebitControls;

                    $fcsp.registerDataSource('naedoControls', $scope.naedoControls);

                    // Setup params for tables
                    if ($scope.naedoControlParams)
                        $scope.naedoControlParams.reload();
                    else
                        $scope.naedoControlParams = FalconTable.new($scope, 'naedoControls', 10, [10]);

                    for (var i = 0; i < $scope.naedoControls.length; i++) {
                        var naedoKey = 'naedo_' + $scope.naedoControls[i].ControlId;
                        $fcsp.registerDataSource(naedoKey, $scope.naedoControls[i].Transactions);
                    }
                }
            };

            var _naedoLoad = function(naedos) {
                $scope.naedos = naedos;
                if ($scope.naedoParams) {
                    $scope.naedoParams.reload();
                } else {
                    $scope.naedoParams = FalconTable.new($scope, 'naedos', 10, [10, 20]);
                }
            };

            $scope.cancelAdditionalDebit = function(controlId, transactionId) {
                NaedoResource.canAdditionalDebit(controlId, transactionId).then(function(result) {
                    if (result.status === 200) {
                        $scope.dismiss();
                        toaster.pop('success', "Cancel Additional Debit", "Transaction has been cancelled!");
                        $scope.naRowSelected.Status = 'Cancelled';
                        $scope.naRowSelected.StatusId = 2;
                        $scope.naRowSelected.StatusColor = 'label label-warning';
                    } else {
                        toaster.pop('danger', "Cancel Additional Debit", result);
                    }
                });
            };

            $scope.addAdditionalDebit = function(controlId) {
                var amount = $scope.additonalDebitAmount.replace("R", "");
                $scope.naShowAmountValidation = (amount <= 5);
                $scope.naShowActionDateValidation = ($scope.additonalDebitActionDate < date);
                if ($scope.naShowAmountValidation === true || $scope.naShowActionDateValidation === true)
                    return;

                NaedoResource.addAdditionalDebit(controlId, amount, $scope.additonalDebitActionDate).then(function(result) {
                    if (result.status === 200) {
                        $scope.dismiss();
                        toaster.pop('success', "Add Additional Debit", "Additional Transaction has been added!");
                        $fdp.promptUpdateRegistars();
                    } else {
                        toaster.pop('danger', "Add Additional Debit", result);
                    }
                });
            };

            // Setup listener
            $fdp.registerListenerCallback(update);

        }
    ]);
}(this.angular));