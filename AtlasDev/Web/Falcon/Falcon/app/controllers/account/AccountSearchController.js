(function(angular) {
    'use strict';
    var app = angular.module('falcon');

    app.controller('AccountSearchController', ['$scope', 'AccountResource', 'FalconTable', '$rootScope',
        function($scope, AccountResource, FalconTable, $rootScope) {
              $rootScope.$broadcast('menu', {
                name: 'Account - Search',
                url: '/Account/Search/Index/',
                searchVisible: true
            });
            $scope.manage = function(id) {
                window.location.href = '/Account/Detail/View/' + id;
            };

            $scope.search = function() {
                $scope.loaded = false;
                $scope.emptyResult = false;


                AccountResource.search($scope.query, $scope.token).then(function(result) {
                    $scope.emptyResult = result.length === 0 ? true : false;
                    $scope.data = result;

                    if ($scope.searchParams)
                        $scope.searchParams.reload();
                    else
                        $scope.searchParams = FalconTable.new($scope, 'data', 10, [10, 25, 50]);


                    $scope.loaded = true;
                });
            };

            $scope.init = function() {
                $scope.loaded = true;
            };
        }
    ]);
}(this.angular));