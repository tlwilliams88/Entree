'use strict';

angular.module('bekApp')
  .controller('DocumentController', ['$scope', 'DocumentService', '$filter',
    function ($scope, DocumentService, $filter) {


    $scope.documents = [];
    $scope.previousPage = [];
    $scope.identifier = $scope.selectedUserContext.customer.customerNumber + '-' + $scope.selectedUserContext.customer.customerBranch;  
    $scope.breadcrumbs = [{name: $scope.selectedUserContext.customer.customerName, url: $scope.identifier}];
    $scope.previousPage.push($scope.identifier);
    $scope.currentPage = $scope.identifier; 
    var orderBy = $filter('orderBy');

    $scope.getDocuments = function(url) {
      DocumentService.getAllDocuments(url).then(function(documents){
        $scope.documents = documents;
      });
    }

    $scope.getDocuments($scope.identifier);

    $scope.getFolder = function(doc) {

      $scope.previousPage.push($scope.currentPage);
      $scope.currentPage = doc.url; 
      var existingBreadcrumb = null;

      $scope.breadcrumbs.forEach(function(crumb, index){
        if (crumb.url === doc.url){
          existingBreadcrumb = index;
        }
      });

      if(existingBreadcrumb != null){
        $scope.breadcrumbs = $scope.breadcrumbs.slice(0, existingBreadcrumb + 1);
      }
      else{
        $scope.breadcrumbs.push(doc);
      }
      $scope.getDocuments(doc.url)
    };

    $scope.sort = function (field, oldSortDescending) {
      var sortDescending = !oldSortDescending;
      $scope.sortField = field;
      $scope.sortDescending = sortDescending;
      $scope.documents = orderBy($scope.documents, field, sortDescending);
    };

}]);
