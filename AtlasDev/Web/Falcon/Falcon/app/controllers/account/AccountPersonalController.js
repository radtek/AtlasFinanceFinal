(function(angular) {
    'use strict';
    var app = angular.module('falcon');

    app.controller('AccountPersonalController', ['$scope', '$FalconDataProvider',
        function($scope, $fdp) {
            var update = function() {
                $scope.account = $fdp.data('AccountData');
            };

            var dialog_show = $scope.$on('dialog:show', function(event, transport) {
                $scope.DlgWnd = transport.DlgWnd;
                $scope[transport.ModalBindingName] = angular.fromJson(transport.ModalData);

            });

            $scope.$on('$destroy', dialog_show);
            // Setup listener
            $fdp.registerListenerCallback(update);
        }
    ]);
}(this.angular));