(function(angular) {
    'use strict';
    var app = angular.module('falcon');

    app.controller('ParentController', ['$scope', 'Hub', 'signalR', '$timeout', 'toaster',
        function($scope, $hub, $signalR, $ts, toaster) {
            var _id = undefined;
            var _fn = undefined;
            var _subscriptions = [];

            var hub = new $hub('general', {
                listeners: {
                    'logout': function(msg) {
                        toaster.pop('info', 'Alert', msg);
                        $ts(function() {
                            window.location.href = '/User/Account/LogOut';
                        }, 5000);

                    },
                    'is_busy': function(channel, userId) {

                        var _subscriptionChannelBusyListener = channel + "IsBusyListener";
                        var _subscriptionChannelBusyConsumer = channel + "IsBusyConsumer";

                        if (angular.indexOf(_subscriptionChannelBusy) == -1)
                            _subscriptions.push(_subscriptionChannelBusy);

                        $scope.$on(_subscriptionChannelBusy, function(event, args) {
                            hub.BusyEvent(channel, userId);
                        });
                    },
                    'notify': function(channel, broadcastSubscriber, data) {
                        $scope.$broadcast(broadcastSubscriber, {
                            data: data
                        });
                    },
                },
                methods: ['subscribeChannel', 'unsubscribeChannel'],
                queryParams: {},
                rootPath: $signalR,

                errorHandler: function(error) {
                    console.error(error);
                }
            });

            // attempt to restablish connection on failure.
            hub.connection.disconnected(function() {
                $ts(function() {
                    toaster.pop('info', 'Connnection Lost', 'Attempting to reconnect to server...');
                    _start();
                }, 5000);
            });

            var _start = function() {
                hub.connection.start().done(function() {
                    _id = hub.connection.id;
                    if (_fn) _fn();
                })
            }
            $scope.init = function() {
                _start();
            };

            $scope.$on('subscribeChannel', function(event, args) {
                _fn = function() {
                    hub.subscribeChannel(args.userId, _id, args.channel)
                };

                if (_id)
                    _fn();
                else
                    _start();
            });

            $scope.$on('unsubscribeChannel', function(event, args) {
                _fn = function() {
                    hub.unsubscribeChannel(args.userId, _id, args.channel)
                };

                if (_id)
                    _fn();
                else
                    _start();
            });
        }
    ]);
}(this.angular));