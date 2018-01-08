(function(angular) {
    'use strict';
    var module = angular.module('Falcon.Factories.UserResource', ['ngResource', 'ngSanitize']);

    module.value('$', $);

    module.factory('UserResource', ['$cookies', '$http', 'apiBase',
        function($cookies, $http, $apiBase) {
            return {
                getUserId: function() {
                    return $cookies['fl_user_id'];
                },
                getUserBranchId: function() {
                    return $cookies['fl_branch_id'];
                },
                getUserBranchName: function() {
                    return $cookies['fl_branch_name'];
                },

                getUserLegacyClientId: function () {
                    return $cookies['fl_legacy_client_code'];
                },
                getUsers: function(token) {
                    return $http.post($apiBase + 'User/Get/', {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function(data, status, headers) {
                        return data;
                    }).error(function(data, status, headers) {
                        return status;
                    });
                },
                getConsultants: function (branchId) {
                    return $http.post($apiBase + 'User/GetConsultants/', {
                        'BranchId': branchId
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
                getLinkedBranches: function(userId) {
                    return $http.post($apiBase + 'User/GetLinkedBranches/', {
                        'UserId': userId
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

    module.factory('UserManagementResource', ['$http', 'apiBase',
        function($http, $apiBase) {
            return {
                list: function(branchId, idNo, firstName, lastName) {
                    return $http.post($apiBase + 'User/List/', {
                        'BranchId': branchId,
                        'IdNo': idNo,
                        'FirstName': firstName,
                        'LastName': lastName
                    }, {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function(data, status, headers) {
                        return data;
                    }).error(function(data, status, headers) {
                        return status;
                    })
                },
                revoke: function(id, token) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/user/?method=revoke&id=' + id)
                    });
                },
                authorise: function(id, token) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/user/?method=authorise&id=' + id)
                    });
                },
                linkUser: function(userId, personId) {
                    return $http.post($apiBase + 'User/LinkUserV2/', {
                        'PersonId': personId,
                        'UserId': userId
                    }, {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function(data, status, headers) {
                        return data;
                    }).error(function(data, status, headers) {
                        return status;
                    })
                },
                unlinkUser: function(userId, personId) {
                    return $http.post($apiBase + 'User/UnLinkUser/', {
                        'PersonId': personId,
                        'UserId': userId
                    }, {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function(data, status, headers) {
                        return data;
                    }).error(function(data, status, headers) {
                        return status;
                    })
                },
                createWebUser: function(userId, personId) {
                    return $http.post($apiBase + 'User/LinkUserV2/', {
                        'PersonId': personId,
                        'UserId': userId
                    }, {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function(data, status, headers) {
                        return data;
                    }).error(function(data, status, headers) {
                        return status;
                    })
                },
                saveRoles: function(userId, roles) {
                    return $http.post('/api/role/save', {
                        'UserId': userId,
                        'Roles': roles
                    }, {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function(data, status, headers) {
                        return data;
                    }).error(function(data, status, headers) {
                        return status;
                    })
                },
                saveUser: function (personId, username, email, password) {
                  return $http.post('/api/user/saveuser', {
                    'PersonId': personId,
                    'Username': username,
                    'Email': email,
                    'Password': password
                  }, {
                    headers: {
                      'Content-Type': 'application/json; charset=utf-8'
                    }
                  }).success(function(data, status, headers) {
                    return data;
                  }).error(function(data, status, headers) {
                    return {
                      data: data,
                      status: status
                    };
                  });
                },
                deleteUser: function(userId) {
                    return $http.post('/api/user/deleteuser', {
                        'UserId': userId
                    }, {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function(data, status, headers) {
                        return data;
                    }).error(function(data, status, headers) {
                        return status;
                    })
                },
                getWebUsers: function() {
                    return $http.get('/api/user/getwebusers', {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function(data, status, headers) {
                        return data;
                    }).error(function(data, status, headers) {
                        return status;
                    })
                },
                getWebUsersByGuid: function (userId, token) {
                    return $http({
                        method: 'GET',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/user/GetWebUsersByGuid/?UserId=' + userId)
                    }).then(function (response) {
                        return response;
                    }, function (error) {
                        return error;
                    });
                },





                // get user roles/claims
               getUserClaimsData: function (userId, token) {
                    return $http({
                        method: 'GET',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/UserClaims/GetUserClaimsData/?userId=' + userId)
                    }).then(function (response) {
                        return response;
                    }, function (error) {
                        return error;
                    });
                },


               changePassword: function (userId,newPassword, token) {
                   return $http({
                       method: 'GET',
                       headers: {
                           'forge_token': token
                       },
                       url: ('/api/UserClaims/ChangeUserPassword/?UserId=' + userId + '&NewPassword=' +newPassword)

                   }).then(function (response) {
                       return response;
                   }, function (error) {
                       return error;
                   });
               },

                //ASS user override call
                authorizeUserForByPassOnFalcon: function (startDate, endDate, userOperatorCode,branchNum, regionalOperatorCode, newlevel, reason) {
                    //return $http.post($apiBase + 'AssUserByPass/AuthorizeUserForByPassOnFalcon',
                  return $http.post('/api/AssUserOverride/AssUserOverride',
                  {
                    "StartDate": startDate,
                    "EndDate": endDate,
                    "UserOperatorCode": userOperatorCode,
                    "BranchNum": branchNum,
                    "RegionalOperatorCode": regionalOperatorCode,
                    "NewLevel": newlevel,
                    "Reason": reason
                  },
                  {
                    headers: {
                      'Content-Type': 'application/json; charset=utf-8'
                    }

                  }).success(function(data, status, headers) {
                    $scope.comments = data.data;
                    return data;
                  }).error(function(data, status, headers) {
                    return status;
                  });
                }

            }

        }
    ]);
}(this.angular));