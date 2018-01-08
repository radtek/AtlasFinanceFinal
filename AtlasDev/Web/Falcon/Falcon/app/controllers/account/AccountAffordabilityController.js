(function(angular) {
    'use strict';
    var app = angular.module('falcon');

    app.controller('AccountAffordabilityController', ['$scope', 'FalconTable', '$FalconDataProvider', '$FalconChangeSelectionProvider', 'AffordabilityResource', 'toaster',
        function($scope, FalconTable, $fdp, $fcsp, AffordabilityResource, toaster) {
            $scope.affordabilityAmount = "R 0.00";

            $scope.changeSelection = function(targetRow, sourceRow, dataSource) {
                $fcsp.changeSelection(targetRow, sourceRow, dataSource);

                $scope[targetRow] = $fcsp.getSelectedItem(dataSource);
            };

            var _getCategories = function() {
                AffordabilityResource.getCategories($scope.token).then(function(resp) {
                    if (resp.status === 200) {
                        $scope.affordabilityCategories = resp.data.categories;
                    } else {
                        toaster.pop('danger', "Loading Categories", "There was a problem loading Categories!");
                        console.error(resp);
                    }
                });
            };

            var update = function() {
                var data = $fdp.data('AccountData');
                if (data) {
                    $scope.account = data;
                    $scope.affordabilityOptions = $scope.account.AffordabilityOptions;
                    $scope.affordabilityItems = $scope.account.AffordabilityItems;
                    $fcsp.registerDataSource('affordabilityOptions', $scope.affordabilityOptions);
                    $fcsp.registerDataSource('affordabilityItems', $scope.affordabilityItems);
                    _getCategories();

                    $scope.affordabilityCategoryTypes = [];
                    $scope.affordabilityCategoryTypes.push({
                        TypeId: 1,
                        Type: "Expense"
                    });
                    $scope.affordabilityCategoryTypes.push({
                        TypeId: 2,
                        Type: "Income"
                    });

                    if ($scope.affordabilityOptionParams)
                        $scope.affordabilityOptionParams.reload();
                    else
                        $scope.affordabilityOptionParams = FalconTable.new($scope, 'affordabilityOptions', 10, [10]);

                    if ($scope.affordabilityParams)
                        $scope.affordabilityParams.reload();
                    else
                        $scope.affordabilityParams = FalconTable.new($scope, 'affordabilityItems', 10, [10]);
                }
            };
            // Setup listener
            $fdp.registerListenerCallback(update);

            $scope.UpdateCategoryByType = function() {
                $scope.affordabilityCategoriesByType = [];

                for (var i = 0; i < $scope.affordabilityCategories.length; i++) {
                    if ($scope.affordabilityCategories[i].TypeId === $scope.affordabilityCategoryType)
                        $scope.affordabilityCategoriesByType.push({
                            CategoryId: $scope.affordabilityCategories[i].AffordabilityCategoryId,
                            Description: $scope.affordabilityCategories[i].Description
                        });
                }
            };

            $scope.addAffordabilityItem = function() {
                var amount = $scope.affordabilityAmount.replace("R", "");
                $scope.afShowAmountValidation = (amount <= 0);
                $scope.afShowCategoryValidation = !$scope.selectedAffordabilityCategory;
                if ($scope.afShowAmountValidation === true || $scope.afShowCategoryValidation === true)
                    return;

                AffordabilityResource.addItem($scope.account.AccountId, $scope.selectedAffordabilityCategory, amount, $scope.personId, $scope.token).then(function(result) {
                    if (result.status === 200) {
                        $scope.dismiss();
                        toaster.pop('success', "New Affordability Item", "New Affordability Item has been added!");
                        $fdp.promptUpdateRegistars();
                    } else {
                        toaster.pop('danger', "New Affordability Item", result);
                    }
                });
            };

            $scope.deleteAffordabilityItem = function(affordabilityId) {
                AffordabilityResource.deleteItem($scope.account.AccountId, affordabilityId, $scope.personId, $scope.token).then(function(result) {
                    if (result.status === 200) {
                        $scope.dismiss();
                        toaster.pop('success', "Delete Affordability Item", "Transaction has been cancelled!");
                        $scope.afRowSelected.DeleteDate = result.data.affordabilityItem.DeleteDate;
                        $scope.afRowSelected.DeleteUser = result.data.affordabilityItem.DeleteUser;
                    } else {
                        toaster.pop('danger', "Delete Affordability Item", result);
                    }
                });
            };

            $scope.canAddDeleteItem = function() {
                if ($scope.affordabilityOptions) {
                    for (var i = 0; i < $scope.affordabilityOptions.length; i++) {
                        if ($scope.affordabilityOptions[i].AffordabilityOptionStatusId == 3 || $scope.affordabilityOptions[i].AffordabilityOptionStatusId == 4 || $scope.affordabilityOptions[i].AffordabilityOptionStatusId == 5)
                            return false;
                    }
                }
                return true;
            };

            $scope.acceptAffordabilityOption = function() {
                AffordabilityResource.acceptOption($scope.account.AccountId, $scope.afOptionRowSelected.AffordabilityOptionId, $scope.token).then(function(result) {
                    if (result.status === 200) {
                        $scope.dismiss();
                        toaster.pop('success', "Accept Affordability Option", "Option has been accepted");
                        $fdp.promptUpdateRegistars();
                    } else {
                        toaster.pop('danger', "Accept Affordability Option", "An error has occured while trying to accept the option");
                    }
                });
            };
        }
    ]);
}(this.angular));