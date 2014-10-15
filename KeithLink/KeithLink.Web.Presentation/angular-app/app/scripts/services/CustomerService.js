'use strict';
 
/**
 * @ngdoc function
 * @name bekApp.service:CustomerService
 * @description
 * # CustomerService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('CustomerService', [ '$q', '$http', 'Customer', function ($q, $http, Customer) {
    
    var categories;
 
    var Service = {
      getCustomers: function() {
        // return Customer.query().$promise.then(function(response) {
        //   console.log(response.data);
        // });
        
        // var deferred = $q.defer();
        // $http.get('/profile/customers').then(function(response) {
        //   var data = response.data;
        //   console.log(data);
        //   if (data.successResponse) {
        //     deferred.resolve(data.successResponse.customers);
        //   } else {
        //     deferred.reject(data.errorMessage);
        //   }
        // });
        // return deferred.promise;
        return [{"customerNumber":"731881","customerName":"La Hacienda Treatment Ctr","customerBranch":"FSA","nationalId":"","dsrNumber":"163","contractId":"G89","isPoRequired":false,"isPowerMenu":false,"customerId":"{0016e189-81d7-4f91-8a2b-55a5e803b0da}","accountId":""},{"customerNumber":"106440","customerName":"The Chalkboard","customerBranch":"FOK","nationalId":"","dsrNumber":"041","contractId":"D106440","isPoRequired":false,"isPowerMenu":false,"customerId":"{0017e644-59e2-4d45-9be0-16c7d282092a}","accountId":""},{"customerNumber":"102721","customerName":"Doc's Wine & Bar","customerBranch":"FOK","nationalId":"","dsrNumber":"041","contractId":"","isPoRequired":false,"isPowerMenu":false,"customerId":"{0025f8c8-d3e1-490b-b284-bc5e12df4799}","accountId":""},{"customerNumber":"422554","customerName":"Pampa School Adm Warehse","customerBranch":"FAM","nationalId":"","dsrNumber":"005","contractId":"D66","isPoRequired":false,"isPowerMenu":false,"customerId":"{002c02e6-8b13-4fa3-9edc-4ac1141d911e}","accountId":""},{"customerNumber":"728142","customerName":"Pulaski Tech - Cahmi","customerBranch":"FLR","nationalId":"","dsrNumber":"031","contractId":"DLR34","isPoRequired":false,"isPowerMenu":false,"customerId":"{813e5eac-3359-49eb-9180-5e650b416845}","accountId":""},{"customerNumber":"100689","customerName":"Jimmy's Egg #3-Melton","customerBranch":"FOK","nationalId":"","dsrNumber":"042","contractId":"D099909","isPoRequired":false,"isPowerMenu":false,"customerId":"{002f9b37-0b93-4e89-b099-58cd5b99b8e8}","accountId":""},{"customerNumber":"701450","customerName":"Sonic #3803/Memphis","customerBranch":"FAM","nationalId":"","dsrNumber":"042","contractId":"G7800AM","isPoRequired":false,"isPowerMenu":false,"customerId":"{00320dae-5f07-47b4-a5c5-00e25e092115}","accountId":""},{"customerNumber":"734444","customerName":"188 South","customerBranch":"FSA","nationalId":"","dsrNumber":"040","contractId":"D735036","isPoRequired":false,"isPowerMenu":false,"customerId":"{814282eb-a8aa-43d4-bfdb-674610db9dae}","accountId":""},{"customerNumber":"733336","customerName":"Tx Rdhouse #411-Baytown","customerBranch":"FHS","nationalId":"","dsrNumber":"591","contractId":"","isPoRequired":false,"isPowerMenu":false,"customerId":"{0032f0e7-7b1d-4034-bd8b-a9f92caa94e3}","accountId":""},{"customerNumber":"718197","customerName":"Cypress Community Christi","customerBranch":"FHS","nationalId":"","dsrNumber":"333","contractId":"","isPoRequired":false,"isPowerMenu":false,"customerId":"{00385cc6-327f-415c-9243-f99734f69d93}","accountId":""},{"customerNumber":"100100","customerName":"--Sonic #4542-L.R.","customerBranch":"FAR","nationalId":"","dsrNumber":"013","contractId":"G7800ARQ","isPoRequired":false,"isPowerMenu":false,"customerId":"{815345ee-7dd5-45f2-b192-84294e515f98}","accountId":""},{"customerNumber":"026010","customerName":"Lone Star Park 3Rd Fine D","customerBranch":"FDF","nationalId":"","dsrNumber":"202","contractId":"D024502","isPoRequired":false,"isPowerMenu":false,"customerId":"{004e3a13-c09f-4444-81a0-c978beb39c98}","accountId":""},{"customerNumber":"106293","customerName":"Billy Sims Bbq-Claremore","customerBranch":"FOK","nationalId":"","dsrNumber":"056","contractId":"D095349","isPoRequired":false,"isPowerMenu":false,"customerId":"{0052c0b0-e3b1-4fa6-85a4-df21fc14b3fc}","accountId":""},{"customerNumber":"318417","customerName":"American Girl Grill & Cat","customerBranch":"FSA","nationalId":"","dsrNumber":"039","contractId":"","isPoRequired":false,"isPowerMenu":false,"customerId":"{815a3596-17eb-4ddb-a150-c1c13e6c4936}","accountId":""},{"customerNumber":"307631","customerName":"Baxters On Main","customerBranch":"FSA","nationalId":"","dsrNumber":"032","contractId":"","isPoRequired":false,"isPowerMenu":false,"customerId":"{00668a02-91d8-406d-a316-9d42a557155f}","accountId":""},{"customerNumber":"730677","customerName":"Driller's Restaurant & Ba","customerBranch":"FSA","nationalId":"","dsrNumber":"055","contractId":"D730677","isPoRequired":false,"isPowerMenu":false,"customerId":"{815a5ef9-62fe-4f67-9b66-5236497d894f}","accountId":""},{"customerNumber":"311677","customerName":"Schobels Restaurant - Col","customerBranch":"FSA","nationalId":"","dsrNumber":"059","contractId":"D083459","isPoRequired":false,"isPowerMenu":false,"customerId":"{0069af64-5fb0-4c92-b70c-63ac0fa94d01}","accountId":""},{"customerNumber":"014441","customerName":"City Bites #16/Edmond","customerBranch":"FOK","nationalId":"","dsrNumber":"077","contractId":"D014439","isPoRequired":false,"isPowerMenu":false,"customerId":"{815ea6be-aa67-40e2-9f2d-b6e40182ad72}","accountId":""},{"customerNumber":"723042","customerName":"Los Lupes #6","customerBranch":"FDF","nationalId":"","dsrNumber":"482","contractId":"D723047","isPoRequired":false,"isPowerMenu":false,"customerId":"{006f72fb-dac0-4a86-b931-f7fd0d4b28f4}","accountId":""},{"customerNumber":"353110","customerName":"Usmd Surgical Hosp Of Arl","customerBranch":"FDF","nationalId":"","dsrNumber":"227","contractId":"","isPoRequired":false,"isPowerMenu":false,"customerId":"{8169f57c-1ed1-4190-91b3-90b761a14cc2}","accountId":""},{"customerNumber":"723537","customerName":"Holiday Inn Express","customerBranch":"FDF","nationalId":"","dsrNumber":"322","contractId":"","isPoRequired":false,"isPowerMenu":false,"customerId":"{008d4a1a-6585-47a0-b662-593d4d9f66b1}","accountId":""}];
      }
  };
 
    return Service;
 
  }]);