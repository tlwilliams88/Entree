'use strict';

describe('Directive: MatchInput', function() {
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
    form.confirmEmail.$setViewValue($scope.user.email);
    expect($scope.model.confirmEmail).to.equal($scope.user.email);
    expect(form.confirmEmail.$valid).to.be.true;
  });

  it('should fail when input does not match', function() {
    form.confirmEmail.$setViewValue($scope.user.email);
    expect($scope.model.confirmEmail).to.equal($scope.user.email);
    expect(form.confirmEmail.$valid).to.be.true;
  });

  it('should fail when matching input changes', function() {
    form.confirmEmail.$setViewValue($scope.user.email);
    expect(form.confirmEmail.$valid).to.be.true;

    $scope.user.email = 'newemail@gmail.com';
    expect($scope.model.confirmEmail).not.to.equal($scope.user.email);
    expect(form.confirmEmail.$valid).to.be.true;
  });
});
