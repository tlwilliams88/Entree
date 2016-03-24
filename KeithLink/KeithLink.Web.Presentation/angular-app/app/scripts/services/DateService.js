'use strict';
 
/**
 * @ngdoc function
 * @name bekApp.service:DateService
 * @description
 * # DateService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('DateService', [function () {

    var Service = {
      /**
       * format a JavaScript date object, used for Item Usage and Registered Users exports
       * @param  {Date} date
       * @return {String}      format of YYYY-MM-DD
       */
      formatJavascriptDate: function(date) {
        var day = date.getDate().toString(),
          month = (date.getMonth() + 1).toString(),
          year = date.getFullYear();

        if (day.length < 2) {
          day = '0' + day;
        }
        if (month.length < 2) {
          month = '0' + month;
        }

        return year + '-' + month + '-' + day;
      },

      momentObject: function(date, formatString, TZ){
       //Generates moment object with correct formatting string
        if(!date){
          date = '';
        }

        var timezone = (date.length > 10);

        if(!formatString && date.indexOf('/') !== -1){
           formatString = (timezone) ? 'MM/DD/YYYY HH:mm:ss':'MM/DD/YYYY';
        }
        else if(!formatString && date.indexOf('-') !== -1){
           formatString = (timezone) ? 'YYYY-MM-DD HH:mm:ss.SSSS':'YYYY-MM-DD';
        }

       if(TZ){
        return moment.tz(date,formatString,TZ)
       }
        else if(date){
          return moment(date,formatString);
        }
        else{
          return moment(new Date());
        }
      }
      };

    return Service;
 
  }]);
