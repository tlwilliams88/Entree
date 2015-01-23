'use strict';

angular.module('bekApp')
  .factory('PrintService', ['$rootScope', '$compile', '$http', '$timeout', 
    function ($rootScope, $compile, $http, $timeout) {
  
  var printHtml = function (html) {
    var hiddenFrame = jQuery('<iframe style="display: none"></iframe>').appendTo('body')[0];
    hiddenFrame.contentWindow.printAndRemove = function() {
      hiddenFrame.contentWindow.print();
      jQuery(hiddenFrame).remove();
    };
    var htmlContent = '<!doctype html>'+
      '<html>'+
          '<body onload="printAndRemove();">' +
              html +
          '</body>'+
      '</html>';
    var doc = hiddenFrame.contentWindow.document.open('text/html', 'replace');
    doc.write(htmlContent);
    doc.close();
  };

  var openNewWindow = function (html) {
    var newWindow = window.open('printTest.html');
    newWindow.addEventListener('load', function(){ 
      jQuery(newWindow.document.body).html(html);
    }, false);
  };

  var print = function (templateUrl, data) {
    $http.get(templateUrl).success(function(template){
      var printScope = $rootScope.$new();
      angular.extend(printScope, data);
      var element = $compile(jQuery('<div>' + template + '</div>'))(printScope);
      var waitForRenderAndPrint = function() {
        if(printScope.$$phase || $http.pendingRequests.length) {
          $timeout(waitForRenderAndPrint);
        } else {
          // Replace printHtml with openNewWindow for debugging
          // openNewWindow(element.html());
          printHtml(element.html());
          printScope.$destroy();
        }
      };
      waitForRenderAndPrint();
    });
  };

  return {
    print: print
  };

}]);
