'use strict';

describe('Controller: HomeController', function () {

  // load the controller's module
  beforeEach(module('bekApp'));

  var MainCtrl,
    scope;

  // Initialize the controller and a mock scope
  beforeEach(inject(function ($controller, $rootScope) {
    scope = $rootScope.$new();
    MainCtrl = $controller('HomeController', {
      $scope: scope
    });
  }));

  it('should...', function() {
    expect(true).toBe(true);
  });

  
});
