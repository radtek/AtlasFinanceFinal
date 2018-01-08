(function (angular) {
    'use strict';

    var app = angular.module('falcon');

    app.controller('TargetLoanMixController', [
        '$rootScope', '$scope', '$filter', 'TargetResource', 'toaster', '$sce',
        function ($rootScope, $scope, $filter, TargetResource, toaster, $sce) {
            $rootScope.$broadcast('menu', {
                name: 'Targets',
                Desc: 'loan mix',
                url: $sce.trustAsUrl('/#!/Target/LoanMix/'),
                searchVisible: true
            });

            $scope.monthNames = [
                { Id: 1, Month: 'January' },
                { Id: 2, Month: 'February' },
                { Id: 3, Month: 'March' },
                { Id: 4, Month: 'April' },
                { Id: 5, Month: 'May' },
                { Id: 6, Month: 'June' },
                { Id: 7, Month: 'July' },
                { Id: 8, Month: 'August' },
                { Id: 9, Month: 'September' },
                { Id: 10, Month: 'October' },
                { Id: 11, Month: 'November' },
                { Id: 12, Month: 'December' }
            ];

            $scope.years = [];
            for (var i = -3; i < 3; i++) {
                var year = 1900 + new Date().getYear() - i;
                $scope.years.push(year);
            }

            $scope.filterMonth = $scope.monthNames[new Date().getMonth()].Id;
            $scope.filterYear = $scope.years[3];

            $scope.init = function () {
                $scope.getTargets();
            };

            $scope.getTargets = function () {
                TargetResource.getLoanMix($scope.filterMonth, $scope.filterYear).then(function (response) {
                    if (response.status === 200) {
                        $scope.targets = response.data;
                    } else {
                        toaster.pop('danger', "Loading Target", "Error loading Loan Mix!");
                        console.error(result);
                    }
                });
            };

            $scope.targetChanged = function (target) {
                target.isDirty = true;
                $scope.enableSave = true;
            };

            $scope.saveTargets = function () {
                $scope.enableSave = false;

                for (var i = 0; i < $scope.targets.length; i++) {
                    if ($scope.targets[i].isDirty) {
                        TargetResource.saveLoanMix($scope.targets[i]).then(function (response) {
                            if (response.status !== 200) {
                                toaster.pop('danger', "Saving Target", "Error Saving Loan Mix Target!");
                                console.error(result);
                                $scope.enableSave = true;
                            }
                        });
                    }
                }
            };
        }
    ]);
}(this.angular));