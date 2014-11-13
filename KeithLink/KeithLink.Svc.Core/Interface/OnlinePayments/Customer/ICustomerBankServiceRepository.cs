using KeithLink.Svc.Core.Models.OnlinePayments.Customer;
using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.OnlinePayments.Customer {
    public interface ICustomerBankServiceRepository {
        List<CustomerBank> GetAllCustomerBanks(string branchId, string customerNumber);

        CustomerBank GetBankAccount(string branchId, string customerNumber, string accountNumber);
    }
}
