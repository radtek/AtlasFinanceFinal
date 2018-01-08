(function(angular) {
    'use strict';
    var app = angular.module('falcon');

    app.controller('AccountApplicationCreationClientDetails', ['$scope', 'ApplicationResource',
        function($scope, $ar) {

            $scope.$on('check', function(event, args) {
                alert(event + $scope.idNo);
                $ar.checkClient($scope.idNo, $scope.token).then(function(data) {
                    alert(data);
                });
                $scope.$emit('nextStep');
            });
        }
    ]);
}(this.angular));