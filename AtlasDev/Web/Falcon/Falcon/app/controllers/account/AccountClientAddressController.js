(function(angular) {
    'use strict';
    var app = angular.module('falcon');

    app.controller('AccountClientAddressController', ['$scope', '$FalconDataProvider', 'AddressResource', 'toaster',
        function($scope, $fdp, AddressResource, toaster) {
            var update = function() {
                var data = $fdp.data('AccountData');
                if (data) {
                    $scope.account = data;
                    $scope.addresses = $scope.account.Addresses;
                    $scope.accountAddressTypes = [];
                    for (var i = 0; i < $scope.addresses.length; i++) {
                        var addressTypeCount = 0;
                        for (var j = $scope.accountAddressTypes.length - 1; j >= 0; j--) {
                            if ($scope.accountAddressTypes[j].AddressTypeId == $scope.addresses[i].AddressTypeId) {
                                addressTypeCount = $scope.accountAddressTypes[j].AddressTypeCount;
                                break;
                            }
                        }
                        addressTypeCount++;
                        var accountAddressType = {
                            AddressId: $scope.addresses[i].AddressId,
                            AddressTypeId: $scope.addresses[i].AddressTypeId,
                            AddressType: $scope.addresses[i].AddressType + ((addressTypeCount === 1) ? '' : ' ' + addressTypeCount),
                            AddressTypeCount: addressTypeCount,
                            IsActive: $scope.addresses[i].IsActive
                        };
                        $scope.accountAddressTypes.push(accountAddressType);

                        if ($scope.filterAddressType === undefined && $scope.addresses[i].IsActive)
                            $scope.filterAddressType = accountAddressType.AddressId;
                    }
                    $scope.selectionChanged();
                }
            };

            $scope.selectionChanged = function() {
                for (var i = 0; i < $scope.addresses.length; i++) {
                    if ($scope.addresses[i].AddressId === $scope.filterAddressType)
                        $scope.selectedAccountAddress = $scope.addresses[i];
                }
            };

            // Setup listener
            $fdp.registerListenerCallback(update);

            $scope.getAddressTypes = function() {
                AddressResource.getTypes($scope.token).then(function(resp) {
                    if (resp.status === 200) {
                        $scope.addressTypes = resp.data.addressTypes;
                    } else {
                        toaster.pop('danger', "Loading Address Types", "There was a problem loading Types!");
                        console.error(resp);
                    }
                });
            };
        }
    ]);
}(this.angular));