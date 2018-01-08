(function (angular) {
    'use strict';


    var app = angular.module('falcon');

    app.controller('PayoutController', ['$scope', '$filter', 'FalconTable', '$http', 'PayoutResource', 'toaster',
        function ($scope, $filter, FalconTable, $http, PayoutResource, toaster) {
            var date = $filter('date')(new Date(), 'yyyy-MM-dd');
            $scope.canHold = false;
            $scope.canRemoveHold = false;

            $scope.filterCompany = 0;
            $scope.filterStartDate = $scope.filterStartDate || date;
            $scope.filterEndDate = $scope.filterEndDate || date;
            $scope.filterPayoutId = 0;
            $scope.filterIdNumber = '';
            $scope.filterBank = 0;

            var _get = function () {
                PayoutResource.getTransactions($scope.filterCompany, $scope.filterStartDate, $scope.filterEndDate, $scope.filterPayoutId, $scope.filterIdNumber, $scope.filterBank, $scope.token).then(function (result) {
                    if (result.status === 200) {
                        $scope.payoutData = result.data.transactions;
                        $scope.statistics = result.data.statistics;
                        if ($scope.tableParams) {
                            console.log("reloaded");
                            $scope.tableParams.reload();
                        }
                        else {
                            console.log("loaded");
                            $scope.tableParams = FalconTable.new($scope, 'payoutData', 20, [10, 20, 50, 100]);
                        }
                        $scope.loaded = true;
                    } else {
                        console.error(result);
                    }
                });
            };

            var _getBanks = function () {
                PayoutResource.getBanks($scope.token).then(function (resp) {
                    if (resp.status === 200) {
                        $scope.banks = resp.data.banks;
                    } else {
                        console.error(resp);
                    }
                });
            };

            $scope.getBanks = function () {
                console.log("getting banks");
                _getBanks();
            };

            var _getAlerts = function () {
                PayoutResource.getAlerts($scope.token).then(function (resp) {
                    if (resp.status === 200) {
                        $scope.alerts = resp.alerts;
                    } else {
                        console.error(resp);
                    }
                });
            };

            /*jshint -W083 */
            $scope.hold = function () {
                for (var i = $scope.payoutData.length - 1; i >= 0; i--) {
                    if ($scope.payoutData[i].Selected) {
                        PayoutResource.holdPayment($scope.payoutData[i], $scope.token).then(function (result) {
                            if (result.PayoutId) {
                                toaster.pop('success', "Hold Payout", "Payout has been placed on hold!");
                                $scope.payoutData[i] = result;
                            } else {
                                toaster.pop('danger', "Hold Payout", result);
                            }
                        });
                        $scope.payoutData[i].Selected = false;
                    }
                }
            };

            $scope.removeHold = function () {
                for (var i = $scope.payoutData.length - 1; i >= 0; i--) {
                    if ($scope.payoutData[i].Selected) {
                        PayoutResource.releasePayment($scope.payoutData[i], $scope.token).then(function (result) {
                            if (result.PayoutId) {
                                toaster.pop('success', "Release Payout", "Payout has been released!");
                                $scope.payoutData[i] = result;
                            } else {
                                toaster.pop('danger', "Release Payout", result);
                            }
                        });
                        $scope.payoutData[i].Selected = false;
                    }
                }
            };

            $scope.refresh = function () {
                console.log("refreshing");
                _get();
                _getAlerts();
            };

            $scope.init = function () {
                _get();
                _getAlerts();
            };

            $scope.dataSelected = function () {
                $scope.canHold = $scope.canRemoveHold = true;
                var selectedCount = 0;
                for (var i = $scope.payoutData.length - 1; i >= 0; i--) {
                    if ($scope.payoutData[i].Selected) {
                        selectedCount++;
                        if ($scope.payoutData[i].PayoutStatusId != PayoutResource.constants.New[0]) {
                            if ($scope.canHold)
                                $scope.canHold = false;
                        }
                        if ($scope.payoutData[i].PayoutStatusId != PayoutResource.constants.OnHold[0]) {
                            if ($scope.canRemoveHold)
                                $scope.canRemoveHold = false;
                        }

                        if (!$scope.canHold && !$scope.canRemoveHold) {
                            break;
                        }
                    }
                }
                if (selectedCount === 0) {
                    $scope.canHold = $scope.canRemoveHold = false;
                }
            };
        }
    ]);
}(this.angular));