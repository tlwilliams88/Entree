using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autofac;
using FluentAssertions;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.SmartResolver;
using Xunit;


namespace KeithLink.Svc.Impl.Tests.Integration.Repository.Lists {
    public class HistoryListHeaderRepositoryTests {
        private static IHistoryListHeaderRepository MakeRepo() {
            IContainer diMap = DependencyMapFactory.GetTestsContainer()
                                                   .Build();
            return diMap.Resolve<IHistoryListHeaderRepository>();
        }

        public class GetHistoryListHeader {
            [Fact]
            public void GoodCustomer_ReturnsExpectedBranchId() {
                // arrange
                var custInfo= new UserSelectedContext() {
                    BranchId = "FRT",
                    CustomerId = "123456"
                };
                var expected = "FRT";
                var repo = MakeRepo();

                // act
                var results = repo.GetHistoryListHeader(custInfo);

                // assert
                results.BranchId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodCustomer_ReturnsExpectedCustomerNumber() {
                // arrange
                var custInfo = new UserSelectedContext() {
                    BranchId = "FRT",
                    CustomerId = "123456"
                };
                var expected = "123456";
                var repo = MakeRepo();

                // act
                var results = repo.GetHistoryListHeader(custInfo);

                // assert
                results.CustomerNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodCustomer_ReturnsExpectedCreatedUtc() {
                // arrange
                var custInfo = new UserSelectedContext() {
                    BranchId = "FRT",
                    CustomerId = "123456"
                };
                var expected = new DateTime(2017, 6, 30, 11, 47, 0, DateTimeKind.Utc);
                var repo = MakeRepo();

                // act
                var results = repo.GetHistoryListHeader(custInfo);

                // assert
                results.CreatedUtc
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodCustomer_ReturnsExpectedModifiedUtc() {
                // arrange
                var custInfo = new UserSelectedContext() {
                    BranchId = "FRT",
                    CustomerId = "123456"
                };
                var expected = new DateTime(2017, 6, 30, 11, 48, 0, DateTimeKind.Utc);
                var repo = MakeRepo();

                // act
                var results = repo.GetHistoryListHeader(custInfo);

                // assert
                results.ModifiedUtc
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void BadBranch_ReturnsNull() {
                // arrange
                var custInfo = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var repo = MakeRepo();

                // act
                var results = repo.GetHistoryListHeader(custInfo);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void BadCustomerNumber_ReturnsNull() {
                // arrange
                var custInfo = new UserSelectedContext() {
                    BranchId = "FRT",
                    CustomerId = "999999"
                };
                var repo = MakeRepo();

                // act
                var results = repo.GetHistoryListHeader(custInfo);

                // assert
                results.Should()
                       .BeNull();
            }
        }
    }
}
