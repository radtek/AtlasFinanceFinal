(function(angular) {
    'use strict';
    var module = angular.module('falcon.services', ['ngResource']);

    module.service('$FalconDataProvider', ['$log',
        function($log) {

            var _obsCb = [];
            var _updCb = [];

            var _registerObserverCallback = function(cb) {
                _obsCb.push(cb);
            };

            var _notifyObservers = function() {
                angular.forEach(_obsCb, function(cb) {
                    cb();
                });
            };

            this.setData = function(section, data) {
                this[section] = data;

                _notifyObservers();
            };

            this.promptUpdateRegistars = function() {
                angular.forEach(_updCb, function(cb) {
                    cb();
                });
            };

            this.registerListenerCallback = function(cb) {
                if (cb === undefined)
                    $log.warn('No specified callback for observer.');

                if (cb)
                    _registerObserverCallback(cb);

                _notifyObservers();
            };

            this.registerUpdateCallback = function(cb) {
                _updCb.push(cb);
            };

            this.data = function(section) {
                return this[section];
            };
        }
    ]);

    module.service('$FalconChangeSelectionProvider', ['$log',
        function($log) {

            var _ds = {};

            this.registerDataSource = function(dataDescription, dataSource) {
                _ds[dataDescription] = dataSource;
            };

            this.getDataSourceItem = function(dataSection, item) {
                if (_ds[dataSection].length > 0)
                    return _ds[dataSection][item];
            };

            this.getSelectedItem = function(dataDescription) {
                if (!_ds[dataDescription])
                    return;
                else {
                    for (var i = _ds[dataDescription].length - 1; i >= 0; i--) {
                        if (_ds[dataDescription][i].$selected)
                            return _ds[dataDescription][i];
                    }
                }
            };

            this.getDataSourceCollection = function(dataDescription) {
                if (!_ds[dataDescription])
                    return;
                else
                    return _ds[dataDescription];
            };

            this.changeSelection = function(targetRow, sourceRow, dataSource) {

                if (!Object[targetRow]) {
                    sourceRow.$selected = true;
                } else {
                    sourceRow.$selected = !sourceRow.$selected;
                }

                Object[targetRow] = sourceRow;

                for (var i = _ds[dataSource].length - 1; i >= 0; i--) {
                    if (_ds[dataSource][i].$selected && _ds[dataSource][i] != sourceRow) {
                        _ds[dataSource][i].$selected = false;
                    }
                }
            };
        }
    ]);

}(this.angular));