    'use strict';

    var app = angular.module('falcon.settings', []);

    app.value('xSocketServer', 'ws://localhost:4502/');
    app.value('baseUrl', 'http://localhost:4817');
    app.value('apiBase', 'http://localhost:8182/api/');
    app.value('signalR', 'http://localhost:5000/signalr/hubs');