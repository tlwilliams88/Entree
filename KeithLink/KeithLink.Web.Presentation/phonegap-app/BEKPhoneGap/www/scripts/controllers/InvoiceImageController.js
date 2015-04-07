  'use strict';

angular.module('bekApp')
  .controller('InvoiceImageController', ['$scope', 'images',
    function ($scope, images) {

  $scope.images = images;

}]);
