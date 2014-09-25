'use strict';

describe('Directive: CheckDuplicateField', function () {

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
        '<input ng-model="model.title" type="text" name="title" check-duplicate-field="title" collection="list" />' +
      '</form>'
    );
    $scope.model = { title: 'Second Name' };
    $scope.list = [
      { title: 'First Name' },
      { title: 'Second Name' },
      { title: 'Third Name' }
    ];
    $compile(element)($scope);
    $scope.$digest();
    form = $scope.form;
  }));

  it('should pass with new title', function() {
    var testTitle = 'New Name';
    form.title.$setViewValue(testTitle);
    expect($scope.model.title).toEqual(testTitle);
    expect(form.title.$valid).toBe(true);
  });

  it('should pass when model has not changed', function() {
    var testTitle = 'Second Name';
    form.title.$setViewValue(testTitle);
    expect($scope.model.title).toEqual(testTitle);
    expect(form.title.$valid).toBe(true);
  });

  // it('should pass when name is changed back to the original model value', function() {
  //   var testTitle = 'Second Name';
  //   form.title.$setViewValue('New Name');
  //   form.title.$setViewValue(testTitle);
  //   expect($scope.model.title).toEqual(testTitle);
  //   expect(form.title.$valid).toBe(true);
  // });

  it('should fail with a duplicate title', function() {
    var testTitle = 'First Name';
    form.title.$setViewValue(testTitle);
    expect($scope.model.title).toEqual($scope.model.title);
    expect(form.title.$valid).toBe(false);
  });

  it('should fail with a duplicate title different case', function() {
    var testTitle = 'first name';
    form.title.$setViewValue(testTitle);
    expect($scope.model.title).toEqual($scope.model.title);
    expect(form.title.$valid).toBe(false);
  });
});