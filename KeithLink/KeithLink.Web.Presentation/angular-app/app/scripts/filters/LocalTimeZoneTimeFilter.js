'use strict';

angular.module('bekApp')
.filter('LocalTimeZoneTime', [ '$filter', function($filter) {
  return function(datetimeobj) {
    var utcTime = moment(datetimeobj);

    var timezoneAbbrev = '';
    if (datetimeobj) {
      var tzName = jstz.determine().name();
      var localTime = moment(utcTime).tz(tzName);
      timezoneAbbrev = localTime.format('h:mma z'); //CST
    }
    return timezoneAbbrev;
  };
}]);

angular.module('bekApp')
.filter('LocalTimeZoneTimeWithDate', [ '$filter', function($filter) {  
  return function(datetimeobj) {
    var utcTime = moment(datetimeobj);
    
    var timezoneAbbrev = '';
    if (datetimeobj) {
      var tzName = jstz.determine().name();
      var localTime = moment(utcTime).tz(tzName);
      timezoneAbbrev = localTime.format('l h:mma z'); //CST
    }
    return timezoneAbbrev;
  };
}]);

angular.module('bekApp')
.filter('adjustDatepicker', ['$filter', function($filter){
    var dateFilter = $filter('date');    
    return function(dateToFix, formatType){
      if(!dateToFix || dateToFix === 'N/A'){
        return dateToFix;
      }
        var localDate, localTime, localOffset, adjustedDate;
        localDate       = new Date(dateToFix);
        localTime       = localDate.getTime();
        localOffset     = localDate.getTimezoneOffset() * 60000;
        adjustedDate    = new Date(localTime + localOffset);    
        return dateFilter(adjustedDate, formatType);   
    };
}]);
