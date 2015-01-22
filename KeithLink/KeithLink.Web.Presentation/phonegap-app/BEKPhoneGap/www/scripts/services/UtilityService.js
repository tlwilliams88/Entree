'use strict';
 
/**
 * @ngdoc function
 * @name bekApp.service:UtilityService
 * @description
 * # UtilityService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('UtilityService', [ '$q', function ($q) {

    function isUsedName(namesList, name, number) {
      return namesList.indexOf(name + ' ' + number) > -1;
    }

    var Service = {
      // accepts nameText as string (List/Cart) and collection of objects to check if the name is used
      generateName: function(nameText, collection) {
        var name = 'New ' + nameText,
          number = 0;

        var namesList = [];
        angular.forEach(collection, function(item, index) {
          namesList.push(item.name);
        });

        var isNameUsed = isUsedName(namesList, name, number);
        while (isNameUsed) {
          number++;
          isNameUsed = isUsedName(namesList, name, number);
        }

        return name + ' ' + number;
      },

      // accepts a collection to search, fieldName (string) and matcher
      // loops through items in collection looking for an object where property fieldName equals matcher
      findObjectByField: function(collection, fieldName, matcher) {
        var obj;
        angular.forEach(collection, function(item, index) {
          if (item[fieldName] === matcher) {
            obj = item;
          }
        });
        return obj;
      },

      // accepts collection of objects and the fieldName (array of strings) to be deleted
      deleteFieldFromObjects: function(collection, fieldNames) {
        angular.forEach(collection, function(item, index) {
          angular.forEach(fieldNames, function(name, index) {
            delete item[name];
          });
        });
      },

      resolvePromise: function(promise) {
        var deferred = $q.defer();

        promise.then(function(response) {
          var data = response.data;
          if (data.successResponse) {
            deferred.resolve(data.successResponse);
          } else {
            deferred.reject(data.errorMessage);
          }
        }, function(error) {
          deferred.reject('An error occurred.');
        });
        return deferred.promise;
      }
    };

    return Service;
 
  }]);