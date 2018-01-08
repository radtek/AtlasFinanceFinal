(function(angular) {
    'use strict';
    var app = angular.module('falcon');

    app.controller('CampaignSmsController', ['$scope', '$rootScope', '$sce', 'CampaignResource', 'FalconTable', '$filter', 'ngProgress', 'toaster',
        function($scope, $rootScope, $sce, CampaignResource, FalconTable, $filter, ngProgress, toaster) {
            $rootScope.$broadcast('menu', {
                name: 'Campaign Manager',
                Desc: 'sms manager',
                url: $sce.trustAsUrl('/#!/Campaign/Sms/Manager/'),
                searchVisible: false
            });

            var date = $filter('date')(new Date(), 'yyyy-MM-dd');
            $scope.filterStartDate = $scope.filterStartDate || date
            $scope.filterEndDate = $scope.filterEndDate || date;
            $scope.smsMessage = '';
            $scope.loading = false;

            var _get = function(notificationId, startDate, endDate) {

                CampaignResource.getSmsList(notificationId, startDate, endDate).success(function(data) {

                    $scope.smsTransactions = data;
                    if ($scope.campaignParams) {
                        $scope.campaignParams.reload();
                    } else {
                        $scope.campaignParams = FalconTable.new($scope, 'smsTransactions', 10, [10]);
                    }
                    $scope.loading = false;

                    ngProgress.complete();
                }).error(function(status) {
                    alert(status);
                });
            };

            $scope.refresh = function() {
                $scope.loading = true;
                ngProgress.start();
                _get(null, $scope.filterStartDate, $scope.filterEndDate);
            }

            $scope.view = function(sms) {
                $scope.smsMessage = sms;
            };

            $scope.resend = function(sms) {
                $scope.loading = true;
                CampaignResource.resendSms(sms.NotificationId).success(function(status) {
                    toaster.pop('success', "Notification", "Notification has been enqueued!");
                    $scope.loading = false;
                    $scope.dismiss();

                    _get(null, $scope.filterStartDate, $scope.filterEndDate);
                }).error(function(status) {

                });
            }

            $scope.init = function() {
                ngProgress.start();
                _get(null, $scope.filterStartDate, $scope.filterEndDate);
            };
        }
    ]);
}(this.angular));