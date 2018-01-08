(function(angular) {
	'use strict';

	var app = angular.module('falcon');

	app.controller('LogoutController', ['$scope', '$http',
		function($scope, $http) {
			$scope.logOut = function() {
				var url = '/api/account/';
				$http.post(url).success(function(data, status, headers, config) {
					if (status === 200) {
						window.location.href = '/';
					}
				});
			}
		}
	]);
})(this.angular);