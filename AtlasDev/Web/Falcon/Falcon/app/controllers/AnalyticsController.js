(function(angular) {
    'use strict';

    var app = angular.module('falcon');

    app.controller('AnalyticsController', ['$scope', 'xSocketsProxy', '$cookieStore', '$cookies',
        function($scope, xSocketsProxy, $cookieStore, $cookies) {
            var ops;

            xSocketsProxy.connection('Analytics');

            xSocketsProxy.publish('stats', {
                Branch: 'unassigned',
                AvsCount: 0,
                SourceRequest: 1
            });
            xSocketsProxy.subscribe("stats").process(function(data) {
                $scope.avs = data.AvsCount;
                console.log('Received response from server with count ' + data.AvsCount + ' for branch ' + data.Branch);
            });
            $scope.transactionLoaded = true;

            $scope.init = function(data) {
                ops = data;
            };
        }
    ]);
}(this.angular));