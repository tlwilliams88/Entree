'use strict';

/**
 * formatDate filter
 * Uses moment.js library to format datetimes and convert times to correct timezone
 */
angular.module('bekApp')
  .filter('formatDate', [function() {
 
    function getFormattedDateTime(dateTime, formatString) {
     // Don't do anything if it's null
     if (!dateTime) {
       return;
     }
    // check UTC time and convert to format 2015-02-13 00:00:00 -- all times coming from the backend are Central
     if (dateTime && dateTime.indexOf && dateTime.indexOf('T') > -1 && dateTime.indexOf('Z') > -1) {
        dateTime = dateTime.replace('T', ' ');
        dateTime = dateTime.replace('Z', '');
      }

        if(!dateTime._isAMomentObject){
          var date = moment(dateTime);
        }
        else{
          var date = dateTime;
        } 
      // convert datetime to local timezone
      date.tz(jstz.determine().name());

      // check if date is a valid moment object and format
      if (dateTime && date.isValid()) {
        return date.format(formatString);
      } else {
        return dateTime;
      }
    }

    return function(dateTime, formatString) {
      // use default format string if none is provided
      if (!formatString) {
        formatString = 'ddd, MMM D, YYYY';
      }

      return getFormattedDateTime(dateTime, formatString);
    };
  }])
  .filter('formatDateWithTimezone', ['$filter', function($filter) {
    return function(dateTime, formatString) {
     // check UTC time and convert to format 2015-02-13 00:00:00 -- all times coming from the backend are Central
     if (dateTime && dateTime.indexOf && dateTime.indexOf('T') > -1 && dateTime.indexOf('Z') > -1) {
        dateTime = dateTime.replace('T', ' ');
        dateTime = dateTime.replace('Z', '');
      }
      var date = moment.tz(dateTime, 'America/Chicago');
      date.tz(jstz.determine().name());
      // use default format string if none is provided
      if (!formatString) {
        formatString = 'ddd, MMM D, YYYY h:mma z';
      }

      return $filter('formatDate')(date, formatString);
    };
  }]);
