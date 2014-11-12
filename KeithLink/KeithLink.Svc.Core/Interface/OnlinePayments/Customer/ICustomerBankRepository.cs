using KeithLink.Svc.Core.Models.OnlinePayments.Customer;
using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.OnlinePayments.Customer {
    public interface ICustomerBankRepository {
        IEnumerable<CustomerBank> ReadForCustomer(string branchId, string customerNumber);
    }
}
