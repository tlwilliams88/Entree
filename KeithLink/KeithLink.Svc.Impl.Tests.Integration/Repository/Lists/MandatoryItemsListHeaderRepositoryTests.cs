using System;
using System.Data.SqlClient;

using Autofac;

using FluentAssertions;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists.MandatoryItem;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.SmartResolver;

using Xunit;

namespace KeithLink.Svc.Impl.Tests.Integration.Repository.Lists {
    public class MandatoryItemsListHeaderRepositoryTests {
        private static IMandatoryItemsListHeadersRepository MakeRepo() {
            IContainer diMap = DependencyMapFactory.GetTestsContainer()
                                                   .Build();
            return diMap.Resolve<IMandatoryItemsListHeadersRepository>();
        }

        public class GetMandatoryItemsListHeader : MigratedDatabaseTest {
            [Fact]
            public void BadContext_BadBranchIdReturnsNull() {
                // arrange
                UserSelectedContext context = new UserSelectedContext() {
                    CustomerId = "654321",
                    BranchId = "XXX"
                };
                IMandatoryItemsListHeadersRepository repo = MakeRepo();

                // act
                MandatoryItemsListHeader results = repo.GetListHeaderForCustomer(context);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void BadContext_BadCustomerNumberReturnsNull() {
                // arrange
                UserSelectedContext context = new UserSelectedContext() {
                    CustomerId = "654321",
                    BranchId = "FRT"
                };
                UserSelectedContext expected = null;
                IMandatoryItemsListHeadersRepository repo = MakeRepo();

                // act
                var results = repo.GetListHeaderForCustomer(context);

                // assert
                results
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodContext_ReturnsExpectedBranchId() {
                // arrange
                UserSelectedContext context = new UserSelectedContext() {
                    CustomerId = "123456",
                    BranchId = "FRT"
                };
                IMandatoryItemsListHeadersRepository repo = MakeRepo();

                string expected = "FRT";

                // act
                MandatoryItemsListHeader results = repo.GetListHeaderForCustomer(context);

                // assert
                results
                        .BranchId
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void GoodContext_ReturnsExpectedCreatedUtc() {
                // arrange
                DateTime expected = new DateTime(2017, 6, 30, 16, 11, 0, DateTimeKind.Utc);

                UserSelectedContext context = new UserSelectedContext() {
                    CustomerId = "123456",
                    BranchId = "FRT"
                };

                IMandatoryItemsListHeadersRepository repo = MakeRepo();

                // act
                MandatoryItemsListHeader results = repo.GetListHeaderForCustomer(context);

                // assert
                results
                        .CreatedUtc
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void GoodContext_ReturnsExpectedCustomerNumber() {
                // arrange
                string expected = "123456";

                UserSelectedContext context = new UserSelectedContext() {
                    CustomerId = "123456",
                    BranchId = "FRT"
                };

                IMandatoryItemsListHeadersRepository repo = MakeRepo();

                // act
                MandatoryItemsListHeader results = repo.GetListHeaderForCustomer(context);

                // assert
                results
                        .CustomerNumber
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void GoodContext_ReturnsExpectedModifiedUtc() {
                // arrange
                DateTime expected = new DateTime(2017, 6, 30, 16, 12, 0, DateTimeKind.Utc);

                UserSelectedContext context = new UserSelectedContext() {
                    CustomerId = "123456",
                    BranchId = "FRT"
                };

                IMandatoryItemsListHeadersRepository repo = MakeRepo();

                // act
                MandatoryItemsListHeader results = repo.GetListHeaderForCustomer(context);

                // assert
                results
                        .ModifiedUtc
                        .Should()
                        .Be(expected);
            }
        }

        public class SaveInventoryValudationListHeader : MigratedDatabaseTest {
            private MandatoryItemsListHeader MakeHeader() {
                return new MandatoryItemsListHeader {
                    BranchId = "FRT",
                    CustomerNumber = "200321",
                    CreatedUtc = new DateTime(2017, 6, 30, 16, 32, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 6, 30, 16, 33, 0, DateTimeKind.Utc)
                };
            }

            [Fact]
            public void GoodHeader_DoesNotUseSetCreatedUtc() {
                // arrange
                UserSelectedContext context = new UserSelectedContext() {
                    CustomerId = "200321",
                    BranchId = "FRT"
                };

                DateTime expected = new DateTime(2017, 6, 30, 16, 32, 0, DateTimeKind.Utc);

                MandatoryItemsListHeader header = MakeHeader();
                IMandatoryItemsListHeadersRepository repo = MakeRepo();

                // act
                var headerId = repo.SaveMandatoryItemsHeader(header);
                var results = repo.GetListHeaderForCustomer(context);

                // assert
                headerId.Should()
                        .NotBe(0);

                results.CreatedUtc
                       .Should()
                       .NotBe(expected);
            }

            [Fact]
            public void GoodHeader_DoesNotUseSetModifiedUtc() {
                // arrange
                DateTime expected = new DateTime(2017, 6, 30, 16, 33, 0, DateTimeKind.Utc);

                UserSelectedContext context = new UserSelectedContext() {
                    CustomerId = "200321",
                    BranchId = "FRT"
                };

                MandatoryItemsListHeader header = MakeHeader();
                IMandatoryItemsListHeadersRepository repo = MakeRepo();

                // act
                var headerId = repo.SaveMandatoryItemsHeader(header);
                var results = repo.GetListHeaderForCustomer(context);

                // assert
                results.ModifiedUtc
                       .Should()
                       .NotBe(expected);
            }

            [Fact]
            public void GoodHeader_SavesCorrectBranchId() {
                // arrange
                string expected = "FRT";

                UserSelectedContext context = new UserSelectedContext() {
                    CustomerId = "200321",
                    BranchId = "FRT"
                };

                MandatoryItemsListHeader header = MakeHeader();
                IMandatoryItemsListHeadersRepository repo = MakeRepo();

                // act
                var headerId = repo.SaveMandatoryItemsHeader(header);
                var results = repo.GetListHeaderForCustomer(context);

                // assert
                results.BranchId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_SavesCorrectCustomerNumber() {
                // arrange
                string expected = "200321";

                UserSelectedContext context = new UserSelectedContext()
                {
                    CustomerId = "200321",
                    BranchId = "FRT"
                };

                MandatoryItemsListHeader header = MakeHeader();
                IMandatoryItemsListHeadersRepository repo = MakeRepo();

                // act
                var headerId = repo.SaveMandatoryItemsHeader(header);
                var results = repo.GetListHeaderForCustomer(context);

                // assert
                results.CustomerNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void NullBranchId_ThrowsSqlException() {
                // arrange
                MandatoryItemsListHeader header = new MandatoryItemsListHeader {
                    CustomerNumber = "200321",
                    CreatedUtc = new DateTime(2017, 6, 30, 16, 32, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 6, 30, 16, 33, 0, DateTimeKind.Utc)
                };
                IMandatoryItemsListHeadersRepository repo = MakeRepo();

                // act
                Action act = () => { repo.SaveMandatoryItemsHeader(header); };

                // assert
                act.ShouldThrow<SqlException>();
            }

            [Fact]
            public void NullCustomerNumber_ThrowsSqlException() {
                // arrange
                MandatoryItemsListHeader header = new MandatoryItemsListHeader {
                    BranchId = "FRT",
                    CreatedUtc = new DateTime(2017, 6, 30, 16, 32, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 6, 30, 16, 33, 0, DateTimeKind.Utc)
                };
                IMandatoryItemsListHeadersRepository repo = MakeRepo();

                // act
                Action act = () => { repo.SaveMandatoryItemsHeader(header); };

                // assert
                act.ShouldThrow<SqlException>();
            }
        }
    }
}