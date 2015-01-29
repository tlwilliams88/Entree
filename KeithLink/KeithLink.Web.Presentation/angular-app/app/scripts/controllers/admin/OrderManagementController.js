'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:CatalogController
 * @description
 * # CatalogController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('OrderManagementController', ['$scope', '$state', 'OrderService', '$modal',
    function ($scope, $state, OrderService, $modal) {


        function loadUnconfirmedOrders() {
            $scope.loadingResults = true;
           OrderService.getUnconfirmedOrders()
                .then(function (items) {
                    $scope.unconfirmedorders = items;
                    $scope.loadingResults = false;
                });
        }

        $scope.resendUnconfirmedOrder = function (item) {
          OrderService.resubmitUnconfirmedOrder(item.controlNumber);
        }

        // INIT
        loadUnconfirmedOrders();
  }]);
