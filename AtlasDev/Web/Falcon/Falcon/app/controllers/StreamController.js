(function (angular) {
  'use strict';
  var app = angular.module('falcon');

  app.controller('StreamCollectionsController', ['$scope', '$rootScope', '$sce', '$filter', 'ngProgress', 'toaster', '$timeout', 'StreamResource', '$compile', 'UserResource',
      function ($scope, $rootScope, $sce, $filter, ngProgress, toaster, $ts, streamResource, $compile, userResource) {
        $scope.groupType = 'Collections';

        $rootScope.$broadcast('menu', {
          name: 'Stream',
          Desc: 'collections',
          url: '/Stream/Work',
          searchVisible: false
        });

        var getStaticData = function (callBackOnSuccess) {
          streamResource.getStaticData($scope.groupType, $scope.token).then(function (result) {
            if (result.status === 200) {
              $scope.streams = result.data.streamTypes;
              $scope.reasonsFollowUps = [];
              $scope.reasonsPTP = [];
              $scope.reasonsPTPUnchanged = [];
              $scope.reasonsNoAction = [];
              $scope.reasonsPTPBroken = [];
              $scope.reasonsEscalate = [];
              $scope.reasonsDeEscalate = [];
              $.each(result.data.comments, function (index, value) {
                if (value.CommentGroup.CommentGroupId === 6) { // Collections_FollowUps
                  $scope.reasonsFollowUps.push(value);
                } else if (value.CommentGroup.CommentGroupId === 7) { // Collections_PTP
                  $scope.reasonsPTP.push(value);
                } else if (value.CommentGroup.CommentGroupId === 8) { // Collections_NoAction
                  $scope.reasonsNoAction.push(value);
                } else if (value.CommentGroup.CommentGroupId === 9) { // Collections_PTPBroken
                  $scope.reasonsPTPBroken.push(value);
                } else if (value.CommentGroup.CommentGroupId === 10) { // Collections_Escalate
                  $scope.reasonsEscalate.push(value);
                } else if (value.CommentGroup.CommentGroupId === 12) { // Collections_DeEscalate
                  $scope.reasonsDeEscalate.push(value);
                } else if (value.CommentGroup.CommentGroupId === 16) { // Collections_PTPUnchanged
                  $scope.reasonsPTPUnchanged.push(value);
                }
              });
              $scope.isLoaded = true;

              if (callBackOnSuccess)
                callBackOnSuccess();
            } else {
              toaster.pop('danger', "Loading Static Data", "Error loading Static Data!");
              console.error(result.statusText);
            }
          }, function (result) {
            toaster.pop('danger', "Loading Static Data", "Error loading Static Data!");
            console.error(result.statusText);
          });
        }

        $scope.actionComplete = false;
        $scope.modalFocus = '';
        $scope.saving = false;
        $scope.streamLoadStatus = '';
        $scope.notes = {};
        $scope.subNotes = {};
        $scope.date = null;
        $scope.amount = null;
        $scope.reason = null;
        $scope.workItemSummaryText = '';
        $scope.caseNo = undefined;
        $scope.accountNo = undefined;
        $scope.filterActionStartDate = undefined;
        $scope.filterActionEndDate = undefined;
        $scope.searching = false;
        $scope.params = {};
        $scope.savingContact = undefined;
        $scope.pageLimit = 10;
        $scope.isManagedItem = false;
        var userId = userResource.getUserId();
        var openRows = [];

        $scope.dateDaysAhead = 45;

        $scope.setReasonDescription = function (selectedReason, reasonList) {
          $scope.reasonDescription = "";
          $.each(reasonList, function (index, value) {
            if (value.CommentId === selectedReason) {
              $scope.reasonDescription = value.Description;
            }
          });
        }

        $scope.$on('collections-notification-popup', function (event, args) {
          args.data = JSON.parse(args.data);
          var summaryText = '';
          if (args.data.length > 0) {
            summaryText = 'There are currently: ';
            for (var i = 0; i < args.data.length; i++) {
              summaryText += '[' + args.data[i].Item2 + '] ' + args.data[i].Item1.Description;
              if (i < (args.data.length - 1))
                summaryText += ', ';
            }
            summaryText += ' waiting for your attention within the next 15 minutes.';
          }
          $scope.workItemSummaryText = $sce.trustAsHtml(summaryText);
          $scope.workItemSummaryText.length = summaryText.length;
          if (!angular.element('#allocateDialog').hasClass('in') && !$scope.searching) {
            _getNextWorkItem();
            $scope.search();
          }
          $rootScope.$apply();
        });


        $scope.$on('collections-no-tasks', function (event, args) {
          $scope.workItemSummaryText = args.data;
          $rootScope.$apply();
        })

        $scope.$on('CollectionsIsBusyConsumer', function (event, args) {
          if (!angular.element('#allocateDialog').hasClass('in')) {
            // $scope.$emit('')
            alert('test');
          }
        })

        $scope.$on('$destroy', function () {
          $scope.$emit('unsubscribeChannel', {
            userId: userId,
            channel: $scope.groupType
          });
        });

        $scope.showContactType = function (type) {
          $scope.contactType = type;
          angular.element('#contactAddDlg').modal('show');
        }


        $scope.init = function () {
          getStaticData(function () {
            $scope.search();
          });
          $scope.$emit('subscribeChannel', {
            userId: userId,
            channel: $scope.groupType,
          });
        }

        $scope.notWorking = 'Start';

        $scope.startStopWorking = function () {
          if ($scope.notWorking === 'Start') {
            $scope.notWorking = 'Stop';
            _getNextWorkItem();
          }
          else {
            $scope.notWorking = 'Start';
          }
        };

        $scope.addContact = function (type, contactNo) {
          $scope.savingContact = true;
          var _contactType = ''
          switch (type) {
            case 'Cell':
              _contactType = 'CellNo';
              break;
            case 'FaxNo':
              _contactType = 'TelNoWorkFax';
              break;
            case 'Email':
              _contactType = 'Email';
              break;
            case 'WorkNo':
              _contactType = 'TelNoWork';
              break;
          }

          streamResource.saveContact($scope.currentWorkItem.DebtorId, _contactType, contactNo, userId, $scope.token).then(function (data) {
            toaster.pop('info', 'Save Contact', 'The contact detail has been saved');
            $scope.savingContact = false;
            angular.element('#contactAddDlg').modal('hide');
            var contact = {
              "ContactId": -2,
              "Value": contactNo
            };
            switch (_contactType) {
              case 'CellNo':
                $scope.currentWorkItem.DebtorCell.push(contact);
                break;
              case 'TelNoWorkFax':
                $scope.currentWorkItem.DebtorFax.push(contact);
                break;
              case 'Email':
                $scope.currentWorkItem.DebtorEmail.push(contact);
                break;
              case 'TelNoWork':
                $scope.currentWorkItem.DebtorWorkNo.push(contact);
                break;
            }

            var _checks = ['DebtorCell', 'DebtorEmail', 'DebtorFax', 'DebtorWorkNo'];
            for (var i = _checks.length - 1; i >= 0; i--) {
              if (angular.isArray($scope.currentWorkItem[_checks[i]]) && $scope.currentWorkItem[_checks[i]].length > 1)
                $scope.currentWorkItem[_checks[i]].IsArray = true;
              else
                $scope.currentWorkItem[_checks[i]].IsArray = false;
            };

            $scope.contactNo = undefined;
          }, function (data) {
            toaster.pop('danger', 'Error', data.data && data.data.ExceptionMessage ? data.data.ExceptionMessage : 'An Error as occured');
            $scope.savingContact = false;
            $scope.contactNo = undefined;
          });
        }

        $scope.syncPayment = function () {
          $scope.synch = true;
          streamResource.syncPayment($scope.currentWorkItem.CaseStreamId).then(function (data) {
            $scope.synch = false;
            if (data.data === 'Pending')
              toaster.pop('info', 'Payments', 'No payments found');
            else
              toaster.pop('info', 'Payments', 'Payments synched');

          }, function (data) {
            toaster.pop('danger', 'Error', data.data && data.data.ExceptionMessage ? data.data.ExceptionMessage : 'An Error as occured');
            $scope.synch = false;
          });
        }

        $scope.search = function () {
          var offset = $scope.params.offset || 0;
          $scope.searching = true;
          streamResource.getWorkItems(userId, null, null, $scope.caseNo, $scope.idNumber, $scope.accountReferenceNo, $scope.filterActionStartDate, $scope.filterActionEndDate, $scope.streamType, $scope.groupType, null, $scope.token).then(function (data) {
            $scope.streamData = data.data;
            $scope.params.total = data.data.length;

            $scope.searching = false;
          }, function (data) {
            toaster.pop('danger', 'Error', data.data && data.data.ExceptionMessage ? data.data.ExceptionMessage : 'An Error as occured');
            $scope.searching = false;
          });
        }


        $scope.escalate = function (reason, cascadeClose, nextItem) {
          $scope.escalating = true;

          var escalationLevel = [];

          if ($scope.chkBranchManager)
            escalationLevel.push('BranchManager');
          if ($scope.chkAdminManager)
            escalationLevel.push('AdminManager');
          if ($scope.chkRegionalManager)
            escalationLevel.push('RegionManager');
          if ($scope.chkDirector)
            escalationLevel.push('Director');

          streamResource.escalate($scope.currentWorkItem.CaseStreamId, userId, escalationLevel, reason, $scope.token).then(function (data) {
            toaster.pop('info', 'Escalated', 'Case has been escalated.');
            $scope.escalating = false;
            $scope.chkBranchManager = undefined;
            $scope.chkAdminManager = undefined;
            $scope.chkRegionalManager = undefined;
            $scope.chkDirector = undefined;

            angular.element('#escalateDlg').modal('hide');
            if (cascadeClose) angular.element('#allocateDialog').modal('hide');
            if (nextItem) _getNextWorkItem();

            $scope.search();

          }, function (data) {
            toaster.pop('danger', 'Error', data.data && data.data.ExceptionMessage ? data.data.ExceptionMessage : 'An Error as occured');
            $scope.escalating = false;
          });
        }

        var _setCurrentWorkItem = function (workItem) {

          streamResource.getWorkItem(workItem.CaseStreamActionId, userId).then(function (data) {
            $scope.currentWorkItem = data.data.Case;
            $scope.previousCompleteNote = data.data.PreviousCaseCompleteNote;
            $scope.notes = data.data.Notes;
            $scope.notes.total = data.data.Notes.length;

            if ($scope.currentWorkItem.FrequencyId == 2)
              $scope.dateDaysAhead = 9;
            else if ($scope.currentWorkItem.FrequencyId == 3)
              $scope.dateDaysAhead = 16;
            else if ($scope.currentWorkItem.FrequencyId == 4)
              $scope.dateDaysAhead = 34;
            $scope.maxDate = new Date().addDays($scope.dateDaysAhead);

            var _checks = ['DebtorCell', 'DebtorEmail', 'DebtorFax', 'DebtorWorkNo'];
            for (var i = _checks.length - 1; i >= 0; i--) {
              if (angular.isArray($scope.currentWorkItem[_checks[i]]) && $scope.currentWorkItem[_checks[i]].length > 1)
                $scope.currentWorkItem[_checks[i]].IsArray = true;
              else
                $scope.currentWorkItem[_checks[i]].IsArray = false;
            };

            angular.element('#allocateDialog').modal('show');
            $scope.searching = false;
          }, function (data) {
            toaster.pop('danger', 'Error', data.data && data.data.ExceptionMessage ? data.data.ExceptionMessage : 'An Error as occured');
            $scope.searching = false;
          });
        }
        $scope.dateTooFar = false;

        $scope.dateChanged = function () {
          $scope.dateTooFar = (new Date($scope.date) > $scope.maxDate);
        }

        var _getNextWorkItem = function () {
          if ($scope.notWorking === 'Start')
            return;
          // Prevents a popup collision if the user is searching
          // then suddenly a fresh popup returns overwriting the currentWorkItem scope.
          $scope.reasonDescription = "";
          if (!$scope.searching) {
            $scope.searching = true;
            streamResource.getNextWorkItem(userId, 0, $scope.groupType, $scope.token).then(function (data) {
              if (data !== 'Empty' && data.data !== null) {
                var offset = $scope.notes.offset || 0;

                $scope.date = undefined;
                $scope.amount = undefined;
                $scope.reason = undefined;
                $scope.currentWorkItem = data.data.Case;
                $scope.previousCompleteNote = data.data.PreviousCaseCompleteNote;
                $scope.notes = data.data.Notes;
                if (data.data.Notes !== undefined)
                  $scope.notes.total = data.data.Notes.length;
                else
                  $scope.notes.total = 0;


                if ($scope.currentWorkItem.FrequencyId == 2)
                  $scope.dateDaysAhead = 9;
                else if ($scope.currentWorkItem.FrequencyId == 3)
                  $scope.dateDaysAhead = 16;
                else if ($scope.currentWorkItem.FrequencyId == 4)
                  $scope.dateDaysAhead = 34;
                $scope.maxDate = new Date().addDays($scope.dateDaysAhead);

                var _checks = ['DebtorCell', 'DebtorEmail', 'DebtorFax', 'DebtorWorkNo'];
                for (var i = _checks.length - 1; i >= 0; i--) {
                  if (angular.isArray($scope.currentWorkItem[_checks[i]]) && $scope.currentWorkItem[_checks[i]].length > 1)
                    $scope.currentWorkItem[_checks[i]].IsArray = true;
                  else
                    $scope.currentWorkItem[_checks[i]].IsArray = false;
                };

                $scope.isManagedItem = false;
                angular.element('#allocateDialog').modal('show');
                $scope.searching = false;
              } else {
                $scope.searching = false;
              }
            });
          }
        };
        $scope.saveAction = function (action, date, amount, comment, dialog, cascadeClose, nextItem) {

          $scope.saving = true;

          if (amount)
            amount = Number(amount.replace(/[^0-9\.]+/g, ""));

          streamResource.save($scope.currentWorkItem.CaseStreamId, $scope.currentWorkItem.CaseStreamActionId, userId, action, date, amount, comment, $scope.token).then(function (data) {
            $scope.saving = false;
            angular.element('#' + dialog).modal('hide');
            if (cascadeClose) angular.element('#allocateDialog').modal('hide');
            if ($scope.isManagedItem) {
              if (nextItem) _getNextWorkItem();
            } else {
              _getNextWorkItem();
            }
            $scope.search();
          }, function (data) {
            toaster.pop('danger', 'Error', data.data && data.data.ExceptionMessage ? data.data.ExceptionMessage : 'An Error as occured');
            $scope.saving = false;
          });
        };


        $scope.manage = function (item, callingControl) {

          _setCurrentWorkItem(item)
          $scope.isManagedItem = true;
        }

        $scope.addComment = function () {
          $scope.savingComment = true;
          streamResource.saveCaseComment($scope.currentWorkItem.CaseId, userId, $scope.newComment).then(function (data) {
            $scope.savingComment = false;
            toaster.pop('info', 'Comment', 'Your note has been saved.');
            $scope.notes.unshift(data.data);
            $scope.showNewComment = false;
          }, function (data) {
            toaster.pop('danger', 'Error', data.data && data.data.ExceptionMessage ? data.data.ExceptionMessage : 'An Error as occured');
          });
        }

        $scope.selectedNote = undefined;

        $scope.commentRow = function () {
          $scope.newComment = "";
          $scope.showNewComment = true;
        };

        $scope.printHistory = function() {
          $('#history').printThis({ importCSS: true, importStyle: true });
        };
      }
  ]);


  app.controller('StreamSalesController', ['$scope', '$rootScope', '$sce', '$filter', 'ngProgress', 'toaster', '$timeout', 'StreamResource', '$compile', 'UserResource', 'BureauResource',
      function ($scope, $rootScope, $sce, $filter, ngProgress, toaster, $ts, streamResource, $compile, userResource, bureauResource) {
        $scope.groupType = 'Sales';
        $rootScope.$broadcast('menu', {
          name: 'Stream',
          Desc: 'sales',
          url: '/Stream/Assigned',
          searchVisible: false
        });

        $scope.actionComplete = false;
        $scope.modalFocus = '';
        $scope.saving = false;
        $scope.streamLoadStatus = '';
        $scope.notes = {};
        $scope.subNotes = {};
        $scope.date = null;
        $scope.amount = null;
        $scope.reason = null;
        $scope.workItemSummaryText = '';
        $scope.caseNo = undefined;
        $scope.accountNo = undefined;
        $scope.filterActionStartDate = undefined;
        $scope.filterActionEndDate = undefined;
        $scope.searching = false;
        $scope.params = {};
        $scope.searching = false;
        $scope.savingContact = undefined;
        $scope.pageLimit = 10;
        $scope.isManagedItem = false;
        $scope.scoring = false;
        var userId = userResource.getUserId();
        var openRows = [];

        $scope.dateDaysAhead = 30;
        $scope.dateTooFar = false;

        $scope.dateChanged = function () {
          $scope.dateTooFar = (new Date($scope.date) > $scope.maxDate);
        }
        var getStaticData = function (callBackOnSuccess) {
          streamResource.getStaticData($scope.groupType, $scope.token).then(function (result) {
            if (result.status === 200) {
              $scope.streams = result.data.streamTypes;
              $scope.reasonsNotInterested = [];
              $scope.reasonsFollowUps = [];
              $scope.reasonsPTC = [];
              $scope.reasonsPTCUnchanged = [];
              $scope.reasonsNoAction = [];
              $scope.reasonsPTCBroken = [];
              $scope.reasonsDeEscalate = [];
              $scope.reasonsForceClosed = [];
              $.each(result.data.comments, function (index, value) {
                if (value.CommentGroup.CommentGroupId === 1) { // Sales_NotInterested
                  $scope.reasonsNotInterested.push(value);
                } else if (value.CommentGroup.CommentGroupId === 2) { // Sales_FollowUps
                  $scope.reasonsFollowUps.push(value);
                } else if (value.CommentGroup.CommentGroupId === 3) { // Sales_PTC
                  $scope.reasonsPTC.push(value);
                } else if (value.CommentGroup.CommentGroupId === 4) { // Sales_NoAction
                  $scope.reasonsNoAction.push(value);
                } else if (value.CommentGroup.CommentGroupId === 5) { // Sales_PTCBroken
                  $scope.reasonsPTCBroken.push(value);
                } else if (value.CommentGroup.CommentGroupId === 11) { // Sales_DeEscalate
                  $scope.reasonsDeEscalate.push(value);
                } else if (value.CommentGroup.CommentGroupId === 15) { // Sales_PTCUnchanged
                  $scope.reasonsPTCUnchanged.push(value);
                } else if (value.CommentGroup.CommentGroupId === 19) { // Sales_ForceClosed
                  $scope.reasonsForceClosed.push(value);
                }
              });
              $scope.isLoaded = true;

              if (callBackOnSuccess)
                callBackOnSuccess();
            } else {
              toaster.pop('danger', "Loading Static Data", "Error loading Static Data!");
              console.error(result.statusText);
            }
          }, function (result) {
            toaster.pop('danger', "Loading Static Data", "Error loading Static Data!");
            console.error(result.statusText);
          });
        }

        $scope.setReasonDescription = function (selectedReason, reasonList) {
          $scope.reasonDescription = "";
          $.each(reasonList, function (index, value) {
            if (value.CommentId === selectedReason) {
              $scope.reasonDescription = value.Description;
            }
          });
        }

        $scope.$on('sales-notification-popup', function (event, args) {
          args.data = JSON.parse(args.data);
          var summaryText = '';
          if (args.data.length > 0) {
            summaryText = 'There are currently: ';
            for (var i = 0; i < args.data.length; i++) {
              summaryText += '[' + args.data[i].Item2 + '] ' + args.data[i].Item1.Description;
              if (i < (args.data.length - 1))
                summaryText += ', ';
            }
            summaryText += ' waiting for your attention within the next 15 minutes.';
          }
          $scope.workItemSummaryText = $sce.trustAsHtml(summaryText);
          $scope.workItemSummaryText.length = summaryText.length;
          if (!angular.element('#allocateDialog').hasClass('in') && !$scope.searching) {
            _getNextWorkItem();
          }
          $rootScope.$apply();
        });


        $scope.$on('sales-no-tasks', function (event, args) {
          $scope.workItemSummaryText = args.data;
          $rootScope.$apply();
        });

        $scope.notWorking = 'Start';

        $scope.startStopWorking = function () {
          if ($scope.notWorking === 'Start') {
            $scope.notWorking = 'Stop';
            _getNextWorkItem();
          }
          else {
            $scope.notWorking = 'Start';
          }
        };

        $scope.addContact = function (type, contactNo) {
          $scope.savingContact = true;
          var _contactType = ''
          switch (type) {
            case 'Cell':
              _contactType = 'CellNo';
              break;
            case 'FaxNo':
              _contactType = 'TelNoWorkFax';
              break;
            case 'Email':
              _contactType = 'Email';
              break;
            case 'WorkNo':
              _contactType = 'TelNoWork';
              break;
          }

          streamResource.saveContact($scope.currentWorkItem.DebtorId, _contactType, contactNo, userId, $scope.token).then(function (data) {
            toaster.pop('info', 'Save Contact', 'The contact detail has been saved');
            $scope.savingContact = false;
            angular.element('#contactAddDlg').modal('hide');
            var contact = {
              "ContactId": -2,
              "Value": contactNo
            };
            switch (_contactType) {
              case 'CellNo':
                $scope.currentWorkItem.DebtorCell.push(contact);
                break;
              case 'TelNoWorkFax':
                $scope.currentWorkItem.DebtorFax.push(contact);
                break;
              case 'Email':
                $scope.currentWorkItem.DebtorEmail.push(contact);
                break;
              case 'TelNoWork':
                $scope.currentWorkItem.DebtorWorkNo.push(contact);
                break;
            }

            var _checks = ['DebtorCell', 'DebtorEmail', 'DebtorFax', 'DebtorWorkNo'];
            for (var i = _checks.length - 1; i >= 0; i--) {
              if (angular.isArray($scope.currentWorkItem[_checks[i]]) && $scope.currentWorkItem[_checks[i]].length > 1)
                $scope.currentWorkItem[_checks[i]].IsArray = true;
              else
                $scope.currentWorkItem[_checks[i]].IsArray = false;
            };

            $scope.contactNo = undefined;
          }, function (data) {
            toaster.pop('danger', 'Error', data.data && data.data.ExceptionMessage ? data.data.ExceptionMessage : 'An Error as occured');
            $scope.savingContact = false;
            $scope.contactNo = undefined;
          });
        }

        $scope.addComment = function () {
          $scope.savingComment = true;
          streamResource.saveCaseComment($scope.currentWorkItem.CaseId, userId, $scope.newComment).then(function (data) {
            $scope.savingComment = false;
            toaster.pop('info', 'Comment', 'Your note has been saved.');
            $scope.notes.unshift(data.data);
            $scope.showNewComment = false;
          }, function (data) {
            toaster.pop('danger', 'Error', data.data && data.data.ExceptionMessage ? data.data.ExceptionMessage : 'An Error as occured');
          });
        }

        $scope.saveNotInterested = function (cascadeClose) {
          $scope.saving = true;
          streamResource.saveNotInterested($scope.currentWorkItem.CaseStreamId, userId, $scope.token).then(function (data) {
            angular.element('#notInterestedDlg').modal('hide');
            $scope.saving = false;
            if ($scope.isManagedItem) {
              if (nextItem) _getNextWorkItem();
            } else {
              _getNextWorkItem();
            }
            $scope.search();
            if (cascadeClose) angular.element('#allocateDialog').modal('hide');
          }, function (data) {
            toaster.pop('danger', 'Error', data.data && data.data.ExceptionMessage ? data.data.ExceptionMessage : 'An Error as occured');
            $scope.saving = false;
          });
        }

        $scope.commentRow = function () {
          $scope.newComment = "";
          $scope.showNewComment = true;
        };

        $scope.performNewScore = function () {
          $scope.scoring = true;
          bureauResource.getRecentScore($scope.currentWorkItem.DebtorId,
              $scope.currentWorkItem.BranchId, 'true', $scope.token).then(function (data) {
                $scope.currentWorkItem.Products = [];

                $scope.currentWorkItem.Score = data.data.Score;
                $scope.currentWorkItem.LastScoreDate = data.data.ScoreDate;
                for (var i = data.data.Products.length - 1; i >= 0; i--) {
                  $scope.currentWorkItem.Products.push({
                    ProductName: data.data.Products[i].Description,
                    Outcome: data.data.Products[i].Outcome
                  });
                };
                $scope.scoring = false;
                angular.element('#surePerformScore').modal('hide');
              }, function (data, status) {
                if (status === '400') {
                  toaster.pop('danger', 'Error', 'Scoring error or budget exceeded.');
                } else {
                  toaster.pop('danger', 'Error', data.data && data.data.ExceptionMessage ? data.data.ExceptionMessage : 'An Error as occured');
                }
                $scope.scoring = false;
              });

        }

        var _getNextWorkItem = function () {
          if ($scope.notWorking === 'Start')
            return;
          // Prevents a popup collision if the user is searching
          // then suddenly a fresh popup returns overwriting the currentWorkItem scope.
          $scope.reasonDescription = "";
          if (!$scope.manualSearch) {
            $scope.searching = true;
            streamResource.getNextWorkItem(userId, 0, $scope.groupType, $scope.token).then(function (data) {
              var offset = $scope.notes.offset || 0;
              if (data !== 'Empty' && data.data !== null) {
                $scope.currentWorkItem = data.data.Case;
                $scope.previousCompleteNote = data.data.PreviousCaseCompleteNote;
                bureauResource.getRecentScore($scope.currentWorkItem.DebtorId,
                    $scope.currentWorkItem.BranchId, 'false', $scope.token).then(function (ddata) {
                      $scope.currentWorkItem.Products = [];

                      if ($scope.currentWorkItem.PerformNewCreditEnquiry) {
                        $scope.currentWorkItem.PerformNewCreditEnquiry = ddata.data.Score === null || ddata.data.Score === 'empty' || ddata.data.Age > 7 ? true : false;
                      }
                      $scope.currentWorkItem.Score = ddata.data.Score;
                      $scope.currentWorkItem.LastScoreDate = ddata.data.ScoreDate;
                      for (var i = ddata.data.Products.length - 1; i >= 0; i--) {
                        $scope.currentWorkItem.Products.push({
                          ProductName: ddata.data.Products[i].Description,
                          Outcome: ddata.data.Products[i].Outcome
                        });
                      };
                    }, function (data) {
                      if (status === '400') {
                        toaster.pop('danger', 'Error', 'Scoring error or budget exceeded.');
                      } else {
                        toaster.pop('danger', 'Error', data.data && data.data.ExceptionMessage ? data.data.ExceptionMessage : 'An Error as occured');
                      }
                      $scope.scoring = false;
                    });

                $scope.date = undefined;
                $scope.amount = undefined;
                $scope.reason = undefined;
                $scope.notes = data.data.Notes;
                $scope.notes.total = data.data.Notes.length;

                if ($scope.currentWorkItem.FrequencyId == 2)
                  $scope.dateDaysAhead = 9;
                else if ($scope.currentWorkItem.FrequencyId == 3)
                  $scope.dateDaysAhead = 16;
                else if ($scope.currentWorkItem.FrequencyId == 4)
                  $scope.dateDaysAhead = 34;
                $scope.maxDate = new Date().addDays($scope.dateDaysAhead);

                var _checks = ['DebtorCell', 'DebtorEmail', 'DebtorFax', 'DebtorWorkNo'];
                for (var i = _checks.length - 1; i >= 0; i--) {
                  if (angular.isArray($scope.currentWorkItem[_checks[i]]) && $scope.currentWorkItem[_checks[i]].length > 1)
                    $scope.currentWorkItem[_checks[i]].IsArray = true;
                  else
                    $scope.currentWorkItem[_checks[i]].IsArray = false;
                };

                $scope.isManagedItem = false;
                angular.element('#allocateDialog').modal('show');
                $scope.searching = false;
              } else {
                $scope.searching = false;
              }
            });
          }
        };

        $scope.saveOutcome = function (outcome, reason, dialog, cascadeClose) {
          $scope.saving = true;
          streamResource.saveOutcome($scope.currentWorkItem.CaseStreamId, userId, outcome, reason, '', $scope.token).then(function (data) {
            $scope.saving = false;
            angular.element('#' + dialog).modal('hide');
            if (cascadeClose) angular.element('#allocateDialog').modal('hide');
            if ($scope.isManagedItem) {
              if (nextItem) _getNextWorkItem();
            } else {
              _getNextWorkItem();
            }
            $scope.outcome = undefined;
            $scope.reason = undefined;
            $scope.search();
          }, function (data) {
            toaster.pop('danger', 'Error', data.data && data.data.ExceptionMessage ? data.data.ExceptionMessage : 'An Error as occured');
            $scope.saving = false;
          });
        }

        $scope.manage = function (item, callingControl) {
          _setCurrentWorkItem(item);
          $scope.isManagedItem = true;
        }

        var _setCurrentWorkItem = function (workItem) {

          streamResource.getWorkItem(workItem.CaseStreamActionId, userId).then(function (data) {
            var offset = $scope.notes.offset || 0;
            if (data !== 'Empty') {
              $scope.currentWorkItem = data.data.Case;
              $scope.previousCompleteNote = data.data.PreviousCaseCompleteNote;
              bureauResource.getRecentScore($scope.currentWorkItem.DebtorId,
                $scope.currentWorkItem.BranchId, 'false', $scope.token).then(function (ddata) {
                  $scope.currentWorkItem.Products = [];

                  $scope.currentWorkItem.Score = ddata.data.Score;
                  $scope.currentWorkItem.LastScoreDate = ddata.data.ScoreDate;
                  if ($scope.currentWorkItem.PerformNewCreditEnquiry) {
                    $scope.currentWorkItem.PerformNewCreditEnquiry = ddata.data.Age > 7 ? true : false;
                  }
                  for (var i = ddata.data.Products.length - 1; i >= 0; i--) {
                    $scope.currentWorkItem.Products.push({
                      ProductName: ddata.data.Products[i].Description,
                      Outcome: ddata.data.Products[i].Outcome
                    });
                  };
                },
                function (data) {
                  toaster.pop('danger', 'Error', data.data && data.data.ExceptionMessage ? data.data.ExceptionMessage : 'An Error as occured');
                });

              $scope.date = undefined;
              $scope.amount = undefined;
              $scope.reason = undefined;
              $scope.notes = data.data.Notes;
              if (data.data.Notes !== undefined)
                $scope.notes.total = data.data.Notes.length;
              else
                $scope.notes.total = 0;

              if ($scope.currentWorkItem.FrequencyId == 2)
                $scope.dateDaysAhead = 9;
              else if ($scope.currentWorkItem.FrequencyId == 3)
                $scope.dateDaysAhead = 16;
              else if ($scope.currentWorkItem.FrequencyId == 4)
                $scope.dateDaysAhead = 34;
              $scope.maxDate = new Date().addDays($scope.dateDaysAhead);

              var _checks = ['DebtorCell', 'DebtorEmail', 'DebtorFax', 'DebtorWorkNo'];
              for (var i = _checks.length - 1; i >= 0; i--) {
                if (angular.isArray($scope.currentWorkItem[_checks[i]]) && $scope.currentWorkItem[_checks[i]].length > 1)
                  $scope.currentWorkItem[_checks[i]].IsArray = true;
                else
                  $scope.currentWorkItem[_checks[i]].IsArray = false;
              };
              angular.element('#allocateDialog').modal('show');
              $scope.searching = false;
            }
          },
            function (data) {
              toaster.pop('danger', 'Error', data.data && data.data.ExceptionMessage ? data.data.ExceptionMessage : 'An Error as occured');
            });
        };


        $scope.saveAction = function (action, date, amount, comment, dialog, cascadeClose, nextItem) {

          $scope.saving = true;

          if (amount)
            amount = Number(amount.replace(/[^0-9\.]+/g, ""));

          streamResource.save($scope.currentWorkItem.CaseStreamId, $scope.currentWorkItem.CaseStreamActionId, userId, action, date, amount, comment, $scope.token).then(function (data) {
            $scope.saving = false;
            angular.element('#' + dialog).modal('hide');
            if (cascadeClose) angular.element('#allocateDialog').modal('hide');
            if ($scope.isManagedItem) {
              if (nextItem) _getNextWorkItem();
            } else {
              _getNextWorkItem();
            }
            $scope.search();
          }, function (data) {
            toaster.pop('danger', 'Error', data.data && data.data.ExceptionMessage ? data.data.ExceptionMessage : 'An Error as occured');
            $scope.saving = false;
          });
        };

        $scope.search = function () {
          var offset = $scope.params.offset || 0;
          $scope.manualSearch = true;
          streamResource.getWorkItems(userId, null, null, $scope.caseNo, $scope.idNumber, $scope.accountNo, $scope.filterActionStartDate, $scope.filterActionEndDate, $scope.streamType, $scope.groupType, null, $scope.token).then(function (data) {
            $scope.streamData = data.data;
            $scope.params.total = data.data.length;
            $scope.manualSearch = false;
          }, function (data) {
            toaster.pop('danger', 'Error', data.data && data.data.ExceptionMessage ? data.data.ExceptionMessage : 'An Error as occured');
            $scope.manualSearch = false;
          });
        }

        $scope.showContactType = function (type) {
          $scope.contactType = type;
          angular.element('#contactAddDlg').modal('show');
        }

        $scope.init = function () {
          getStaticData(function () {
            $scope.search();
          });
          //$scope.$emit('subscribeChannel', {
          //    userId: userId,
          //    channel: $scope.groupType,
          //});
        }


        $scope.$on('$destroy', function () {
          $scope.$emit('unsubscribeChannel', {
            userId: userId,
            channel: $scope.groupType
          });
        });


        $scope.printHistory = function () {
          $('#history').printThis({ importCSS: true, importStyle: true });
        };
      }
  ]);

  app.controller('StreamManageController', ['$scope', '$rootScope', '$sce', '$filter', 'ngProgress', 'toaster', '$timeout', 'StreamResource', '$compile', 'UserResource',
      function ($scope, $rootScope, $sce, $filter, ngProgress, toaster, $ts, streamResource, $compile, userResource) {
        $rootScope.$broadcast('menu', {
          name: 'Stream',
          Desc: 'manage work items',
          url: '/Stream/Assigned',
          searchVisible: false
        });
        $scope.actionComplete = false;
        $scope.modalFocus = '';
        $scope.saving = false;
        $scope.notes = {};
        $scope.subNotes = {};
        $scope.date = null;
        $scope.amount = null;
        $scope.reason = null;
        $scope.workItemSummaryText = '';
        $scope.caseNo = undefined;
        $scope.accountNo = undefined;
        $scope.filterActionStartDate = undefined;
        $scope.filterActionEndDate = undefined;
        $scope.searching = false;
        $scope.params = {};
        $scope.searching = false;
        $scope.savingContact = undefined;
        $scope.pageLimit = 10;
        $scope.isManagedItem = false;
        $scope.consultantLoading = false;
        $scope.escalatedView = true;
        $scope.transferMultiButtonView = false;
        $scope.transferringMulti = "Transfer to";

        $scope.dateDaysAhead = 45;

        $scope.dateTooFar = false;

        $scope.dateChanged = function () {
          $scope.dateTooFar = (new Date($scope.date) > $scope.maxDate);
        }

        $scope.branchName = userResource.getUserBranchName();
        var branchId = userResource.getUserBranchId();
        var userId = userResource.getUserId();
        var openRows = [];

        $scope.categories = [{
          name: 'Collections',
          value: 'Collections'
        }, {
          name: 'Sales',
          value: 'Sales'
        }];

        $scope.categoryType = $scope.categories[0].value;

        var collectionComments = [];
        var salesComments = [];
        var collectionStreamTypes = [];
        var caseStatuses = [];
        var salesStreamTypes = [];
        var updateComments = function (comments) {
          $scope.reasonsNotInterested = [];
          $scope.reasonsFollowUps = [];
          $scope.reasonsPTC = [];
          $scope.reasonsPTP = [];
          $scope.reasonsNoAction = [];
          $scope.reasonsPTCBroken = [];
          $scope.reasonsPTPBroken = [];
          $scope.reasonsEscalate = [];
          $scope.reasonsDeEscalate = [];
          $scope.reasonsTransferCase = [];
          $scope.reasonsForceClosed = [];
          $.each(comments, function (index, value) {
            if (value.CommentGroup.CommentGroupId === 1) { // Sales_NotInterested
              $scope.reasonsNotInterested.push(value);
            } else if (value.CommentGroup.CommentGroupId === 2) { // Sales_FollowUps
              $scope.reasonsFollowUps.push(value);
            } else if (value.CommentGroup.CommentGroupId === 3) { // Sales_PTC
              $scope.reasonsPTC.push(value);
            } else if (value.CommentGroup.CommentGroupId === 4) { // Sales_NoAction
              $scope.reasonsNoAction.push(value);
            } else if (value.CommentGroup.CommentGroupId === 5) { // Sales_PTCBroken
              $scope.reasonsPTCBroken.push(value);
            } else if (value.CommentGroup.CommentGroupId === 6) { // Collections_FollowUps
              $scope.reasonsFollowUps.push(value);
            } else if (value.CommentGroup.CommentGroupId === 7) { // Collections_PTP
              $scope.reasonsPTP.push(value);
            } else if (value.CommentGroup.CommentGroupId === 8) { // Collections_NoAction
              $scope.reasonsNoAction.push(value);
            } else if (value.CommentGroup.CommentGroupId === 9) { // Collections_PTPBroken
              $scope.reasonsPTPBroken.push(value);
            } else if (value.CommentGroup.CommentGroupId === 10) { // Collections_Escalate
              $scope.reasonsEscalate.push(value);
            } else if (value.CommentGroup.CommentGroupId === 11) { // Sales_DeEscalate
              $scope.reasonsDeEscalate.push(value);
            } else if (value.CommentGroup.CommentGroupId === 12) { // Collections_DeEscalate
              $scope.reasonsDeEscalate.push(value);
            } else if (value.CommentGroup.CommentGroupId === 13) { // Sales_TransferCase
              $scope.reasonsTransferCase.push(value);
            } else if (value.CommentGroup.CommentGroupId === 14) { // Collections_TransferCase
              $scope.reasonsTransferCase.push(value);
            } else if (value.CommentGroup.CommentGroupId === 19) { // Sales_ForceClosed
              $scope.reasonsForceClosed.push(value);
            }
          });
        };

        var getStaticData = function (groupType) {
          if (groupType === 'Collections' && collectionComments.length === 0) {
            // perform rest call
            streamResource.getStaticData(groupType, $scope.token).then(function (result) {
              debugger;
              if (result.status === 200) {
                collectionComments = result.data.comments;
                collectionStreamTypes = result.data.streamTypes;
                caseStatuses = result.data.caseStatuses;
                $scope.caseStatuses = caseStatuses;
                $scope.streams = collectionStreamTypes;
                updateComments(collectionComments);
              } else {
                toaster.pop('danger', "Loading Static Data", "Error loading Static Data!");
                console.error(result.statusText);
              }
            }, function (result) {
              toaster.pop('danger', "Loading Static Data", "Error loading Static Data!");
              console.error(result.statusText);
            });
          } else if (groupType === 'Collections') {
            updateComments(collectionComments);
            $scope.streams = collectionStreamTypes;
            $scope.caseStatuses = caseStatuses;
          }
          if (groupType === 'Sales' && salesComments.length === 0) {
            // perform rest call
            streamResource.getStaticData(groupType, $scope.token).then(function (result) {
              debugger;
              if (result.status === 200) {
                salesComments = result.data.comments;
                salesStreamTypes = result.data.streamTypes;
                caseStatuses = result.data.caseStatuses;
                $scope.caseStatuses = caseStatuses;
                $scope.streams = salesStreamTypes;
                updateComments(salesComments);
              } else {
                toaster.pop('danger', "Loading Static Data", "Error loading Static Data!");
                console.error(result.statusText);
              }
            }, function (result) {
              toaster.pop('danger', "Loading Static Data", "Error loading Static Data!");
              console.error(result.statusText);
            });
          } else if (groupType === 'Sales') {
            updateComments(salesComments);
            $scope.streams = salesStreamTypes;
            $scope.caseStatuses = caseStatuses;
          }
        };

        $scope.setReasonDescription = function (selectedReason, reasonList) {
          $scope.reasonDescription = "";
          $.each(reasonList, function (index, value) {
            if (value.CommentId === selectedReason) {
              $scope.reasonDescription = value.Description;
            }
          });
        }

        $scope.showContactType = function (type) {
          $scope.contactType = type;
          angular.element('#contactAddDlg').modal('show');
        }

        $scope.addContact = function (type, contactNo) {
          $scope.savingContact = true;
          var _contactType = ''
          switch (type) {
            case 'Cell':
              _contactType = 'CellNo';
              break;
            case 'FaxNo':
              _contactType = 'TelNoWorkFax';
              break;
            case 'Email':
              _contactType = 'Email';
              break;
            case 'WorkNo':
              _contactType = 'TelNoWork';
              break;
          }
          streamResource.saveContact($scope.currentWorkItem.DebtorId, _contactType, contactNo, userId, $scope.token).then(function (data) {
            toaster.pop('info', 'Save Contact', 'The contact detail has been saved');
            $scope.savingContact = false;
            angular.element('#contactAddDlg').modal('hide');
            var contact = {
              "ContactId": -2,
              "Value": contactNo
            };
            switch (_contactType) {
              case 'CellNo':
                $scope.currentWorkItem.DebtorCell.push(contact);
                break;
              case 'TelNoWorkFax':
                $scope.currentWorkItem.DebtorFax.push(contact);
                break;
              case 'Email':
                $scope.currentWorkItem.DebtorEmail.push(contact);
                break;
              case 'TelNoWork':
                $scope.currentWorkItem.DebtorWorkNo.push(contact);
                break;
            }

            var _checks = ['DebtorCell', 'DebtorEmail', 'DebtorFax', 'DebtorWorkNo'];
            for (var i = _checks.length - 1; i >= 0; i--) {
              if (angular.isArray($scope.currentWorkItem[_checks[i]]) && $scope.currentWorkItem[_checks[i]].length > 1)
                $scope.currentWorkItem[_checks[i]].IsArray = true;
              else
                $scope.currentWorkItem[_checks[i]].IsArray = false;
            };

            $scope.contactNo = undefined;
          }, function (data) {
            toaster.pop('danger', 'Error', data.data && data.data.ExceptionMessage ? data.data.ExceptionMessage : 'An Error as occured');
            $scope.savingContact = false;
            $scope.contactNo = undefined;
          });
        }

        $scope.addComment = function () {
          $scope.savingComment = true;
          streamResource.saveCaseComment($scope.currentWorkItem.CaseId, userId, $scope.newComment).then(function (data) {
            $scope.savingComment = false;
            toaster.pop('info', 'Comment', 'Your note has been saved.');
            $scope.notes.unshift(data.data);
            $scope.showNewComment = false;
          }, function (data) {
            toaster.pop('danger', 'Error', data.data && data.data.ExceptionMessage ? data.data.ExceptionMessage : 'An Error as occured');
          });
        }

        $scope.saveNotInterested = function (cascadeClose) {
          $scope.saving = true;
          streamResource.saveNotInterested($scope.currentWorkItem.CaseStreamId, userId, $scope.token).then(function (data) {
            angular.element('#notInterestedDlg').modal('hide');
            $scope.saving = false;
            if (cascadeClose) angular.element('#allocateDialog').modal('hide');
          }, function (data) {
            toaster.pop('danger', 'Error', data.data && data.data.ExceptionMessage ? data.data.ExceptionMessage : 'An Error as occured');
            $scope.saving = false;
          });
        }


        $scope.commentRow = function () {
          $scope.newComment = "";
          $scope.showNewComment = true;
        };

        $scope.saveOutcome = function (outcome, reason, dialog, cascadeClose) {
          $scope.saving = true;
          streamResource.saveOutcome($scope.currentWorkItem.CaseStreamId, userId, outcome, reason, '', $scope.token).then(function (data) {
            $scope.saving = false;
            angular.element('#' + dialog).modal('hide');
            if (cascadeClose) angular.element('#allocateDialog').modal('hide');
            if ($scope.isManagedItem) {
              if (nextItem) _getNextWorkItem();
            } else {
              _getNextWorkItem();
            }
            $scope.outcome = undefined;
            $scope.reason = undefined;
          }, function (data) {
            toaster.pop('danger', 'Error', data.data && data.data.ExceptionMessage ? data.data.ExceptionMessage : 'An Error as occured');
            $scope.saving = false;
          });
        }

        $scope.manage = function (item) {
          var _buttonid = 'button_loading_' + item.CaseStreamActionId;
          $scope[_buttonid] = true;
          //angular.element('#' + callingControl).disab
          _setCurrentWorkItem(item, _buttonid)
          $scope.isManagedItem = true;
        }

        var _setCurrentWorkItem = function (workItem, callingControl) {

          streamResource.getWorkItem(workItem.CaseStreamActionId, userId).then(function (data) {
            $scope.currentWorkItem = data.data.Case;
            $scope.previousCompleteNote = data.data.PreviousCaseCompleteNote;
            getStaticData($scope.currentWorkItem.Group);
            $scope.notes = data.data.Notes;
            if (data.data.Notes !== undefined)
              $scope.notes.total = data.data.Notes.length;
            else
              $scope.notes.total = 0;

            if ($scope.currentWorkItem.FrequencyId == 2)
              $scope.dateDaysAhead = 9;
            else if ($scope.currentWorkItem.FrequencyId == 3)
              $scope.dateDaysAhead = 16;
            else if ($scope.currentWorkItem.FrequencyId == 4)
              $scope.dateDaysAhead = 34;
            $scope.maxDate = new Date().addDays($scope.dateDaysAhead);

            var _checks = ['DebtorCell', 'DebtorEmail', 'DebtorFax', 'DebtorWorkNo'];
            for (var i = _checks.length - 1; i >= 0; i--) {
              if (angular.isArray($scope.currentWorkItem[_checks[i]]) && $scope.currentWorkItem[_checks[i]].length > 1)
                $scope.currentWorkItem[_checks[i]].IsArray = true;
              else
                $scope.currentWorkItem[_checks[i]].IsArray = false;
            };

            angular.element('#allocateDialog').modal('show');
            $scope.searching = false;
            $scope[callingControl] = false;
          }, function (data) {
            toaster.pop('danger', 'Error', 'There was an error while trying to load the case.');
            $scope.searching = false;
          });
        }

        $scope.saveAction = function (action, date, amount, comment, dialog, cascadeClose, nextItem) {

          $scope.saving = true;

          if (amount)
            amount = Number(amount.replace(/[^0-9\.]+/g, ""));

          streamResource.save($scope.currentWorkItem.CaseStreamId, $scope.currentWorkItem.CaseStreamActionId, userId, action, date, amount, comment, $scope.token).then(function (data) {
            $scope.saving = false;
            angular.element('#' + dialog).modal('hide');
            if (cascadeClose) angular.element('#allocateDialog').modal('hide');
          }, function (data) {
            toaster.pop('danger', 'Error', data.data && data.data.ExceptionMessage ? data.data.ExceptionMessage : 'An Error as occured');
            $scope.saving = false;
          });
        };

        $scope.transferMultipleToConsultant = function (consultant) {
          debugger;
          $scope.saving = true;
          $scope.transferringMulti = "Transferring Case(s)";

          var caseStreams = [];
          $.each($scope.streamData, function (index, value) {
            if (value.transfer === true) {
              var transferObj = {
                "CaseStreamId": value.CaseStreamId,
                "UserId": userId,
                "CurrentUserId": value.AllocatedUserId,
                "NewUserId": consultant
              };
              caseStreams.push(transferObj);
            }
          });

          streamResource.transferMultipleCaseStreamsTo(caseStreams, $scope.token).then(function (data) {
            $scope.saving = false;
            $scope.transferringMulti = "Transfer to";
            $scope.search();
            if (data.status === 500) {
              toaster.pop('danger', 'Error', data.data && data.data.ExceptionMessage ? data.data.ExceptionMessage : 'An Error as occured');
            }
          }, function (data) {
            toaster.pop('danger', 'Transfer to', 'A error courred while trying to transfer to the desired user!');
            $scope.saving = false;
            $scope.transferringMulti = "Transfer to";
          });
        };

        $scope.multiTransferSelected = function () {
          $scope.transferMultiButtonView = false;
          $.each($scope.streamData, function (index, value) {
            if (value.transfer === true) {
              $scope.transferMultiButtonView = true;
              return false; // break loop
            }
          });
        }

        $scope.transferTo = function () {
          $scope.saving = true;
          streamResource.transferTo($scope.currentWorkItem.CaseStreamId, userId, $scope.currentWorkItem.AllocatedUserId, $scope.transferToConsultant, $scope.token).then(function (data) {
            $scope.saving = false;
            angular.element('#transferCaseTo').modal('hide');
            angular.element('#allocateDialog').modal('hide');
            $scope.search();
          }, function (data) {
            toaster.pop('danger', 'Error', data.data ? data.data.ExceptionMessage : 'A error courred while trying to transfer to the desired user!');
            $scope.saving = false;
          });
        }

        $scope.saveTransferBack = function () {
          $scope.saving = true;
          streamResource.completeEscalation($scope.currentWorkItem.CaseStreamId, userId, $scope.transferBackEscalationReason, $scope.token).then(function (data) {
            $scope.saving = false;
            angular.element('#transferBackEscalation').modal('hide');
            angular.element('#allocateDialog').modal('hide');
            _getEscalatedItems();
          }, function (data) {
            toaster.pop('danger', 'Escalate Transfer', data.data ? data.data.ExceptionMessage : 'A error courred while trying to complete the escalation!');
            $scope.saving = false;
          });
        }

        $scope.search = function () {
          var offset = $scope.params.offset || 0;
          $scope.escalatedView = false;
          $scope.manualSearch = true;
          streamResource.getWorkItems(null, $scope.consultant, $scope.branch, $scope.caseNo, $scope.idNumber, $scope.accountReferenceNo, $scope.filterActionStartDate, $scope.filterActionEndDate, $scope.filterStreamType, $scope.categoryType, null, $scope.token, $scope.filterCaseStatus, $scope.filterCreateStartDate, $scope.filterCreateEndDate).then(function (data) {
            $scope.streamData = data.data;
            $scope.params.total = data.data.length;
            $scope.manualSearch = false;
          }, function (data) {
            toaster.pop('danger', 'Search', data.data ? data.data.ExceptionMessage : 'A search error has occurred!');
            $scope.manualSearch = false;
          });
        };

        var _getEscalatedItems = function () {
          $scope.escalatedView = true;
          var offset = $scope.params.offset || 0;
          $scope.manualSearch = true;
          streamResource.getEscalatedItems(userId, branchId).then(function (data) {
            debugger;
            $scope.streamData = data.data;
            $scope.params.total = data.data.length;
            $scope.manualSearch = false;
            ngProgress.complete();
          }, function (data) {
            toaster.pop('danger', 'Search', data.data ? data.data.ExceptionMessage : 'A search error has occurred!');
            $scope.manualSearch = false;
          });
        }

        $scope.syncPayment = function () {
          $scope.synch = true;
          streamResource.syncPayment($scope.currentWorkItem.CaseStreamId).then(function (data) {
            $scope.synch = false;
            if (data.data === 'Pending')
              toaster.pop('info', 'Payments', 'No payments found');
            else
              toaster.pop('info', 'Payments', 'Payments synched');

          }, function (status) {
            toaster.pop('danger', 'Sync Payment', status.data ? status.data.ExceptionMessage : 'An error occurred while trying to sync the payment.');
            $scope.synch = false;
          })

        }

        var _getBranches = function () {
          $scope.branchLoading = true;
          userResource.getLinkedBranches(userId).then(function (data) {
            debugger;
            $scope.branches = data.data;
            if ($scope.branches.length === 1)
              $scope.branch = $scope.branches[0];
            else
              try {
                $scope.branch = parseInt(branchId);
                $scope.getConsultants();
              }
              catch (err) {
              }
            $scope.branchLoading = false;
          }, function (data) {
            toaster.pop('danger', 'Branches', data.data ? data.data.ExceptionMessage : 'There was an error while trying to load the Branches!');
          });
        }

        $scope.getConsultants = function () {
          $scope.consultantLoading = true;
          userResource.getConsultants($scope.branch).then(function (data) {
            $scope.consultants = data.data;
            $scope.consultantLoading = false;
          }, function (data) {
            toaster.pop('danger', 'Consultants', data.data ? data.data.ExceptionMessage : 'There was an error while trying to load the consultants!');
          });
        }


        $scope.init = function () {
          ngProgress.start();
          getStaticData($scope.categoryType);
          _getBranches();
          _getEscalatedItems();
        }

        $scope.selectCategory = function (category) {
          //alert(category);
        }

        $scope.$on('$destroy', function () {

        });

        $scope.printHistory = function () {
          $('#history').printThis({ importCSS: true, importStyle: true });
        };

        $scope.printResults = function () {
          $('#resultsGrid').printThis({ importCSS: true, importStyle: true });
        };
      }
  ]);


}(this.angular));