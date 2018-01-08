(function (angular) {
  'use strict';
  var module = angular.module('atlas.otp', [
    'atlas.factories'
  ]);

  // Resource
  module.factory('OTPResource', ['$apiResource', function ($apiResource) {
    return $apiResource('/OTP/:method', { }, {
      request: { method: 'GET', params: { method: 'requestOtp' } },
      requestFirst: { method: 'GET', params: { method: 'requestOtp', first: true } },
      validate: { method: 'POST', params: { method: 'validate' } }
    });
  }]);

  // Controller
  module.controller('OtpController', ['$scope', 'OTPResource', function ($scope, OTP) {
    var defaultErrorMsg = 'An error has occurred. Please try again later.';

    $scope.otp = '';
    $scope.otpValidated = false;

    var sendOk = function (resp) {
      $scope.otpSent = resp.Sent;
      $scope.remainingResendRetries = resp.Count;
      $scope.disableUI = false;
    };

    var sendError = function (e) {
      $scope.otpError = {
        message: e.Message || defaultErrorMsg
      };
      $scope.disableUI = false;
    };

    var sendOtpRequest = function (first) {      
      var fn = first ? 'requestFirst' : 'request';

      $scope.disableUI = true;
      $scope.otpSent = null;

      OTP[fn](sendOk, sendError);
    };

    var validateOk = function (resp) {
      if (resp.Validated) {
        $scope.otpValidated = true;
      } else {
        $scope.otpAttemptFailed = true;
        $scope.otpSent = false;
      }
      $scope.disableUI = false;
    };

    var validateError = function (e) {
      $scope.otpError = {
        message: e.Message || defaultErrorMsg
      };
      $scope.disableUI = false;
    };

    var validateOtp = function () {
      if (canSubmit()) {
        $scope.otpError = null;
        $scope.disableUI = true;
        OTP.validate({otp: $scope.otp}, validateOk, validateError);
      }
    };

    var canSubmit = $scope.canSubmit = function () {
      return !$scope.disableUI && $scope.otp;
    };

    var canResend = $scope.canResend = function () {
      return !$scope.disableUI && $scope.remainingResendRetries > 0;
    };

    $scope.validateOtp = validateOtp;

    $scope.resendOtp = function () {
      if (canResend()) {
        $scope.otpError = null;
        sendOtpRequest();
      }
    };

    sendOtpRequest(true);
  }]);

} (this.angular));