(function(angular) {
    'use strict';
    var app = angular.module('falcon');

    app.controller('AccountClientRelationController', ['$scope', '$FalconDataProvider', '$FalconChangeSelectionProvider', 'FalconTable', 'RelationResource', 'toaster',
        function($scope, $fdp, $fcsp, FalconTable, RelationResource, toaster) {

            $scope.changeSelection = function(targetRow, sourceRow, dataSource) {
                $fcsp.changeSelection(targetRow, sourceRow, dataSource);

                $scope[targetRow] = $fcsp.getSelectedItem(dataSource);
            };

            var update = function() {
                var data = $fdp.data('AccountData');
                if (data) {
                    $scope.relations = data.Relations;

                    $fcsp.registerDataSource('relations', $scope.relations);

                    if ($scope.relationParams)
                        $scope.relationParams.reload();
                    else
                        $scope.relationParams = FalconTable.new($scope, 'relations', 10, [10]);
                }
            };
            // Setup listener
            $fdp.registerListenerCallback(update);

            $scope.getRelationTypes = function() {
                RelationResource.getTypes($scope.token).then(function(resp) {
                    if (resp.status === 200) {
                        $scope.relationTypes = resp.data.relationTypes;
                    } else {
                        toaster.pop('danger', "Loading Relation Types", "There was a problem loading Relation Types!");
                        console.error(resp);
                    }
                });
            };

            $scope.addRelation = function() {
                RelationResource.add($scope.account.PersonId, $scope.personId, $scope.newRelation.FirstName, $scope.newRelation.LastName, $scope.newRelation.CellNo, $scope.newRelation.RelationTypeId, $scope.token).then(function(result) {
                    if (result.status === 200) {
                        $scope.dismiss();
                        toaster.pop('success', "New Relation", "New Relation has been added!");
                        $fdp.promptUpdateRegistars();
                    } else {
                        toaster.pop('danger', "New Relation", "There was a problem adding new Relation!");
                        console.log(result);
                    }
                });
            };

            $scope.updateRelation = function() {
                RelationResource.update($scope.account.PersonId, $scope.newRelationPersonId, $scope.newRelationFirstName, $scope.newRelationLastName, $scope.newRelationCellNo, $scope.newRelationRelationTypeId, $scope.token).then(function(result) {
                    if (result.status === 200) {
                        $scope.dismiss();
                        toaster.pop('success', "Update Relation", "Relation has been updated!");
                        $fdp.promptUpdateRegistars();
                    } else {
                        toaster.pop('danger', "Update Relation", "There was a problem updating Relation!");
                        console.log(result);
                    }
                });
            };

            $scope.updateRelationDialogue = function(selectedRelation) {
                if (selectedRelation === undefined) {
                    $scope.newRelationPersonId = 0;
                    $scope.newRelationFirstName = "";
                    $scope.newRelationLastName = "";
                    $scope.newRelationCellNo = "";
                    $scope.newRelationRelationTypeId = 0;
                } else {
                    $scope.newRelationPersonId = selectedRelation.PersonId;
                    $scope.newRelationFirstName = selectedRelation.FirstName;
                    $scope.newRelationLastName = selectedRelation.LastName;
                    $scope.newRelationCellNo = selectedRelation.CellNo;
                    $scope.newRelationRelationTypeId = selectedRelation.RelationTypeId;
                }
            };

        }
    ]);
}(this.angular));