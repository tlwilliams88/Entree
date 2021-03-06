'use strict';

angular.module('bekApp')
  .factory('ConfigSettings', [ '$resource', function ($resource) {
    return $resource('/appsettings/', { }, {

    	update: {
        url: '/appsettings',
        method: 'PUT'
      },

      getSetting: {
        url: '/appsetting/featureflag/:setting',
        method: 'GET'
      }

    });

}]);