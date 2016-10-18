'use strict';

angular.module('bekApp')
  .controller('NotificationsController', ['$scope', '$modal', 'NotificationService', 'PagingModel', 'UtilityService',
    function ($scope, $modal, NotificationService, PagingModel, UtilityService) {

  function markMessagesRead(messages) {
    NotificationService.updateUnreadMessages(angular.copy(messages));
  }
  function setNotifications(data) {
    $scope.notifications = data.results;
    $scope.totalNotifications = data.totalResults;
    markMessagesRead(data.results);
  }
  function appendNotifications(data) {
    $scope.notifications = $scope.notifications.concat(data.results);
    markMessagesRead(data.results);
  }
  function startLoading() {
    $scope.loadingResults = true;
  }
  function stopLoading() {
    $scope.loadingResults = false;
  }

  $scope.isMobile = UtilityService.isMobileDevice();
  var size = 'lg';
  if($scope.isMobile){
    size = 'md';
  }

  // Remove focus from notifications icon in header bar
  document.activeElement.blur();

  $scope.sort = {
    field: 'messagecreated',
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
      size: size,
      resolve: {
        notification: function() {
          return notification;
        }
      }
    });
  };

}]);
