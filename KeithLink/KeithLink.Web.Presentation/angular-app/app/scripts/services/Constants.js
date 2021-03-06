'use strict';

angular.module('bekApp')
  .constant('Constants', {

    localStorage : {
      userProfile: 'userProfile',
      userToken: 'userToken',
      branchId: 'branchId',
      customerNumber: 'customerNumber',
      leadGenInfo: 'leadGenInfo',
      currentCustomer: 'currentCustomer',
      tempContext: 'tempContext',
      tempBranch: 'tempBranch',
      lastList: 'lastList',
      lastOrderList: 'lastOrderList',
      pageSize: 'pageSize',
      defaultSort: 'defaultSort',
      defaultView: 'defaultView',
      userName: 'userName',
      hideTutorialHome: 'hideTutorialHome',
      hideTutorialSearch: 'hideTutorialSearch',
      hideTutorialAddToOrder: 'hideTutorialAddToOrder',
      searchTerms: 'searchTerms'
    },
    
    listType: {
      Custom: 0,  
      Favorite: 1,  
      Contract: 2,  
      Recent: 3,  
      Notes: 4,  
      Worksheet: 5,  
      ContractItemsAdded: 6,  
      ContractItemsDeleted: 7,  
      Reminders: 8,  
      Mandatory: 9,  
      Recommended: 10,  
      InventoryValuation: 11,  
      RecentlyOrdered: 12,  
      RecentlyViewed: 13,  
      CustomInventory: 14
    },

    catalogType: {
      BEK: 'BEK',
      UNFI: 'UNFI',
      ES: 'E&S'
    },

    emptyValue: {
      emptyGUID: '00000000-0000-0000-0000-000000000000'
    },
    
    exportType: {
      invoiceExport: 'invoiceExport',
      listExport: 'listExport',
      orderItemsExport: 'orderItemsExport',
      ordersExport: 'ordersExport',
      searchExport: 'searchExport',
      marketingExport: 'marketingExport',
      itemUsageExport: 'itemUsageExport',
      cartItemsExport: 'cartItemsExport',
      invoiceItemsExport: 'invoiceItemsExport'
    },

    offlineLocalStorage: {
      labels: 'labels',
      deletedListGuids: 'deletedListGuids',
      shipDates: 'shipDates',
      deletedCartGuids: 'deletedCartGuids'
    },

    dateFormat: {
     yearMonthDayFullTimestampDashes: 'YYYY-MM-DD HH:mm:ss.SSSS',
     yearMonthDayHourMinute : 'YYYYMMDDHHmm',
     monthDayYearSlashes: 'MM/DD/YYYY',
     monthDayYearTimeDashes: 'M-D-YY h:mma',
     monthDayYearHourMinuteSecondSlashes: 'MM/DD/YYYY HH:mm:ss',
     yearMonthDayHourMinuteSecondDashes: 'YYYY-MM-DDTHH:mm:ss',
     yearMonthDayDashes : 'YYYY-MM-DD',
     hourMinuteSecond : 'HHmmss',
     yearMonthDay : 'YYYYMMDD',
     month : 'MM',
     year: 'YYYY'
    },

    servicelocatorUrl: '../servicelocator', // DEPRECATED

    roles: {
      // external
      OWNER: 'owner',
      ACCOUNTING: 'accounting',
      APPROVER: 'approver',
      BUYER: 'buyer',
      GUEST: 'guest',

      // internal
      DSR: 'dsr',
      DSM: 'dsm',
      SYS_ADMIN: 'beksysadmin',
      POWER_USER: 'poweruser',
      BRANCH_MANAGER: 'branchismanager',
      KBIT_ADMIN: 'kbitadmin',
      MARKETING: 'marketing'
    },

    checkoutSteps: {
      CreateCart: 1,
      ViewCart: 2,
      SubmitCart: 3,
      CheckStatus: 4,
      StartChangeOrder: 5,
      SubmitChangeOrder: 6
    },

    jskeycodes: {
      backspace: 8,
      tab: 9,
      enter: 13,
      shift: 16,
      alt: 18,
      pause_break: 19,
      capslock: 20,
      escape: 27,
      pageup: 33,
      pagedown: 34,
      end: 35,
      home: 36,
      leftarrow: 37,
      uparrow: 38,
      rightarrow: 39,
      downarrow: 40,
      insert: 45,
      delete: 46,
      int0: 48,
      int1: 49,
      int2: 50,
      int3: 51,
      int4: 52,
      int5: 53,
      int6: 54,
      int7: 55,
      int8: 56,
      int9: 57,
      a: 65,
      b: 66,
      c: 67,
      d: 68,
      e: 69,
      f: 70,
      g: 71,
      h: 72,
      i: 73,
      j: 74,
      k: 75,
      l: 76,
      m: 77,
      n: 78,
      o: 79,
      p: 80,
      q: 81,
      r: 82,
      s: 83,
      t: 84,
      u: 85,
      v: 86,
      w: 87,
      x: 88,
      y: 89,
      z: 90,
      leftwindowkey: 91,
      rightwindowkey: 92,
      selectkey: 93,
      numpad0: 96,
      numpad1: 97,
      numpad2: 98,
      numpad3: 99,
      numpad4: 100,
      numpad5: 101,
      numpad6: 102,
      numpad7: 103,
      numpad8: 104,
      numpad9: 105,
      multiply: 106,
      add: 107,
      subtract : 109,
      decimalpoint: 110,
      divide: 111,
      f1: 112,
      f2: 113,
      f3: 114,
      f4: 115,
      f5: 116,
      f6: 117,
      f7: 118,
      f8: 119,
      f9: 120,
      f10: 121,
      f11: 122,
      f12: 123,
      numlock: 144,
      scrolllock: 145,
      semicolon: 186,
      equalsign: 187,
      comma: 188,
      dash: 189,
      period: 190,
      forwardslash: 191,
      graveaccent: 192,
      openbracket: 219,
      backslash: 220,
      closebracket: 221,
      singlequote: 222
    },

    Analytics: {
      HowManyProductsToThisChunk: 40
    },

    infiniteScrollPageSize: 50,
    promoItemsSize: 6,

    recommendedItemParameters: {
      Desktop: {
        getimages: true,
        ATO: {
          pagesize: null,
          getimages: null
        },
        Cart: {
          pagesize: 5
        },
        ItemDetails: {
          pagesize: 3
        }
      },
      Mobile: {
        getimages: false,
        pagesize: null
      }
    }
  });