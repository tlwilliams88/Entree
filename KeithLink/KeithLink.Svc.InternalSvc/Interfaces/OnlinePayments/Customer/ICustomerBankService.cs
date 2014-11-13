using KeithLink.Svc.Core.Models.OnlinePayments.Customer;
using EF = KeithLink.Svc.Core.Models.OnlinePayments.Customer.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Threading.Tasks;

namespace KeithLink.Svc.InternalSvc.Interfaces.OnlinePayments.Customer {
    [ServiceContract]
    public interface ICustomerBankService {
        [OperationContract]
        List<CustomerBank> GetAllCustomerBanks(string branchId, string customerNumber);

        [OperationContract]
        CustomerBank GetBankAccount(string branchId, string customerNumber, string accountNumber);
    }
}
