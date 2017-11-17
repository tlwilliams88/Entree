using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Models.Marketing;


namespace KeithLink.Svc.Core.Tests.Unit.Models.Marketing {
    public class CatalogCampaignItemTests {
        private static CatalogCampaignItem MakeItem() {
            return new CatalogCampaignItem() {
                Active = true,
                CatalogCampaignHeaderId = 23,
                Id = 15,
                ItemNumber = "Fake Item Number"
            };
        }

        public class Active {
            [Fact]
            public void GoodItem_HasExpectecValue() {
                // arrange
                var fakeItem = MakeItem();
                var expected = true;

                // act

                // assert
                fakeItem.Active
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitializedItem_HasDefaultValue() {
                // arrange
                var test = new CatalogCampaignItem();
                var expected = false;

                // act

                // assert
                test.Active
                    .Should()
                    .Be(expected);
            }
        }

        public class CatalogCampaignHeaderId {
            [Fact]
            public void GoodItem_HasExpectecValue() {
                // arrange
                var fakeItem = MakeItem();
                var expected = 23;

                // act

                // assert
                fakeItem.CatalogCampaignHeaderId
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitializedItem_HasDefaultValue() {
                // arrange
                var test = new CatalogCampaignItem();
                var expected = 0;

                // act

                // assert
                test.CatalogCampaignHeaderId
                    .Should()
                    .Be(expected);
            }
        }

        public class Id {
            [Fact]
            public void GoodItem_HasExpectecValue() {
                // arrange
                var fakeItem = MakeItem();
                var expected = 15;

                // act

                // assert
                fakeItem.Id
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitializedItem_HasDefaultValue() {
                // arrange
                var test = new CatalogCampaignItem();
                var expected = 0;

                // act

                // assert
                test.Id
                    .Should()
                    .Be(expected);
            }
        }

        public class ItemNumber {
            [Fact]
            public void GoodItem_HasExpectecValue() {
                // arrange
                var fakeItem = MakeItem();
                var expected = "Fake Item Number";

                // act

                // assert
                fakeItem.ItemNumber
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitializedItem_HasDefaultValue() {
                // arrange
                var test = new CatalogCampaignItem();

                // act

                // assert
                test.ItemNumber
                    .Should()
                    .BeNull();
            }
        }
    }
}
