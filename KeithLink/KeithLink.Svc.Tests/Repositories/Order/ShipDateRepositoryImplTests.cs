using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.Orders;

namespace KeithLink.Svc.Test.Repositories.Order {
    [TestClass]
    public class ShipDateRepositoryImplTests {
        #region attributes
        ShipDateRepositoryImpl _shipDayService;
        CatalogInfo _customerInfo;
        #endregion

        #region ctor
        public ShipDateRepositoryImplTests() {
            _shipDayService = new ShipDateRepositoryImpl();
            _customerInfo = new CatalogInfo();
        }
        #endregion

        #region methods
        [TestMethod]
        public void GetGoodData() {
            _customerInfo.BranchId = "FDF";
            _customerInfo.CustomerId = "709333";

            ShipDateReturn retVal = _shipDayService.GetShipDates(_customerInfo);

            Assert.IsTrue(retVal.ShipDates.Count > 0);
        }

        [TestMethod]
        public void GetBadDataInvalidCustomer() {
            _customerInfo.BranchId = "fdf";
            _customerInfo.CustomerId = "909090";
            
            ShipDateReturn retVal = _shipDayService.GetShipDates(_customerInfo);

            Assert.IsTrue(retVal.ShipDates.Count == 0);
        }

        [TestMethod]
        public void GetBadDataNullCustomer() {
            _customerInfo.BranchId = "FDF";
            _customerInfo.CustomerId = null;
            
            ShipDateReturn retVal = _shipDayService.GetShipDates(_customerInfo);

            Assert.IsTrue(retVal.ShipDates.Count == 0);
        }

        [TestMethod]
        public void GetBadDataNullBranch() {
            _customerInfo.BranchId = null;
            _customerInfo.CustomerId = "70933";

            ShipDateReturn retVal = _shipDayService.GetShipDates(_customerInfo);

            Assert.IsTrue(retVal.ShipDates.Count == 0);
        }

        [TestMethod]
        public void GetBadDataInvalidBranch() {
            _customerInfo.BranchId = "XXX";
            _customerInfo.CustomerId = "709333";
            
            ShipDateReturn retVal = _shipDayService.GetShipDates(_customerInfo);

            Assert.IsTrue(retVal.ShipDates.Count == 0);
        }

        [TestMethod]
        public void GetBadDataNullData() {
            _customerInfo.BranchId = null;
            _customerInfo.CustomerId = null;

            ShipDateReturn retVal = _shipDayService.GetShipDates(_customerInfo);

            Assert.IsTrue(retVal.ShipDates.Count == 0);
        }
        #endregion
    }
}
