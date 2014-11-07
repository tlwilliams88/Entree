'use strict';
 
/**
 * @ngdoc function
 * @name bekApp.service:NotificationService
 * @description
 * # NotificationService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('NotificationService', [
    function () {
    
    var Service = {
      
      getUnreadMessageCount: function() {

      },

      getAllMessages: function() {
        return [{
          id: '12345',
          userid: '12345',
          customernumber: '12345',
          notificationtype: 'Order',
          messagereadutc: '2014-03-15',
          messagecreatedutc: '2014-12-15',
          ordernumber: '12345',
          subject: 'Order # 23453 was shipped.',
          body: 'Additional info',
          mandatory: false
        }, {
          id: '12345',
          userid: '12345',
          customernumber: '12345',
          notificationtype: 'Invoice',
          messagereadutc: '2014-03-15',
          messagecreatedutc: '2014-01-15',
          ordernumber: '12345',
          subject: 'Invoice # 12343 is late.',
          body: 'Additional info',
          mandatory: true
        }, {
          id: '12345',
          userid: '12345',
          customernumber: '12345',
          notificationtype: 'Order',
          messagereadutc: null,
          messagecreatedutc: '2014-03-15',
          ordernumber: '12345',
          subject: 'Order # 23453 was shipped.',
          body: 'Additional info',
          mandatory: false
        }];
      },

      updateMessages: function() {

      }
    };
 
    return Service;
 
  }]);