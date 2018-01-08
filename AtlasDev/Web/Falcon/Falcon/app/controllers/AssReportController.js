(function (angular) {
  'use strict';

  var app = angular.module('falcon');

  app.controller('AssReportController', ['$scope', '$filter', 'AssReportingResource', '$localStorage', 'toaster',
      function ($scope, $filter, reportingResource, localStorage, toaster) {

        var date = $filter('date')(new Date(), 'yyyy-MM-dd');
        $scope.isLoaded = false;
        $scope.isAmountChart = true;
        $scope.filterStartDate = $scope.filterStartDate || date;
        $scope.filterEndDate = $scope.filterEndDate || date;

        var _getRegions = function () {
          reportingResource.getRegions($scope.personId, $scope.token).then(function (result) {
            if (result.status === 200) {
              $scope.regions = result.data.regions;
              $scope.isLoaded = true;
            } else {
              toaster.pop('danger', "Loading Regions", "Error loading Regions!");
              console.error(result);
            }
          });
        };

        var _getBranches = function (regionIds) {
          var _addBranches = function (branches) {
            if (branches.length > 0) {
              if ($scope.branches == undefined)
                $scope.branches = branches;
              else
                $scope.branches = $scope.branches.concat(branches);
            }
          };

          $scope.branches = undefined;
          for (var i = 0; i < regionIds.length; i++) {
            var tempBranches = JSON.parse(localStorage.getItem('Falcon_Ass_Region_' + regionIds[i] + '_branches'));

            if (tempBranches == undefined) {
              reportingResource.getBranches(regionIds[i], $scope.token).then(function (result) {
                if (result.status === 200) {
                  localStorage.setItem('Falcon_Ass_Region_' + regionIds[i] + '_branches', JSON.stringify(result.data.Branches));
                  _addBranches(result.data.branches);
                } else {
                  toaster.pop('danger', "Loading Branches", "Error loading Branches!");
                  console.error(result);
                }
              });
            }
            else {
              _addBranches(tempBranches);
            }
          }
        };

        var loadCharts = function () {
          $scope.loanmixChartSettings = {
            dataSource: new DevExpress.data.DataSource({
              load: function () {
                var def = $.Deferred();
                reportingResource.getLoanMix($scope.filterRegion, $scope.filterBranch, $scope.filterStartDate, $scope.filterEndDate, $scope.token).then(function (result) {
                  if (result.status === 200) {
                    def.resolve(result.data.response);
                  } else {
                    toaster.pop('danger', "", "!");
                    console.error(result);
                  }
                });
                return def.promise();
              }
            }),
            commonSeriesSettings: {
              argumentField: 'BranchName',
              type: 'bar',
              axis: 'quantityAxis',
              label: {
                format: 'largeNumber'
              }
            },
            tooltip: {
              enabled: true
            },
            valueAxis: [{
              name: 'quantityAxis',
              title: {
                text: 'Monthly Quantity'
              }
            }, {
              name: 'mixAxis',
              position: 'right',
              title: {
                text: 'Loan Mix',
                format: 'percent'
              }
            }],
            series: [
                { valueField: 'PayNoLoan1', name: '1 Month' },
                { valueField: 'PayNoLoan2', name: '2 Months' },
                { valueField: 'PayNoLoan3', name: '3 Months' },
                { valueField: 'PayNoLoan4', name: '4 Months' },
                { valueField: 'PayNoLoan5', name: '5 Months' },
                { valueField: 'PayNoLoan6', name: '6 Months' },
                { valueField: 'PayNoLoan12', name: '12 Months' },
                { valueField: 'PayNoLoan24', name: '24 Months' },
                {
                  valueField: 'PayNoLoanTot',
                  color: 'limegreen',
                  name: 'Total Loans',
                  type: 'scatter'
                },
                {
                  axis: 'mixAxis',
                  type: 'spline',
                  valueField: 'PayNoLoanMix1',
                  name: 'Loan Mix - Short',
                  label: {
                    format: 'largeNumber'
                  }
                },
                {
                  axis: 'mixAxis',
                  type: 'spline',
                  valueField: 'PayNoLoanMixNot1',
                  name: 'Loan mix - Long',
                  label: {
                    format: 'largeNumber'
                  }
                }
            ],
            legend: {
              horizontalAlignment: 'center',
              verticalAlignment: 'bottom'
            }
          };
          $scope.interestChartSettings = {
            dataSource: new DevExpress.data.DataSource({
              load: function () {
                var def = $.Deferred();
                reportingResource.getInterest($scope.filterRegion, $scope.filterBranch, $scope.filterStartDate, $scope.filterEndDate, $scope.token).then(function (result) {
                  if (result.status === 200) {
                    def.resolve(result.data.response);
                  } else {
                    toaster.pop('danger', "", "!");
                    console.error(result);
                  }
                });
                return def.promise();
              }
            }),
            commonAxisSettings: {
              label: {
                format: 'currency'
              }
            },
            commonSeriesSettings: {
              argumentField: 'BranchName',
              type: 'bar',
              axis: 'monthlyAxis',
              label: {
                format: 'largeNumber'
              }
            },
            tooltip: {
              enabled: true,
              format: 'currency'
            },
            valueAxis: [{
              name: 'monthlyAxis',
              title: {
                text: 'Monthly Amount'
              }
            }, {
              name: 'totalAxis',
              position: 'right',
              title: {
                text: 'Total Amount'
              }
            }],
            series: [
                { valueField: 'PayNoInterest1', name: '1 Month' },
                { valueField: 'PayNoInterest2', name: '2 Months' },
                { valueField: 'PayNoInterest3', name: '3 Months' },
                { valueField: 'PayNoInterest4', name: '4 Months' },
                { valueField: 'PayNoInterest5', name: '5 Months' },
                { valueField: 'PayNoInterest6', name: '6 Months' },
                { valueField: 'PayNoInterest12', name: '12 Months' },
                { valueField: 'PayNoInterest24', name: '24 Months' },
                {
                  axis: 'totalAxis',
                  type: 'spline',
                  valueField: 'PayNoInterestTot',
                  color: 'limegreen',
                  name: 'Total'
                }
            ],
            legend: {
              horizontalAlignment: 'center',
              verticalAlignment: 'bottom'
            }
          };
          $scope.insuranceChartSettings = {
            dataSource: new DevExpress.data.DataSource({
              load: function () {
                var def = $.Deferred();
                reportingResource.getInsurance($scope.filterRegion, $scope.filterBranch, $scope.filterStartDate, $scope.filterEndDate, $scope.token).then(function (result) {
                  if (result.status === 200) {
                    def.resolve(result.data.response);
                  } else {
                    toaster.pop('danger', "", "!");
                    console.error(result);
                  }
                });
                return def.promise();
              }
            }),
            commonAxisSettings: {
              label: {
                format: 'currency'
              }
            },
            commonSeriesSettings: {
              argumentField: 'BranchName',
              type: 'bar',
              axis: 'monthlyAxis',
              label: {
                format: 'largeNumber'
              }
            },
            tooltip: {
              enabled: true,
              format: 'currency'
            },
            valueAxis: [{
              name: 'monthlyAxis',
              title: {
                text: 'Monthly Amount'
              }
            }, {
              name: 'totalAxis',
              position: 'right',
              title: {
                text: 'Total Amount'
              }
            }],
            series: [
                { valueField: 'PayNoInsurance1', name: '1 Month' },
                { valueField: 'PayNoInsurance2', name: '2 Months' },
                { valueField: 'PayNoInsurance3', name: '3 Months' },
                { valueField: 'PayNoInsurance4', name: '4 Months' },
                { valueField: 'PayNoInsurance5', name: '5 Months' },
                { valueField: 'PayNoInsurance6', name: '6 Months' },
                { valueField: 'PayNoInsurance12', name: '12 Months' },
                { valueField: 'PayNoInsurance24', name: '24 Months' },
                {
                  axis: 'totalAxis',
                  type: 'spline',
                  valueField: 'PayNoInsuranceTot',
                  color: 'limegreen',
                  name: 'Total'
                }
            ],
            legend: {
              horizontalAlignment: 'center',
              verticalAlignment: 'bottom'
            }
          };
          $scope.interestPercentChartSettings = {
            dataSource: new DevExpress.data.DataSource({
              load: function () {
                var def = $.Deferred();
                reportingResource.getInterestPercentile($scope.filterRegion, $scope.filterBranch, $scope.filterStartDate, $scope.filterEndDate, $scope.token).then(function (result) {
                  if (result.status === 200) {
                    def.resolve(result.data.response);
                  } else {
                    toaster.pop('danger', "", "!");
                    console.error(result);
                  }
                });
                return def.promise();
              }
            }),
            commonSeriesSettings: {
              argumentField: 'BranchName',
              type: 'fullStackedBar',
              axis: 'monthlyAxis',
              label: {
                format: 'percent'
              }
            },
            tooltip: {
              enabled: true,
              format: 'percent'
            },
            valueAxis: {
              name: 'monthlyAxis',
              title: {
                text: 'Cheque / Interest'
              }
            },
            series: [
                { valueField: 'PayNoInterest1', name: '1 Month' },
                { valueField: 'PayNoInterest2', name: '2 Months' },
                { valueField: 'PayNoInterest3', name: '3 Months' },
                { valueField: 'PayNoInterest4', name: '4 Months' },
                { valueField: 'PayNoInterest5', name: '5 Months' },
                { valueField: 'PayNoInterest6', name: '6 Months' },
                { valueField: 'PayNoInterest12', name: '12 Months' },
                { valueField: 'PayNoInterest24', name: '24 Months' },
                { type: 'spline', valueField: 'PayNoInterestTot', name: 'Total', color: 'limegreen' }
            ],
            legend: {
              horizontalAlignment: 'center',
              verticalAlignment: 'bottom'
            }
          };
          $scope.insurancePercentChartSettings = {
            dataSource: new DevExpress.data.DataSource({
              load: function () {
                var def = $.Deferred();
                reportingResource.getInsurancePercentile($scope.filterRegion, $scope.filterBranch, $scope.filterStartDate, $scope.filterEndDate, $scope.token).then(function (result) {
                  if (result.status === 200) {
                    def.resolve(result.data.response);
                  } else {
                    toaster.pop('danger', "", "!");
                    console.error(result);
                  }
                });
                return def.promise();
              }
            }),
            commonSeriesSettings: {
              argumentField: 'BranchName',
              type: 'fullStackedBar',
              axis: 'monthlyAxis',
              label: {
                format: 'percent'
              }
            },
            tooltip: {
              enabled: true,
              format: 'percent'
            },
            valueAxis: {
              name: 'monthlyAxis',
              title: {
                text: 'Cheque / Insurance'
              }
            },
            series: [
                { valueField: 'PayNoInsurance1', name: '1 Month' },
                { valueField: 'PayNoInsurance2', name: '2 Months' },
                { valueField: 'PayNoInsurance3', name: '3 Months' },
                { valueField: 'PayNoInsurance4', name: '4 Months' },
                { valueField: 'PayNoInsurance5', name: '5 Months' },
                { valueField: 'PayNoInsurance6', name: '6 Months' },
                { valueField: 'PayNoInsurance12', name: '12 Months' },
                { valueField: 'PayNoInsurance24', name: '24 Months' },
                { type: 'spline', valueField: 'PayNoInsuranceTot', name: 'Total', color: 'limegreen' }
            ],
            legend: {
              horizontalAlignment: 'center',
              verticalAlignment: 'bottom'
            }
          };
          $scope.chequeChartSettings = {
            dataSource: new DevExpress.data.DataSource({
              load: function () {
                var def = $.Deferred();
                reportingResource.getCheque($scope.filterRegion, $scope.filterBranch, $scope.filterStartDate, $scope.filterEndDate, $scope.token).then(function (result) {
                  if (result.status === 200) {
                    def.resolve(result.data.response);
                  } else {
                    toaster.pop('danger', "", "!");
                    console.error(result);
                  }
                });
                return def.promise();
              }
            }),
            commonAxisSettings: {
              label: {
                format: 'currency'
              }
            },
            commonSeriesSettings: {
              argumentField: 'BranchName',
              type: 'bar',
              axis: 'monthlyAxis',
              label: {
                format: 'largeNumber'
              }
            },
            tooltip: {
              enabled: true,
              format: 'currency'
            },
            valueAxis: [{
              name: 'monthlyAxis',
              title: {
                text: 'Monthly Amount'
              }
            }, {
              name: 'totalAxis',
              position: 'right',
              title: {
                text: 'Total Amount'
              }
            }],
            series: [
                { valueField: 'PayNoCheque1', name: '1 Month' },
                { valueField: 'PayNoCheque2', name: '2 Months' },
                { valueField: 'PayNoCheque3', name: '3 Months' },
                { valueField: 'PayNoCheque4', name: '4 Months' },
                { valueField: 'PayNoCheque5', name: '5 Months' },
                { valueField: 'PayNoCheque6', name: '6 Months' },
                { valueField: 'PayNoCheque12', name: '12 Months' },
                { valueField: 'PayNoCheque24', name: '24 Months' },
                {
                  axis: 'totalAxis',
                  type: 'spline',
                  valueField: 'PayNoChequeTot',
                  color: 'limegreen',
                  name: 'Total'
                }
            ],
            legend: {
              horizontalAlignment: 'center',
              verticalAlignment: 'bottom'
            }
          };
          $scope.averageNewClientLoanChartSettings = {
            dataSource: new DevExpress.data.DataSource({
              load: function () {
                var def = $.Deferred();
                reportingResource.getAverageNewClientLoan($scope.filterRegion, $scope.filterBranch, $scope.filterStartDate, $scope.filterEndDate, $scope.token).then(function (result) {
                  if (result.status === 200) {
                    def.resolve(result.data.response);
                  } else {
                    toaster.pop('danger', "", "!");
                    console.error(result);
                  }
                });
                return def.promise();
              }
            }),
            commonAxisSettings: {
              label: {
                format: 'currency'
              }
            },
            commonSeriesSettings: {
              argumentField: 'BranchName',
              type: 'bar',
              axis: 'monthlyAxis',
              label: {
                format: 'largeNumber'
              }
            },
            tooltip: {
              enabled: true,
              format: 'currency'
            },
            valueAxis: {
              name: 'monthlyAxis',
              title: {
                text: 'New Client Loan Amount / New Client Loan quantity'
              }
            },
            series: [
                { valueField: 'PayNoAverageNewCientLoan1', name: '1 Month' },
                { valueField: 'PayNoAverageNewCientLoan2', name: '2 Months' },
                { valueField: 'PayNoAverageNewCientLoan3', name: '3 Months' },
                { valueField: 'PayNoAverageNewCientLoan4', name: '4 Months' },
                { valueField: 'PayNoAverageNewCientLoan5', name: '5 Months' },
                { valueField: 'PayNoAverageNewCientLoan6', name: '6 Months' },
                { valueField: 'PayNoAverageNewCientLoan12', name: '12 Months' },
                { valueField: 'PayNoAverageNewCientLoan24', name: '24 Months' },
                { type: 'spline', valueField: 'PayNoAverageNewCientLoanTot', color: 'limegreen', name: 'Total' }
            ],
            legend: {
              horizontalAlignment: 'center',
              verticalAlignment: 'bottom'
            }
          };
          $scope.averageLoanChartSettings = {
            dataSource: new DevExpress.data.DataSource({
              load: function () {
                var def = $.Deferred();
                reportingResource.getAverageLoan($scope.filterRegion, $scope.filterBranch, $scope.filterStartDate, $scope.filterEndDate, $scope.token).then(function (result) {
                  if (result.status === 200) {
                    def.resolve(result.data.response);
                  } else {
                    toaster.pop('danger', "", "!");
                    console.error(result);
                  }
                });
                return def.promise();
              }
            }),
            commonAxisSettings: {
              label: {
                format: 'currency'
              }
            },
            commonSeriesSettings: {
              argumentField: 'BranchName',
              type: 'bar',
              axis: 'monthlyAxis',
              label: {
                format: 'largeNumber'
              }
            },
            tooltip: {
              enabled: true,
              format: 'currency'
            },
            valueAxis: {
              name: 'monthlyAxis',
              title: {
                text: 'Loan Amount / Loan quantity'
              }
            },
            series: [
                { valueField: 'PayNoAverage1', name: '1 Month' },
                { valueField: 'PayNoAverage2', name: '2 Months' },
                { valueField: 'PayNoAverage3', name: '3 Months' },
                { valueField: 'PayNoAverage4', name: '4 Months' },
                { valueField: 'PayNoAverage5', name: '5 Months' },
                { valueField: 'PayNoAverage6', name: '6 Months' },
                { valueField: 'PayNoAverage12', name: '12 Months' },
                { valueField: 'PayNoAverage24', name: '24 Months' },
                { type: 'spline', valueField: 'PayNoAverageTot', color: 'limegreen', name: 'Total' }
            ],
            legend: {
              horizontalAlignment: 'center',
              verticalAlignment: 'bottom'
            }
          };
        };

        var _updateCharts = function () {
          $scope.chequeChartSettings.dataSource.load();
          $scope.loanmixChartSettings.dataSource.load();
          $scope.interestChartSettings.dataSource.load();
          $scope.insuranceChartSettings.dataSource.load();
          $scope.insurancePercentChartSettings.dataSource.load();
          $scope.interestPercentChartSettings.dataSource.load();
          $scope.averageNewClientLoanChartSettings.dataSource.load();
          $scope.averageLoanChartSettings.dataSource.load();
        };

        $scope.getBranches = function () {
          _getBranches($scope.filterRegion);
        };

        $scope.init = function (personId) {
          $scope.personId = personId;
          $scope.getRegions = _getRegions;
          //loadCharts();
        };

        $scope.refresh = function () {
          //_updateCharts();
        };
      }
  ]);

  app.controller('AssDashboardController', ['$scope', '$filter', 'AssReportingResource', '$localStorage', 'toaster', '$rootScope', '$sce',
      function ($scope, $filter, reportingResource, localStorage, toaster, $rootScope, $sce) {
        Globalize.culture("en-ZA");

        $rootScope.$broadcast('menu', {
          name: 'Reports',
          Desc: 'ci report',
          url: $sce.trustAsUrl('/#!/Dashboard/Ass/'),
          searchVisible: true
        });

        var date = $filter('date')(new Date(), 'yyyy-MM-dd');
        $scope.filterStartDate = $scope.filterStartDate || date;
        $scope.filterEndDate = $scope.filterEndDate || date;
        $scope.isLoaded = false;
        $scope.btnExportText = "Export";
        $scope.btnApplyText = "Apply";
        $scope.selectedReport = "sales";

        $scope.monthNames = [
            { Id: 0, Month: 'Today' },
            { Id: 1, Month: 'January' },
            { Id: 2, Month: 'February' },
            { Id: 3, Month: 'March' },
            { Id: 4, Month: 'April' },
            { Id: 5, Month: 'May' },
            { Id: 6, Month: 'June' },
            { Id: 7, Month: 'July' },
            { Id: 8, Month: 'August' },
            { Id: 9, Month: 'September' },
            { Id: 10, Month: 'October' },
            { Id: 11, Month: 'November' },
            { Id: 12, Month: 'December' }];
        $scope.years = [];
        for (var i = 0; i < 3; i++) {
          var year = 1900 + new Date().getYear() - i;
          $scope.years.push(year);
        }
        $scope.filterMonth = $scope.monthNames[new Date().getMonth() + 1].Id;
        $scope.filterYear = $scope.years[0];

        $scope.rBranches = [{
          Name: 'Loading',
          Ticked: false
        }];
        var _getRegions = function () {
          reportingResource.getRegionBranches($scope.personId, $scope.token).then(function (result) {
            if (result.status === 200) {
              $scope.rBranches = [];
              for (var i = 0; i < result.data.regionBranches.length; i++) {
                if (result.data.regionBranches[i].MultiSelectGroup) {
                  $scope.rBranches.push({
                    BranchId: result.data.regionBranches[i].BranchId,
                    Name: result.data.regionBranches[i].Name,
                    Ticked: result.data.regionBranches[i].Ticked,
                    MultiSelectGroup: true
                  });
                } else if (result.data.regionBranches[i].BranchId === 0) {
                  $scope.rBranches.push({
                    MultiSelectGroup: false
                  });
                }
                else {
                  $scope.rBranches.push({
                    BranchId: result.data.regionBranches[i].BranchId,
                    Name: result.data.regionBranches[i].Name,
                    Ticked: result.data.regionBranches[i].Ticked
                  });
                }
              }
              $scope.isLoaded = true;
            } else {
              toaster.pop('danger', "Loading Regions", "Error loading Regions!");
              console.error(result);
            }
          });
        };

        $scope.monthSelectionChanged = function () {
          $scope.disableYearPicker = ($scope.filterMonth === 0);
        }

        var blankData = [{
          Branch: "",
          BranchName: "",
          PayNo1: 0,
          PayNo2: 0,
          PayNo3: 0,
          PayNo4: 0,
          PayNo5: 0,
          PayNo6: 0,
          PayNo12: 0,
          PayNo24: 0,
          PayNoTot: 0
        }];


        //var saleInterestService = new DevExpress.data.DataSource({
        //    load: function () {
        //        var def = $.Deferred();
        //        reportingResource.getInterest($scope.filterBranch, $scope.filterMonth, $scope.filterYear, $scope.token).then(function (result) {
        //            if (result.status === 200) {
        //                def.resolve(result.data.response);
        //            } else {
        //                toaster.pop('danger', "", "!");
        //                console.error(result);
        //            }
        //        });
        //        return def.promise();
        //    }
        //});

        //var salesChequeService = new DevExpress.data.DataSource({
        //    load: function () {
        //        var def = $.Deferred();
        //        reportingResource.getCheque($scope.filterBranch, $scope.filterMonth, $scope.filterYear, $scope.token).then(function (result) {
        //            if (result.status === 200) {
        //                def.resolve(result.data.response);
        //            } else {
        //                toaster.pop('danger', "", "!");
        //                console.error(result);
        //            }
        //        });
        //        return def.promise();
        //    }
        //});

        //var salesInterestPercentData = new DevExpress.data.DataSource({
        //    load: function () {
        //        var def = $.Deferred();
        //        reportingResource.getInterestPercentile($scope.filterBranch, $scope.filterMonth, $scope.filterYear, $scope.token).then(function (result) {
        //            if (result.status === 200) {
        //                def.resolve(result.data.response);
        //            } else {
        //                toaster.pop('danger', "", "!");
        //                console.error(result);
        //            }
        //        });
        //        return def.promise();
        //    }
        //});

        var loadCharts = function () {
          $scope.amountChartSettings = {
            dataSource: new DevExpress.data.DataSource({
              load: function () {
                var def = $.Deferred();
                reportingResource.getCheque($scope.filterBranch, $scope.filterMonth, $scope.filterYear, $scope.token).then(function (result) {
                  if (result.status === 200) {
                    def.resolve(result.data.response);
                  } else {
                    toaster.pop('danger', "", "!");
                    console.error(result);
                  }
                });
                return def.promise();
              }
            }),
            commonAxisSettings: {
              label: {
                format: 'currency'
              }
            },
            commonSeriesSettings: {
              argumentField: 'BranchName',
              type: 'bar',
              axis: 'monthlyAxis',
              label: {
                format: 'largeNumber'
              }
            },
            tooltip: {
              enabled: true,
              format: 'currency'
            },
            valueAxis: [{
              name: 'monthlyAxis',
              title: {
                text: 'Per Term'
              }
            }, {
              name: 'totalAxis',
              position: 'right',
              title: {
                text: 'Total'
              }
            }],
            series: [
                { valueField: 'PayNo1', name: '1 Month' },
                { valueField: 'PayNo2', name: '2 Months' },
                { valueField: 'PayNo3', name: '3 Months' },
                { valueField: 'PayNo4', name: '4 Months' },
                { valueField: 'PayNo5', name: '5 Months' },
                { valueField: 'PayNo6', name: '6 Months' },
                { valueField: 'PayNo12', name: '12 Months' },
                { valueField: 'PayNo24', name: '24 Months' },
                {
                  axis: 'totalAxis',
                  type: 'spline',
                  valueField: 'PayNoTot',
                  color: 'limegreen',
                  name: 'Total'
                }
            ],
            legend: {
              horizontalAlignment: 'center',
              verticalAlignment: 'bottom'
            }
          };
        };

        var _populateChart = function (runAfter) {
          if ($scope.selectedReport === 'sales') {
            $scope.isAmountChart = true;
            if (localStorage.getItem('chequeData') === null || localStorage.getItem('chequeData') === 'undefined') {
              salesChequeService.load().then(function (response) {
                $scope.$apply(function () {
                  localStorage.setItem('chequeData', JSON.stringify(response));
                  $scope.amountChartSettings.dataSource = response;
                  runAfter();
                });
              });
            }
            else {
              $scope.amountChartSettings.dataSource = JSON.parse(localStorage.getItem('chequeData'));
              runAfter();
            }
          } else {
            $scope.isAmountChart = false;
            runAfter();
          }
        }

        var _resetData = function () {
          localStorage.clear();
        }

        var _populateFilterBranch = function () {
          $scope.filterBranch = [];
          angular.forEach($scope.rBranches, function (value, key) {
            if (value.Ticked === true && value.BranchId) {
              $scope.filterBranch.push(value.BranchId);
            }
          });
        }

        $scope.init = function (personId) {
          $scope.personId = personId;
          _getRegions();
          //loadCharts();
        };

        $scope.refresh = function () {
          $scope.disableButtons = true;
          //$scope.btnApplyText = "Processing..";
          _populateFilterBranch();
          _resetData();
          //_populateChart(function () {
          //    $scope.disableButtons = false;
          //    $scope.btnApplyText = "Apply";
          //});
          $scope.disableButtons = false;
        };

        function open(strData, strMimeType) {
          var newdata = "data:" + strMimeType + ";base64," + encodeURI(strData);
          //To open in new window
          window.open(newdata, "_blank");
          return true;
        }

        $scope.export = function () {
          $scope.disableButtons = true;
          $scope.btnExportText = "Processing..";

          _populateFilterBranch();

          reportingResource.getExportFile($scope.filterBranch, $scope.filterMonth, $scope.filterYear, $scope.token).then(function (result) {
            if (result.status === 200) {
              window.open(result.data.response, "_newtab");
              $scope.disableButtons = false;
              $scope.btnExportText = "Export";
            } else {
              toaster.pop('danger', "", "!");
              console.error(result);
              $scope.disableButtons = false;
              $scope.btnExportText = "Export";
            }
          });
          //reportingResource.getExportFile($scope.filterBranch, $scope.filterStartDate, $scope.filterEndDate, $scope.token).then(function (result) {
          //    if (result.status === 200) {
          //        open(result.data.response, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
          //        //window.open(result.data.response, "_newtab");
          //        $scope.disableButtons = false;
          //        $scope.btnExportText = "Export";
          //    } else {
          //        toaster.pop('danger', "", "!");
          //        console.error(result);
          //        $scope.disableButtons = false;
          //        $scope.btnExportText = "Export";
          //    }
          //});
        };

        $scope.showChart = function (type) {
          $scope.isLoaded = false;
          $scope.selectedReport = type;
          //_populateChart(function () {

          //});
          $scope.isLoaded = true;
        };
      }
  ]);

  app.controller('AssDashboardController_NEW', ['$scope', '$filter', 'AssReportingResource', 'UserResource', '$localStorage', 'toaster', '$rootScope', '$sce',
      function ($scope, $filter, reportingResource, userResource, localStorage, toaster, $rootScope, $sce) {
        Globalize.culture("en-ZA");

        $rootScope.$broadcast('menu', {
          name: 'Reports',
          Desc: 'ci report',
          url: $sce.trustAsUrl('/#!/Dashboard/Ass/'),
          searchVisible: true
        });

        var date = $filter('date')(new Date(), 'yyyy-MM-dd');
        $scope.filterStartDate = $scope.filterStartDate || date;
        $scope.filterEndDate = $scope.filterEndDate || date;
        $scope.isLoaded = false;
        $scope.btnExportText = "Export";
        $scope.btnApplyText = "Apply";
        $scope.selectedReport = "sales";
        $scope.btnEmailText = "Email";
        var userId = userResource.getUserId();

        $scope.monthNames = [
            { Id: 0, Month: 'Today' },
            { Id: 1, Month: 'January' },
            { Id: 2, Month: 'February' },
            { Id: 3, Month: 'March' },
            { Id: 4, Month: 'April' },
            { Id: 5, Month: 'May' },
            { Id: 6, Month: 'June' },
            { Id: 7, Month: 'July' },
            { Id: 8, Month: 'August' },
            { Id: 9, Month: 'September' },
            { Id: 10, Month: 'October' },
            { Id: 11, Month: 'November' },
            { Id: 12, Month: 'December' }];
        $scope.years = [];
        for (var i = 0; i < 3; i++) {
          var year = 1900 + new Date().getYear() - i;
          $scope.years.push(year);
        }
        $scope.filterMonth = $scope.monthNames[new Date().getMonth() + 1].Id;
        $scope.filterYear = $scope.years[0];

        $scope.rBranches = [{
          Name: 'Loading',
          Ticked: false
        }];
        var _getRegions = function () {
          reportingResource.getFilterData($scope.token).then(function (result) {
            if (result.status === 200) {
              $scope.branches = result.data.branches;
              $scope.regions = result.data.regions;
              $scope.selectedRegionBranches = [];
            } else {
              toaster.pop('danger', "Loading Regions", "Error loading Regions!");
              console.error(result);
            }
          });
        };

        $scope.monthSelectionChanged = function () {
          $scope.disableYearPicker = ($scope.filterMonth === 0);
        }

        if (!Array.prototype.remove) {
          Array.prototype.remove = function (val) {
            var i = this.indexOf(val);
            return i > -1 ? this.splice(i, 1) : [];
          };
        }

        $scope.regionSelected = function(data) {
          if (data.Ticked === true) {
            angular.forEach($scope.branches, function(branch, branckKey) {
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
        var _resetData = function () {
          localStorage.clear();
        }

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
          _getRegions();
          //loadCharts();
        };

        $scope.refresh = function () {
          $scope.disableButtons = true;
          _populateFilterBranch();
          _resetData();
          $scope.disableButtons = false;
        };

        function open(strData, strMimeType) {
          var newdata = "data:" + strMimeType + ";base64," + encodeURI(strData);
          //To open in new window
          window.open(newdata, "_blank");
          return true;
        }

        $scope.export = function () {
          $scope.disableButtons = true;
          $scope.btnExportText = "Processing..";

          _populateFilterBranch();

          reportingResource.getExportFileNew($scope.filterBranch, $scope.filterStartDate, $scope.filterEndDate, $scope.token).then(function (result) {
            if (result.status === 200) {
              open(result.data.response, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
              //window.open(result.data.response, "_newtab");
              $scope.disableButtons = false;
              $scope.btnExportText = "Export";
            } else {
              toaster.pop('danger', "", "!");
              console.error(result);
              $scope.disableButtons = false;
              $scope.btnExportText = "Export";
            }
          });
        };

        $scope.email = function () {
          $scope.disableButtons = true;
          $scope.btnEmailText = "Processing..";

          _populateFilterBranch();

          reportingResource.EmailCiReport($scope.filterBranch, $scope.filterStartDate, $scope.filterEndDate, userId, $scope.email, $scope.token).then(function (result) {
            if (result.status === 200) {
              toaster.pop('info', "Your email will come through shortly", "Success!");
              $scope.disableButtons = false;
              $scope.btnEmailText = "Email";
            } else {
              toaster.pop('danger', "", "!");
              console.error(result);
              $scope.disableButtons = false;
              $scope.btnEmailText = "Email";
            }
          });
        };

        $scope.showChart = function (type) {
          $scope.isLoaded = false;
          $scope.selectedReport = type;
          //_populateChart(function () {

          //});
          $scope.isLoaded = true;
        };
      }
  ]);
}(this.angular));