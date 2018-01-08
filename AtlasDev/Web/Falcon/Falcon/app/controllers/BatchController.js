(function(angular) {
    'use strict';

    var app = angular.module('falcon');

    app.controller('BatchController', ['$scope', '$filter', 'BatchResource',
        function($scope, $filter, BatchResource) {

            $scope.currentPage = 0;
            $scope.sortingOrder = sortingOrder;
            $scope.reverse = false;
            $scope.transactionList = [];
            $scope.transactionsLoaded = false;

            $scope.options = [{
                name: "20",
                size: 20
            }, {
                name: "50",
                size: 50
            }, {
                name: "Max",
                size: -1
            }];
            $scope.pageSizeItem = $scope.options[0];

            $scope.changePageSize = function() {
                if ($scope.pageSizeItem.name === "Max") {
                    $scope.numPerPage = $scope.transactionList.length;
                    $scope.noOfPages = 1;
                    $scope.currentPage = 0;
                } else {
                    $scope.numPerPage = $scope.pageSizeItem.size;
                    $scope.noOfPages = Math.ceil($scope.transactionList.length / $scope.numPerPage);
                    $scope.currentPage = ($scope.noOfPages - 1);
                }
                $scope.setPage();
            };



            $scope.dataSource = function() {
                BatchResource.getJobs(function(resp) {
                    $scope.transactionList = angular.fromJson(resp.enquiryCollection);
                }, function() {
                    alert('Error fetching Batch Job Results');
                });
            };

            $scope.$watch('transactionList', function() {
                $scope.numPerPage = 20;
                $scope.noOfPages = Math.ceil($scope.transactionList.length / $scope.numPerPage);
                $scope.currentPage !== 0 ? $scope.currentPage : 0;
                $scope.setPage();
                $scope.transactionsLoaded = true;
            });

            $scope.setPage = function() {
                var begin = ($scope.currentPage * $scope.numPerPage),
                    end = begin + $scope.numPerPage;
                $scope.pagedItems = $scope.transactionList.slice(begin, end);
            };

            $scope.sort_by = function(sortOrder) {
                if ($scope.sortingOrder == sortOrder)
                    $scope.reverse = !$scope.reverse;
                $scope.sortingOrder = sortOrder;
            };

            $scope.range = function(start, end) {
                var ret = [];
                if (!end) {
                    end = start;
                    start = 0;
                }
                for (var i = start; i < end; i++) {
                    ret.push(i);
                }
                return ret;
            };

            $scope.nextPage = function() {
                if ($scope.currentPage < $scope.noOfPages - 1) {
                    $scope.currentPage++;
                }
            };

            $scope.prevPage = function() {
                if ($scope.currentPage > 0) {
                    $scope.currentPage--;
                }
            };

            $scope.refresh = function() {
                $scope.dataSource();
            };

            $scope.$watch('currentPage', $scope.setPage);
            $scope.dataSource();
        }
    ]);
})(this.angular);