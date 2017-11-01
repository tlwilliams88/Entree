using System.Collections.Generic;
using System.Linq;

using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Tests.Unit.Models.Lists {
    public class ListModelTests {
        private static ListModel MakeModel() {
            return new ListModel {
                BranchId = "FUT",
                CustomerNumber = "123456",
                ListId = 1,
                Name = "Fake Name",
                Type = ListType.Custom,
                Items = new List<ListItemModel> {
                    new ListItemModel {
                        ItemNumber = "111111",
                        Position = 99,
                        Each = false,
                        Pack = "1",
                        Size = "2 OZ",
                        CatalogId = "FUT",
                        CustomInventoryItemId = 2
                    },
                    new ListItemModel {
                        ItemNumber = "555555",
                        Position = 55,
                        Pack = "3",
                        Size = "4 OZ",
                        Each = true,
                        CatalogId = "FUT",
                        CustomInventoryItemId = 0
                    }
                }
            };
        }

        public class NewCopy {
            [Fact]
            public void NewCopy_ResultHasExpectedBranch() {
                // arrange
                ListModel testmodel = MakeModel();
                string expected = "FUT";

                // act
                ListModel result = testmodel.NewCopy();

                // assert - Always returns what is setup provided the mock is called
                result.BranchId
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void NewCopy_ResultHasExpectedCustomerNumber() {
                // arrange
                ListModel testmodel = MakeModel();
                string expected = "123456";

                // act
                ListModel result = testmodel.NewCopy();

                // assert - Always returns what is setup provided the mock is called
                result.CustomerNumber
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void NewCopy_ResultHasExpectedName() {
                // arrange
                ListModel testmodel = MakeModel();
                string expected = "Fake Name copy";

                // act
                ListModel result = testmodel.NewCopy();

                // assert - Always returns what is setup provided the mock is called
                result.Name
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void NewCopy_ResultHasExpectedPosition() {
                // arrange
                ListModel testmodel = MakeModel();
                string itemNumber = "555555";
                int expected = 55;

                // act
                ListModel result = testmodel.NewCopy();

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
                ListModel testmodel = MakeModel();
                ListType expected = ListType.Custom;

                // act
                ListModel result = testmodel.NewCopy();

                // assert - Always returns what is setup provided the mock is called
                result.Type
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void NewCopy_ResultHasItemWithExpectedCatalogId() {
                // arrange
                ListModel testmodel = MakeModel();
                string expected = "FUT";

                // act
                ListModel result = testmodel.NewCopy();

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
                ListModel testmodel = MakeModel();
                int expected = 2;

                // act
                ListModel result = testmodel.NewCopy();

                // assert - Always returns what is setup provided the mock is called
                result.Items
                      .First()
                      .CustomInventoryItemId
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void NewCopy_ResultHasItemWithExpectedEach() {
                // arrange
                ListModel testmodel = MakeModel();
                bool expected = false;

                // act
                ListModel result = testmodel.NewCopy();

                // assert - Always returns what is setup provided the mock is called
                result.Items
                      .First()
                      .Each
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void NewCopy_ResultHasItemWithExpectedItemNumber() {
                // arrange
                ListModel testmodel = MakeModel();
                string expected = "111111";

                // act
                ListModel result = testmodel.NewCopy();

                // assert - Always returns what is setup provided the mock is called
                result.Items
                      .First()
                      .ItemNumber
                      .Should()
                      .Be(expected);
            }
        }

    }
}