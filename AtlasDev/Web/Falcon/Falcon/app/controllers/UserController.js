(function(angular) {
    'use strict';
    var app = angular.module('falcon');

    app.controller('UserController', ['$scope',
        function($scope) {

            $scope.updateProfile = function() {
                alert('Profile Updated');
            };

        }
    ]);
}(this.angular));