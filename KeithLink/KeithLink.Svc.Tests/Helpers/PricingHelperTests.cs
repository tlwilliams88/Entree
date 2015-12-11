using KeithLink.Common.Core.Extensions;

using KeithLink.Svc.Core.Helpers;
using KeithLink.Svc.Core.Models.ShoppingCart;

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeithLink.Svc.Test.Helpers {
    [TestClass]
    public class PricingHelperTests {
        [TestMethod]
        public void TestGoodCasePrice() {
            ShoppingCartItem item = new ShoppingCartItem();
            item.Quantity = 2;
            item.CasePriceNumeric = 24.00;
            item.PackagePriceNumeric = 9;
            item.Each = false;
            item.CatchWeight = false;
            item.AverageWeight = 0;
            item.Pack = "3";

            int qty = (int)item.Quantity;
            
            decimal cartTotal = (decimal)PricingHelper.GetPrice(qty, item.CasePriceNumeric, item.PackagePriceNumeric, item.Each, item.CatchWeight, item.AverageWeight, item.Pack.ToInt(1));

            Assert.IsTrue(  cartTotal>= 0);
        }

        [TestMethod]
        public void TestGoodPackagePrice() {
            ShoppingCartItem item = new ShoppingCartItem();
            item.Quantity = 2;
            item.CasePriceNumeric = 24.00;
            item.PackagePriceNumeric = 9;
            item.Each = true;
            item.CatchWeight = false;
            item.AverageWeight = 0;
            item.Pack = "3";

            int qty = (int)item.Quantity;

            decimal cartTotal = (decimal)PricingHelper.GetPrice(qty, item.CasePriceNumeric, item.PackagePriceNumeric, item.Each, item.CatchWeight, item.AverageWeight, item.Pack.ToInt(1));

            Assert.IsTrue(cartTotal >= 0);
        }

        [TestMethod]
        public void TestGoodCatchweightCasePrice() {
            ShoppingCartItem item = new ShoppingCartItem();
            item.Quantity = 2;
            item.CasePriceNumeric = 2.46;
            item.PackagePriceNumeric = 2.46;
            item.Each = false;
            item.CatchWeight = true;
            item.AverageWeight = 83.11;
            item.Pack = "4";

            int qty = (int)item.Quantity;
            decimal cartTotal = (decimal)PricingHelper.GetPrice(qty, item.CasePriceNumeric, item.PackagePriceNumeric, item.Each, item.CatchWeight, item.AverageWeight, item.Pack.ToInt(1));

            Assert.IsTrue(cartTotal >= 0);
        }

        [TestMethod]
        public void TestGoodCatchweightPackagePrice() {
            ShoppingCartItem item = new ShoppingCartItem();
            item.Quantity = 2;
            item.CasePriceNumeric = 2.46;
            item.PackagePriceNumeric = 2.46;
            item.Each = true;
            item.CatchWeight = true;
            item.AverageWeight = 83.11;
            item.Pack = "4";

            int qty = (int)item.Quantity;
            decimal cartTotal = (decimal)PricingHelper.GetPrice(qty, item.CasePriceNumeric, item.PackagePriceNumeric, item.Each, item.CatchWeight, item.AverageWeight, item.Pack.ToInt(1));

            Assert.IsTrue(cartTotal >= 0);
        }

        [TestMethod]
        public void TestCatchweightPackagePriceWithNullPack() {
            ShoppingCartItem item = new ShoppingCartItem();
            item.Quantity = 2;
            item.CasePriceNumeric = 2.46;
            item.PackagePriceNumeric = 2.46;
            item.Each = true;
            item.CatchWeight = true;
            item.AverageWeight = 83.11;
            item.Pack = null;

            int qty = (int)item.Quantity;
            decimal cartTotal = (decimal)PricingHelper.GetPrice(qty, item.CasePriceNumeric, item.PackagePriceNumeric, item.Each, item.CatchWeight, item.AverageWeight, item.Pack.ToInt(1));

            Assert.IsTrue(cartTotal >= 0);
        }
    }
}
