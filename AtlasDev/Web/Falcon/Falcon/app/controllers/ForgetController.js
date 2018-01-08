(function(angular) {
    'use strict';

    var app = angular.module('falcon');

    app.controller('ForgotController', ['$scope',
        function($scope) {
            $scope.forgot = true;

            $scope.submitForm = function() {
                if ($scope.forgetForm.$valid) {
                    alert('moo');
                };
            }
        }
    ])
})(this.angular);