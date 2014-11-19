'use strict';

angular.module('bekApp')
  .controller('NotificationsController', ['$scope', '$modal', 'Constants', 'NotificationService',
    function ($scope, $modal, Constants, NotificationService) {
    

  function loadNotifications(params) {
    $scope.loadingResults = true;
    return NotificationService.getMessages(params).then(function(data) {
      $scope.loadingResults = false;
      $scope.totalNotifications = data.totalResults;
      
      // mark messages read
      // NotificationService.updateUnreadMessages(angular.copy(data.results));

      return data.results;
    });
  }

  function setNotifications(notifications) {
    $scope.notifications = notifications;
  }

  $scope.filterNotifications = function(filterFields) {
    var filterList = [];
    for(var propertyName in filterFields) {
      if (filterFields[propertyName] && filterFields[propertyName] !== '') {
        var filterObject = {
          field: propertyName,
          value: filterFields[propertyName] 
        };
        filterList.push(filterObject);  
      }
    }

    $scope.notificationParams.filter = filterList;

    // reset paging
    $scope.notificationParams.size = Constants.infiniteScrollPageSize;
    $scope.notificationParams.from = 0;

    loadNotifications($scope.notificationParams).then(setNotifications);

  };

  $scope.clearFilters = function() {
    $scope.filterFields = {};
    $scope.notificationParams.filter = [];
    loadNotifications($scope.notificationParams).then(setNotifications);
  };

  $scope.sortNotifications = function(field, order) {
    $scope.sortOrder = order;
    var sortObject = {
      field: field
    };
    if (order === true) {
      sortObject.order = 'desc';
    } else {
      sortObject.order = 'asc';
    }

    var sort = [
      sortObject,
      // always sort by date desc
      {
        field: 'messagecreatedutc',
        order: 'desc'
    }];

    // reset paging
    $scope.notificationParams.size = Constants.infiniteScrollPageSize;
    $scope.notificationParams.from = 0;

    $scope.notificationParams.sort = sort;

    loadNotifications($scope.notificationParams).then(setNotifications);
  };

  $scope.infiniteScrollLoadMore = function() {
    if (($scope.notifications && $scope.notifications.length >= $scope.totalResults) || $scope.loadingResults) {
      return;
    }

    $scope.notificationParams.from += $scope.notificationParams.size;

    loadNotifications($scope.notificationParams).then(function(notifications) {
      $scope.notifications = $scope.notifications.concat(notifications);
    });
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
    sort: [{
      field: 'messagecreatedutc',
      order: 'desc'
    }],
    filter: [
      // {
      //   field: 'name',
      //   value: 'value'
      // }
    ]
  };

  $scope.notifications = [];

  loadNotifications($scope.notificationParams).then(setNotifications);


  }]);
