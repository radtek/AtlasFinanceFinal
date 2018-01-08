(function(angular) {

    var app = angular.module('falcon');

    app.controller('FingerPrintController', ['$scope', 'xSocketsProxy', 'GuidResource', '$http', 'toaster', '$timeout',
        function($scope, $socky, $gr, $http, $toast, $ts) {
            $scope.FingerPrint = 'Idle.';
            $scope.trackingGuid = '';
            $scope.isConnected = false;
            $scope.fpData = null;
            $scope.fingerColour = '003366';
            document.getElementById('something');
            var _tracerId = null;
            var _isAuthorized = false;

            $scope.init = function() {
                $socky.connectionFP("FingerPrint", true).then(function(ctx) {
                    _tracerId = ctx.ClientGuid;
                    $scope.trackingGuid = _tracerId;
                    $socky.subscribe('finger_check').process(function(result) {
                        var resp = result.response;

                        if (resp.HasError)
                            $toast.pop('danger', resp.ErrorMessage);
                        else {
                            $scope.FingerPrint = resp.Authenticated ? 'Authorized' : 'Not Authorized';
                            if (resp.Authenticated) {
                                _isAuthorized = true;
                                path2830.style.fill = '#266A2E';
                                // TODO REDIRECTION PROTECTION LOGIC
                            } else {
                                path2830.style.fill = '#FF0000'
                            }

                        }
                    });
                    $scope.isConnected = true;
                });
            };


            $scope.scanFinger = function() {
                if ($scope.isConnected) {
                    path2830.style.fill = '#003366';
                    $scope.FingerPrint = 'Validating...';
                    _isAuthorized = false;

                    delete $http.defaults.headers.common['X-Requested-With'];

                    $ts(function() {
                        $http({
                            method: 'POST',
                            headers: {
                                'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8'
                            },
                            params: {
                                "Password": "123",
                                "Hash": "12312312312",
                                "TrackingId": _tracerId
                            },
                            url: ('http://127.0.0.1:3579/')
                        }).success(function(data, status, headers, config) {
                            if (!_isAuthorized)
                                $scope.FingerPrint = data.Status;
                        }).error(function(data, status, headers, config) {
                            $toast.pop('danger', "Unable to connect to finger print authorization server!");
                            $scope.FingerPrint = 'Error';
                            path2830.style.fill = '#FF0000'
                        });
                    }, 4000);
                };
            }
        }
    ]);
}(this.angular));