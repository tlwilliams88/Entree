'use strict';

describe('Directive: UpdateQuantityFromParlevel', function() {
  beforeEach(module('bekApp'));

  var $httpBackend;
  beforeEach(inject(function($injector) {
    $httpBackend = $injector.get('$httpBackend');
    $httpBackend.when('GET', 'views/register.html').respond();
  }));
  
  var $scope, form, element;

  beforeEach(inject(function($compile, $rootScope) {
    $scope = $rootScope;
    element = angular.element(
      '<form name="form">' +
        '<input ng-model="model.onhand" type="text" name="onhand" update-quantity-from-parlevel parlevel="item.parlevel" quantity="item.quantity" />' +
      '</form>'
    );
    $scope.model = { };
    $scope.item = {
      parlevel: 3
    };
    $compile(element)($scope);
    $scope.$digest();
    form = $scope.form;
  }));

  it('should subtract onhand amount from parlevel and set quantity', function() {
    form.onhand.$setViewValue('2');
    $scope.$digest();
    expect($scope.item.quantity).toBe(1);
  });

  it('should take an empty string as 0 and set quantity equal to parlevel', function() {
    form.onhand.$setViewValue('');
    $scope.$digest();
    expect($scope.item.quantity).toBe(3);
  });

  it('should not set quantity when onhand value is above parlevel', function() {
    form.onhand.$setViewValue('5');
    $scope.$digest();
    expect($scope.item.quantity).toBe(null);
  });

  it('should not set quantity when onhand value is equal to parlevel', function() {
    form.onhand.$setViewValue('3');
    $scope.$digest();
    expect($scope.item.quantity).toBe(null);
  });
  
});