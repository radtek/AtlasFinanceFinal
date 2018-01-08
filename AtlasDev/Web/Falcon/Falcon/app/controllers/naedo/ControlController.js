(function(angular) {
    'use strict';

    var app = angular.module('falcon');

    app.controller('ControlController', ['$scope', 'FalconTable', 'ControlStatus', 'NaedoResource', '$filter', '$rootScope', 'ngProgress',
        function($scope, FalconTable, ControlStatus, NaedoResource, $filter, $rootScope, ngProgress) {
            $rootScope.$broadcast('menu', {
                name: 'Debit Orders',
                Desc: 'orders',
                url: '/Naedo/Control/Index/',
                searchVisible: false
            });
            $scope.hasTransactions = false;
            var date = $filter('date')(new Date(), 'yyyy-MM-dd');
            $scope.filterActionStartDate = $scope.filterActionStartDate || date
            $scope.filterActionEndDate = $scope.filterActionEndDate || date;

            var _get = function(id) {
                NaedoResource.getControls(4, null, $scope.filterActionStartDate, $scope.filterActionEndDate, true, $scope.token).then(function(response) {
                    $scope.hasTransactions = true;
                    for (var i = response.data.length - 1; i >= 0; i--) {
                        response.data[i]["ControlStatusColour"] = ControlStatus[response.data[i]["ControlStatus"]];
                    };

                    $scope.controlTransactions = response.data;

                    if ($scope.controlParams) {
                        $scope.controlParams.reload();
                    } else {
                        $scope.controlParams = FalconTable.new($scope, 'controlTransactions', 10, [10]);
                    }
                    ngProgress.complete();
                });
            };

            $scope.refresh = function() {
                ngProgress.start();
                _get();
            }

            $scope.setControlId = function(id) {
                $scope.controlId = id;
            }

            $scope.confirmDeletion = function(id) {
                alert(id);
            }

            $scope.manage = function(id) {
                window.location = "/#!/Naedo/Batch/Control/" + id + "/0";
            }

            $scope.init = function(id) {
                _get(id);
            };
        }
    ]);

    app.controller('NaedoControlController', ['$scope', 'NaedoTransactionStatus', 'FalconTable', 'NaedoResource', '$rootScope', '$routeParams', 'ngProgress',
        function($scope, NaedoTransactionStatus, FalconTable, NaedoResource, $rootScope, $routeParams, ngProgress) {
            $rootScope.$broadcast('menu', {
                name: 'Debit Orders',
                Desc: 'control',
                url: '/#!/Naedo/Batch/Control/' + $routeParams.controlId + '/' + $routeParams.transactionId,
                searchVisible: false
            });
            $scope.recordNotFound = false;


            var _get = function(id) {
                NaedoResource.getControl(id, $scope.token).then(function(result) {

                    var control = result.data;
                    $scope.controlStatus = control.ControlStatusDescription;
                    $scope.controlType = control.ControlTypeDescription;
                    $scope.avsCheckType = control.AVSCheckTypeDescription;
                    $scope.bankStatementRef = control.BankStatementReference;
                    $scope.currentRep = control.CurrentRepetition;
                    $scope.failureType = control.FailureTypeDescription;
                    $scope.firstInstalmentDate = control.FirstInstalmentDate;
                    $scope.frequency = control.FrequencyDescription;
                    $scope.instalment = control.Instalment;
                    $scope.lastInstalmentDate = control.LastInstalmentUpdate;
                    $scope.payDateOfEachMonth = control.PayDateDayOfMonth;
                    $scope.payeDateOfEachWeek = control.PayDateDayOfWeek;
                    $scope.payDatetype = control.PayDateTypeDescription;
                    $scope.payRule = control.PayRuleDescription;
                    $scope.repetitions = control.Repetitions;
                    $scope.thirdPartyReference = control.ThirdPartyReference;
                    $scope.trackingDays = control.TrackingDaysDescription;
                    $scope.bank = control.Bank;
                    $scope.bankAccountName = control.BankAccountName;
                    $scope.bankAccountNo = control.BankAccountNo;
                    $scope.idNumber = control.IdNumber;


                    $scope.overrideTrackingDayColumn = false;
                    $scope.overrideTrackingDate = false;
                    $scope.overrideTrackingAmount = false;
                    $scope.cancelDate = false;

                    $scope.controlTransactions = control.ResponseTransactions;
                    for (var i = $scope.controlTransactions.length - 1; i >= 0; i--) {
                        if (!$scope.overrideTrackingDayColumn)
                            $scope.overrideTrackingDayColumn = $scope.controlTransactions[i].OverrideTrackingDays ? true : false;

                        if (!$scope.overrideTrackingDate)
                            $scope.overrideTrackingDate = $scope.controlTransactions[i].OverrideActionDate ? true : false;

                        if (!$scope.overrideTrackingAmount)
                            $scope.overrideTrackingAmount = $scope.controlTransactions[i].OverrideAmount ? true : false;

                        if (!$scope.cancelDate)
                            $scope.cancelDate = $scope.controlTransactions[i].CancelDate !== null ? true : false;
                        /*jshint -W069 */
                        $scope.controlTransactions[i]["StatusColour"] = NaedoTransactionStatus[$scope.controlTransactions[i]["Status"]];
                    }
                    $scope.loaded = true;
                    if ($scope.controlParams) {
                        $scope.controlParams.reload();
                    } else {
                        $scope.controlParams = FalconTable.new($scope, 'controlTransactions', 10, [10]);
                    }
                    ngProgress.complete();


                });
            };

            $scope.init = function() {
                _get($routeParams.controlId);
            };
        }
    ]);

}(this.angular));