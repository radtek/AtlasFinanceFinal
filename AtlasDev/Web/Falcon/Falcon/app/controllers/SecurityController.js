(function(angular) {
    'use strict';

    var app = angular.module('falcon');

    app.controller('LoginController', ['$scope', '$http',
        function($scope, $http) {
            $scope.loginMessage = '';

            var pattern = /invalid/g;
            if (pattern.test(document.URL)) {
                $scope.loginError = true;
                $scope.loginMessage = 'Invalid Username or Password';
            }
        }
    ]);

    app.controller('AssociateController', ['$scope', '$http',
        function($scope, $http) {
            $scope.associateMessage = '';
            
        }
    ]);

    app.controller('ForgotController', ['$scope', 'baseUrl', 'ForgotPasswordResource',
        function($scope, baseUrl, $fpr) {
            $scope.forgot = true;

            $scope.submitForm = function() {
                $scope.submitted = true;
                $scope.is_processing = true;

                var dt = {
                    Username: $scope.forgetForm.Username.$viewValue,
                    IdNo: $scope.forgetForm.IDNo.$viewValue,
                    CellNo: $scope.forgetForm.CellNo.$viewValue
                };

                $fpr.verify(dt, $scope.token).success(function(data, status) {
                    window.location.href = '/User/Account/OTP/' + data._;
                    $scope.is_processing = false;
                }).error(function(status) {
                    if (status === 430)
                        $scope.resetFault = true;
                    $scope.is_processing = false;
                });

            }

            $scope.init = function() {

            }

            $scope.back = function() {
                window.location.href = baseUrl;
            }
        }
    ]);

    app.controller('OTPController', ['$scope', 'OTPResource',
        function($scope, OTPResource) {
            $scope.otpError = false;
            $scope.verifying = false;

            var hashSegment = URLHelper.getLast(document.URL)

            $scope.resend = function(userId) {
                $scope.is_processing = true;
                var url = '/api/account/?user=' + userId;
                $http.post(url).success(function(data, status, headers, config) {
                    if (status === 200) {
                        $scope.resent = true;
                    }
                }).error(function(data, status, headers, config) {
                    $scope.error = true;
                    if (status === 400) {
                        $scope.errorMsg = 'Unable to verify OTP ';
                    }

                    $scope.submitted = false;
                    $scope.loading = false;
                });
                $scope.is_processing = false;
            };

            $scope.submitForm = function() {
                if ($scope.otpForm.$valid) {
                    $scope.verifying = true;

                    OTPResource.verify(hashSegment, $scope.otpForm.OTP.$viewValue, $scope.token).success(function(data) {
                        $scope.verifying = false;
                        if (data.isValid)
                            window.location.href = '/User/Account/Password/' + data.Hash;
                    }).error(function(data) {
                        $scope.otpError = true;
                        $scope.error = data.Message;

                        $scope.verifying = false;
                    });

                }
            };
        }
    ]);

    app.controller('PasswordResetController', ['$scope', 'ResetPasswordResource', 'baseUrl',
        function($scope, ResetPasswordResource, baseUrl) {
            $scope.resetError = false;

            var hashSegment = URLHelper.getLast(document.URL)

            $scope.submitForm = function() {
                if ($scope.setPassword.$valid) {
                    $scope.resetting = true;

                    ResetPasswordResource.reset(hashSegment, $scope.setPassword.Password.$viewValue, $scope.token).success(function(data) {
                        $scope.resetting = false;
                        if (data.isValid)
                            window.location.href = baseUrl;
                    }).error(function(data) {
                        $scope.resetError = true;
                        $scope.error = data.Message;

                        $scope.resetting = false;
                    });

                }
            };
        }
    ]);
})(this.angular);