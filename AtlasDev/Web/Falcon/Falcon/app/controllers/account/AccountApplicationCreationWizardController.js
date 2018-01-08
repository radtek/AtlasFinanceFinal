(function(angular) {
    'use strict';
    var app = angular.module('falcon');

    app.controller('AccountApplicationCreationControllerWizardController', ['$scope', 'toaster',
        function($scope, toaster) {
            $scope.buttonEnabled = true;
            $scope.$on('nextStep', function(event, args) {
                $scope.step += 1;
                $scope.buttonEnabled = true;
            });
            $scope.Loaded = true;
            $scope.steps = ['ClientCheck', 'ClientDetails', 'three'];
            var stepMethods = {
                ClientCheck: 'check',
                two: 'check'
            };
            $scope.step = 0;

            $scope.isCurrentStep = function(step) {
                return $scope.step === step;
            };

            $scope.setCurrentStep = function(step) {
                $scope.step = step;
            };

            $scope.getCurrentStep = function() {
                return $scope.steps[$scope.step];
            };

            $scope.isFirstStep = function() {
                return $scope.step === 0;
            };

            $scope.isLastStep = function() {
                return $scope.step === ($scope.steps.length - 1);
            };

            $scope.getNextLabel = function() {
                return ($scope.isLastStep()) ? 'Submit' : 'Next';
            };

            $scope.handlePrevious = function() {
                $scope.step -= ($scope.isFirstStep()) ? 0 : 1;
                 $scope.buttonEnabled = true;
            };

            $scope.handleNext = function() {
                $scope.buttonEnabled = false;
                if ($scope.isLastStep()) {
                    //dismiss();
                } else {
                    $scope.$broadcast(stepMethods[$scope.steps[$scope.step]], {});
                }
            };
        }

    ]);
}(this.angular));