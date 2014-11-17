'use strict';

angular.module('bekApp')
  .factory('Marketing', [ '$resource', 
  function ($resource) {
    return $resource('/cms/contentitem', { }, {

      // defaults: SAVE
      
    });
  
  }]);
