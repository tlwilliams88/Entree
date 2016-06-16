'use strict';

angular.module('bekApp')
  .factory('ConfigSettings', [ '$resource', function ($resource) {
    return $resource('/appsettings/', { }, {

    	update: {
        url: '/appsettings',
        method: 'PUT'
      },

    });

}]);