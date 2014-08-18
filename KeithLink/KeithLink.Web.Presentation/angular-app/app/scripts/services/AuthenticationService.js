'use strict';

angular.module('bekApp')
  .factory('AuthenticationService', ['$http', '$q', 'UserProfileService', function ($http, $q, UserProfileService) {
    
    var isValid = false;

    function validateUser(username, password) {
      // var data = {
      //   username: username,
      //   password: password,
      //   grant_type: 'password'
      // };

      var data = "grant_type=password&username=" + username + "&password=" + password;

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
          validateUser(username, password),
          getProfile(username, password)
        ]).then(function(data) {
          var token = data[0],
            profile = data[1];
          
          profile.token = token;

          UserProfileService.setProfile(profile);
        });        
      },

      isLoggedIn: function() {
        return false;
      }
    };
 
    return Service;

  }]);
