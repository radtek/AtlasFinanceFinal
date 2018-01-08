(function(angular) {
    'use strict';
    var module = angular.module('Falcon.Factories.PayoutResource', ['ngResource', 'ngSanitize']);

    module.value('$', $);

    module.factory('PayoutResource', ['$http',
        function($http) {
            var CONSTANTS = {
                New: [1, 'New', 'label label-info'],
                Cancelled: [2, 'Cancelled', 'label label-warning'],
                OnHold: [3, 'On Hold', 'label label-info'],
                Batched: [4, 'Batched', 'label label-default'],
                Submitted: [5, 'Submitted', 'label label-default'],
                Successful: [6, 'Successful', 'label label-success'],
                Failed: [7, 'Failed', 'label label-danger'],
                Removed: [8, 'Removed', 'label label-warning']
            };

            var _updateStatus = function(payout, payoutStatus) {
                payout.PayoutStatusId = payoutStatus[0];
                payout.PayoutStatus = payoutStatus[1];
                payout.PayoutStatusColor = payoutStatus[2];
                return payout;
            };

            return {
                constants: CONSTANTS,
                getTransactions: function(branchId, startRangeActionDate, endRangeActionDate, payoutId, idNumber, bankId, token) {
                    return $http({
                        method: 'GET',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/Payout/GetTransactions/?branchId=' + branchId + '&startRangeActionDate=' + startRangeActionDate + '&endRangeActionDate=' + endRangeActionDate + '&payoutId=' + payoutId + '&idNumber=' + idNumber + '&bankId=' + bankId)
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                getBanks: function(token) {
                    return $http({
                        method: 'GET',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/Payout/GetBanks/?getBanks=0')
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                holdPayment: function(payout, token) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/Payout/PlaceOnHold/?payoutToHold=' + payout.PayoutId)
                    }).then(function(response) {
                        return _updateStatus(payout, CONSTANTS.OnHold);
                    }, function() {
                        return "Error placing payout on hold!";
                    });
                },
                releasePayment: function(payout, token) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/Payout/RemoveHoldFromPayout/?payoutToRemoveHold=' + payout.PayoutId)
                    }).then(function(response) {
                        return _updateStatus(payout, CONSTANTS.New);
                    }, function() {
                        return "Error removing hold from payout!";
                    });
                },
                getAlerts: function(token) {
                    return $http({
                        method: 'GET',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/Payout/GetAlerts/?getAlerts=0')
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                }
            };
        }
    ]);


}(this.angular));