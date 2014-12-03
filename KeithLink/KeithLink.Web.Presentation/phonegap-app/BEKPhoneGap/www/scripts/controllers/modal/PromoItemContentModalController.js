'use strict';

angular.module('bekApp')
.controller('PromoItemContentModalController', ['$scope', '$modalInstance', 'promoItem', 
  function ($scope, $modalInstance, promoItem) {

  $scope.item = promoItem;

  $scope.cancel = function () {
    $modalInstance.dismiss('cancel');
  };

}]);