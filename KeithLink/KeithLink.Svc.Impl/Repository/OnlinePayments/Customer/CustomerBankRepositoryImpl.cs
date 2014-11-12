using KeithLink.Svc.Core.Interface.OnlinePayments;
using KeithLink.Svc.Core.Interface.OnlinePayments.Customer;
using KeithLink.Svc.Core.Models.OnlinePayments.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.OnlinePayments.Customer {
    public class CustomerBankRepositoryImpl : ICustomerBankRepository {
        #region attributes
        private readonly IKPayDBContext _dbContext;
        #endregion

        #region ctor
        public CustomerBankRepositoryImpl(IKPayDBContext kpayDbContext) {
            _dbContext = kpayDbContext;
        }
        #endregion

        #region methods
        public IEnumerable<CustomerBank> ReadForCustomer(string branchId, string customerNumber) {
            return _dbContext.CustomerBanks.Where<CustomerBank>(b => b.BranchId.Equals(branchId) && b.CustomerNumber.Equals(customerNumber));
        }
        #endregion
    }
}
