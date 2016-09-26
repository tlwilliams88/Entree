'use strict';

angular.module('bekApp')
.controller('NotificationDetailsModalController', ['$scope', '$modalInstance', '$sce', '$state', 'notification',
  function ($scope, $modalInstance, $sce, $state, notification) {

  $scope.notification = notification;
  $scope.originalBody = notification.body;
  $scope.notification.body = $sce.trustAsHtml(notification.body);

  $scope.closeModal = function() {
    $scope.notification.body = $scope.originalBody;
    $modalInstance.dismiss('cancel');
  };

  $scope.goToPage = function(stateName) {
    $modalInstance.close();
    $state.go(stateName);
  };

  $scope.printNotificationModal =  function () {
    var originalNotification = document.getElementById("printThis - " + notification.id),
        printNotification;

    printElement(originalNotification);
    
    printNotification = document.querySelector("#printSection");
    
    window.print();

    document.querySelector("#printSection").remove();
  };

  function printElement(elem) {
      var domClone = elem.cloneNode(true),
          $printSection = document.getElementById("printSection");
      
      if (!$printSection) {
          var $printSection = document.createElement("div");
          $printSection.id = "printSection";
          document.body.appendChild($printSection);
      }
      
      $printSection.innerHTML = "";
      
      $printSection.appendChild(domClone);
  }

}]);