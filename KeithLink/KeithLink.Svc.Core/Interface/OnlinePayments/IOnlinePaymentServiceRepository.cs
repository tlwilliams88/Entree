using KeithLink.Svc.Core.Models.OnlinePayments.Customer;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.OnlinePayments {
    public interface IOnlinePaymentServiceRepository {
        List<CustomerBank> GetAllCustomerBanks(UserSelectedContext userContext);

        CustomerBank GetBankAccount(UserSelectedContext userContext, string accountNumber);
    }
}
