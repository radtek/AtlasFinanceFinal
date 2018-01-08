(function(angular) {
    'use strict';
    var app = angular.module('falcon');

    app.controller('AccountEnquiriesCreditController', ['$scope', 'FalconTable', '$FalconDataProvider', '$FalconChangeSelectionProvider', 'CreditResource', 'toaster',
        function($scope, FalconTable, $fdp, $fcsp, CreditResource, toaster) {

            $scope.changeSelection = function(targetRow, sourceRow, dataSource) {
                $fcsp.changeSelection(targetRow, sourceRow, dataSource);
                $scope.row = $fcsp.getSelectedItem('creditEnquiries');
            };

            var update = function() {
                var data = $fdp.data('AccountData');
                if (data) {
                    $scope.creditEnquiries = data.CreditEnquiries;

                    $fcsp.registerDataSource('creditEnquiries', $scope.creditEnquiries);

                    // Setup params for tables
                    if ($scope.creditParams)
                        $scope.creditParams.reload();
                    else
                        $scope.creditParams = FalconTable.new($scope, 'creditEnquiries', 10, [10]);
                }
            };

            $scope.reSubmit = function() {
                $scope.DlgWnd = 1;
                CreditResource.reSubmit($scope.row.EnquiryId, $scope.token).then(function(response) {
                    if (response.status === 200) {

                    } else if (response.status === 500) {
                        $scope.status = response.data.result;
                    }
                    $scope.dismiss();
                });
            };

            $scope.report = function() {
                $scope.DlgWnd = 2;
                CreditResource.report($scope.row.EnquiryId, $scope.token).then(function(response) {
                    if (response.status === 200) {

                    } else if (response.status === 500) {

                    }
                });
            };

            $scope.performEnquiry = function() {
                $scope.DlgWnd = 3;

                setTimeout(function() {
                    $scope.dismiss();
                    toaster.pop('success', "Enquiry", "New enquiry available!");
                    $fdp.promptUpdateRegistars();
                }, 5000);
            };
            // Setup listener
            $fdp.registerListenerCallback(update);
        }
    ]);
}(this.angular));