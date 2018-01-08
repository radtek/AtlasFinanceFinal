(function (window, angular) {
  'use strict'

  // Atlas module
  var module = angular.module('atlas.core', [
    'ui.date',    
    'fixate.directives',
    'atlas.factories',
    'atlas.ui',
    'atlas.config'
  ]);

  module.controller('GenericFormCtrl', ['$scope', function ($scope) {
    $scope.submit = function (form, event) {
      form.$submitted = true;
      if (form.$invalid) {
        event.preventDefault();
        return; 
      }
    };
  }]);

}(this, this.angular));