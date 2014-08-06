'use strict';
 
/**
 * @ngdoc function
 * @name bekApp.service:ListService
 * @description
 * # ListService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('ListService', function ($http) {
   
    var Service = {

      getAllLists: function() {
        return $http.get('/list').then(function(response) {
          return response.data;
        });
      },

      getList: function(listId) {
        return $http.get('/list/' + listId).then(function(response) {
          return response.data;
        });
      },

      createList: function() {
        return $http.post('/list').then(function(response) {
          return response.data;
        });
      },

      getAllLabels: function() {
        return $http.get('/list/labels').then(function(response) {
          return response.data;
        });
      },

      getLabelsForList: function(listId) {
        return $http.get('/list/' + listId + '/labels').then(function(response) {
          return response.data;
        });
      }

    };

    return Service;
 
  });