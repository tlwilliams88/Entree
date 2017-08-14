using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using Autofac;
using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists.Favorites;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.SmartResolver;

namespace KeithLink.Svc.Impl.Tests.Integration.Repository.Lists {
    public class FavoriteListHeadersRepositoryTests {
        private static IFavoriteListHeadersRepository MakeRepo() {
            IContainer diMap = DependencyMapFactory.GetTestsContainer()
                                                   .Build();
            return diMap.Resolve<IFavoriteListHeadersRepository>();
        }

        public class GetFavoritesList : MigratedDatabaseTest {
            [Fact]
            public void GoodUserAndCustomer_ReturnsExpectedHeader() {
                // arrange
                var expected = 1;
                var fakeUser = new Guid("e4ef9796-153d-423a-96e2-d85753d2e9bd");
                var fakeCustomer = new UserSelectedContext() {
                    BranchId = "FIT",
                    CustomerId = "503321"
                };
                var repo = MakeRepo();

                // act
                var result = repo.GetFavoritesList(fakeUser, fakeCustomer);

                // assert
                result.Id
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void BadUser_ReturnsNull() {
                // arrange
                var fakeUser = Guid.Empty;
                var fakeCustomer = new UserSelectedContext() {
                    BranchId = "FIT",
                    CustomerId = "503321"
                };
                var repo = MakeRepo();

                // act
                var result = repo.GetFavoritesList(fakeUser, fakeCustomer);

                // assert
                result.Should()
                      .BeNull();
            }

            [Fact]
            public void BadBranch_ReturnsNull() {
                // arrange
                var fakeUser = new Guid("e4ef9796-153d-423a-96e2-d85753d2e9bd");
                var fakeCustomer = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "503321"
                };
                var repo = MakeRepo();

                // act
                var result = repo.GetFavoritesList(fakeUser, fakeCustomer);

                // assert
                result.Should()
                      .BeNull();
            }

            [Fact]
            public void BadCustomer_ReturnsNull() {
                // arrange
                var fakeUser = new Guid("e4ef9796-153d-423a-96e2-d85753d2e9bd");
                var fakeCustomer = new UserSelectedContext() {
                    BranchId = "FIR",
                    CustomerId = "999999"
                };
                var repo = MakeRepo();

                // act
                var result = repo.GetFavoritesList(fakeUser, fakeCustomer);

                // assert
                result.Should()
                      .BeNull();
            }

            [Fact]
            public void NullCustomer_ThrowNullReferenceException() {
                // arrange
                var fakeUser = new Guid("e4ef9796-153d-423a-96e2-d85753d2e9bd");
                UserSelectedContext fakeCustomer = null;
                var repo = MakeRepo();

                // act
                Action act = () => { repo.GetFavoritesList(fakeUser, fakeCustomer); };

                // assert
                act.ShouldThrow<NullReferenceException>();
            }
        }

        public class SaveFavoriteListHeader : MigratedDatabaseTest {
            private static FavoritesListHeader MakeHeader() {
                return new FavoritesListHeader() {
                    BranchId = "FRT",
                    CustomerNumber = "123456",
                    UserId = new Guid("bbe14aea-6c68-43a3-ae13-c57a641b5fc6"),
                    CreatedUtc = new DateTime(2017, 6, 29, 15, 49, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 6, 29, 13, 50, 0, DateTimeKind.Utc)
                };
            }

            [Fact]
            public void GoodHeader_SavesCorrectBranchId() {
                // arrange
                var expected = "FRT";
                var header = MakeHeader();
                var repo = MakeRepo();
                var user = new Guid("bbe14aea-6c68-43a3-ae13-c57a641b5fc6");
                var customer = new UserSelectedContext(){
                    BranchId = "FRT",
                    CustomerId = "123456"
                };

                // act
                repo.SaveFavoriteListHeader(header);
                var results = repo.GetFavoritesList(user, customer);

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
                var user = new Guid("bbe14aea-6c68-43a3-ae13-c57a641b5fc6");
                var customer = new UserSelectedContext() {
                    BranchId = "FRT",
                    CustomerId = "123456"
                };

                // act
                repo.SaveFavoriteListHeader(header);
                var results = repo.GetFavoritesList(user, customer);

                // assert
                results.CustomerNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_SavesCorrectUserId() {
                // arrange
                var expected = new Guid("bbe14aea-6c68-43a3-ae13-c57a641b5fc6");
                var header = MakeHeader();
                var repo = MakeRepo();
                var user = new Guid("bbe14aea-6c68-43a3-ae13-c57a641b5fc6");
                var customer = new UserSelectedContext() {
                    BranchId = "FRT",
                    CustomerId = "123456"
                };

                // act
                repo.SaveFavoriteListHeader(header);
                var results = repo.GetFavoritesList(user, customer);

                // assert
                results.UserId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_DoesNotSaveSetCreatedUtc() {
                // arrange
                var expected = new DateTime(2017, 6, 29, 15, 49, 0, DateTimeKind.Utc);
                var header = MakeHeader();
                var repo = MakeRepo();
                var user = new Guid("bbe14aea-6c68-43a3-ae13-c57a641b5fc6");
                var customer = new UserSelectedContext() {
                    BranchId = "FRT",
                    CustomerId = "123456"
                };

                // act
                repo.SaveFavoriteListHeader(header);
                var results = repo.GetFavoritesList(user, customer);

                // assert
                results.CreatedUtc
                       .Should()
                       .NotBe(expected);
            }

            [Fact]
            public void GoodHeader_DoesNotSaveSetModifiedUtc() {
                // arrange
                var expected = new DateTime(2017, 6, 29, 13, 50, 0, DateTimeKind.Utc);
                var header = MakeHeader();
                var repo = MakeRepo();
                var user = new Guid("bbe14aea-6c68-43a3-ae13-c57a641b5fc6");
                var customer = new UserSelectedContext() {
                    BranchId = "FRT",
                    CustomerId = "123456"
                };

                // act
                repo.SaveFavoriteListHeader(header);
                var results = repo.GetFavoritesList(user, customer);

                // assert
                results.ModifiedUtc
                       .Should()
                       .NotBe(expected);
            }
        }
    }
}
