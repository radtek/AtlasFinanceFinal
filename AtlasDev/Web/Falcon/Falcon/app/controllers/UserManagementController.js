(function (angular) {
  'use strict';

  var app = angular.module('falcon');

  app.controller('UserManagementListController', [
    '$scope', 'UserManagementResource', 'FalconTable', '$rootScope', 'BranchResource', 'toaster', 'ngProgress',
    function ($scope, userManagementResource, $ft, $rootScope, branchResource, toaster, ngProgress) {
      $scope.loaded = false;
      $scope.savingBranchAssociation = false;
      $scope.userId = null;
      $scope.modalMode = 'revoke';
      $scope.searching = false;
      $scope.pageLimit = 10;
      $scope.webUserPageLimit = 10;
      $scope.webUsers = [];
      $scope.allocateUser = undefined;
      $scope.unassignedRoles = [];
      $scope.assignedroles = [];
      $scope.allRoles = [];
      $scope.branchSelector = false;
      $scope.savingUnLink = false;
      $scope.noneLinked = false;
      $scope.savingUser = false;
      $scope.falconUsers = [];
      $scope.loadingClaims = false;

      $rootScope.$broadcast('menu', {
        name: 'User Management',
        Desc: 'user list',
        url: '/User/Management/List',
        searchVisible: false
      });
      var _load = function () {
        $scope.branchLoading = true;
        loadWebUsers();
        branchResource.get().then(function (data) {
          $scope.branches = data.data;
          $scope.branchLoading = false;
        }, function (data) {
          toaster.pop('danger', 'Load Branches', 'An error occurred while trying to load the branches.');
          $scope.branchLoading = false;
        });
      }
      $scope.search = function () {
        $scope.searching = true;
        userManagementResource.list($scope.branch, $scope.idNo, $scope.firstName, $scope.lastName).then(function (data) {
          $scope.users = data.data;
          $scope.searching = false;
        }, function (data) {
          toaster.pop('danger', 'Load Users', 'An error occurred while trying to load the users.');
          $scope.searching = false;
        });
      }

      // Change pasword 
      var _changePassword = function () {
        $scope.changed = true;
        userManagementResource.changePassword($scope.userId, $scope.newPassword).then(function (data) {
          ngProgress.complete();
          toaster.pop('info', 'Change pasword', 'Password changed successful.');
          $scope.changed = false;
          $scope.newPassword = "";
        }, function (data) {
          toaster.pop('danger', 'Change pasword', 'An error occurred while trying to change the password.');
          ngProgress.complete();
          $scope.changed = false;
        });
      };
      $scope.changePwrd = _changePassword;


      $scope.link = function (user) {
        $scope.allocateUser = user;
        angular.element('#linkDlg').modal('show');
      }
      $scope.setWebUser = function (user) {
        $scope.linkSelectedUser = user;
      }
      $scope.viewLinkInfo = function (user) {
        alert(user.Id);
      }

      $scope.linkToUser = function (allocated, user) {
        $scope.savingLink = true;
        userManagementResource.linkUser(user.Id, allocated.PersonId).then(function (data) {
          toaster.pop('info', 'Link User', 'The two users have been linked.');
          $scope.savingLink = false;
          $scope.branchSelector = false;
          angular.element('#confirmLinkDlg').modal('hide');
          angular.element('#manageUserDlg').modal('hide');
          loadWebUsersByGuId(user.WebReference);
        }, function (data) {
          toaster.pop('danger', 'Link User', 'An error occurred while trying to link the two users.');
          $scope.savingLink = false;
        });
      }

      $scope.unlinkUser = function () {
        $scope.savingUnLink = true;
        userManagementResource.unlinkUser($scope.linkSelectedUser.Id, $scope.allocateUser.PersonId).then(function (data) {
          $scope.savingUnLink = false;
          angular.element('#confirmUnlinkDlg').modal('hide');
          angular.element('#manageUserDlg').modal('hide');
          loadWebUsersByGuId(user.WebReference);
        }, function (data) {
          toaster.pop('danger', 'UnLink User', 'An error occurred while trying to unlink the two users.');
          $scope.savingUnLink = false;
        });
      }

      $scope.viewRoles = function () {

      }

      $scope.manage = function (user) {
        $scope.allocateUser = user;
        $scope.associatedBranchId = user.Branch ? user.Branch.BranchId : undefined;
        $scope.username = ($scope.allocateUser.Firstname.split(' ').shift() + $scope.allocateUser.Lastname.split(' ').pop()[0]).toLowerCase();
        $scope.email = $scope.username + '@unknown.co.za';
        $scope.password = $scope.confirmPassword = '';
        $scope.userId = user.WebReference;
        var _associatedUser = findIn($scope.webUsers, 'Id', user.WebReference);
        if (!_associatedUser)
          $scope.noneLinked = true;
        else
          $scope.noneLinked = false;
        $scope.assignedroles = _associatedUser.Roles ? _associatedUser.Roles : [];
        $scope.assignableRoles = cleanArrayList();
        loadWebUsersByGuId(user.WebReference, '');
        _getUserClaimsData(user.WebReference, '');
      }
      // get user roles /claims data
      var _getUserClaimsData = function (userId) {
        $scope.loadingClaims = true;
        userManagementResource.getUserClaimsData(userId).then(function (data) {
          ngProgress.complete();
          $scope.assignedroles = data.data.Roles;
          $scope.assignableRoles = cleanArrayList();
          $scope.loadingClaims = false;
        }, function (data) {
          toaster.pop('danger', 'Loading', 'An error occurred while trying to load the user claims data.');
          ngProgress.complete();
          $scope.loadingClaims = false;
        });
      };

      var loadWebUsers = function () {
        $scope.loaded = false;
        userManagementResource.getWebUsers().then(function (data) {
          $scope.webUsers = data.data.WebUsers;
          $scope.allRoles = data.data.Roles;
          ngProgress.complete();
          $scope.loaded = true;
        }, function (data) {
          toaster.pop('danger', 'Loading', 'An error occurred while trying to load the user data.');
          ngProgress.complete();
        });
      };

      var loadWebUsersByGuId = function (userId) {
        $scope.loaded = false;
        userManagementResource.getWebUsersByGuid(userId).then(function (data) {
          $scope.falconUsers = data.data.FalconUsers;
          ngProgress.complete();

          $scope.loaded = true;
        }, function (data) {
          toaster.pop('danger', 'Loading', 'An error occurred while trying to load the user data.');
          ngProgress.complete();
        });
      };
      $scope.init = function () {
        ngProgress.start();
        _load();
      };

      $scope.addRole = function () {
        $scope.assignedroles = addToRoleList($scope.assignableRole);
      };

      $scope.removeRole = function () {
        $scope.assignableRoles = removeItem($scope.assignedRole);
      };

      $scope.saveBranchAssociation = function () {
        $scope.savingBranchAssociation = true;
        branchResource.associateUser($scope.associatedBranchId, $scope.allocateUser.PersonId, $scope.token).then(function (data) {
          toaster.pop('info', 'Associate Branch', 'Branch saved for user.');
          $scope.savingBranchAssociation = false;

          $scope.branchSelector = false;
          loadWebUsers();
          $scope.search();
        }, function (data) {
          toaster.pop('danger', 'Associate Branch', 'An error occurred while trying to associate the branch to the user.');
          $scope.savingBranchAssociation = false;

          $scope.branchSelector = false;
        });
      }

      $scope.saveRoles = function () {
        $scope.savingRoles = true;
        userManagementResource.saveRoles($scope.allocateUser.WebReference, $scope.assignedroles).then(function (data) {
          $scope.savingRoles = false;
          loadWebUsers();
          angular.element('#assignRolesDlg').modal('hide');
        }, function (data) {
          toaster.pop('danger', 'Save Roles', 'An error occurred while trying to save the roles.');
          $scope.savingRoles = false;
        })
      }

      $scope.saveUser = function (personId, username, email, password) {
        $scope.savingUser = true;
        userManagementResource.saveUser(personId, username, email, password).then(function(response) {
          toaster.pop('info', 'Creating User', 'The user was successfully created.');
          $scope.savingUser = false;
          angular.element('#addUserDlg').modal('hide');
          loadWebUsersByGuId(response.data.user);
          $scope.allocateUser.WebReference = $scope.userId = response.data.user;
        }, function(data) {
          toaster.pop('danger', 'Creating User', 'An error occurred while trying to create the specified user. ' + data.data.ex.Message);
          $scope.savingUser = false;
        });
      }

      $scope.deleteUser = function (userId) {
        userId = '';
        userManagementResource.deleteUser(userId).then(function (data) {
          toaster.pop('info', 'Deleting User', 'The user was successfully delete.');
          $scope.savingUser = false;
        }, function (data) {
          toaster.pop('danger', 'Deleting User', 'An error occurred while trying to delete the specified user.');
          $scope.savingUser = false;
        });
      }

      //utility section
      var findIn = function (arr, name, value) {
        for (var i = 0, len = arr.length; i < len; i++) {
          if (name in arr[i] && arr[i][name] === value) return arr[i];
        };
        return false;
      }

      var addToRoleList = function (roles) {
        var arr = [];

        for (var i = roles.length - 1; i >= 0; i--) {
          arr.push({
            'RoleName': roles[i]
          });
        };
        arr = arr.concat.apply(arr, $scope.assignedroles);

        for (var i = $scope.assignableRoles.length - 1; i >= 0; i--) {
          for (var b = arr.length - 1; b >= 0; b--) {
            if ($scope.assignableRoles[i]["RoleName"] === arr[b]["RoleName"]) {
              $scope.assignableRoles.splice(i, 1);
              break;
            }
          };
        };
        return arr;
      }

      var removeItem = function (roles, item) {
        var arr = [];

        for (var i = roles.length - 1; i >= 0; i--) {
          arr.push({
            'RoleName': roles[i]
          });
        };
        arr = arr.concat.apply(arr, $scope.assignableRoles);

        for (var i = $scope.assignedroles.length - 1; i >= 0; i--) {
          for (var b = arr.length - 1; b >= 0; b--) {
            if ($scope.assignedroles[i]["RoleName"] === arr[b]["RoleName"]) {
              $scope.assignedroles.splice(i, 1);
              break;
            }
          };
        };
        return arr;
      }

      var cleanArrayList = function () {
        var arr = [];
        if ($scope.assignedroles === undefined || $scope.assignedroles.length === 0) {
          arr = arr.concat.apply(arr, $scope.allRoles);
          return arr;
        }

        for (var i = 0; i < $scope.allRoles.length; i++) {
          var isAssigned = false;
          if ($scope.assignedroles !== undefined) {
            for (var j = 0; j < $scope.assignedroles.length; j++) {
              if ($scope.allRoles[i].RoleName === $scope.assignedroles[j].RoleName) {
                isAssigned = true;
                break;
              }
            }
          }
          if (!isAssigned) {
            arr.push($scope.allRoles[i]);
          }
        }

        return arr;
      };
    }
  ]);

  app.controller('UserManagementAddController', [
    '$scope', 'UserManagementResource', 'FalconTable', '$rootScope',
    function ($scope, UserManagementResource, $ft, $rootScope) {
      $rootScope.$broadcast('menu', {
        name: 'User Management - Add User',
        url: '/User/Management/Add',
        searchVisible: false
      });
      $scope.Step = 'Add';

      $scope.init = function () {

      };
    }
  ]);

}(this.angular));