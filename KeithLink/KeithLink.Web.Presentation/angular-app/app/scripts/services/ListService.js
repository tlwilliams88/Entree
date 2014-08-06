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

      getAllLists: function(onlyHeaders) {
        onlyHeaders = typeof onlyHeaders !== 'undefined' ? onlyHeaders : false;

        return $http.get('/list', {
          params: {
            header: onlyHeaders
          }
        }).then(function(response) {
          return response.data;
        });
      },

      getList: function(listId) {
        return $http.get('/list/' + listId).then(function(response) {
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
      },

      createList: function(list) {
        return $http.post('/list', list).then(function(response) {
          return response.data; // return listId
        });
      },

      deleteList: function(listId) {
        return $http.delete('/list/' + listId);
      },

      addItem: function(listId, item) {
        return $http.post('/list/' + listId + '/item', item).then(function(response) {
          debugger;
          return response.data;
        });
      },

      updateItem: function(listId, item) {
        return $http.put('/list/' + listId + '/item', item).then(function(response) {
          debugger;
        });
      },

      deleteItem: function(listId, itemId) {
        return $http.delete('/list/' + listId + '/item/' + itemId).then(function(response) {
          debugger;
        });
      },

      updateList: function(list) {
        return $http.put('/list', list).then(function(response) {
          
        });
      }

    };

    return Service;
 
  });