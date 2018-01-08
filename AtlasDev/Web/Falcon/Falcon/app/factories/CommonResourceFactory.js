(function(angular) {
    'use strict';
    var module = angular.module('Falcon.Factories.CommonResource', ['ngResource', 'ngSanitize']);

    module.value('$', $);

    module.factory('BranchResource', ['$http', 'apiBase',
        function($http, $apiBase) {
            return {
                get: function(token) {
                    return $http.post($apiBase + 'Branch/Get/', {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function(data, status, headers) {
                        return data;
                    }).error(function(data, status, headers) {
                        return status;
                    })
                },
                associateUser: function(branchId, personId, token) {
                    return $http.post($apiBase + 'Branch/AssociateUser/', {
                        'BranchId': branchId,
                        'PersonId': personId
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
            }
        }
    ]);
}(this.angular));