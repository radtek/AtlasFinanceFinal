(function(angular) {

    var app = angular.module('falcon');

    app.controller('NotificationBarController', ['$scope', 'NotificationService', '$rootScope',
        function($scope, NotificationService, $rootScope) {

            // Begin Routing

            $scope.SeeAllNotifications = function(branchId, userId) {
                window.location = "/User/Notification/Index/?branchId=" + branchId + "&userId=" + userId;
            };

            $scope.SeeNotificationDetail = function(userId, detailId) {
                window.location = "/User/Notification/Detail/?userId=" + userId + "&notificationId=" + detailId;
            };

            // End Routing

            // Begin Methods

            $rootScope.UpdateNotificationFigures = function() {
                $scope.notifications = NotificationService.notes();
                $scope.noteCount = NotificationService.noteCount();
            };

            $scope.init = function(branchId, userId) {
                // Begin Notification Updates

                $rootScope.UpdateNotificationFigures();

                NotificationService.connection('Notification');
                NotificationService.setProperty('UserId', userId);

                NotificationService.subscribe('new_notification', {}).process(function(data) {
                    NotificationService.saveNotification(data);
                    $rootScope.UpdateNotificationFigures();
                });

                // End Notification Updates
            };

            // End Methods
        }
    ]);

    app.controller('NotificationController', ['$scope', '$http', '$rootScope',
        function($scope, http, $rootScope) {
            var _selected = [];
            $scope.loaded = false;

            var getUrl = function() {
                return '/api/Notification';
            };
            var _init = function(branchId, userId) {
                http.get(getUrl(), {
                    params: {
                        branchId: branchId || 0,
                        userId: userId
                    }
                }).success(function(resp) {
                    $scope.notifications = resp.notifications;
                    $scope.loaded = true;
                }).error(function(e) {
                    console.log('Error Fetching Notifications: ' + e);
                });
            };

            var postUrl = function(userId, notificationId) {
                return '/api/Notification/MarkAsRead/?userId=' + userId + '&notificationId=' + notificationId;
            };

            function RemoveNote(id) {
                for (i = $scope.notifications.length - 1; i >= 0; i--) {
                    if ($scope.notifications[i].NotificationId === id) {
                        $scope.notifications.splice(i, 1);
                    }
                }

                var notification = JSON.parse(localStorage.getItem('Falcon_Notifications'));
                for (i = notification.Notes.length - 1; i >= 0; i--) {
                    if (notification.Notes[i].NotificationId === id) {
                        notification.Notes.splice(i, 1);
                    }
                }
                notification.NoteCount--;

                localStorage.setItem('Falcon_Notifications', JSON.stringify(notification));
                $rootScope.UpdateNotificationFigures();
            }

            function MarkAsRead(userId, notificationId) {
                http.post(postUrl(userId, notificationId)).success(function() {
                    // remove note from data
                    RemoveNote(notificationId);
                }).error(function(data, status, headers, config) {
                    console.log("error marking notification as read: " + notificationId + ", " + status);
                });
            }

            $scope.markAsRead = function(userId, notificationId) {
                MarkAsRead(userId, notificationId);
            };

            $scope.markSelectedAsRead = function(userId) {
                for (i = $scope.notifications.length - 1; i >= 0; i--) {
                    if ($scope.notifications[i].Selected) {
                        MarkAsRead(userId, $scope.notifications[i].NotificationId);
                    }
                }
            };

            $scope.markAllAsRead = function(userId) {
                for (i = $scope.notifications.length - 1; i >= 0; i--) {
                    MarkAsRead(userId, $scope.notifications[i].NotificationId);
                }
            };

            $scope.init = function(branchId, userId) {
                _init(branchId, userId);
            };

            $scope.refresh = function(branchId, userId) {
                _init(branchId, userId);
            };

            $scope.goTo = function(url) {
                window.location = url;
            };
        }
    ]);

    app.controller('NotificationDetailController', ['$scope', '$http',
        function($scope, http) {
            $scope.loaded = false;

            var getUrl = function() {
                return '/api/Notification/GetNotification';
            };

            $scope.init = function(userId, notificationId) {
                http.get(getUrl(), {
                    params: {
                        userId: userId,
                        notificationId: notificationId
                    }
                }).success(function(resp) {
                    $scope.notifications = resp.notification;
                    $scope.loaded = true;

                }).error(function(e) {
                    console.log('Error Fetching Notification: ' + e);
                });
            };
        }
    ]);

}(this.angular));