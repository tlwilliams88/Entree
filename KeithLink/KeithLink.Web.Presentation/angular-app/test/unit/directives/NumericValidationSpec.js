'use strict';

describe('Directive: NumericValidation', function () {

  // load the controller's module
  beforeEach(module('bekApp'));

  var $httpBackend;
  beforeEach(inject(function($injector) {
    $httpBackend = $injector.get('$httpBackend');
    $httpBackend.when('GET', 'views/register.html').respond();
  }));

  var $scope, form;

  beforeEach(inject(function($compile, $rootScope) {
    $scope = $rootScope.$new();
    var element = angular.element(
      '<form name="form">' +
        '<input ng-model="model.somenum" type="text" name="somenum" numeric-validation />' +
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
    expect($scope.model.somenum).to.equal(testNum);
    expect(form.somenum.$valid).to.be.true;
  });

  it('should pass with a decimal with one decimal place', function() {
    var testNum = 4.0;
    form.somenum.$setViewValue(testNum);
    expect($scope.model.somenum).to.equal(testNum);
    expect(form.somenum.$valid).to.be.true;
  });

  it('should pass when blank', function() {
    var testNum = '';
    form.somenum.$setViewValue(testNum);
    expect(form.somenum.$valid).to.be.true;
  });

  it('should fail with a number ending in a decimal', function() {
    var testNum = '5.';
    form.somenum.$setViewValue(testNum);
    expect($scope.model.somenum).to.be.undefined;
    expect(form.somenum.$valid).to.be.false;
  });

  it('should take the absolute value of a negative number', function() {
    var testNum = -8;
    form.somenum.$setViewValue(testNum);
    expect($scope.model.somenum).to.equal(8);
    expect(form.somenum.$valid).to.be.true;
  });

  it('should pass with a decimal with more than one decimal place, but truncate it to one place if not indicated to allow two', function() {
    var testNum = 3.45;
    form.somenum.$setViewValue(testNum);
    expect($scope.model.somenum).to.equal(3.4);
    expect(form.somenum.$valid).to.be.true;
  });
});
