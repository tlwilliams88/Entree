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
    getAllBankAccounts: function(customer, branch) {
      return BankAccount.get({
        customerId: customer,
        branchId: branch
      }).$promise.then(function(resp){
        return resp.successResponse;
      });
    },

    // gets one bank account
    getBankAccountByAccountNumber: function(bankAccountNumber) {
      return BankAccount.get({
        bankAccountNumber: bankAccountNumber
      }).$promise.then(function(resp){
        return resp.successResponse;
      });
    }

  };

    return Service;

  }]);
