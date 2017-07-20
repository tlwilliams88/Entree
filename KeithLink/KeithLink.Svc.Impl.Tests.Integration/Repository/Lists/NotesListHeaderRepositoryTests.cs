using System;
using System.Data.SqlClient;

using Autofac;

using FluentAssertions;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists.Notes;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.SmartResolver;

using Xunit;

namespace KeithLink.Svc.Impl.Tests.Integration.Repository.Lists {
    public class NotesListHeaderRepositoryTests {
        private static INotesHeadersListRepository MakeRepo() {
            IContainer diMap = DependencyMapFactory.GetTestsContainer()
                                                   .Build();
            return diMap.Resolve<INotesHeadersListRepository>();
        }

        public class GetNotesListHeader : MigratedDatabaseTest {
            [Fact]
            public void BadContext_BadBranchIdReturnsNull() {
                // arrange
                UserSelectedContext context = new UserSelectedContext() {
                    CustomerId = "654321",
                    BranchId = "XXX"
                };
                INotesHeadersListRepository repo = MakeRepo();

                // act
                NotesListHeader results = repo.GetHeadersForCustomer(context);

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
                INotesHeadersListRepository repo = MakeRepo();

                // act
                var results = repo.GetHeadersForCustomer(context);

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
                INotesHeadersListRepository repo = MakeRepo();

                string expected = "FRT";

                // act
                NotesListHeader results = repo.GetHeadersForCustomer(context);

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

                INotesHeadersListRepository repo = MakeRepo();

                // act
                NotesListHeader results = repo.GetHeadersForCustomer(context);

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

                INotesHeadersListRepository repo = MakeRepo();

                // act
                NotesListHeader results = repo.GetHeadersForCustomer(context);

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

                INotesHeadersListRepository repo = MakeRepo();

                // act
                NotesListHeader results = repo.GetHeadersForCustomer(context);

                // assert
                results
                        .ModifiedUtc
                        .Should()
                        .Be(expected);
            }

        }

        public class SaveInventoryValudationListHeader : MigratedDatabaseTest {
            private NotesListHeader MakeHeader() {
                return new NotesListHeader {
                    BranchId = "FRT",
                    CustomerNumber = "200321",
                    CreatedUtc = new DateTime(2017, 6, 30, 16, 32, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 6, 30, 16, 33, 0, DateTimeKind.Utc)
                };
            }

            [Fact]
            public void GoodHeader_ReturnsGoodHeaderId() {
                // arrange
                int expected = 4;

                NotesListHeader header = MakeHeader();
                INotesHeadersListRepository repo = MakeRepo();

                // act
                long headerId = repo.Save(header);

                // assert
                headerId.Should()
                        .Be(expected);
            }

            [Fact]
            public void GoodHeader_DoesNotUseSetCreatedUtc() {
                // arrange
                UserSelectedContext context = new UserSelectedContext() {
                    CustomerId = "200321",
                    BranchId = "FRT"
                };

                DateTime expected = new DateTime(2017, 6, 30, 16, 32, 0, DateTimeKind.Utc);

                NotesListHeader header = MakeHeader();
                INotesHeadersListRepository repo = MakeRepo();

                // act
                var headerId = repo.Save(header);
                var results = repo.GetHeadersForCustomer(context);

                // assert
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

                NotesListHeader header = MakeHeader();
                INotesHeadersListRepository repo = MakeRepo();

                // act
                var headerId = repo.Save(header);
                var results = repo.GetHeadersForCustomer(context);

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

                NotesListHeader header = MakeHeader();
                INotesHeadersListRepository repo = MakeRepo();

                // act
                var headerId = repo.Save(header);
                var results = repo.GetHeadersForCustomer(context);

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

                NotesListHeader header = MakeHeader();
                INotesHeadersListRepository repo = MakeRepo();

                // act
                var headerId = repo.Save(header);
                var results = repo.GetHeadersForCustomer(context);

                // assert
                results.CustomerNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void NullBranchId_ThrowsSqlException() {
                // arrange
                NotesListHeader header = new NotesListHeader {
                    CustomerNumber = "200321",
                    CreatedUtc = new DateTime(2017, 6, 30, 16, 32, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 6, 30, 16, 33, 0, DateTimeKind.Utc)
                };
                INotesHeadersListRepository repo = MakeRepo();

                // act
                Action act = () => { repo.Save(header); };

                // assert
                act.ShouldThrow<SqlException>();
            }

            [Fact]
            public void NullCustomerNumber_ThrowsSqlException() {
                // arrange
                NotesListHeader header = new NotesListHeader {
                    BranchId = "FRT",
                    CreatedUtc = new DateTime(2017, 6, 30, 16, 32, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 6, 30, 16, 33, 0, DateTimeKind.Utc)
                };
                INotesHeadersListRepository repo = MakeRepo();

                // act
                Action act = () => { repo.Save(header); };

                // assert
                act.ShouldThrow<SqlException>();
            }
        }
    }
}