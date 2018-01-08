(function(angular) {
    'use strict';
    var module = angular.module('Falcon.Factories.BureauResource', ['ngResource', 'ngSanitize']);

    module.value('$', $);

    module.factory('BureauResource', ['$http', 'apiBase',
        function($http, $apiBase) {
            return {
                getRecentScore: function(debtorId, branchId, newScore, token) {
                    return $http.post($apiBase + 'Bureau/GetRecentScore/', {
                        'DebtorId': debtorId,
                        'BranchId': branchId,
                        'NewScore': newScore
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
            }
        }
    ]);

}(this.angular));