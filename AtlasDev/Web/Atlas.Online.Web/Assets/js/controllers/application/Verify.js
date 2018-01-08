(function (angular) {

  var module = angular.module('atlas.application');

  var POLL_INTERVAL = 2000;

  module.directive('atlSvgSpinner', ['$timeout', '$window', function ($timeout, $window) {
    return {
      restrict: 'EAC',
      template: '<div class="verify"><img data-src="{{imgSrc}}" />' +
        '<svg class="verify-spinner" width="166" height="166" viewbox="0 0 166 166">'+
		      '<path class="verify-spinner-inner" transform="translate(83, 83) scale(.84)"/>' +		      
	      '</svg></div>',
      replace: true,
      scope: {
        imgSrc: '@'
      },
      link: function (scope, element, attrs) {
        var frameHnd,
            path = element.find('.verify-spinner-inner'),
            a = 0,
            α = 0,
			      π = Math.PI,
			      t = 30;

        function draw() {
          a += 2;
          if (a == 360) a++;
          α = a % 360;
          a %= 720;
          var r = (α * π / 180),
              x = Math.sin(r) * 83,
              y = Math.cos(r) * -83,
              mid = (α > 180) ? 1 : 0,
              sweep = (a > 360) ? 0 : 1,
              anim;
          
          if (!sweep) {
            mid = +!mid;
          }

          anim = 'M 0 0 v -83 A 83 83 0 ' +
                mid + ' ' + sweep + ' '+
                x + ' ' +
                y + ' z';

          path.attr('d', anim);

          // Redraw
          frameHnd = window.requestAnimationFrame(draw); 
        };

        // Wait for image to load before starting animation
        $timeout(function () {
          var img = element.find('img');
          img.attr('src', scope.imgSrc).load(function () {
            frameHnd = window.requestAnimationFrame(draw);
          });
        });

        scope.$on('$destroy', function () {
          $window.cancelAnimationFrame(frameHnd);
        });
      }
    };
  }]);

  module.controller('VerifyCtrl', ['$scope', '$window', '$apiResource', 'UI', 'Application', 'atlasConfig', '$timeout',
    function ($scope, $window, $apiResource, UI, Application, config, $timeout) {
      UI.loading = true;
      UI.showButtons = false;
      UI.showSteps = false;
      UI.heading = "Processing Application";
      UI.title = false;

      Application.setupScope($scope);

      var AppStatusResource = $apiResource('/ApplicationStep/ProcessingRedirect/:id', { 'id': '@id' });

      $scope.init = function (timeoutRedirect) {

        AppStatusResource.poller({ interval: POLL_INTERVAL, timeout: 60000 * 5 }, { id: $scope.data.ApplicationId }, function (resp, headersFn) {
          UI.clearError();

          var headers = headersFn(),
            location = headers[config.redirectHeader];

          // If the server is setting the location to the same url (but probably set at a different step)
          // refresh so that we can "redirect" to the correct step as setting location to the same url has no effect.
          // Perhaps consider using a flag to indicate this case.
          if (location == $window.location.origin + $window.location.pathname) {
            $window.location.reload();
            return;
          }

          if (location) {
            $window.location = location;
            return true;
          }
        })
        .timeout(function () {
          $window.location = timeoutRedirect;
        })
        .error(function (e, resp) {
          var _showError = function () {
            UI.setError("There has been an error communicating with our servers, but you can safely resume your application later. Click retry to try again.",
              function () { $window.location.reload(); }
            );
          };

          if (resp && resp.status == 0) {
            $timeout(_showError, 5000);
          } else { _showError(); }
        })
        .poll();
      };

    }]);
}(this.angular));