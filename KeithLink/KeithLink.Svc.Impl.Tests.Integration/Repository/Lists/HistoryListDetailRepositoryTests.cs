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
    public class HistoryListDetailRepositoryTests {
        private static IHistoryListDetailRepository MakeRepo() {
            IContainer diMap = DependencyMapFactory.GetTestsContainer()
                                                   .Build();
            return diMap.Resolve<IHistoryListDetailRepository>();
        }

        public class GetAllHistoryDetails : MigratedDatabaseTest {
            [Fact]
            public void BadHeaderId_ReturnsExpectedCount() {
                // arrange
                var expected = 0;
                var headerId = 900;
                var repo = MakeRepo();

                // act
                var results = repo.GetAllHistoryDetails(headerId);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderId_ReturnsExpectedCount() {
                // arrange
                var expected = 4;
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetAllHistoryDetails(headerId);
                
                // assert
                results.Count
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
                var results = repo.GetAllHistoryDetails(headerId);

                // assert
                results[0]
                    .HeaderId
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
                var results = repo.GetAllHistoryDetails(headerId);

                // assert
                results[0]
                    .LineNumber
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
                var results = repo.GetAllHistoryDetails(headerId);

                // assert
                results[0]
                    .ItemNumber
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
                var results = repo.GetAllHistoryDetails(headerId);

                // assert
                results[0]
                    .Each
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedCatalogId() {
                // arrange
                var expected = "FRT";
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetAllHistoryDetails(headerId);

                // assert
                results[0]
                    .CatalogId
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedCreatedUtc() {
                // arrange
                var expected = new DateTime(2017, 6, 30, 15, 15, 0, DateTimeKind.Utc);
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetAllHistoryDetails(headerId);

                // assert
                results[0]
                    .CreatedUtc
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedModifiedUtc() {
                // arrange
                var expected = new DateTime(2017, 6, 30, 15, 16, 0, DateTimeKind.Utc);
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetAllHistoryDetails(headerId);

                // assert
                results[0]
                    .ModifiedUtc
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void MissingEach_ReturnsNull() {
                // arrange
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetAllHistoryDetails(headerId);

                // assert
                results[3]
                    .Each
                    .Should()
                    .BeNull();
            }

            [Fact]
            public void MissingCatalog_ReturnsNull() {
                // arrange
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetAllHistoryDetails(headerId);

                // assert
                results[3]
                    .CatalogId
                    .Should()
                    .BeNull();
            }
        }
    }
}
