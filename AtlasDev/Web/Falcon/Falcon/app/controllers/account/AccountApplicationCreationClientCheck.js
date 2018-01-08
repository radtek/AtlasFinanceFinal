(function(angular) {
    'use strict';
    var app = angular.module('falcon');

    app.controller('AccountApplicationCreationClientCheck', ['$scope', '$rootScope', 'ApplicationResource',
        function($scope, $rootScope, $ar) {
            // Mandatory in order to wire up navigation -- WIP maybe move this to a baseController instance.
            $rootScope.$broadcast('menu',{ name: 'Account - Application Creation', url: '/account/application/create' , searchVisible: true});

            $scope.$on('check', function(event, args) {
                $scope.infoBox = true;
                $scope.message = "Searching..."
                $ar.checkClient($scope.idNo, $scope.token).then(function(data) {
                    $scope.infoBox = false;
                    $scope.$emit('nextStep');
                });
                
            });
        }
    ]);
}(this.angular));