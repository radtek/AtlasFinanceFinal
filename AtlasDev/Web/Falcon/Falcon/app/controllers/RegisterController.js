(function(angular) {
    'use strict';

    var app = angular.module('falcon');

    app.controller('RegisterController', ['$scope', '$http', '$interval', '$timeout',

        function($scope, $http, $interval, $timeout) {
            $scope.isRegistered = false;
            $scope.errorDisplay = false;
            $scope.progTotal = 100;
            $scope.currentTotal = 0;
            $scope.register = function() {
                if ($scope.registerForm.$valid) {
                    $scope.loading = true;
                    var url = '/api/account/?username=' + $scope.username + '&password=' + $scope.password + '&cellNo=' + $scope.cellNo + '&idNo=' + $scope.idNoOrPassportNo;
                    $http.post(url).success(function(data, status, headers, config) {
                        if (status === 200) {
                            url = '/api/account/?username=' + $scope.username + '&password=' + $scope.password + '&remember=' + '&returnUrl=';
                            $http.post(url).success(function(data, status, headers, config) {
                                if (status === 200) {
                                    $scope.loading = false;
                                    window.location.href = '/User/Account/OTP';
                                }
                            }).error(function(data, status, headers, config) {
                                errorDisplay(status);
                            });
                        }
                    }).error(function(data, status, headers, config) {
                        errorDisplay(status);
                    });
                }
            };

            var errorDisplay = function(status) {
                $scope.loading = false;
                switch (status) {
                    case 400: // unable to locate user
                        $scope.errorDisplay = true;
                        $scope.errorDisplayDescription = 'Unable to continue, identity number was not found on core.';
                        break;
                    case 403: // Forbidden
                        $scope.errorDisplay = true;
                        $scope.errorDisplayDescription = 'A request attempted was invalid.';
                        break;
                    case 405: // Throttling
                        $scope.errorDisplay = true;
                        $scope.errorDisplayDescription = 'Abuse detected. Requsts are being rejected.';
                        break;

                }
            };

            $scope.back = function() {
                window.location.href = '/User/Account/Login';
            };
        }
    ]);
})(this.angular);