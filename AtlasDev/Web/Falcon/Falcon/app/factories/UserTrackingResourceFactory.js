(function(angular) {
    'use strict';
    var module = angular.module('Falcon.Factories.UserTrackingResource', ['ngResource', 'ngSanitize']);

    module.value('$', $);

    module.factory('UserTrackingResource', ['$http', 'apiBase',
        function($http, $apiBase) {
            return {
                trackUser: function(startDate, endDate, branchId, userId, token) {
                    return $http.post($apiBase + 'UserTracking/TrackUser/', {
                        'BranchId': branchId,
                        'UserId': userId,
                        'StartDate': startDate,
                        'EndDate': endDate
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
                savePin: function(userId, alertType, severity, elapse, value, notify, token) {
                    return $http.post($apiBase + 'UserTracking/SavePin/', {
                        'UserId': userId,
                        'AlertType': alertType,
                        'Severity': severity,
                        'Elapse': elapse,
                        'Value': value,
                        'Notify': notify
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
                getPinned: function(active, token) {
                    return $http.post($apiBase + 'UserTracking/GetPinned', {
                        'Active': active
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
                removePin: function(pinnedUserId, token) {
                    return $http.post($apiBase + 'UserTracking/RemovePin', {
                        'PinnedUserId': pinnedUserId
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