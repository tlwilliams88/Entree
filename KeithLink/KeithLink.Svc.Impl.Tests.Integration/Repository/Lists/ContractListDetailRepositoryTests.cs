using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autofac;

using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Impl.Repository.SmartResolver;

namespace KeithLink.Svc.Impl.Tests.Integration.Repository.Lists {
    public class ContractListDetailRepositoryTests {
        private static IContractListDetailsRepository MakeRepo() {
            IContainer diMap = DependencyMapFactory.GetTestsContainer()
                                                   .Build();

            return diMap.Resolve<IContractListDetailsRepository>();
        }

        public class GetContractListDetails : MigratedDatabaseTest {
            [Fact]
            public void BadHeader_ReturnsNoRecords() {
                // arrange
                var repo = MakeRepo();
                var headerId = 1101;
                var expected = 0;

                // act
                var results = repo.GetContractListDetails(headerId);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedCount() {
                // arrange
                var repo = MakeRepo();
                var headerId = 1;
                var expected = 3;

                // act
                var results = repo.GetContractListDetails(headerId);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedHeaderId() {
                // arrange
                var repo = MakeRepo();
                var headerId = 1;
                var expected = 1;

                // act
                var results = repo.GetContractListDetails(headerId);
                var test = results[0];

                // assert
                test.HeaderId
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedLineNumber() {
                // arrange
                var repo = MakeRepo();
                var headerId = 1;
                var expected = 1;

                // act
                var results = repo.GetContractListDetails(headerId);
                var test = results[0];

                // assert
                test.LineNumber
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedItemNumber() {
                // arrange
                var repo = MakeRepo();
                var headerId = 1;
                var expected = "123456";

                // act
                var results = repo.GetContractListDetails(headerId);
                var test = results[0];

                // assert
                test.ItemNumber
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedFromDate() {
                // arrange
                var repo = MakeRepo();
                var headerId = 1;
                var expected = new DateTime(2017, 6, 1);

                // act
                var results = repo.GetContractListDetails(headerId);
                var test = results[0];

                // assert
                test.FromDate
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedFromDateKind() {
                // arrange
                var repo = MakeRepo();
                var headerId = 1;
                var expected = DateTimeKind.Unspecified;

                // act
                var results = repo.GetContractListDetails(headerId);
                var test = results[0];

                // assert
                test.FromDate
                    .Value
                    .Kind
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedToDate() {
                // arrange
                var repo = MakeRepo();
                var headerId = 1;
                var expected = new DateTime(2017, 6, 30);

                // act
                var results = repo.GetContractListDetails(headerId);
                var test = results[0];

                // assert
                test.ToDate
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedToDateKind() {
                // arrange
                var repo = MakeRepo();
                var headerId = 1;
                var expected = DateTimeKind.Unspecified;

                // act
                var results = repo.GetContractListDetails(headerId);
                var test = results[0];

                // assert
                test.ToDate
                    .Value
                    .Kind
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedEach() {
                // arrange
                var repo = MakeRepo();
                var headerId = 1;
                var expected = false;

                // act
                var results = repo.GetContractListDetails(headerId);
                var test = results[0];

                // assert
                test.Each
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedCategory() {
                // arrange
                var repo = MakeRepo();
                var headerId = 1;
                var expected = "Fake Category";

                // act
                var results = repo.GetContractListDetails(headerId);
                var test = results[0];

                // assert
                test.Category
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedCatalogId() {
                // arrange
                var repo = MakeRepo();
                var headerId = 1;
                var expected = "FDF";

                // act
                var results = repo.GetContractListDetails(headerId);
                var test = results[0];

                // assert
                test.CatalogId
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedCreatedUtc() {
                // arrange
                var repo = MakeRepo();
                var headerId = 1;
                var expected = new DateTime(2017, 6, 22, 15, 1, 0, DateTimeKind.Utc);

                // act
                var results = repo.GetContractListDetails(headerId);
                var test = results[0];

                // assert
                test.CreatedUtc
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedCreatedUtcKind() {
                // arrange
                var repo = MakeRepo();
                var headerId = 1;
                var expected = DateTimeKind.Utc;

                // act
                var results = repo.GetContractListDetails(headerId);
                var test = results[0];

                // assert
                test.CreatedUtc
                    .Kind
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedModifiedUtc() {
                // arrange
                var repo = MakeRepo();
                var headerId = 1;
                var expected = new DateTime(2017, 6, 22, 15, 2, 0, DateTimeKind.Utc);

                // act
                var results = repo.GetContractListDetails(headerId);
                var test = results[0];

                // assert
                test.ModifiedUtc
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedModifiedUtcKind() {
                // arrange
                var repo = MakeRepo();
                var headerId = 1;
                var expected = DateTimeKind.Utc;

                // act
                var results = repo.GetContractListDetails(headerId);
                var test = results[0];

                // assert
                test.ModifiedUtc
                    .Kind
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void MissingFromDate_ReturnsNull() {
                // arrange
                var repo = MakeRepo();
                var headerId = 1;

                // act
                var results = repo.GetContractListDetails(headerId);
                var test = results[2];

                // assert
                test.FromDate
                    .Should()
                    .BeNull();
            }

            [Fact]
            public void MissingToDate_ReturnsNull() {
                // arrange
                var repo = MakeRepo();
                var headerId = 1;

                // act
                var results = repo.GetContractListDetails(headerId);
                var test = results[2];

                // assert
                test.ToDate
                    .Should()
                    .BeNull();
            }

            [Fact]
            public void MissingEach_ReturnsNull() {
                // arrange
                var repo = MakeRepo();
                var headerId = 1;

                // act
                var results = repo.GetContractListDetails(headerId);
                var test = results[2];

                // assert
                test.Each
                    .Should()
                    .BeNull();
            }
        }
    }
}
