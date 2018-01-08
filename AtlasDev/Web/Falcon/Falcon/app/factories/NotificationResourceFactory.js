(function(angular) {
    'use strict';
    var module = angular.module('Falcon.Factories.NotificationResource', ['ngResource', 'ngSanitize']);

    module.value('$', $);

  module.factory('NotificationService', ['$rootScope', 'SocketWrapper',
        function($rootScope, SocketWrapper) {
            var _connected, _listeners = {},
                _sock, _queued = [],
                _notes = [],
                _noteCount;

            function publish(topic, data) {
                _queued = SocketWrapper.Publish(_connected, _sock, topic, data, _queued);
            }

            function subscribe(topic, data) {
                publish(topic, data);
                _listeners = SocketWrapper.Bind(_sock, topic, _listeners);
                return _listeners[topic];
            }

            var notes = function() {
                try {
                    _notes = JSON.parse(localStorage.getItem('Falcon_Notifications')).Notes || [];
                } catch (e) {
                    _notes = [];
                }
                return _notes;
            };

            var noteCount = function() {
                try {
                    _noteCount = JSON.parse(localStorage.getItem('Falcon_Notifications')).NoteCount;
                } catch (e) {
                    _noteCount = 0;
                }
                return _noteCount;
            };

            function saveNotification(notification) {
                localStorage.setItem('Falcon_Notifications', JSON.stringify(notification));
            }

            function connection(url) {
                _sock = SocketWrapper.CreateNewSocket(url);
                SocketWrapper.Connect(_sock).then(function(ctx) {
                    _connected = true;
                    _queued.forEach(function(msg, i) {
                        publish(msg.t, msg.d);
                    });
                    _queued = [];
                });
            }

            function setProperty(name, newValue) {
                publish('set_' + name, {
                    value: newValue
                });
            }

            return {
                connection: connection,
                subscribe: subscribe,
                setProperty: setProperty,
                notes: notes,
                noteCount: noteCount,
                saveNotification: saveNotification
            };
        }
    ]);

}(this.angular));