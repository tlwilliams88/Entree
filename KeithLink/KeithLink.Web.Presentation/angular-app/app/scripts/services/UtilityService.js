'use strict';
 
/**
 * @ngdoc function
 * @name bekApp.service:UtilityService
 * @description
 * # UtilityService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('UtilityService', [ '$q', 'DateService', 'Constants', function ($q, DateService, Constants) {

    function isUsedName(namesList, name, number) {
      return namesList.indexOf(name + ' ' + number) > -1;
    }

    var Service = {
      
      /**
       * generates a unique name for new lists and carts
       * @param  {String} nameText   string to be used when generating the name
       * @param  {Array} collection  array of objects that must have a "name" property. Used to determine what will be a unique name
       * @return {String}            generated name string, "New List 1" for example
       */
      generateName: function(nameText, collection) {
        var namesList = [];
        //Select the numeric portion of the name. Can be numeric indicator for lists or datetime of creation for carts
        var number = (nameText === 'New List') ? 0 : DateService.momentObject().format(Constants.dateFormat.monthDayYearTimeDashes);

        angular.forEach(collection, function(item) {
          namesList.push(item.name);
        });
        var isNameUsed = isUsedName(namesList, nameText, number);
        var duplicates = 0;
        var tempNumber = number;
        while (isNameUsed) {
          duplicates++;
          //Increment indicator for lists or increment indicator and append to date for carts
          tempNumber = (nameText === 'New List') ? duplicates : number + ' - ' + duplicates;
          isNameUsed = isUsedName(namesList, nameText, tempNumber);
        }
        return nameText + ' ' + tempNumber;
      },

      /**
       * determines if current device is mobile device by userAgent and screen size
       * @return {Boolean} if current device is a mobile device
       */
      isMobileDevice: function() {
        var check = false;
        
        // Check for mobile browsers using http://detectmobilebrowsers.com/ script
        (function(a,b){
          if(/(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino|android|ipad|playbook|silk/i.test(a)||/1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(a.substr(0,4))) {
            check = true;
          }
        })(navigator.userAgent||navigator.vendor||window.opera);

        // Check for screen width
        if (window.innerWidth < 992) {
          check = true;
        }
        return check;
      },

      /**
       * finds one object in collection array where the given fieldName matches the matcher (findListById for example)
       * @param  {Array}  collection  array of objects to search through (Array of Lists for example)
       * @param  {String} fieldName  property name to do a comparison on ('id' for example)
       * @param  {String/Number/Boolean} matcher    String/Number/Boolean that the given field must match to (12345 for example)
       * @return {Object}            Object found that matches or null if no match was found
       */
      findObjectByField: function(collection, fieldName, matcher) {
        var obj;
        angular.forEach(collection, function(item, index) {
          if (item[fieldName] === matcher) {
            obj = item;
          }
        });
        return obj;
      },

      // accepts collection of objects and the fieldName (array of strings) to be deleted
      
      /**
       * Deletes the given properties from the objects in the given array
       * @param  {Array} collection Array of Objects to remove the property from
       * @param  {Array} fieldNames Array of Strings of property names to remove
       * @return {null}
       */
      deleteFieldFromObjects: function(collection, fieldNames) {
        angular.forEach(collection, function(item, index) {
          angular.forEach(fieldNames, function(name, index) {
            delete item[name];
          });
        });
      },
     
      /**
       * general way to resolve most of our endpoints that return an object of the following format
       * where the request was sucessful if successResponse is not null
       * {
       *   successResponse: ({}, [], true), 
       *   errorMessage: "Error message"
       * }
       * @param  {Promise} promise Promise that, when resolved, contains successResponse and errorMessage properties
       * @return {Promise}         the promise to chain off of
       */
      resolvePromise: function(promise) {
        var deferred = $q.defer();

        promise.then(function(response) {
          var data = response.data;
          if (data.successResponse) {
            deferred.resolve(data.successResponse);
          } else {
            deferred.reject(data.errorMessage);
          }
        }, function(error) {
          deferred.reject('An error occurred.');
        });
        return deferred.promise;
      }
    };

    return Service;
 
  }]);
