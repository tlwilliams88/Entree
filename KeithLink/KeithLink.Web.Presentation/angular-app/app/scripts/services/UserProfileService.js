'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:UserProfileService
 * @description
 * # UserProfileService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('UserProfileService', function ($http) {
    
    var profile = {
      'name': 'Steven',
      'customerNumber': 12345,
      'imageUrl': '../images/placeholder-user.png',
      'role': 'Owner',
      'phone': 9348234934,
      'location': 'Dallas',
      'stores': [{
        'name': 'Dallas Ft Worth',
        'customerNumber': 453234,
        'branchId': 'fdf'
      }, {
        'name': 'San Antonio',
        'customerNumber': 534939,
        'branchId': 'fsa'
      }, {
        'name': 'Amarillo',
        'customerNumber': 534939,
        'branchId': 'fam'
      }],
      'accountNumber': 9783459,
      'salesRep': {
        'id': 34234,
        'name': 'Heather Hill',
        'phone': '(888) 912-2342',
        'email': 'heather.hill@ben.e.keith.com',
        'imageUrl': '../images/placeholder-dsr.jpg'
      },
      'currentLocation': {
        'name': 'Dallas Ft Worth',
        'customerNumber': 453234,
        'branchId': 'fdf'
      }
    };

    var currentLocation = profile.stores[0];

    var Service = {
      getProfile: function() {
        return profile;
      },

      getCurrentLocation: function() {
        return profile.currentLocation;
      }
    };

    return Service;

  });
