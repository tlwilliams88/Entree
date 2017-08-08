'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:CategoryService
 * @description
 * # CategoryService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('BranchService', ['$http', '$q', 'localStorageService', function ($http, $q, localStorageService) {

  var Service = {

    // gets all available branches, does not required Authentication header
    /*
    [
      {
        "id":"FAQ",
        "name":"Albuquerque",
        "branchsupport":{
          "branchid":"FAQ",
          "supportphonenumber":"5056819950",
          "tollfreenumber":"8006752949x2308",
          "email":"Faq-dis-mis@benekeith.com"
        }
      },{
        "id":"FAM",
        "name":"Amarillo",
        "branchsupport":{
          "branchid":"FAM",
          "supportphonenumber":"8064684555",
          "tollfreenumber":"8006589790x4555",
          "email":"Fam-dis-mis@benekeith.com"
        }
      }
    ]
    */
    getBranches: function() {
      var deferred = $q.defer(),
          branches = localStorageService.get('branches');

      if(branches){
        localStorageService.remove('branches');
      }

      $http.get('/catalog/divisions').then(function (response) {
        localStorageService.set('branches', response.data.successResponse);
        Service.branches = response.data.successResponse;
        return deferred.resolve(response.data.successResponse);
      });

      return deferred.promise;
    }
  };

  return Service;

}]);
