using KeithLink.Svc.Core.Interface.OnlinePayments;
using KeithLink.Svc.Core.Models.OnlinePayments.Payment;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.SiteCatalog;

using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace KeithLink.Svc.Test.Logic.InternalSrv {
    [TestClass]
    public class InternalOnlinePaymentLogicTests {
        #region attributes
        private readonly IOnlinePaymentsLogic _logic;		 
        #endregion

        #region ctor
        public InternalOnlinePaymentLogicTests() {
            IContainer diMap = DependencyMap.Build();

            // ***********************************************************************
            // could not get dependancy to resolve in a timely manner
            // ***********************************************************************
            //_logic = diMap.Resolve<IOnlinePaymentsLogic>();
        }
        #endregion

        #region methods
        [TestMethod]
        public void GetAllPaymentTransactionsForCustomer() {
            //UserSelectedContext customer = new UserSelectedContext() {BranchId = "fdf", CustomerId = "714090"};
            //PagingModel page = new PagingModel(){From = 0, Size = 30};

            //var results = _logic.PendingTransactions(customer, page);

            //Assert.IsTrue(results.Results.Count > 0);
        }
        #endregion
    }
}
