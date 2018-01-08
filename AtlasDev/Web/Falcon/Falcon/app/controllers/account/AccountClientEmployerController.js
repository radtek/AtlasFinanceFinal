(function(angular) {
    'use strict';
    var app = angular.module('falcon');

    app.controller('AccountClientEmployerController', ['$scope', '$FalconDataProvider', 'FalconTable',
        function($scope, $fdp, FalconTable) {
            var update = function() {
                var data = $fdp.data('AccountData');
                if (data) {
                    $scope.employers = data.Employers;
                    $scope.selectedItem = $scope.employers[0].CompanyId;

                    $scope.$watch('selectedItem', function() {
                        $scope.employer = Enumerable.From($scope.employers).First('$.CompanyId == ' + $scope.selectedItem);
                    });
                }
            };

            $scope.UpdateAddress = function(id) {
                var AddressModal = Enumerable.From($scope.employer.Addresses).First('$.AddressId == ' + id);
                $scope.$emit("dialog:show", {
                    DlgWnd: "UpdateAddressModal",
                    ModalBindingName: 'Address',
                    ModalData: angular.toJson(AddressModal)
                });
            };
            // Setup listener
            $fdp.registerListenerCallback(update);
        }
    ]);
}(this.angular));