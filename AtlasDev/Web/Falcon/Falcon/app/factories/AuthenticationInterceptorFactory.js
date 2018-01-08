(function(angular) {
    'use strict';
    var module = angular.module('Falcon.Factories.AuthenticationInterceptor', ['ngResource', 'ngSanitize']);

    module.factory('AuthenticationInterceptor', ['$rootScope', '$q', '$window', '$cookies', '$cookieStore',
        function($rootScope, $q, $window, $cookies, $cookieStore) {
            return {
                request: function(config) {
                    config.headers = config.headers || {};                  
                    if ($cookies['fs_b_0123']) {
                        $window.sessionStorage.token = $cookies['fs_b_0123'];      
                        //$cookieStore.remove("fs_b_0123");                
                    }

                    if ($window.sessionStorage.token) {
                        config.headers.Authorization = 'Bearer ' + $window.sessionStorage.token;
                    }
                    return config;
                },
                response: function(response) {
                    if (response.status === 401) {

                    }
                    return response || $q.when(response);
                }
            };
        }
    ]);
}(this.angular));