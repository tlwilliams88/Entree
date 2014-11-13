using KeithLink.Svc.Core.Models.OnlinePayments.Customer.EF;
using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.OnlinePayments.Customer {
    public interface ICustomerBankRepository {
        List<CustomerBank> GetAllCustomerBanks(string division, string customerNumber);

        CustomerBank GetBankAccount(string division, string customerNumber, string accountNumber);
    }
}
