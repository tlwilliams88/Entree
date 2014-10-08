using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Impl.Repository.Orders;

namespace KeithLink.Svc.Test.Repositories.Order {
    [TestClass]
    public class ShipDateRepositoryImplTests {
        #region attributes
        ShipDateRepositoryImpl _shipDayService;
        #endregion

        #region ctor
        public ShipDateRepositoryImplTests() {
            _shipDayService = new ShipDateRepositoryImpl();
        }
        #endregion

        #region methods
        [TestMethod]
        public void GetGoodData() {
            ShipDateReturn retVal = _shipDayService.GetShipDates("FDF", "709333");

            Assert.IsTrue(retVal.ShipDays.Count > 0);
        }

        [TestMethod]
        public void GetBadDataInvalidCustomer() {
            ShipDateReturn retVal = _shipDayService.GetShipDates("FDF", "909090");

            Assert.IsTrue(retVal.ShipDays.Count == 0);
        }

        [TestMethod]
        public void GetBadDataNullCustomer() {
            ShipDateReturn retVal = _shipDayService.GetShipDates("FDF", null);

            Assert.IsTrue(retVal.ShipDays.Count == 0);
        }

        [TestMethod]
        public void GetBadDataNullBranch() {
            ShipDateReturn retVal = _shipDayService.GetShipDates(null, "709333");

            Assert.IsTrue(retVal.ShipDays.Count == 0);
        }

        [TestMethod]
        public void GetBadDataInvalidBranch() {
            ShipDateReturn retVal = _shipDayService.GetShipDates("XXX", "709333");

            Assert.IsTrue(retVal.ShipDays.Count == 0);
        }

        [TestMethod]
        public void GetBadDataNullData() {
            ShipDateReturn retVal = _shipDayService.GetShipDates(null, null);

            Assert.IsTrue(retVal.ShipDays.Count == 0);
        }
        #endregion
    }
}
