(function(angular) {
    'use strict';
    var app = angular.module('falcon');

    app.controller('UserTrackingController', ['$scope', '$rootScope', '$sce', 'FalconTable', '$filter', 'ngProgress', 'BranchResource', 'UserResource', 'UserTrackingResource', 'toaster',
        function($scope, $rootScope, $sce, FalconTable, $filter, ngProgress, BranchResource, UserResource, UserTrackingResource, toaster) {
            $rootScope.$broadcast('menu', {
                name: 'User Tracking',
                Desc: 'pin users',
                url: '/User/Management/List',
                searchVisible: false
            });
            $scope.contactValues = [];

            $scope.filterBranch = null;
            $scope.filterUser = null;
            var date = $filter('date')(new Date(), 'yyyy-MM-dd');
            $scope.filterCompany = 0;
            $scope.filterStartDate = $scope.filterStartDate || date
            $scope.filterEndDate = $scope.filterEndDate || date;
            $scope.isSearching = false;
            $scope.filterElapse = null;
            $scope.filterSeverity = null;
            $scope.filterAlertType = null;
            $scope.elapseValue = null;
            $scope.selectStateUser = "Loading...";
            $scope.selectStateBranch = "Loading...";

            $scope.$watch('filterAlertType', function() {
                if ($scope.filterAlertType === 1)
                    $scope.alertTypeMessage = 'Contact No(s)';
                else
                    $scope.alertTypeMessage = 'Email Addresses';
            });

            var _getAlertTypes = function() {
                $scope.alertTypes = [{
                    name: "Email",
                    Id: 0
                }, {
                    name: "SMS",
                    Id: 1
                }];
                $scope.filterAlertType = $scope.alertTypes[0].Id;
            }

            var _getSeverities = function() {
                $scope.severity = [{
                    name: "Minor",
                    Id: 0
                }, {
                    name: "Low",
                    Id: 1
                }, {
                    name: "Medium",
                    Id: 2
                }, {
                    name: "High",
                    Id: 3
                }, {
                    name: "Critical",
                    Id: 4
                }];
                $scope.filterSeverity = $scope.severity[0].Id;
            }

            var _getElapses = function() {
                $scope.elapses = [{
                    name: "Minutes",
                    Id: 0
                }, {
                    name: "Hours",
                    Id: 1
                }, {
                    name: "Days",
                    Id: 2
                }];
                $scope.filterElapse = $scope.elapses[0].Id;
            }


            var _getBranches = function() {
                BranchResource.get($scope.token).then(function(data) {
                        $scope.branches = data.data;
                        $scope.selectStateBranch = "Make Selection...";
                        _getUsers();
                    },
                    function(status) {
                        toaster.pop('danger', "Branch Load", "There was a problem attempting to load the user list!");
                    });
            }

            var _getUsers = function() {
                UserResource.getUsers($scope.token).then(function(data) {
                    $scope.users = data.data;
                    ngProgress.complete();
                    $scope.selectStateUser = "Make Selection...";
                }, function(status) {
                    toaster.pop('danger', "User Load", "There was a problem attempting to load the user list!");

                });
            }

            var _getUserTracking = function() {
                UserTrackingResource.trackUser($scope.filterStartDate, $scope.filterEndDate, $scope.filterBranch, $scope.filterUser, $scope.token).then(function(data) {

                        $scope.tracingData = data.data;
                        if ($scope.userParams) {
                            $scope.userParams.reload();
                        } else {
                            $scope.userParams = FalconTable.new($scope, 'tracingData', 10, [10]);
                        }
                        ngProgress.complete();$scope.isSearching = false;
                    },
                    function(status) {
                        toaster.pop('danger', "Error Tracing", "There was a problem while attempting to load trace data!");
                    });
            }

            $scope.apply = function() {
            	$scope.isSearching = true;
            	ngProgress.start();
                _getUserTracking();
            }

            $scope.init = function() {
                ngProgress.start();
                _getBranches();
                _getAlertTypes();
                _getSeverities();
                _getElapses();

            }

            $scope.pin = function(trace) {
                $scope.trace = trace;
            };

            $scope.savepin = function(trace) {
                var _arr = [];
                for (var i = $scope.contactValues.length - 1; i >= 0; i--) {
                    _arr.push($scope.contactValues[i].text);
                };
                UserTrackingResource.savePin(trace.UserId, $scope.filterAlertType, $scope.filterSeverity, $scope.filterElapse, $scope.elapseValue, _arr.join(";"), $scope.token).then(function(data) {
                        toaster.pop('success', "Pin Saved", "Your tracking pin has been saved!");
                        $scope.dismiss();
                    },
                    function(status) {
                        toaster.pop('danger', 'Error Saving Pin', 'An error has occurred while trying to save the pin!');
                    });
                o
            }
        }
    ]);

    app.controller('UserTrackingPinnedController', ['$scope', '$rootScope', '$sce', 'FalconTable', '$filter', 'ngProgress', 'BranchResource', 'UserResource', 'UserTrackingResource', 'toaster',
        function($scope, $rootScope, $sce, FalconTable, $filter, ngProgress, BranchResource, UserResource, UserTrackingResource, toaster) {
            $rootScope.$broadcast('menu', {
                name: 'User Tracking',
                Desc: 'pinned users',
                url: '',
                searchVisible: false
            });
            $scope.pin = null;

            var _getPinnedUsers = function() {
                UserTrackingResource.getPinned(true, $scope.token).then(function(data) {
                    $scope.pinnedUsers = data.data;
                    if ($scope.userParams) {
                        $scope.userParams.reload();
                    } else {
                        $scope.userParams = FalconTable.new($scope, 'pinnedUsers', 10, [10]);
                    }

                }, function(status) {
                    toaster.pop('danger', 'Error Loading Pins', 'An error occurred while attempting to load pinned users!');
                })
            }

            $scope.setPin = function(pin)
            {
            	$scope.pin = pin;
            }

            $scope.removePin = function(pin) {
                UserTrackingResource.removePin(pin.PinnedUserId, $scope.token).then(function(data) {
                    toaster.pop('success', 'Pin Removed', 'The pin has been removed!');
                    $scope.dismiss();
                    _getPinnedUsers();
                }, function(status) {
                    toaster.pop('danger', 'Error Removing Pin', 'An error occurred trying to remove the pin!');
                })
            };

            $scope.init = function() {
                _getPinnedUsers();
            };
        }
    ]);
}(this.angular));