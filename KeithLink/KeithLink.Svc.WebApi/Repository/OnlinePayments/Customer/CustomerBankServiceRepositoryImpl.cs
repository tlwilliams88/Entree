using KeithLink.Svc.Core.Interface.OnlinePayments.Customer;
using KeithLink.Svc.Core.Models.OnlinePayments.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeithLink.Svc.WebApi.Repository.OnlinePayments.Customer {
    public class CustomerBankServiceRepositoryImpl : ICustomerBankServiceRepository {
        #region attributes
        private com.benekeith.CustomerBankService.ICustomerBankService _client;
        #endregion

        #region ctor
        public CustomerBankServiceRepositoryImpl(com.benekeith.CustomerBankService.ICustomerBankService serviceClient) {
            _client = serviceClient;
        }
        #endregion

        #region methods

        public List<CustomerBank> GetAllCustomerBanks(string branchId, string customerNumber) {
            return _client.GetAllCustomerBanks(branchId, customerNumber).ToList<CustomerBank>();
        }

        public Core.Models.OnlinePayments.Customer.CustomerBank GetBankAccount(string branchId, string customerNumber, string accountNumber) {
            return _client.GetBankAccount(branchId, customerNumber, accountNumber);
        }

        #endregion
    }
}