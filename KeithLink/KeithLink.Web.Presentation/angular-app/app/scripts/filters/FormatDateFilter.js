'use strict';

angular.module('bekApp')
  .filter('formatDate', [function() {
    function getFormattedDateTime(dateTime, formatString) {

      // check UTC time and convert to format 2015-02-13 00:00:00 -- all times coming from the backend are Central
      if (dateTime && dateTime.indexOf && dateTime.indexOf('T') > -1 && dateTime.indexOf('Z') > -1) {
        dateTime = dateTime.replace('T', ' ');
        dateTime = dateTime.replace('Z', '');
      }

      // all times from the backend are Central time
      var date = moment.tz(dateTime, 'America/Chicago');

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
      if (!formatString) {
        formatString = 'ddd, MMM D, YYYY';
      }

      return getFormattedDateTime(dateTime, formatString);
    };
  }])
  .filter('formatDateWithTimezone', ['$filter', function($filter) {
    return function(dateTime, formatString) {

      if (!formatString) {
        formatString = 'ddd, MMM D, YYYY h:mma z';
      }

      return $filter('formatDate')(dateTime, formatString);
    };
  }]);
