(function(angular) {
  'use strict';
  var app = angular.module('falcon');

  app.controller('ETLController', ['$scope', 'HttpFactory', 'xSocketsProxy', 'ngTableParams', '$filter', 'StageColour',
    function($scope, HttpFactory, xSocketsProxy, ngTableParams, $filter, StageColour) {
      var _url = function(id, batchOnly) {
        return '/api/ETL/' + id + '?b=' + batchOnly;
      };


      var _get = function(id, batchOnly) {
        HttpFactory.get(_url(id, batchOnly)).success(function(resp) {
          if (resp.eltDebitOrderBatch.length > 0) {
            $scope.Branch = resp.eltDebitOrderBatch[0].Branch;
            $scope.Stage = resp.eltDebitOrderBatch[0].Stage;
            $scope.CreateDate = resp.eltDebitOrderBatch[0].CreateDate;
            $scope.LastStageDate = resp.eltDebitOrderBatch[0].LastStageDate;
            $scope.StageColour = _stageColor(resp.eltDebitOrderBatch[0].Stage);

            var data = resp.eltDebitOrderBatch[0].DebitOrders;

            for (var i = data.length - 1; i >= 0; i--) {
              data[i]["StageColour"] = StageColour[data[i]["Stage"]];
            };

            $scope.loaded = true;

            $scope.tableParams = new ngTableParams({
              page: 1,
              count: 10,
              sorting: {
                name: 'asc'
              }
            }, {
              total: data.length, // length of data
              getData: function($defer, params) {

                var filteredData = params.filter() ? $filter('filter')(data, params.filter()) : data;
                var orderedData = params.sorting() ? $filter('orderBy')(filteredData, params.orderBy()) : data;
                params.total(orderedData.length);
                $defer.resolve(orderedData.slice((params.page() - 1) * params.count(), params.page() * params.count()));
              }
            });
          } else {
            $scope.id = id;
            $scope.recordNotFound = true;
          }
        }).error(function(data, status, headers, config) {
          alert(status);
        });
      };

      $scope.init = function(id, batchOnly) {
        $scope.loaded = false;
        _get(id, batchOnly);
      };
    }
  ]);

  app.controller('ETLStageController', ['$scope', 'HttpFactory', 'xSocketsProxy', 'ngTableParams', '$filter','toaster','StageColour',
    function($scope, HttpFactory, xSocketsProxy, ngTableParams, $filter ,toaster,StageColour) {
      $scope.hasTransactions = false;
      var _url = function(id) {
        return '/api/ETL/' + id;
      };

      var _get = function(id) {
        HttpFactory.get(_url(id)).success(function(resp) {
          if (resp.eltDebitOrderBatch.length > 0) {

            var data = resp.eltDebitOrderBatch;

            for (var i = data.length - 1; i >= 0; i--) {
              data[i]["StageColour"] = StageColour[data[i]["Stage"]];
            };

            $scope.loaded = true;
            $scope.hasTransactions = true;

            $scope.tableParams = new ngTableParams({
              page: 1,
              count: 10,
              sorting: {
                name: 'asc'
              }
            }, {
              total: data.length, // length of data
              getData: function($defer, params) {

                var filteredData = params.filter() ? $filter('filter')(data, params.filter()) : data;
                var orderedData = params.sorting() ? $filter('orderBy')(filteredData, params.orderBy()) : data;
                params.total(orderedData.length);
                $defer.resolve(orderedData.slice((params.page() - 1) * params.count(), params.page() * params.count()));
              }
            });
          } else {
            $scope.id = id;
          }
        }).error(function(data, status, headers, config) {
          if (status === 500) {
            toaster.pop('error', "Service", 'Error communicating with backend services.', 10000, 'trustedHtml');
          }
        });
      };

      $scope.manage = function(id, batchOnly) {
        window.location = "/Naedo/Staging/View/" + id + "?b=" + batchOnly;
      }

      $scope.init = function(id) {
        $scope.loaded = false;
        _get(id);
      };
    }
  ]);

  app.controller('ETLStageUploadController', ['$scope', '$filter', '$fileUploader', '$http', 'toaster',
    function($scope, $filter, $fileUploader, $http, toaster) {

      // create a uploader with options
      var completeCount = 0;

      var uploader = $scope.uploader = $fileUploader.create({
        scope: $scope, // to automatically update the html. Default: $rootScope
        url: '/Naedo/Staging/Upload'
      });

      uploader.bind('error', function(event, xhr, item, response) {
        console.info('Error', xhr, item, response);
      });

      uploader.bind('complete', function(event, xhr, item, response) {
        $http.post('/api/ETL/?p=' + response).success(function(data, status, headers, config) {

          if (status === 200) {
            toaster.pop('success', "Batch", 'File has been staged', 5000, 'trustedHtml');
            completeCount++;
          }
          if (completeCount === uploader.queue.length) {
            completeCount = 0;
          }
        }).error(function(data, status, headers, config) {
          if (status === 400)
            toaster.pop('error', "Batch", 'File has to many transactions.', 5000, 'trustedHtml');
          else if (status === 500) {
            toaster.pop('error', "Batch", 'Internal error processing file.', 5000, 'trustedHtml');
          }
        });
      });

      uploader.bind('progressall', function(event, progress) {
        console.info('Total progress: ' + progress);
      });

      uploader.bind('progress', function(event, item, progress) {
        console.info('Progress: ' + progress, item);
      });

      uploader.bind('completeall', function(event, items) {

      });
    }
  ]);
}(this.angular));