(function(angular) {
    'use strict';
    var app = angular.module('falcon');

    app.controller('AccountClientPersonalController', ['$scope', '$FalconDataProvider', '$FalconChangeSelectionProvider', 'FalconTable', 'AddressResource', 'ContactResource', 'toaster', 'UniversalResource',
        function($scope, $fdp, $fcsp, FalconTable, AddressResource, ContactResource, toaster, UniversalResource) {

            $scope.changeSelection = function(targetRow, sourceRow, dataSource) {
                $fcsp.changeSelection(targetRow, sourceRow, dataSource);

                $scope[targetRow] = $fcsp.getSelectedItem(dataSource);
            };

            var update = function() {
                var data = $fdp.data('AccountData');
                if (data) {
                    $scope.account = data;
                    $scope.accountAddressTypes = [];
                    $scope.contacts = $scope.account.Contacts;
                    for (var i = 0; i < $scope.account.Addresses.length; i++) {
                        var addressTypeCount = 0;
                        for (var j = $scope.accountAddressTypes.length - 1; j >= 0; j--) {
                            if ($scope.accountAddressTypes[j].AddressTypeId == $scope.account.Addresses[i].AddressTypeId) {
                                addressTypeCount = $scope.accountAddressTypes[j].AddressTypeCount;
                                break;
                            }
                        }
                        addressTypeCount++;
                        var accountAddressType = {
                            AddressId: $scope.account.Addresses[i].AddressId,
                            AddressTypeId: $scope.account.Addresses[i].AddressTypeId,
                            AddressType: $scope.account.Addresses[i].AddressType + ((addressTypeCount === 1) ? '' : ' ' + addressTypeCount),
                            AddressTypeCount: addressTypeCount,
                            IsActive: $scope.account.Addresses[i].IsActive
                        };
                        $scope.accountAddressTypes.push(accountAddressType);

                        if ($scope.filterAddressType === undefined && $scope.account.Addresses[i].IsActive)
                            $scope.filterAddressType = accountAddressType.AddressId;
                    }
                    $scope.accountAddressSelectionChanged();

                    $fcsp.registerDataSource('contacts', $scope.contacts);
                    if ($scope.contactParams)
                        $scope.contactParams.reload();
                    else
                        $scope.contactParams = FalconTable.new($scope, 'contacts', 10, [10]);
                }
            };

            $scope.accountAddressSelectionChanged = function() {
                for (var i = 0; i < $scope.account.Addresses.length; i++) {
                    if ($scope.account.Addresses[i].AddressId === $scope.filterAddressType) {
                        $scope.selectedAccountAddress = $scope.account.Addresses[i];
                        break;
                    }
                }
            };

            // Setup listener
            $fdp.registerListenerCallback(update);

            $scope.getAddressTypes = function() {
                AddressResource.getTypes($scope.token).then(function(resp) {
                    if (resp.status === 200) {
                        $scope.addressTypes = resp.data.addressTypes;
                    } else {
                        toaster.pop('danger', "Loading Address Types", "There was a problem loading Address Types!");
                        console.error(resp);
                    }
                });
            };

            $scope.getContactTypes = function() {
                ContactResource.getTypes($scope.token).then(function(resp) {
                    if (resp.status === 200) {
                        $scope.contactTypes = resp.data.contactTypes;
                    } else {
                        toaster.pop('danger', "Loading Contact Types", "There was a problem loading Contact Types!");
                        console.error(resp);
                    }
                });
            };

            $scope.getProvinces = function() {
                UniversalResource.getProvinces($scope.token).then(function(resp) {
                    if (resp.status === 200) {
                        $scope.provinces = resp.data.provinces;
                    } else {
                        toaster.pop('danger', "Loading Provinces", "There was a problem loading Provinces!");
                        console.error(resp);
                    }
                });
            };

            $scope.addAddress = function() {
                AddressResource.add($scope.account.PersonId, $scope.personId, $scope.newAddressTypeId, $scope.newAddressProvinceId, $scope.newAddressLine1,
                    $scope.newAddressLine2, $scope.newAddressLine3, $scope.newAddressLine4, $scope.newAddressCode, $scope.token).then(function(result) {
                    if (result.status === 200) {
                        $scope.dismiss();
                        toaster.pop('success', "New Address", "New Address has been added!");
                        $fdp.promptUpdateRegistars();
                    } else {
                        toaster.pop('danger', "New Address", "There was a problem adding new Address!");
                        console.log(result);
                    }
                });
            };

            $scope.deleteAddress = function() {
                AddressResource.disable($scope.account.PersonId, $scope.selectedAccountAddress.AddressId, $scope.token).then(function(result) {
                    if (result.status === 200) {
                        $scope.dismiss();
                        toaster.pop('success', "Delete Address", "Address has been deleted!");
                        $fdp.promptUpdateRegistars();
                    } else {
                        toaster.pop('danger', "Delete Address", "There was a problem deleting Address!");
                        console.log(result);
                    }
                });
            };

            $scope.addContact = function() {
                ContactResource.add($scope.account.PersonId, $scope.newContactTypeId, $scope.newContactValue, $scope.token).then(function(result) {
                    if (result.status === 200) {
                        $scope.dismiss();
                        toaster.pop('success', "New Contact", "New Contact has been added!");
                        $fdp.promptUpdateRegistars();
                    } else {
                        toaster.pop('danger', "New Contact", "There was a problem adding new Contact!");
                        console.log(result);
                    }
                });
            };

            $scope.deleteContact = function() {
                ContactResource.disable($scope.account.PersonId, $scope.coRowSelected.ContactId, $scope.token).then(function(result) {
                    if (result.status === 200) {
                        $scope.dismiss();
                        toaster.pop('success', "Delete Contact", "Contact has been deleted!");
                        $fdp.promptUpdateRegistars();
                    } else {
                        toaster.pop('danger', "Delete Contact", "There was a problem deleting Contact!");
                        console.log(result);
                    }
                });
            };
        }
    ]);
}(this.angular));