'use strict';
 
/**
 * @ngdoc function
 * @name bekApp.service:NameGeneratorService
 * @description
 * # NameGeneratorService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('NameGeneratorService', [ function () {

    function isUsedName(namesList, name, number) {
      return namesList.indexOf(name + ' ' + number) > -1;
    }

    var Service = {
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
      }
    };

    return Service;
 
  }]);