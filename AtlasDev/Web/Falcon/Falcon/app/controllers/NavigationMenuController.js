(function(angular) {
    'use strict';
    var app = angular.module('falcon');

    app.controller('NavigationMenuController', ['$scope',
        function($scope) {
            $scope.$on('menu', function(event, args) {
                $scope.activeMenu = args.name;
                $scope.activeMenuDesc = args.Desc;
                $scope.url = args.ur;
                $scope.searchVisible = args.searchVisible;
            });
        }
    ]);
}(this.angular));