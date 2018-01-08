(function(angular) {
    'use strict';
    var app = angular.module('falcon');

    app.controller('WorkflowController', ['$scope', '$filter', 'ngTableParams', 'UniversalResource', 'WorkflowResource',
        function($scope, $filter, ngTableParams, UniversalResource, WorkflowResource) {
            var date = $filter('date')(new Date(), 'yyyy-MM-dd');

            $scope.filterHost = 0;
            $scope.filterCompany = 0;
            $scope.filterAccountNo = '';
            $scope.filterStartDate = $scope.filterStartDate || date;
            $scope.filterEndDate = $scope.filterEndDate || date;

            $scope.loaded = false;
            //var _url = function () {
            //  return '/api/Workflow';
            //};
            //var _urlUsers = function () {
            //  return '/api/User';
            //};

            var _getCompany = function() {
                $scope.companies = [];
            };

            var _getHosts = function(personId) {
                UniversalResource.getHostsLinkedToPerson(personId).then(function(result) {
                    if (result.status === 200) {
                        $scope.hosts = resp.hosts;
                    } else {
                        toaster.pop('danger', "Load hosts", "Hosts could not be loaded!");
                        console.error(result);
                    }
                });
                //$http.get(_urlUsers(), {
                //  params: {
                //    getAccessibleHostsPersonId: personId
                //  }
                //}).success(function (resp) {
                //  $scope.hosts = resp.hosts;
                //}).error(function (e) {
                //  console.error(e);
                //});
            };

            var _get = function() {
                WorkflowResource.getWorkflows($scope.filterHost, $scope.filterCompany, $scope.filterAccountNo, $scope.filterStartDate, $scope.filterEndDate).then(function(result) {
                    if (result.status === 200) {
                        if ($scope.tableParams) {
                            $scope.$watch('processes', function() {
                                $scope.tableParams.reload();
                            }, true);

                            $scope.processes = result.data.processes;
                        } else {
                            $scope.processes = result.data.processes;
                            $scope.loaded = true;
                            $scope.tableParams = new ngTableParams({
                                page: 1,
                                count: 10,
                                sorting: {
                                    name: 'asc'
                                }
                            }, {
                                total: $scope.processes.length,
                                getData: function($defer, params) {
                                    var filteredData = params.filter() ? $filter('filter')($scope.processes, params.filter()) : $scope.processes;
                                    var orderedData = params.sorting() ? $filter('orderBy')(filteredData, params.orderBy()) : $scope.processes;
                                    params.total(orderedData.length);
                                    $defer.resolve(orderedData.slice((params.page() - 1) * params.count(), params.page() * params.count()));
                                }
                            });
                        }
                    } else {
                        toaster.pop('danger', "Load Workflows", "Workflow Processes could not be loaded!");
                        console.error(result);
                    }
                });

                //$http.get(_url(), {
                //  params: {
                //    hostId: $scope.filterHost,
                //    branchId: $scope.filterCompany,
                //    accountNo: $scope.filterAccountNo,
                //    startDate: $scope.filterStartDate,
                //    endDate: $scope.filterEndDate
                //  }
                //}).success(function (resp) {
                //  if ($scope.tableParams) {
                //    $scope.$watch('processes', function () {
                //      $scope.tableParams.reload();
                //    }, true);

                //    $scope.processes = resp.processes;
                //  } else {
                //    $scope.processes = resp.processes;
                //    $scope.loaded = true;
                //    $scope.tableParams = new ngTableParams({
                //      page: 1,
                //      count: 10,
                //      sorting: {
                //        name: 'asc'
                //      }
                //    }, {
                //      total: $scope.processes.length,
                //      getData: function ($defer, params) {
                //        var filteredData = params.filter() ? $filter('filter')($scope.processes, params.filter()) : $scope.processes;
                //        var orderedData = params.sorting() ? $filter('orderBy')(filteredData, params.orderBy()) : $scope.processes;
                //        params.total(orderedData.length);
                //        $defer.resolve(orderedData.slice((params.page() - 1) * params.count(), params.page() * params.count()));
                //      }
                //    });
                //  }
                //}).error(function (e) {
                //  console.error(e);
                //});
            };

            $scope.getCompanys = function() {
                console.log("getting companies");
                _getCompany();
            };

            $scope.getHosts = function(personId) {
                console.log("getting Hosts");
                _getHosts(personId);
            };

            $scope.refresh = function() {
                _get();
            };

            $scope.init = function() {
                _get();
            };
        }
    ]);

    app.controller('WorkflowProcessController', ['$scope', 'ngTableParams', '$http',
        function($scope, ngTableParams, $http) {

            var _get = function() {
                $http.get(_url(), {
                    params: {
                        processJobId: $scope.processJobId
                    }
                }).success(function(resp) {
                    if ($scope.tableParams) {
                        $scope.$watch('processes', function() {
                            $scope.tableParams.reload();
                        }, true);

                        $scope.processes = resp.processes;
                    } else {
                        $scope.processes = resp.processes;
                        $scope.loaded = true;
                        $scope.tableParams = new ngTableParams({
                            page: 1,
                            count: 10,
                            sorting: {
                                name: 'asc'
                            }
                        }, {
                            total: $scope.processes.length,
                            getData: function($defer, params) {
                                var filteredData = params.filter() ? $filter('filter')($scope.processes, params.filter()) : $scope.processes;
                                var orderedData = params.sorting() ? $filter('orderBy')(filteredData, params.orderBy()) : $scope.processes;
                                params.total(orderedData.length);
                                $defer.resolve(orderedData.slice((params.page() - 1) * params.count(), params.page() * params.count()));
                            }
                        });
                    }
                }).error(function(e) {
                    console.error(e);
                });
            };

            $scope.refresh = function() {
                _get();
            };

            $scope.init = function() {
                _get();
            };
        }
    ]);
}(this.angular));