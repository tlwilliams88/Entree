using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autofac;
using FluentAssertions;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists.InventoryValuationList;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.SmartResolver;
using Xunit;

namespace KeithLink.Svc.Impl.Tests.Integration.Repository.Lists {
    public class InventoryValuationListHeaderRepositoryTests {
        private static IInventoryValuationListHeadersRepository MakeRepo() {
            IContainer diMap = DependencyMapFactory.GetTestsContainer()
                                                   .Build();
            return diMap.Resolve<IInventoryValuationListHeadersRepository>();
        }

        public class GetInventoryValuationListHeader : MigratedDatabaseTest {
            [Fact]
            public void BadHeaderId_ReturnsNull() {
                // arrange
                var headerId = 17;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationListHeader(headerId);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void GoodHeaderId_ReturnsExpectedBranchId() {
                // arrange
                var expected = "FRT";
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationListHeader(headerId);

                // assert
                results
                    .BranchId
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodHeaderId_ReturnsExpectedCustomerNumber() {
                // arrange
                var expected = "123456";
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationListHeader(headerId);

                // assert
                results
                    .CustomerNumber
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodHeaderId_ReturnsExpectedName() {
                // arrange
                var expected = "Fake Name 1";
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationListHeader(headerId);

                // assert
                results
                    .Name
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodHeaderId_ReturnsExpectedCreatedUtc() {
                // arrange
                var expected = new DateTime(2017, 6, 30, 16, 11, 0, DateTimeKind.Utc);
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationListHeader(headerId);

                // assert
                results
                    .CreatedUtc
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodHeaderId_ReturnsExpectedModifiedUtc() {
                // arrange
                var expected = new DateTime(2017, 6, 30, 16, 12, 0, DateTimeKind.Utc);
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationListHeader(headerId);

                // assert
                results
                    .ModifiedUtc
                    .Should()
                    .Be(expected);
            }
        }

        public class GetInventoryValuationListHeaders : MigratedDatabaseTest {
            [Fact]
            public void GoodCustomer_ReturnsExpectedCount() {
                // arrange
                var customerInfo = new UserSelectedContext() {
                    BranchId = "FRT",
                    CustomerId = "123456"
                };
                var expected = 2;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationListHeaders(customerInfo);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void BadBranch_ReturnsNoResults() {
                // arrange
                var customerInfo = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var expected = 0;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationListHeaders(customerInfo);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void BadCustomerNumber_ReturnsNoResults() {
                // arrange
                var customerInfo = new UserSelectedContext() {
                                                                 BranchId = "FRT",
                                                                 CustomerId = "999999"
                                                             };
                var expected = 0;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationListHeaders(customerInfo);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }
        }

        public class SaveInventoryValudationListHeader : MigratedDatabaseTest { 
            private InventoryValuationListHeader MakeHeader() {
                return new InventoryValuationListHeader() { 
                    BranchId = "FRT",
                    CustomerNumber = "200321",
                    Name = "Fake Name",
                    CreatedUtc = new DateTime(2017, 6, 30, 16, 32, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 6, 30, 16, 33, 0, DateTimeKind.Utc)
                };
            }

            [Fact]
            public void GoodHeader_SavesCorrectBranchId() {
                // arrange
                var expected = "FRT";
                var header = MakeHeader();
                var repo = MakeRepo();

                // act
                var headerId = repo.SaveInventoryValudationListHeader(header);
                var results = repo.GetInventoryValuationListHeader(headerId);

                // assert
                results.BranchId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_SavesCorrectCustomerNumber() {
                // arrange
                var expected = "200321";
                var header = MakeHeader();
                var repo = MakeRepo();

                // act
                var headerId = repo.SaveInventoryValudationListHeader(header);
                var results = repo.GetInventoryValuationListHeader(headerId);

                // assert
                results.CustomerNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_SavesCorrectName() {
                // arrange
                var expected = "Fake Name";
                var header = MakeHeader();
                var repo = MakeRepo();

                // act
                var headerId = repo.SaveInventoryValudationListHeader(header);
                var results = repo.GetInventoryValuationListHeader(headerId);

                // assert
                results.Name
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_DoesNotUseSetCreatedUtc() {
                // arrange
                var expected = new DateTime(2017, 6, 30, 16, 32, 0, DateTimeKind.Utc);
                var header = MakeHeader();
                var repo = MakeRepo();

                // act
                var headerId = repo.SaveInventoryValudationListHeader(header);
                var results = repo.GetInventoryValuationListHeader(headerId);

                // assert
                results.CreatedUtc
                       .Should()
                       .NotBe(expected);
            }

            [Fact]
            public void GoodHeader_DoesNotUseSetModifiedUtc() {
                // arrange
                var expected = new DateTime(2017, 6, 30, 16, 33, 0, DateTimeKind.Utc);
                var header = MakeHeader();
                var repo = MakeRepo();

                // act
                var headerId = repo.SaveInventoryValudationListHeader(header);
                var results = repo.GetInventoryValuationListHeader(headerId);

                // assert
                results.ModifiedUtc
                       .Should()
                       .NotBe(expected);
            }

            [Fact]
            public void NullBranchId_ThrowsSqlException() {
                // arrange
                var header = new InventoryValuationListHeader() {
                    CustomerNumber = "200321",
                    Name = "Fake Name",
                    CreatedUtc = new DateTime(2017, 6, 30, 16, 32, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 6, 30, 16, 33, 0, DateTimeKind.Utc)
                };
                var repo = MakeRepo();

                // act
                Action act = () => { repo.SaveInventoryValudationListHeader(header); };

                // assert
                act.ShouldThrow<SqlException>();
            }

            [Fact]
            public void NullCustomerNumber_ThrowsSqlException() {
                // arrange
                var header = new InventoryValuationListHeader() {
                    BranchId = "FRT",
                    Name = "Fake Name",
                    CreatedUtc = new DateTime(2017, 6, 30, 16, 32, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 6, 30, 16, 33, 0, DateTimeKind.Utc)
                };
                var repo = MakeRepo();

                // act
                Action act = () => { repo.SaveInventoryValudationListHeader(header); };

                // assert
                act.ShouldThrow<SqlException>();
            }

            [Fact]
            public void NullName_ThrowsSqlException() {
                // arrange
                var header = new InventoryValuationListHeader() {
                    BranchId = "FRT",
                    CustomerNumber = "200321",
                    CreatedUtc = new DateTime(2017, 6, 30, 16, 32, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 6, 30, 16, 33, 0, DateTimeKind.Utc)
                };
                var repo = MakeRepo();

                // act
                Action act = () => { repo.SaveInventoryValudationListHeader(header); };

                // assert
                act.ShouldThrow<SqlException>();
            }
        }
    }
}
