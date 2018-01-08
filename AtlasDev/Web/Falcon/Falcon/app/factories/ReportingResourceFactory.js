(function (angular) {
    'use strict';
    var module = angular.module('Falcon.Factories.ReportResource', ['ngResource', 'ngSanitize']);

    module.value('$', $);

    module.factory('AssReportingResource', ['$http', 'apiBase',
           function ($http, $apiBase) {
             return {
               getFilterData: function (token) {
                 return $http.get($apiBase + 'CiReport/GetFilterData/', {
                   headers: {
                     'Content-Type': 'application/json; charset=utf-8'
                   }
                 }).success(function (data, status, headers) {
                   return data;
                 }).error(function (data, status, headers) {
                   return status;
                 });
               },
                   getRegionBranches: function (regionId, token) {
                       return $http({
                           method: 'GET',
                           headers: {
                               'forge_token': token
                           },
                           url: ('/api/AssReport/?personId=' + regionId)
                       }).then(function (response) {
                           return response;
                       }, function (error) {
                           return error;
                       });
                   },
                   getCheque: function (branchId, month, year, token) {
                       return $http({
                           method: 'GET',
                           headers: {
                               'forge_token': token
                           },
                           url: ('/api/AssReport/?getCheque=0&branchIds=' + branchId + '&month=' + month + '&year=' + year)
                       }).then(function (response) {
                           return response;
                       }, function (error) {
                           return error;
                       });
                   },
                   getInterestPercentile: function (branchId, month, year, token) {
                       return $http({
                           method: 'GET',
                           headers: {
                               'forge_token': token
                           },
                           url: ('/api/AssReport/?getInterestPercentile=0&branchIds=' + branchId + '&month=' + month + '&year=' + year)
                       }).then(function (response) {
                           return response;
                       }, function (error) {
                           return error;
                       });
                   },
                   getInsurancePercentile: function (branchId, month, year, token) {
                       return $http({
                           method: 'GET',
                           headers: {
                               'forge_token': token
                           },
                           url: ('/api/AssReport/?getInsurancePercentile=0&branchIds=' + branchId + '&month=' + month + '&year=' + year)
                       }).then(function (response) {
                           return response;
                       }, function (error) {
                           return error;
                       });
                   },
                   getInsurance: function (branchId, month, year, token) {
                       return $http({
                           method: 'GET',
                           headers: {
                               'forge_token': token
                           },
                           url: ('/api/AssReport/?getInsurance=0&branchIds=' + branchId + '&month=' + month + '&year=' + year)
                       }).then(function (response) {
                           return response;
                       }, function (error) {
                           return error;
                       });
                   },
                   getInterest: function (branchId, month, year, token) {
                       return $http({
                           method: 'GET',
                           headers: {
                               'forge_token': token
                           },
                           url: ('/api/AssReport/?getInterest=0&branchIds=' + branchId + '&month=' + month + '&year=' + year)
                       }).then(function (response) {
                           return response;
                       }, function (error) {
                           return error;
                       });
                   },
                   getLoanMix: function (branchId, month, year, token) {
                       return $http({
                           method: 'GET',
                           headers: {
                               'forge_token': token
                           },
                           url: ('/api/AssReport/?getLoanMix=0&branchIds=' + branchId + '&month=' + month + '&year=' + year)
                       }).then(function (response) {
                           return response;
                       }, function (error) {
                           return error;
                       });
                   },
                   getAverageNewClientLoan: function (branchId, month, year, token) {
                       return $http({
                           method: 'GET',
                           headers: {
                               'forge_token': token
                           },
                           url: ('/api/AssReport/?getAverageNewClientLoan=0&branchIds=' + branchId + '&month=' + month + '&year=' + year)
                       }).then(function (response) {
                           return response;
                       }, function (error) {
                           return error;
                       });
                   },
                   getAverageLoan: function (branchId, month, year, token) {
                       return $http({
                           method: 'GET',
                           headers: {
                               'forge_token': token
                           },
                           url: ('/api/AssReport/?getAverageLoan=0&branchIds=' + branchId + '&month=' + month + '&year=' + year)
                       }).then(function (response) {
                           $window.open()
                           return response;
                       }, function (error) {
                           return error;
                       });
                   },
                   getExport: function (branchId, month, year, token) {
                       return $http({
                           method: 'GET',
                           headers: {
                               'forge_token': token
                           },
                           responseType: 'arraybuffer',
                           url: ('/api/AssReport/?getXls=0&branchIds=' + branchId + '&month=' + month + '&year=' + year)
                       }).then(function (response) {
                           return response;
                       }, function (error) {
                           return error;
                       });
                   },
                   getExportFile: function (branchId, month, year, token) {
                       return $http({
                           method: 'GET',
                           headers: {
                               'forge_token': token
                           },
                           url: ('/api/AssReport/?getXlsPath=0&branchIds=' + branchId + '&month=' + month + '&year=' + year)
                       }).then(function (response) {
                           return response;
                       }, function (error) {
                           return error;
                       });
                   },
                   getExportFileNew: function (branchIds, startDate, endDate, token) {
                     return $http.post($apiBase + 'CiReport/GetCiReport/', {
                       'BranchIds': branchIds,
                       'StartDate': startDate,
                       'EndDate': endDate
                     }, {
                       headers: {
                         'Content-Type': 'application/json; charset=utf-8'
                       }
                     }).then(function (response) {
                       return response;
                     }, function (error) {
                       return error;
                     });
                   },
                   EmailCiReport: function (branchIds, startDate, endDate, userId, email, token) {
                     return $http.post($apiBase + 'CiReport/EmailCiReport/', {
                       'BranchIds': branchIds,
                       'StartDate': startDate,
                       'EndDate': endDate,
                       'UserId': userId,
                       'Email':email
                     }, {
                       headers: {
                         'Content-Type': 'application/json; charset=utf-8'
                       }
                     }).then(function (response) {
                       return response;
                     }, function (error) {
                       return error;
                     });
                   }
               };
           }
    ]);

    module.factory('StreamReportingResource', ['$http', 'apiBase',
           function ($http, $apiBase) {
               return {
                   getFilterData: function (token) {
                     return $http.get($apiBase + 'StreamReport/GetFilterData/', {
                       headers: {
                         'Content-Type': 'application/json; charset=utf-8'
                       }
                     }).success(function(data, status, headers) {
                       return data;
                     }).error(function(data, status, headers) {
                       return status;
                     });
                   },
                   getAccountData: function (groupTypeId, regionId, categoryId, branchIds, startDate, endDate, drillDownLevel, token) {
                       return $http.post($apiBase + 'StreamReport/GetAccountData/', {
                           'GroupTypeId': groupTypeId,
                           'RegionId': regionId,
                           'CategoryId': categoryId,
                           'BranchIds': branchIds,
                           'StartDate': startDate,
                           'EndDate': endDate,
                           'DrillDownLevel': drillDownLevel
                       }, {
                           headers: {
                               'Content-Type': 'application/json; charset=utf-8'
                           }
                       }).then(function (response) {
                           return response;
                       }, function (error) {
                           return error;
                       });
                   },
                   getAccounts: function (groupTypeId, regionId, categoryId, subCategoryId, branchIds, startDate, endDate, allocatedUserId, colIndex, token) {
                       return $http.post($apiBase + 'StreamReport/GetAccounts/', {
                           'GroupTypeId': groupTypeId,
                           'RegionId': regionId,
                           'CategoryId': categoryId,
                           'SubCategoryId': subCategoryId,
                           'BranchIds': branchIds,
                           'StartDate': startDate,
                           'EndDate': endDate,
                           'AllocatedUserId': allocatedUserId,
                           'Column': colIndex
                       }, {
                           headers: {
                               'Content-Type': 'application/json; charset=utf-8'
                           }
                       }).then(function (response) {
                           return response;
                       }, function (error) {
                           return error;
                       });
                   },
                   getExportFile: function (groupTypeId, branchIds, startDate, endDate, token) {
                       return $http.post($apiBase + 'StreamReport/GetPerformanceReport/', {
                           'GroupTypeId': groupTypeId,
                           'BranchIds': branchIds,
                           'StartDate': startDate,
                           'EndDate': endDate,
                       }, {
                           headers: {
                               'Content-Type': 'application/json; charset=utf-8'
                           }
                       }).then(function (response) {
                           return response;
                       }, function (error) {
                           return error;
                       });
                   },
                   getAccountExportFile: function (groupTypeId, regionId, categoryId, branchIds, startDate, endDate, allocatedUserId, colIndex, token) {
                       return $http.post($apiBase + 'StreamReport/GetAccountsExport/', {
                           'GroupTypeId': groupTypeId,
                           'RegionId': regionId,
                           'CategoryId': categoryId,
                           'BranchIds': branchIds,
                           'StartDate': startDate,
                           'EndDate': endDate,
                           'AllocatedUserId': allocatedUserId,
                           'Column': colIndex
                       }, {
                           headers: {
                               'Content-Type': 'application/json; charset=utf-8'
                           }
                       }).then(function (response) {
                           return response;
                       }, function (error) {
                           return error;
                       });
                   },
                   getDetailExportFile: function (groupTypeId, branchId, streamIds, caseStatusIds, token) {
                     return $http.post($apiBase + 'StreamReport/GetDetailReport/', {
                       'GroupTypeId': groupTypeId,
                       'BranchId': branchId,
                       'StreamIds': streamIds,
                       'CaseStatusIds': caseStatusIds
                     }, {
                       headers: {
                         'Content-Type': 'application/json; charset=utf-8'
                       }
                     }).then(function (response) {
                       return response;
                     }, function (error) {
                       return error;
                     });
                   }
               };
           }
    ]);

}(this.angular));