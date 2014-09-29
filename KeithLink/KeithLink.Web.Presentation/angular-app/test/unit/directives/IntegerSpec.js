'use strict';

describe('Directive: Integer', function() {
  beforeEach(module('bekApp'));

  var $httpBackend;
  beforeEach(inject(function($injector) {
    $httpBackend = $injector.get('$httpBackend');
    $httpBackend.when('GET', 'views/register.html').respond();
  }));
  
  var $scope, form;

  beforeEach(inject(function($compile, $rootScope) {
    $scope = $rootScope;
    var element = angular.element(
      '<form name="form">' +
        '<input ng-model="model.somenum" type="text" name="somenum" integer />' +
      '</form>'
    );
    $scope.model = { somenum: null };
    $compile(element)($scope);
    $scope.$digest();
    form = $scope.form;
  }));

  it('should pass with integer', function() {
    var testInteger = 3;
    form.somenum.$setViewValue(testInteger);
    expect($scope.model.somenum).toEqual(testInteger);
    expect(form.somenum.$valid).toBe(true);
  });
  
  it('should ignore letters', function() {
    form.somenum.$setViewValue('a');
    expect($scope.model.somenum).toEqual('');
    expect(form.somenum.$valid).toBe(true);
  });
  
  it('should ignore negative', function() {
    form.somenum.$setViewValue('-1');
    expect($scope.model.somenum).toEqual('1');
    expect(form.somenum.$valid).toBe(true);
  });
  
  it('should ignore decimals', function() {
    form.somenum.$setViewValue('1.5');
    expect($scope.model.somenum).toEqual('15');
    expect(form.somenum.$valid).toBe(true);
  });
});