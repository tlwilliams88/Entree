'use strict';

angular.module('bekApp')
  .factory('DocumentService', ['$http', '$q', 'localStorageService', function ($http, $q, localStorageService) {

  var Service = {
   
    getAllDocuments: function(identifier) {

      var data = { 
        params: {
          identifier: identifier
        }
      };

      return $http.get('/documents', data).then(function (response) {
        if(response.data){

          var folders = [];
          var files = [];

          response.data.forEach(function(element){
            if(element.type === 'folder'){
              folders.push(element);
            }
            else{
              files.push(element);
            }
          });
          return folders.concat(files);
        }
        else{
          return [];
        }
      });     
    }
  };

  return Service;

}]);
