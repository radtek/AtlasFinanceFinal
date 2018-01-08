(function(angular) {
    'use strict';
    var module = angular.module('Falcon.Factories.AvsResource', ['ngResource', 'ngSanitize']);

    module.value('$', $);

    module.factory('AvsResource', ['$http', 'apiBase',
        function($http, $apiBase) {
            return {
                getTransactions: function(branchId, startDate, endDate, transactionId, idNumber, bankId, token) {
                    return $http.post($apiBase + 'Avs/Transactions/', {
                        'BranchId': branchId,
                        'StartDate': startDate,
                        'EndDate': endDate,
                        'TransactionId': transactionId,
                        'IdNumber': idNumber,
                        'BankId': bankId
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
                resend: function (transactionId, serviceId, token) {
                    return $http.post($apiBase + 'Avs/Resend/', {
                        'TransactionId': transactionId,
                        'ServiceId': serviceId
                    }, {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function (data, status, headers) {
                        return data;
                    }).error(function (data, status, headers) {
                        return status;
                    })
                },
                updateServiceSettings: function (services, serviceBanks, token) {
                    return $http.post($apiBase + 'Avs/UpdateServiceSettings/', {
                        'ServicesString': services,
                        'ServiceBanksString': serviceBanks
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
                getStaticData: function (token) {
                    return $http.get($apiBase + 'Avs/GetStaticData/', {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function (data, status, headers) {
                        return data;
                    }).error(function (data, status, headers) {
                        return status;
                    })
                },
                getBanks: function(token) {
                    return $http({
                        method: 'GET',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/Avs/GetBanks/?getBanks=0')
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                cancelAVS: function(transactionId, token) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/Avs/Cancel/?cancelTransactionId=' + transactionId)
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                resendAVS: function(transactionId, token) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/Avs/Resend/?resendTransactionId=' + transactionId)
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
                        url: ('/api/Avs/SaveServiceSettings/?servicesString=' + services + '&serviceBanksString=' + serviceBanks)
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
                        url: ('/api/Avs/GetServiceSchedules/?getServiceSchedules=0')
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