using KeithLink.Svc.Core.Models.OnlinePayments.Customer.EF;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using KeithLink.Svc.Impl.Repository.OnlinePayments.Customer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Test.Repositories.OnlinePayments {
    [TestClass]
    public class CustomerBankRepositoryTests {
        #region attributes
        private KPayDBContext _dbContext;
        private CustomerBankRepositoryImpl _bankRepo;
        #endregion

        #region ctor
        public CustomerBankRepositoryTests() {
            //_dbContext = new KPayDBContext("KPAYDBContext");
            _dbContext = new KPayDBContext("KPAYDBContext");
            _bankRepo = new CustomerBankRepositoryImpl(_dbContext);
        }
        #endregion

        #region methods
        [TestMethod]
        public void SuccessfullyGetCustomerBanks() {
            List<CustomerBank> banks = _bankRepo.GetAllCustomerBanks("FAM04", "700353");

            Assert.IsTrue(banks.Count > 0);
        }
        #endregion
    }
}
