'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:BankAccountService
 * @description
 * # BankAccountService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('BankAccountService', [ 'BankAccount', function (BankAccount) {

  var Service = {

    // gets all bank accounts for a given customer using the userSelectedContext header
    getAllBankAccounts: function() {
      return BankAccount.query({}).$promise;
    },

    // gets one bank account
    getBankAccountByAccountNumber: function(bankAccountNumber) {
      return BankAccount.get({
        bankAccountNumber: bankAccountNumber
      }).$promise;
    }

  };

    return Service;

  }]);
