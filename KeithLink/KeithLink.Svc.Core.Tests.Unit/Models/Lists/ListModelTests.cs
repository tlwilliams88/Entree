using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentAssertions;

using Moq;

using Xunit;

using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Lists;

namespace KeithLink.Svc.Core.Tests.Unit.Models.Lists {
    public class ListModelTests {
        private static ListModel MakeModel() {
            return new ListModel() {
                BranchId = "FUT",
                CustomerNumber = "123456",
                ListId = 1,
                Name = "Fake Name",
                Type = ListType.Custom,
                Items = new List<ListItemModel>() {
                    new ListItemModel() {
                        ItemNumber = "111111",
                        Position = 99,
                        Each = false,
                        CatalogId = "FUT",
                        CustomInventoryItemId = 2
                    },
                    new ListItemModel() {
                        ItemNumber = "555555",
                        Position = 55,
                        Each = true,
                        CatalogId = "FUT",
                        CustomInventoryItemId = 0
                    }
                }
            };
        }

        #region NewCopy
        public class NewCopy {
            [Fact]
            public void NewCopy_ResultHasExpectedBranch() {
                // arrange
                var testmodel = MakeModel();
                var expected = "FUT";

                // act
                var result = testmodel.NewCopy();

                // assert - Always returns what is setup provided the mock is called
                result.BranchId
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void NewCopy_ResultHasExpectedCustomerNumber() {
                // arrange
                var testmodel = MakeModel();
                var expected = "123456";

                // act
                var result = testmodel.NewCopy();

                // assert - Always returns what is setup provided the mock is called
                result.CustomerNumber
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void NewCopy_ResultHasExpectedName() {
                // arrange
                var testmodel = MakeModel();
                var expected = "Fake Name copy";

                // act
                var result = testmodel.NewCopy();

                // assert - Always returns what is setup provided the mock is called
                result.Name
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void NewCopy_ResultHasExpectedPosition() {
                // arrange
                var testmodel = MakeModel();
                var itemNumber = "555555";
                var expected = 55;

                // act
                var result = testmodel.NewCopy();

                // assert - Always returns what is setup provided the mock is called
                result.Items
                      .First(x => x.ItemNumber.Equals(itemNumber))
                      .Position
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void NewCopy_ResultHasExpectedType() {
                // arrange
                var testmodel = MakeModel();
                var expected = ListType.Custom;

                // act
                var result = testmodel.NewCopy();

                // assert - Always returns what is setup provided the mock is called
                result.Type
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void NewCopy_ResultHasItemWithExpectedItemNumber() {
                // arrange
                var testmodel = MakeModel();
                var expected = "111111";

                // act
                var result = testmodel.NewCopy();

                // assert - Always returns what is setup provided the mock is called
                result.Items
                      .First()
                      .ItemNumber
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void NewCopy_ResultHasItemWithExpectedEach() {
                // arrange
                var testmodel = MakeModel();
                var expected = false;

                // act
                var result = testmodel.NewCopy();

                // assert - Always returns what is setup provided the mock is called
                result.Items
                      .First()
                      .Each
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void NewCopy_ResultHasItemWithExpectedCatalogId() {
                // arrange
                var testmodel = MakeModel();
                var expected = "FUT";

                // act
                var result = testmodel.NewCopy();

                // assert - Always returns what is setup provided the mock is called
                result.Items
                      .First()
                      .CatalogId
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void NewCopy_ResultHasItemWithExpectedCustomInventoryItemId() {
                // arrange
                var testmodel = MakeModel();
                var expected = 2;

                // act
                var result = testmodel.NewCopy();

                // assert - Always returns what is setup provided the mock is called
                result.Items
                      .First()
                      .CustomInventoryItemId
                      .Should()
                      .Be(expected);
            }
        }
        #endregion
    }
}