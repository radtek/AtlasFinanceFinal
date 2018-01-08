(function(angular) {
	'use strict';

	
	var app = angular.module('falcon');

	app.controller('CreditController', ['$scope', 'CreditResource',
		function($scope, CreditResource) {
			//$scope.sortingOrder = sortingOrder;
			$scope.currentPage = 0;
			$scope.reverse = false;
			$scope.transactionLoaded = false;
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

			$scope.transactionList = [];

			$scope.dataSource = function() {
				CreditResource.getTransactions(function(resp) {
					$scope.transactionList = angular.fromJson(resp.enquiryCollection);
					console.info('original count :' + $scope.transactionList.length);
				}, function() {
					alert('Error fetching credit enquiries');
				});
			};

			$scope.$watch('transactionList', function() {
				$scope.numPerPage = 20;
				$scope.noOfPages = Math.ceil($scope.transactionList.length / $scope.numPerPage);
				$scope.currentPage != 0 ? $scope.currentPage : 0;
				$scope.setPage();
				$scope.transactionLoaded = true;
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

			$scope.style = function(thing) {
				return {
					"background": "red"
				};
			}

			$scope.$watch('currentPage', $scope.setPage);
			$scope.dataSource();
		}
	]);
})(this.angular);