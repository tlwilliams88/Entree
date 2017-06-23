using System;

using Autofac;
using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.SmartResolver;

namespace KeithLink.Svc.Impl.Tests.Integration.Repository.Lists {
    public class ContractListHeaderRepositoryTests {
        private static IContractListHeadersRepository MakeRepo() {
            IContainer diMap = DependencyMapFactory.GetTestsContainer()
                                                   .Build();
            return diMap.Resolve<IContractListHeadersRepository>();
        }

        public class GetListHeaderForCustomer : MigratedDatabaseTest {
            [Fact]
            public void GoodCustomer_ReturnsExpectedContractId() {
                // arrange
                var repo = MakeRepo();
                var expected = "12345678";
                UserSelectedContext customerInfo = new UserSelectedContext() {
                    BranchId = "FDF",
                    CustomerId = "123456"
                };

                // act
                var result = repo.GetListHeaderForCustomer(customerInfo);

                // assert
                result.ContractId
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodCustomer_ReturnsExpectedBranchId() {
                // arrange
                var repo = MakeRepo();
                var expected = "FDF";
                UserSelectedContext customerInfo = new UserSelectedContext() {
                                                                                 BranchId = "FDF",
                                                                                 CustomerId = "123456"
                                                                             };

                // act
                var result = repo.GetListHeaderForCustomer(customerInfo);

                // assert
                result.BranchId
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodCustomer_ReturnsExpectedCustomerNumber() {
                // arrange
                var repo = MakeRepo();
                var expected = "123456";
                UserSelectedContext customerInfo = new UserSelectedContext() {
                                                                                 BranchId = "FDF",
                                                                                 CustomerId = "123456"
                                                                             };

                // act
                var result = repo.GetListHeaderForCustomer(customerInfo);

                // assert
                result.CustomerNumber
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodCustomer_ReturnsExpectedCreatedUtc() {
                // arrange
                var repo = MakeRepo();
                var expected = new DateTime(2017, 6, 16, 15, 28, 0, DateTimeKind.Utc);
                UserSelectedContext customerInfo = new UserSelectedContext() {
                                                                                 BranchId = "FDF",
                                                                                 CustomerId = "123456"
                                                                             };

                // act
                var result = repo.GetListHeaderForCustomer(customerInfo);

                // assert
                result.CreatedUtc
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodCustomer_ReturnsExpectedCreatedUtcKind() {
                // arrange
                var repo = MakeRepo();
                var expected = DateTimeKind.Utc;
                UserSelectedContext customerInfo = new UserSelectedContext() {
                                                                                 BranchId = "FDF",
                                                                                 CustomerId = "123456"
                                                                             };

                // act
                var result = repo.GetListHeaderForCustomer(customerInfo);

                // assert
                result.CreatedUtc
                      .Kind
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodCustomer_ReturnsExpectedModifiedUtc() {
                // arrange
                var repo = MakeRepo();
                var expected = new DateTime(2017, 6, 16, 15, 28, 0, DateTimeKind.Utc);
                UserSelectedContext customerInfo = new UserSelectedContext() {
                                                                                 BranchId = "FDF",
                                                                                 CustomerId = "123456"
                                                                             };

                // act
                var result = repo.GetListHeaderForCustomer(customerInfo);

                // assert
                result.ModifiedUtc
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodCustomer_ReturnsExpectedModifiedUtcKind() {
                // arrange
                var repo = MakeRepo();
                var expected = DateTimeKind.Utc;
                UserSelectedContext customerInfo = new UserSelectedContext() {
                                                                                 BranchId = "FDF",
                                                                                 CustomerId = "123456"
                                                                             };

                // act
                var result = repo.GetListHeaderForCustomer(customerInfo);

                // assert
                result.ModifiedUtc
                      .Kind
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void BadCustomer_ReturnsNull() {
                // arrange
                var repo = MakeRepo();
                var customerInfo = new UserSelectedContext() {
                    BranchId = "FDF",
                    CustomerId = "999999"
                };

                // act
                var result = repo.GetListHeaderForCustomer(customerInfo);

                // assert
                result.Should()
                      .BeNull();
            }

            [Fact]
            public void BadBranch_ReturnsNull() {
                // arrange
                var repo = MakeRepo();
                var customerInfo = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };

                // act
                var result = repo.GetListHeaderForCustomer(customerInfo);

                // assert
                result.Should()
                      .BeNull();
            }
        }
    }
}
