using FluentAssertions;

using KeithLink.Svc.Core.Models.Reports;

using Xunit;

using KeithLink.Svc.Core.Models.ShoppingCart;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Tests.Unit.Models.Reports
{
    public class ItemUsageReportItemModelTests
    {
        #region setup
        private static ItemUsageReportItemModel MakeTestData()
        {
            return new ItemUsageReportItemModel()
            {
                ItemNumber = "123456",
                IsValid = true,
                Name = "Fake Name",
                Detail = "Fake Detail",
                Description = "Fake Description",
                Pack = "Fake Pack",
                Size = "Fake Size",
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
                CatalogId = "FUT"
            };
        }
        #endregion

        #region Get_ItemNumber
        public class Get_ItemNumber
        {
            [Fact]
            public void GoodTest_ReturnsExpectedValue()
            {
                // arrange
                var fakeItem = MakeTestData();
                var expected = "123456";

                // act

                // assert
                fakeItem.ItemNumber
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedTest_HasDefaultValue()
            {
                // arrange
                var test = new ShoppingCartItem();

                // act

                // assert
                test.ItemNumber
                    .Should()
                    .BeNull();
            }
        }
        #endregion

        #region Get_IsValid
        public class Get_IsValid
        {
            [Fact]
            public void GoodTest_ReturnsExpectedValue()
            {
                // arrange
                var fakeItem = MakeTestData();
                var expected = true;

                // act

                // assert
                fakeItem.IsValid
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedTest_HasDefaultValue()
            {
                // arrange
                var test = new ShoppingCartItem();

                // act

                // assert
                test.IsValid
                    .Should()
                    .BeFalse();
            }
        }
        #endregion

        #region Get_Name
        public class Get_Name
        {
            [Fact]
            public void GoodTest_ReturnsExpectedValue()
            {
                // arrange
                var fakeItem = MakeTestData();
                var expected = "Fake Name";

                // act

                // assert
                fakeItem.Name
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedTest_HasDefaultValue()
            {
                // arrange
                var test = new ShoppingCartItem();

                // act

                // assert
                test.Name
                    .Should()
                    .BeNull();
            }
        }
        #endregion

        #region Get_Detail
        public class Get_Detail
        {
            [Fact]
            public void GoodTest_ReturnsExpectedValue()
            {
                // arrange
                var fakeItem = MakeTestData();
                var expected = "Fake Detail";

                // act

                // assert
                fakeItem.Detail
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedTest_HasDefaultValue()
            {
                // arrange
                var test = new ShoppingCartItem();

                // act

                // assert
                test.Detail
                    .Should()
                    .BeNull();
            }
        }
        #endregion

        #region Get_Description
        public class Get_Description
        {
            [Fact]
            public void GoodTest_ReturnsExpectedValue()
            {
                // arrange
                var fakeItem = MakeTestData();
                var expected = "Fake Description";

                // act

                // assert
                fakeItem.Description
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedTest_HasDefaultValue()
            {
                // arrange
                var test = new ShoppingCartItem();

                // act

                // assert
                test.Description
                    .Should()
                    .BeNull();
            }
        }
        #endregion

        #region Get_Pack
        public class Get_Pack
        {
            [Fact]
            public void GoodTest_ReturnsExpectedValue()
            {
                // arrange
                var fakeItem = MakeTestData();
                var expected = "Fake Pack";

                // act

                // assert
                fakeItem.Pack
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedTest_HasDefaultValue()
            {
                // arrange
                var test = new ShoppingCartItem();

                // act

                // assert
                test.Pack
                    .Should()
                    .BeNull();
            }
        }
        #endregion

        #region Get_Size
        public class Get_Size
        {
            [Fact]
            public void GoodTest_ReturnsExpectedValue()
            {
                // arrange
                var fakeItem = MakeTestData();
                var expected = "Fake Size";

                // act

                // assert
                fakeItem.Size
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedTest_HasDefaultValue()
            {
                // arrange
                var test = new ShoppingCartItem();

                // act

                // assert
                test.Size
                    .Should()
                    .BeNull();
            }
        }
        #endregion

        #region Get_Brand
        public class Get_Brand
        {
            [Fact]
            public void GoodTest_ReturnsExpectedValue()
            {
                // arrange
                var fakeItem = MakeTestData();
                var expected = "Fake Brand";

                // act

                // assert
                fakeItem.Brand
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedTest_HasDefaultValue()
            {
                // arrange
                var test = new ShoppingCartItem();

                // act

                // assert
                test.Brand
                    .Should()
                    .BeNull();
            }
        }
        #endregion

        #region Get_BrandExtendedDescription
        public class Get_BrandExtendedDescription
        {
            [Fact]
            public void GoodTest_ReturnsExpectedValue()
            {
                // arrange
                var fakeItem = MakeTestData();
                var expected = "Fake Brand";

                // act

                // assert
                fakeItem.BrandExtendedDescription
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedTest_HasDefaultValue()
            {
                // arrange
                var test = new ShoppingCartItem();

                // act

                // assert
                test.BrandExtendedDescription
                    .Should()
                    .BeNull();
            }
        }
        #endregion

        #region Get_ReplacedItem
        public class Get_ReplacedItem
        {
            [Fact]
            public void GoodTest_ReturnsExpectedValue()
            {
                // arrange
                var fakeItem = MakeTestData();
                var expected = "Fake ReplacedItem";

                // act

                // assert
                fakeItem.ReplacedItem
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedTest_HasDefaultValue()
            {
                // arrange
                var test = new ShoppingCartItem();

                // act

                // assert
                test.ReplacedItem
                    .Should()
                    .BeNull();
            }
        }
        #endregion

        #region Get_ReplacementItem
        public class Get_ReplacementItem
        {
            [Fact]
            public void GoodTest_ReturnsExpectedValue()
            {
                // arrange
                var fakeItem = MakeTestData();
                var expected = "Fake ReplacementItem";

                // act

                // assert
                fakeItem.ReplacementItem
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedTest_HasDefaultValue()
            {
                // arrange
                var test = new ShoppingCartItem();

                // act

                // assert
                test.ReplacementItem
                    .Should()
                    .BeNull();
            }
        }
        #endregion

        #region Get_NonStock
        public class Get_NonStock
        {
            [Fact]
            public void GoodTest_ReturnsExpectedValue()
            {
                // arrange
                var fakeItem = MakeTestData();
                var expected = "Fake NonStock";

                // act

                // assert
                fakeItem.NonStock
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedTest_HasDefaultValue()
            {
                // arrange
                var test = new ShoppingCartItem();

                // act

                // assert
                test.NonStock
                    .Should()
                    .BeNull();
            }
        }
        #endregion

        #region Get_ChildNutrition
        public class Get_ChildNutrition
        {
            [Fact]
            public void GoodTest_ReturnsExpectedValue()
            {
                // arrange
                var fakeItem = MakeTestData();
                var expected = "Fake ChildNutrition";

                // act

                // assert
                fakeItem.ChildNutrition
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedTest_HasDefaultValue()
            {
                // arrange
                var test = new ShoppingCartItem();

                // act

                // assert
                test.ChildNutrition
                    .Should()
                    .BeNull();
            }
        }
        #endregion

        #region Get_CatchWeight
        public class Get_CatchWeight
        {
            [Fact]
            public void GoodTest_ReturnsExpectedValue()
            {
                // arrange
                var fakeItem = MakeTestData();
                var expected = true;

                // act

                // assert
                fakeItem.CatchWeight
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedTest_HasDefaultValue()
            {
                // arrange
                var test = new ShoppingCartItem();

                // act

                // assert
                test.CatchWeight
                    .Should()
                    .BeFalse();
            }
        }
        #endregion

        #region Get_TempZone
        public class Get_TempZone
        {
            [Fact]
            public void GoodTest_ReturnsExpectedValue()
            {
                // arrange
                var fakeItem = MakeTestData();
                var expected = "Fake TempZone";

                // act

                // assert
                fakeItem.TempZone
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedTest_HasDefaultValue()
            {
                // arrange
                var test = new ShoppingCartItem();

                // act

                // assert
                test.TempZone
                    .Should()
                    .BeNull();
            }
        }
        #endregion

        #region Get_ItemClass
        public class Get_ItemClass
        {
            [Fact]
            public void GoodTest_ReturnsExpectedValue()
            {
                // arrange
                var fakeItem = MakeTestData();
                var expected = "Fake ItemClass";

                // act

                // assert
                fakeItem.ItemClass
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedTest_HasDefaultValue()
            {
                // arrange
                var test = new ShoppingCartItem();

                // act

                // assert
                test.ItemClass
                    .Should()
                    .BeNull();
            }
        }
        #endregion

        #region Get_CategoryCode
        public class Get_CategoryCode
        {
            [Fact]
            public void GoodTest_ReturnsExpectedValue()
            {
                // arrange
                var fakeItem = MakeTestData();
                var expected = "Fake CategoryCode";

                // act

                // assert
                fakeItem.CategoryCode
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedTest_HasDefaultValue()
            {
                // arrange
                var test = new ShoppingCartItem();

                // act

                // assert
                test.CategoryCode
                    .Should()
                    .BeNull();
            }
        }
        #endregion

        #region Get_SubCategoryCode
        public class Get_SubCategoryCode
        {
            [Fact]
            public void GoodTest_ReturnsExpectedValue()
            {
                // arrange
                var fakeItem = MakeTestData();
                var expected = "Fake SubCategoryCode";

                // act

                // assert
                fakeItem.SubCategoryCode
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedTest_HasDefaultValue()
            {
                // arrange
                var test = new ShoppingCartItem();

                // act

                // assert
                test.SubCategoryCode
                    .Should()
                    .BeNull();
            }
        }
        #endregion

        #region Get_CategoryName
        public class Get_CategoryName
        {
            [Fact]
            public void GoodTest_ReturnsExpectedValue()
            {
                // arrange
                var fakeItem = MakeTestData();
                var expected = "Fake CategoryName";

                // act

                // assert
                fakeItem.CategoryName
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedTest_HasDefaultValue()
            {
                // arrange
                var test = new ShoppingCartItem();

                // act

                // assert
                test.CategoryName
                    .Should()
                    .BeNull();
            }
        }
        #endregion

        #region Get_UPC
        public class Get_UPC
        {
            [Fact]
            public void GoodTest_ReturnsExpectedValue()
            {
                // arrange
                var fakeItem = MakeTestData();
                var expected = "Fake UPC";

                // act

                // assert
                fakeItem.UPC
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedTest_HasDefaultValue()
            {
                // arrange
                var test = new ShoppingCartItem();

                // act

                // assert
                test.UPC
                    .Should()
                    .BeNull();
            }
        }
        #endregion

        #region Get_VendorItemNumber
        public class Get_VendorItemNumber
        {
            [Fact]
            public void GoodTest_ReturnsExpectedValue()
            {
                // arrange
                var fakeItem = MakeTestData();
                var expected = "Fake VendorItemNumber";

                // act

                // assert
                fakeItem.VendorItemNumber
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedTest_HasDefaultValue()
            {
                // arrange
                var test = new ShoppingCartItem();

                // act

                // assert
                test.VendorItemNumber
                    .Should()
                    .BeNull();
            }
        }
        #endregion

        #region Get_Cases
        public class Get_Cases
        {
            [Fact]
            public void GoodTest_ReturnsExpectedValue()
            {
                // arrange
                var fakeItem = MakeTestData();
                var expected = "Fake Cases";

                // act

                // assert
                fakeItem.Cases
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedTest_HasDefaultValue()
            {
                // arrange
                var test = new ShoppingCartItem();

                // act

                // assert
                test.Cases
                    .Should()
                    .BeNull();
            }
        }
        #endregion

        #region Get_Kosher
        public class Get_Kosher
        {
            [Fact]
            public void GoodTest_ReturnsExpectedValue()
            {
                // arrange
                var fakeItem = MakeTestData();
                var expected = "Fake Kosher";

                // act

                // assert
                fakeItem.Kosher
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedTest_HasDefaultValue()
            {
                // arrange
                var test = new ShoppingCartItem();

                // act

                // assert
                test.Kosher
                    .Should()
                    .BeNull();
            }
        }
        #endregion

        #region Get_ManufacturerName
        public class Get_ManufacturerName
        {
            [Fact]
            public void GoodTest_ReturnsExpectedValue()
            {
                // arrange
                var fakeItem = MakeTestData();
                var expected = "Fake ManufacturerName";

                // act

                // assert
                fakeItem.ManufacturerName
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedTest_HasDefaultValue()
            {
                // arrange
                var test = new ShoppingCartItem();

                // act

                // assert
                test.ManufacturerName
                    .Should()
                    .BeNull();
            }
        }
        #endregion

        #region Get_ManufacturerNumber
        public class Get_ManufacturerNumber
        {
            [Fact]
            public void GoodTest_ReturnsExpectedValue()
            {
                // arrange
                var fakeItem = MakeTestData();
                var expected = "Fake ManufacturerNumber";

                // act

                // assert
                fakeItem.ManufacturerNumber
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedTest_HasDefaultValue()
            {
                // arrange
                var test = new ShoppingCartItem();

                // act

                // assert
                test.ManufacturerNumber
                    .Should()
                    .BeNull();
            }
        }
        #endregion

        #region Get_Nutritional
        public class Get_Nutritional
        {
            [Fact]
            public void GoodTest_NotNull()
            {
                // arrange
                var fakeItem = MakeTestData();

                // act

                // assert
                fakeItem.Nutritional
                        .Should()
                        .NotBeNull();
            }

            [Fact]
            public void InitalizedTest_HasDefaultValue()
            {
                // arrange
                var test = new ShoppingCartItem();

                // act

                // assert
                test.Nutritional
                    .Should()
                    .BeNull();
            }
        }
        #endregion

        #region Get_CatalogId
        public class Get_CatalogId
        {
            [Fact]
            public void GoodTest_ReturnsExpectedValue()
            {
                // arrange
                var fakeItem = MakeTestData();
                var expected = "FUT";

                // act

                // assert
                fakeItem.CatalogId
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedTest_HasDefaultValue()
            {
                // arrange
                var test = new ShoppingCartItem();

                // act

                // assert
                test.CatalogId
                    .Should()
                    .BeNull();
            }
        }
        #endregion

    }
}
