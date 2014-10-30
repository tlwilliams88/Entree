'use strict';

angular.module('bekApp')
  .controller('AddUserDetailsController', ['$scope', 'UserProfileService', 'LocalStorage',
    function ($scope, UserProfileService, LocalStorage) {

      /*Create vs Find User*/
      $scope.isExistingUser = false;

      /*Roles*/
      $scope.userRoles = ["Owner", "Accounting", "Approver", "Buyer", "Guest"];

      /*test customer data*/
      $scope.customers = [
        {
          "customerNumber": 410300,
          "customerName": "Amarillo Club",
          "customerBranch": "fam",
          "nationalOrRegionalAccountNumber": "",
          "nationalId": null,
          "dsrNumber": 20,
          "contractId": "D07",
          "isPoRequired": false,
          "isPowerMenu": true,
          "customerId": "76bb74d7-4bbe-4e4d-9777-267e8981fb76",
          "accountId": null
        },
        {
          "customerNumber": 418700,
          "customerName": "Pueblo Club",
          "customerBranch": "fam",
          "nationalOrRegionalAccountNumber": "",
          "nationalId": null,
          "dsrNumber": 20,
          "contractId": "D07",
          "isPoRequired": false,
          "isPowerMenu": true,
          "customerId": "76bb74d7-4bbe-4e8d-9777-267e8981fb76",
          "accountId": null
        },
        {
          "customerNumber": 416500,
          "customerName": "Tipico Club",
          "customerBranch": "fam",
          "nationalOrRegionalAccountNumber": "",
          "nationalId": null,
          "dsrNumber": 20,
          "contractId": "D07",
          "isPoRequired": false,
          "isPowerMenu": true,
          "customerId": "76bb74d7-4bbe-4e4d-8777-267e8981fb76",
          "accountId": null
        },
        {
          "customerNumber": 734898,
          "customerName": "Big Al's Bbq",
          "customerBranch": "fdf",
          "nationalOrRegionalAccountNumber": "N6000",
          "nationalId": null,
          "dsrNumber": 551,
          "contractId": "",
          "isPoRequired": false,
          "isPowerMenu": false,
          "customerId": "1f7639e8-de44-4a44-a486-5126261a540e",
          "accountId": null
        },
        {
          "customerNumber": 701441,
          "customerName": "Sonic #1596/Amarillo",
          "customerBranch": "fam",
          "nationalOrRegionalAccountNumber": "N6000",
          "nationalId": null,
          "dsrNumber": 42,
          "contractId": "G7800AM",
          "isPoRequired": false,
          "isPowerMenu": false,
          "customerId": "23a0a223-2e4f-4801-82b0-c16898c9d983",
          "accountId": null
        },
        {
          "customerNumber": 767998,
          "customerName": "Smashburger",
          "customerBranch": "fdf",
          "nationalOrRegionalAccountNumber": "N2000",
          "nationalId": null,
          "dsrNumber": 551,
          "contractId": "",
          "isPoRequired": false,
          "isPowerMenu": false,
          "customerId": "1f7639e8-de44-4a44-a486-5667261a540e",
          "accountId": null
        },
        {
          "customerNumber": 134898,
          "customerName": "Chipotle",
          "customerBranch": "fdf",
          "nationalOrRegionalAccountNumber": "R5000",
          "nationalId": null,
          "dsrNumber": 551,
          "contractId": "",
          "isPoRequired": false,
          "isPowerMenu": false,
          "customerId": "1f7639e8-de44-4a44-a486-5606451a540e",
          "accountId": null
        }
      ];
      //$scope.customers = LocalStorage.getProfile().user_customers;

      /*region groups - groups customers by their Regional Account number, assigns all customers with no group number to the
      * unassigned category*/

      $scope.regionGroups = [];
      $scope.unassignedGroup  = [];
      var groupExists = false;

      //for each customer on the profile
      $scope.customers.forEach(function(customer){
        groupExists = false;

        //take customers group number
        var currentCustomersGroupNumber = customer.nationalOrRegionalAccountNumber;
        customer.selected = false;

        //if the group number exists
        if(currentCustomersGroupNumber) {
          //check if there is already a group object with the same group number
          $scope.regionGroups.forEach(function(index){
              //if there is a group with the same number
              if (index.groupNumber == currentCustomersGroupNumber) {
                //add the customer to that groups customer list
                index.customers.push(customer);
                //prevent a new group from being created
                groupExists = true;
              }
          });
          //if there isn't a group with the same number
          if(groupExists == false) {
            //make a new group object that contains the current customer and add it to the collection
            $scope.regionGroups.push({"groupNumber" : currentCustomersGroupNumber, "show" : false , "customers" : [customer]});
          }
        }
        //otherwise assign it to the unassigned customer group
        else {
            $scope.unassignedGroup.push(customer);
        }
      });

      $scope.testFunction = function(text){
        console.log(text);
      };


      //selected customers

      $scope.selectedCustomers = [];

      $scope.toggleSelectCustomer = function(customer) {
        var idx = $scope.selectedCustomers.indexOf(customer);
        // is currently selected
        if (idx > -1) {
          $scope.selectedCustomers.splice(idx, 1);
        }

        // is newly selected
        else {
          $scope.selectedCustomers.push(customer);
        }
      };

      $scope.toggleSelectCustomerGroup = function(customerGroup){
        customerGroup.forEach(function(customer){
          var idx = $scope.selectedCustomers.indexOf(customer);
          // is currently selected
          if (idx > -1) {
            $scope.selectedCustomers.splice(idx, 1);
          }

          // is newly selected
          else {
            $scope.selectedCustomers.push(customer);
          }
        })
      };
  }]);
