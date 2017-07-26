using System;

using Autofac;
using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists.RecommendedItems;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.SmartResolver;

namespace KeithLink.Svc.Impl.Tests.Integration.Repository.Lists {
    public class RecommendedListHeaderRepositoryTests {
        private static IRecommendedItemsListHeadersRepository MakeRepo() {
            IContainer diMap = DependencyMapFactory.GetTestsContainer()
                                                   .Build();
            return diMap.Resolve<IRecommendedItemsListHeadersRepository>();
        }

        public class GetRecommendedItemsHeader : MigratedDatabaseTest {
            [Fact]
            public void GoodCustomer_ReturnsExpectedHeader() {
                // arrange
                var expected = 1;
                var fakeCustomer = new UserSelectedContext() {
                    BranchId = "FDF",
                    CustomerId = "123456"
                };
                var repo = MakeRepo();

                // act
                var result = repo.GetRecommendedItemsHeaderByCustomerNumberBranch(fakeCustomer);

                // assert
                result.Id
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodCustomer_ReturnsExpectedBranchId() {
                // arrange
                var expected = "FDF";
                var fakeCustomer = new UserSelectedContext() {
                    BranchId = "FDF",
                    CustomerId = "123456"
                };
                var repo = MakeRepo();

                // act
                var result = repo.GetRecommendedItemsHeaderByCustomerNumberBranch(fakeCustomer);

                // assert
                result.BranchId
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodCustomer_ReturnsExpectedCustomerNumber() {
                // arrange
                var expected = "123456";
                var fakeCustomer = new UserSelectedContext() {
                    BranchId = "FDF",
                    CustomerId = "123456"
                };
                var repo = MakeRepo();

                // act
                var result = repo.GetRecommendedItemsHeaderByCustomerNumberBranch(fakeCustomer);

                // assert
                result.CustomerNumber
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodCustomer_ReturnsExpectedCreatedUtc() {
                // arrange
                var expected = new DateTime(2017, 7, 3, 16, 8, 0, DateTimeKind.Utc);
                var fakeCustomer = new UserSelectedContext() {
                    BranchId = "FDF",
                    CustomerId = "123456"
                };
                var repo = MakeRepo();

                // act
                var result = repo.GetRecommendedItemsHeaderByCustomerNumberBranch(fakeCustomer);

                // assert
                result.CreatedUtc
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodCustomer_ReturnsExpectedModifiedUtc() {
                // arrange
                var expected = new DateTime(2017, 7, 3, 16, 9, 0, DateTimeKind.Utc);
                var fakeCustomer = new UserSelectedContext() {
                    BranchId = "FDF",
                    CustomerId = "123456"
                };
                var repo = MakeRepo();

                // act
                var result = repo.GetRecommendedItemsHeaderByCustomerNumberBranch(fakeCustomer);

                // assert
                result.ModifiedUtc
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void BadBranch_ReturnsNull() {
                // arrange
                var fakeCustomer = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var repo = MakeRepo();

                // act
                var result = repo.GetRecommendedItemsHeaderByCustomerNumberBranch(fakeCustomer);

                // assert
                result.Should()
                      .BeNull();
            }

            [Fact]
            public void BadCustomer_ReturnsNull() {
                // arrange
                var fakeCustomer = new UserSelectedContext() {
                    BranchId = "FDF",
                    CustomerId = "999999"
                };
                var repo = MakeRepo();

                // act
                var result = repo.GetRecommendedItemsHeaderByCustomerNumberBranch(fakeCustomer);

                // assert
                result.Should()
                      .BeNull();
            }

            [Fact]
            public void NullCustomer_ThrowNullReferenceException() {
                // arrange
                UserSelectedContext fakeCustomer = null;
                var repo = MakeRepo();

                // act
                Action act = () => { repo.GetRecommendedItemsHeaderByCustomerNumberBranch(fakeCustomer); };

                // assert
                act.ShouldThrow<NullReferenceException>();
            }
        }

        public class SaveRecommendedListHeader : MigratedDatabaseTest {
            private static RecommendedItemsListHeader MakeHeader() {
                return new RecommendedItemsListHeader() {
                    BranchId = "FRT",
                    CustomerNumber = "123456",
                    CreatedUtc = new DateTime(2017, 7, 3, 16, 13, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 7, 3, 16, 14, 0, DateTimeKind.Utc)
                };
            }

            [Fact]
            public void GoodHeader_SavesCorrectBranchId() {
                // arrange
                var expected = "FRT";
                var header = MakeHeader();
                var repo = MakeRepo();
                var customer = new UserSelectedContext() {
                    BranchId = "FRT",
                    CustomerId = "123456"
                };

                // act
                repo.SaveRecommendedItemsHeader(header);
                var results = repo.GetRecommendedItemsHeaderByCustomerNumberBranch(customer);

                // assert
                results.BranchId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_SavesCorrectCustomerNumber() {
                // arrange
                var expected = "123456";
                var header = MakeHeader();
                var repo = MakeRepo();
                var customer = new UserSelectedContext() {
                    BranchId = "FRT",
                    CustomerId = "123456"
                };

                // act
                repo.SaveRecommendedItemsHeader(header);
                var results = repo.GetRecommendedItemsHeaderByCustomerNumberBranch(customer);

                // assert
                results.CustomerNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_DoesNotSaveSetCreatedUtc() {
                // arrange
                var expected = new DateTime(2017, 7, 3, 16, 13, 0, DateTimeKind.Utc);
                var header = MakeHeader();
                var repo = MakeRepo();
                var customer = new UserSelectedContext() {
                    BranchId = "FRT",
                    CustomerId = "123456"
                };

                // act
                repo.SaveRecommendedItemsHeader(header);
                var results = repo.GetRecommendedItemsHeaderByCustomerNumberBranch(customer);

                // assert
                results.CreatedUtc
                       .Should()
                       .NotBe(expected);
            }

            [Fact]
            public void GoodHeader_DoesNotSaveSetModifiedUtc() {
                // arrange
                var expected = new DateTime(2017, 7, 3, 16, 14, 0, DateTimeKind.Utc);
                var header = MakeHeader();
                var repo = MakeRepo();
                var customer = new UserSelectedContext() {
                    BranchId = "FRT",
                    CustomerId = "123456"
                };

                // act
                repo.SaveRecommendedItemsHeader(header);
                var results = repo.GetRecommendedItemsHeaderByCustomerNumberBranch(customer);

                // assert
                results.ModifiedUtc
                       .Should()
                       .NotBe(expected);
            }
        }
    }
}
