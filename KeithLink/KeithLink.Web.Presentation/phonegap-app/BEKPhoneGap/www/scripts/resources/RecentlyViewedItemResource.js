'use strict';

angular.module('bekApp')
  .factory('RecentlyViewedItem', [ '$resource', 
  function ($resource) {
    return $resource('/recent/:itemNumber', {}, {

      // defaults: QUERY, SAVE, DELETE

      // DELETE, /recent

    });
  
  }]);
