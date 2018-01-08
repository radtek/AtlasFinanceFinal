(function(angular) {
    'use strict';
    var app = angular.module('falcon');

    app.controller('AccountClientHistoryController', ['$scope', '$FalconDataProvider', 'FalconTable',
        function($scope, $fdp, FalconTable) {
            var update = function() {
                var data = $fdp.data('AccountData');
                if (data) {
                    $scope.accountHistory = data.AccountHistory;

                    if ($scope.relationParams)
                        $scope.historyParams.reload();
                    else
                        $scope.historyParams = FalconTable.new($scope, 'accountHistory', 10, [10]);
                }
            };
            // Setup listener
            $fdp.registerListenerCallback(update);
        }
    ]);
}(this.angular));