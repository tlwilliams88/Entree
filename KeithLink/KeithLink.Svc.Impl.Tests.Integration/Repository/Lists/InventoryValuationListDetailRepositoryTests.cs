using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autofac;
using FluentAssertions;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Impl.Repository.SmartResolver;
using Xunit;

namespace KeithLink.Svc.Impl.Tests.Integration.Repository.Lists {
    public class InventoryValuationListDetailRepositoryTests {
        public static IInventoryValuationListDetailsRepository MakeRepo() {
            IContainer diMap = DependencyMapFactory.GetTestsContainer()
                                                   .Build();
            return diMap.Resolve<IInventoryValuationListDetailsRepository>();
        }

        public class GetInventoryValuationDetails : MigratedDatabaseTest {
            [Fact]
            public void BadHeaderId_ReturnsNoResults() {
                // arrange
                var expected = 0;
                var headerId = 0;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationDetails(headerId);
                
                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodeaderId_ReturnsExpectedCount() {
                // arrange
                var expected = 3;
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationDetails(headerId);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodeaderId_ReturnsExpectedCountOfActive() {
                // arrange
                var expected = 3;
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationDetails(headerId);

                // assert
                results.Count(r => r.Active)
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodeaderId_ReturnsExpectedCountOfInactive() {
                // arrange
                var expected = 0;
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationDetails(headerId);

                // assert
                results.Count(r => r.Active == false)
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedHeaderId() {
                // arrange
                var expected = 1;
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationDetails(headerId);

                // assert
                results[0]
                    .HeaderId
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedCustomInventoryItemId() {
                // arrange
                var expected = 100;
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationDetails(headerId);

                // assert
                results[0]
                    .CustomInventoryItemId
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedItemNumber() {
                // arrange
                var expected = "123456";
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationDetails(headerId);

                // assert
                results[0]
                    .ItemNumber
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedLineNumber() {
                // arrange
                var expected = 1;
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationDetails(headerId);

                // assert
                results[0]
                    .LineNumber
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedEach() {
                // arrange
                var expected = true;
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationDetails(headerId);

                // assert
                results[0]
                    .Each
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedQuantity() {
                // arrange
                var expected = 2;
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationDetails(headerId);

                // assert
                results[0]
                    .HeaderId
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedCatalogId() {
                // arrange
                var expected = "FDF";
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationDetails(headerId);

                // assert
                results[0]
                    .CatalogId
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedActive() {
                // arrange
                var expected = true;
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationDetails(headerId);

                // assert
                results[0]
                    .Active
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedCreatedUtc() {
                // arrange
                var expected = new DateTime(2017, 7, 3, 11, 33, 0, DateTimeKind.Utc);
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationDetails(headerId);

                // assert
                results[0]
                    .CreatedUtc
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedModifiedUtc() {
                // arrange
                var expected = new DateTime(2017, 7, 3, 11, 34, 0, DateTimeKind.Utc);
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationDetails(headerId);

                // assert
                results[0]
                    .ModifiedUtc
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void DetailWithNullEach_ReturnsNull() {
                // arrange
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationDetails(headerId);

                // assert
                results[3]
                    .Each
                    .Should()
                    .BeNull();
            }

            [Fact]
            public void DetailWithNullCatalogId_ReturnsNull() {
                // arrange
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationDetails(headerId);

                // assert
                results[3]
                    .CatalogId
                    .Should()
                    .BeNull();
            }

            [Fact]
            public void DetailWithNullCustomInventoryItemId_ReturnsNull() {
                // arrange
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationDetails(headerId);

                // assert
                results[3]
                    .CustomInventoryItemId
                    .Should()
                    .BeNull();
            }
        }

        public class SaveInventoryValudationDetail : MigratedDatabaseTest { }
    }
}
