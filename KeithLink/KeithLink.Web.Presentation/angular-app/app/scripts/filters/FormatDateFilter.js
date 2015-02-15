'use strict';

function getFormattedDateTime(dateTime, formatString, useTimezone) {

  // fix for dates that formatted like 2015-02-13T00:00:00Z
  // when parsed they are returned at 2015-02-12 but should be 2015-02-13
  if (dateTime && dateTime.indexOf && dateTime.indexOf('T00:00:00Z') > -1) {
    dateTime = dateTime.substr(0, 10);
  }

  var date = moment(dateTime);
  if (dateTime && date.isValid()) {
    if (useTimezone) {
      var timezoneName = 'America/Chicago';
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
