(function(angular) {
    'use strict';

    var app = angular.module('falcon');

    app.controller('NaedoController', ['$scope', 'BatchStatus', 'NaedoResource', 'FalconTable', '$filter', '$rootScope', 'ngProgress',
        function($scope, BatchStatus, NaedoResource, FalconTable, $filter, $rootScope, ngProgress) {
            $rootScope.$broadcast('menu', {
                name: 'Debit Orders',
                Desc: 'transacting',
                url: '/#!/Naedo/Batch/',
                searchVisible: false
            });

            $scope.hasTransactions = false;
            $scope.naedoTransactions = [];
            var date = $filter('date')(new Date(), 'yyyy-MM-dd');
            $scope.filterStartDate = $scope.filterStartDate || date
            $scope.filterEndDate = $scope.filterEndDate || date;
            $scope.message = null;


            var _get = function(id) {

                NaedoResource.getBatches(null, null, null, $scope.filterStartDate, $scope.filterEndDate, $scope.token).then(function(result) {
                    $scope.naedoTransactions = result.data;
                    if (result.data.length > 0)
                        $scope.hasTransactions = true;

                    for (var i = $scope.naedoTransactions.length - 1; i >= 0; i--) {
                        /*jshint -W069 */
                        $scope.naedoTransactions[i]["StatusColour"] = BatchStatus[$scope.naedoTransactions[i]["BatchStatus"]];
                    }
                    if ($scope.naedoParams) {
                        $scope.naedoParams.reload();
                    } else {
                        $scope.naedoParams = FalconTable.new($scope, 'naedoTransactions', 10, [10]);
                    }
                    ngProgress.complete();
                });
            };

            $scope.refresh = function() {
                ngProgress.start();
                _get();
            };

            $scope.setBatchId = function(id) {
                $scope.batchId = id;
            };

            $scope.confirmDeletion = function(id) {
                alert(id);
            };

            $scope.manage = function(id) {
                window.location = "/#!/Naedo/Batch/Transactions/" + id;
            };

            $scope.init = function(id) {
                _get(id);
            };
        }
    ]);


    app.controller('NaedoTransactionController', ['$scope', 'NaedoTransactionStatus', 'FalconTable', 'NaedoResource', '$rootScope', '$routeParams',
        function($scope, NaedoTransactionStatus, FalconTable, NaedoResource, $rootScope, $routeParams) {
            $scope.transactionId = null;
            $rootScope.$broadcast('menu', {
                name: 'Debit Orders',
                Desc: 'batch transactions',
                url: '/#!/Naedo/Batch/Transactions/' + $routeParams.transactionId,
                searchVisible: true
            });
            $scope.recordNotFound = false;
            var _get = function(id) {

                NaedoResource.getTransaction(id, $scope.token).then(function(result) {
                    if (result.data.length > 0) {

                        var rec = result.data[0];
                        $scope.batchStatusDescription = rec.BatchStatusDescription;
                        $scope.batchStatus = rec.BatchStatus;
                        $scope.transmissionNo = rec.TransmissionNo;
                        $scope.submitDate = rec.SubmitDate;
                        $scope.lastStatusDate = rec.LastStatusDate;
                        $scope.transmissionAccepted = rec.TransmissionAccepted ? 'Yes' : 'No';
                        $scope.createDate = rec.CreateDate;
                        $scope.actionDate = rec.NaedoBatchTransactions[0].ActionDate;

                        $scope.naedoTransactions = rec.NaedoBatchTransactions;

                        $scope.overrideTrackingDayColumn = false;
                        $scope.overrideTrackingDate = false;
                        $scope.overrideTrackingAmount = false;
                        $scope.cancelDate = false;

                        for (var i = $scope.naedoTransactions.length - 1; i >= 0; i--) {

                            if (!$scope.overrideTrackingDayColumn)
                                $scope.overrideTrackingDayColumn = $scope.naedoTransactions[i].OverrideTrackingDays ? true : false;

                            if (!$scope.overrideTrackingDate)
                                $scope.overrideTrackingDate = $scope.naedoTransactions[i].OverrideActionDate ? true : false;

                            if (!$scope.overrideTrackingAmount)
                                $scope.overrideTrackingAmount = $scope.naedoTransactions[i].OverrideAmount ? true : false;

                            if (!$scope.cancelDate)
                                $scope.cancelDate = $scope.naedoTransactions[i].CancelDate !== null ? true : false;

                            $scope.naedoTransactions[i]["StatusColour"] = NaedoTransactionStatus[$scope.naedoTransactions[i]["Status"]];
                        };
                        if ($scope.transactionParams) {
                            $scope.transactionParams.reload();
                        } else {
                            $scope.transactionParams = FalconTable.new($scope, 'naedoTransactions', 10, [10]);
                        }
                    } else {
                        $scope.recordNotFound = true;
                    }
                });
            }

            $scope.manage = function(id, tId) {
                window.location = "/#!/Naedo/Batch/Control/" + id + '/' + tId;
            }

            $scope.init = function() {
                $scope.transactionId = $routeParams.transactionId;
                _get($routeParams.transactionId);
            };
        }
    ]);

}(this.angular));