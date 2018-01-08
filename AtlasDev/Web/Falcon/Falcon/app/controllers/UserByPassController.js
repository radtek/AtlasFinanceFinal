(function(angular) {
    'use strict';

    var app = angular.module('falcon');
    app.controller('UserByPassController', ['$scope', 'UserManagementResource', 'FalconTable', '$rootScope', 'BranchResource', 'toaster', 'ngProgress','UserResource',
        function ($scope, userManagementResource, $ft, $rootScope, branchResource, toaster, ngProgress, userResource) {
            $scope.loaded = false;
            $scope.userId = null;
            $scope.modalMode = 'revoke';
            $scope.searching = false;
            $scope.pageLimit = 10;
            $scope.webUserPageLimit = 10;
            $scope.allocateUser = undefined;
            $scope.filterAuthorisationStartDate = new Date();
            $scope.filterAuthorisationEndDate = new Date();
            $scope.regionalManagerCode = userResource.getUserLegacyClientId();
            $scope.userOperatorCode = $scope.hidLegacyCode;
            //Get Authorization set and expiry default dates;
            var d = new Date();
            var year = d.getFullYear();
            var month = d.getMonth() + 1; 
            if (month < 10) {
                month = "0" + month;
            };
            var day = d.getDate();
            $scope.filterAuthorisationStartDate = year + "-" + month + "-" + day;
            $scope.filterAuthorisationEndDate = year + "-" + month + "-" + (day+1);
            
            $rootScope.$broadcast('menu', {
                name: 'User Management',
                Desc: 'ByPass Users',
                url: '/User/ByPass/List',
                searchVisible: false
            });
            var _load = function() {
                $scope.branchLoading = true;
               // _loadWebUsers();
                branchResource.get().then(function(data) {
                    $scope.branches = data.data;
                    $scope.branchLoading = false;
                    ngProgress.complete();
                }, function(data) {
                    toaster.pop('danger', 'Load Branches', 'An error occurred while trying to load the branches.');
                    $scope.branchLoading = false;
                })
                $scope.loaded = true;
            }

            $scope.search = function() {
                $scope.searching = true;
                userManagementResource.list($scope.branch, $scope.idNo, $scope.firstName, $scope.lastName).then(function (data) {
                    $scope.users = data.data;
                    $scope.searching = false;
                    $scope.processing = false;
                    $scope.branch = '';
                    $scope.reasonCode = '';
                 }, function(data) {
                    toaster.pop('danger', 'Load Users', 'An error occurred while trying to load the users.');
                    $scope.searching = false;
                });
            }
            $scope.bypass = function(user) {
                $scope.allocateUser = user;
               
                $scope.associatedBranchId = user.Branch ? user.Branch.BranchId : undefined;
                $scope.userOperatorCode = user.LegacyClientCode;
                $scope.username = ($scope.allocateUser.Firstname.split(' ').shift() + $scope.allocateUser.Lastname.split(' ').pop()[0]).toLowerCase();
                $scope.email = $scope.username + '@unknown.co.za';
                $scope.password = $scope.confirmPassword = '';

                var _associatedUser = user.WebReference;//findIn($scope.webUsers, 'Id', user.WebReference)
                if (!_associatedUser)
                    $scope.noneLinked = true;
                else
                    $scope.noneLinked = false;
            }

            $scope.init = function() {
                ngProgress.start();

                _load();
            };

            $scope.processAuthorisation = function () {
                $scope.processAuthorisation = true;
            
                var branchCode = 0;
                var reasoncode = "";
                var newOperatorLevel = 5;
                if ($scope.branch != null)
                {
                    branchCode = $scope.branch.LegacyBranchNum;
                }
                else
                {
                    $scope.processAuthorisation = false;
                    toaster.pop('danger', 'Override  Ass User', 'Please choose Branch.');
                }

                if ($scope.reasonCode != null) {
                    reasoncode = $scope.reasonCode;
                }
                else {
                    $scope.processAuthorisation = false;
                    toaster.pop('danger', 'Override  Ass User', 'Please choose Reason.');
                }

                if ($scope.newOperatorLevel != null)
                {
                    newOperatorLevel= $scope.newOperatorLevel
                }

                if ($scope.processAuthorisation == true) {
                    $scope.processing = true;
                userManagementResource.authorizeUserForByPassOnFalcon($scope.filterAuthorisationStartDate, $scope.filterAuthorisationEndDate, $scope.userOperatorCode , branchCode,
                                                                       $scope.regionalManagerCode, newOperatorLevel, reasoncode).then(function (data) {
                                                                           ngProgress.complete();
                                                                           toaster.pop('danger', 'Override  Ass User', 'Success');

                }, function(data) {
                    toaster.pop('danger', 'Override  Ass User', 'An error occurred while trying to overridng the ASS user.');
                    $scope.processAuthorisation = false;
                    })
                }
            }

            var findIn = function (arr, name, value) {
                for (var i = 0, len = arr.length; i < len; i++) {
                    if (name in arr[i] && arr[i][name] == value) return arr[i];
                };
                return false;
            }
        }
    ]);
}(this.angular));