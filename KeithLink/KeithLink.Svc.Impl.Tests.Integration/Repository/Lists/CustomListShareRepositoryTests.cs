using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autofac;
using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists.CustomListShares;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.SmartResolver;

namespace KeithLink.Svc.Impl.Tests.Integration.Repository.Lists {
    public class CustomListShareRepositoryTests {
        private static ICustomListSharesRepository MakeRepo() {
            IContainer diMap = DependencyMapFactory.GetTestsContainer()
                                                   .Build();
            return diMap.Resolve<ICustomListSharesRepository>();
        }

        public class DeleteCustomListShares : MigratedDatabaseTest {
            [Fact]
            public void GoodId_MarksTheItemAsInactive() {
                // arrange
                var expected = false;
                var fakeShare = new CustomListShare() {
                    Active = true,
                    BranchId = "FDF",
                    CustomerNumber = "999999",
                    HeaderId = 1
                };
                var repo = MakeRepo();
                
                // act
                var id = repo.SaveCustomListShare(fakeShare);
                repo.DeleteCustomListShares(id);
                var test = repo.GetCustomListShare(id);

                // assert
                test.Active
                    .Should()
                    .Be(expected);
            }
        }

        public class GetCustomListShare : MigratedDatabaseTest {
            [Fact]
            public void GoodId_ReturnExpectedActive() {
                // arrange
                var expected = true;
                var id = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListShare(id);

                // assert
                results.Active
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodId_ReturnExpectedBranchId() {
                // arrange
                var expected = "FDF";
                var id = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListShare(id);

                // assert
                results.BranchId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodId_ReturnExpectedCreateUtc() {
                // arrange
                var expected = new DateTime(2017, 6, 29, 9, 17, 0, DateTimeKind.Utc);
                var id = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListShare(id);

                // assert
                results.CreatedUtc
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodId_ReturnExpectedCustomerNumber() {
                // arrange
                var expected = "600123";
                var id = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListShare(id);

                // assert
                results.CustomerNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodId_ReturnExpectedHeaderId() {
                // arrange
                var expected = 1;
                var id = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListShare(id);

                // assert
                results.HeaderId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodId_ReturnExpectedModifiedUtc() {
                // arrange
                var expected = new DateTime(2017, 6, 29, 9, 18, 0, DateTimeKind.Utc);
                var id = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListShare(id);

                // assert
                results.ModifiedUtc
                       .Should()
                       .Be(expected);
            }
        }

        public class GetCustomListShares_ByCustomer : MigratedDatabaseTest {
            [Fact]
            public void GoodCustomer_ReturnsExpectedCountOfActiveShares() {
                // arrange
                var fakeCustomer = new UserSelectedContext() {
                    BranchId = "FDF",
                    CustomerId = "600123"
                };
                var expected = 2;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListSharesByCustomer(fakeCustomer);

                // assert
                results.Count(s => s.Active)
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodCustomer_ReturnsExpectedCountOfInactiveShares() {
                // arrange
                var fakeCustomer = new UserSelectedContext() {
                    BranchId = "FDF",
                    CustomerId = "600123"
                };
                var expected = 0;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListSharesByCustomer(fakeCustomer);

                // assert
                results.Count(s => s.Active == false)
                       .Should()
                       .Be(expected);
            }
        }

        public class GetCustomListShares_ByHeaderId : MigratedDatabaseTest {
            [Fact]
            public void GoodCustomer_ReturnsExpectedCountOfActiveShares() {
                // arrange
                var expected = 2;
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListSharesByHeaderId(headerId);

                // assert
                results.Count(s => s.Active)
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodCustomer_ReturnsExpectedCountOfInactiveShares() {
                // arrange
                var expected = 0;
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListSharesByHeaderId(headerId);

                // assert
                results.Count(s => s.Active == false)
                       .Should()
                       .Be(expected);
            }
        }

        public class SaveCustomListShare : MigratedDatabaseTest {
            private static CustomListShare MakeShare() {
                return new CustomListShare() {
                    Active = true,
                    BranchId = "FRT",
                    CustomerNumber = "903752",
                    HeaderId = 1001,
                    CreatedUtc = new DateTime(2017, 6, 29, 13, 45, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 6, 29, 13, 47, 0, DateTimeKind.Utc)
                };
            }

            [Fact]
            public void GoodShare_SavesCorrectActive() {
                // arrange
                var expected = true;
                var repo = MakeRepo();
                var share = MakeShare();

                // act
                var shareId = repo.SaveCustomListShare(share);
                var results = repo.GetCustomListShare(shareId);

                // assert
                results.Active
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodShare_SavesCorrectBranchId() {
                // arrange
                var expected = "FRT";
                var repo = MakeRepo();
                var share = MakeShare();

                // act
                var shareId = repo.SaveCustomListShare(share);
                var results = repo.GetCustomListShare(shareId);

                // assert
                results.BranchId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodShare_SavesCorrectCustomerNumber() {
                // arrange
                var expected = "903752";
                var repo = MakeRepo();
                var share = MakeShare();

                // act
                var shareId = repo.SaveCustomListShare(share);
                var results = repo.GetCustomListShare(shareId);

                // assert
                results.CustomerNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodShare_SavesCorrectHeaderId() {
                // arrange
                var expected = 1001;
                var repo = MakeRepo();
                var share = MakeShare();

                // act
                var shareId = repo.SaveCustomListShare(share);
                var results = repo.GetCustomListShare(shareId);

                // assert
                results.HeaderId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodShare_DoesNotSaveSetCreatedUtc() {
                // arrange
                var expected = new DateTime(2017, 6, 29, 13, 45, 0, DateTimeKind.Utc);
                var repo = MakeRepo();
                var share = MakeShare();

                // act
                var shareId = repo.SaveCustomListShare(share);
                var results = repo.GetCustomListShare(shareId);

                // assert
                results.CreatedUtc
                       .Should()
                       .NotBe(expected);
            }

            [Fact]
            public void GoodShare_DoesNotSaveSetModifiedUtc() {
                // arrange
                var expected = new DateTime(2017, 6, 29, 13, 47, 0, DateTimeKind.Utc);
                var repo = MakeRepo();
                var share = MakeShare();

                // act
                var shareId = repo.SaveCustomListShare(share);
                var results = repo.GetCustomListShare(shareId);

                // assert
                results.ModifiedUtc
                       .Should()
                       .NotBe(expected);
            }

            [Fact]
            public void NullBranchId_ThrowsSqlException() {
                // arrange
                var repo = MakeRepo();
                var fakeShare = new CustomListShare() {
                    Active = true,
                    CustomerNumber = "903752",
                    HeaderId = 1001,
                    CreatedUtc = new DateTime(2017, 6, 29, 13, 45, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 6, 29, 13, 47, 0, DateTimeKind.Utc)
                };

                // act
                Action act = () => { repo.SaveCustomListShare(fakeShare); };

                // assert
                act.ShouldThrow<SqlException>();
            }

            [Fact]
            public void NullCustomerNumber_ThrowsSqlException() {
                // arrange
                var repo = MakeRepo();
                var fakeShare = new CustomListShare() {
                    Active = true,
                    BranchId = "FRT",
                    HeaderId = 1001,
                    CreatedUtc = new DateTime(2017, 6, 29, 13, 45, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 6, 29, 13, 47, 0, DateTimeKind.Utc)
                };

                // act
                Action act = () => { repo.SaveCustomListShare(fakeShare); };

                // assert
                act.ShouldThrow<SqlException>();
            }
        }
    }
}
