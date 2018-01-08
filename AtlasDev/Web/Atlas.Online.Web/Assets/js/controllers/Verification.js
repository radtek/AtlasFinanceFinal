(function (angular) {
  'use strict';
  var module = angular.module('atlas.verification', [
    'atlas.config',
    'atlas.factories',
  ]);

  // Simple shared object for UI 
  module.factory("UI", function () {
    var UI = function () {
      this.loading = true;
      this.error = null;
      this.showSteps = true;
      this.showButtons = true;

      this.setError = function (message, retryHandler) {
        this.error = {
          message: message,
          retryHandler: retryHandler
        };

        return this;
      };

      this.clearError = function () {
        this.error = null;
      };
    };

    return UI._instance || (UI._instance = new UI());
  });

  module.controller('VerificationCtrl', ['$scope', '$http', '$window', 'UI', 'atlasConfig', function ($scope, $http, $window, UI, config) {
    var applicationId, questions, answers;

    var _getUrl = function() {
      return '/api/VerificationQuestions/'+applicationId;
    };

    var _fetchQuestions = function () {
      answers = {};
      questions = [];
      UI.loading = true;

      $http.get(_getUrl())
        .success(function(resp) {      
          UI.loading = false;
          UI.clearError();

          $scope.questionNumber = 1;
          questions = $scope.questions = resp;
          $scope.currentQuestion = resp[0];
        })
        .error(function (e) {
          UI.loading = false;
          var data = e.data || {};
          UI.setError(data.ExceptionMessage || 'An error has occurred.');
        });
    };

    // A poor man's state machine - transition between various application states
    var _setState = function (state) {
      var oldState = $scope.state;
      $scope.state = state;      

      UI.showButtons = (state == 'questions');

      if (oldState != 'questions' && state == 'questions') {
        _fetchQuestions();
      }
    };

    var _submit = function () {
      UI.clearError();

      _setState('pending');

      // Convert to correct format for backend
      var questions = $scope.questions, i = 0, j;

      for (; i < questions.length; i++) {
        var question = questions[i];
        for (j = 0; j < question.Answers.length; j++) {
          var answer = question.Answers[j];
          answer.IsAnswer = answer.AnswerId == answers[question.QuestionId];
        }
      }

      $http.post(_getUrl(), questions)
        .success(function (resp) {
          if (resp && !resp.Success) {
            _setState('failed');
          }
        })
        .error(function (e) {
          UI.setError('An error occurred while submitting your questions. Click retry to try again.', _submit);
        });
    };

    $scope.booting = false;
    $scope.UI = UI;
    $scope.answerSet = false;
    
    $scope.App = {
      stepId: 4
    };

    $scope.questionNumber = 0;

    $scope.hasPrevious = function () {
      return $scope.questionNumber > 1;
    };

    $scope.hasNext = function () {
      return questions.length > $scope.questionNumber;
    };

    $scope.retry = function () {
      _setState('questions');
    };

    $scope.next = function () {      
      if ($scope.hasNext()) {
        $scope.currentQuestion = questions[$scope.questionNumber++];
        $scope.answerSet = $scope.currentQuestion.QuestionId in answers;
      } else {
        _submit();
      }
    };

    $scope.previous = function () {
      if ($scope.hasPrevious()) {
        $scope.answerSet = true;
        $scope.questionNumber--;
        $scope.currentQuestion = questions[$scope.questionNumber - 1];
      }
    };

    $scope.nextLabel = function () {
      if ($scope.hasNext()) {
        return "Question " + ($scope.questionNumber + 1);
      } else {
        return "Submit Questions";
      }
    };

    $scope.previousLabel = function () {
      return "Question " + ($scope.questionNumber - 1);
    };

    $scope.setAnswer = function (qId, aId) {
      $scope.answerSet = true;
      answers[qId] = aId;
    };

    $scope.init = function (_applicationId) {
      applicationId = _applicationId;
      _setState('questions');
    };

  }]);

}(this.angular));