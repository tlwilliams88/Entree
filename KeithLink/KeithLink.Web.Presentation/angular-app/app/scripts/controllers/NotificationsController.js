'use strict';

angular.module('bekApp')
  .controller('NotificationsController', ['$scope', '$modal', 'Constants', 'NotificationService',
    function ($scope, $modal, Constants, NotificationService) {
    

  function loadNotifications(params) {
    $scope.loadingResults = true;
    NotificationService.getMessages(params).then(function(data) {
      $scope.loadingResults = false;
      $scope.notifications = $scope.notifications.concat(data.results);
      $scope.totalNotifications = data.totalResults;
      // mark messages read
      NotificationService.updateUnreadMessages(angular.copy(data.results));
    });
  }

  $scope.infiniteScrollLoadMore = function() {
    if (($scope.notifications && $scope.notifications.length >= $scope.totalResults) || $scope.loadingResults) {
      return;
    }

    $scope.notificationParams.from += $scope.notificationParams.size;

    loadNotifications($scope.notificationParams);
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

  $scope.sortBy = 'messagecreatedutc';
  $scope.sortOrder = true;

  $scope.notificationParams = {
    size: Constants.infiniteScrollPageSize,
    from: 0,
    sort: {
      sfield: 'messagecreatedutc',
      sdir: 'desc'
    },
    filter: [
      // {
      //   ffield: 'name',
      //   fvalue: 'value'
      // }
    ]
  };

  $scope.notifications = [];

  loadNotifications($scope.notificationParams);


  }]);
