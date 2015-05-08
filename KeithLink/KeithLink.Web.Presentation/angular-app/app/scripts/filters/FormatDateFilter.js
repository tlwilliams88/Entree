'use strict';

angular.module('bekApp')
  .filter('formatDate', [function() {
    function getFormattedDateTime(dateTime, formatString, useTimezone) {

      // fix for dates that formatted like 2015-02-13T00:00:00Z
      // when parsed they are returned at 2015-02-12 but should be 2015-02-13
      if (dateTime && dateTime.indexOf && dateTime.indexOf('T00:00:00Z') > -1) {
        dateTime = dateTime.substr(0, 10);

        // check UTC time and convert to central
      } else if (dateTime && dateTime.indexOf && dateTime.indexOf('T') > -1 && dateTime.indexOf('Z') > -1) {
        dateTime = dateTime.replace('T', ' ');
        dateTime = dateTime.replace('Z', '');
      }

      // all times from the backend are Central time
      var date = moment.tz(dateTime, 'America/Chicago');

      if (useTimezone === true) {
        date.tz(jstz.determine().name());
      }

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

      return getFormattedDateTime(dateTime, formatString, true);
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
