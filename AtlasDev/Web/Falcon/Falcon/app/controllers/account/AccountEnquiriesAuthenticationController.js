(function(angular) {
    'use strict';
    var app = angular.module('falcon');

    app.controller('AccountEnquiriesAuthenticationController', ['$scope', 'AccountResource', 'FalconTable', '$FalconDataProvider', '$FalconChangeSelectionProvider', 'toaster',
        function($scope, AccountResource, FalconTable, $fdp, $fcsp, toaster) {

            $scope.changeSelection = function(targetRow, sourceRow, dataSource) {
                $fcsp.changeSelection(targetRow, sourceRow, dataSource);
                $scope.row = $fcsp.getSelectedItem('authentication');
            };

            var update = function() {
                var data = $fdp.data('AccountData');
                if (data) {
                    $scope.authentication = data.Authentication;

                    // Register data source to grid selection provider.
                    $fcsp.registerDataSource('authentication', $scope.authentication);

                    // Setup params for tables
                    if ($scope.xdsParams)
                        $scope.xdsParams.reload();
                    else
                        $scope.xdsParams = FalconTable.new($scope, 'authentication', 10, [10]);
                }
            };

            $scope.resetAuthenticationAttempts = function(a) {
                $scope.authenticationResetAttempts = a;
            };

            $scope.authentcationDlgFn = function(oId, fn) {
                $scope.is_processing = true;
                AccountResource.authenticationFn($scope.row.AuthenticationId, $scope.authentication.OverrideReason, $scope.token, oId, fn).then(
                    function(response) {
                        if (response.data.Fn === 'override' && response.status === 200)
                            toaster.pop('warning', "Authentication Override", "Authentication result has been overriden.", 3000);
                        else if (response.data.Fn === 'reset' && response.status === 200)
                            toaster.pop('warning', "Authentication Reset", "Authentication result has been reset.", 3000);

                        $fdp.promptUpdateRegistars();
                        $scope.authentication.OverrideReason = null;
                        $scope.is_processing = false;
                        $scope.dismiss();
                    },
                    function(error) {
                        if (response.data.Fn === 'override')
                            toaster.pop('error', "Authentication Override", error, 3000);
                        else
                            toaster.pop('error', "Authentication Reset", error, 3000);
                    });
            };

            // Setup listener
            $fdp.registerListenerCallback(update);

        }
    ]);
}(this.angular));