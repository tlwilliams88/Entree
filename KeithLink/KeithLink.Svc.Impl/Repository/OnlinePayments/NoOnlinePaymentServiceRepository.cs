using KeithLink.Svc.Core.Interface.OnlinePayments;
using KeithLink.Svc.Core.Models.OnlinePayments.Customer;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.OnlinePayments {
    public class NoOnlinePaymentServiceRepository : IOnlinePaymentServiceRepository {
        #region methods
        public List<CustomerBank> GetAllCustomerBanks(UserSelectedContext userContext) {
            throw new NotImplementedException();
        }

        public CustomerBank GetBankAccount(UserSelectedContext userContext, string accountNumber) {
            throw new NotImplementedException();
        }
        #endregion
    }
}
