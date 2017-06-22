'use strict';

describe('Directive: NoDuplicates', function () {

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
    $rootScope.$$watchers = [];
    var element = angular.element(
      '<form name="form">' +
        '<input ng-model="model.title" type="text" name="title" no-duplicates="title" collection="list" />' +
      '</form>'
    );
    $scope.model = { title: 'Second Name' };
    $scope.list = [
      { title: 'First Name' },
      { title: 'Second Name' },
      { title: 'Third Name' }
    ];
    $compile(element)($scope);
    // $scope.$apply();
    form = $scope.form;
  }));

  it('should pass with new title', function() {
    var testTitle = 'New Name';
    form.title.$setViewValue(testTitle);
    $scope.$digest();
    expect($scope.model.title).to.equal(testTitle);
    expect(form.title.$valid).to.be.true;
  });

  it('should pass when model has not changed', function() {
    var testTitle = 'Second Name';
    form.title.$setViewValue(testTitle);
    $scope.$digest();
    expect($scope.model.title).to.equal(testTitle);
    expect(form.title.$valid).to.be.true;
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
    // $scope.$digest();
    expect($scope.model.title).to.equal($scope.model.title);
    expect(form.title.$valid).to.be.false;
  });

  it('should fail with a duplicate title different case', function() {
    var testTitle = 'first name';
    form.title.$setViewValue(testTitle);
    // $scope.$digest();
    expect($scope.model.title).to.equal($scope.model.title);
    expect(form.title.$valid).to.be.false;
  });
});
