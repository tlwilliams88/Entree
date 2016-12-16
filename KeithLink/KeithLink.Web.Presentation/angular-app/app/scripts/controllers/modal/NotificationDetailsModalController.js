'use strict';

angular.module('bekApp')
.controller('NotificationDetailsModalController', ['$scope', '$modalInstance', '$sce', '$state', 'notification', 'NotificationService',
  function ($scope, $modalInstance, $sce, $state, notification, NotificationService) {

  $scope.notification = notification;
  $scope.originalBody = notification.body;
  $scope.notification.body = $sce.trustAsHtml(notification.body);

  $scope.closeModal = function() {
    $scope.notification.body = $scope.originalBody;
    $(document.body).removeClass('NotificationPrinting');
    $modalInstance.dismiss('cancel');
  };

  $scope.goToPage = function(stateName) {
    $modalInstance.close();
    $state.go(stateName);
  };

  $scope.printNotificationModal =  function() {
    var originalNotification = document.getElementById('printThis - ' + notification.id),
        printNotification;

    printElement(originalNotification);
    
    printNotification = document.querySelector('#printSection');
    
    window.print();

    document.querySelector('#printSection').remove();
  };

  $scope.emailNotification = function(notification, emailaddress) {
    var data = {
      id: notification,
      emailaddress: emailaddress
    };

    $scope.sendToEmailAddress = '';

    NotificationService.forwardNotification(data);
  };

  function printElement(elem) {
      var domClone = elem.cloneNode(true),
          $printSection = document.getElementById('printSection');
      
      if (!$printSection) {
          var $printSection = document.createElement('div');
          $printSection.id = 'printSection';
          document.body.appendChild($printSection);
      }

      $(document.body).addClass('NotificationPrinting');
      
      $printSection.innerHTML = '';
      
      $printSection.appendChild(domClone);
  }

}]);