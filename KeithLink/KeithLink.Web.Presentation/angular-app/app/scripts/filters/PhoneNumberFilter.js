'use strict';

/**
 * phoneNumber filter
 * Formats phone numbers
 */
angular.module('bekApp')
.filter('phoneNumber', function () {
  return function (tel) {
    if (!tel) { return ''; }

    var value = tel.toString().trim().replace(/\D/g, '');

    if (value.match(/[^0-9]/)) {
      return tel;
    }

    var country, city, number, ext;

    switch (value.length) {
      case 10: // +1PPP####### -> C (PPP) ###-####
        country = 1;
        city = value.slice(0, 3);
        number = value.slice(3);
        break;

      case 11: // +CPPP####### -> CCC (PP) ###-####
        country = value[0];
        city = value.slice(1, 4);
        number = value.slice(4);
        break;

      case 12: // +CCCPP####### -> CCC (PP) ###-####
        country = value.slice(0, 3);
        city = value.slice(3, 5);
        number = value.slice(5);
        break;
        
      case 14: // (###) ###-#### x####
        country = 1;
        city = value.slice(0, 3);
        number = value.slice(3, 10);
        ext = value.slice(10);
        break;
          
      default:
        return tel;
    }

    if (country === 1) {
      country = '';
    }

    number = number.slice(0, 3) + '-' + number.slice(3);
    ext = ext ? ' x' + ext : '';

    return (country + ' (' + city + ') ' + number + ext).trim();
  };
});
