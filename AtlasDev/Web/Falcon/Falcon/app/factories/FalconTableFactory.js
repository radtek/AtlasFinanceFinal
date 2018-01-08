(function(angular) {
    'use strict';
    var module = angular.module('Falcon.Factories.TableResource', ['ngResource', 'ngSanitize']);

    module.value('$', $);

    module.factory('FalconTable', ['ngTableParams', '$filter',
        function(ngTableParams, $filter) {
            return {
                new: function($scope, obj, pageLength, pageCount) {
                    /*jshint -W055 */
                    return new ngTableParams({
                        page: 1,
                        count: pageLength,
                        sorting: {
                            name: 'asc'
                        }
                    }, {
                        total: $scope[obj].length, // length of data
                        getData: function($defer, params) {

                            var filteredData = params.filter() ? $filter('filter')($scope[obj], params.filter()) : $scope[obj];
                            var orderedData = params.sorting() ? $filter('orderBy')(filteredData, params.orderBy()) : $scope[obj];
                            params.total(orderedData.length);
                            $defer.resolve(orderedData.slice((params.page() - 1) * params.count(), params.page() * params.count()));
                        }
                    });
                }
            };
        }
    ]);
}(this.angular));