(function(angular) {
    'use strict';
    var app = angular.module('falcon');

    app.controller('AccountWorkflowController', ['$scope', 'FalconTable', '$FalconDataProvider', '$FalconChangeSelectionProvider', 'WorkflowResource', 'toaster',
        function($scope, FalconTable, $fdp, $fcsp, WorkflowResource, toaster) {

            $scope.changeSelection = function(targetRow, sourceRow, dataSource) {
                $fcsp.changeSelection(targetRow, sourceRow, dataSource);
                $scope.wfRowSelected = $fcsp.getSelectedItem(dataSource);
            };

            var update = function() {
                var data = $fdp.data('AccountData');
                if (data) {
                    $scope.workflows = data.Workflow;

                    $fcsp.registerDataSource('workflows', $scope.workflows);
                    // Setup params for tables
                    if ($scope.workflowParams)
                        $scope.workflowParams.reload();
                    else
                        $scope.workflowParams = FalconTable.new($scope, 'workflows', 10, [10]);
                }
            };

            $scope.redirectWorkflow = function() {
                $scope.dismiss();
                WorkflowResource.redirectProcess($scope.wfRowSelected.ProcessStepJobAccountId, $scope.personId).then(function(result) {
                    if (result.status === 200) {
                        toaster.pop('success', "Redirect Process Step", "Step has been redirected!");
                        $fdp.promptUpdateRegistars();
                    } else {
                        toaster.pop('danger', "Redirect Process Step", "Step cannot be redirected!");
                    }
                });
            };

            // Setup listener
            $fdp.registerListenerCallback(update);
        }
    ]);
}(this.angular));