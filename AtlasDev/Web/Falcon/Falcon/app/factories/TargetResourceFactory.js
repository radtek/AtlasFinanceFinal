(function(angular) {
    'use strict';
    var module = angular.module('Falcon.Factories.TargetResource', ['ngResource', 'ngSanitize']);

    module.value('$', $);

    module.factory('TargetResource', [
        '$http', 'apiBase',
        function($http, $apiBase) {
            return {
                getMonthlyCiFilterData: function () {
                    return $http.get($apiBase + 'Target/GetMonthlyCiFilterData/', {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function (data, status, headers) {
                        return data;
                    }).error(function (data, status, headers) {
                        return status;
                    });
                },
                getHandoverTargetFilterData: function () {
                    return $http.get($apiBase + 'Target/GetHandoverTargetFilterData/', {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function (data, status, headers) {
                        return data;
                    }).error(function (data, status, headers) {
                        return status;
                    });
                },
                getDailySales: function (targetMonth, targetYear) {
                    return $http.post($apiBase + 'Target/GetDailySales/', {
                        'TargetMonth': targetMonth,
                        'TargetYear': targetYear
                    }, {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function (data, status, headers) {
                        return data;
                    }).error(function (data, status, headers) {
                        return status;
                    });
                },
                getLoanMix: function (targetMonth, targetYear) {
                    return $http.post($apiBase + 'Target/GetLoanMix/', {
                        'TargetMonth': targetMonth,
                        'TargetYear': targetYear
                    }, {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function (data, status, headers) {
                        return data;
                    }).error(function (data, status, headers) {
                        return status;
                    });
                },
                getBranchCiMonthly: function (branchId, hostId, targetMonth, targetYear) {
                    return $http.post($apiBase + 'Target/GetBranchCiMonthly/', {
                        'BranchId': branchId,
                        'HostId': hostId,
                        'TargetMonth': targetMonth,
                        'TargetYear': targetYear
                    }, {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function (data, status, headers) {
                        return data;
                    }).error(function (data, status, headers) {
                        return status;
                    });
                },
                getHandoverTarget: function (branchId, hostId, targetMonth, targetYear) {
                    return $http.post($apiBase + 'Target/GetHandoverTarget/', {
                        'BranchId': branchId,
                        'HostId': hostId,
                        'TargetMonth': targetMonth,
                        'TargetYear': targetYear
                    }, {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function(data, status, headers) {
                        return data;
                    }).error(function(data, status, headers) {
                        return status;
                    });
                },
                saveDailySales: function (target) {
                    return $http.post($apiBase + 'Target/AddDailySale/', {
                        'TargetDate': target.TargetDate,
                        'Percent': target.Percent,
                        'Amount': target.Amount,
                        'BranchId': target.BranchId,
                        'HostId': target.HostId
                    }, {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function (data, status, headers) {
                        return data;
                    }).error(function (data, status, headers) {
                        return status;
                    });
                },
                saveLoanMix: function (target) {
                    return $http.post($apiBase + 'Target/AddLoanMix/', {
                        'TargetDate': target.TargetDate,
                        'Percent': target.Percent,
                        'PayNo': target.PayNo
                    }, {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function (data, status, headers) {
                        return data;
                    }).error(function (data, status, headers) {
                        return status;
                    });
                },
                saveBranchCiMonthly: function (target) {
                    return $http.post($apiBase + 'Target/AddBranchCiMonthly/', {
                        'BranchId': target.BranchId,
                        'HostId': target.HostId,
                        'TargetDate': target.TargetDate,
                        'Amount': target.Amount,
                        'Percent': target.Percent
                    }, {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function (data, status, headers) {
                        return data;
                    }).error(function (data, status, headers) {
                        return status;
                    });
                },
                saveHandoverTarget: function(target) {
                    return $http.post($apiBase + 'Target/AddHandoverTarget/', {
                        'BranchId': target.BranchId,
                        'HostId': target.HostId,
                        'TargetDate': target.TargetDate,
                        'HandoverBudget': target.HandoverBudget,
                        'ArrearTarget': target.ArrearTarget
                    }, {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function(data, status, headers) {
                        return data;
                    }).error(function(data, status, headers) {
                        return status;
                    });
                }
            }
        }
    ]);
}(this.angular));