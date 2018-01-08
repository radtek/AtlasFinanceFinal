(function (angular) {
  'use strict';
  var app = angular.module('falcon');

  app.controller('StreamReportController', ['$scope', '$filter', 'StreamReportingResource', 'StreamResource', '$localStorage', 'toaster', '$rootScope', '$sce',
      function ($scope, $filter, reportingResource, streamResource, localStorage, toaster, $rootScope, $sce) {
        Globalize.culture("en-ZA");

        $rootScope.$broadcast('menu', {
          name: 'Reports',
          Desc: 'Stream Performance Report',
          url: $sce.trustAsUrl('/#!/Report/Stream/'),
          searchVisible: true
        });

        $scope.isLoaded = false;
        $scope.btnExportAccountsText = $scope.btnExportText = "Export";
        $scope.btnApplyText = "Apply";
        $scope.selectedReport = "collections";
        $scope.notes = {};

        var date = $filter('date')(new Date(), 'yyyy-MM-dd');
        $scope.filterStartDate = $scope.filterStartDate || date;
        $scope.filterEndDate = $scope.filterEndDate || date;

        $scope.drillDownLevel = 1;
        $scope.PTPPTC = "PTPs";

        var filterGroup = 1;
        $scope.headerColumn = "Accounts";
        var drillDownGroup = [];
        var drillDownRegionId = [];
        var drillDownCategoryId = [];
        var drillDownBranchId = [];
        var drillDownStartDate = [];
        var drillDownEndDate = [];

        $scope.showCreated = true;
        $scope.showDues = $scope.showFollowUps = $scope.showActions = false;

        $scope.rBranches = [{
          Name: 'Loading',
          Ticked: false
        }];
        var _getFilterData = function () {
          reportingResource.getFilterData($scope.token).then(function (result) {
            if (result.status === 200) {
              $scope.branches = result.data.branches;
              $scope.regions = result.data.regions;
              $scope.selectedRegionBranches = [];

              $scope.rBranches = [];
              $scope.groupTypes = result.data.groupTypes;
              
              $scope.isLoaded = true;
            } else {
              toaster.pop('danger', "Loading Filter Data", "Error loading Filter Data!");
              console.error(result.statusText);
            }
          }, function (result) {
            toaster.pop('danger', "Loading Filter Data", "Error loading Filter Data!");
            console.error(result.statusText);
          });
        };


        if (!Array.prototype.remove) {
          Array.prototype.remove = function (val) {
            var i = this.indexOf(val);
            return i > -1 ? this.splice(i, 1) : [];
          };
        }

        $scope.regionSelected = function (data) {
          if (data.Ticked === true) {
            angular.forEach($scope.branches, function (branch, branckKey) {
              if (branch.RegionId === data.RegionId) {
                $scope.selectedRegionBranches.push(branch);
              }
            });
          } else {
            for (var i = 0; i < $scope.selectedRegionBranches.length; i++) {
              if ($scope.selectedRegionBranches[i].RegionId === data.RegionId) {
                $scope.selectedRegionBranches.remove($scope.selectedRegionBranches[i]);
                i--;
              }
            }
          }
        };

        $scope.regionSelectAll = function () {
          $scope.selectedRegionBranches = $scope.branches;
        };

        $scope.regionClearBranches = function () {
          alert('clear');
          $scope.selectedRegionBranches = [];
        };

        var _populateFilterBranch = function () {
          $scope.filterBranch = [];
          angular.forEach($scope.selectedBranches, function (value, key) {
            if (value.Ticked === true && value.BranchId) {
              $scope.filterBranch.push(value.BranchId);
            }
          });
        }

        $scope.init = function (personId) {
          $scope.personId = personId;
          _getFilterData();
        };

        $scope.refresh = function () {
          $scope.disableButtons = true;
          $scope.btnApplyText = "Processing..";
          _populateFilterBranch();
        };

        $scope.export = function () {
          $scope.disableButtons = true;
          $scope.btnExportText = "Processing..";

          _populateFilterBranch();

          reportingResource.getExportFile($scope.filterGroup, $scope.filterBranch, $scope.filterStartDate, $scope.filterEndDate, $scope.token).then(function (result) {
            if (result.status === 200) {
              open(result.data.response, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
              //open(result.dat.responsea, "application/vnd.ms-excel");
              $scope.disableButtons = false;
              $scope.btnExportText = "Export";
            } else {
              toaster.pop('danger', "", "!");
              console.error(result);
              $scope.disableButtons = false;
              $scope.btnExportText = "Export";
            }
          }, function (result) {
            toaster.pop('danger', "", "!");
            console.error(result);
            $scope.disableButtons = false;
            $scope.btnExportText = "Export";
          });
        };

        $scope.exportAccounts = function () {
          $scope.disableButtons = true;
          $scope.btnExportAccountsText = "Processing..";

          reportingResource.getAccountExportFile($scope.drilldownAccountGroup, $scope.drilldownAccountRegion, $scope.drilldownAccountCategoryId,
              $scope.drilldownAccountBranch, $scope.drilldownAccountMonth, $scope.drilldownAccountYear, $scope.drilldownAccountAllocatedUserId,
              $scope.drilldownAccountColIndex, $scope.token).then(function (result) {
                if (result.status === 200) {
                  open(result.data.response, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                  //open(result.dat.responsea, "application/vnd.ms-excel");
                  $scope.disableButtons = false;
                  $scope.btnExportAccountsText = "Export";
                } else {
                  toaster.pop('danger', "", "!");
                  console.error(result);
                  $scope.disableButtons = false;
                  $scope.btnExportAccountsText = "Export";
                }
              }, function (result) {
                toaster.pop('danger', "", "!");
                console.error(result);
                $scope.disableButtons = false;
                $scope.btnExportAccountsText = "Export";
              });
        };

        function open(strData, strMimeType) {
          var newdata = "data:" + strMimeType + ";base64," + encodeURI(strData);
          //To open in new window
          window.open(newdata, "_blank");
          return true;
        }

        var _getAccountData = function (filterGroup, filterRegion, filterCategoryId, filterBranch, filterStartDate, filterEndDate, drillDownLevel) {
          $scope.disableButtons = true;

          drillDownGroup[drillDownLevel] = filterGroup;
          drillDownRegionId[drillDownLevel] = filterRegion;
          drillDownCategoryId[drillDownLevel] = filterCategoryId;
          drillDownBranchId[drillDownLevel] = filterBranch;
          drillDownStartDate[drillDownLevel] = filterStartDate;
          drillDownEndDate[drillDownLevel] = filterEndDate;

          $scope.btnApplyText = "Processing..";
          reportingResource.getAccountData(filterGroup, filterRegion, filterCategoryId, filterBranch, filterStartDate, filterEndDate, drillDownLevel, $scope.token).then(function (result) {
            if (result.status === 200) {
              $scope.accountData = result.data.accountData;
            } else {
              toaster.pop('danger', "Loading Account Data", "Error loading Account Data!");
              console.error(result.statusText);
            }
            $scope.disableButtons = false;
            $scope.btnApplyText = "Apply";
          }, function (result) {
            toaster.pop('danger', "", "!");
            console.error(result);
            $scope.disableButtons = false;
            $scope.btnApplyText = "Apply";
          });
        }

        var _getAccounts = function (filterGroup, filterRegion, filterCategoryId, filterSubCategoryId, filterBranch, filterStartDate, filterEndDate, allocatedUserId, colIndex) {
          $scope.drilldownAccountGroup = filterGroup;
          $scope.drilldownAccountRegion = filterRegion;
          $scope.drilldownAccountCategoryId = filterCategoryId;
          $scope.drilldownAccountSubCategoryId = filterSubCategoryId;
          $scope.drilldownAccountBranch = filterBranch;
          $scope.drilldownAccountMonth = filterStartDate;
          $scope.drilldownAccountYear = filterEndDate;
          $scope.drilldownAccountAllocatedUserId = allocatedUserId;
          $scope.drilldownAccountColIndex = colIndex;

          $scope.disableButtons = true;
          $scope.streamData = [];
          $scope.btnApplyText = "Processing..";
          reportingResource.getAccounts(filterGroup, filterRegion, filterCategoryId, filterSubCategoryId, filterBranch, filterStartDate, filterEndDate, allocatedUserId, colIndex, $scope.token).then(function (result) {
            if (result.status === 200) {
              $scope.streamData = result.data.streamData;
            } else {
              toaster.pop('danger', "Loading Account Data", "Error loading Account Data!");
              console.error(result.statusText);
            }
            $scope.disableButtons = false;
            $scope.btnApplyText = "Apply";
          }, function (result) {
            toaster.pop('danger', "", "!");
            console.error(result);
            $scope.disableButtons = false;
            $scope.btnApplyText = "Apply";
          });
        }

        $scope.apply = function () {
          $scope.regionsDrilldownfilter = "";
          $scope.branchesDrilldownfilter = "";
          $scope.usersDrilldownfilter = "";
          $scope.drillDownLevel = 1; // resets drill down

          filterGroup = $scope.filterGroup;
          $scope.PTPPTC = filterGroup === 1 ? "PTPs" : "PTCs";

          _populateFilterBranch();
          _getAccountData(filterGroup, 0, 0, $scope.filterBranch, $scope.filterStartDate, $scope.filterEndDate, $scope.drillDownLevel);
        }

        $scope.drillDown = function (line) {
          if ($scope.drillDownLevel < 2) {
            $scope.drillDownLevel++;
            if ($scope.drillDownLevel === 2) {
              // branch selected
              $scope.regionsDrilldownfilter = "[" + line.Region;
              if (line.CategoryId != 0)
                $scope.regionsDrilldownfilter += " - " + line.Category;
              $scope.regionsDrilldownfilter += "]";
            }
            //else if ($scope.drillDownLevel === 3) {
            //  // branch selected
            //  $scope.branchesDrilldownfilter = "[" + line.Branch;
            //  if (line.CategoryId != 0 && $scope.regionsDrilldownfilter.indexOf(line.Category) === -1)
            //    $scope.branchesDrilldownfilter += " - " + line.Category;
            //  $scope.branchesDrilldownfilter += "]";
            //}

            _getAccountData(filterGroup, line.RegionId, line.CategoryId, line.BranchId === 0 ? $scope.filterBranch : [line.BranchId], $scope.filterStartDate, $scope.filterEndDate, $scope.drillDownLevel);
          }
        };

        $scope.drillDownToAccounts = function (line, column) {
          if ($scope.drillDownLevel >= 2) {
            $scope.usersDrilldownfilter = "[" + line.AllocatedUser;
            if (line.CategoryId != 0 && $scope.regionsDrilldownfilter.indexOf(line.Category) === -1 && $scope.branchesDrilldownfilter.indexOf(line.Category) === -1)
              $scope.usersDrilldownfilter += " - " + line.Category;
            $scope.usersDrilldownfilter += "]";

            $scope.drillDownLevel++;

            _getAccounts(filterGroup, line.RegionId,
                line.CategoryId, line.SubCategoryId, line.BranchId === 0 ? $scope.filterBranch : [line.BranchId], $scope.filterStartDate,
                $scope.filterEndDate, line.AllocatedUserId, column);
          }
        };

        $scope.fastNavigateDrilldown = function (drillDownLevel) {
          //if (drillDownLevel === 3) {
          //  $scope.usersDrilldownfilter = "";
          //} else
          if (drillDownLevel === 2) {
            $scope.branchesDrilldownfilter = "";
            $scope.usersDrilldownfilter = "";
          } else if (drillDownLevel === 1) {
            $scope.regionsDrilldownfilter = "";
            $scope.branchesDrilldownfilter = "";
            $scope.usersDrilldownfilter = "";
          }

          $scope.drillDownLevel = drillDownLevel;

          if (drillDownGroup[drillDownLevel])
            _getAccountData(drillDownGroup[drillDownLevel], drillDownRegionId[drillDownLevel], drillDownCategoryId[drillDownLevel], drillDownBranchId[drillDownLevel],
                drillDownStartDate[drillDownLevel], drillDownEndDate[drillDownLevel], drillDownLevel);
        };

        // Shows/Hides columns when primary header is clicked
        $scope.changeView = function (toView) {
          $scope.showCreated = $scope.showDues = $scope.showFollowUps = $scope.showActions = false;
          switch (toView) {
            case 'created': $scope.showCreated = true;
              $scope.headerColumn = "Accounts";
              break;
            case 'dues': $scope.showDues = true;
              $scope.headerColumn = $scope.PTPPTC;
              break;
            case 'results': $scope.showFollowUps = true;
              $scope.headerColumn = "Results";
              break;
            case 'actions': $scope.showActions = true;
              $scope.headerColumn = "Actions";
              break;
            default:
              break;
          }
        }

        $scope.viewHistory = function (item) {
          _setCurrentWorkItem(item)
          $scope.isManagedItem = true;
        }

        var _setCurrentWorkItem = function (workItem) {

          $scope.disableButtons = true;
          streamResource.getNoteHistory(workItem.CaseId).then(function (data) {
            var offset = $scope.notes.offset || 0;
            if (data !== 'Empty') {
              $scope.currentWorkItem = workItem;
              $scope.previousCompleteNote = data.data.PreviousCaseCompleteNote;

              $scope.date = undefined;
              $scope.amount = undefined;
              $scope.reason = undefined;
              $scope.notes = data.data.Notes;
              if (data.data.Notes !== undefined)
                $scope.notes.total = data.data.Notes.length;
              else
                $scope.notes.total = 0;

              angular.element('#allocateDialog').modal('show');
              $scope.disableButtons = false;
            }
          },
              function (data) {
                toaster.pop('danger', 'Error', 'There was an error while trying to load the case.');
                $scope.disableButtons = false;
              })
        };
      }
  ]);

  app.controller('StreamDetailReportController', [
    '$scope', '$filter', 'StreamReportingResource', 'StreamResource', '$localStorage', 'toaster', '$rootScope', '$sce',
    function($scope, $filter, reportingResource, streamResource, localStorage, toaster, $rootScope, $sce) {
      Globalize.culture("en-ZA");

      $rootScope.$broadcast('menu', {
        name: 'Reports',
        Desc: 'Stream Performance Report',
        url: $sce.trustAsUrl('/#!/Report/Stream/Detail/'),
        searchVisible: true
      });

      $scope.isLoaded = false;
      $scope.btnExportText = "Export";
      $scope.selectedReport = "collections";
      $scope.notes = {};

      var _getFilterData = function () {
        reportingResource.getFilterData($scope.token).then(function (result) {
          if (result.status === 200) {
            $scope.branches = result.data.branches;
            $scope.regions = result.data.regions;
            $scope.selectedRegionBranches = [];

            $scope.groupTypes = result.data.groupTypes;
            $scope.streamTypes = [];
            $scope.caseStatuses = [];

            for (var i = 0; i < result.data.caseStatuses.length; i++) {
              if (result.data.caseStatuses[i].CaseStatusId <= 3) {
                $scope.caseStatuses.push({
                  CaseStatusId: result.data.caseStatuses[i].CaseStatusId,
                  Description: result.data.caseStatuses[i].Description,
                  Ticked: true
                });
              }
            }
            for (var i = 0; i < result.data.streamTypes.length; i++) {
              $scope.streamTypes.push({
                StreamId: result.data.streamTypes[i].StreamId,
                Description: result.data.streamTypes[i].Description,
                Ticked: true
              });
            }

            $scope.isLoaded = true;
          } else {
            toaster.pop('danger', "Loading Filter Data", "Error loading Filter Data!");
            console.error(result.statusText);
          }
        }, function (result) {
          toaster.pop('danger', "Loading Filter Data", "Error loading Filter Data!");
          console.error(result.statusText);
        });
      };

      function open(strData, strMimeType) {
        var newdata = "data:" + strMimeType + ";base64," + encodeURI(strData);
        //To open in new window
        window.open(newdata, "_blank");
        return true;
      }

      $scope.init = function (personId) {
        $scope.personId = personId;
        _getFilterData();
      };

      var _populateFilterStreamIds = function () {
        $scope.filterStreamIds = [];
        angular.forEach($scope.streamTypes, function (value, key) {
          if (value.Ticked === true && value.StreamId) {
            $scope.filterStreamIds.push(value.StreamId);
          }
        });
      }

      var _populateFilterCaseStatusIds = function () {
        $scope.filterCaseStatusIds = [];
        angular.forEach($scope.caseStatuses, function (value, key) {
          if (value.Ticked === true && value.CaseStatusId) {
            $scope.filterCaseStatusIds.push(value.CaseStatusId);
          }
        });
      }

      $scope.export = function () {
        $scope.disableButtons = true;
        $scope.btnExportText = "Processing..";

        _populateFilterStreamIds();
        _populateFilterCaseStatusIds();

        reportingResource.getDetailExportFile($scope.filterGroup, $scope.filterBranch, $scope.filterStreamIds, $scope.filterCaseStatusIds, $scope.token).then(function (result) {
          if (result.status === 200) {
            open(result.data.response, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            //open(result.dat.responsea, "application/vnd.ms-excel");
            $scope.disableButtons = false;
            $scope.btnExportText = "Export";
          } else {
            toaster.pop('danger', result.data.Message, result.data.ExceptionMessage);
            console.error(result);
            $scope.disableButtons = false;
            $scope.btnExportText = "Export";
          }
        }, function (result) {
          toaster.pop('danger', result.data.Message, result.data.ExceptionMessage);
          console.error(result);
          $scope.disableButtons = false;
          $scope.btnExportText = "Export";
        });
      };
    }
  ]);
}(this.angular));