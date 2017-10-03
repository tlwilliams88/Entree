using KeithLink.Svc.Core.Extensions.Orders;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.SiteCatalog;

using FluentAssertions;
using Xunit;

namespace KeithLink.Svc.Core.Tests.Unit.Extensions.Orders
{
    public class OrderLineExtensionTests
    {
        #region setup

        private static OrderLine MakeTestData()
        {
            return new OrderLine()
            {
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
                Nutritional = new Nutritional()
                {
                    BrandOwner = "Fake BrandOwner"
                },
                TotalShippedWeight = 42,
                CatalogId = "FUT"
            };
        }

        #endregion setup

        #region ToInvoiceItem

        public class ToInvoiceItem
        {
            [Fact]
            public void GoodOrderLine_ReturnsExpectedItemNumber()
            {
                // arrange
                var testdata = MakeTestData();
                var expected = "123456";

                // act
                var results = testdata.ToInvoiceItem();

                // assert
                results.ItemNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedItemPrice()
            {
                // arrange
                var testdata = MakeTestData();
                var expected = 2;

                // act
                var results = testdata.ToInvoiceItem();

                // assert
                results.ItemPrice
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedQuantityShipped()
            {
                // arrange
                var testdata = MakeTestData();
                var expected = 1;

                // act
                var results = testdata.ToInvoiceItem();

                // assert
                results.QuantityShipped
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedQuantityOrdered()
            {
                // arrange
                var testdata = MakeTestData();
                var expected = 1;

                // act
                var results = testdata.ToInvoiceItem();

                // assert
                results.QuantityOrdered
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedLineNumber()
            {
                // arrange
                var testdata = MakeTestData();
                var expected = "17";

                // act
                var results = testdata.ToInvoiceItem();

                // assert
                results.LineNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedIsValid()
            {
                // arrange
                var testdata = MakeTestData();
                var expected = true;

                // act
                var results = testdata.ToInvoiceItem();

                // assert
                results.IsValid
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedName()
            {
                // arrange
                var testdata = MakeTestData();
                var expected = "Fake Name";

                // act
                var results = testdata.ToInvoiceItem();

                // assert
                results.Name
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedDetail()
            {
                // arrange
                var testdata = MakeTestData();
                var expected = "Fake Detail";

                // act
                var results = testdata.ToInvoiceItem();

                // assert
                results.Detail
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedDescription()
            {
                // arrange
                var testdata = MakeTestData();
                var expected = "Fake Description";

                // act
                var results = testdata.ToInvoiceItem();

                // assert
                results.Description
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedPackSize()
            {
                // arrange
                var testdata = MakeTestData();
                var expected = "Fake Pack / Fake Size";

                // act
                var results = testdata.ToInvoiceItem();

                // assert
                results.PackSize
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedPack()
            {
                // arrange
                var testdata = MakeTestData();
                var expected = "Fake Pack";

                // act
                var results = testdata.ToInvoiceItem();

                // assert
                results.Pack
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedSize()
            {
                // arrange
                var testdata = MakeTestData();
                var expected = "Fake Size";

                // act
                var results = testdata.ToInvoiceItem();

                // assert
                results.Size
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedBrand()
            {
                // arrange
                var testdata = MakeTestData();
                var expected = "Fake Brand";

                // act
                var results = testdata.ToInvoiceItem();

                // assert
                results.Brand
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedBrandExtendedDescription()
            {
                // arrange
                var testdata = MakeTestData();
                var expected = "Fake Brand";

                // act
                var results = testdata.ToInvoiceItem();

                // assert
                results.BrandExtendedDescription
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedReplacedItem()
            {
                // arrange
                var testdata = MakeTestData();
                var expected = "Fake ReplacedItem";

                // act
                var results = testdata.ToInvoiceItem();

                // assert
                results.ReplacedItem
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedReplacementItem()
            {
                // arrange
                var testdata = MakeTestData();
                var expected = "Fake ReplacementItem";

                // act
                var results = testdata.ToInvoiceItem();

                // assert
                results.ReplacementItem
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedNonStock()
            {
                // arrange
                var testdata = MakeTestData();
                var expected = "Fake NonStock";

                // act
                var results = testdata.ToInvoiceItem();

                // assert
                results.NonStock
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedChildNutrition()
            {
                // arrange
                var testdata = MakeTestData();
                var expected = "Fake ChildNutrition";

                // act
                var results = testdata.ToInvoiceItem();

                // assert
                results.ChildNutrition
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedCatchWeight()
            {
                // arrange
                var testdata = MakeTestData();
                var expected = true;

                // act
                var results = testdata.ToInvoiceItem();

                // assert
                results.CatchWeight
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedTempZone()
            {
                // arrange
                var testdata = MakeTestData();
                var expected = "Fake TempZone";

                // act
                var results = testdata.ToInvoiceItem();

                // assert
                results.TempZone
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedItemClass()
            {
                // arrange
                var testdata = MakeTestData();
                var expected = "Fake ItemClass";

                // act
                var results = testdata.ToInvoiceItem();

                // assert
                results.ItemClass
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedCategoryCode()
            {
                // arrange
                var testdata = MakeTestData();
                var expected = "Fake CategoryCode";

                // act
                var results = testdata.ToInvoiceItem();

                // assert
                results.CategoryCode
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedSubCategoryCode()
            {
                // arrange
                var testdata = MakeTestData();
                var expected = "Fake SubCategoryCode";

                // act
                var results = testdata.ToInvoiceItem();

                // assert
                results.SubCategoryCode
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedCategoryName()
            {
                // arrange
                var testdata = MakeTestData();
                var expected = "Fake CategoryName";

                // act
                var results = testdata.ToInvoiceItem();

                // assert
                results.CategoryName
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedUPC()
            {
                // arrange
                var testdata = MakeTestData();
                var expected = "Fake UPC";

                // act
                var results = testdata.ToInvoiceItem();

                // assert
                results.UPC
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedVendorItemNumber()
            {
                // arrange
                var testdata = MakeTestData();
                var expected = "Fake VendorItemNumber";

                // act
                var results = testdata.ToInvoiceItem();

                // assert
                results.VendorItemNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedCases()
            {
                // arrange
                var testdata = MakeTestData();
                var expected = "Fake Cases";

                // act
                var results = testdata.ToInvoiceItem();

                // assert
                results.Cases
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedKosher()
            {
                // arrange
                var testdata = MakeTestData();
                var expected = "Fake Kosher";

                // act
                var results = testdata.ToInvoiceItem();

                // assert
                results.Kosher
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedManufacturerName()
            {
                // arrange
                var testdata = MakeTestData();
                var expected = "Fake ManufacturerName";

                // act
                var results = testdata.ToInvoiceItem();

                // assert
                results.ManufacturerName
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedManufacturerNumber()
            {
                // arrange
                var testdata = MakeTestData();
                var expected = "Fake ManufacturerNumber";

                // act
                var results = testdata.ToInvoiceItem();

                // assert
                results.ManufacturerNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedNutritionalBrandOwner()
            {
                // arrange
                var testdata = MakeTestData();
                var expected = "Fake BrandOwner";

                // act
                var results = testdata.ToInvoiceItem();

                // assert
                results.Nutritional
                       .BrandOwner
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedExtCatchWeight()
            {
                // arrange
                var testdata = MakeTestData();
                var expected = 42;

                // act
                var results = testdata.ToInvoiceItem();

                // assert
                results.ExtCatchWeight
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodOrderLine_ReturnsExpectedCatalogId()
            {
                // arrange
                var testdata = MakeTestData();
                var expected = "FUT";

                // act
                var results = testdata.ToInvoiceItem();

                // assert
                results.CatalogId
                       .Should()
                       .Be(expected);
            }
        }

        #endregion ToInvoiceItem
    }
}