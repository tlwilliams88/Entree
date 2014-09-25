'use strict';

xdescribe('Directive: MatchInput', function() {
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
        '<input ng-model="model.confirmEmail" type="text" name="confirmEmail" match-input="user.email" />' +
      '</form>'
    );
    $scope.model = { confirmEmail: null };
    $scope.user = {
      email: 'test@gmail.com'
    };
    $compile(element)($scope);
    $scope.$digest();
    form = $scope.form;
  }));

  it('should pass when input matches given value', function() {
    var testValue = 'test@gmail.com';
    form.confirmEmail.$setViewValue(testValue);
    expect($scope.model.confirmEmail).toEqual(testValue);
    expect(form.confirmEmail.$valid).toBe(true);
  });

  it('should fail when input does not match', function() {
    var testValue = 'email@gmail.com';
    form.confirmEmail.$setViewValue(testValue);
    expect($scope.model.confirmEmail).toEqual(testValue);
    expect(form.confirmEmail.$valid).toBe(false);
  });

  it('should fail when matching input changes', function() {
    var testValue = 'test@gmail.com';
    form.confirmEmail.$setViewValue(testValue);
    expect(form.confirmEmail.$valid).toBe(true);

    $scope.user.email = 'newemail@gmail.com';
    expect($scope.model.confirmEmail).toEqual(testValue);
    expect(form.confirmEmail.$valid).toBe(false);
  });
});