(function(angular) {
    'use strict';
    var app = angular.module('falcon');

    app.controller('AccountDetailController', ['$scope', 'AccountResource', 'FalconTable', 'AccountDetailStatusColour', '$FalconDataProvider','$rootScope',
        function($scope, AccountResource, FalconTable, AccountDetailStatusColour, $fdp, $rootScope) {
            $rootScope.$broadcast('menu', {
                name: 'Account - Search',
                url: '/Account/Detail/Index/',
                searchVisible: true
            });

            var update = function() {
                var data = $fdp.data('AccountData');
                if (data) {
                    $scope.account = data;
                    $scope.statement = data.Statement;

                    if ($scope.statementParams)
                        $scope.statementParams.reload();
                    else
                        $scope.statementParams = FalconTable.new($scope, 'statement', 10, [10]);
                }
            };

            $scope.accountStatus = function(status) {
                return AccountDetailStatusColour[status];
            };

            // Setup listener
            $fdp.registerListenerCallback(update);

            $scope.getCombined = function() {
                var width = 200;
                var height = 150;
                return parseInt(width, 10) + parseInt(height, 10);
            };

            $scope.progressDeliquency = function(isDanger) {
                if ($scope.account) {
                    var width = 0;
                    if (isDanger)
                        width = $scope.account.DelinquencyPercentage;
                    else
                        width = 100 - $scope.account.DelinquencyPercentage;
                    return {
                        width: width + '%',
                    };
                }
            };

            $scope.getDelinquency = function() {
                if ($scope.account) {
                    var description = $scope.account.Delinquency + ' Month';
                    if ($scope.account.Delinquency > 1)
                        description += 's';
                    return description;
                }
            };
        }
    ]);
}(this.angular));