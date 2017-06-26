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
using KeithLink.Svc.Core.Models.Lists.CustomList;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.SmartResolver;

namespace KeithLink.Svc.Impl.Tests.Integration.Repository.Lists {
    public class CustomListHeaderRepositoryTests {
        private static ICustomListHeadersRepository MakeRepo() {
            IContainer diMap = DependencyMapFactory.GetTestsContainer()
                                                   .Build();
            return diMap.Resolve<ICustomListHeadersRepository>();
        }

        public class GetCustomListHeader : MigratedDatabaseTest {
            [Fact]
            public void GoodCustomer_ReturnsExpectedCount() {
                // arrange
                var custInfo = new UserSelectedContext {
                    BranchId = "FDF",
                    CustomerId = "123456"
                };
                var expected = 3;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListHeadersByCustomer(custInfo);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodCustomer_ReturnsExpectedCountOfActiveHeaders() {
                // arrange
                var custInfo = new UserSelectedContext {
                                                           BranchId = "FDF",
                                                           CustomerId = "123456"
                                                       };
                var expected = 3;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListHeadersByCustomer(custInfo);

                // assert
                results.Count(l => l.Active)
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodCustomer_ReturnsExpectedCountOfInactiveHeaders() {
                // arrange
                var custInfo = new UserSelectedContext {
                                                           BranchId = "FDF",
                                                           CustomerId = "123456"
                                                       };
                var expected = 0;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListHeadersByCustomer(custInfo);

                // assert
                results.Count(l => l.Active == false)
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void BadBranchId_ReturnsNoResults() {
                // arrange
                var custInfo = new UserSelectedContext {
                                                           BranchId = "XXX",
                                                           CustomerId = "123456"
                                                       };
                var expected = 0;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListHeadersByCustomer(custInfo);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void BadCustomerNumber_ReturnsNoResults() {
                // arrange
                var custInfo = new UserSelectedContext {
                                                           BranchId = "FDF",
                                                           CustomerId = "999999"
                                                       };
                var expected = 0;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListHeadersByCustomer(custInfo);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }
        }

        public class GetCustomListHeadersByCustomer : MigratedDatabaseTest {
            [Fact]
            public void BadHeaderId_ReturnsNull() {
                // arrange
                var headerId = 1011;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListHeader(headerId);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void GoodHeaderId_ReturnsExpectedUserId() {
                // arrange
                var expected = new Guid("60ffa8da-737d-4dbf-bacb-fe9774b9731f");
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListHeader(headerId);
                
                // assert
                results.UserId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderId_ReturnsExpectedBranchId() {
                // arrange
                var expected = "FDF";
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListHeader(headerId);

                // assert
                results.BranchId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderId_ReturnsExpectedCustomerNumber() {
                // arrange
                var expected = "FDF";
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListHeader(headerId);

                // assert
                results.BranchId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderId_ReturnsExpectedName() {
                // arrange
                var expected = "Fake Custom List 1";
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListHeader(headerId);

                // assert
                results.Name
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderId_ReturnsExpectedActive() {
                // arrange
                var expected = true;
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListHeader(headerId);

                // assert
                results.Active
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderId_ReturnsExpectedCreatedUtc() {
                // arrange
                var expected = new DateTime(2017, 6, 23, 11, 2, 0, DateTimeKind.Utc);
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListHeader(headerId);

                // assert
                results.CreatedUtc
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderId_ReturnsExpectedCreatedUtcKind() {
                // arrange
                var expected = DateTimeKind.Utc;
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListHeader(headerId);

                // assert
                results.CreatedUtc
                       .Kind
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderId_ReturnsExpectedModifiedUtc() {
                // arrange
                var expected = new DateTime(2017, 6, 23, 11, 3, 0, DateTimeKind.Utc);
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListHeader(headerId);

                // assert
                results.ModifiedUtc
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderId_ReturnsExpectedModifiedUtcKind() {
                // arrange
                var expected = DateTimeKind.Utc;
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListHeader(headerId);

                // assert
                results.ModifiedUtc
                       .Kind
                       .Should()
                       .Be(expected);
            }
        }

        public class SaveCustomListHeader : MigratedDatabaseTest {
            private static CustomListHeader MakeGoodHeader() {
                return new CustomListHeader() {
                    UserId = new Guid("0dd15d06-7476-4ec9-9d2c-a78da51fc8bf"),
                    Active = true,
                    BranchId = "FDF",
                    CustomerNumber = "100234",
                    Name = "Fake Custom List",
                    CreatedUtc = new DateTime(2017, 6, 23, 16, 13, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 6, 23, 16, 14, 0, DateTimeKind.Utc)
                };
            }

            [Fact]
            public void GoodHeader_ReturnsCorrectHeaderId() {
                // arrange
                var header = MakeGoodHeader();
                var expected = 6;
                var repo = MakeRepo();

                // act
                var results = repo.SaveCustomListHeader(header);
                
                // assert
                results.Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_SavesCorrectUserId() {
                // arrange
                var header = MakeGoodHeader();
                var expected = new Guid("0dd15d06-7476-4ec9-9d2c-a78da51fc8bf");
                var repo = MakeRepo();

                // act
                var headerId = repo.SaveCustomListHeader(header);
                var result = repo.GetCustomListHeader(headerId);

                // assert
                result.UserId
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodHeader_SavesCorrectBranchId() {
                // arrange
                var header = MakeGoodHeader();
                var expected = "FDF";
                var repo = MakeRepo();

                // act
                var headerId = repo.SaveCustomListHeader(header);
                var result = repo.GetCustomListHeader(headerId);

                // assert
                result.BranchId
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodHeader_SavesCorrectCustomerNumber() {
                // arrange
                var header = MakeGoodHeader();
                var expected = "100234";
                var repo = MakeRepo();

                // act
                var headerId = repo.SaveCustomListHeader(header);
                var result = repo.GetCustomListHeader(headerId);

                // assert
                result.CustomerNumber
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodHeader_SavesCorrectName() {
                // arrange
                var header = MakeGoodHeader();
                var expected = "Fake Custom List";
                var repo = MakeRepo();

                // act
                var headerId = repo.SaveCustomListHeader(header);
                var result = repo.GetCustomListHeader(headerId);

                // assert
                result.Name
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodHeader_SavesCorrectActive() {
                // arrange
                var header = MakeGoodHeader();
                var expected = true;
                var repo = MakeRepo();

                // act
                var headerId = repo.SaveCustomListHeader(header);
                var result = repo.GetCustomListHeader(headerId);

                // assert
                result.Active
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodHeader_DoesNotSaveSuppliedCreatedUtc() {
                // arrange
                var header = MakeGoodHeader();
                var expected = new DateTime(2017, 6, 23, 16, 13, 0, DateTimeKind.Utc);
                var repo = MakeRepo();

                // act
                var headerId = repo.SaveCustomListHeader(header);
                var result = repo.GetCustomListHeader(headerId);

                // assert
                result.CreatedUtc
                      .Should()
                      .NotBe(expected);
            }

            [Fact]
            public void GoodHeader_DoesNotSaveSuppliedModifiedUtc() {
                // arrange
                var header = MakeGoodHeader();
                var expected = new DateTime(2017, 6, 23, 16, 14, 0, DateTimeKind.Utc);
                var repo = MakeRepo();

                // act
                var headerId = repo.SaveCustomListHeader(header);
                var result = repo.GetCustomListHeader(headerId);

                // assert
                result.ModifiedUtc
                      .Should()
                      .NotBe(expected);
            }

            [Fact]
            public void UpdatingHeader_ReturnsTheSameHeaderId() {
                // arrange
                var expected = 1;
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var originalHeader = repo.GetCustomListHeader(headerId);
                var test = repo.SaveCustomListHeader(originalHeader);

                // assert
                test.Should()
                    .Be(expected);
            }

            [Fact]
            public void UpdatingHeader_DoesNotChangeCreatedUtc() {
                // arrange
                var expected = new DateTime(2017, 6, 23, 11, 2, 0, DateTimeKind.Utc);
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var originalHeader = repo.GetCustomListHeader(headerId);
                repo.SaveCustomListHeader(originalHeader);
                var test = repo.GetCustomListHeader(headerId);

                // assert
                test.CreatedUtc
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void UpdatingHeader_ChangesModifiedUtc() {
                // arrange
                var expected = new DateTime(2017, 6, 23, 11, 3, 0, DateTimeKind.Utc);
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var originalHeader = repo.GetCustomListHeader(headerId);
                repo.SaveCustomListHeader(originalHeader);
                var test = repo.GetCustomListHeader(headerId);

                // assert
                test.ModifiedUtc
                    .Should()
                    .NotBe(expected);
            }

            [Fact]
            public void EmptyUserId_SavesWithBadUserId() {
                // arrange
                var expected = Guid.Empty;
                var repo = MakeRepo();
                var test = new CustomListHeader() {
                    UserId = Guid.Empty,
                    Active = true,
                    BranchId = "FDF",
                    CustomerNumber = "100234",
                    Name = "Fake Custom List",
                    CreatedUtc = new DateTime(2017, 6, 23, 16, 13, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 6, 23, 16, 14, 0, DateTimeKind.Utc)
                };

                // act
                var headerId = repo.SaveCustomListHeader(test);
                var results = repo.GetCustomListHeader(headerId);

                // assert
                results.UserId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void NullUserId_SavesCorrectly() {
                // arrange
                var repo = MakeRepo();
                var test = new CustomListHeader() {
                    Active = true,
                    BranchId = "FDF",
                    CustomerNumber = "100234",
                    Name = "Fake Custom List",
                    CreatedUtc = new DateTime(2017, 6, 23, 16, 13, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 6, 23, 16, 14, 0, DateTimeKind.Utc)
                };

                // act
                var headerId = repo.SaveCustomListHeader(test);
                var results = repo.GetCustomListHeader(headerId);

                // assert
                results.UserId
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void NullBranchId_ThrowsSqlException() {
                // arrange
                var repo = MakeRepo();
                var test = new CustomListHeader() {
                    UserId = new Guid("0dd15d06-7476-4ec9-9d2c-a78da51fc8bf"),
                    Active = true,
                    CustomerNumber = "100234",
                    Name = "Fake Custom List",
                    CreatedUtc = new DateTime(2017, 6, 23, 16, 13, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 6, 23, 16, 14, 0, DateTimeKind.Utc)
                };

                // act
                Action act = () => repo.SaveCustomListHeader(test);
            
                // assert
                act.ShouldThrow<SqlException>();
            }

            [Fact]
            public void TooLongBranchId_TruncatesExcessiveData() {
                // arrange
                var expected = "123";
                var repo = MakeRepo();
                var test = new CustomListHeader() {
                    UserId = new Guid("0dd15d06-7476-4ec9-9d2c-a78da51fc8bf"),
                    BranchId = "1234",
                    Active = true,
                    CustomerNumber = "100234",
                    Name = "Fake Custom List",
                    CreatedUtc = new DateTime(2017, 6, 23, 16, 13, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 6, 23, 16, 14, 0, DateTimeKind.Utc)
                };

                // act
                var headerId = repo.SaveCustomListHeader(test);
                var results = repo.GetCustomListHeader(headerId);

                // assert
                results.BranchId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void ShortBranchId_ShouldPadRightWithSpaces() {
                // arrange
                var expected = "1  ";
                var repo = MakeRepo();
                var test = new CustomListHeader() {
                                                      UserId = new Guid("0dd15d06-7476-4ec9-9d2c-a78da51fc8bf"),
                                                      BranchId = "1",
                                                      Active = true,
                                                      CustomerNumber = "100234",
                                                      Name = "Fake Custom List",
                                                      CreatedUtc = new DateTime(2017, 6, 23, 16, 13, 0, DateTimeKind.Utc),
                                                      ModifiedUtc = new DateTime(2017, 6, 23, 16, 14, 0, DateTimeKind.Utc)
                                                  };

                // act
                var headerId = repo.SaveCustomListHeader(test);
                var results = repo.GetCustomListHeader(headerId);

                // assert
                results.BranchId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void NullCustomerNumber_ThrowsSqlException() {
                // arrange
                var repo = MakeRepo();
                var test = new CustomListHeader() {
                                                      UserId = new Guid("0dd15d06-7476-4ec9-9d2c-a78da51fc8bf"),
                                                      Active = true,
                                                      BranchId = "FDF",
                                                      Name = "Fake Custom List",
                                                      CreatedUtc = new DateTime(2017, 6, 23, 16, 13, 0, DateTimeKind.Utc),
                                                      ModifiedUtc = new DateTime(2017, 6, 23, 16, 14, 0, DateTimeKind.Utc)
                                                  };

                // act
                Action act = () => repo.SaveCustomListHeader(test);

                // assert
                act.ShouldThrow<SqlException>();
            }

            [Fact]
            public void TooLongCustomerNumber_TruncatesExcessiveData() {
                // arrange
                var expected = "100234";
                var repo = MakeRepo();
                var test = new CustomListHeader() {
                                                      UserId = new Guid("0dd15d06-7476-4ec9-9d2c-a78da51fc8bf"),
                                                      BranchId = "FDF",
                                                      Active = true,
                                                      CustomerNumber = "1002345",
                                                      Name = "Fake Custom List",
                                                      CreatedUtc = new DateTime(2017, 6, 23, 16, 13, 0, DateTimeKind.Utc),
                                                      ModifiedUtc = new DateTime(2017, 6, 23, 16, 14, 0, DateTimeKind.Utc)
                                                  };

                // act
                var headerId = repo.SaveCustomListHeader(test);
                var results = repo.GetCustomListHeader(headerId);

                // assert
                results.CustomerNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void ShortCustomerNumber_ShouldPadRightWithSpaces() {
                // arrange
                var expected = "1     ";
                var repo = MakeRepo();
                var test = new CustomListHeader() {
                                                      UserId = new Guid("0dd15d06-7476-4ec9-9d2c-a78da51fc8bf"),
                                                      BranchId = "FDF",
                                                      Active = true,
                                                      CustomerNumber = "1",
                                                      Name = "Fake Custom List",
                                                      CreatedUtc = new DateTime(2017, 6, 23, 16, 13, 0, DateTimeKind.Utc),
                                                      ModifiedUtc = new DateTime(2017, 6, 23, 16, 14, 0, DateTimeKind.Utc)
                                                  };

                // act
                var headerId = repo.SaveCustomListHeader(test);
                var results = repo.GetCustomListHeader(headerId);

                // assert
                results.CustomerNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void NullName_ThrowsException() {
                // arrange
                var repo = MakeRepo();
                var test = new CustomListHeader() {
                    UserId = new Guid("0dd15d06-7476-4ec9-9d2c-a78da51fc8bf"),
                    Active = true,
                    BranchId = "FDF",
                    CustomerNumber = "100234",
                    CreatedUtc = new DateTime(2017, 6, 23, 16, 13, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 6, 23, 16, 14, 0, DateTimeKind.Utc)
                };

                // act
                Action act = () => repo.SaveCustomListHeader(test);
                
                // assert
                act.ShouldThrow<SqlException>();
            }

            [Fact]
            public void TooLongName_TruncatesName() {
                // arrange
                var expected = "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890";
                var repo = MakeRepo();
                var test = new CustomListHeader() {
                    UserId = new Guid("0dd15d06-7476-4ec9-9d2c-a78da51fc8bf"),
                    Active = true,
                    BranchId = "FDF",
                    CustomerNumber = "100234",
                    Name = "12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901",
                    CreatedUtc = new DateTime(2017, 6, 23, 16, 13, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 6, 23, 16, 14, 0, DateTimeKind.Utc)
                };

                // act
                var headerId = repo.SaveCustomListHeader(test);
                var results = repo.GetCustomListHeader(headerId);

                // assert
                results.Name
                       .Should()
                       .Be(expected);
            }
        }
    }
}
