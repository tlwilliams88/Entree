'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:UserFeedbackService
 * @description
 * # UserFeedbackService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('UserFeedbackService', ['$http', 'UtilityService',
    function ($http, UtilityService) {

      var Service = {

        submitUserFeedback: function (userFeedback) {
          var promise = $http.put('/userfeedback', userFeedback);
          return UtilityService.resolvePromise(promise);
        },

      };

      return Service;

    }]);
