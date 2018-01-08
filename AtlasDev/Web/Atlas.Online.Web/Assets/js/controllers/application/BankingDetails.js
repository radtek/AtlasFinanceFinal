(function (angular) {

  var module = angular.module('atlas.application');  

  module.directive('atlBankingDetails', function () {
    return {
      restrict: 'AC',
      controller: ['$scope', 'AvsResource', '$window', '$timeout', 'atlasEnums', 'Application', 'UI',
        function ($scope, AvsResource, $window, $timeout, atlasEnums, Application, UI) {

          var bankData = angular.copy(Application.stepData.BankDetail); // IBankDetailsDtoWrapper

          // Let view use the enums
          angular.extend($scope, atlasEnums);

          var AVS_RESULT = atlasEnums.AVS_RESULT;

          $scope.allowBankChanges = false;

          var cdvValidate = function () {
            $scope._BankDetail_AccountNo.validateRemotely.revalidate();
          };

          var tryAVSCheck = function () {
            $scope.bankDetailsLoading = true;
            var accChecked = false;

            AvsResource.poller(20000, function (resp) {
              UI.clearError();

              var status = +resp.Status;
              var result = $scope.allowBankChanges = (
                // Disable if AVS has started but we don't yet have a result back.
                status !== AVS_RESULT.NORESULT
              );

              $scope.data.AvsResult = $scope.avsResult = status;
              $scope.bankDetailsLoading = false;

              if (status) {
                if (status === AVS_RESULT.PASSED) {
                  return true;
                }

                if (status === AVS_RESULT.FAILED) {
                  cdvValidate();
                  return false;
                }
              } else {
                $scope.avsResult = null;
                $scope.allowBankChanges = true;
                return true;
              }

              // If we can change bank details, do the initial account validation check
              if (result && !accChecked) {              
                $timeout(function () {
                  cdvValidate();
                });

                accChecked = true;
              }
            })
            .error(function () {
              $scope.bankDetailsLoading = false;

              $scope.allowBankChanges = false;
              $scope.avsResult = false;
              UI.setError("There has been a system error when checking your bank details status. Please click retry to attempt again or refresh your browser.", function () {
                tryAVSCheck();
                UI.clearError();
              });
            })
            .poll();          
          };
          
          var status = $scope.avsResult = $scope.data.AvsResult;
          // If we have an AVS status don't do the check
          if (!status || status == AVS_RESULT.NORESULT) {
            tryAVSCheck();
          } else {
            $scope.allowBankChanges = true;
            // Revalidate the account number (timeout occurs after all directives have run)
            $timeout(function () {
              cdvValidate();
            });
          };

          $scope.$watch('data.BankDetail', function (value) {
            if (bankData == null || !angular.equals(value, bankData)) {
              $scope.avsResult = null;
            } else {
              $scope.avsResult = $scope.data.AvsResult;
            }
          }, true);

          $scope.revalidateAccNo = cdvValidate;

          $scope.getAccNoValidationParams = function () {
            var bankDetail = $scope.data.BankDetail;

            if (!bankDetail.BankName || !bankDetail.AccountType) {
              return false;
            }

            return {
              bank: bankDetail.BankName,
              accType: bankDetail.AccountType
            };
          };

          //$scope.resubmitAvs = function ($event) {
          //  if ($event) { $event.preventDefault(); }
          //  if ($scope.avsResult != AVS_RESULT.FAILED) { return; }

          //  $scope.bankDetailsLoading = true;
          //  var oldStatus = $scope.avsResult;
          //  $scope.avsResult = false;
          //  bankData = angular.copy($scope.data.BankDetail);
          //  AvsResource.submit({ ApplicationId: Application.Id, data: bankData }, function (resp) {             
          //    tryAVSCheck();
          //  }, function (e) {
          //    $scope.avsResult = oldStatus;
          //    $scope.bankDetailsLoading = true;
          //    UI.setError("Failed to resubmit your banking details. " + (e.Message || ''), $scope.resubmitAvs);
          //  });
          //};
        }]
    };
  });
}(this.angular));
