'use strict';

angular.module('bekApp')
  .controller('NotificationsController', ['$scope', '$modal', 'Constants', 'NotificationService', 'PagingModel',
    function ($scope, $modal, Constants, NotificationService, PagingModel) {

  function setNotifications(data) {
    $scope.notifications = data.results;
    $scope.totalNotifications = data.totalResults;

    // mark messages read
    NotificationService.updateUnreadMessages(angular.copy(data.results));
  }
  function appendNotifications(data) {
    $scope.notifications = $scope.notifications.concat(data.results);
  }
  function startLoading() {
    $scope.loadingResults = true;
  }
  function stopLoading() {
    $scope.loadingResults = false;
  }

  $scope.sort = {
    field: 'messagecreatedutc',
    sortDescending: true
  };

  var notificationPagingModel = new PagingModel( 
    NotificationService.getMessages, 
    setNotifications,
    appendNotifications,
    startLoading,
    stopLoading,
    null,
    $scope.sort
  );

  notificationPagingModel.loadData();
    
  $scope.filterNotifications = function(filterFields) {
    notificationPagingModel.filterData(filterFields);
  };
  $scope.clearFilters = function() {
    $scope.filterFields = {};
    notificationPagingModel.clearFilters();
  };
  $scope.infiniteScrollLoadMore = function() {
    notificationPagingModel.loadMoreData($scope.notifications, $scope.totalNotifications, $scope.loadingResults);
  };
  $scope.sortNotifications = function(field, sortDescending) {
    $scope.sort = {
      field: field,
      sortDescending: sortDescending
    };
    notificationPagingModel.sortData($scope.sort);
  };

  $scope.showAdditionalInfo = function(notification) {
    var modalInstance = $modal.open({
      templateUrl: 'views/modals/notificationdetailsmodal.html',
      controller: 'NotificationDetailsModalController',
      windowClass: 'color-background-modal',
      scope: $scope,
      resolve: {
        notification: function() {
          return notification;
        }
      }
    });
  };

}]);
