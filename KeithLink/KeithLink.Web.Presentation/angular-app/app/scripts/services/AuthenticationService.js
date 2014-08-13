'use strict';

angular.module('bekApp')
  .factory('AuthenticationService', ['$http', 'UserProfileService', function ($http, UserProfileService) {
    
    var isValid = false;

    var Service = {
      login: function(email, password) {
        var params = {
          email: email,
          password: password
        };

        return $http.post('/profile/login', params).then(function (response) {
          var profile = response.data.UserProfiles[0];

          profile.stores = [{
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
          }];

          profile.salesRep = {
            'id': 34234,
            'name': 'Heather Hill',
            'phone': '(888) 912-2342',
            'email': 'heather.hill@ben.e.keith.com',
            'imageUrl': '../images/placeholder-dsr.jpg'
          };

          profile.currentLocation = profile.stores[0];

          UserProfileService.setProfile(profile);

          isValid = true;
        });
      },

      logout: function() {
        angular.copy({}, Service.profile);
        isValid = false;
      },

      isLoggedIn: function(profile) {
        return isValid;
        // return (profile && profile.token && profile.expiration && profile.currentSite );
      }
  };
 
    return Service;

  }]);
