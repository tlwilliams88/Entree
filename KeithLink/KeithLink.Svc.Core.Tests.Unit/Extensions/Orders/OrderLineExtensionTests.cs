using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Extensions.Orders;
using KeithLink.Svc.Core.Models.Invoices;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Tests.Unit.Extensions.Orders {
    public class OrderLineExtensionTests {
        #region setup
        private static OrderLine MakeTestData() {
            return new OrderLine {
                ItemNumber = "123456",
                Price = 2,
                QantityShipped = 1,
                QuantityOrdered = 1,
                LineNumber = 17,
                IsValid = true,
                Name = "Fake Name",
                Detail = "Fake Detail",
                Description = "Fake Description",
                Pack = "Fake Pack",
                Size = "Fake Size",
                Each = true,
                Brand = "Fake Brand",
                BrandExtendedDescription = "Fake Brand",
                ReplacedItem = "Fake ReplacedItem",
                ReplacementItem = "Fake ReplacementItem",
                NonStock = "Fake NonStock",
                ChildNutrition = "Fake ChildNutrition",
                CatchWeight = true,
                TempZone = "Fake TempZone",
                ItemClass = "Fake ItemClass",
                CategoryCode = "Fake CategoryCode",
                SubCategoryCode = "Fake SubCategoryCode",
                CategoryName = "Fake CategoryName",
                UPC = "Fake UPC",
                VendorItemNumber = "Fake VendorItemNumber",
                Cases = "Fake Cases",
                Kosher = "Fake Kosher",
                ManufacturerName = "Fake ManufacturerName",
                ManufacturerNumber = "Fake ManufacturerNumber",
                Nutritional = new Nutritional {
                    BrandOwner = "Fake BrandOwner"
                },
                TotalShippedWeight = 42,
                CatalogId = "FUT"
            };
        }
        #endregion setup

        #region ToInvoiceItem
        public class ToInvoiceItem {
            [Fact]
            public void GoodOrderLine_ReturnsExpectedBrand() {
                // arrange
                OrderLine testdata = MakeTestData();
                string expected = "Fake Brand";

                // act
                InvoiceItemModel results = testdata.ToInvoiceItem();

                // assert
                results.Brand
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedBrandExtendedDescription() {
                // arrange
                OrderLine testdata = MakeTestData();
                string expected = "Fake Brand";

                // act
                InvoiceItemModel results = testdata.ToInvoiceItem();

                // assert
                results.BrandExtendedDescription
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedCases() {
                // arrange
                OrderLine testdata = MakeTestData();
                string expected = "Fake Cases";

                // act
                InvoiceItemModel results = testdata.ToInvoiceItem();

                // assert
                results.Cases
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedCatalogId() {
                // arrange
                OrderLine testdata = MakeTestData();
                string expected = "FUT";

                // act
                InvoiceItemModel results = testdata.ToInvoiceItem();

                // assert
                results.CatalogId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedCatchWeight() {
                // arrange
                OrderLine testdata = MakeTestData();
                bool expected = true;

                // act
                InvoiceItemModel results = testdata.ToInvoiceItem();

                // assert
                results.CatchWeight
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedCategoryCode() {
                // arrange
                OrderLine testdata = MakeTestData();
                string expected = "Fake CategoryCode";

                // act
                InvoiceItemModel results = testdata.ToInvoiceItem();

                // assert
                results.CategoryCode
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedCategoryName() {
                // arrange
                OrderLine testdata = MakeTestData();
                string expected = "Fake CategoryName";

                // act
                InvoiceItemModel results = testdata.ToInvoiceItem();

                // assert
                results.CategoryName
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedChildNutrition() {
                // arrange
                OrderLine testdata = MakeTestData();
                string expected = "Fake ChildNutrition";

                // act
                InvoiceItemModel results = testdata.ToInvoiceItem();

                // assert
                results.ChildNutrition
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedDescription() {
                // arrange
                OrderLine testdata = MakeTestData();
                string expected = "Fake Description";

                // act
                InvoiceItemModel results = testdata.ToInvoiceItem();

                // assert
                results.Description
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedDetail() {
                // arrange
                OrderLine testdata = MakeTestData();
                string expected = "Fake Detail";

                // act
                InvoiceItemModel results = testdata.ToInvoiceItem();

                // assert
                results.Detail
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedExtCatchWeight() {
                // arrange
                OrderLine testdata = MakeTestData();
                int expected = 42;

                // act
                InvoiceItemModel results = testdata.ToInvoiceItem();

                // assert
                results.ExtCatchWeight
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedIsValid() {
                // arrange
                OrderLine testdata = MakeTestData();
                bool expected = true;

                // act
                InvoiceItemModel results = testdata.ToInvoiceItem();

                // assert
                results.IsValid
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedItemClass() {
                // arrange
                OrderLine testdata = MakeTestData();
                string expected = "Fake ItemClass";

                // act
                InvoiceItemModel results = testdata.ToInvoiceItem();

                // assert
                results.ItemClass
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedItemNumber() {
                // arrange
                OrderLine testdata = MakeTestData();
                string expected = "123456";

                // act
                InvoiceItemModel results = testdata.ToInvoiceItem();

                // assert
                results.ItemNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedItemPrice() {
                // arrange
                OrderLine testdata = MakeTestData();
                int expected = 2;

                // act
                InvoiceItemModel results = testdata.ToInvoiceItem();

                // assert
                results.ItemPrice
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedKosher() {
                // arrange
                OrderLine testdata = MakeTestData();
                string expected = "Fake Kosher";

                // act
                InvoiceItemModel results = testdata.ToInvoiceItem();

                // assert
                results.Kosher
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedLineNumber() {
                // arrange
                OrderLine testdata = MakeTestData();
                string expected = "17";

                // act
                InvoiceItemModel results = testdata.ToInvoiceItem();

                // assert
                results.LineNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedManufacturerName() {
                // arrange
                OrderLine testdata = MakeTestData();
                string expected = "Fake ManufacturerName";

                // act
                InvoiceItemModel results = testdata.ToInvoiceItem();

                // assert
                results.ManufacturerName
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedManufacturerNumber() {
                // arrange
                OrderLine testdata = MakeTestData();
                string expected = "Fake ManufacturerNumber";

                // act
                InvoiceItemModel results = testdata.ToInvoiceItem();

                // assert
                results.ManufacturerNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedName() {
                // arrange
                OrderLine testdata = MakeTestData();
                string expected = "Fake Name";

                // act
                InvoiceItemModel results = testdata.ToInvoiceItem();

                // assert
                results.Name
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedNonStock() {
                // arrange
                OrderLine testdata = MakeTestData();
                string expected = "Fake NonStock";

                // act
                InvoiceItemModel results = testdata.ToInvoiceItem();

                // assert
                results.NonStock
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedNutritionalBrandOwner() {
                // arrange
                OrderLine testdata = MakeTestData();
                string expected = "Fake BrandOwner";

                // act
                InvoiceItemModel results = testdata.ToInvoiceItem();

                // assert
                results.Nutritional
                       .BrandOwner
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedPack() {
                // arrange
                OrderLine testdata = MakeTestData();
                string expected = "Fake Pack";

                // act
                InvoiceItemModel results = testdata.ToInvoiceItem();

                // assert
                results.Pack
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedPackSize() {
                // arrange
                OrderLine testdata = MakeTestData();
                string expected = "Fake Pack / Fake Size";

                // act
                InvoiceItemModel results = testdata.ToInvoiceItem();

                // assert
                results.PackSize
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedQuantityOrdered() {
                // arrange
                OrderLine testdata = MakeTestData();
                int expected = 1;

                // act
                InvoiceItemModel results = testdata.ToInvoiceItem();

                // assert
                results.QuantityOrdered
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedQuantityShipped() {
                // arrange
                OrderLine testdata = MakeTestData();
                int expected = 1;

                // act
                InvoiceItemModel results = testdata.ToInvoiceItem();

                // assert
                results.QuantityShipped
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedReplacedItem() {
                // arrange
                OrderLine testdata = MakeTestData();
                string expected = "Fake ReplacedItem";

                // act
                InvoiceItemModel results = testdata.ToInvoiceItem();

                // assert
                results.ReplacedItem
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedReplacementItem() {
                // arrange
                OrderLine testdata = MakeTestData();
                string expected = "Fake ReplacementItem";

                // act
                InvoiceItemModel results = testdata.ToInvoiceItem();

                // assert
                results.ReplacementItem
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedSize() {
                // arrange
                OrderLine testdata = MakeTestData();
                string expected = "Fake Size";

                // act
                InvoiceItemModel results = testdata.ToInvoiceItem();

                // assert
                results.Size
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedSubCategoryCode() {
                // arrange
                OrderLine testdata = MakeTestData();
                string expected = "Fake SubCategoryCode";

                // act
                InvoiceItemModel results = testdata.ToInvoiceItem();

                // assert
                results.SubCategoryCode
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedTempZone() {
                // arrange
                OrderLine testdata = MakeTestData();
                string expected = "Fake TempZone";

                // act
                InvoiceItemModel results = testdata.ToInvoiceItem();

                // assert
                results.TempZone
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedUPC() {
                // arrange
                OrderLine testdata = MakeTestData();
                string expected = "Fake UPC";

                // act
                InvoiceItemModel results = testdata.ToInvoiceItem();

                // assert
                results.UPC
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedVendorItemNumber() {
                // arrange
                OrderLine testdata = MakeTestData();
                string expected = "Fake VendorItemNumber";

                // act
                InvoiceItemModel results = testdata.ToInvoiceItem();

                // assert
                results.VendorItemNumber
                       .Should()
                       .Be(expected);
            }
        }
        #endregion ToInvoiceItem
    }
}