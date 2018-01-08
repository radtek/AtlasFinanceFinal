(function(angular) {
    'use strict';
    var app = angular.module('falcon');

    app.controller('AccountRouteHistoryController', ['$scope', '$FalconDataProvider',
        function($scope, $fdp) {

            var update = function() {
                $scope.account = $fdp.data('AccountData');
            };

            $scope.image = null;
            $scope.imageFileName = '';

            // Setup listener
            $fdp.registerListenerCallback(update);
        }
    ]);
}(this.angular));