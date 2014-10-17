'use strict';

describe('Directive: AllowOnePositiveDecimal', function () {

  // load the controller's module
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
        '<input ng-model="model.somenum" type="text" name="somenum" allow-one-positive-decimal />' +
      '</form>'
    );
    $scope.model = { somenum: null };
    $compile(element)($scope);
    $scope.$digest();
    form = $scope.form;
  }));

  it('should pass with integer', function() {
    var testNum = 3;
    form.somenum.$setViewValue(testNum);
    expect($scope.model.somenum).toEqual(testNum);
    expect(form.somenum.$valid).toBe(true);
  });

  it('should pass with a decimal with one decimal place', function() {
    var testNum = 3.0;
    form.somenum.$setViewValue(testNum);
    expect($scope.model.somenum).toEqual(testNum);
    expect(form.somenum.$valid).toBe(true);
  });

  it('should pass when blank', function() {
    var testNum = '';
    form.somenum.$setViewValue(testNum);
    expect(form.somenum.$valid).toBe(true);
  });

  it('should fail with a number ending in a decimal', function() {
    var testNum = '3.';
    form.somenum.$setViewValue(testNum);
    expect($scope.model.somenum).toBeUndefined();
    expect(form.somenum.$valid).toBe(false);
  });

  it('should fail with a negative number', function() {
    var testNum = -3;
    form.somenum.$setViewValue(testNum);
    expect($scope.model.somenum).toBeUndefined();
    expect(form.somenum.$valid).toBe(false);
  });

  it('should fail with a decimal with more than one decimal place', function() {
    var testNum = 3.45;
    form.somenum.$setViewValue(testNum);
    expect($scope.model.somenum).toBeUndefined();
    expect(form.somenum.$valid).toBe(false);
  });
});