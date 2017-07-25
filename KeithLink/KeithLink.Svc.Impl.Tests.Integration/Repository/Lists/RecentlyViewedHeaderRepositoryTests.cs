using System;

using Autofac;
using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists.RecentlyViewed;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.SmartResolver;

namespace KeithLink.Svc.Impl.Tests.Integration.Repository.Lists {
    public class RecentlyViewedHeaderRepositoryTests {
        private static IRecentlyViewedListHeadersRepository MakeRepo() {
            IContainer diMap = DependencyMapFactory.GetTestsContainer()
                                                   .Build();
            return diMap.Resolve<IRecentlyViewedListHeadersRepository>();
        }

        private const string GOOD_USER_ID = "e4ef9796-153d-423a-96e2-d85753d2e9bd";
        private const string GOOD_CUSTOMER_NUMBER = "123456";

        private const string NEW_USER_ID =  "11111111-1111-2222-3333-444444444444";
        private const string NEW_CUSTOMER_NUMBER = "777777";

        public class GetRecentlyViewedItemsHeader : MigratedDatabaseTest {
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
                var result = repo.GetRecentlyViewedHeader(Guid.Parse(GOOD_USER_ID), fakeCustomer);

                // assert
                result.Id
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodCustomer_ReturnsExpectedUserId() {
                // arrange
                Guid expected = new Guid(GOOD_USER_ID);
                UserSelectedContext fakeCustomer = new UserSelectedContext() {
                    BranchId = "FDF",
                    CustomerId = "123456"
                };
                IRecentlyViewedListHeadersRepository repo = MakeRepo();

                // act
                RecentlyViewedListHeader result = repo.GetRecentlyViewedHeader(Guid.Parse(GOOD_USER_ID), fakeCustomer);

                // assert
                result.UserId
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
                var result = repo.GetRecentlyViewedHeader(Guid.Parse(GOOD_USER_ID), fakeCustomer);

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
                var result = repo.GetRecentlyViewedHeader(Guid.Parse(GOOD_USER_ID), fakeCustomer);

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
                var result = repo.GetRecentlyViewedHeader(Guid.Parse(GOOD_USER_ID), fakeCustomer);

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
                var result = repo.GetRecentlyViewedHeader(Guid.Parse(GOOD_USER_ID), fakeCustomer);

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
                var result = repo.GetRecentlyViewedHeader(Guid.Parse(GOOD_USER_ID), fakeCustomer);

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
                var result = repo.GetRecentlyViewedHeader(Guid.Parse(GOOD_USER_ID), fakeCustomer);

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
                Action act = () => { repo.GetRecentlyViewedHeader(Guid.Parse(GOOD_USER_ID), fakeCustomer); };

                // assert
                act.ShouldThrow<NullReferenceException>();
            }
        }

        public class SaveRecentlyViewedListHeader : MigratedDatabaseTest {
            private static RecentlyViewedListHeader MakeHeader() {
                return new RecentlyViewedListHeader() {
                    UserId = new Guid(NEW_USER_ID),
                    BranchId = "FRT",
                    CustomerNumber = NEW_CUSTOMER_NUMBER,
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
                    CustomerId = NEW_CUSTOMER_NUMBER
                };

                // act
                repo.Save(header);
                var results = repo.GetRecentlyViewedHeader(Guid.Parse(NEW_USER_ID), customer);

                // assert
                results.BranchId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_SavesCorrectCustomerNumber() {
                // arrange
                var expected = NEW_CUSTOMER_NUMBER;
                var header = MakeHeader();
                var repo = MakeRepo();
                var customer = new UserSelectedContext() {
                    BranchId = "FRT",
                    CustomerId = NEW_CUSTOMER_NUMBER
                };

                // act
                repo.Save(header);
                var results = repo.GetRecentlyViewedHeader(Guid.Parse(NEW_USER_ID) ,customer);

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
                    CustomerId = NEW_CUSTOMER_NUMBER
                };

                // act
                repo.Save(header);
                var results = repo.GetRecentlyViewedHeader(Guid.Parse(NEW_USER_ID),customer);

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
                    CustomerId = NEW_CUSTOMER_NUMBER
                };

                // act
                repo.Save(header);
                var results = repo.GetRecentlyViewedHeader(Guid.Parse(NEW_USER_ID) ,customer);

                // assert
                results.ModifiedUtc
                       .Should()
                       .NotBe(expected);
            }

            [Fact]
            public void GoodHeader_SavesCorrectUserId() {
                // arrange
                var expected = new Guid(NEW_USER_ID);
                var header = MakeHeader();
                var repo = MakeRepo();
                var customer = new UserSelectedContext() {
                    BranchId = "FRT",
                    CustomerId = NEW_CUSTOMER_NUMBER
                };

                // act
                repo.Save(header);
                var results = repo.GetRecentlyViewedHeader(Guid.Parse(NEW_USER_ID), customer);

                // assert
                results.UserId
                       .Should()
                       .Be(expected);
            }

        } // Save Class
    } // Class
} // Namespace
