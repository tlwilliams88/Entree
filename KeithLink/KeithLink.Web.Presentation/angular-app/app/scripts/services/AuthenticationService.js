'use strict';

angular.module('bekApp')
  .factory('AuthenticationService', ['$http', '$q', 'UserProfileService', function ($http, $q, UserProfileService) {
    
    var isValid = false;

    function validateUser(username, password) {
      var data = {
        username: username,
        password: password,
        grant_type: 'password'
      };

      var headers = { headers : {
          'Content-Type': 'application/x-www-form-urlencoded' 
        }
      };

      return $http.post('/authen' , data, headers).then(function(response){
        
        return response.data;
      });
    }

    function getProfile(email, password) {
      var params = {
        email: email,
        password: password
      };

      return $http.post('/profile/login', params).then(function (response) {
        var profile = response.data.userProfiles[0];

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

        return profile;
      });
    }

    var Service = {

      login: function(username, password) {
        $q.all([ 
          //validateUser(username, password),
          getProfile(username, password)
        ]).then(function(data) {
          var profile = data[0];
          debugger;

          profile.token = {
            'access_token': '3GT4Y5XUpED5W8Ri6hptEJKtE2P3EBDU03jSMPXiar3C0YIzK5W9PhPC36nQMgm2qTaYTLvvCu_VVq1nsyaxjEDvyViSHrue0Q-mjOg46cbnRrhNqb9FdVW2b57fRL0_69C782HfFAmsHnFv4-FAr2CUw0mqr-W48gWtq_qZNz-f4T5SCXkWhoLqrqeDIbYPJl_cxfH47nwLnYEcxrmpH7wzuKoY_zm49A1Yp2R7gXfhVv7Ci-YDaySvD596cfLq1ZqKb4KGl_o9gm3VYFyoNQ',
            'token_type': 'bearer',
            'expires_in': 86399
          };

          UserProfileService.setProfile(profile);
        });        
      },

      isLoggedIn: function() {
        return false;
      }
    };
 
    return Service;

  }]);
