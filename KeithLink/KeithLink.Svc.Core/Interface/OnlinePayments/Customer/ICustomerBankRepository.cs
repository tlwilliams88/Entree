using KeithLink.Svc.Core.Models.OnlinePayments.Customer;
using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.OnlinePayments.Customer {
    public interface ICustomerBankRepository {
        List<CustomerBank> ReadForCustomer(string branchId, string customerNumber);
    }
}
