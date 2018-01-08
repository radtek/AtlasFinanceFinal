(function (window, angular) {
  'use strict'

  var config = {
      apiBase: '/api',
    viewPath: '/Assets/js/views',

    redirectHeader: 'x-client-redirect',
  };

  var module = angular.module('atlas.config', []);

  module.constant('atlasConfig', config)
    .config(['$httpProvider', function ($httpProvider) {
      // Allow backend to detect XHR requests
      $httpProvider.defaults.headers.common['X-Requested-With'] = 'XMLHttpRequest';      
    }])
    // Prevent scrolling for anchors
  .value('$anchorScroll', angular.noop);

  // Angular Helpers
  angular.isBoolean = function (val) { return val === true || val === false; };

}(this, this.angular));