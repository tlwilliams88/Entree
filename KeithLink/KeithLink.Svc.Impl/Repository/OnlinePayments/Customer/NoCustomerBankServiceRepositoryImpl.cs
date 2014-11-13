using KeithLink.Svc.Core.Interface.OnlinePayments.Customer;
using KeithLink.Svc.Core.Models.OnlinePayments.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.OnlinePayments.Customer {
    public class NoCustomerBankServiceRepositoryImpl : ICustomerBankServiceRepository {
        #region methods
        public List<CustomerBank> GetAllCustomerBanks(string branchId, string customerNumber) {
            throw new NotImplementedException();
        }

        public CustomerBank GetBankAccount(string branchId, string customerNumber, string accountNumber) {
            throw new NotImplementedException();
        }
        #endregion
    }
}
