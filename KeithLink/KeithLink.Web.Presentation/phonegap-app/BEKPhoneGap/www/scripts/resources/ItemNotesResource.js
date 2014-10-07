'use strict';

angular.module('bekApp')
  .factory('ItemNotes', [ '$resource', 
  function ($resource) {
    return $resource('/itemnote/:itemNumber', {}, {

      // defaults: SAVE, DELETE

    });
  
  }]);
