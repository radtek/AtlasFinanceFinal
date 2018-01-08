(function(angular) {
    'use strict';
    var module = angular.module('Falcon.Factories.CampaignResource', ['ngResource', 'ngSanitize']);

    module.value('$', $);

    module.factory('CampaignResource', ['$http', 'apiBase',
        function($http, $apiBase) {
            return {
                getSmsList: function(notificationId, startDate, endDate) {
                    return $http.post($apiBase + 'Campaign/GetSms/', {
                        'NotificationId': notificationId,
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
                getSms: function(personId, contactTypeId, value, token) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        url: ("/api/loan/ContactAdd/?personId=" + personId + "&contactTypeId=" + contactTypeId + "&value=" + value)
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                resendSms: function(notificationId) {
                    return $http.post($apiBase + 'Campaign/Resend/', {
                        'NotificationId': notificationId
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
                getEmail: function(personId, contactId, token) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        url: ("/api/loan/ContactDisable/?personId=" + personId + "&contactId=" + contactId)
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                getEmailList: function(personId, contactId, token) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        url: ("/api/loan/ContactDisable/?personId=" + personId + "&contactId=" + contactId)
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