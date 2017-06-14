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
    $scope = $rootScope.$new();
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
    expect($scope.item.quantity).to.equal(1);
  });

  it('should take an empty string as 0 and set quantity equal to parlevel', function() {
    form.onhand.$setViewValue('');
    $scope.$digest();
    expect($scope.item.quantity).to.equal(3);
  });

  it('should round up quantity when given partial onhand amount', function() {
    form.onhand.$setViewValue('1.5');
    $scope.$digest();
    expect($scope.item.quantity).to.equal(2);
  });

  it('should not set quantity when onhand value is above parlevel', function() {
    form.onhand.$setViewValue('5');
    $scope.$digest();
    expect($scope.item.quantity).to.be.null;
  });

  it('should not set quantity when onhand value is equal to parlevel', function() {
    form.onhand.$setViewValue('3');
    $scope.$digest();
    expect($scope.item.quantity).to.be.null;
  });

});
