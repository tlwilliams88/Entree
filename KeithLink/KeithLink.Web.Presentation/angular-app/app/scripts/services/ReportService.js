'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:CustomerService
 * @description
 * # CustomerService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('ReportService', [ '$q', '$http', 'ExportService',  function ($q, $http, ExportService) {

    var categories;

    var Service = {
      itemUsageParams: false,
      getItemUsageReport: function(fromDate, toDate, sortField, sortDir) {
        var deferred = $q.defer();

        var data = {
            params: {
                fromdate: fromDate,
                todate: toDate,
                sortfield: sortField,
                sortdir: sortDir
            }
          };

        $http.get('/report/itemusage', data).then(function(response) {
          var data = response.data;
          if (data.successResponse) {
            deferred.resolve(data.successResponse);
          } else {
            deferred.reject(data.errorMessage);
          }
        });
        return deferred.promise;
      },
      
      /****************
      EXPORT
      ****************/
      getExportConfig: function() {
        return $http.get('/report/itemusage/export').then(function(response) {
          return response.data;
        });
      },

      exportItem: function(config, url) {  
        ExportService.export(url, config);
      }
  };



    return Service;

  }]);
