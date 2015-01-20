'use strict';

angular.module('bekApp')
.filter('LocalTimeZoneTime', [ '$filter', function($filter) {
  return function(datetimeobj) {
    var utcTime = moment(datetimeobj);
    var tzName = jstz.determine().name();
    var localTime = moment(utcTime).tz(tzName)
    var timezoneAbbrev = localTime.format('h:mma z'); //CST
    return timezoneAbbrev;
  };
}]);

angular.module('bekApp')
.filter('LocalTimeZoneTimeWithDate', [ '$filter', function($filter) {
  return function(datetimeobj) {
    var utcTime = moment(datetimeobj);
    var tzName = jstz.determine().name();
    var localTime = moment(utcTime).tz(tzName)
    var timezoneAbbrev = localTime.format('l h:mma z'); //CST
    return timezoneAbbrev;
  };
}]);