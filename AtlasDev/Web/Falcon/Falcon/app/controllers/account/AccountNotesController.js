(function(angular) {
    'use strict';
    var app = angular.module('falcon');

    app.controller('AccountNotesController', ['$scope', 'AccountResource', '$FalconDataProvider', 'xSocketsProxy', '$timeout',
        function($scope, AccountResource, $fdp, xSocketsProxy, $timeout) {
            $scope.notes = [];
            $scope.glued = true;

            var _handleNoteAdd = function(note) {

                $scope.notes.push(note);
            };

            var update = function() {
                var data = $fdp.data('AccountData');
                if (data) {
                    xSocketsProxy.connection("AccountDetail");
                    xSocketsProxy.setProperty('accountId', data.AccountId);
                    xSocketsProxy.subscribe('update_data').process(function(result) {
                        if (result.updateType === 'note') {

                            _handleNoteAdd(result.data);

                            $timeout(function() {
                                $('#chats li:last-child').effect("highlight", {
                                    color: /*'#C3FDB8'*/ '#FFFFC2'
                                }, 2000);
                            }, 0);
                        }
                    });

                    // Setup notes
                    for (var i = data.Notes.length - 1; i >= 0; i--) {
                        _handleNoteAdd(data.Notes[i]);
                    }
                }
            };

            $scope.addNote = function() {
                AccountResource.addNote($scope.accountId, $scope.personId, $scope.token, $scope.newNote);
                $scope.newNote = '';

            };

            // Setup listener
            $fdp.registerListenerCallback(update);
        }
    ]);
}(this.angular));