'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:UserProfileService
 * @description
 * # UserProfileService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('UserProfileService', [ function () {

    var Service = {
      // profile: {'UserId':'{4065067c-bae0-41cd-a2f9-e89f377d4386}','UserName':'sabroussard@somecompany.com','FirstName':'Steven','LastName':'Broussard','EmailAddress':'','PhoneNumber':'','CustomerName':'','stores':[{'name':'Dallas Ft Worth','customerNumber':453234,'branchId':'fdf'},{'name':'San Antonio','customerNumber':534939,'branchId':'fsa'},{'name':'Amarillo','customerNumber':534939,'branchId':'fam'}],'salesRep':{'id':34234,'name':'Heather Hill','phone':'(888) 912-2342','email':'heather.hill@ben.e.keith.com','imageUrl':'../images/placeholder-dsr.jpg'},'currentLocation':{'name':'Dallas Ft Worth','customerNumber':453234,'branchId':'fdf'}},

      profile: {},

      getProfile: function() {
        return Service.profile;
      },

      setProfile: function(newProfile) {
        angular.copy(newProfile, Service.profile);
      },

      getCurrentLocation: function() {
        return Service.profile.currentLocation;
      },

      createUser: function() {

      },

      updateUser: function() {

      }
    };

    return Service;

  }]);
