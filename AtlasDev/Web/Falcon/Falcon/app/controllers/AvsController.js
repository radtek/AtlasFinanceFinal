(function(angular) {
    'use strict';

    var app = angular.module('falcon');

    app.controller('AvsController', ['$scope', '$filter', 'AvsResource', 'xSocketsProxy', '$sce', 'toaster', '$rootScope', 'ngProgress','$compile',
        function($scope, $filter, AvsResource, xSocketsProxy, $sce, toaster, $rootScope, ngProgress, $compile) {
            $rootScope.$broadcast('menu', {
                name: 'Avs',
                Desc: 'transactions',
                url: '/#!/Avs/Transactions/',
                searchVisible: true
            });

            var _date = $filter('date')(new Date(), 'yyyy-MM-dd');

            $scope.isLoaded = false;
            $scope.filterCompany = 0;

            $scope.filterStartDate = $scope.filterStartDate || _date

            $scope.filterEndDate = $scope.filterEndDate || _date;
            $scope.filterBank = 0;
            $scope.filterIdNumber = '';
            $scope.filterTransactionId = '';
            $scope.isRefreshing = false;
            $scope.limit = 16;

            $scope.loaded = false;

            xSocketsProxy.connection('Analytics');
            xSocketsProxy.publish('update_avs');
            xSocketsProxy.subscribe("update_avs").process(function(data) {
                _get();
            });
            var _timediff = function(start, end) {
                if (start && end)
                    return moment.utc(moment(end).diff(moment(start))).format("HH:mm:ss");
                else
                    return "";
            }


            $scope.avsResultHtml = function(avs) {
                return $sce.trustAsHtml('<table class=\"table\"><tbody>' +
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
                    '<tr class=\"label-success\"><td><center>Passed</center></td></tr>' +
                    '<tr class=\"label-warning\"><td><center>Warning</center></td></tr>' +
                    '<tr class=\"label-danger\"><td><center>Failed</center></td></tr>' +
                    '</tbody></table>');
            };

            var _openRows = [];

            $scope.results = function(avs) {
                var row = '<tr id="avs_result_' + avs.TransactionId + '"><td colspan="12">Results go here</td></tr>'
                var rowId = "#avs_result_" + avs.TransactionId;

                // Remove all previous open rows.
                for (var i = _openRows.length - 1; i >= 0; i--) {
                    angular.element(_openRows[i]).remove();
                    _openRows.splice(i, 1);
                };
                if (angular.element(rowId).length === 0) {
                    _openRows.push(rowId);
                    angular.element('#transaction_' + avs.TransactionId).after($compile($sce.trustAsHtml(row).toString())($scope));
                } else {
                    angular.element(rowId).remove();
                }
            }

            var _getStaticData = function () {
                AvsResource.getStaticData($scope.token).then(function (result) {
                    if (result.status === 200) {
                        $scope.banks = result.data.banks;
                        $scope.services = result.data.services;
                        $scope.isLoaded = true;
                    } else {
                        toaster.pop('danger', "Loading Static Data", "Error loading Static Data!");
                        console.error(result.statusText);
                    }
                }, function (result) {
                    toaster.pop('danger', "Loading Static Data", "Error loading Static Data!");
                    console.error(result.statusText);
                });
            }

            var _get = function() {
                //$scope.gridDb.dataSource.load();
                AvsResource.getTransactions($scope.filterCompany, $scope.filterStartDate, $scope.filterEndDate, $scope.filterTransactionId, $scope.filterIdNumber, $scope.filterBank, $scope.token).then(
                    function(resp) {
                        if (resp.status === 200) {
                            $scope.avsData = resp.data.Transactions;
                            $scope.avsStats = resp.data.Statistics;
                            $scope.isRefreshing = false;
                            ngProgress.complete();

                            for (var i = $scope.avsData.length - 1; i >= 0; i--) {
                                $scope.avsData[i].TimeTaken = _timediff($scope.avsData[i].CreateDate, $scope.avsData[i].ResponseDate);
                            };

                            /*if ($scope.tableParams) {
                                $scope.tableParams.reload();
                            } else {
                                $scope.tableParams = FalconTable.new($scope, 'avsData', 20);
                            }*/

                            $scope.isLoaded = true;
                        } else {
                            toaster.pop('danger', "Loading AVS Transactions", "There was a problem loading AVS Transactions!");
                            console.error(resp);
                        }
                    });
                $scope.isLoaded = true;
            };

            var _getCompany = function() {
                $scope.companies = [];
            };

            //var _getBanks = function() {
            //    AvsResource.getBanks($scope.token).then(function(resp) {
            //        if (resp.status === 200) {
            //        } else {
            //            toaster.pop('danger', "Loading Banks", "There was a problem loading Banks!");
            //            console.error(resp);
            //        }
            //    });
            //};

            var _cancelTransaction = function (transactionId, load) {
                AvsResource.cancelAVS(transactionId, $scope.token).then(function(result) {
                    if (result.status === 200) {
                        if (load)
                            _get();
                        toaster.pop('success', "Cancel AVS", "AVS transaction has been cancelled!");
                    } else {
                        toaster.pop('danger', "Cancel AVS", "There was a problem cancelling transaction no. " + transactionId + "!");
                        console.log("error cancelling avs transactions: " + transactionId);
                    }
                });
            }

            //function resend(transactionId, load) {
            //    AvsResource.resendAVS(transactionId, $scope.token).then(function(result) {
            //        if (result.status === 200) {
            //            if (load)
            //                _get();
            //            toaster.pop('success', "Resend AVS", "AVS transaction has been resent!");
            //        } else {
            //            toaster.pop('danger', "Resend AVS", "There was a problem resending transaction no. " + transactionId + "!");
            //            console.log("error resending avs transactions: " + transactionId);
            //        }
            //    });
            //}

            $scope.cancel = function() {
                console.log("cancelling ");
                var transactionIds = [];
                for (var i = $scope.avsData.length - 1; i >= 0; i--) {
                    if ($scope.avsData[i].Selected) {
                        transactionIds.push($scope.avsData[i].TransactionId);
                    }
                }
                for (i = transactionIds.length - 1; i >= 0; i--) {
                    _cancelTransaction(transactionIds[i], i === 0);
                }
            };

            $scope.resend = function(avs, serviceId) {
                console.log("resending");
                var transactionIds = [];
                AvsResource.resend(avs.TransactionId, serviceId).then(function(data) {
                    toaster.pop('success', "Resend AVS", "AVS transaction has been resent!");
                }, function(data) {
                    toaster.pop('danger', "Resend AVS", "There was a problem resending transaction no. " + avs.TransactionId, +"!");
                });
                /*for (var i = $scope.avsData.length - 1; i >= 0; i--) {
                    if ($scope.avsData[i].Selected) {
                        transactionIds.push($scope.avsData[i].TransactionId);
                    }
                }
                for (i = transactionIds.length - 1; i >= 0; i--) {
                    resend(transactionIds[i], i === 0);
                }*/
            };

            $scope.resendMultiple = function (serviceId) {
                angular.forEach($scope.avsData, function (value, key) {
                    if (value.Selected === true) {
                        AvsResource.resend(value.TransactionId, serviceId).then(function (data) {
                            toaster.pop('success', "Resend AVS", "AVS transaction has been resent!");
                            value.Selected = false;
                        }, function (data) {
                            toaster.pop('danger', "Resend AVS", "There was a problem resending transaction no. " + value.TransactionId, +"!");
                        });
                    }
                });
            }

            $scope.refresh = function() {
                $scope.isRefreshing = true;
                ngProgress.start();
                _get();

            };

            $scope.getCompanys = function() {
                _getCompany();
            };

            $scope.getBanks = function() {
                _getBanks();
            };

            $scope.init = function() {
                _getStaticData();
                _get();
            };
        }
    ]);

    app.controller('AvsAdminController', ['$scope', 'AvsResource', 'toaster',
        function($scope, AvsResource, toaster) {
            $scope.unlinkedBanks = [];
            $scope.showBankWarning = false;
            $scope.loaded = false;

            var _getServiceSchedule = function() {
                $scope.loaded = false;

                AvsResource.getServiceSchedule($scope.token).then(function(result) {
                    if (result.status === 200) {
                        $scope.services = result.data.services;
                        $scope.serviceBanks = {};
                        var serviceBanksTemp = result.data.serviceBanks;
                        for (var i = 0; i < $scope.services.length; i++) {
                            for (var j = 0; j < serviceBanksTemp.length; j++) {
                                if (serviceBanksTemp[j].ServiceId == $scope.services[i].ServiceId) {
                                    if (!$scope.serviceBanks[$scope.services[i].ServiceId])
                                        $scope.serviceBanks[$scope.services[i].ServiceId] = [];
                                    $scope.serviceBanks[$scope.services[i].ServiceId].push(serviceBanksTemp[j]);
                                }
                            }
                        }
                        if ($scope.services)
                            $scope.getBanks($scope.services[0].ServiceId);
                        $scope.loaded = true;
                    } else {
                        toaster.pop('danger', "Service Settings", "Error loading Service Settings!");
                        console.error(result);
                    }
                });
            };

            $scope.getBanks = function(serviceId) {
                $scope.selectedServiceBanks = $scope.serviceBanks[serviceId];
                for (var i = 0; i < $scope.services.length; i++) {
                    if ($scope.services[i].ServiceId == serviceId)
                        $scope.selectedService = $scope.services[i];
                }
            };

            $scope.checked = function (serviceBank) {
                serviceBank.IsLinked = !serviceBank.IsLinked;

                if (serviceBank.IsLinked) {
                    //unlink from other services
                    for (var j = 0; j < $scope.services.length; j++) {
                        for (var i = 0; i < $scope.serviceBanks[$scope.services[j].ServiceId].length; i++) {
                            if ($scope.serviceBanks[$scope.services[j].ServiceId][i].BankId == serviceBank.BankId
                                && $scope.serviceBanks[$scope.services[j].ServiceId][i].ServiceId != serviceBank.ServiceId) {
                                $scope.serviceBanks[$scope.services[j].ServiceId][i].IsLinked = false;
                            }
                        }
                    }
                    // update unlinked banks
                    $scope.unlinkedBanks = jQuery.grep($scope.unlinkedBanks, function (value) {
                        return value.BankId != serviceBank.BankId;
                    });
                } else {
                    $scope.unlinkedBanks.push(serviceBank);
                }

                //if (!serviceBank.IsLinked) {
                //    // unlink from other services
                //    for (var j = 0; j < $scope.services.length; j++) {
                //        for (var i = 0; i < $scope.serviceBanks[$scope.services[j].ServiceId].length; i++) {
                //            if ($scope.serviceBanks[$scope.services[j].ServiceId][i].BankId == serviceBank.BankId
                //                && $scope.serviceBanks[$scope.services[j].ServiceId][i].ServiceId != serviceBank.ServiceId) {
                //                $scope.serviceBanks[$scope.services[j].ServiceId][i].IsLinked = false;
                //            }
                //        }
                //    }

                //    // update unlinked banks
                //    $scope.unlinkedBanks = jQuery.grep($scope.unlinkedBanks, function (value) {
                //        return value.BankId != serviceBank.BankId;
                //    });
                //} else {
                //    $scope.unlinkedBanks.push(serviceBank);
                //}

                //if (!serviceBank.IsLinked) {
                //    for (var j = 0; j < $scope.services.length; j++) {
                //        for (var i = 0; i < $scope.serviceBanks[$scope.services[j].ServiceId].length; i++) {
                //            if ($scope.serviceBanks[$scope.services[j].ServiceId][i].BankId == serviceBank.BankId
                //                && $scope.serviceBanks[$scope.services[j].ServiceId][i].ServiceId == serviceBank.ServiceId) {
                //                $scope.serviceBanks[$scope.services[j].ServiceId][i].IsLinked = serviceBank.IsLinked;
                //            }
                //        }
                //    }
                //    $scope.unlinkedBanks = jQuery.grep($scope.unlinkedBanks, function (value) {
                //        return value.BankId != serviceBank.BankId;
                //    });
                //} else {
                //    $scope.unlinkedBanks.push(serviceBank);
                //}
                $scope.showBankWarning = ($scope.unlinkedBanks.length > 0);
            };

            $scope.init = function() {
                _getServiceSchedule();
            };

            $scope.reset = function() {
                $scope.init();
            };

            $scope.apply = function() {
                _saveService();
            };

            var _saveService = function () {
                var serviceBanks = [];
                for (var i = 0; i < $scope.services.length; i++) {
                    $.merge(serviceBanks, $scope.serviceBanks[$scope.services[i].ServiceId]);
                }

                AvsResource.updateServiceSettings(JSON.stringify($scope.services), JSON.stringify(serviceBanks), $scope.token).then(function (result) {
                    if (result.status === 200) {
                        console.log("successfully saved service settings");
                        toaster.pop('success', "Service Settings", "Schedules have been successfully updated!");
                    } else {
                        toaster.pop('danger', "Service Settings", "Error saving Service Settings!");
                        console.log("error saving service settings: " + JSON.stringify($scope.serviceBanks));
                    }
                });
            }
        }
    ]);

}(this.angular));