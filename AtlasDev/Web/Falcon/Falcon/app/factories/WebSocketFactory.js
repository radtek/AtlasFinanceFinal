(function(angular) {
    'use strict';
    var module = angular.module('Falcon.Factories.WebSocketResource', ['ngResource', 'ngSanitize']);

    module.value('$', $);

    module.factory('SocketWrapper', ['$rootScope', 'xSocketServer', '$q',
        function($rootScope, xSocketServer, $q) {

            var Listener = (function() {
                function Listener(fn) {
                    this._a = fn;
                }
                Listener.prototype.process = function(fn) {
                    this._a = fn;
                    return this;
                };

                Listener.prototype.invoke = function(a) {
                    this._a(a);
                    return this;
                };
                return Listener;
            })();

            function Bind(sock, eventName, listeners, fn) {
                if (!listeners.hasOwnProperty(eventName)) {
                    listeners[eventName] = new Listener();
                    sock.on(eventName, function(fn) {
                        $rootScope.$apply(function() {
                            listeners[eventName].invoke(fn);
                        });
                    });
                    return listeners;
                } else {
                    return listeners;
                }
            }

            function Publish(connected, sock, eventName, data, queued) {
                if (connected && typeof(sock) != "undefined") {
                    try {
                        sock.publish(eventName, data);
                    } catch (error) {
                        console.log("caught error: " + error);
                        queued.push({
                            t: eventName,
                            d: data || {}
                        });
                    }
                } else {
                    queued.push({
                        t: eventName,
                        d: data || {}
                    });
                }
                return queued;
            }

            function CreateNewSocket(url) {
                return new XSockets.WebSocket(xSocketServer + url);
            }

            var Connect = function(sock) {
                var def = $q.defer();
                sock.on(XSockets.Events.open, function(conn) {
                    $rootScope.$apply(function() {
                        def.resolve(conn);
                    });
                });
                return def.promise;
            };

            return {
                Listener: Listener,
                Bind: Bind,
                Publish: Publish,
                CreateNewSocket: CreateNewSocket,
                Connect: Connect
            };
        }
    ]);

    module.factory('xSocketsProxy', ['$rootScope', 'SocketWrapper',
        function($rootScope, SocketWrapper) {
            var _connected, _listeners = {},
                _sock, _queued = [];

            function publish(topic, data) {
                _queued = SocketWrapper.Publish(_connected, _sock, topic, data, _queued);
            }

            function subscribe(topic) {
                return bind(topic, {});
            }

            function subscribeToSpecific(topic, data) {
                return bind(topic, data);
            }

            function bind(topic, data) {
                publish(topic, data);
                _listeners = SocketWrapper.Bind(_sock, topic, _listeners);
                return _listeners[topic];
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

            function connectionFP(url, returnPromise) {
                _sock = SocketWrapper.CreateNewSocket(url);

                if (returnPromise)
                    return SocketWrapper.Connect(_sock);
                else
                    connection(url);
            }

            function setProperty(name, newValue) {
                publish('set_' + name, {
                    value: newValue
                });
            }

            return {
                isConnected: _connected,
                connection: connection,
                subscribe: subscribe,
                subscribeToSpecific: subscribeToSpecific,
                publish: publish,
                setProperty: setProperty,
            };
        }
    ]);

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