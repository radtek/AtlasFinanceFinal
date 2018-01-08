(function(angular) {
    'use strict';
    var module = angular.module('Falcon.Factories.NaedoResource', ['ngResource', 'ngSanitize']);

    module.value('$', $);

    module.factory('NaedoResource', ['$http', 'apiBase',
        function($http, $apiBase) {
            return {
                getBatches: function(branchId, batchId, batchStatus, startRange, endRange, token) {
                    return $http.post($apiBase + 'debitorder/batch/', {
                        'BatchId': batchId,
                        'BranchId': branchId,
                        'BatchStatus': batchStatus,
                        'StartRange': startRange,
                        'EndRange': endRange,
                        'QueryBatchOnly': true
                    }, {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function(data, status, headers) {
                        return data;
                    }).error(function(data, status, headers) {
                        return status;
                    })
                },
                getTransaction: function(batchId, token) {
                    return $http.post($apiBase + 'debitorder/batch/', {
                        'BatchId': batchId,
                        'QueryBatchOnly': false
                    }, {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function(data, status, headers) {
                        return data;
                    }).error(function(data, status, headers) {
                        return status;
                    })
                },
                getControl: function(controlId, token) {
                    return $http.post($apiBase + 'debitorder/control/', {
                        'ControlId': controlId
                    }, {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function(data, status, headers) {
                        return data;
                    }).error(function(data, status, headers) {
                        return status;
                    })
                },
                addAdditionalDebit: function(controlId, amount, actionDate, token) {
                    return $http.post($apiBase + 'debitorder/additionaldebitorder/', {
                        'ControlId': controlId,
                        'Instalment': amount,
                        'ActionDate': actionDate
                    }, {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function(data, status, headers) {
                        return data;
                    }).error(function(data, status, headers) {
                        return status;
                    })
                },
                canAdditionalDebit: function(controlId, transactionId, token) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/Naedo/CancelAdditionalDebitOrder/?controlId=' + controlId + '&transactionId=' + transactionId)
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                saveServiceSchedule: function(services, serviceBanks, token) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/Naedo/SaveServiceSettings/?servicesString=' + services + '&serviceBanksString=' + serviceBanks)
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                getServiceSchedule: function(token) {
                    return $http({
                        method: 'GET',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/Naedo/GetServiceSchedules/?getServiceSchedules=0')
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                getControls: function(host, branchId, startDate, endDate, controlOnly, token) {
                    return $http.post($apiBase + 'debitorder/controls/', {
                        'Host': host,
                        'BranchId': branchId,
                        'StartRange': startDate,
                        'EndRange': endDate,
                        'ControlOnly': controlOnly
                    }, {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function(data, status, headers) {
                        return data;
                    }).error(function(data, status, headers) {
                        return status;
                    })
                }
            };
        }
    ]);
}(this.angular));