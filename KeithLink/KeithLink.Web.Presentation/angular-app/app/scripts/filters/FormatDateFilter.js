'use strict';

function getFormattedDateTime(dateTime, formatString, useTimezone) {
  var date = moment(dateTime);
  if (dateTime && date.isValid()) {
    if (useTimezone) {
      var timezoneName = jstz.determine().name();
      date.tz(timezoneName);
    }
    return date.format(formatString);
  } else {
    return dateTime;
  }
}

angular.module('bekApp')
  .filter('formatDate', [ function(){
    return function(dateTime, formatString){

    if (!formatString) {
      formatString = 'YYYY-MM-DD';
    }
    
    return getFormattedDateTime(dateTime, formatString);
  };
}])
  .filter('formatDateWithTimezone', [ function(){
    return function(dateTime, formatString){

    if (!formatString) {
      formatString = 'YYYY-MM-DD h:mma z';
    }
    
    return getFormattedDateTime(dateTime, formatString, true);
  };
}]);
