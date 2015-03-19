'use strict';
 
/**
 * @ngdoc function
 * @name bekApp.service:ExportService
 * @description
 * # ExportService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('ExportService', [ '$http', '$log',
    function ($http, $log) {
    
    function downloadFile(data, status, headers, fileType, httpPath) {

      var octetStreamMime = 'application/octet-stream';
      var success = false;

      // Get the headers
      headers = headers();

      // Get the filename from the x-filename header or default to 'download.bin'
      // var filename = headers['x-filename'] || 'download.bin';

      var filename;

      if (fileType === 'CSV') {
        filename = 'export.csv';
      } else if (fileType === 'EXCEL') {
        filename = 'export.xlsx';
      } else if (fileType === 'PDF') {
        filename = 'export.pdf';
      }else {
        filename = 'export.txt';
      }

      // Determine the content type from the header or default to 'application/octet-stream'
      var contentType = headers['content-type'] || octetStreamMime;

      try {
        // Try using msSaveBlob if supported
        $log.debug('Trying saveBlob method ...');
        var blob = new Blob([data], { type: contentType });
        if(navigator.msSaveBlob) {
          navigator.msSaveBlob(blob, filename);
        }
        else {
          // Try using other saveBlob implementations, if available
          var saveBlob = navigator.webkitSaveBlob || navigator.mozSaveBlob || navigator.saveBlob || saveAs;
          if(saveBlob === undefined) {
            throw 'Not supported';
          }
          saveBlob(blob, filename);
        }
        $log.debug('saveBlob succeeded');
        success = true;
      } catch(ex) {
        $log.debug('saveBlob method failed with the following exception:');
        $log.debug(ex);
      }

      if(!success) {
        // Get the blob url creator
        var urlCreator = window.URL || window.webkitURL || window.mozURL || window.msURL;
        if(urlCreator) {
          // Try to use a download link
          var link = document.createElement('a');
          if('download' in link) {
            // Try to simulate a click
            try {
              // Prepare a blob URL
              $log.debug('Trying download link method with simulated click ...');
              var blob = new Blob([data], { type: contentType }); // jshint ignore:line
              var url = urlCreator.createObjectURL(blob);
              link.setAttribute('href', url);

              // Set the download attribute (Supported in Chrome 14+ / Firefox 20+)
              link.setAttribute('download', filename);

              // Simulate clicking the download link
              var event = document.createEvent('MouseEvents');
              event.initMouseEvent('click', true, true, window, 1, 0, 0, 0, 0, false, false, false, false, 0, null);
              link.dispatchEvent(event);
              $log.debug('Download link method with simulated click succeeded');
              success = true;

            } catch(ex) {
              $log.debug('Download link method with simulated click failed with the following exception:');
              $log.debug(ex);
            }
          }

          if(!success) {
            // Fallback to window.location method
            try {
              // Prepare a blob URL
              // Use application/octet-stream when using window.location to force download
              $log.debug('Trying download link method with window.location ...');
              var blob = new Blob([data], { type: octetStreamMime }); // jshint ignore:line
              var url = urlCreator.createObjectURL(blob); // jshint ignore:line
              window.location = url;
              $log.debug('Download link method with window.location succeeded');
              success = true;
            } catch(ex) {
              $log.debug('Download link method with window.location failed with the following exception:');
              $log.debug(ex);
            }
          }
        }
      }

      if(!success) {
        // Fallback to window.open method
        $log.debug('No methods worked for saving the arraybuffer, using last resort window.open');
        window.open(httpPath, '_blank', '');
      }
    }

    var Service = {

      /* 
      httpPath : string - url used to export file
      config   : obj - selected options and fields for export, see example

      CONFIG OBJECT EXAMPLE
      {
        selectedtype: "CSV",

        // only custom exports should have this object
        fields: [{
          field: "Notes",
          label: "Note",
          order: 0,
          selected: false,
        }, {
          field: "ItemNumber",
          label: "Item",
          order: 1,
          selected: true,
        }]
      }
      */

      /*
      code from this Stack Overflow question
      http://stackoverflow.com/questions/24080018/download-file-from-a-webapi-method-using-angularjs
      */

      export: function(httpPath, config) {

        return $http.post(httpPath, config, { responseType: 'arraybuffer' })
          .success(function(data, status, headers) {

            var selectedType;
            // special config structure for invoices
            if (config.export) {
              selectedType = config.export.selectedtype;

            // default config structure
            } else {
              selectedType = config.selectedtype;              
            }

            downloadFile(data, status, headers, selectedType, httpPath);
          });
      },

      print: function(printPromise) {
        return printPromise.then(function(response) {
          downloadFile(response.data, response.status, response.headers, 'PDF');
        });
      }

    };
 
    return Service;
 
  }]);