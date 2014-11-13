using KeithLink.Svc.Core.Interface.OnlinePayments;
using KeithLink.Svc.Core.Interface.OnlinePayments.Customer;
using KeithLink.Svc.Core.Models.OnlinePayments.Customer.EF;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
        public List<CustomerBank> GetAllCustomerBanks(string division, string customerNumber) {
            SqlParameter branchParm = new SqlParameter("@Division", division);
            SqlParameter custParm = new SqlParameter("@CustNum", customerNumber);

            return _dbContext.Context.Database.SqlQuery<CustomerBank>(
                        "procGetCustomerBankAccounts @Division, @CustNum", 
                        branchParm, 
                        custParm
                    ).ToList();
        }

        public CustomerBank GetBankAccount(string division, string customerNumber, string accountNumber) {
            SqlParameter branchParm = new SqlParameter("@Division", division);
            SqlParameter custParm = new SqlParameter("@CustNum", customerNumber);
            SqlParameter acctParm = new SqlParameter("@AcctNum", accountNumber);

            return _dbContext.Context.Database.SqlQuery<CustomerBank>(
                    "procGetBankAccount @Division, @CustNum, @AcctNum",
                    branchParm, 
                    custParm,
                    acctParm
                ).FirstOrDefault();
        }
        #endregion
    }
}
