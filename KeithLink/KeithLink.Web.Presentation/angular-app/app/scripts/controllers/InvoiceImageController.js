  'use strict';

angular.module('bekApp')
  .controller('InvoiceImageController', ['$scope', 'images', 'ENV',
    function ($scope, images, ENV) {

  $scope.images = images;
  $scope.isMobileApp = ENV.mobileApp;

}]);
