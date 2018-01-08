(function(window, angular) {
    'use strict';

    var config = {
        apiBase: 'http://localhost:4817/api/',
        redirectHeader: 'x-client-redirect',
    };

    var app = angular.module('falcon', ['ngRoute', 'ngResource', 'ngCookies', 'ngTable',
        'falcon.settings', 'Falcon.Factories', 'falcon.filters', 'falcon.constants', 'falcon.services',
        'falcon.directives', 'Falcon.Factories.ReportResource', 'Falcon.Factories.AccountResource', 'Falcon.Factories.UserResource', 'ngAnimate',
        'Falcon.Factories.AvsResource', 'Falcon.Factories.TableResource', 'Falcon.Factories.NaedoResource',
        'Falcon.Factories.NotificationResource', 'Falcon.Factories.PayoutResource', 'Falcon.Factories.WebSocketResource',
        'Falcon.Factories.AuthenticationInterceptor', 'ngProgress', 'isteven-multi-select', 'toaster', 'remoteValidation', 'angularMoment',
        'Falcon.Factories.CampaignResource', 'Falcon.Factories.CommonResource', 'Falcon.Factories.UserTrackingResource','Falcon.Factories.BureauResource',
        'ngTagsInput', 'Falcon.Factories.StreamResource', 'Falcon.Factories.TargetResource', 'SignalR', 'ng-context-menu',
    ]);



    app.constant('falconConfig', config)
        .config(['$httpProvider', '$routeProvider', '$locationProvider',
            function($httpProvider, $routeProvider, $locationProvider) {
                // Allow backend to detect XHR requests
                $httpProvider.defaults.headers.common['X-Requested-With'] = 'XMLHttpRequest';
                //$httpProvider.interceptors.push('AuthenticationInterceptor');
                $routeProvider
                    .when('/home', {
                        templateUrl: '/home/index'
                    })
                    .when('/Avs/Transactions/', {
                        templateUrl: '/Avs/Transactions/'
                    })
                    .when('/Naedo/Batch', {
                        templateUrl: '/Naedo/Batch/'
                    }).
                    when('/Naedo/Control/', {
                        templateUrl: '/Naedo/Control/'
                    })
                    .when('/Naedo/Batch/Transactions/:transactionId', {
                        templateUrl: '/Naedo/Batch/Transactions'
                    })
                    .when('/Naedo/Batch/Control/:controlId/:transactionId', {
                        templateUrl: '/Naedo/Batch/Control'
                    })
                    .when('/Reports/CIReport', {
                        templateUrl: '/Reports/CIReport/'
                    })
                    .when('/Reports/Stream', {
                        templateUrl: '/Reports/Stream'
                    })
                    .when('/UserTracking/Tracking', {
                        templateUrl: '/UserTracking/Tracking'
                    }).
                    when('/UserTracking/Pinned', {
                        templateUrl: '/UserTracking/Pinned'
                    })
                    .when('/Campaign/Sms/Manager/', {
                        templateUrl: '/Campaign/Sms/Manager/'
                    })
                    .when('/Stream/Collections/', {
                        templateUrl: '/Stream/Collections/'
                    })
                    .when('/Stream/Sales/', {
                        templateUrl: '/Stream/Sales/'
                    })
                    .when('/Stream/Manage/', {
                        templateUrl: '/Stream/Manage/'
                    }).when('/User/Management', {
                        templateUrl: '/User/Management/Index'
                    }).when('/User/ByPass/', {
                        templateUrl: '/User/ByPass/Index'
                    }).when('/Target/MonthlyCi/', {
                        templateUrl: '/Target/MonthlyCi/Index'
                    }).when('/Target/DailySales/', {
                        templateUrl: '/Target/DailySales/Index'
                    }).when('/Target/Handover/', {
                        templateUrl: '/Target/Handover/Index'
                    }).when('/Target/LoanMix/', {
                        templateUrl: '/Target/LoanMix/Index'
                    });
                $locationProvider.html5Mode(false).hashPrefix('!');
            }

        ]);

    app.run(["$rootScope", "ngProgress", "$templateCache",
        function($rootScope, ngProgress, $templateCache) {
            $rootScope.$on('$routeChangeStart', function(event, next, current) {
                /*if (typeof(current) !== 'undefined') {
                $templateCache.remove(current.templateUrl);
            }*/
                ngProgress.start();
            });
            $rootScope.$on('$routeChangeSuccess', function() {
                ngProgress.complete();
            });
        }
    ]);

}(this, this.angular));