using KeithLink.Svc.Core.Models.OnlinePayments.Customer;
using KeithLink.Svc.Core.Models.SiteCatalog;
using EF = KeithLink.Svc.Core.Models.OnlinePayments.Customer.EF;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace KeithLink.Svc.InternalSvc.Interfaces {
    [ServiceContract]
    public interface IOnlinePaymentService {
        [OperationContract]
        List<CustomerBank> GetAllBankAccounts(UserSelectedContext userContext);

        [OperationContract]
        CustomerBank GetBankAccount(UserSelectedContext userContext, string accountNumber);
    }
}
